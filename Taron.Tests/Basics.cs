using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;

using Taron;
using Taron.Model;
using Taron.Translator;

namespace Taron.Tests
{
    /// <summary>
    /// Contains basic tests
    /// </summary>
    [TestClass]
    public class Basics
    {
        /// <summary>
        /// Tests parsing of empty string and whitespace
        /// </summary>
        [TestMethod]
        public void Basic_Empty()
        {
            // Test empty string
            Assert.IsNull(TaronParser.Parse(""));

            // Test variations of whitespace
            Assert.IsNull(TaronParser.Parse("\t"));
            Assert.IsNull(TaronParser.Parse(" "));
            Assert.IsNull(TaronParser.Parse("\t "));
            Assert.IsNull(TaronParser.Parse(" \t"));
            Assert.IsNull(TaronParser.Parse("\t \n"));
            Assert.IsNull(TaronParser.Parse(" \t\n"));
            Assert.IsNull(TaronParser.Parse("\t \n\t"));
            Assert.IsNull(TaronParser.Parse("\t \n "));
            Assert.IsNull(TaronParser.Parse("\t \n\t "));
            Assert.IsNull(TaronParser.Parse(" \t\n \t"));
        }

        /// <summary>
        /// Tests parsing of simple string key values
        /// </summary>
        [TestMethod]
        public void Basic_Strings()
        {
            // Iterate each test value
            string[] testValues = new string[] { "", "test", " ", "TestString", "\\\"" };
            foreach (string testValue in testValues)
            {
                // Parse
                Node node = TaronParser.Parse($"TestString = \"{testValue}\"");

                // Test type and size
                Assert.IsInstanceOfType(node, typeof(MapValue));
                MapValue mapNode = node.As<MapValue>();
                Assert.IsNotNull(mapNode);
                Assert.AreEqual(1, mapNode.Count);

                // Test property
                Assert.AreEqual("TestString", mapNode.Keys.First());
                TestUtils.TestPrimitiveProperty(mapNode, "TestString", testValue);
            }
        }

        /// <summary>
        /// Tests parsing of simple decimal key values
        /// </summary>
        [TestMethod]
        public void Basic_Decimals()
        {
            // Iterate each test value
            string[] testValues = new string[] { "0.1", "10.2", "5.43", "-400.3", "-0.5", "-0.0" };
            foreach (string testValue in testValues)
            {
                // Parse
                Node node = TaronParser.Parse($"TestNumber = {testValue}");

                // Test type and size
                Assert.IsInstanceOfType(node, typeof(MapValue));
                MapValue mapNode = node.As<MapValue>();
                Assert.IsNotNull(mapNode);
                Assert.AreEqual(1, mapNode.Count);

                // Test property
                Assert.AreEqual("TestNumber", mapNode.Keys.First());
                TestUtils.TestPrimitiveProperty(mapNode, "TestNumber", double.Parse(testValue));
            }

            // Test error on invalid numbers
            Exception ex = null;
            try
            {
                TaronParser.Parse("TestNumber = .2");
            }
            catch (Exception theEx)
            {
                ex = theEx;
            }
            finally
            {
                Assert.IsNotNull(ex);
            }
        }

        /// <summary>
        /// Tests parsing of simple integer key values
        /// </summary>
        [TestMethod]
        public void Basic_Integers()
        {
            // Iterate each test value
            string[] testValues = new string[] { "0", "10", "5", "3456", "-7", "-0", "-01234", "024" };
            foreach (string testValue in testValues)
            {
                // Parse
                Node node = TaronParser.Parse($"TestNumber = {testValue}");

                // Test type and size
                Assert.IsInstanceOfType(node, typeof(MapValue));
                MapValue mapNode = node.As<MapValue>();
                Assert.IsNotNull(mapNode);
                Assert.AreEqual(1, mapNode.Count);

                // Test property
                Assert.AreEqual("TestNumber", mapNode.Keys.First());
                TestUtils.TestPrimitiveProperty(mapNode, "TestNumber", int.Parse(testValue));
            }
        }

        /// <summary>
        /// Tests parsing of simple boolean key values
        /// </summary>
        [TestMethod]
        public void Basic_Booleans()
        {
            // Iterate each test value
            string[] testValues = new string[] { "true", "false" };
            foreach (string testValue in testValues)
            {
                // Parse
                Node node = TaronParser.Parse($"TestBoolean = {testValue}");

                // Test type and size
                Assert.IsInstanceOfType(node, typeof(MapValue));
                MapValue mapNode = node.As<MapValue>();
                Assert.IsNotNull(mapNode);
                Assert.AreEqual(1, mapNode.Count);

                // Test property
                Assert.AreEqual("TestBoolean", mapNode.Keys.First());
                TestUtils.TestPrimitiveProperty(mapNode, "TestBoolean", bool.Parse(testValue));
            }

            // Test error on invalid booleans
            Exception ex = null;
            try
            {
                TaronParser.Parse("TestBoolean = True");
                TaronParser.Parse("TestBoolean = False");
            }
            catch (Exception theEx)
            {
                ex = theEx;
            }
            finally
            {
                Assert.IsNotNull(ex);
            }
        }

        [TaronEnum("EnumTest", typeof(EnumTest))]
        public enum EnumTest
        {
            One = 1,
            Two = 2
        }

        /// <summary>
        /// Tests parsing of simple enum key value
        /// </summary>
        [TestMethod]
        public void Basic_Comment()
        {
            // Iterate each test value
            string[] testValues = new string[] { "// Test Comment" };
            foreach (string testValue in testValues)
            {
                // Parse
                Node node = TaronParser.Parse($" {testValue}");
            }
        }

        /// <summary>
        /// Tests parsing of simple enumeration key values
        /// </summary>
        [TestMethod]
        public void Basic_Enumerations()
        {
            // Iterate each test value
            string[] testValues = new string[] { "A.B", "Test.Thing", "   Whitespace  .   Yep " };
            foreach (string testValue in testValues)
            {
                // Parse
                Node node = TaronParser.Parse($"TestEnum = {testValue}");

                // Test type and size
                Assert.IsInstanceOfType(node, typeof(MapValue));
                MapValue mapNode = node.As<MapValue>();
                Assert.IsNotNull(mapNode);
                Assert.AreEqual(1, mapNode.Count);

                // Test property
                Assert.AreEqual("TestEnum", mapNode.Keys.First());
            }

            // Test error on invalid enums
            Exception ex = null;
            try
            {
                TaronParser.Parse("TestEnum = A");
            }
            catch (Exception theEx)
            {
                ex = theEx;
            }
            finally
            {
                Assert.IsNotNull(ex);
            }
        }

        /// <summary>
        /// Tests parsing of a group of keyvalues
        /// </summary>
        [TestMethod]
        public void Basic_Group()
        {
            // Parse
            Node node = TaronParser.Parse(TestUtils.ReusableMapTest);

            // Test type and size
            Assert.IsInstanceOfType(node, typeof(MapValue));
            MapValue mapNode = node.As<MapValue>();
            Assert.IsNotNull(mapNode);

            // Test content
            TestUtils.TestReusableMap(mapNode);
        }
    }
}
