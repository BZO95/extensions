// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using Microsoft.Extensions.DiagnosticAdapter.Infrastructure;
using Microsoft.Extensions.DiagnosticAdapter.Internal;

namespace Microsoft.Extensions.DiagnosticAdapter
{
    public class ProxyDiagnosticSourceMethodAdapter : IDiagnosticSourceMethodAdapter
    {
        private readonly IProxyFactory _factory = new ProxyFactory();

        public Func<object, object, bool> Adapt(MethodInfo method, Type inputType)
        {
#if NETCOREAPP2_0 || NET461
            var proxyMethod = ProxyMethodEmitter.CreateProxyMethod(method, inputType);
            return (listener, data) => proxyMethod(listener, data, _factory);
#elif NETSTANDARD2_0
            throw new PlatformNotSupportedException("This platform does not support creating proxy types and methods.");
#else
#error Target frameworks should be updated
#endif
        }
    }
}
