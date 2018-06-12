using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace TG.JSON.Tests
{

    [TestFixture]
    public class SerializationTests
    {
        private string GetTestFile(string name)
        {
            return Path.Combine(TestContext.CurrentContext.TestDirectory, name);
        }

        [Test]
        public void SerializeTest()
        {
            JsonArray json = new JsonArray(File.ReadAllText(GetTestFile("people.json")));
            List<Person> people = new List<Person>();
            json.DeserializeInto(people);
            Assert.IsTrue(people.Count == 1);

            Assert.AreEqual(people[0].FirstName, "John");
            Assert.AreEqual(people[0].LastName, "Doe");
        }

        [Test]
        public void SerializeRoundTripTest()
        {
            JsonArray json = new JsonArray(File.ReadAllText(GetTestFile("people.json")));
            List<Person> people = new List<Person>();
            json.DeserializeInto(people);
            Assert.IsTrue(people.Count == 1);

            JsonArray json2 = new JsonArray(people);
            Assert.IsTrue(json == json2);
        }
    }

    public class Person
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int Age { get; set; }

        public bool Works { get; set; }

        public Person Spouse { get; set; }

        public List<Person> Children { get; } = new List<Person>();
    }
}
