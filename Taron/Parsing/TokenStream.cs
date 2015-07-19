using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taron.Parsing
{
    public sealed class TokenStream
    {
        private IEnumerator<Symbol> symbolStream;

        private Symbol nextSymbol;

        public bool EoS { get; private set; }

        public TokenStream(IEnumerable<Symbol> symbolStream)
        {
            this.symbolStream = symbolStream.GetEnumerator();
            if (!this.symbolStream.MoveNext())
            {
                EoS = true;
            }
            else
            {
                nextSymbol = this.symbolStream.Current;
            }
        }

        public Symbol Read()
        {
            if (EoS) return null;
            Symbol ret = nextSymbol;
            if (!symbolStream.MoveNext())
            {
                EoS = true;
                nextSymbol = null;
            }
            else
            {
                nextSymbol = symbolStream.Current;
            }
            return ret;
        }

        public Symbol Peek()
        {
            if (EoS)
                return null;
            else
                return nextSymbol;
        }
    }
}
