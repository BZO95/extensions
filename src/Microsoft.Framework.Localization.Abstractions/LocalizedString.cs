﻿// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

namespace Microsoft.Framework.Localization
{
    /// <summary>
    /// A locale specific string.
    /// </summary>
    public struct LocalizedString
    {
        /// <summary>
        /// Creates a new <see cref="LocalizedString"/>.
        /// </summary>
        /// <param name="name">The name of the string in the resource it was loaded from.</param>
        /// <param name="value">The actual string.</param>
        public LocalizedString(string name, string value)
            : this(name, value, resourceNotFound: false)
        {

        }

        /// <summary>
        /// Creates a new <see cref="LocalizedString"/>.
        /// </summary>
        /// <param name="name">The name of the string in the resource it was loaded from.</param>
        /// <param name="value">The actual string.</param>
        /// <param name="resourceNotFound">Whether the string was found in a resource. Set this to <c>false</c> to indicate an alternate string value was used.</param>
        public LocalizedString(string name, string value, bool resourceNotFound)
        {
            Name = name;
            Value = value;
            ResourceNotFound = resourceNotFound;
        }

        public static implicit operator string (LocalizedString localizedString)
        {
            return localizedString.Value;
        }

        /// <summary>
        /// The name of the string in the resource it was loaded from.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The actual string.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Whether the string was found in a resource. If <c>false</c>, an alternate string value was used.
        /// </summary>
        public bool ResourceNotFound { get; }

        /// <summary>
        /// Returns the actual string.
        /// </summary>
        /// <returns>The actual string.</returns>
        public override string ToString() => Value;
    }
}