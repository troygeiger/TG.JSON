using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.CompilerServices;

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

        [Test]
        public void ArraySerializationTest()
        {
            JsonArray array = new JsonArray();
            array.Add(new JsonObject(new Person() { FirstName = "Name1" }));
            array.Add(new JsonObject(new Person() { FirstName = "Name2" }));
            Person[] items = array.DeserializeArray<Person[]>();
            Assert.IsTrue(items.Length == array.Count);
            for (int i = 0; i < items.Length; i++)
            {
                Assert.IsNotNull(items[i]);
                Assert.IsTrue(!string.IsNullOrEmpty(items[i].FirstName));
            }
        }

        [Test]
        public void CharTest()
        {
            CharProp item = new CharProp() { Value = 'L' };
            JsonObject json = new JsonObject(item);
            CharProp item2 = json.DeserializeObject<CharProp>();
            Assert.AreEqual(item.Value, item2.Value);
        }

        [Test]
        public void CharArrayTest()
        {
            var a1 = new char[] { 'A', 'B', 'C' };
            JsonArray array = new JsonArray(a1);
            var output = array.DeserializeArray<char[]>();
            Assert.AreEqual(a1, output);
        }

#if !NET20
        [Test]
        public void TestObservableCollection()
        {
            ObservableCollection<Person> people = new ObservableCollection<Person>();
            people.Add(new Person() { FirstName = "John", LastName = "Doe" });
            JsonArray array = new JsonArray(people);
            Assert.IsTrue(array.Count == 1);
            people = array.Deserialize<ObservableCollection<Person>>();
            Assert.IsTrue(people.Count == 1);
            Assert.IsTrue(people[0].FirstName == "John");
        }
#endif
        [Test]
        public void TestCustomCollection()
        {
            CustomPeopleCollection people = new CustomPeopleCollection();
            people.Add(new Person() { FirstName = "John", LastName = "Doe" });
            JsonArray array = new JsonArray(people);
            Assert.IsTrue(array.Count == 1);
            people = array.Deserialize<CustomPeopleCollection>();
            Assert.IsTrue(people.Count == 1);
            Assert.IsTrue(people[0].FirstName == "John");
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

    public class CharProp
    {
        public char Value { get; set; }
    }

    public class CustomPeopleCollection : CollectionBase
    {
        public void Add(Person person)
        {
            List.Add(person);
        }

        public Person this[int index]
        {
            get
            {
                return (Person)List[index];
            }
            set
            {
                List[index] = value;
            }
        }
    }
}
