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
            o = new JsonObject();
            o.SerializeObjectWithAttributes(new Obj() { Name = "Jon", Hello = "World", YesNo = true });
            propertyGrid1.SelectedObject = o;
            try
            {
                textBox1.DataBindings.Add(new Binding("Text", o, "Hello"));
            }
            catch (Exception)
            {
                
            }
            JsonObject proto = new JsonObject("Name", "Location", "Value", new JsonString(null) { EncryptValue = true }, "Number", 0, "Enabled", true);
            proto.GlobalEncryptionKeyString = "Key";
            JsonArrayBindingSource source = new JsonArrayBindingSource(arr
                , proto);
            source.ListChanged += Source_ListChanged;
            dataGridView1.DataSource = source;
        }

        private void Source_ListChanged(object sender, ListChangedEventArgs e)
        {
            textBox2.Text = arr.ToString(Formatting.Indented);
        }

        public class Obj
        {
            [Category("Bla")]
            public string Hello { get; set; }

            public string Name { get; set; }

            public bool YesNo { get; set; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool yesno = (bool)o["YesNo"];
            string values = arr.ToString();
        }
    }

}
