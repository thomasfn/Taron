using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

using System.Linq;

using Taron;
using Taron.Model;
using TaronTranslator = Taron.Translator.ReflectionTranslator;
using TaronTranslatorOptions = Taron.Translator.TranslateOptions;

namespace Taron.Tests
{
    /// <summary>
    /// Contains tests for the translator
    /// </summary>
    [TestClass]
    public class Translator
    {
        /// <summary>
        /// Tests translating of a basic map from a model to a weakly typed dictionary
        /// </summary>
        [TestMethod]
        public void Translator_BasicMap_FromModel_ToDict()
        {
            // Parse the basic map
            Node node = TaronParser.Parse(TestUtils.ReusableMapTest);
            Assert.IsNotNull(node);
            MapValue mapValue = node.As<MapValue>();
            Assert.IsNotNull(mapValue);

            // Translate to weakly-typed dict
            var t = new TaronTranslator(TaronTranslatorOptions.Default);
            var dict = t.Deserialise(typeof(Dictionary<string, object>), mapValue) as Dictionary<string, object>;
            Assert.IsNotNull(dict);
            Assert.AreEqual(5, dict.Count);
            Assert.AreEqual(10.0, dict["DecimalVal"]);
            Assert.AreEqual(5, dict["IntegerVal"]);
            Assert.AreEqual("thingy", dict["StringVal"]);
            Assert.AreEqual(true, dict["BooleanVal"]);
        }

        /// <summary>
        /// Tests translating of a basic map from a model to a struct
        /// </summary>
        [TestMethod]
        public void Translator_BasicMap_FromModel_ToStruct()
        {
            // Parse the basic map
            Node node = TaronParser.Parse(TestUtils.ReusableMapTest);
            Assert.IsNotNull(node);
            MapValue mapValue = node.As<MapValue>();
            Assert.IsNotNull(mapValue);

            // Translate to a strongly-typed struct
            var t = new TaronTranslator(TaronTranslatorOptions.Default);
            var strct = (TestUtils.ReusableMapStruct)t.Deserialise(typeof(TestUtils.ReusableMapStruct), mapValue);
            strct.Test();
        }


    }
}
