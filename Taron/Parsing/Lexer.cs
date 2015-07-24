using System;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace Taron.Parsing
{
    /// <summary>
    /// Specifies that the symbol can be matched at the lexer stage using a regex pattern
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    public sealed class TokenPattern : Attribute
    {
        public readonly string Pattern;
        public readonly bool Discard;

        public TokenPattern(string pattern, bool discard = false)
        {
            Pattern = pattern;
            Discard = discard;
        }
    }


    /// <summary>
    /// Responsible for tokenising a string
    /// </summary>
    public sealed class Lexer
    {
        /// <summary>
        /// Represents information about a certain token
        /// </summary>
        private class TokenInfo
        {
            public readonly Regex Pattern;
            public readonly SymbolType Token;
            public readonly bool Discard;

            public TokenInfo(Regex pattern, SymbolType token, bool discard)
            {
                Pattern = pattern;
                Token = token;
                Discard = discard;
            }

            public override string ToString()
            {
                return string.Format("{0} [{1}]", Token, Pattern);
            }
        }

        // Ordered list of token rules to match against
        private IList<TokenInfo> tokenInfoList;

        const int TAB_SIZE = 4; // This will never be 100% accurate, but it's better than just 1

        /// <summary>
        /// Initialises a new instance of the Lexer class
        /// </summary>
        public Lexer()
        {
            // Get all token rules
            Type t = typeof(SymbolType);
            FieldInfo[] fields = t.GetFields();
            tokenInfoList = new List<TokenInfo>();
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType == t)
                {
                    foreach(object token in field.GetCustomAttributes(typeof(TokenPattern), false))
                    {
                        TokenPattern patternAttr = (TokenPattern)token;
                        if (patternAttr != null)
                        {
                            tokenInfoList.Add(new TokenInfo(
                                new Regex(patternAttr.Pattern, RegexOptions.Compiled),
                                (SymbolType)field.GetRawConstantValue(),
                                patternAttr.Discard
                            ));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Tokenises the specified string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public IEnumerable<Symbol> Tokenise(string input)
        {
            int ptr = 0;
            SymbolPosition currentPos = new SymbolPosition(1, 0);
            while (ptr < input.Length)
            {
                bool found = false;
                for (int i = 0; i < tokenInfoList.Count; i++)
                {
                    TokenInfo tokenInfo = tokenInfoList[i];
                    Match match = tokenInfo.Pattern.Match(input, ptr);
                    if (match.Success && match.Index == ptr)
                    {
                        string segment = input.Substring(match.Index, match.Length);
                        if (!tokenInfo.Discard) yield return new Symbol(tokenInfo.Token, segment, currentPos);
                        ptr = match.Index + match.Length;
                        int numNewLines = 0, newCol = currentPos.Column;
                        for (int j = 0; j < segment.Length; j++)
                        {
                            if (segment[j] == '\n')
                            {
                                numNewLines++;
                                newCol = 0;
                            }
                            else if (segment[j] == '\t')
                                newCol += TAB_SIZE;
                            else if (segment[j] != '\r')
                                newCol++;
                        }
                        currentPos = new SymbolPosition(currentPos.Line + numNewLines, newCol);
                        found = true;
                        break;
                    }
                }
                if (!found) throw new Exception(string.Format("Unrecognised symbol '{0}'", StringUtils.Escape(input.Substring(ptr, 1))));
            }
            yield break;
        }

    }
}
