// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Hosting
{
    public static class WebHostBuilderExtensions
    {
        private static readonly string ServerUrlsSeparator = ";";

        /// <summary>
        /// Use the default hosting configuration settings on the web host.
        /// </summary>
        /// <param name="hostBuilder">The <see cref="IWebHostBuilder"/> to configure.</param>
        /// <returns>The <see cref="IWebHostBuilder"/>.</returns>
        public static IWebHostBuilder UseDefaultHostingConfiguration(this IWebHostBuilder hostBuilder)
        {
            return hostBuilder.UseDefaultHostingConfiguration(args: null);
        }

        /// <summary>
        /// Use the default hosting configuration settings on the web host and allow for override by command line arguments.
        /// </summary>
        /// <param name="hostBuilder">The <see cref="IWebHostBuilder"/> to configure.</param>
        /// <param name="args">The command line arguments used to override default settings.</param>
        /// <returns>The <see cref="IWebHostBuilder"/>.</returns>
        public static IWebHostBuilder UseDefaultHostingConfiguration(this IWebHostBuilder hostBuilder, string[] args)
        {
            return hostBuilder.UseConfiguration(WebHostConfiguration.GetDefault(args));
        }

        /// <summary>
        /// Use the given configuration settings on the web host.
        /// </summary>
        /// <param name="hostBuilder">The <see cref="IWebHostBuilder"/> to configure.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/> containing settings to be used.</param>
        /// <returns>The <see cref="IWebHostBuilder"/>.</returns>
        public static IWebHostBuilder UseConfiguration(this IWebHostBuilder hostBuilder, IConfiguration configuration)
        {
            foreach (var setting in configuration.AsEnumerable())
            {
                hostBuilder.UseSetting(setting.Key, setting.Value);
            }

            return hostBuilder;
        }

        /// <summary>
        /// Set whether startup errors should be captured in the configuration settings of the web host.
        /// When enabled, startup exceptions will be caught and an error page will be returned. If disabled, startup exceptions will be propagated.
        /// </summary>
        /// <param name="hostBuilder">The <see cref="IWebHostBuilder"/> to configure.</param>
        /// <param name="captureStartupErrors"><c>true</c> to use startup error page; otherwise <c>false</c>.</param>
        /// <returns>The <see cref="IWebHostBuilder"/>.</returns>
        public static IWebHostBuilder CaptureStartupErrors(this IWebHostBuilder hostBuilder, bool captureStartupErrors)
        {
            return hostBuilder.UseSetting(WebHostDefaults.CaptureStartupErrorsKey, captureStartupErrors ? "true" : "false");
        }
        
        /// <summary>
        /// Specify the startup method to be used to configure the web application.
        /// </summary>
        /// <param name="hostBuilder">The <see cref="IWebHostBuilder"/> to configure.</param>
        /// <param name="configureApp">The delegate that configures the <see cref="IApplicationBuilder"/>.</param>
        /// <returns>The <see cref="IWebHostBuilder"/>.</returns>
        public static IWebHostBuilder Configure(this IWebHostBuilder hostBuilder, Action<IApplicationBuilder> configureApp)
        {
            if (configureApp == null)
            {
                throw new ArgumentNullException(nameof(configureApp));
            }
            
            var startupAssemblyName = configureApp.GetMethodInfo().DeclaringType.GetTypeInfo().Assembly.GetName().Name;
            
            return hostBuilder.UseSetting(WebHostDefaults.ApplicationKey, startupAssemblyName)
                              .ConfigureServices(services =>
                              {
                                  services.AddSingleton<IStartup>(new DelegateStartup(configureApp));
                              });
        }
        
        
        /// <summary>
        /// Specify the startup type to be used by the web host. 
        /// </summary>
        /// <param name="hostBuilder">The <see cref="IWebHostBuilder"/> to configure.</param>
        /// <param name="startupType">The <see cref="Type"/> to be used.</param>
        /// <returns>The <see cref="IWebHostBuilder"/>.</returns>
        public static IWebHostBuilder UseStartup(this IWebHostBuilder hostBuilder, Type startupType)
        {
            var startupAssemblyName = startupType.GetTypeInfo().Assembly.GetName().Name;
            
            return hostBuilder.UseSetting(WebHostDefaults.ApplicationKey, startupAssemblyName)
                              .ConfigureServices(services =>
                              {
                                  if (typeof(IStartup).GetTypeInfo().IsAssignableFrom(startupType.GetTypeInfo()))
                                  {
                                      services.AddSingleton(typeof(IStartup), startupType);
                                  }
                                  else
                                  {
                                      services.AddSingleton(typeof(IStartup), sp => 
                                      {
                                          var hostingEnvironment = sp.GetRequiredService<IHostingEnvironment>();
                                          return new ConventionBasedStartup(StartupLoader.LoadMethods(sp, startupType, hostingEnvironment.EnvironmentName));
                                      });
                                  }
                              });
        }
        
        /// <summary>
        /// Specify the startup type to be used by the web host.
        /// </summary>
        /// <param name="hostBuilder">The <see cref="IWebHostBuilder"/> to configure.</param>
        /// <typeparam name ="TStartup">The type containing the startup methods for the application.</typeparam>
        /// <returns>The <see cref="IWebHostBuilder"/>.</returns>
        public static IWebHostBuilder UseStartup<TStartup>(this IWebHostBuilder hostBuilder) where TStartup : class
        {
            return hostBuilder.UseStartup(typeof(TStartup));
        }

        /// <summary>
        /// Specify the assembly containing the startup type to be used by the web host.
        /// </summary>
        /// <param name="hostBuilder">The <see cref="IWebHostBuilder"/> to configure.</param>
        /// <param name="startupAssemblyName">The name of the assembly containing the startup type.</param>
        /// <returns>The <see cref="IWebHostBuilder"/>.</returns>
        public static IWebHostBuilder UseStartup(this IWebHostBuilder hostBuilder, string startupAssemblyName)
        {
            if (startupAssemblyName == null)
            {
                throw new ArgumentNullException(nameof(startupAssemblyName));
            }

                
            return hostBuilder
                    .UseSetting(WebHostDefaults.ApplicationKey, startupAssemblyName)
                    .UseSetting(WebHostDefaults.StartupAssemblyKey, startupAssemblyName);
        }

        /// <summary>
        /// Specify the assembly containing the server to be used by the web host.
        /// </summary>
        /// <param name="hostBuilder">The <see cref="IWebHostBuilder"/> to configure.</param>
        /// <param name="assemblyName">The name of the assembly containing the server to be used.</param>
        /// <returns>The <see cref="IWebHostBuilder"/>.</returns>
        public static IWebHostBuilder UseServer(this IWebHostBuilder hostBuilder, string assemblyName)
        {
            if (assemblyName == null)
            {
                throw new ArgumentNullException(nameof(assemblyName));
            }

            return hostBuilder.UseSetting(WebHostDefaults.ServerKey, assemblyName);
        }

        /// <summary>
        /// Specify the server to be used by the web host.
        /// </summary>
        /// <param name="hostBuilder">The <see cref="IWebHostBuilder"/> to configure.</param>
        /// <param name="server">The <see cref="IServer"/> to be used.</param>
        /// <returns>The <see cref="IWebHostBuilder"/>.</returns>
        public static IWebHostBuilder UseServer(this IWebHostBuilder hostBuilder, IServer server)
        {
            if (server == null)
            {
                throw new ArgumentNullException(nameof(server));
            }

            return hostBuilder.ConfigureServices(services =>
            {
                // It would be nicer if this was transient but we need to pass in the
                // factory instance directly
                services.AddSingleton(server);
            });
        }

        /// <summary>
        /// Specify the environment to be used by the web host.
        /// </summary>
        /// <param name="hostBuilder">The <see cref="IWebHostBuilder"/> to configure.</param>
        /// <param name="environment">The environment to host the application in.</param>
        /// <returns>The <see cref="IWebHostBuilder"/>.</returns>
        public static IWebHostBuilder UseEnvironment(this IWebHostBuilder hostBuilder, string environment)
        {
            if (environment == null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            return hostBuilder.UseSetting(WebHostDefaults.EnvironmentKey, environment);
        }

        /// <summary>
        /// Specify the content root directory to be used by the web host.
        /// </summary>
        /// <param name="hostBuilder">The <see cref="IWebHostBuilder"/> to configure.</param>
        /// <param name="contentRoot">Path to root directory of the application.</param>
        /// <returns>The <see cref="IWebHostBuilder"/>.</returns>
        public static IWebHostBuilder UseContentRoot(this IWebHostBuilder hostBuilder, string contentRoot)
        {
            if (contentRoot == null)
            {
                throw new ArgumentNullException(nameof(contentRoot));
            }

            return hostBuilder.UseSetting(WebHostDefaults.ContentRootKey, contentRoot);
        }

        /// <summary>
        /// Specify the webroot directory to be used by the web host.
        /// </summary>
        /// <param name="hostBuilder">The <see cref="IWebHostBuilder"/> to configure.</param>
        /// <param name="webRoot">Path to the root directory used by the web server.</param>
        /// <returns>The <see cref="IWebHostBuilder"/>.</returns>
        public static IWebHostBuilder UseWebRoot(this IWebHostBuilder hostBuilder, string webRoot)
        {
            if (webRoot == null)
            {
                throw new ArgumentNullException(nameof(webRoot));
            }

            return hostBuilder.UseSetting(WebHostDefaults.WebRootKey, webRoot);
        }

        /// <summary>
        /// Specify the urls the web host will listen on.
        /// </summary>
        /// <param name="hostBuilder">The <see cref="IWebHostBuilder"/> to configure.</param>
        /// <param name="urls">The urls the hosted application will listen on.</param>
        /// <returns>The <see cref="IWebHostBuilder"/>.</returns>
        public static IWebHostBuilder UseUrls(this IWebHostBuilder hostBuilder, params string[] urls)
        {
            if (urls == null)
            {
                throw new ArgumentNullException(nameof(urls));
            }

            return hostBuilder.UseSetting(WebHostDefaults.ServerUrlsKey, string.Join(ServerUrlsSeparator, urls));
        }

        /// <summary>
        /// Start the web host and listen on the speficied urls.
        /// </summary>
        /// <param name="hostBuilder">The <see cref="IWebHostBuilder"/> to start.</param>
        /// <param name="urls">The urls the hosted application will listen on.</param>
        /// <returns>The <see cref="IWebHostBuilder"/>.</returns>
        public static IWebHost Start(this IWebHostBuilder hostBuilder, params string[] urls)
        {
            var host = hostBuilder.UseUrls(urls).Build();
            host.Start();
            return host;
        }
    }
}
