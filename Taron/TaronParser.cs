using System;

using Taron.Model;
using Taron.Parsing;

namespace Taron
{
    /// <summary>
    /// Encapsulates the entry point for Taron functionality
    /// </summary>
    public static class TaronParser
    {
        private static Lexer lexer;
        private static Parser parser;

        static TaronParser()
        {
            lexer = new Lexer();
            parser = new Parser();
        }

        /// <summary>
        /// Parses the specified Taron source into a model node
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Node Parse(string source)
        {
            return parser.Parse(new TokenStream(lexer.Tokenise(source)));
        }

    }
}
