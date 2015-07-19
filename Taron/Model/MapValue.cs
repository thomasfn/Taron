using System;
using System.Collections;
using System.Collections.Generic;

namespace Taron.Model
{
    /// <summary>
    /// Represents a set of properties
    /// </summary>
    public class MapValue : ValueNode, IDictionary<string, ValueNode>
    {
        private IDictionary<string, ValueNode> properties;

        #region IDictionary

        public ValueNode this[string key]
        {
            get
            {
                return properties[key];
            }

            set
            {
                properties[key] = value;
            }
        }

        public int Count
        {
            get
            {
                return properties.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return properties.IsReadOnly;
            }
        }

        public ICollection<string> Keys
        {
            get
            {
                return properties.Keys;
            }
        }

        public ICollection<ValueNode> Values
        {
            get
            {
                return properties.Values;
            }
        }

        public void Add(KeyValuePair<string, ValueNode> item)
        {
            properties.Add(item);
        }

        public void Add(string key, ValueNode value)
        {
            properties.Add(key, value);
        }

        public void Clear()
        {
            properties.Clear();
        }

        public bool Contains(KeyValuePair<string, ValueNode> item)
        {
            return properties.Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return properties.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, ValueNode>[] array, int arrayIndex)
        {
            properties.CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<string, ValueNode>> GetEnumerator()
        {
            return properties.GetEnumerator();
        }

        public bool Remove(KeyValuePair<string, ValueNode> item)
        {
            return properties.Remove(item);
        }

        public bool Remove(string key)
        {
            return properties.Remove(key);
        }

        public bool TryGetValue(string key, out ValueNode value)
        {
            return properties.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return properties.GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Initialises a new instance of the MapValue class
        /// </summary>
        public MapValue()
        {
            properties = new Dictionary<string, ValueNode>();
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(TypeName))
                return $"TaronMap<{TypeName}>";
            else
                return "TaronMap";
        }
    }
}
