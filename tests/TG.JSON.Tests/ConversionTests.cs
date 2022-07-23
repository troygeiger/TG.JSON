using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace TG.JSON.Tests
{
    [TestFixture]
    public class ConversionTests
    {

        /// <summary>
        /// If nothing throws an exception, I consider that a pass.
        /// </summary>
        [Test]
        [TestCase(5)]
        [TestCase(true)]
        [TestCase("Text")]
        public void TestJsonValueToInt(object src)
        {
            JsonValue value = JsonValue.GetValueFromObject(src);
            int intVal = value;
        }

        /// <summary>
        /// If nothing throws an exception, I consider that a pass.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="target"></param>
        [Test]
        [TestCase(new byte[] {0,3,2,5}, typeof(byte[]))]
        [TestCase(true, typeof(int))]
        [TestCase(null, typeof(bool))]
        public void TestChangeType(object data, Type target)
        {
            JsonValue json = JsonValue.GetValueFromObject(data);
            object backTo = Convert.ChangeType(json, target);
        }

        
    }

   
}
