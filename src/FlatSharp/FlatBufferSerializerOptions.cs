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
 
 namespace FlatSharp
{
    /// <summary>
    /// Defines various confiration settings for serializing and deserializing buffers.
    /// </summary>
    public class FlatBufferSerializerOptions
    {
        /// <summary>
        /// Indicates if list vectors should have their data cached after reading. This option will cause more allocations
        /// on deserializing, but will improve performance in cases of duplicate accesses to the same indices.
        /// </summary>
        public bool CacheListVectorData { get; set; } = false;
        
        /// <summary>
        /// Test hook instructing the deserializer to generate subclasses that implement <see cref="IDeserializedObject"/>.
        /// </summary>
        internal bool ImplementIDeserializedObject { get; set; } = false;
    }
}
