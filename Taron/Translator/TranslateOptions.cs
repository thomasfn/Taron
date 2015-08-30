using System;
using System.Collections.Generic;
using System.Linq;

using Taron.Model;

namespace Taron.Translator
{
    /// <summary>
    /// Represents options used to translate
    /// </summary>
    public sealed class TranslateOptions
    {
        private HashSet<IModelTranslator> translators;
        private IDictionary<Type, Type> defaultTypeMap;

        /// <summary>
        /// Gets a set of default translation options
        /// </summary>
        public static TranslateOptions Default
        {
            get
            {
                TranslateOptions opts = new TranslateOptions();
                opts.AddTranslator(new ObjectTranslator());
                opts.AddTranslator(new PrimitiveTranslator());
                opts.AddTranslator(new EnumTranslator());
                opts.AddTranslator(new ReflectionTranslator(opts));
                return opts;
            }
        }

        /// <summary>
        /// Initialises a new instance of the TranslateOptions class
        /// </summary>
        public TranslateOptions()
        {
            // Init
            translators = new HashSet<IModelTranslator>();
            defaultTypeMap = new Dictionary<Type, Type>()
            {
                { typeof(ArrayValue), typeof(List<object>) },
                { typeof(MapValue), typeof(Dictionary<string, object>) }
            };
        }

        /// <summary>
        /// Adds the specified translator
        /// </summary>
        /// <param name="translator"></param>
        public void AddTranslator(IModelTranslator translator)
        {
            translators.Add(translator);
        }

        /// <summary>
        /// Adds a default type mapping
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mapTo"></param>
        public void AddDefaultType<T>(Type mapTo) where T : ValueNode
        {
            defaultTypeMap[typeof(T)] = mapTo;
        }

        /// <summary>
        /// Finds a translator capable of translating the specified type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="matchCap"></param>
        /// <returns></returns>
        internal IModelTranslator FindTranslator(Type type, TranslateCapability matchCap)
        {
            // Note: this does not deal with conflicts
            foreach (IModelTranslator translator in translators)
            {
                if ((translator.CanTranslate(type) & matchCap) == matchCap)
                    return translator;
            }
            return null;
        }

        /// <summary>
        /// Serialises the specified object into a value node
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal bool Serialise(object obj, out ValueNode node)
        {
            if (obj == null)
            {
                node = null;
                return true;
            }
            IModelTranslator translator = FindTranslator(obj.GetType(), TranslateCapability.CanSerialise);
            if (translator == null)
            {
                node = null;
                return false;
            }
            node = translator.Serialise(obj);
            return true;
        }

        /// <summary>
        /// Deserialises the specified value node into a certain type
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal bool Deserialise(Type type, ValueNode node, out object obj)
        {
            // Check the value node
            if (node == null)
            {
                if (type.IsValueType)
                    obj = Activator.CreateInstance(type);
                else
                    obj = null;
                return true;
            }

            // Check for type overrides
            type = GetTypeOverride(type, node.TypeName);

            // Find default
            if (type == null)
            {
                if ((type = GetDefaultType(node)) == null)
                {
                    throw new Exception($"No default type found for {node}");
                }
            }

            // Find translator
            IModelTranslator translator = FindTranslator(type, TranslateCapability.CanDeserialise);
            if (translator == null)
            {
                if (type.IsValueType)
                    obj = Activator.CreateInstance(type);
                else
                    obj = null;
                return false;
            }

            // Deserialise
            obj = translator.Deserialise(type, node);
            return true;
        }

        /// <summary>
        /// Deserialises the specified value node into the specified object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal bool Populate(object obj, ValueNode node)
        {
            // Check the value node
            if (obj == null) return false;
            if (node == null) return true;

            // Check for type overrides
            Type type = obj.GetType();
            type = GetTypeOverride(type, node.TypeName);            

            // Find translator
            IModelTranslator translator = FindTranslator(type, TranslateCapability.CanDeserialise);
            if (translator == null) return false;

            // Deserialise
            translator.Deserialise(obj, node);
            return true;
        }

        /// <summary>
        /// Gets the default type for the specified node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        internal Type GetDefaultType(ValueNode node)
        {
            Type mapOutput;
            if (defaultTypeMap.TryGetValue(node.GetType(), out mapOutput))
            {
                return mapOutput;
            }
            else
            {
                Type t = node.GetType();

                if (t.ContainsGenericParameters)
                {
                    Type[] genArgs = t.GetGenericArguments();
                    if (typeof(PrimitiveValue<>).MakeGenericType(genArgs[0]) == t)
                    {
                        return genArgs[0];
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets an overrided type using the specified typeName
        /// </summary>
        /// <param name="baseType"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        internal Type GetTypeOverride(Type baseType, string typeName)
        {
            if (!string.IsNullOrEmpty(typeName))
            {
                // TODO: Check against a collection of interfaces to see who can handle typeName
                // For now, just find derived types
                Type derivedType = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .Where(t => baseType.IsAssignableFrom(t))
                    .SingleOrDefault(t => t.Name == typeName);
                if (derivedType != null) return derivedType;

                // Nothing found, just return base back
                return baseType;
            }
            else
            {
                return baseType;
            }
        }

        /// <summary>
        /// Gets if type is a specialisation of generic baseType
        /// </summary>
        /// <param name="baseType"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static bool IsGenericType(Type baseType, Type type)
        {
            Type[] genArgs = type.GetGenericArguments();
            if (genArgs.Length == 0) return false;
            Type testType = baseType.MakeGenericType(genArgs);
            return testType.IsAssignableFrom(type);
        }
    }
}
