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
        /// Tests the primitive property on the specified map
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void TestPrimitiveProperty<T>(MapValue parent, string key, T value)
        {
            // Test presense of property
            ValueNode testStringNode;
            Assert.IsTrue(parent.TryGetValue(key, out testStringNode));
            Assert.IsNotNull(testStringNode);

            // Test value of property
            Assert.IsInstanceOfType(testStringNode, typeof(PrimitiveValue<T>));
            PrimitiveValue<T> testStringNodeStr = testStringNode.As<PrimitiveValue<T>>();
            Assert.IsNotNull(testStringNodeStr);
            Assert.AreEqual(value, testStringNodeStr.Value);
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
            ValueNode testStringNode;
            Assert.IsTrue(parent.TryGetValue(key, out testStringNode));
            Assert.IsNotNull(testStringNode);

            // Test value of property
            Assert.IsInstanceOfType(testStringNode, typeof(PrimitiveValue<T>));
            PrimitiveValue<T> testStringNodeStr = testStringNode.As<PrimitiveValue<T>>();
            Assert.IsNotNull(testStringNodeStr);
            Assert.AreEqual(typeName, testStringNodeStr.TypeName);
            Assert.AreEqual(value, testStringNodeStr.Value);
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
