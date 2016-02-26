using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TG.JSON;

namespace TestApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            JsonObject o = new JsonObject();
            o.Add("Hello", "World");
            o.Add("Monkey", new JsonString());
            propertyGrid1.SelectedObject = o;
        }
    }
}
