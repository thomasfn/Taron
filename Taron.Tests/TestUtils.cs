using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Taron.Model;

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

        #region Basic Reusable Map Test

        public const string ReusableMapTest = "DecimalVal = 10.0 IntegerVal = 5 StringVal = \"thingy\" BooleanVal = true";
        public const string ReusableMapTest_Map = "{" + ReusableMapTest + "}";

        /// <summary>
        /// Tests the specified map against the signature of ReusableMapTest
        /// </summary>
        /// <param name="map"></param>
        public static void TestReusableMap(MapValue map)
        {
            Assert.AreEqual(4, map.Count);
            TestPrimitiveProperty(map, "DecimalVal", 10.0);
            TestPrimitiveProperty(map, "IntegerVal", 5);
            TestPrimitiveProperty(map, "StringVal", "thingy");
            TestPrimitiveProperty(map, "BooleanVal", true);
        }

        #endregion

    }
}
