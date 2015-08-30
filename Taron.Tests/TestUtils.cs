using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Taron.Model;
using Taron.Translator;

namespace Taron.Tests
{
    /// <summary>
    /// Contains testing utilities
    /// </summary>
    public static class TestUtils
    {
        /// <summary>
        /// Tests the primitive value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="valueNode"></param>
        /// <param name="expectedValue"></param>
        public static void TestPrimitiveValue<T>(ValueNode valueNode, T expectedValue)
        {
            Assert.IsInstanceOfType(valueNode, typeof(PrimitiveValue<T>));
            PrimitiveValue<T> testStringNodeStr = valueNode.As<PrimitiveValue<T>>();
            Assert.IsNotNull(testStringNodeStr);
            Assert.AreEqual(expectedValue, testStringNodeStr.Value);
        }

        /// <summary>
        /// Tests the primitive property on the specified map
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <param name="key"></param>
        /// <param name="expectedValue"></param>
        public static void TestPrimitiveProperty<T>(MapValue parent, string key, T expectedValue)
        {
            // Test presense of property
            ValueNode valueNode;
            Assert.IsTrue(parent.TryGetValue(key, out valueNode));
            Assert.IsNotNull(valueNode);

            // Test value of property
            TestPrimitiveValue(valueNode, expectedValue);
        }

        /// <summary>
        /// Tests the primitive typed property on the specified map
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void TestPrimitiveTypedProperty<T>(MapValue parent, string key, string typeName, T value)
        {
            // Test presense of property
            ValueNode valueNode;
            Assert.IsTrue(parent.TryGetValue(key, out valueNode));
            Assert.IsNotNull(valueNode);

            // Test value of property
            TestPrimitiveValue(valueNode, value);
            Assert.AreEqual(typeName, valueNode.TypeName);
        }

        /// <summary>
        /// Tests the enum value
        /// </summary>
        /// <param name="valueNode"></param>
        /// <param name="expectedFullValue"></param>
        public static void TestEnumValue(ValueNode valueNode, string expectedFullValue)
        {
            Assert.IsInstanceOfType(valueNode, typeof(EnumValue));
            EnumValue enumValue = valueNode.As<EnumValue>();
            Assert.IsNotNull(enumValue);
            Assert.AreEqual(expectedFullValue, enumValue.Full);
        }

        /// <summary>
        /// Tests the enum property on the specified map
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="key"></param>
        /// <param name="expectedFullValue"></param>
        public static void TestEnumProperty(MapValue parent, string key, string expectedFullValue)
        {
            // Test presense of property
            ValueNode valueNode;
            Assert.IsTrue(parent.TryGetValue(key, out valueNode));
            Assert.IsNotNull(valueNode);

            // Test value of property
            TestEnumValue(valueNode, expectedFullValue);
        }

        #region Basic Reusable Map Test

        [TaronEnum("EnumTest", typeof(EnumTest))]
        public enum EnumTest
        {
            One = 1,
            Two = 2
        }

        public const string ReusableMapTest = @"
            // Testing a decimal value
            DecimalVal = 10.0 
            // Testing a int value
            IntegerVal = 5 
            // Testing a string value
            StringVal = ""thingy""
            // Testing a boolean value
            BooleanVal = true 
            // Testing a enum value
            EnumValue = EnumTest.One
        ";
        public const string ReusableMapTest_Map = "{" + ReusableMapTest + "}";

        /// <summary>
        /// Tests the specified map against the signature of ReusableMapTest
        /// </summary>
        /// <param name="map"></param>
        public static void TestReusableMap(MapValue map)
        {
            Assert.AreEqual(5, map.Count);
            TestPrimitiveProperty(map, "DecimalVal", 10.0);
            TestPrimitiveProperty(map, "IntegerVal", 5);
            TestPrimitiveProperty(map, "StringVal", "thingy");
            TestPrimitiveProperty(map, "BooleanVal", true);
            TestEnumProperty(map, "EnumValue", "EnumTest.One");
        }

        public struct ReusableMapStruct
        {
            public double DecimalVal;
            public int IntegerVal;
            public string StringVal;
            public bool BooleanVal;
            // TODO: Enum

            public void Test()
            {
                Assert.AreEqual(10.0, DecimalVal);
                Assert.AreEqual(5, IntegerVal);
                Assert.AreEqual("thingy", StringVal);
                Assert.AreEqual(true, BooleanVal);
            }
        }

        #endregion

    }
}
