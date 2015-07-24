using System;
using System.Collections.Generic;
using System.Linq;

namespace Taron.Parsing
{
    /// <summary>
    /// Represents the position of a symbol in a file
    /// </summary>
    public struct SymbolPosition : IEquatable<SymbolPosition>
    {
        /// <summary>
        /// The line number
        /// </summary>
        public readonly int Line;

        /// <summary>
        /// The column number
        /// </summary>
        public readonly int Column;

        /// <summary>
        /// Initialises a new instance of the SymbolPosition struct
        /// </summary>
        /// <param name="line"></param>
        /// <param name="col"></param>
        public SymbolPosition(int line, int col)
        {
            Line = line;
            Column = col;
        }

        /// <summary>
        /// Gets a new position ahead of this one - if lines is > 0, chars is absolute
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="chars"></param>
        /// <returns></returns>
        public SymbolPosition Forward(int lines, int chars)
        {
            if (lines > 0)
                return new SymbolPosition(Line + lines, chars);
            else
                return new SymbolPosition(Line, Column + chars);
        }

        public bool Equals(SymbolPosition other)
        {
            return Line == other.Line && Column == other.Column;
        }
    }

    /// <summary>
    /// Represents a symbol of a specified type in the parse tree
    /// </summary>
    public sealed class Symbol
    {
        /// <summary>
        /// The type of this symbol
        /// </summary>
        public readonly SymbolType Type;

        /// <summary>
        /// The children symbols of this symbol
        /// </summary>
        public readonly Symbol[] Children;

        /// <summary>
        /// The string value of this symbol
        /// </summary>
        public readonly string Value;

        /// <summary>
        /// The parse state of this symbol
        /// </summary>
        public readonly int State;

        /// <summary>
        /// The position of this symbol
        /// </summary>
        public readonly SymbolPosition Position;

        /// <summary>
        /// Gets if this symbol is terminal
        /// </summary>
        public bool IsTerminal { get { return (Type & SymbolType.Terminal) == SymbolType.Terminal; } }

        /// <summary>
        /// Gets if this symbol is non-terminal
        /// </summary>
        public bool IsNonTerminal { get { return (Type & SymbolType.NonTerminal) == SymbolType.NonTerminal; } }

        /// <summary>
        /// Gets if this symbol represents a parse state
        /// </summary>
        public bool IsParseState { get { return Type == SymbolType.ParseState; } }

        /// <summary>
        /// Gets the index of this terminal symbol
        /// </summary>
        public int TerminalIndex {  get { return (int)(Type & ~SymbolType.Terminal) - 1; } }

        /// <summary>
        /// Gets the index of this terminal symbol
        /// </summary>
        public int NonTerminalIndex { get { return (int)(Type & ~SymbolType.NonTerminal) - 1; } }

        /// <summary>
        /// Initialises a new instance of the Symbol class
        /// </summary>
        /// <param name="type"></param>
        /// <param name="children"></param>
        public Symbol(SymbolType type, Symbol[] children)
        {
            Type = type;
            Children = children;
            Value = string.Join(" ", children.Select(c => c.Value).ToArray());
        }

        /// <summary>
        /// Initialises a new instance of the Symbol class
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public Symbol(SymbolType type, string value, SymbolPosition position)
        {
            Type = type;
            Value = value;
            Position = position;
        }

        /// <summary>
        /// Initialises a new instance of the Symbol class
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public Symbol(SymbolType type, int value)
        {
            Type = type;
            State = value;
        }

        public override string ToString()
        {
            if (IsParseState)
                return State.ToString();
            else
                return $"{Type} [{StringUtils.Escape(Value)}]";
        }

        public static Symbol ParseState(int state)
        {
            return new Symbol(SymbolType.ParseState, state);
        }
    }
}
