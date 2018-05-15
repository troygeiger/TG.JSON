using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TG.JSON.Tests
{
    [TestClass]
    public class NullTests
    {
        

        [TestMethod]
        public void JsonBooleanNullTest()
        {
             JsonBoolean json = null;
            Assert.IsNull(json, "Fails on IsNull");
            Assert.IsFalse(json != null, "Fails on != null");
        }

        [TestMethod]
        public void JsonStringNullTest()
        {
            JsonString json = null;
            Assert.IsNull(json, "Fails on IsNull");
            Assert.IsFalse(json != null, "Fails on != null");
        }

        [TestMethod]
        public void JsonNumberNullTest()
        {
            JsonNumber json = null;
            Assert.IsNull(json, "Fails on IsNull");
            Assert.IsFalse(json != null, "Fails on != null");
        }

        [TestMethod]
        public void JsonNullNullTest()
        {
            JsonNull json = null;
            Assert.IsNull(json, "Fails on IsNull");
            Assert.IsFalse(json != null, "Fails on != null");
        }

        [TestMethod]
        public void JsonArrayNullTest()
        {
            JsonArray json = null;
            Assert.IsNull(json, "Fails on IsNull");
            Assert.IsFalse(json != null, "Fails on != null");
        }

        [TestMethod]
        public void JsonObjectNullTest()
        {
            JsonObject json = null;
            Assert.IsNull(json, "Fails on IsNull");
            Assert.IsFalse(json != null, "Fails on != null");
        }
    }
}
