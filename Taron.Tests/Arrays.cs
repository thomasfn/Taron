using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;

using Taron;
using Taron.Model;

namespace Taron.Tests
{
    /// <summary>
    /// Contains tests for arrays
    /// </summary>
    [TestClass]
    public class Arrays
    {
        /// <summary>
        /// Tests parsing of an empty array
        /// </summary>
        [TestMethod]
        public void Array_Empty()
        {
            // Parse
            Node node = TaronParser.Parse($"TestArray [ ]");

            // Test type and size
            Assert.IsInstanceOfType(node, typeof(MapValue));
            MapValue mapNode = node.As<MapValue>();
            Assert.IsNotNull(mapNode);
            Assert.AreEqual(1, mapNode.Count);

            // Retrieve the array
            ValueNode arrayValueNode;
            Assert.IsTrue(mapNode.TryGetValue("TestArray", out arrayValueNode));
            Assert.IsNotNull(arrayValueNode);
            Assert.IsInstanceOfType(arrayValueNode, typeof(ArrayValue));
            ArrayValue arrayValue = arrayValueNode.As<ArrayValue>();
            Assert.IsNotNull(arrayValue);

            // Test it's content
            Assert.AreEqual(0, arrayValue.Count);
        }

        /// <summary>
        /// Tests parsing of an integer array
        /// </summary>
        [TestMethod]
        public void Array_Integer()
        {
            // Test little array
            TestArray(new int[] { 2 });

            // Test big array
            TestArray(new int[] { 0, 10, 50, -4, -0, -53 });
        }

        /// <summary>
        /// Tests parsing of an decimal array
        /// </summary>
        [TestMethod]
        public void Array_Decimal()
        {
            // Test little array
            TestArray(new double[] { 8.7 });

            // Test big array
            TestArray(new double[] { 0.1, 10.777, 50.4, -4.2, -0.92, -100.8111 });
        }

        /// <summary>
        /// Tests parsing of a string array
        /// </summary>
        [TestMethod]
        public void Array_String()
        {
            // Test little array
            TestArray(new string[] { "" });

            // Test big array
            TestArray(new string[] { "hello", "STRINGS", " ", "b", "", "\\\"" });
        }

        /// <summary>
        /// Tests parsing of a boolean array
        /// </summary>
        [TestMethod]
        public void Array_Boolean()
        {
            // Test little array
            TestArray(new bool[] { true });

            // Test big array
            TestArray(new bool[] { true, false, true, true, false, true, false, false });
        }

        /// <summary>
        /// Tests the specified array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="testValues"></param>
        private void TestArray<T>(T[] testValues)
        {
            // Parse
            Node node;
            if (typeof(T) == typeof(string))
            {
                node = TaronParser.Parse($"TestArray [ {string.Join(", ", testValues.Select(s => $"\"{s}\"")) } ]");
            }
            else if (typeof(T) == typeof(bool))
            {
                node = TaronParser.Parse($"TestArray [ {string.Join(", ", testValues.Select(s => s.ToString().ToLowerInvariant())) } ]");
            }
            else
            {
                node = TaronParser.Parse($"TestArray [ {string.Join(", ", testValues)} ]");
            }

            // Test type and size
            Assert.IsInstanceOfType(node, typeof(MapValue));
            MapValue mapNode = node.As<MapValue>();
            Assert.IsNotNull(mapNode);
            Assert.AreEqual(1, mapNode.Count);

            // Retrieve the array
            ValueNode arrayValueNode;
            Assert.IsTrue(mapNode.TryGetValue("TestArray", out arrayValueNode));
            Assert.IsNotNull(arrayValueNode);
            Assert.IsInstanceOfType(arrayValueNode, typeof(ArrayValue));
            ArrayValue arrayValue = arrayValueNode.As<ArrayValue>();
            Assert.IsNotNull(arrayValue);

            // Test it's content
            Assert.AreEqual(testValues.Length, arrayValue.Count);
            for (int i = 0; i < testValues.Length; i++)
            {
                TestUtils.TestPrimitiveValue(arrayValue[i], testValues[i]);
            }
        }

    }
}
