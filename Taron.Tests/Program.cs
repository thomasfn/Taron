using System;
using System.Collections.Generic;

using Taron.Parsing;
using Taron.Model;
using Taron.Translator;

namespace Taron.Tests
{
    class Program
    {
        struct Element
        {
            public string Name;
        }

        class Behaviour
        {

        }

        class b_fuel : Behaviour
        {
            public int FuelLevel;
            public int MaxFuelLevel;
            public Element[] Elements;
            public bool BooleanTest;

            public b_fuel()
            {

            }
        }

        static void Main(string[] args)
        {
            Lexer lexer = new Lexer();
            TokenStream strm = new TokenStream(lexer.Tokenise(Tests.test_a));
            Parser parser = new Parser();

            Node root;
            try
            {
                root = parser.Parse(strm);
                Console.WriteLine("It worked! {0}", root);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadKey();
                return;
            }

            var behavioursModel = root.As<MapValue>()["Behaviours"].As<MapValue>();

            Dictionary<string, Behaviour> behaviourDict = new Dictionary<string, Behaviour>();
            Translate.Populate(behaviourDict, behavioursModel);
            

            Console.ReadKey();
        }
    }
}
