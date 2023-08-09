﻿/*
 * Copyright 2020 James Courtney
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

namespace FlatSharp.TypeModel;

/// <summary>
/// A <see cref="TypeModelContainer"/> describes how FlatSharp resolves types. Each container contains 
/// <see cref="ITypeModelProvider"/> instances, which are capable of resolving a CLR Type
/// into an <see cref="ITypeModel"/> instance. A <see cref="ITypeModel"/> describes formatting
/// rules and implements C# methods for serializing, parsing, and computing the max size
/// of the type that it describes.
/// 
/// Type Models are resolved by the container in the order by which their providers were registered.
/// </summary>
public sealed class TypeModelContainer
{
    private readonly object SyncRoot = new();
    private readonly List<ITypeModelProvider> providers = new();
    private readonly Queue<ITypeModel> validationQueue = new();

    // Tracks the recursion depth in TryCreateTypeModel.
    private Dictionary<Type, ITypeModel> cache = new();
    private int recursiveTypeModelDepth;

    private TypeModelContainer()
    {
    }

    /// <summary>
    /// Creates a FlatSharp type model container with default support.
    /// </summary>
    public static TypeModelContainer CreateDefault()
    {
        var container = new TypeModelContainer();
        container.RegisterProvider(new ScalarTypeModelProvider());
        container.RegisterProvider(new FlatSharpTypeModelProvider());
        return container;
    }

    /// <summary>
    /// Registers a custom type model provider. Custom providers can be thought of
    /// as plugins and used to extend FlatSharp or alter properties of the 
    /// serialization system. Custom providers are a very advanced feature and 
    /// shouldn't be used without extensive testing and knowledge of FlatBuffers.
    /// 
    /// Use of this API almost certainly means that the binary format of FlatSharp
    /// will no longer be compatible with the official FlatBuffers library.
    /// 
    /// ITypeModelProvider instances are evaluated in registration order.
    /// </summary>
    public void RegisterProvider(ITypeModelProvider provider)
    {
        FlatSharpInternal.Assert(provider is not null, "Provider can't be null");
        this.providers.Add(provider);
    }

    /// <summary>
    /// Attempts to look up a type model for the given type, if it exists.
    /// </summary>
    public bool TryGetTypeModel(
        Type type,
        [NotNullWhen(true)] out ITypeModel? typeModel) => this.cache.TryGetValue(type, out typeModel);

    /// <summary>
    /// Attempts to resolve a type model from the given type.
    /// </summary>
    public bool TryCreateTypeModel(
        Type type,
        [NotNullWhen(true)] out ITypeModel? typeModel) => this.TryCreateTypeModel(type, true, out typeModel);

    /// <summary>
    /// Attempts to resolve a type model from the given type.
    /// </summary>
    public bool TryCreateTypeModel(
        Type type,
        bool throwOnError,
        [NotNullWhen(true)] out ITypeModel? typeModel)
    {
        lock (this.SyncRoot)
        {
            // Try to retrieve from cache.
            if (this.cache.TryGetValue(type, out typeModel))
            {
                return true;
            }

            // Not in cache -- we are creating something new. If we are the
            // first item on the stack, make a copy of the current cache
            // and store in 'oldCache'. This is in case we need to roll back
            // the change in case of an error in the new type model we are
            // going to create.
            Dictionary<Type, ITypeModel>? oldCache = null;

            if (++this.recursiveTypeModelDepth == 1)
            {
                // We are the first call. Let's clone the existing cache.
                oldCache = this.cache;
                this.cache = new Dictionary<Type, ITypeModel>(oldCache);
                this.validationQueue.Clear();
            }

            bool success = false;

            try
            {
                if (this.TryCreateTypeModelImpl(type, throwOnError, out typeModel))
                {
                    success = true;
                    this.validationQueue.Enqueue(typeModel);
                }
            }
            catch
            {
                success = false;
                if (throwOnError)
                {
                    throw;
                }
            }
            finally
            {
                // Decrement stack depth. If we are removing the last stack frame, then do validation.
                --this.recursiveTypeModelDepth;
                if (oldCache is not null)
                {
                    FlatSharpInternal.Assert(this.recursiveTypeModelDepth == 0, "Expecting 0 depth");

                    try
                    {
                        this.ProcessValidationQueue();
                    }
                    catch
                    {
                        success = false;

                        if (throwOnError)
                        {
                            throw;
                        }
                    }
                    finally
                    {
                        this.validationQueue.Clear();
                        if (!success)
                        {
                            // In case of a validation error, roll back changes.
                            this.cache = oldCache;
                        }
                    }
                }
            }

            return success;
        }
    }

    internal IEnumerable<ITypeModel> GetEnumerator()
    {
        return this.cache.Values;
    }

    /// <summary>
    /// Processes the queue of pending validations, retrying until progress is no longer being made.
    /// This approach can accomodate most circular dependencies, where items depend upon each other.
    /// </summary>
    private void ProcessValidationQueue()
    {
        bool progress = true;

        // Keep going until we no longer make progress.
        while (progress)
        {
            progress = false;
            int toProcess = this.validationQueue.Count;

            while (toProcess-- > 0)
            {
                ITypeModel toValidate = this.validationQueue.Dequeue();

                try
                {
                    toValidate.Validate();
                    progress = true;
                }
                catch (InvalidFlatBufferDefinitionException)
                {
                    // If we failed to validate, it might be legitimate, or it might be that
                    // we just don't have enough context yet.
                    this.validationQueue.Enqueue(toValidate);
                }
            }
        }

        if (validationQueue.Count > 0)
        {
            foreach (var item in this.validationQueue)
            {
                item.Validate();
            }
        }
    }

    private bool TryCreateTypeModelImpl(
        Type type,
        bool throwOnError,
        [NotNullWhen(true)] out ITypeModel? typeModel)
    {
        typeModel = null;

        foreach (var provider in this.providers)
        {
            if (provider.TryCreateTypeModel(this, type, out typeModel))
            {
                break;
            }
        }

        if (typeModel is not null)
        {
            this.cache[type] = typeModel;

            try
            {
                typeModel.Initialize();
                return true;
            }
            catch
            {
                this.cache.Remove(type);

                if (throwOnError)
                {
                    throw;
                }
                else
                {
                    return false;
                }
            }
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Creates the a type model for the given type or throws an exception.
    /// </summary>
    public ITypeModel CreateTypeModel(Type type)
    {
        FlatSharpInternal.Assert(
            this.TryCreateTypeModel(type, out var typeModel),
            $"Failed to create or find type model for type '{CSharpHelpers.GetCompilableTypeName(type)}'.");

        return typeModel;
    }
}
