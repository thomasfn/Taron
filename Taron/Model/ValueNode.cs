using System;

namespace Taron.Model
{
    /// <summary>
    /// Represents a value node
    /// </summary>
    public abstract class ValueNode : Node
    {
        /// <summary>
        /// Gets the type name of this node
        /// </summary>
        public string TypeName { get; set; }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(TypeName))
                return $"TaronValueNode<{TypeName}>";
            else
                return "TaronValueNode";
        }
    }
}
