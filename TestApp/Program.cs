﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TG.JSON;
using System.Windows.Forms;
using System.ComponentModel;
using System.Diagnostics;
using TG.JSON.Serialization;
using System.Security.Cryptography;

namespace TestApp
{
    class Program
    {


        [STAThread]
        static void Main(string[] args)
        {

            switch (3)
            {
                case 1:

                    Test test = new Test();
                    test.Number = 32;
                    test.Numbers.AddRange(new int[] { 3, 2, 1 });
                    test.InitMyObject();
                    test.SubTest.Add(new Test2() { Name = "Sub Test" });
                    test.SubClass.Value = "Hello, world";

                    EncryptionHandler encryption = new EncryptionHandler("Hello World");
                    JsonObject obj = new JsonObject(encryption);
                    obj.SerializeObject(test, new JsonSerializationOptions(5, false, false, null, new string[] { "SubClass" }));

                    string s = obj.ToString(Formatting.Indented);
                    obj = new JsonObject(s, encryption);

                    Test test2 = obj.DeserializeObject<Test>();

                    break;
                case 2:
                    PerformanceTest();
                    break;
                case 3:

                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new Form1());
                    break;
                case 4:
                    JsonArray a = new JsonArray();
                    a.Add(new JsonArray() { 1, 2, 3 });
                    a.Add(new JsonArray() { 4, 5, 6 });
                    a.Add(new JsonObject("Hello", "World", "Age", 33));
                    string sa = a.ToString();
                    a = new JsonArray(sa);
                    string sa2 = a.ToString();
                    break;
                case 5:

                    break;
                default:
                    break;
            }

        }

        static void PerformanceTest()
        {
            Test test = new Test()
            {
                Name = "TEST",
                Date = DateTime.Now,
                Number = 1,
                Numbers = { 1, 2, 3, 4, 5 },
                Strings = { "Hello", "World" }
            };
            Console.WriteLine("Press Ctrl+c to Exit; Enter to run again.");
            //EncryptionHandler encryption = new EncryptionHandler("Hello World");

            Stopwatch stopwatch = new Stopwatch();
            JsonValue value;
            string cmd = "";
            do
            {
                stopwatch.Reset();
                stopwatch.Start();
                string json = new JsonObject(test).ToString(Formatting.Indented);// new JsonObject(test).ToString(Formatting.Indented);
                stopwatch.Stop();
                Console.WriteLine($"Serialize Elapsed: {stopwatch.Elapsed.TotalMilliseconds} ms");
                stopwatch.Reset();
                stopwatch.Start();
                Test test2 = new JsonObject(json).DeserializeObject<Test>();
                stopwatch.Stop();
                Console.WriteLine($"Deserialize Elapsed: {stopwatch.Elapsed.TotalMilliseconds} ms");

                stopwatch.Reset();
                stopwatch.Start();
                json = Newtonsoft.Json.JsonConvert.SerializeObject(test);
                stopwatch.Stop();
                Console.WriteLine($"Newtonsoft Serialize Elapsed: {stopwatch.Elapsed.TotalMilliseconds} ms");
                stopwatch.Reset();
                stopwatch.Start();
                test2 = Newtonsoft.Json.JsonConvert.DeserializeObject<Test>(json);
                stopwatch.Stop();
                Console.WriteLine($"Newtonsoft Deserialize Elapsed: {stopwatch.Elapsed.TotalMilliseconds} ms");

                test = new Test()
                {
                    Name = "TEST",
                    Date = DateTime.Now,
                    Number = 1,
                    Numbers = { 1, 2, 3, 4, 5 },
                    Strings = { "Hello", "World" }
                };


                cmd = Console.ReadLine();
            } while (cmd == "");
        }

        private static void Program_ValueChanged(object sender, EventArgs e)
        {

        }
    }

    class Test
    {
        public Test()
        {

        }

        public void InitMyObject()
        {
            MyObject = new JsonObject("Hello", "World");
        }

        [JsonProperty]
        private JsonObject MyObject { get; set; }

        //      [JsonEncryptValue]
        public string Name { get; set; }

        //        [JsonEncryptValue]
        public DateTime? Date { get; set; }

        //[JsonEncryptValue, DisplayName("Some Number")]
        public int Number { get; set; }
        //public Test2Collection Tests { get; } = new Test2Collection();

        //        [JsonEncryptValue]
        public List<int> Numbers { get; } = new List<int>();

        public List<string> Strings { get; } = new List<string>();

        public List<bool> Bools { get; } = new List<bool>();

        public List<Test2> SubTest { get; } = new List<Test2>();

        public MyClassToEncrypt SubClass { get; set; } = new MyClassToEncrypt();

    }

    class Test2Collection : System.Collections.DictionaryBase
    {
        public void Add(string key, Test2 item)
        {
            Dictionary.Add(key, item);
        }

        public Test2 this[string i]
        {
            get { return Dictionary[i] as Test2; }
        }
    }

    class Test2
    {
        [Category("Test")]
        public string Name { get; set; }
    }

    class MyClassToEncrypt
    {
        [JsonEncryptValue]
        public string Value { get; set; }
    }
}
