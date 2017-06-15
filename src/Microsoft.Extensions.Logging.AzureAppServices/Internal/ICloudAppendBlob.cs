﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Logging.AzureAppServices.Internal
{
    /// <summary>
    /// Represents an append blob, a type of blob where blocks of data are always committed to the end of the blob.
    /// </summary>
    public interface ICloudAppendBlob
    {
        /// <summary>
        /// Initiates an asynchronous operation to open a stream for writing to the blob.
        /// </summary>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> object of type <see cref="Stream" /> that represents the asynchronous operation.</returns>
        Task<Stream> OpenWriteAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Initiates an asynchronous operation to create an empty append blob.
        /// </summary>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> object that represents the asynchronous operation.</returns>
        Task CreateAsync(CancellationToken cancellationToken);
    }
}