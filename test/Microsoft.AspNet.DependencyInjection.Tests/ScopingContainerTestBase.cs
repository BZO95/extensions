﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.DependencyInjection.Tests.Fakes;
using Xunit;

namespace Microsoft.AspNet.DependencyInjection.Tests
{
    public abstract class ScopingContainerTestBase : AllContainerTestsBase
    {
        [Fact]
        public void LastServiceReplacesPreviousServices()
        {
            var container = CreateContainer();

            var service = container.GetService<IFakeMultipleService>();

            Assert.Equal("FakeTwoMultipleServiceAnotherMethod", service.SimpleMethod());
        }

        [Fact]
        public void SingletonServiceCanBeResolved()
        {
            var container = CreateContainer();

            var service1 = container.GetService<IFakeSingletonService>();
            var service2 = container.GetService<IFakeSingletonService>();

            Assert.NotNull(service1);
            Assert.Equal(service1, service2);
        }

        [Fact]
        public void ScopedServiceCanBeResolved()
        {
            var container = CreateContainer();

            var scopeFactory = container.GetService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
                var containerScopedService = container.GetService<IFakeScopedService>();
                var scopedService1 = scope.ServiceProvider.GetService<IFakeScopedService>();
                var scopedService2 = scope.ServiceProvider.GetService<IFakeScopedService>();

                Assert.NotEqual(containerScopedService, scopedService1);
                Assert.Equal(scopedService1, scopedService2);
            }
        }

        [Fact]
        public void NestedScopedServiceCanBeResolved()
        {
            var container = CreateContainer();

            var outerScopeFactory = container.GetService<IServiceScopeFactory>();
            using (var outerScope = outerScopeFactory.CreateScope())
            {
                var innerScopeFactory = outerScope.ServiceProvider.GetService<IServiceScopeFactory>();
                using (var innerScope = innerScopeFactory.CreateScope())
                {
                    var outerScopedService = outerScope.ServiceProvider.GetService<IFakeScopedService>();
                    var innerScopedService = innerScope.ServiceProvider.GetService<IFakeScopedService>();

                    Assert.NotEqual(outerScopedService, innerScopedService);
                }
            }
        }

        [Fact]
        public void DisposingScopeDisposesService()
        {
            var container = CreateContainer();
            FakeService disposableService;

            var scopeFactory = container.GetService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
                disposableService = (FakeService)scope.ServiceProvider.GetService<IFakeScopedService>();

                Assert.False(disposableService.Disposed);
            }

            Assert.True(disposableService.Disposed);
        }

        [Fact]
        public void ServicesCanBeResolvedFromFallbackServiceProvider()
        {
            var container = CreateContainer();

            var service = container.GetService<string>();

            Assert.Equal("FakeFallbackServiceProvider", service);
        }

        [Fact]
        public void IEnumerableServicesCanBeResolvedFromFallbackServiceProvider()
        {
            var container = CreateContainer();

            var service = container.GetService<IEnumerable<string>>();

            Assert.Equal(1, service.Count());
            Assert.Equal("FakeFallbackServiceProvider", service.First());
        }

        [Fact]
        public void ServicesFromFallbackServiceProviderCanBeReplaced()
        {
            var container = CreateContainer();

            var service = container.GetService<IFakeFallbackService>();

            Assert.Equal("FakeServiceSimpleMethod", service.SimpleMethod());
        }

        [Fact]
        public void ServicesFromFallbackServiceProviderCanBeReplacedAndIEnumerableResolved()
        {
            var container = CreateContainer();

            var services = container.GetService<IEnumerable<IFakeFallbackService>>();
            var messages = services.Select(service => service.SimpleMethod());

            Assert.Equal(1, services.Count());
            Assert.Contains("FakeServiceSimpleMethod", messages);
        }

        [Fact]
        public void NestedScopedServiceCanBeResolvedFromFallbackProvider()
        {
            var container = CreateContainer();

            var outerScopeFactory = container.GetService<IServiceScopeFactory>();
            using (var outerScope = outerScopeFactory.CreateScope())
            {
                var innerScopeFactory = outerScope.ServiceProvider.GetService<IServiceScopeFactory>();
                using (var innerScope = innerScopeFactory.CreateScope())
                {
                    var outerScopedService = outerScope.ServiceProvider.GetService<string>();
                    var innerScopedService = innerScope.ServiceProvider.GetService<string>();

                    Assert.Equal("scope-FakeFallbackServiceProvider", outerScopedService);
                    Assert.Equal("scope-scope-FakeFallbackServiceProvider", innerScopedService);
                }
            }
        }

        [Fact]
        public void FallbackScopeGetsDisposedAlongWithChainedScope()
        {
            var container = CreateContainer();

            var scopeFactory = container.GetService<IServiceScopeFactory>();
            IServiceProvider fallbackProvider;
            using (var scope = scopeFactory.CreateScope())
            {
                fallbackProvider = scope.ServiceProvider.GetService<IFakeFallbackServiceProvider>();
                var scopedService = fallbackProvider.GetService<string>();

                Assert.Equal("scope-FakeFallbackServiceProvider", scopedService);
            }

            var disposedScopedService = fallbackProvider.GetService<string>();

            Assert.Equal("disposed-FakeFallbackServiceProvider", disposedScopedService);
        }

        [Fact]
        public void OpenGenericServicesCanBeResolved()
        {
            var container = CreateContainer();

            var genericService = container.GetService<IFakeOpenGenericService<IFakeSingletonService>>();
            var singletonService = container.GetService<IFakeSingletonService>();

            Assert.Equal(singletonService, genericService.SimpleMethod());
        }

        [Fact]
        public void ClosedServicesPreferredOverOpenGenericServices()
        {
            var container = CreateContainer();

            var service = container.GetService<IFakeOpenGenericService<string>>();

            Assert.Equal("FakeServiceSimpleMethod", service.SimpleMethod());
        }
    }
}
