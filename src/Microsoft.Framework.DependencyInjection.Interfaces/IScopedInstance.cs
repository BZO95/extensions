// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.Framework.DependencyInjection
{
#if ASPNET50 || ASPNETCORE50
    [Microsoft.Framework.Runtime.AssemblyNeutral]
#endif
    public interface IScopedInstance<TValue>
    {
        TValue Value { get; set; }
    }
}