// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Net;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Diagnostics.Entity.Utilities;
using Microsoft.AspNet.Http;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNet.Diagnostics.Entity
{
    /// <summary>
    /// Processes requests to execute migrations operations. The middleware will listen for requests to the path configured in the supplied options.
    /// </summary>
    public class MigrationsEndPointMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;
        private readonly MigrationsEndPointOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationsEndPointMiddleware"/> class
        /// </summary>
        /// <param name="next">Delegate to execute the next piece of middleware in the request pipeline.</param>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> to resolve services from.</param>
        /// <param name="logger">The <see cref="Logger{T}"/> to write messages to.</param>
        /// <param name="options">The options to control the behavior of the middleware.</param>
        public MigrationsEndPointMiddleware(
            [NotNull] RequestDelegate next, 
            [NotNull] IServiceProvider serviceProvider, 
            [NotNull] ILogger<MigrationsEndPointMiddleware> logger, 
            [NotNull] MigrationsEndPointOptions options)
        {
            Check.NotNull(next, "next");
            Check.NotNull(serviceProvider, "serviceProvider");
            Check.NotNull(logger, "logger");
            Check.NotNull(options, "options");

            _next = next;
            _serviceProvider = serviceProvider;
            _logger = logger;
            _options = options;
        }

        /// <summary>
        /// Process an individual request.
        /// </summary>
        /// <param name="context">The context for the current request.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public virtual async Task Invoke([NotNull] HttpContext context)
        {
            Check.NotNull(context, "context");

            if (context.Request.Path.Equals(_options.Path))
            {
                _logger.LogDebug(Strings.FormatMigrationsEndPointMiddleware_RequestPathMatched(context.Request.Path));

                var db = await GetDbContext(context, _logger);
                if (db != null)
                {
                    try
                    {
                        _logger.LogDebug(Strings.FormatMigrationsEndPointMiddleware_ApplyingMigrations(db.GetType().FullName));

                        db.Database.Migrate();

                        context.Response.StatusCode = (int)HttpStatusCode.NoContent;
                        context.Response.Headers.Add("Pragma", new[] { "no-cache" });
                        context.Response.Headers.Add("Cache-Control", new[] { "no-cache" });

                        _logger.LogDebug(Strings.FormatMigrationsEndPointMiddleware_Applied(db.GetType().FullName));
                    }
                    catch (Exception ex)
                    {
                        var message = Strings.FormatMigrationsEndPointMiddleware_Exception(db.GetType().FullName) + ex.ToString();
                        _logger.LogError(message);
                        throw new InvalidOperationException(message, ex);
                    }
                }
            }
            else
            {
                await _next(context);
            }
        }

        private static async Task<DbContext> GetDbContext(HttpContext context, ILogger logger)
        {
            var form = await context.Request.ReadFormAsync();
            var contextTypeName = form["context"];
            if (string.IsNullOrWhiteSpace(contextTypeName))
            {
                logger.LogError(Strings.MigrationsEndPointMiddleware_NoContextType);
                await WriteErrorToResponse(context.Response, Strings.MigrationsEndPointMiddleware_NoContextType);
                return null;
            }

            var contextType = Type.GetType(contextTypeName);
            if (contextType == null)
            {
                var message = Strings.FormatMigrationsEndPointMiddleware_InvalidContextType(contextTypeName);
                logger.LogError(message);
                await WriteErrorToResponse(context.Response, message);
                return null;
            }

            var db = (DbContext)context.RequestServices.GetService(contextType);
            if (db == null)
            {
                var message = Strings.FormatMigrationsEndPointMiddleware_ContextNotRegistered(contextType.FullName);
                logger.LogError(message);
                await WriteErrorToResponse(context.Response, message);
                return null;
            }

            return db;
        }

        private static async Task WriteErrorToResponse(HttpResponse response, string error)
        {
            response.StatusCode = (int)HttpStatusCode.BadRequest;
            response.Headers.Add("Pragma", new[] { "no-cache" });
            response.Headers.Add("Cache-Control", new[] { "no-cache" });
            response.ContentType = "text/plain";

            // Padding to >512 to ensure IE doesn't hide the message
            // http://stackoverflow.com/questions/16741062/what-rules-does-ie-use-to-determine-whether-to-show-the-entity-body
            await response.WriteAsync(error.PadRight(513));
        }
    }
}
