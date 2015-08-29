using System;

namespace Taron.Model
{
    /// <summary>
    /// Represents an enumeration value node
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EnumValue : ValueNode
    {
        /// <summary>
        /// Gets or sets the value of this node
        /// </summary>
        public string[] Value { get; set; }

        /// <summary>
        /// Gets or sets the concatenated value of this node
        /// </summary>
        public string Full
        {
            get
            {
                return string.Join(".", Value);
            }
            set
            {
                Value = value.Split('.');
            }
        }

        /// <summary>
        /// Initialises a new instance of the EnumValue class
        /// </summary>
        /// <param name="value"></param>
        public EnumValue(string[] value)
        {
            Value = value;
        }

        /// <summary>
        /// Initialises a new instance of the EnumValue class
        /// </summary>
        /// <param name="value"></param>
        public EnumValue(string value)
        {
            Full = value;
        }

        public override string ToString()
        {
            return $"{base.ToString()} ({Full})";
        }
    }
}
