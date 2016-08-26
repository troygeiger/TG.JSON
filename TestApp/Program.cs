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
        class Test
        {
            public string Hello { get; set; }

            public bool Edger { get; set; }
        }

        class Test2
        {
            public string Monkey { get; set; }
        }

        [STAThread]
        static void Main(string[] args)
        {
            JsonObject obj = new JsonObject(new Test() { Hello = "Test" });
            obj.SerializeObject(new Test2() { Monkey = "Banana" });

            var t = obj.DeserializeObject<Test>();
            var t2 = obj.DeserializeObject<Test2>();

            return;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            
        }

        private static void Program_ValueChanged(object sender, EventArgs e)
        {
            
        }
    }
}
