using System;
using System.Collections.Generic;
using System.Linq;

using Taron.Model;

namespace Taron.Translator
{
    /// <summary>
    /// Represents a translator capable of translating object types
    /// </summary>
    public class ObjectTranslator : IModelTranslator
    {
        internal ObjectTranslator()
        {
        }

        /// <summary>
        /// Gets the capability of this translator to translate the specified type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public TranslateCapability CanTranslate(Type type)
        {
            return type == typeof(object) ? TranslateCapability.CanDeserialise : TranslateCapability.None;
        }

        /// <summary>
        /// Serialises the specified object into a value node
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public ValueNode Serialise(object obj)
        {
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
            Type nt = node.GetType();
            Type[] genArgs = nt.GetGenericArguments();
            if (genArgs?.Length > 0 && typeof(PrimitiveValue<>).MakeGenericType(genArgs) == nt)
            {
                return nt.GetProperty("Value").GetValue(node, null);
            }
            else if (node is EnumValue)
            {
                EnumValue enumvalue = (EnumValue)node;

                Type enumtype;

                if (EnumTranslator.EnumTypes.TryGetValue(String.Join(".", enumvalue.Value, 0, enumvalue.Value.Length - 1), out enumtype))
                    return Translate.Deserialise(enumvalue, enumtype);

                return null;
            }
            else
            {
                return null;
            }
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
