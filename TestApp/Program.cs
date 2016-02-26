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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            JsonObject obj = new JsonObject("Hello", "World");
            var pd = TypeDescriptor.GetProperties(obj);
            
        }

    }
}
