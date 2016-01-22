// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.EntityFrameworkCore.Tests
{
    public static class TestHelperExtensions
    {
        public static EntityFrameworkServicesBuilder AddProviderServices(this EntityFrameworkServicesBuilder entityServicesBuilder)
        {
            return entityServicesBuilder.AddInMemoryDatabase();
        }

        public static DbContextOptions UseProviderOptions(this DbContextOptions options)
        {
            return options;
        }
    }
}
