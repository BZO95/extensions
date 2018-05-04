// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

namespace Microsoft.AspNetCore.Analyzer.Testing
{
    internal class Assert : Xunit.Assert
    {
        public static void DiagnosticLocation(DiagnosticLocation expected, Location actual)
        {
            var actualSpan = actual.GetLineSpan();
            var actualLinePosition = actualSpan.StartLinePosition;

            // Only check line position if there is an actual line in the real diagnostic
            if (actualLinePosition.Line > 0)
            {
                if (actualLinePosition.Line + 1 != expected.Line)
                {
                    throw new DiagnosticLocationAssertException(
                        expected,
                        actual,
                        $"Expected diagnostic to be on line \"{expected.Line}\" was actually on line \"{actualLinePosition.Line + 1}\"");
                }
            }

            // Only check column position if there is an actual column position in the real diagnostic
            if (actualLinePosition.Character > 0)
            {
                if (actualLinePosition.Character + 1 != expected.Column)
                {
                    throw new DiagnosticLocationAssertException(
                        expected,
                        actual,
                        $"Expected diagnostic to start at column \"{expected.Column}\" was actually on line \"{actualLinePosition.Character + 1}\"");
                }
            }
        }

        private class DiagnosticLocationAssertException : Xunit.Sdk.EqualException
        {
            public DiagnosticLocationAssertException(
                DiagnosticLocation expected,
                Location actual,
                string message)
                : base(expected, actual)
            {
                Message = message;
            }

            public override string Message { get; }
        }
    }
}
