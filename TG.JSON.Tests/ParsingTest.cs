using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TG.JSON;

namespace TG.JSON.Tests
{
    [TestFixture]
    public class ParsingTest
    {
        private string GetTestFile(string name)
        {
            return Path.Combine(TestContext.CurrentContext.TestDirectory, name);
        }

        [Test]
        public void ParsePeopleTest()
        {
            string jsonText = File.ReadAllText(GetTestFile("people.json"));

            JsonArray array = new JsonArray(jsonText);

            Assert.NotZero(array.Count);

            Assert.IsTrue(array[0].ValueType == JsonValueTypes.Object);

            JsonObject obj = array[0] as JsonObject;

            Assert.IsNotNull(obj["FirstName"], "FirstName doesn't exist or is missing.");

            Assert.IsNotNull(obj["LastName"], "LastName doesn't exist or is missing.");
        }

        [Test]
        public void RoundTripParseTest()
        {
            string jsonText = File.ReadAllText(GetTestFile("people.json"));

            JsonArray array = new JsonArray(jsonText);

            JsonArray array2 = new JsonArray(array.ToString());

            Assert.IsTrue(array == array2);
        }

        [Test]
        public void TryParseTest()
        {
            string jsonText = File.ReadAllText(GetTestFile("people.json"));

            JsonValue value;
            Assert.IsTrue(JsonValue.TryParse(jsonText, out value), "Failed to parse");

            Assert.IsTrue(value.ValueType == JsonValueTypes.Array);
        }
    }
}
