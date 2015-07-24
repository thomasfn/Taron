using System;

using Taron.Model;

namespace Taron.Translator
{
    /// <summary>
    /// Represents how able a translator is to translate a model into a .NET object
    /// </summary>
    [Flags]
    public enum TranslateCapability
    {
        None = 0x00,
        CanSerialise = 0x01,
        CanDeserialise = 0x02
    }

    /// <summary>
    /// Represents an object capable of translating models into .NET objects
    /// </summary>
    public interface IModelTranslator
    {
        /// <summary>
        /// Gets the capability of this translator to translate the specified type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        TranslateCapability CanTranslate(Type type);

        /// <summary>
        /// Serialises the specified object into a value node
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        ValueNode Serialise(object obj);

        /// <summary>
        /// Deserialises the specified value node into a .NET object
        /// </summary>
        /// <param name="t"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        object Deserialise(Type t, ValueNode node);

        /// <summary>
        /// Populates the a .NET object with the specified value node
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="node"></param>
        void Deserialise(object obj, ValueNode node);
    }
}
