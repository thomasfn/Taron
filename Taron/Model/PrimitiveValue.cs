using System;

namespace Taron.Model
{
    /// <summary>
    /// Represents a primitive value node
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PrimitiveValue<T> : ValueNode
    {
        /// <summary>
        /// Gets the value of this node
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// Initialises a new instance of the PrimitiveValue class
        /// </summary>
        /// <param name="value"></param>
        public PrimitiveValue(T value = default(T))
        {
            Value = value;
        }

        public override string ToString()
        {
            return $"{base.ToString()} ({Value})";
        }
    }
}
