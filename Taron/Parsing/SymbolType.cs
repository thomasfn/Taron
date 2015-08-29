using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Taron.Parsing
{
    /// <summary>
    /// Represents the type of a symbol
    /// </summary>
    public enum SymbolType
    {
        None        = 0x0000,
        ParseState  = 0x0001,
        Goal        = 0x0002,
        EndOfStream = 0x0003,

        Terminal    = 0x1000,
        NonTerminal = 0x2000,

        #region Terminals (Tokens)

        [TokenPattern(@"\s+", true)] Whitespace   = Terminal | 0x001,

        [TokenPattern("{")] OpenMap         = Terminal | 0x002,
        [TokenPattern("}")] CloseMap        = Terminal | 0x003,

        [TokenPattern(@"\[")] OpenArray     = Terminal | 0x004,
        [TokenPattern(@"\]")] CloseArray    = Terminal | 0x005,

        [TokenPattern("<")] OpenTypeDef     = Terminal | 0x006,
        [TokenPattern(">")] CloseTypeDef    = Terminal | 0x007,

        [TokenPattern(@"=")] Assign         = Terminal | 0x008,
        [TokenPattern(@",")] Seperator      = Terminal | 0x009,

        [TokenPattern("\"(([^\"\\\\])*(\\\\\")?)*\\\"")] StringLiteral      = Terminal | 0x00A,
        [TokenPattern(@"-?([0-9]+(\.[0-9]+)?)")] NumberLiteral              = Terminal | 0x00B,
        [TokenPattern("(?:false|true)")] BooleanLiteral                     = Terminal | 0x00C,

        [TokenPattern(@"[a-zA-Z_](\w*)")] Identifier        = Terminal | 0x00D,
        [TokenPattern(@"\.")] Dot                           = Terminal | 0x00E,

        TerminalCount = 0x00E,

        #endregion

        #region Non-Terminals (Parsed)

        TypeName            = NonTerminal | 0x001,
        PrimitiveValue      = NonTerminal | 0x002,
        EnumValue           = NonTerminal | 0x003,
        MapValue            = NonTerminal | 0x004,
        ArrayValue          = NonTerminal | 0x005,
        TypedMapValue       = NonTerminal | 0x006,
        TypedArrayValue     = NonTerminal | 0x007,
        ComplexValue        = NonTerminal | 0x008,
        KeyValue            = NonTerminal | 0x009,
        KeyValueSeq         = NonTerminal | 0x00A,
        ArraySeq            = NonTerminal | 0x00B,

        NonTerminalCount = 0x00B

        #endregion
    }

    
}
