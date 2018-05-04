// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.DependencyModel.Resolution;
using Xunit;

namespace Microsoft.AspNetCore.Analyzer.Testing
{
    /// <summary>
    /// Base type for executing a <see cref="DiagnosticAnalyzer" />. Derived types implemented in the test assembly will
    /// correctly resolve reference assemblies required for compilaiton.
    /// </summary>
    public abstract class DiagnosticAnalyzerRunner
    {
        /// <summary>
        /// File name prefix used to generate Documents instances from source.
        /// </summary>
        public static string DefaultFilePathPrefix = "Test";

        /// <summary>
        /// Project name.
        /// </summary>
        public static string TestProjectName = "TestProject";

        /// <summary>
        /// Given classes in the form of strings, and an DiagnosticAnalyzer to apply to it, return the diagnostics found in the string after converting it to a document.
        /// </summary>
        /// <param name="sources">Classes in the form of strings</param>
        /// <param name="analyzer">The analyzer to be run on the sources</param>
        /// <param name="additionalEnabledDiagnostics">Additional diagnostics to enable at Info level</param>
        /// <param name="getAllDiagnostics">
        /// When <c>true</c>, returns all diagnostics including compilation errors.
        /// Otherwise; only returns analyzer diagnostics.
        /// </param>
        /// <returns>An IEnumerable of Diagnostics that surfaced in the source code, sorted by Location</returns>
        protected Task<Diagnostic[]> GetDiagnosticsAsync(
            string[] sources,
            DiagnosticAnalyzer analyzer,
            string[] additionalEnabledDiagnostics,
            bool getAllDiagnostics = true)
        {
            return GetDiagnosticsAsync(GetDocuments(sources), analyzer, additionalEnabledDiagnostics);
        }

        /// <summary>
        /// Given an analyzer and a document to apply it to, run the analyzer and gather an array of diagnostics found in it.
        /// The returned diagnostics are then ordered by location in the source document.
        /// </summary>
        /// <param name="documents">The Documents that the analyzer will be run on</param>
        /// <param name="analyzer">The analyzer to run on the documents</param>
        /// <param name="additionalEnabledDiagnostics">Additional diagnostics to enable at Info level</param>
        /// <param name="getAllDiagnostics">
        /// When <c>true</c>, returns all diagnostics including compilation errors.
        /// Otherwise only returns analyzer diagnostics.
        /// </param>
        /// <returns>An IEnumerable of Diagnostics that surfaced in the source code, sorted by Location</returns>
        protected async Task<Diagnostic[]> GetDiagnosticsAsync(
            Document[] documents,
            DiagnosticAnalyzer analyzer,
            string[] additionalEnabledDiagnostics,
            bool getAllDiagnostics = true)
        {
            var projects = new HashSet<Project>();
            foreach (var document in documents)
            {
                projects.Add(document.Project);
            }

            var diagnostics = new List<Diagnostic>();
            foreach (var project in projects)
            {
                var compilation = await project.GetCompilationAsync();

                // Enable any additional diagnostics
                var options = ConfigureCompilationOptions(compilation.Options);
                if (additionalEnabledDiagnostics.Length > 0)
                {
                    options = compilation.Options
                        .WithSpecificDiagnosticOptions(
                            additionalEnabledDiagnostics.ToDictionary(s => s, s => ReportDiagnostic.Info));
                }

                var compilationWithAnalyzers = compilation
                    .WithOptions(options)
                    .WithAnalyzers(ImmutableArray.Create(analyzer));

                if (getAllDiagnostics)
                {
                    var diags = await compilationWithAnalyzers.GetAllDiagnosticsAsync();

                    Assert.DoesNotContain(diags, d => d.Id == "AD0001");

                    // Filter out non-error diagnostics not produced by our analyzer
                    // We want to KEEP errors because we might have written bad code. But sometimes we leave warnings in to make the
                    // test code more convenient
                    diags = diags.Where(d => d.Severity == DiagnosticSeverity.Error || analyzer.SupportedDiagnostics.Any(s => s.Id.Equals(d.Id))).ToImmutableArray();

                    foreach (var diag in diags)
                    {
                        if (diag.Location == Location.None || diag.Location.IsInMetadata)
                        {
                            diagnostics.Add(diag);
                        }
                        else
                        {
                            foreach (var document in documents)
                            {
                                var tree = await document.GetSyntaxTreeAsync();
                                if (tree == diag.Location.SourceTree)
                                {
                                    diagnostics.Add(diag);
                                }
                            }
                        }
                    }
                }
                else
                {
                    diagnostics.AddRange(await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync());
                }
            }

            return diagnostics.OrderBy(d => d.Location.SourceSpan.Start).ToArray();
        }

        protected virtual CodeAnalysis.CompilationOptions ConfigureCompilationOptions(CodeAnalysis.CompilationOptions options)
        {
            return options.WithOutputKind(OutputKind.DynamicallyLinkedLibrary);
        }

        private Document[] GetDocuments(string[] sources)
        {
            var project = CreateProject(sources);
            var documents = project.Documents.ToArray();

            Debug.Assert(sources.Length == documents.Length);

            return documents;
        }

        private Project CreateProject(string[] sources)
        {
            var fileNamePrefix = DefaultFilePathPrefix;

            var projectId = ProjectId.CreateNewId(debugName: TestProjectName);

            var solution = new AdhocWorkspace()
                .CurrentSolution
                .AddProject(projectId, TestProjectName, TestProjectName, LanguageNames.CSharp);

            foreach (var defaultCompileLibrary in DependencyContext.Load(GetType().Assembly).CompileLibraries)
            {
                foreach (var resolveReferencePath in defaultCompileLibrary.ResolveReferencePaths(new AppLocalResolver()))
                {
                    solution = solution.AddMetadataReference(projectId, MetadataReference.CreateFromFile(resolveReferencePath));
                }
            }

            for (var i = 0; i < sources.Length; i++)
            {
                var newFileName = fileNamePrefix;
                if (sources.Length > 1)
                {
                    newFileName += i;
                }
                newFileName += ".cs";

                var documentId = DocumentId.CreateNewId(projectId, debugName: newFileName);
                solution = solution.AddDocument(documentId, newFileName, SourceText.From(sources[i]));
            }

            return solution.GetProject(projectId);
        }

        // Required to resolve compilation assemblies inside unit tests
        private class AppLocalResolver : ICompilationAssemblyResolver
        {
            public bool TryResolveAssemblyPaths(CompilationLibrary library, List<string> assemblies)
            {
                foreach (var assembly in library.Assemblies)
                {
                    var dll = Path.Combine(Directory.GetCurrentDirectory(), "refs", Path.GetFileName(assembly));
                    if (File.Exists(dll))
                    {
                        assemblies.Add(dll);
                        return true;
                    }

                    dll = Path.Combine(Directory.GetCurrentDirectory(), Path.GetFileName(assembly));
                    if (File.Exists(dll))
                    {
                        assemblies.Add(dll);
                        return true;
                    }
                }

                return false;
            }
        }
    }
}
