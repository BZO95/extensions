// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Framework.Internal;

namespace Microsoft.Dnx.TestAdapter
{
    public interface ITestDiscoverySink
    {
        void SendTest([NotNull] Test test);
    }
}