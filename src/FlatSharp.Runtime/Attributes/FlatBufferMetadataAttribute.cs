﻿/*
 * Copyright 2021 James Courtney
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

namespace FlatSharp.Attributes;

/// <summary>
/// Describes kinds of metadata.
/// </summary>
public enum FlatBufferMetadataKind
{
    /// <summary>
    /// A custom get/set accessor for a flatbuffer field.
    /// </summary>
    Accessor = 1,

    /// <summary>
    /// A FBS attribute
    /// </summary>
    FbsAttribute = 2,
}

/// <summary>
/// Defines a member of a FlatBuffer struct or table.
/// </summary>
[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
public class FlatBufferMetadataAttribute : Attribute
{
    /// <summary>
    /// Initializes a new FlatBufferMetadataAttribute.
    /// </summary>
    public FlatBufferMetadataAttribute(FlatBufferMetadataKind kind, string key, string? value)
    {
        this.Kind = kind;
        this.Key = key;
        this.Value = value;
    }

    /// <summary>
    /// The key.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// The value.
    /// </summary>
    public string? Value { get; }

    /// <summary>
    /// The kind of metadata.
    /// </summary>
    public FlatBufferMetadataKind Kind { get; }
}
