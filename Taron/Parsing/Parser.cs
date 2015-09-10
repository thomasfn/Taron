using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Taron.Parsing
{
    /// <summary>
    /// Responsible for parsing a token stream into a syntax tree
    /// </summary>
    public sealed class Parser
    {
        private enum ParserAction { Shift, Reduce, Done, Error }

        /// <summary>
        /// Represents a specific parser state
        /// </summary>
        private class StateDefinition
        {
            public struct Rule
            {
                public readonly ParserAction Action;
                public readonly int Arg;

                public Rule(ParserAction action, int arg)
                {
                    Action = action;
                    Arg = arg;
                }

                public override string ToString()
                {
                    return $"{Action} {Arg}";
                }
            }

            public readonly Rule[] LookaheadTable;
            public readonly int[] GotoTable;

            public StateDefinition(Rule[] lookaheadTable, int[] gotoTable)
            {
                LookaheadTable = lookaheadTable;
                GotoTable = gotoTable;
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append('[');
                sb.Append(string.Join(", ", LookaheadTable.Select(r => r.ToString()).ToArray()));
                sb.Append("] [");
                sb.Append(string.Join(", ", GotoTable.Select(i => i.ToString()).ToArray()));
                sb.Append(']');
                return sb.ToString();
            }

        }

        private GrammarRule[] grammarRules;
        private StateDefinition[] parseStates;

        private SymbolType goalSymbol;

        const int SYMCNT_TERMINAL = (int)SymbolType.TerminalCount + 1;
        const int SYMCNT_NONTERMINAL = (int)SymbolType.NonTerminalCount;
        const int SYMCNT_TOTAL = SYMCNT_TERMINAL + SYMCNT_NONTERMINAL;

        /// <summary>
        /// Initialises a new instance of the Parser class
        /// </summary>
        public Parser()
        {
            // Generate grammar rules
            grammarRules = new List<GrammarRule>()
            {
                // TypeName := '<' Identifier '>'
                GrammarRule.Sequence(SymbolType.TypeName,               SymbolType.OpenTypeDef, SymbolType.Identifier, SymbolType.CloseTypeDef),

                // PrimitiveValue := String | Number | Boolean
                GrammarRule.Sequence(SymbolType.PrimitiveValue,         SymbolType.StringLiteral),
                GrammarRule.Sequence(SymbolType.PrimitiveValue,         SymbolType.NumberLiteral),
                GrammarRule.Sequence(SymbolType.PrimitiveValue,         SymbolType.BooleanLiteral),

                // EnumValue := Identifier '.' Identifier | Identifier '.' EnumValue
                GrammarRule.Sequence(SymbolType.EnumValue,              SymbolType.Identifier, SymbolType.Dot, SymbolType.Identifier),
                GrammarRule.Sequence(SymbolType.EnumValue,              SymbolType.Identifier, SymbolType.Dot, SymbolType.EnumValue),

                // MapValue := '{' KeyValueSeq '}' | '{' '}'
                GrammarRule.Sequence(SymbolType.MapValue,               SymbolType.OpenMap, SymbolType.KeyValueSeq, SymbolType.CloseMap),
                GrammarRule.Sequence(SymbolType.MapValue,               SymbolType.OpenMap, SymbolType.CloseMap),

                // ArrayValue := '[' ArraySeq ']' | '[' ']'
                GrammarRule.Sequence(SymbolType.ArrayValue,            SymbolType.OpenArray, SymbolType.ArraySeq, SymbolType.CloseArray),
                GrammarRule.Sequence(SymbolType.ArrayValue,            SymbolType.OpenArray, SymbolType.CloseArray),

                // TypedMapValue = TypeName MapValue
                GrammarRule.Sequence(SymbolType.TypedMapValue,            SymbolType.TypeName, SymbolType.MapValue),

                // TypedArrayValue = TypeName ArrayValue
                GrammarRule.Sequence(SymbolType.TypedArrayValue,            SymbolType.TypeName, SymbolType.ArrayValue),

                // ComplexValue := TypedMapValue | TypedArrayValue | MapValue | ArrayValue
                GrammarRule.Sequence(SymbolType.ComplexValue,            SymbolType.TypedMapValue),
                GrammarRule.Sequence(SymbolType.ComplexValue,            SymbolType.TypedArrayValue),
                GrammarRule.Sequence(SymbolType.ComplexValue,            SymbolType.MapValue),
                GrammarRule.Sequence(SymbolType.ComplexValue,            SymbolType.ArrayValue),

                // KeyValue := Identifier ComplexValue | Identifier '=' PrimitiveValue | Identifier '=' EnumValue | TypeName Identifier '=' PrimitiveValue
                GrammarRule.Sequence(SymbolType.KeyValue,            SymbolType.Identifier, SymbolType.ComplexValue),
                GrammarRule.Sequence(SymbolType.KeyValue,            SymbolType.Identifier, SymbolType.Assign, SymbolType.PrimitiveValue),
                GrammarRule.Sequence(SymbolType.KeyValue,            SymbolType.Identifier, SymbolType.Assign, SymbolType.EnumValue),
                GrammarRule.Sequence(SymbolType.KeyValue,            SymbolType.TypeName, SymbolType.Identifier, SymbolType.Assign, SymbolType.PrimitiveValue),

                // KeyValueSeq := KeyValue* | Ignore
                GrammarRule.Sequence(SymbolType.KeyValueSeq,            SymbolType.KeyValueSeq, SymbolType.KeyValue),
                GrammarRule.Sequence(SymbolType.KeyValueSeq,            SymbolType.KeyValue),

                // ArraySeq := ArraySeq ',' PrimitiveValue | ArraySeq ',' ComplexValue | ArraySeq ',' EnumValue | PrimitiveValue | ComplexValue | EnumValue
                GrammarRule.Sequence(SymbolType.ArraySeq,            SymbolType.ArraySeq, SymbolType.Seperator, SymbolType.PrimitiveValue),
                GrammarRule.Sequence(SymbolType.ArraySeq,            SymbolType.ArraySeq, SymbolType.Seperator, SymbolType.ComplexValue),
                GrammarRule.Sequence(SymbolType.ArraySeq,            SymbolType.ArraySeq, SymbolType.Seperator, SymbolType.EnumValue),
                GrammarRule.Sequence(SymbolType.ArraySeq,            SymbolType.PrimitiveValue),
                GrammarRule.Sequence(SymbolType.ArraySeq,            SymbolType.ComplexValue),
                GrammarRule.Sequence(SymbolType.ArraySeq,            SymbolType.EnumValue),

            }.ToArray();
            goalSymbol = SymbolType.KeyValueSeq;

            /*grammarRules = new List<GrammarRule>()
            {
                GrammarRule.Sequence(SymbolType.E,      SymbolType.E, SymbolType.Mult, SymbolType.B),
                GrammarRule.Sequence(SymbolType.E,      SymbolType.E, SymbolType.Add, SymbolType.B),
                GrammarRule.Sequence(SymbolType.E,      SymbolType.B),
                GrammarRule.Sequence(SymbolType.B,      SymbolType.Zero),
                GrammarRule.Sequence(SymbolType.B,      SymbolType.One)
            }.ToArray();
            goalSymbol = SymbolType.E;*/

            // Generate parse tables
            GenerateParseTables();
        }

        #region Parse Table Generation

        private struct RuleSymbolLink : IEquatable<RuleSymbolLink>
        {
            public readonly GrammarRule Rule;
            public readonly int Index;
            public readonly SymbolType Lookahead;

            public bool PrecedesNonTerminal
            {
                get
                {
                    return Index < Rule.MatchSymbols.Length && (Rule.MatchSymbols[Index] & SymbolType.NonTerminal) == SymbolType.NonTerminal;
                }
            }

            public RuleSymbolLink(GrammarRule rule, int index, SymbolType lookahead = SymbolType.None)
            {
                Rule = rule;
                Index = index;
                Lookahead = lookahead;
            }

            public bool GetNextSymbol(out SymbolType nextSymbol)
            {
                if (Index < Rule.MatchSymbols.Length)
                {
                    nextSymbol = Rule.MatchSymbols[Index];
                    return true;
                }
                else
                {
                    nextSymbol = SymbolType.None;
                    return false;
                }
            }

            public override string ToString()
            {
                return Rule.ToString(Index);
            }

            public bool Equals(RuleSymbolLink other)
            {
                return Rule == other.Rule && Index == other.Index && Lookahead == other.Lookahead;
            }
        }

        private struct ItemSet : IEquatable<ItemSet>
        {
            public readonly GrammarRule SourceRule;
            public readonly RuleSymbolLink[] Items;
            public readonly int[] TableRow;

            public ItemSet(GrammarRule sourceRule, RuleSymbolLink[] items)
            {
                SourceRule = sourceRule;
                Items = items;
                TableRow = new int[SYMCNT_TOTAL];
                for (int i = 0; i < SYMCNT_TOTAL; i++)
                {
                    TableRow[i] = -1;
                }
            }

            public bool Equals(ItemSet other)
            {
                if (Items.Length != other.Items.Length) return false;
                for (int i = 0; i < Items.Length; i++)
                {
                    if (!Items[i].Equals(other.Items[i]))
                        return false;
                }
                return true;
            }
        }

        private void GenerateParseTables()
        {
            // Produce item sets from rules (item set = list of rules that a symbol might be a part of)
            // 1 item set per symbol, 1 parser state per item set

            // Produce all item sets
            GrammarRule goalRule = GrammarRule.Sequence(SymbolType.Goal, goalSymbol);

            ItemSet goalItemSet;
            List<ItemSet> itemSets = new List<ItemSet>();
            {
                List<RuleSymbolLink> itemSet = new List<RuleSymbolLink>();
                BuildItemSet(goalRule, itemSet);
                itemSets.Add(goalItemSet = new ItemSet(goalRule, itemSet.ToArray()));
            }
            /*for (int i = 0; i < grammarRules.Length; i++)
            {
                GrammarRule rule = grammarRules[i];
                List<RuleSymbolLink> itemSet = new List<RuleSymbolLink>();
                BuildItemSet(rule, itemSet);
                itemSets.Add(new ItemSet(rule, itemSet.ToArray()));
            }*/

            // Derive all item sets
            Stack<int> toProcess = new Stack<int>();
            toProcess.Push(0);
            while (toProcess.Count > 0)
            {
                int curItemSetIdx = toProcess.Pop();
                ItemSet curItemSet = itemSets[curItemSetIdx];

                HashSet<SymbolType> possibleSymbols = new HashSet<SymbolType>();
                for (int i = 0; i < curItemSet.Items.Length; i++)
                {
                    SymbolType symbol;
                    if (curItemSet.Items[i].GetNextSymbol(out symbol)) possibleSymbols.Add(symbol);
                }
                foreach (SymbolType pSymbol in possibleSymbols)
                {
                    List<RuleSymbolLink> subset = new List<RuleSymbolLink>();
                    for (int i = 0; i < curItemSet.Items.Length; i++)
                    {
                        SymbolType symbol;
                        if (curItemSet.Items[i].GetNextSymbol(out symbol) && pSymbol == symbol)
                        {
                            RuleSymbolLink item = curItemSet.Items[i];
                            item = new RuleSymbolLink(item.Rule, item.Index + 1, item.Lookahead);
                            subset.Add(item);
                        }
                    }
                    CloseItemSet(subset);
                    ItemSet newItemSet = new ItemSet(null, subset.ToArray());
                    int newIdx = itemSets.IndexOf(newItemSet);
                    if (newIdx == -1)
                    {
                        newIdx = itemSets.Count;
                        itemSets.Add(newItemSet);
                        toProcess.Push(newIdx);
                    }

                    curItemSet.TableRow[SymbolTypeToCol(pSymbol)] = newIdx;
                }
            }

            // Create states
            parseStates = new StateDefinition[itemSets.Count];
            for (int i = 0; i < itemSets.Count; i++)
            {
                ItemSet itemSet = itemSets[i];
                StateDefinition.Rule[] lookaheadTable = new StateDefinition.Rule[SYMCNT_TERMINAL];
                int[] gotoTable = new int[SYMCNT_NONTERMINAL];
                for (int j = 0; j < SYMCNT_TERMINAL - 1; j++)
                {
                    SymbolType symbol = SymbolType.Terminal | (SymbolType)(j + 1);
                    int arg = itemSet.TableRow[SymbolTypeToCol(symbol)];
                    if (arg != -1)
                        lookaheadTable[j] = new StateDefinition.Rule(ParserAction.Shift, arg);
                    else
                        lookaheadTable[j] = new StateDefinition.Rule(ParserAction.Error, 0);
                }
                if (itemSet.Items.Any(item => item.Rule == goalRule))
                {
                    lookaheadTable[SYMCNT_TERMINAL - 1] = new StateDefinition.Rule(ParserAction.Done, 0);
                }
                else
                {
                    lookaheadTable[SYMCNT_TERMINAL - 1] = new StateDefinition.Rule(ParserAction.Error, 0);
                }
                for (int j = 0; j < SYMCNT_NONTERMINAL; j++)
                {
                    SymbolType symbol = SymbolType.NonTerminal | (SymbolType)(j + 1);
                    gotoTable[j] = itemSet.TableRow[SymbolTypeToCol(symbol)];
                }
                parseStates[i] = new StateDefinition(lookaheadTable, gotoTable);
            }
            for (int i = 0; i < itemSets.Count; i++)
            {
                ItemSet itemSet = itemSets[i];
                bool reduce = false;
                RuleSymbolLink reduceItem = default(RuleSymbolLink);
                for (int j = 0; j < itemSet.Items.Length; j++)
                {
                    RuleSymbolLink item = itemSet.Items[j];
                    if (item.Rule != goalRule && item.Index >= item.Rule.MatchSymbols.Length)
                    {
                        reduce = true;
                        reduceItem = item;
                        break;
                    }
                }
                if (reduce)
                {
                    int ruleIndex = -1;
                    for (int j = 0; j < grammarRules.Length; j++)
                    {
                        if (grammarRules[j] == reduceItem.Rule)
                        {
                            ruleIndex = j;
                            break;
                        }
                    }
                    var pS = parseStates[i];
                    for (int j = 0; j < SYMCNT_TERMINAL; j++)
                    {
                        if (pS.LookaheadTable[j].Action == ParserAction.Shift)
                        {
                            // Shift-reduce conflict
                            throw new Exception($"Shift-reduce conflict ({grammarRules[ruleIndex]})");
                        }
                        else if (pS.LookaheadTable[j].Action == ParserAction.Reduce)
                        {
                            // Reduce-reduce conflict
                            throw new Exception($"Reduce-reduce conflict ({grammarRules[ruleIndex]})");
                        }
                        else
                        {
                            pS.LookaheadTable[j] = new StateDefinition.Rule(ParserAction.Reduce, ruleIndex);
                        }
                    }
                }
            }
        }

        private static int SymbolTypeToCol(SymbolType symbol, bool relative = false)
        {
            if (symbol == SymbolType.EndOfStream)
                return SYMCNT_TERMINAL - 1;
            else if ((symbol & SymbolType.Terminal) == SymbolType.Terminal)
                return (int)(symbol & ~SymbolType.Terminal) - 1;
            else if ((symbol & SymbolType.NonTerminal) == SymbolType.NonTerminal)
                return (int)(symbol & ~SymbolType.NonTerminal) + (relative ? 0 : SYMCNT_TERMINAL) - 1;
            else
                return -1;
        }

        private void BuildItemSet(GrammarRule rule, List<RuleSymbolLink> itemSet, bool justIndex0 = false)
        {
            if (rule.MatchSymbols.Length == 1 || justIndex0)
            {
                itemSet.Add(new RuleSymbolLink(rule, 0));
            }
            else
            {
                for (int j = 0; j <= rule.MatchSymbols.Length; j++)
                {
                    itemSet.Add(new RuleSymbolLink(rule, j));
                }
            }
            CloseItemSet(itemSet);
        }

        private void CloseItemSet(List<RuleSymbolLink> itemSet)
        {
            List<RuleSymbolLink> closedItems = new List<RuleSymbolLink>();
            for (int i = 0; i < itemSet.Count; i++)
            {
                RuleSymbolLink item = itemSet[i];
                if (item.PrecedesNonTerminal)
                {
                    List<RuleSymbolLink> tmp = new List<RuleSymbolLink>();
                    foreach (var otherRule in grammarRules.Where(r => r.OutputSymbol == item.Rule.MatchSymbols[item.Index] && r.OutputSymbol != item.Rule.OutputSymbol))
                    {
                        var link = new RuleSymbolLink(otherRule, 0);
                        tmp.Add(link);
                    }
                    CloseItemSet(tmp);
                    foreach (var tmpItem in tmp)
                        if (!closedItems.Contains(tmpItem))
                            closedItems.Add(tmpItem);
                }
            }
            itemSet.AddRange(closedItems);
        }

        private SymbolType[] GenerateFirstSet(SymbolType symbol)
        {
            HashSet<SymbolType> set = new HashSet<SymbolType>();
            foreach (GrammarRule rule in grammarRules.Where(g => g.OutputSymbol == symbol))
            {
                SymbolType firstSymbol = rule.MatchSymbols[0];
                if ((firstSymbol & SymbolType.Terminal) == SymbolType.Terminal)
                {
                    set.Add(firstSymbol);
                }
                else
                {
                    foreach (SymbolType subSymbol in GenerateFirstSet(firstSymbol))
                        set.Add(subSymbol);
                }
            }
            return set.ToArray();
        }

        private SymbolType[] GenerateFollowSet(List<RuleSymbolLink> itemSet, SymbolType nonTerminal)
        {
            HashSet<SymbolType> set = new HashSet<SymbolType>();
            for (int i = 0; i < itemSet.Count; i++)
            {
                RuleSymbolLink item = itemSet[i];
                int idx = item.Index + 1;
                if (idx < item.Rule.MatchSymbols.Length)
                {
                    SymbolType symbol = item.Rule.MatchSymbols[idx];
                    if ((symbol & SymbolType.Terminal) == SymbolType.Terminal)
                        set.Add(symbol);
                }
                else
                    set.Add(SymbolType.EndOfStream);
            }
            return set.ToArray();
        }

        #endregion

        public Model.Node Parse(TokenStream strm)
        {
            // Sanity check
            if (strm.EoS) return null;

            // Initialise parse state
            Stack<Symbol> parseStack = new Stack<Symbol>();
            parseStack.Push(Symbol.ParseState(0));
            Symbol lookahead = strm.Read();

            // Loop until we're done
            bool done = false;
            while (!done)
            {
                // Read current parse state
                Symbol parseState = parseStack.Peek();
                if (!parseState.IsParseState) throw new InvalidOperationException();
                StateDefinition state = parseStates[parseState.State];

                // Select next action
                int terminalIndex = lookahead != null ? lookahead.TerminalIndex : SYMCNT_TERMINAL - 1;
                var rule = state.LookaheadTable[terminalIndex];
                switch (rule.Action)
                {
                    case ParserAction.Shift:
                        parseStack.Push(lookahead);
                        lookahead = strm.Read();
                        parseStack.Push(Symbol.ParseState(rule.Arg));
                        break;
                    case ParserAction.Reduce:
                        GrammarRule grammarRule = grammarRules[rule.Arg];
                        int toRemove = grammarRule.MatchSymbols.Length;
                        Symbol[] children = new Symbol[toRemove];
                        while (toRemove > 0)
                        {
                            Symbol s = parseStack.Pop();
                            if (!s.IsParseState)
                            {
                                children[toRemove - 1] = s;
                                toRemove--;
                            }
                        }
                        Symbol newSymbol = new Symbol(grammarRule.OutputSymbol, children);
                        Symbol priorState = parseStack.Peek();
                        if (!priorState.IsParseState) throw new InvalidOperationException();
                        StateDefinition priorStateDef = parseStates[priorState.State];
                        int nextState = priorStateDef.GotoTable[SymbolTypeToCol(grammarRule.OutputSymbol, true)];
                        parseStack.Push(newSymbol);
                        parseStack.Push(Symbol.ParseState(nextState));
                        break;
                    case ParserAction.Done:
                        done = true;
                        break;
                    case ParserAction.Error:
                        if (lookahead != null)
                            throw new InvalidOperationException($"Unexpected symbol '{lookahead.Value}' (line {lookahead.Position.Line}, col {lookahead.Position.Column})");
                        else
                            throw new InvalidOperationException($"Unexpected end of stream");
                }
            }

            // Locate the parse tree
            Symbol parseTree = null;
            while (parseStack.Count > 0)
            {
                Symbol sym = parseStack.Pop();
                if (!sym.IsParseState)
                {
                    if (parseTree != null)
                        throw new InvalidOperationException($"Multiple parse trees generated (got '{sym.Type}', already had '{parseTree.Type}')");
                    else
                        parseTree = sym;
                }
            }
            if (parseTree == null) throw new InvalidOperationException($"Parse tree not generated");

            // Identify it
            switch (parseTree.Type)
            {
                case SymbolType.KeyValueSeq:
                    // We're going to emit a map
                    var map = new Model.MapValue();
                    PopulateMap(map, parseTree);
                    return map;
                case SymbolType.ArraySeq:
                    // We're going to emit an array
                    var arr = new Model.ArrayValue();
                    PopulateArray(arr, parseTree);
                    return arr;
                default:
                    throw new InvalidOperationException($"Unexpected root symbol '{parseTree.Type}'");
            }
        }

        #region Parse Tree Decomposition

        private static void PopulateMap(Model.MapValue map, Symbol keyValueSeq)
        {
            // Verify
            if (keyValueSeq.Type != SymbolType.KeyValueSeq) throw new InvalidOperationException($"Trying to populate map from invalid symbol '{keyValueSeq.Type}'");

            // Get the key value
            Symbol keyValue = keyValueSeq.Children[keyValueSeq.Children.Length - 1];
            if (keyValue.Type != SymbolType.KeyValue) throw new InvalidOperationException($"Trying to populate map entry from invalid symbol '{keyValue.Type}'");
            bool hasTypeName = keyValue.Children[0].Type == SymbolType.TypeName;
            string key = GetIdentifer(keyValue.Children[hasTypeName ? 1 : 0]);
            Model.ValueNode value = CreateValueNode(keyValue.Children[keyValue.Children.Length - 1]);
            if (hasTypeName) value.TypeName = GetTypeName(keyValue.Children[0]);

            // Insert it
            map.Add(key, value);

            // Recurse
            if (keyValueSeq.Children.Length == 2)
                PopulateMap(map, keyValueSeq.Children[0]);
        }

        private static void PopulateArray(Model.ArrayValue arr, Symbol arraySeq)
        {
            // Verify
            if (arraySeq.Type != SymbolType.ArraySeq) throw new InvalidOperationException($"Trying to populate array from invalid symbol '{arraySeq.Type}'");

            if (arraySeq.Children[0].Type == SymbolType.ArraySeq) PopulateArray(arr, arraySeq.Children[0]);

            var valueNode = CreateValueNode(arraySeq.Children[arraySeq.Children.Length - 1]);
            arr.Add(valueNode);

            
        }

        private static string GetIdentifer(Symbol identifier)
        {
            // Verify
            if (identifier.Type != SymbolType.Identifier) throw new InvalidOperationException($"Trying to read identifier from invalid symbol '{identifier.Type}'");

            // Return
            return identifier.Value;
        }

        private static string GetTypeName(Symbol typeName)
        {
            // Verify
            if (typeName.Type != SymbolType.TypeName) throw new InvalidOperationException($"Trying to read type name from invalid symbol '{typeName.Type}'");

            // Return
            return typeName.Children[1].Value;
        }

        private static Model.ValueNode CreateValueNode(Symbol src)
        {
            // What type of symbol is it?
            switch (src.Type)
            {
                case SymbolType.PrimitiveValue:
                    return FromPrimitiveValue(src);
                case SymbolType.EnumValue:
                    return FromEnumValue(src);
                case SymbolType.ComplexValue:
                    return FromComplexValue(src);
                default:
                    return null;
            }
        }

        private static Model.ValueNode FromComplexValue(Symbol complexValue)
        {
            // Verify
            if (complexValue.Type != SymbolType.ComplexValue) throw new InvalidOperationException($"Trying to read complex value from invalid symbol '{complexValue.Type}'");

            Symbol child = complexValue.Children[0];
            return FromMapOrArrayValue(child);
        }

        private static Model.ValueNode FromMapOrArrayValue(Symbol val)
        {
            Model.ValueNode node;
            string typeName;
            switch (val.Type)
            {
                case SymbolType.TypedMapValue:
                case SymbolType.TypedArrayValue:
                    typeName = GetTypeName(val.Children[0]);
                    node = FromMapOrArrayValue(val.Children[1]);
                    node.TypeName = typeName;
                    return node;
                case SymbolType.MapValue:
                    var map = new Model.MapValue();
                    if (val.Children.Length > 2)
                        PopulateMap(map, val.Children[1]);
                    return map;
                case SymbolType.ArrayValue:
                    var arr = new Model.ArrayValue();
                    if (val.Children.Length > 2)
                        PopulateArray(arr, val.Children[1]);
                    return arr;
            }
            return null;
        }

        private static Model.ValueNode FromPrimitiveValue(Symbol primValue)
        {
            // Verify
            if (primValue.Type != SymbolType.PrimitiveValue) throw new InvalidOperationException($"Trying to read primitive value from invalid symbol '{primValue.Type}'");

            Symbol child = primValue.Children[0];
            switch (child.Type)
            {
                case SymbolType.NumberLiteral:
                    return FromNumberLiteral(child.Value);
                case SymbolType.StringLiteral:
                    return new Model.PrimitiveValue<string>(child.Value.Substring(1, child.Value.Length - 2));
                case SymbolType.BooleanLiteral:
                    return FromBooleanLiteral(child.Value);
                default:
                    throw new InvalidOperationException($"Unknown primitive value '{primValue.Type}'");
            }
        }

        private static Model.ValueNode FromEnumValue(Symbol enumValue)
        {
            List<string> idList = new List<string>();
            Symbol curEnum = enumValue;
            while (curEnum != null)
            {
                if (curEnum.Type == SymbolType.Identifier)
                {
                    idList.Add(curEnum.Value);
                    curEnum = null;
                }
                else if (curEnum.Type == SymbolType.EnumValue)
                {
                    idList.Add(curEnum.Children[0].Value);
                    curEnum = curEnum.Children[2];
                }
                else
                {
                    throw new InvalidOperationException($"Trying to read enumeration value from invalid symbol '{curEnum.Type}'");
                }
            }

            return new Model.EnumValue(idList.ToArray());
        }

        private static Model.ValueNode FromBooleanLiteral(string literal)
        {
            bool bVal;
            if (bool.TryParse(literal, out bVal))
                return new Model.PrimitiveValue<bool>(bVal);
            throw new InvalidOperationException($"Unknown boolean primitive '{literal}'");
        }

        private static Model.ValueNode FromNumberLiteral(string literal)
        {
            int iVal;
            if (int.TryParse(literal, out iVal))
                return new Model.PrimitiveValue<int>(iVal);
            double dVal;
            if (double.TryParse(literal, out dVal))
                return new Model.PrimitiveValue<double>(dVal);
            throw new InvalidOperationException($"Unknown numeric primitive '{literal}'");
        }

        #endregion

    }
}
