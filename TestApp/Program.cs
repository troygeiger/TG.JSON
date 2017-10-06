﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TG.JSON;
using System.Windows.Forms;
using System.ComponentModel;

namespace TestApp
{
    class Program
    {
   

        [STAThread]
        static void Main(string[] args)
        {
            Test test = new Test();
            test.Numbers.Add(1);
            test.Bools.Add(true);
            test.Strings.Add("Hello World");
            List<Test> tests = new List<Test>() { test };
            JsonArray a = new JsonArray(tests.ToArray());
            tests.Clear();
            a.DeserializeInto(tests);

            //JsonObject obj = new JsonObject(new Test() { Hello = "Test" });
            //obj.SerializeObject(new Test2() { Monkey = "Banana" });

            //var t = obj.DeserializeObject<Test>();
            //var t2 = obj.DeserializeObject<Test2>();
            
            //obj.Properties.Test = "true";
            //bool b = obj.Properties.Test;
            //var i = a.FindAllObjects("i", null);
            //var s = obj.GetValueAs<string>("Hello");
            //(obj.Navigate("Parent/Child/GrandChild", true) as JsonObject)["Age"] = 15;
            return;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            
        }

        private static void Program_ValueChanged(object sender, EventArgs e)
        {
            
        }
    }

    class Test
    {
        public string Name { get; set; }

        [JsonIgnoreProperty]
        public DateTime? Date { get; set; }

        public int? Number { get; set; }
        //public Test2Collection Tests { get; } = new Test2Collection();

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
