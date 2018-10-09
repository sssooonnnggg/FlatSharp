﻿/*
 * Copyright 2018 James Courtney
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace FlatSharp.Attributes
{
    using System;

    /// <summary>
    /// Defines a member of a FlatBuffer struct or table.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class FlatBufferItemAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new FlatBufferItemAttribute.
        /// </summary>
        /// <param name="index">The field index within the struct or table.</param>
        public FlatBufferItemAttribute(ushort index)
        {
            this.Index = index;
        }

        /// <summary>
        /// The index within the struct or table.
        /// </summary>
        public ushort Index { get; }

        /// <summary>
        /// For tables, indicates if this field is deprecated. Deprecated fields are not written or read.
        /// </summary>
        public bool Deprecated { get; set; }
    }
}
