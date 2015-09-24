// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNet.Builder;

namespace Microsoft.AspNet.Hosting.Fakes
{
    public class StartupCtorThrows
    {
        public StartupCtorThrows()
        {
            throw new Exception("Exception from constructor");
        }

        public void Configure(IApplicationBuilder app)
        {
        }
    }
}