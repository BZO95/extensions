﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.AspNet.Diagnostics.Entity.FunctionalTests.Helpers
{
    public class PlatformHelper
    {
        public static bool IsMono
        {
            get
            {
                return Type.GetType("Mono.Runtime") != null;
            }
        }
    }
}