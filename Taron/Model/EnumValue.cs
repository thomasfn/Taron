using System;

namespace Taron.Model
{
    /// <summary>
    /// Represents a primitive value node
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EnumValue : ValueNode
    {
        /// <summary>
        /// Gets the type value of this node
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets the value value of this node
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Initialises a new instance of the PrimitiveValue class
        /// </summary>
        /// <param name="value"></param>
        public EnumValue(string type, string value)
        {
            Type = type;
            Value = value;
        }

        public override string ToString()
        {
            return $"{base.ToString()} ({Type}.{Value})";
        }
    }
}
