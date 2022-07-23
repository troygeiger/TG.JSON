using NUnit.Framework;
using TG.JSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TG.JSON.Tests
{
    [TestFixture()]
    public class JsonBinaryTests
    {
        [Test()]
        public void CloneTest()
        {
            JsonBinary binary = new JsonBinary(new byte[] { 1, 2, 3, 4 });
            JsonBinary copy = binary.Clone() as JsonBinary;
            Assert.AreEqual(binary.Value, copy.Value);
        }
    }
}