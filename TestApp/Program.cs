using System;
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
            //JsonObject obj = new JsonObject(new Test() { Hello = "Test" });
            //obj.SerializeObject(new Test2() { Monkey = "Banana" });

            //var t = obj.DeserializeObject<Test>();
            //var t2 = obj.DeserializeObject<Test2>();

            JsonObject oj = new JsonObject(
                "Name", null,
                "Date", null,
                "Number", null
                );
            
            Test t = oj.DeserializeObject<Test>();
            t.Date = DateTime.Now;

            oj = new JsonObject(t);
            //obj.Properties.Test = "true";
            //bool b = obj.Properties.Test;
            //var i = a.FindAllObjects("i", null);
            //var s = obj.GetValueAs<string>("Hello");
            //(obj.Navigate("Parent/Child/GrandChild", true) as JsonObject)["Age"] = 15;
            //return;
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

        public DateTime? Date { get; set; }

        public int? Number { get; set; }
        //public Test2Collection Tests { get; } = new Test2Collection();
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
