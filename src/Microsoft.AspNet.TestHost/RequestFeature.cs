// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNet.Http.Features;
using Microsoft.Extensions.Primitives;

namespace Microsoft.AspNet.TestHost
{
    internal class RequestFeature : IHttpRequestFeature
    {
        public RequestFeature()
        {
            Body = Stream.Null;
            Headers = new Dictionary<string, StringValues>(StringComparer.OrdinalIgnoreCase);
            Method = "GET";
            Path = "";
            PathBase = "";
            Protocol = "HTTP/1.1";
            QueryString = "";
            Scheme = "http";
        }

        public Stream Body { get; set; }

        public IDictionary<string, StringValues> Headers { get; set; }

        public string Method { get; set; }

        public string Path { get; set; }

        public string PathBase { get; set; }

        public string Protocol { get; set; }

        public string QueryString { get; set; }

        public string Scheme { get; set; }
    }
}
