using System;
using System.Collections.Generic;

using Taron.Model;

namespace Taron.Translator
{
    /// <summary>
    /// Represents a translator capable of translating primitive types
    /// </summary>
    public class PrimitiveTranslator : IModelTranslator
    {
        private delegate object FromModel(ValueNode node);
        private delegate ValueNode FromObject(object obj);

        private struct ToFro
        {
            public readonly FromModel FromModel;
            public readonly FromObject FromObject;
            public ToFro(FromModel fromModel, FromObject fromObject)
            {
                FromModel = fromModel;
                FromObject = fromObject;
            }
        }

        private IDictionary<Type, ToFro> typeMap;

        internal PrimitiveTranslator()
        {
            typeMap = new Dictionary<Type, ToFro>()
            {
                { typeof(byte), new ToFro(node => (node as PrimitiveValue<int>).Value, obj => new PrimitiveValue<int>((byte)obj)) },
                { typeof(ushort), new ToFro(node => (node as PrimitiveValue<int>).Value, obj => new PrimitiveValue<int>((ushort)obj)) },
                { typeof(short), new ToFro(node => (node as PrimitiveValue<int>).Value, obj => new PrimitiveValue<int>((short)obj)) },
                { typeof(uint), new ToFro(node => (node as PrimitiveValue<int>).Value, obj => new PrimitiveValue<int>((int)(uint)obj)) },
                { typeof(int), new ToFro(node => (node as PrimitiveValue<int>).Value, obj => new PrimitiveValue<int>((int)obj)) },
                { typeof(string), new ToFro(node => (node as PrimitiveValue<string>).Value, obj => new PrimitiveValue<string>((string)obj)) },
                { typeof(bool), new ToFro(node => (node as PrimitiveValue<bool>).Value, obj => new PrimitiveValue<bool>((bool)obj)) },
                { typeof(double), new ToFro(node => (node as PrimitiveValue<double>).Value, obj => new PrimitiveValue<double>((double)obj)) },
                { typeof(float), new ToFro(node => (node as PrimitiveValue<double>).Value, obj => new PrimitiveValue<double>((float)obj)) }
            };
        }

        /// <summary>
        /// Gets the capability of this translator to translate the specified type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public TranslateCapability CanTranslate(Type type)
        {
            return typeMap.ContainsKey(type) ? TranslateCapability.CanSerialise | TranslateCapability.CanDeserialise :
                TranslateCapability.None;
        }

        /// <summary>
        /// Serialises the specified object into a value node
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public ValueNode Serialise(object obj)
        {
            if (obj == null) return null;
            ToFro toFro;
            if (!typeMap.TryGetValue(obj.GetType(), out toFro)) return null;
            return toFro.FromObject(obj);
        }

        /// <summary>
        /// Deserialises the specified value node into a .NET object
        /// </summary>
        /// <param name="t"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public object Deserialise(Type t, ValueNode node)
        {
            ToFro toFro;
            if (!typeMap.TryGetValue(t, out toFro)) return null;
            return toFro.FromModel(node);
        }

        /// <summary>
        /// Populates the a .NET object with the specified value node
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="node"></param>
        public void Deserialise(object obj, ValueNode node)
        {
            // We can't populate a value type, this won't work
            throw new InvalidOperationException("Can't populate value type");
        }
    }
}
