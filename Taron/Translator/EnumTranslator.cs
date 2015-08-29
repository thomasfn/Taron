using System;
using System.Reflection;
using Taron.Model;
using System.Linq;
using System.Collections.Generic;

namespace Taron.Translator
{
    /// <summary>
    /// Represents a translator capable of translating enum
    /// </summary>
    public class EnumTranslator : IModelTranslator
    {
        public static Dictionary<string, Type> EnumTypes = new Dictionary<string, Type>();

        internal EnumTranslator()
        {
        }

        /// <summary>
        /// Gets the capability of this translator to translate the specified type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public TranslateCapability CanTranslate(Type type)
        {
            return type.IsEnum ? TranslateCapability.CanSerialise | TranslateCapability.CanDeserialise: TranslateCapability.None;
        }

        /// <summary>
        /// Serialises the specified object into a value node
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public ValueNode Serialise(object obj)
        {
            if (obj.GetType().IsEnum)
            {
                Enum enumobj = (Enum)obj;

                System.Diagnostics.Debug.WriteLine("yay");
            }
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
            if (t.IsEnum && node is EnumValue)
            {
                var enumvalue = (EnumValue)node;

                Type enumtype;

                if (EnumTypes.TryGetValue(String.Join(".", enumvalue.Value, 0, enumvalue.Value.Length - 1), out enumtype))
                {
                    var result = Enum.Parse(enumtype, enumvalue.Value.Last());
                    if (result != null)
                        return result;
                }
                else
                    throw new InvalidOperationException($"No such TaronEnumAttribute with typename of ({enumvalue.Type})");
            }
            return null;
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
