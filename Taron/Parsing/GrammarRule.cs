using System;
using System.Text;
using System.Linq;

namespace Taron.Parsing
{
    /// <summary>
    /// Represents a grammar rule
    /// </summary>
    public sealed class GrammarRule
    {
        /// <summary>
        /// The symbols that allow this rule to match
        /// </summary>
        public readonly SymbolType[] MatchSymbols;

        /// <summary>
        /// The symbol that this rule outputs
        /// </summary>
        public readonly SymbolType OutputSymbol;

        /// <summary>
        /// Initialises a new instance of the GrammarRule class
        /// </summary>
        /// <param name="matchSymbols"></param>
        /// <param name="outputSymbol"></param>
        public GrammarRule(SymbolType[] matchSymbols, SymbolType outputSymbol)
        {
            MatchSymbols = matchSymbols;
            OutputSymbol = outputSymbol;
        }

        /// <summary>
        /// Gets the index of the specified symbol type in the match list, or -1 if not found
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public int IndexOfMatchSymbol(SymbolType symbol, int offset = 0)
        {
            for (int i = offset; i < MatchSymbols.Length; i++)
            {
                if (MatchSymbols[i] == symbol)
                    return i;
            }
            return -1;
        }

        public override string ToString()
        {
            return $"{OutputSymbol} := {string.Join(" ", MatchSymbols)}";
        }

        public string ToString(int ptr)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < MatchSymbols.Length; i++)
            {
                if (i > 0) sb.Append(' ');
                if (ptr == i) sb.Append(" . ");
                sb.Append(MatchSymbols[i]);
            }
            if (ptr >= MatchSymbols.Length) sb.Append(" .");
            return $"{OutputSymbol} := {sb}";
        }

        public static GrammarRule Sequence(SymbolType outputSymbol, params SymbolType[] matchSymbols)
        {
            return new GrammarRule(matchSymbols, outputSymbol);
        }
    }
}
