using System;
using System.Collections.Generic;

using Taron.Model;

namespace Taron.Translator
{
    /// <summary>
    /// Represents a translator capable of translating types via reflection
    /// </summary>
    public sealed class ReflectionTranslator : IModelTranslator
    {
        private TranslateOptions translateOpts;

        /// <summary>
        /// Initialises a new instance of the ReflectionTranslator class
        /// </summary>
        /// <param name="opts"></param>
        public ReflectionTranslator(TranslateOptions opts)
        {
            translateOpts = opts;
        }

        /// <summary>
        /// Gets the capability of this translator to translate the specified type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public TranslateCapability CanTranslate(Type type)
        {
            if (type.IsPrimitive)
                return TranslateCapability.None;
            else
                return TranslateCapability.CanSerialise | TranslateCapability.CanDeserialise;
        }

        /// <summary>
        /// Serialises the specified object into a value node
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public ValueNode Serialise(object obj)
        {
            // TODO: Scan obj's type for all public fields (which aren't readonly) and public properties (with setters)
            // Then, set them, recursing through translateOpts and hence other translators
            return null;
        }

        /// <summary>
        /// Deserialises the specified value node into a .NET object
        /// </summary>
        /// <param name="t"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public object Deserialise(Type t, ValueNode node)
        {
            // TODO: This
            return null;
        }

        /// <summary>
        /// Populates the a .NET object with the specified value node
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="node"></param>
        public void Deserialise(object obj, ValueNode node)
        {
            // TODO: This
        }
    }
}
