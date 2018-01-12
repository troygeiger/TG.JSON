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
        JsonObject o;
        JsonArray arr = new JsonArray();
        public Form1()
        {
            InitializeComponent();
            o = new JsonObject("Hello", "World", "Name", "Troy", "YesNo", true);
            Obj co = o.DeserializeObject(typeof(Obj)) as Obj;
            propertyGrid1.SelectedObject = o;
            try
            {
                textBox1.DataBindings.Add(new Binding("Text", o, "Hello"));
            }
            catch (Exception)
            {
                
            }
            dataGridView1.DataSource = o;
        }

        public class Obj
        {
            public string Hello { get; set; }

            public string Name { get; set; }

            public bool YesNo { get; set; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool yesno = (bool)o["YesNo"];
        }
    }

}
