using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
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
            o.SerializeObject(new Obj() { Name = "Jon", Hello = "World", YesNo = true }, true);
            propertyGrid1.SelectedObject = o;
            try
            {
                textBox1.DataBindings.Add(new Binding("Text", o, "Hello"));
            }
            catch (Exception)
            {
                
            }
            JsonObject proto = new JsonObject("Name", "Location", "Value", new JsonString(null) { EncryptValue = true }, "Number", 0, "Enabled", true
                , "Data", new JsonBinary());
            proto.SetPropertyTypeConverter("Data", typeof(JsonBinaryTypeConverter));
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
            [Category("Bla"), Description("Hello World")]
            public string Hello { get; set; }

            public string Name { get; set; }

            public bool YesNo { get; set; }

            [ReadOnly(true)]
            public bool ReadOnlyProperty { get; set; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool yesno = (bool)o["YesNo"];
            string values = arr.ToString();
        }
    }

    class JsonBinaryTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(Image);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            byte[] bytes = value as byte[];
            if (bytes == null) return new Bitmap(1, 1);
            if (bytes.Length == 0) return new Bitmap(1, 1);

            MemoryStream ms = new MemoryStream();

            ms.Write(bytes, 0, bytes.Length);
            ms.Position = 0;
            return Image.FromStream(ms);


        }

    }
}
