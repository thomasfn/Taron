using System;

namespace Taron.Model
{
    /// <summary>
    /// Represents a node in a Taron model hierarchy
    /// </summary>
    public abstract class Node
    {
        /// <summary>
        /// Casts this node to the specified subclass
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual T As<T>() where T : Node
        {
            return this as T;
        }

        /// <summary>
        /// Gets if this node is the specified subclass type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual bool Is<T>() where T : Node
        {
            return this is T;
        }

        public override string ToString()
        {
            return "TaronNode";
        }
    }
}
