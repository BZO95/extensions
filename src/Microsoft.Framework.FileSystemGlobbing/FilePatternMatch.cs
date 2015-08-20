﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Framework.Internal;

namespace Microsoft.Framework.FileSystemGlobbing
{
    public struct FilePatternMatch : IEquatable<FilePatternMatch>
    {
        public string Path { get; }
        public string Stem { get; }

        public FilePatternMatch(string path, string stem)
        {
            Path = path;
            Stem = stem;
        }

        public bool Equals(FilePatternMatch other)
        {
            return string.Equals(other.Path, Path, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(other.Stem, Stem, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            return Equals((FilePatternMatch)obj);
        }

        public override int GetHashCode()
        {
            return HashCodeCombiner.Start()
                .Add(Path)
                .Add(Stem)
                .CombinedHash;
        }
    }
}