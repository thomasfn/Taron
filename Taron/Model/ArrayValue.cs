using System;
using System.Collections;
using System.Collections.Generic;

namespace Taron.Model
{
    /// <summary>
    /// Represents an array of values
    /// </summary>
    public class ArrayValue : ValueNode, IList<ValueNode>
    {
        private IList<ValueNode> items;

        #region IList

        public ValueNode this[int index]
        {
            get
            {
                return items[index];
            }

            set
            {
                items[index] = value;
            }
        }

        public int Count
        {
            get
            {
                return items.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return items.IsReadOnly;
            }
        }

        public void Add(ValueNode item)
        {
            items.Add(item);
        }

        public void Clear()
        {
            items.Clear();
        }

        public bool Contains(ValueNode item)
        {
            return items.Contains(item);
        }

        public void CopyTo(ValueNode[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }

        public IEnumerator<ValueNode> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        public int IndexOf(ValueNode item)
        {
            return items.IndexOf(item);
        }

        public void Insert(int index, ValueNode item)
        {
            items.Insert(index, item);
        }

        public bool Remove(ValueNode item)
        {
            return items.Remove(item);
        }

        public void RemoveAt(int index)
        {
            items.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Initialises a new instance of the ArrayValue class
        /// </summary>
        public ArrayValue()
        {
            items = new List<ValueNode>();
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(TypeName))
                return $"TaronArray<{TypeName}>";
            else
                return "TaronArray";
        }
    }
}
