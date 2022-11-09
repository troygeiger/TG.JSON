using NUnit.Framework;
using System;


namespace TG.JSON.Tests
{
    [TestFixture]
    public class EncryptionTests
    {

        [Test]
        public void RoundTripEncryptionTest()
        {
            JsonObject json = new JsonObject();
            json.GlobalEncryptionHandler = new TG.JSON.EncryptionHandler("MySecret");
            string secret = "I'm a secret.";
            json.Add("Secret", new JsonString(secret) { EncryptValue = true });
            string output = json.ToString();
            Console.WriteLine(output);
            Assert.IsFalse(output.Contains(secret));

            JsonObject json2 = new JsonObject(json, json.GlobalEncryptionHandler);

            Assert.AreEqual(secret, (string)json2["Secret"]);

        }

        [Test]
        public void RoundTripEncryptionMissingHandlerTest()
        {
            JsonObject json = new JsonObject();
            json.GlobalEncryptionHandler = new TG.JSON.EncryptionHandler("MySecret");
            string secret = "I'm a secret.";
            json.Add("Secret", new JsonString(secret) { EncryptValue = true });
            string output = json.ToString();

            JsonObject json2 = new JsonObject(json);

            Assert.AreNotEqual(secret, (string)json2["Secret"]);
            Console.WriteLine((string)json2["Secret"]);
        }

        [Test]
        public void SerializeEncryptTest()
        {
            EncryptClass obj = new EncryptClass() { MySecretProperty = "I'm a secret." };
            var handler = new EncryptionHandler("MySecret");
            JsonObject json = new JsonObject(obj, handler);
            Assert.IsTrue(((JsonString)json["MySecretProperty"]).EncryptValue);
        }

        public class EncryptClass
        {
            [JsonEncryptValue]
            public string? MySecretProperty { get; set; }
        }
    }
}
