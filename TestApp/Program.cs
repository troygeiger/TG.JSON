using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TG.JSON;
using System.Windows.Forms;
using System.ComponentModel;
using System.Diagnostics;

namespace TestApp
{
    class Program
    {


        [STAThread]
        static void Main(string[] args)
        {
            Test test = new Test();
            test.Number = 32;
            test.Numbers.AddRange(new int[] { 3, 2, 1 });

            EncryptionHandler encryption = new EncryptionHandler("Hello World");
            JsonObject obj = new JsonObject(encryption);
            obj.SerializeObject(test);
            string s = obj.ToString(Formatting.Indented);
            obj = new JsonObject(s, encryption);

            Test test2 = obj.DeserializeObject<Test>();
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
            PerformanceTest();
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
            EncryptionHandler encryption = new EncryptionHandler("Hello World");

            Stopwatch stopwatch = new Stopwatch();
            JsonValue value;
            DoAgain:
            stopwatch.Reset();
            stopwatch.Start();
            string json = new JsonObject(test, encryption).ToString(Formatting.Indented);// new JsonObject(test).ToString(Formatting.Indented);
            stopwatch.Stop();
            Console.WriteLine($"Serialize Elapsed: {stopwatch.Elapsed.TotalMilliseconds} ms");
            stopwatch.Reset();
            stopwatch.Start();
            Test test2 = new JsonObject(json, encryption).DeserializeObject<Test>();
            stopwatch.Stop();
            Console.WriteLine($"Deserialize Elapsed: {stopwatch.Elapsed.TotalMilliseconds} ms");
            string cmd = Console.ReadLine();
            if (cmd == "") goto DoAgain;
        }

        private static void Program_ValueChanged(object sender, EventArgs e)
        {

        }
    }

    class Test
    {
        [JsonEncryptValue]
        public string Name { get; set; }

        [JsonEncryptValue]
        public DateTime? Date { get; set; }

        [JsonEncryptValue]
        public int Number { get; set; }
        //public Test2Collection Tests { get; } = new Test2Collection();

        [JsonEncryptValue]
        public List<int> Numbers { get; } = new List<int>();

        public List<string> Strings { get; } = new List<string>();

        public List<bool> Bools { get; } = new List<bool>();
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
        public string Name { get; set; }
    }
}
