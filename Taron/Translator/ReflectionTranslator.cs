using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
            else if (type == typeof(string))
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
            // Sanity check
            if (obj == null) return null;

            Type t = obj.GetType();

            // Is it a map type?
            if (typeof(IDictionary).IsAssignableFrom(t))
            {
                // Create a map type
                MapValue mapVal = new MapValue();
                IDictionary dict = obj as IDictionary;
                foreach (var key in dict.Keys)
                {
                    if (key is string)
                    {
                        ValueNode tmp;
                        if (translateOpts.Serialise(dict[key], out tmp))
                            mapVal.Add(key as string, tmp);
                        else
                            mapVal.Add(key as string, null);
                    }
                }
                return mapVal;
            }

            // Is it an array type?
            else if (t.IsArray)
            {
                // Create an array type
                ArrayValue arrVal = new ArrayValue();
                var arr = obj as Array;
                for (int i = 0; i < arr.Length; i++)
                {
                    ValueNode tmp;
                    if (translateOpts.Serialise(arr.GetValue(i), out tmp))
                        arrVal.Add(tmp);
                    else
                        arrVal.Add(null);
                }
                return arrVal;
            }
            else if (typeof(IList).IsAssignableFrom(t))
            {
                // Create an array type
                ArrayValue arrVal = new ArrayValue();
                var list = obj as IList;
                for (int i = 0; i < list.Count; i++)
                {
                    ValueNode tmp;
                    if (translateOpts.Serialise(list[i], out tmp))
                        arrVal.Add(tmp);
                    else
                        arrVal.Add(null);
                }
                return arrVal;
            }
            else if (typeof(ICollection).IsAssignableFrom(t))
            {
                // Create an array type
                ArrayValue arrVal = new ArrayValue();
                var collection = obj as ICollection;
                foreach (object item in collection)
                {
                    ValueNode tmp;
                    if (translateOpts.Serialise(item, out tmp))
                        arrVal.Add(tmp);
                    else
                        arrVal.Add(null);
                }
                return arrVal;
            }

            // Fill a map via reflection
            MapValue mapValue = new MapValue();
            foreach (var field in t.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                ValueNode tmp;
                if (translateOpts.Serialise(field.GetValue(obj), out tmp) && tmp != null)
                    mapValue.Add(field.Name, tmp);
            }
            foreach (var property in t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (property.GetGetMethod() != null)
                {
                    ValueNode tmp;
                    if (translateOpts.Serialise(property.GetValue(obj, null), out tmp) && tmp != null)
                        mapValue.Add(property.Name, tmp);
                }
            }

            return mapValue;
        }

        /// <summary>
        /// Deserialises the specified value node into a .NET object
        /// </summary>
        /// <param name="t"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public object Deserialise(Type t, ValueNode node)
        {
            

            // Instantiate
            if (t.IsAbstract) throw new Exception($"Can't instantiate type '{t}'");
            object obj;
            try
            {
                // Special case for arrays
                if (t.IsArray)
                {
                    obj = Activator.CreateInstance(t, node.As<ArrayValue>().Count);
                }
                else
                {
                    obj = Activator.CreateInstance(t);
                }
            }
            catch (Exception ex)
            {
                // TODO: Handle this better
                throw ex;
            }

            // Populate
            Deserialise(obj, node);

            // Return
            return obj;
        }

        /// <summary>
        /// Populates the a .NET object with the specified value node
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="node"></param>
        public void Deserialise(object obj, ValueNode node)
        {
            // Sanity check
            if (obj == null) return;

            Type t = obj.GetType();

            // Is it a map type?
            if (typeof(IDictionary).IsAssignableFrom(t) && node is MapValue)
            {
                IDictionary dict = obj as IDictionary;
                MapValue mapVal = node.As<MapValue>();

                // Is it a generic dict?
                if (TranslateOptions.IsGenericType(typeof(IDictionary<,>), t))
                {
                    Type[] genArgs = t.GetGenericArguments();
                    if (genArgs[0] != typeof(string)) return; // TODO: Throw error?
                    foreach (var pair in mapVal)
                    {
                        object val;
                        if (translateOpts.Deserialise(genArgs[1], pair.Value, out val))
                            dict[pair.Key] = val;
                    }
                }
                else
                {
                    foreach (var pair in mapVal)
                    {
                        object val;
                        if (translateOpts.Deserialise(null, pair.Value, out val))
                            dict.Add(pair.Key, val);
                    }
                }
            }

            // Is it an array type?
            else if (t.IsArray)
            {
                Array arr = obj as Array;
                ArrayValue arrVal = node.As<ArrayValue>();
                Type elementType = t.GetElementType();
                for (int i = 0; i < Math.Min(arrVal.Count, arr.Length); i++)
                {
                    object val;
                    translateOpts.Deserialise(elementType == typeof(object) ? null : elementType, arrVal[i], out val);
                    arr.SetValue(val, i);
                }
            }
            else if (typeof(IList).IsAssignableFrom(t))
            {
                var list = obj as IList;
                ArrayValue arrVal = node.As<ArrayValue>();
                list.Clear();

                // Is it a generic list?
                if (TranslateOptions.IsGenericType(typeof(IList<>), t))
                {
                    Type elementType = t.GetGenericArguments()[0];
                    for (int i = 0; i < arrVal.Count; i++)
                    {
                        object val;
                        translateOpts.Deserialise(elementType == typeof(object) ? null : elementType, arrVal[i], out val);
                        list.Add(val);
                    }
                }
                else
                {
                    for (int i = 0; i < arrVal.Count; i++)
                    {
                        object val;
                        translateOpts.Deserialise(null, arrVal[i], out val);
                        list.Add(val);
                    }
                }
            }
            else if (typeof(ICollection).IsAssignableFrom(t))
            {
                throw new NotImplementedException("Can't deserialise into an ICollection");
            }

            MapValue mapValue = node as MapValue;
            if (mapValue == null) return; // Throw error?

            foreach (var pair in mapValue)
            {
                MemberInfo member = t.GetMember(pair.Key, BindingFlags.Public | BindingFlags.Instance)
                    .Where(m => m is FieldInfo || m is PropertyInfo)
                    .SingleOrDefault();
                if (member != null)
                {
                    FieldInfo fieldInfo = member as FieldInfo;
                    if (fieldInfo != null)
                    {
                        object val;
                        translateOpts.Deserialise(fieldInfo.FieldType, pair.Value, out val);
                        if ((val == null && !fieldInfo.FieldType.IsValueType) || fieldInfo.FieldType.IsAssignableFrom(val.GetType()))
                            fieldInfo.SetValue(obj, val);
                    }
                    PropertyInfo propertyInfo = member as PropertyInfo;
                    if (propertyInfo != null)
                    {
                        object val;
                        translateOpts.Deserialise(propertyInfo.PropertyType, pair.Value, out val);
                        if ((val == null && !propertyInfo.PropertyType.IsValueType) || propertyInfo.PropertyType.IsAssignableFrom(val.GetType()))
                            propertyInfo.SetValue(obj, val, null);
                    }
                }
            }
        }
    }
}
