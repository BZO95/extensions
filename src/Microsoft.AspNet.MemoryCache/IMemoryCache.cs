﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Framework.Runtime;

namespace Microsoft.AspNet.MemoryCache
{
    [AssemblyNeutral]
    public interface IMemoryCache : IDisposable
    {
        /// <summary>
        /// Create or overwrite an entry in the cache.
        /// </summary>
        /// <param name="key">A string identifiing the entry. This is case sensitive.</param>
        /// <param name="state">Application state that will be passed to the creation factory.</param>
        /// <param name="create">A factory that will create and configure the entry.</param>
        /// <returns>The object that was created.</returns>
        object Set(string key, object state, Func<ICacheAddContext, object> create);

        /// <summary>
        /// Gets the item associated with this key if present.
        /// </summary>
        /// <param name="key">A string identifying the requested entry.</param>
        /// <param name="value">The located value or null.</param>
        /// <returns>True if the key was found.</returns>
        bool TryGetValue(string key, out object value);

        /// <summary>
        /// Removes the object associated with the given key.
        /// </summary>
        /// <param name="key">A string identifiing the entry. This is case sensitive.</param>
        void Remove(string key);
    }
}