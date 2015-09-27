namespace TG.JSON
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml.Serialization;
    using System.Reflection;

	/// <summary>
	/// Represents a JSON Object. Ex. { "Hello" : "World" }
	/// </summary>
	/// <example>
	/// <code>
	///		class TestClass
	///		{
	///			static void Main()
	///			{
	///				<see cref="JsonObject"/> json = new <see cref="JsonObject"/>("cars", 1000, "isTrue", true);
	///				json.Add("Hello", "World");
	///				
	///				if ((bool)json["isTrue"])
	///				{
	///					int cars = (int)json["cars"];
	///					Console.Write(cars.ToString());
	///				}
	///				
	///				Console.Write(json.ToString());
	///			}
	///		}
	///	</code>
	/// </example>
	///  <remarks>
	/// <list type="definition">
	///	<item>
	///	<term>Viewing with a PropertyGrid</term>
	///	<description>When viewing a JsonObject with a PropertyGrid, all entries within the JsonObject display as properties.
	/// You can omit an entry by adding an underscore prefix. Ex. _myHiddenProperty</description>
	///	</item>
	/// <item>
	/// <term>Special Properties</term>
	/// <description>
	/// <para>Adding an item with a property "_category" and a JsonString value will set the PropertyDescriptor Category to that value.</para>
	/// <para>Adding an item with a property "_description[item key] and a JsonString value will add a description to key within the brackets.</para>
	/// <para>Adding an item with a property "_defaultValue[item key] and a JsonString value will add set the DefaultValue property of the PropertyDescriptor that will be used for resetting the value.</para>
	/// </description>
	/// </item>
	/// </list>
	/// </remarks>
#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough()]
#endif
	[TypeConverter(typeof(ComponentConverter))]
    [Serializable]
    public sealed class JsonObject : JsonValue, ICustomTypeDescriptor, INotifyPropertyChanged, ISerializable, IXmlSerializable, IEnumerable
    {
        #region Fields

        //Type myType;
        Dictionary<string, JsonValue> propertyValue = new Dictionary<string, JsonValue>();
		static Dictionary<Type, PropertyInfo[]> propertyCache = new Dictionary<Type, PropertyInfo[]>();
		static object locker = new object();

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public JsonObject()
        {
            //myType = GetType();
        }

        /// <summary>
        /// Initializes a new instance of <see cref="JsonObject"/> that parses the specified stringified JSON object.
        /// </summary>
        /// <param name="json">JSON Object formatted string. Ex. { "Hello" : "World" } </param>
        /// <example>
        /// <code>
        /// JsonObject json = new JsonObject("{ \"Hello\" : \"World\" }");
        /// </code>
        /// </example>
        public JsonObject(string json)
            : this()
        {
            this.InternalParser(json);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="JsonObject"/> that is populated with an initial property name and string value.
        /// </summary>
        /// <param name="property">A unique property name used for the initial entry.</param>
        /// <param name="value">A <see cref="System.String"/> value for the initial entry.</param>
        /// <example>
        ///	<code>
        ///	JsonObject json = new JsonObject("{\"Hello\": \"World\"}");
        ///	</code>
        /// </example>
        public JsonObject(string property, string value)
            : this()
        {
            this.Add(property, value);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="JsonObject"/> with a range of property/value pairs in an array.
        /// </summary>
        /// <example>
        /// <code>
        /// JsonObject json = new JsonObject(
        ///		"hello", "world",
        ///		"awesome", true,
        ///		"IAM", 35);
        /// </code>
        /// </example>
        /// <param name="propertiesValues">A series of property string/value arguments using a "property, value, property, value...." pattern.</param>
        public JsonObject(params object[] propertiesValues)
            : this()
        {
            this.AddRange(propertiesValues);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="JsonObject"/> that serializes the specified object.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
		public JsonObject(object obj)
            : this()
        {
            SerializeObject(obj);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="JsonObject"/> using the specified <see cref="JsonReader"/> to parse a stringified JSON object.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> that will be used to parse a JSON string.</param>
		public JsonObject(JsonReader reader)
        {
            InternalParser(reader);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="JsonObject"/> using for deserialization.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        JsonObject(SerializationInfo info, StreamingContext context)
            : this()
        {
            InternalParser(info.GetString("Value"));
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// Event that is called when a property has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        /// <summary>
        /// Get the quantity of properties in the current <see cref="JsonObject"/>.
        /// </summary>
        public int Count
        {
            get { return propertyValue.Count; }
        }

		/// <summary>
		/// Gets or Sets a <see cref="JsonObject"/> property attributes for properties within the current <see cref="JsonObject"/>.
		/// </summary>
		/// <remarks>
		/// <para>
		/// If a table is defined, this property returns null.
		/// </para>
		/// <para>
		/// The AttributesTable is structured like the following:
		/// {
		///		"PropertyName" : {
		///			"Category": "General",
		///			"Description": "This tells something about the property."
		///			"DefaultValue": null
		///			}
		/// }
		/// </para>
		/// </remarks>
		public JsonObject AttributesTable
		{
			get
			{
				return this["_attributesTable"] as JsonObject;
			}
			set
			{
				this["_attributesTable"] = value;
			}
		}

        /// <summary>
        /// Get a string array of all keys.
        /// </summary>
        public string[] Properties
        {
            get
            {
                string[] k = new string[propertyValue.Count];
                propertyValue.Keys.CopyTo(k, 0);
                return k;
            }
            set
            {
                if (value != null)
                {
                    foreach (var cur in Properties)
                    {
                        foreach (var item in value)
                        {
                            if (cur == item)
                                continue;
                        }
                        Remove(cur);
                    }
                    Regex rex = new Regex(@"([a-zA-Z]+)\(([a-zA-Z]+)\)|([a-zA-Z]+)");
                    foreach (var item in value)
                    {
                        if (!propertyValue.ContainsKey(item))
                        {
                            var m = rex.Match(item.Trim());
                            if (m.Success)
                            {
                                if (m.Groups[1].Length > 0 && m.Groups[2].Length > 0)
                                {
                                    JsonValue v;
                                    switch (m.Groups[2].Value.ToLower())
                                    {
                                        case "jsonarray":
                                        case "array":
                                            v = new JsonArray();
                                            break;
                                        case "jsonbinary":
                                        case "binary":
                                            v = new JsonBinary();
                                            break;
                                        case "jsonboolean":
                                        case "boolean":
                                        case "bool":
                                            v = new JsonBoolean();
                                            break;
                                        case "jsonnumber":
                                        case "number":
                                            v = new JsonNumber();
                                            break;
                                        case "jsonobject":
                                        case "object":
                                            v = new JsonObject();
                                            break;
                                        default:
                                            v = new JsonString();
                                            break;
                                    }
                                    internalAdd(m.Groups[1].Value, v);
                                }
                                else
                                    internalAdd(m.Value, new JsonString());
                            }
                        }
                    }

                }

            }
        }

        /// <summary>
        /// Get a <see cref="JsonValue"/> array of values.
        /// </summary>
        public JsonValue[] Values
        {
            get
            {
                JsonValue[] v = new JsonValue[propertyValue.Count];
                propertyValue.Values.CopyTo(v, 0);
                return v;
            }
        }

        /// <summary>
        /// Returns <see cref="JsonValueTypes.Object"/>
        /// </summary>
		public override JsonValueTypes ValueType
        {
            get { return JsonValueTypes.Object; }
        }

        //internal bool ShowProperties { get; set; }

        #endregion Properties

        #region Indexers

        /// <summary>
        /// Gets or sets associated with the specified property.
        /// </summary>
        /// <param name="property">The property string of the value to get or set. This is case sensitive.</param>
        /// <returns>
        /// The <see cref="JsonValue"/> associated with the specified property.
        /// If the specified property is not found, a null value is returned and a set operation creates a new entry with the specified property.
        /// </returns>
        public JsonValue this[string property]
        {
            get
            {
                if (propertyValue.ContainsKey(property))
                    return propertyValue[property];

                return null;
            }
            set
            {
                if (propertyValue.ContainsKey(property))
                {
                    propertyValue[property] = value;
                    OnValueChanged();
                    OnPropertyChanged(property);
                }
                else
                    Add(property, value);
                
            }
        }

        #endregion Indexers

        #region Methods

        /// <summary>
        /// Creates a <see cref="JsonObject"/> by passing an array starting with a property string followed by a value.
        /// </summary>
        /// <code>
        /// JsonObject obj = JsonObject.Create("hello", "world", "number", 1, "bool", true);
        /// </code>
        /// <param name="propertiesValues">A series of property string/value arguments using a "property, value, property, value...." pattern.</param>
        /// <returns>A new instance of <see cref="JsonObject"/></returns>
        public static JsonObject Create(params object[] propertiesValues)
        {
            return new JsonObject(propertiesValues);
        }

        /// <summary>
        /// A static method for creating a JsonObject using provided JSON object string.
        /// </summary>
        /// <param name="json">A JSON object string</param>
        /// <returns>A new instance of <see cref="JsonObject"/> based on the parsed JSON string.</returns>
        /// <example>
        /// <code>
        /// class MyClass
        /// {
        ///		static void Main()
        ///		{
        ///			JsonObject json = JsonObject.Parse("{ \"Hello\" : \"World\" , \"IsAwesome\" : true }");
        ///			Console.Write(json.ToString(false));
        ///		}
        /// }
        /// </code>
        /// </example>
        public static JsonObject Parse(string json)
        {
            return new JsonObject(json);
        }

		/// <summary>
		/// Adds a new entry to the current JsonObject's collection.
		/// </summary>
		/// <param name="property">A unique key used for the new entry.</param>
		/// <param name="value">A <see cref="JsonValue"/> value for the new entry.</param>
		/// <example>
		/// <code>
		/// JsonObject json = New JsonObject();
		/// json.Add("Hello", new JsonArray("[ 1, 2, true, false]"));
		/// json.Add("IsAwesome", true);
		///
		/// //This method returns itself. This makes possible in-line calling.
		/// JsonObject j = new JsonObject("hello", "world");
		/// j.Remove("hello").Add("quote", "Everything is Awesome!");
		/// </code>
		/// </example>
		public JsonObject Add(string property, JsonValue value)
        {
            this.internalAdd(property, value);
            OnValueChanged();
            OnPropertyChanged(property);
            return this;
        }

		/// <summary>
		/// Add a range of property/value pairs in an array.
		/// </summary>
		/// <example>
		/// <code>
		/// JsonObject json = new JsonObject();
		/// json.AddRange(
		///		"hello", "world",
		///		"awesome", true,
		///		"IAM", 1);
		///
		/// // This method returns itself. This makes possible in-line calling.
		/// JsonObject j = new JsonObject("hello", "world");
		/// j.Remove("hello").Add("quote", "Everything is Awesome!");
		/// </code>
		/// </example>
		/// <param name="propertiesValues">A series of property string/value arguments using a "property, value, property, value...." pattern.</param>
		public JsonObject AddRange(params object[] propertiesValues)
        {
            if (propertiesValues == null)
                return this;
            decimal d = (decimal)propertiesValues.Length / 2;
            decimal b = Math.Floor(d);
            if (b < d)
                throw new ArgumentOutOfRangeException("keysValues");
            for (int i = 0; i < propertiesValues.Length; i += 2)
            {
                if (propertiesValues[i] is string)
                    this.internalAdd((string)propertiesValues[i], ValueFromObject(propertiesValues[i + 1]));
            }
            OnValueChanged();
            return this;
        }

        /// <summary>
        /// Clears all entries from the collection.
        /// </summary>
        public JsonObject Clear()
        {
            propertyValue.Clear();
            OnValueChanged();
            return this;
        }

        /// <summary>
        /// Creates a new instance of <see cref="JsonObject"/> with an exact copy of it's properties.
        /// </summary>
        /// <remarks>
        /// This method calls the <see cref="ToString"/> method and passes the JSON string to the constructor of a new <see cref="JsonObject"/>, thus parsing the JSON string to the new <see cref="JsonObject"/>.
        /// </remarks>
        /// <returns>The copy of the <see cref="JsonObject"/>.</returns>
		public override JsonValue Clone()
        {
            return new JsonObject(this.ToString());
        }

        /// <summary>
        /// Determines if the provided property exists in the current JsonObject's collection.
        /// </summary>
        /// <param name="property">A <see cref="string"/> property to search for. This is case sensitive.</param>
        /// <returns>Returns true if the property exists; otherwise false.</returns>
        public bool ContainsProperty(string property)
        {
            return propertyValue.ContainsKey(property);
        }

        /// <summary>
        /// Creates an instance of the specified <see cref="Type"/> then populates matching properties from this <see cref="JsonObject"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> that should be created from the <see cref="JsonObject"/>.</param>
        /// <returns>A new instance of <paramref name="type"/>.</returns>
		public object DeserializeObject(Type type)
        {
            object obj = null;
            if (type == typeof(System.Drawing.Color))
            {
                if (this.ContainsProperty("color"))
                    obj = System.Drawing.ColorTranslator.FromHtml((string)this["color"]);
                else
                    obj = System.Drawing.Color.Black;
            }
            else
            {
                if (type == typeof(object) && this.ContainsProperty("_type"))
                {
                    Regex rex = new Regex(@"^([^,]+),\s([^,]+),\sVersion=(\d{1,}\.\d{1,}\.\d{1,}\.\d{1,}),\sCulture=(\w+),\sPublicKeyToken=(\w+)");
                    Match m = rex.Match((string)this["_type"]);
                    if (m.Success)
                        obj = Activator.CreateInstance(m.Groups[2].Value, m.Groups[1].Value).Unwrap();

                }
                else
                    obj = Activator.CreateInstance(type);
                foreach (var property in GetTypeProperties(type))
                {
                    if (this.ContainsProperty(property.Name))
                        DeserializeJsonValueInto(property, obj, this[property.Name]);
                }
            }
            return obj;
        }

        /// <summary>
        /// Creates an instance of the specified <see cref="Type"/> then populates matching properties from this <see cref="JsonObject"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> that should be created from the <see cref="JsonObject"/>.</typeparam>
        /// <returns>A new instance of <typeparamref name="T"/>.</returns>
        public T DeserializeObject<T>()
        {
            return (T)DeserializeObject(typeof(T));
        }

        /// <summary>
        /// Populates the properties from this <see cref="JsonObject"/> to matching properties of the provided object.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to deserialize into.</param>
        public void DeserializeIntoObject(object obj)
        {
            if (obj == null)
                return;

            foreach (var property in GetTypeProperties(obj.GetType()))
            {
                if (this.ContainsProperty(property.Name))
                    DeserializeJsonValueInto(property, obj, this[property.Name]);
            }

        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="JsonObject"/>'s property/value.
        /// </summary>
		public IEnumerator GetEnumerator()
        {
            return propertyValue.GetEnumerator();
        }

        /// <summary>
        /// Returns the type of pattern of the specified property.
        /// </summary>
        /// <param name="property">The property to examine for the type.</param>
		public JsonValueTypes GetPropertyType(string property)
        {
            if (ContainsProperty(property))
                return this[property].ValueType;
            else
                return JsonValueTypes.Null;
        }

        AttributeCollection ICustomTypeDescriptor.GetAttributes()
        {
            return new AttributeCollection(new Attribute[0]);
        }

        string ICustomTypeDescriptor.GetClassName()
        {
            return this.GetType().Name;
        }

        string ICustomTypeDescriptor.GetComponentName()
        {
            return this.GetType().Name;
        }

        TypeConverter ICustomTypeDescriptor.GetConverter()
        {
            return null;
        }

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
        {
            return null;
        }

        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
        {
            return null;
        }

        object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
        {
            return null;
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
        {
            return new EventDescriptorCollection(new EventDescriptor[0]);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return new EventDescriptorCollection(new EventDescriptor[0]);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            return new PropertyDescriptorCollection(getProperties());
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            return new PropertyDescriptorCollection(getProperties());
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Value", this.ToString());
        }

        System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
        {
            InternalParser(reader.ReadString());
        }

        void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteCData(this.ToString());
        }

        /// <summary>
        /// Removes an entry using the provided key string value.
        /// </summary>
        /// <example>
        /// //This method returns itself. This makes possible in-line calling.
        /// JsonObject j = new JsonObject("hello", "world");
        /// j.Remove("hello").Add("quote", "Everything is Awesome!");
        /// </example>
        /// <param name="property">A <see cref="System.String"/> key value.</param>
        public JsonObject Remove(string property)
        {
            propertyValue.Remove(property);
            OnValueChanged();
            return this;
        }

        /// <summary>
        /// Serializes the properties, of the specified object, into this <see cref="JsonObject"/>.
        /// </summary>
        /// <example>
        /// //This method returns itself. This makes possible in-line calling.
        /// JsonObject j = new JsonObject("hello", "world");
        /// j.Remove("hello").Add("quote", "Everything is Awesome!");
        /// </example>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>The current instance of <see cref="JsonObject"/>. (Returns itself)</returns>
		public JsonObject SerializeObject(object obj)
        {
            return SerializeObject(obj, int.MaxValue);
        }

        /// <summary>
        /// Serializes the properties, of the specified object, into this <see cref="JsonObject"/>.
        /// </summary>
        /// <example>
        /// //This method returns itself. This makes possible in-line calling.
        /// JsonObject j = new JsonObject("hello", "world");
        /// j.Remove("hello").Add("quote", "Everything is Awesome!");
        /// </example>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="maxDepth">The maximum property depth that should be serialized.</param>
        /// <param name="ignoreProperties">Property names that should be ignored when serializing.</param>
        /// <returns>The current instance of <see cref="JsonObject"/>. (Returns itself)</returns>
		public JsonObject SerializeObject(object obj, int maxDepth, params string[] ignoreProperties)
        {
            if (obj == null)
                return this;
            List<string> ignore = new List<string>(ignoreProperties);

            foreach (var property in GetTypeProperties(obj.GetType()))
            {
                if (ignore.Contains(property.Name))
                    continue;
                if (property.PropertyType == typeof(object))
                {
                    object pval = property.GetValue(obj, null);
                    JsonValue v = ValueFromObject(pval, maxDepth, ignoreProperties);
                    if (v.ValueType == JsonValueTypes.Object)
                    {
                        ((JsonObject)v).internalAdd("_type", pval.GetType().AssemblyQualifiedName);
                    }

                    this.internalAdd(property.Name, v);
                }
                else
                    this.internalAdd(property.Name, ValueFromObject(property.GetValue(obj, null), maxDepth, ignoreProperties));
            }
            return this;
        }

		/// <summary>
		/// Sets the property details for the given property to the _detailsTable <see cref="JsonObject"/>.
		/// </summary>
		/// <param name="property">The name of the property that should have its details set.</param>
		/// <param name="category">The Category of the property.</param>
		/// <param name="description">The Description of the property.</param>
		/// <param name="defaultValue">The DefaultValue of the property.</param>
		public void SetPropertyAttributes(string property, string category, string description, JsonValue defaultValue)
		{
			if (string.IsNullOrEmpty(property))
				return;
			JsonObject table = AttributesTable;
			if (table == null)
			{
				table = new JsonObject();
				AttributesTable = table;
			}
			JsonObject details = table[property] as JsonObject ?? new JsonObject();
			
			details["Category"] = category;
			details["Description"] = description;
			details["DefaultValue"] = defaultValue;
			table[property] = details;
		}
		
        /// <summary>
        /// Sort the <see cref="JsonObject"/> by property. 
        /// </summary>
		public void Sort()
        {
            Sort(ListSortDirection.Ascending);
        }

        /// <summary>
        /// Sort the <see cref="JsonObject"/> by property. 
        /// </summary>
        /// <param name="sortOrder">The order in which to sort the properties.</param>
		public void Sort(ListSortDirection sortOrder)
        {
            List<string> k = new List<string>(this.Count);
            k.AddRange(this.Properties);
            k.Sort();
            if (sortOrder == ListSortDirection.Descending)
                k.Reverse();
            Dictionary<string, JsonValue> newKeyValue = new Dictionary<string, JsonValue>(this.Count);
            foreach (string key in k)
                newKeyValue.Add(key, propertyValue[key]);
            propertyValue = newKeyValue;
            OnValueChanged();
        }

        /// <summary>
        /// Returns the JSON string generated by all key/values associated with this <see cref="TG.JSON.JsonObject"/>.
        /// </summary>
        public override string ToString()
        {
            return this.ToString(Formatting.Compressed);
        }

        /// <summary>
        /// Attempts to retrieve a value with the specified property. If the property does not exist, the <paramref name="defaultValue"/> is returned.
        /// </summary>
        /// <param name="property">The name of the property to get.</param>
        /// <param name="defaultValue">The value to return if the property does not exist.</param>
		public JsonValue TryGetValue(string property, JsonValue defaultValue)
        {
            if (propertyValue.ContainsKey(property))
                return propertyValue[property];
            return defaultValue;
        }

        internal JsonObject internalAdd(string property, JsonValue value)
        {
            if (value == null)
                value = new JsonNull();
            value.Parent = this;
            //key = Unescape(key);
            if (!propertyValue.ContainsKey(property))
                propertyValue.Add(property, value);
            else
                propertyValue[property] = value;
            return this;
        }

        /*
		/// <summary>
		/// Returns the JSON string generated by all key/values associated with this <see cref="TGSolutions.JSON.JsonObject"/>.
		/// </summary>
		/// <param name="compress">False will generate the JSON with spaces in between each element for easier readability. True removes all spaces outside of keys and string values.</param>
		public override string ToString(bool compress)
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			result.Append("{");
			if (!compress)
				result.Append(" ");
			int count = 0;
			foreach (string key in keyValue.Keys)
			{
				string value = keyValue[key].ToString(compress);
				if (value == null)
					value = "";
				if (compress)
					result.Append(string.Format("{0}\"{1}\":{2}", count > 0 ? "," : "", Escape(key), value));
				else
					result.Append(string.Format("{0}\"{1}\" : {2}", count > 0 ? " , " : "", Escape(key), value));
				count++;
			}

			if (!compress)
				result.Append(" ");
			result.Append("}");
			return result.ToString();
		}
		*/
        internal override string InternalToString(Formatting format, int depth)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            int count = 0;
            int nextDepth = depth + 1;
            string root = null;
            string indent = null;
            switch (format)
            {
                case Formatting.Compressed:
                    sb.Append("{");
                    foreach (string key in propertyValue.Keys)
                    {
                        string value = propertyValue[key].InternalToString(format, 0);
                        if (value == null)
                            value = "";
                        sb.Append(string.Format("{0}\"{1}\":{2}", count > 0 ? "," : "", key, value));
                        count++;
                    }
                    sb.Append("}");
                    break;
                case Formatting.Spaces:
                    sb.Append("{ ");
                    foreach (string key in propertyValue.Keys)
                    {
                        string value = propertyValue[key].InternalToString(format, 0);
                        if (value == null)
                            value = "";
                        sb.Append(string.Format("{0}\"{1}\" : {2}", count > 0 ? " , " : "", key, value));
                        count++;
                    }
                    sb.Append(" }");
                    break;
                case Formatting.Indented:
                    sb.AppendLine("{");
                    count = propertyValue.Count;

                    root = GenerateIndents(depth);
                    indent = GenerateIndents(nextDepth);
                    foreach (string key in propertyValue.Keys)
                    {
                        sb.AppendLine(indent + string.Format("\"{1}\": {2}{0}",
                            count > 1 ? "," : "",
                            key,
                            propertyValue[key].InternalToString(format, nextDepth)));
                        count--;
                    }
                    sb.Append(root + "}");
                    break;
                case Formatting.JavascriptCompressed:
                    sb.Append("{");
                    foreach (string key in propertyValue.Keys)
                    {
                        string value = propertyValue[key].InternalToString(format, 0);
                        if (value == null)
                            value = "";
                        sb.Append(string.Format("{0}{1}:{2}", count > 0 ? "," : "", key, value));
                        count++;
                    }
                    sb.Append("}");
                    break;
                case Formatting.JavascriptIndented:
                    sb.AppendLine("{");
                    count = propertyValue.Count;
                    root = GenerateIndents(depth);
                    indent = GenerateIndents(nextDepth);
                    foreach (string key in propertyValue.Keys)
                    {
                        sb.AppendLine(indent + string.Format("{1}: {2}{0}",
                            count > 1 ? "," : "",
                            key,
                            propertyValue[key].InternalToString(format, nextDepth)));
                        count--;
                    }
                    sb.Append(root + "}");
                    break;
                default:
                    break;
            }
            return sb.ToString();
        }

        internal void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null)
                return;
            var eventArgs = new PropertyChangedEventArgs(propertyName);
            PropertyChanged(this, eventArgs);
        }
        static Dictionary<Type, PropertyInfo[]> cache = new Dictionary<Type, PropertyInfo[]>();
        private void DeserializeJsonValueInto(System.Reflection.PropertyInfo property, object obj, JsonValue value)
        {
            try
            {
                if (property.PropertyType == value.GetType())
                {
                    if (property.CanWrite)
                        property.SetValue(obj, value, null);
                }
                else
                {
                    switch (value.ValueType)
                    {
                        case JsonValueTypes.String:
                            if (property.PropertyType == typeof(string))
                                property.SetValue(obj, (string)value, null);
							else if (property.PropertyType.BaseType == typeof(Enum))
							{
								property.SetValue(obj, Enum.Parse(property.PropertyType, (string)value), null);

							}
                            break;
                        case JsonValueTypes.Object:
                            if (!property.CanWrite)
                            {
                                object o2 = property.GetValue(obj, null);
                                JsonObject jo = (JsonObject)value;

								foreach (PropertyInfo property2 in GetTypeProperties(o2.GetType()))
                                    if (jo.ContainsProperty(property2.Name))
                                        DeserializeJsonValueInto(property2, o2, jo[property2.Name]);
                            }
                            else
                                property.SetValue(obj, ((JsonObject)value).DeserializeObject(property.PropertyType), null);
                            break;
                        case JsonValueTypes.Array:
                            if (!property.CanWrite)
                            {
                                if (typeof(System.Collections.IList).IsAssignableFrom(property.PropertyType))
                                    ((JsonArray)value).DeserializeInto((IList)property.GetValue(obj, null));
                                else if (typeof(System.Collections.IDictionary).IsAssignableFrom(property.PropertyType))
                                    ((JsonArray)value).DeserializeInto((IDictionary)property.GetValue(obj, null));
                            }
                            else
                                property.SetValue(obj, ((JsonArray)value).Deserialize(property.PropertyType), null);
                            break;
                        case JsonValueTypes.Number:
                            if (property.PropertyType == typeof(short))
                                property.SetValue(obj, (short)value, null);
                            else if (property.PropertyType == typeof(int))
                                property.SetValue(obj, (int)value, null);
                            else if (property.PropertyType == typeof(long))
                                property.SetValue(obj, (long)value, null);
                            else if (property.PropertyType == typeof(ushort))
                                property.SetValue(obj, (ushort)value, null);
                            else if (property.PropertyType == typeof(uint))
                                property.SetValue(obj, (uint)value, null);
                            else if (property.PropertyType == typeof(ulong))
                                property.SetValue(obj, (ulong)value, null);
                            else if (property.PropertyType == typeof(float))
                                property.SetValue(obj, (float)value, null);
                            else if (property.PropertyType == typeof(double))
                                property.SetValue(obj, (double)value, null);
                            else if (property.PropertyType == typeof(decimal))
                                property.SetValue(obj, (decimal)value, null);
                            else if (property.PropertyType == typeof(byte))
                                property.SetValue(obj, (byte)value, null);
                            break;
                        case JsonValueTypes.Boolean:
                            if (property.PropertyType == typeof(bool))
                                property.SetValue(obj, (bool)value, null);
                            break;
                        case JsonValueTypes.Binary:
                            if (property.PropertyType == typeof(byte[]))
                                property.SetValue(obj, (byte[])value, null);
                            break;
                        case JsonValueTypes.Null:
                            property.SetValue(obj, null, null);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private string Escape(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                value = value.Replace("\\", "\\\\");
                value = value.Replace("\r", "\\r");
                value = value.Replace("\n", "\\n");
                value = value.Replace("\"", "\\\"");
                value = value.Replace("\t", "\\t");
                value = value.Replace("/", "\\/");
            }
            return value;
        }

        private PropertyDescriptor[] getProperties()
        {
            List<PropertyDescriptor> props = new List<PropertyDescriptor>();
			string dt = "_attributesTable";
			
			JsonObject details = this[dt] as JsonObject ?? new JsonObject();
			/*
            if (this.ContainsProperty("_category"))
                if (this["_category"] is JsonString)
                    category = ((JsonString)this["_category"]).Value;
			*/
            foreach (string key in this.Properties)
            {
				if (key == dt)
					continue;
				JsonObject pd = details[key] as JsonObject;
				string category = (string)pd?["Category"];
				string desc = (string)pd?["Description"];
                JsonValue defaultValue = pd?["DefaultValue"];
                bool readOnly = false;
                bool browsable = true;
                if (key.StartsWith("_"))
                    browsable = false;
                if (key.EndsWith("_"))
                    readOnly = true;

                props.Add(new JsonObjectPropertyDescriptor(key, this[key].GetType(), category, desc, readOnly, browsable)
                {
                    DefaultValue = defaultValue
                });
            }
            if (props.Count == 0)
            {
                var pdc = TypeDescriptor.GetProperties(typeof(JsonObject));
                props.Add(pdc["Properties"]);
            }
            return props.ToArray();
        }

        private void InternalParser(string json)
        {
            InternalParser(new JsonReader(json));
        }

        private void InternalParser(JsonReader reader)
        {
            string key = null;
            bool inObj = false;
            bool inKey = false;
            bool inString = false;
            bool inEsc = false;
            char chr;
            StringBuffer buffer = new StringBuffer();

            while (!reader.EndOfJson)
            {
                chr = reader.Read();
                if (inEsc)
                {
                    #region Escape
                    char echr;
                    switch (chr)
                    {
                        case 'r':
                            echr = '\r';
                            break;
                        case 'n':
                            echr = '\n';
                            break;
                        case 'f':
                            echr = '\f';
                            break;
                        case 'b':
                            echr = '\b';
                            break;
                        case 't':
                            echr = '\t';
                            break;
                        default:
                            echr = chr;
                            break;
                    }
                    buffer.Add(echr);
                    inEsc = false;
                    continue;
                    #endregion //Escape
                }
                switch (chr)
                {
                    case '\\':
                        if (inString)
                            inEsc = true;
                        break;
                    case '{':
                        if (inString)
                            buffer.Add(chr);
                        else if (!inObj)
                            inKey = inObj = true;
                        else
                        {
                            if (!string.IsNullOrEmpty(key))
                            {
                                reader.Position--;
                                this.internalAdd(key, new JsonObject(reader));
                            }
                        }
                        break;
                    case '}':
                        if (inString)
                            buffer.Add(chr);
                        else
                        {
                            if (!string.IsNullOrEmpty(key) && buffer.Length > 0)
                                this.internalAdd(key, ValueFromString(buffer.Dump()));
                            return;
                        }
                        break;
                    case ':':
                        if (!inString)
                        {
                            key = buffer.Dump();
                            if (string.IsNullOrEmpty(key))
                                throw new Exception("Bad JSON Format.");
                        }
                        else
                            buffer.Add(chr);
                        break;
                    case '"':
                        inString = !inString;
                        if (!inString && key != null)
                        {
                            string value = buffer.Dump();
                            if (value.StartsWith("base64"))
                                this.internalAdd(key, new JsonBinary(value));
                            else
                                this.internalAdd(key, new JsonString(value));
                        }
                        break;
                    case ',':
                        if (inString)
                            buffer.Add(chr);
                        else
                        {
                            if (!string.IsNullOrEmpty(key) && buffer.Length > 0)
                                this.internalAdd(key, ValueFromString(buffer.Dump()));
                            key = null;
                        }
                        break;
                    case '\r':
                    case '\n':
                    case '\t':
                        break;
                    case ' ':
                        if (inString)
                            buffer.Add(chr);
                        break;
                    case '[':
                        if (inString)
                            buffer.Add(chr);
                        else if (!string.IsNullOrEmpty(key))
                        {
                            reader.Position--;
                            this.internalAdd(key, new JsonArray(reader));
                        }
                        break;
                    default:
                        buffer.Add(chr);
                        break;
                }
            }
        }

        private void NotifyToRefreshAllProperties()
        {
            OnPropertyChanged(String.Empty);
        }

        private string Unescape(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                value = value.Replace("\\r", "\r");
                value = value.Replace("\\n", "\n");
                value = value.Replace("\\\"", "\"");
                value = value.Replace("\\t", "\t");
                value = value.Replace("\\\\", "\\");
                value = value.Replace("\\/", "/");
            }
            return value;
        }

        private JsonValue ValueFromString(string value)
        {
            decimal d = 0;
            bool b = false;
            if (string.IsNullOrEmpty(value) || value == "null")
                return new JsonNull();
            if (bool.TryParse(value, out b))
                return new JsonBoolean(b);
            else if (decimal.TryParse(value, out d))
                return new JsonNumber(d);
            else
                return new JsonNull();
        }

		private PropertyInfo[] GetTypeProperties(Type type)
		{
			lock (locker)
			{
				if (propertyCache.ContainsKey(type))
					return propertyCache[type];
				PropertyInfo[] info = type.GetProperties();
				propertyCache.Add(type, info);
				return info;
			}
		}

        #endregion Methods

        #region Nested Types

		/// <summary>
		/// A <see cref="PropertyDescriptor"/> to represent the properties of a <see cref="JsonObject"/>.
		/// </summary>
        public class JsonObjectPropertyDescriptor : PropertyDescriptor
        {
            #region Fields

            string _category;
            string _description;
            string _displayName;
            Type _propertyType;
            bool _readOnly = false;

            #endregion Fields

            #region Constructors

			/// <summary>
			/// Initialized a new instance of <see cref="JsonObjectPropertyDescriptor"/>.
			/// </summary>
			/// <param name="name">The name of the property.</param>
			/// <param name="propertyType">The value type the property gets and sets.</param>
			/// <param name="category">The category the property belongs.</param>
			/// <param name="description">A description of the property.</param>
			/// <param name="readOnly">Determines if the property is read only.</param>
			/// <param name="browsable">Determines if the property is browsable.</param>
            public JsonObjectPropertyDescriptor(string name, Type propertyType, string category, string description, bool readOnly, bool browsable)
                : base(name, new Attribute[] { new BrowsableAttribute(browsable), new ReadOnlyAttribute(readOnly) })
            {
                _readOnly = readOnly;
                _displayName = name.Replace("_", "");
                var d = this.AttributeArray;
                CanSetNull = true;
                _propertyType = propertyType;
                _category = category;
                _description = description;
            }

            #endregion Constructors

            #region Properties

			/// <summary>
			/// Gets or Sets if the property can be set as null.
			/// </summary>
            public bool CanSetNull
            {
                get;
                set;
            }

			/// <summary>
			/// Gets the name that should be displayed.
			/// </summary>
            public override string DisplayName
            {
                get
                {
                    return _displayName;
                }
            }

			/// <summary>
			/// Gets the category of this property.
			/// </summary>
            public override string Category
            {
                get
                {
                    return _category;
                }
            }

			/// <summary>
			/// Gets the type <see cref="JsonObject"/>.
			/// </summary>
            public override Type ComponentType
            {
                get { return typeof(JsonObject); }
            }

			/// <summary>
			/// Gets or Sets the default value.
			/// </summary>
            public JsonValue DefaultValue
            {
                get;
                set;
            }

			/// <summary>
			/// Gets the property description.
			/// </summary>
            public override string Description
            {
                get
                {
                    return _description;
                }
            }
            
			/// <summary>
			/// Gets of Sets the property owner.
			/// </summary>
            public JsonObject Owner
            {
                get;
                set;
            }

			/// <summary>
			/// Gets the type this property Gets or Sets.
			/// </summary>
            public override Type PropertyType
            {
                get { return _propertyType; }
            }

			/// <summary>
			/// Gets whether the property is read only.
			/// </summary>
            public override bool IsReadOnly
            {
                get
                {
                    return _readOnly;
                }
            }

            #endregion Properties

            #region Methods

			/// <summary>
			/// If the property contains a default value.
			/// </summary>
			/// <param name="component">Not used.</param>
			/// <returns>Returns true if <see cref="DefaultValue"/> is not null.</returns>
            public override bool CanResetValue(object component)
            {
                return DefaultValue != null;
            }

			/// <summary>
			/// Returns the value property of the provided component.
			/// </summary>
			/// <param name="component">The <see cref="JsonObject"/> to get the value from.</param>
			/// <returns>The value of the component.</returns>
            public override object GetValue(object component)
            {
                if (component is JsonObject)
                    return ((JsonObject)component)[Name];
                else if (Owner != null)
                    return Owner[Name];
                else
                    return null;
            }

			/// <summary>
			/// Resets the value of the property with the value from <see cref="DefaultValue"/>.
			/// </summary>
			/// <param name="component">The component whos value should be reset.</param>
            public override void ResetValue(object component)
            {
                if (DefaultValue != null)
                {
                    if (component is JsonObject)
                        SetValue(component, DefaultValue);
                    else if (Owner != null)
                        SetValue(Owner, DefaultValue);

                }
            }

			/// <summary>
			/// A mothod used to set the category.
			/// </summary>
			/// <param name="category"></param>
            public void SetCategory(string category)
            {
                _category = category;
            }

			/// <summary>
			/// Sets the value of the property.
			/// </summary>
			/// <param name="component">The properties whos value should be set.</param>
			/// <param name="value">The value that should be set to the property.</param>
            public override void SetValue(object component, object value)
            {
                if (!(component is JsonObject))
                    component = Owner;
                if (component is JsonObject)
                {
                    JsonObject obj = (JsonObject)component;
                    if (value is JsonValue)
                        obj[Name] = ((JsonValue)value).Clone();
                    else
                        obj[Name] = obj.ValueFromObject(value);
                    /*else if (value is string)
						obj[Name] = new JsonString((string)value);
					else if (value is bool)
						obj[Name] = new JsonBoolean((bool)value);
					else if (value is decimal)
						obj[Name] = new JsonNumber((decimal)value);
					else if (value is int)
						obj[Name] = new JsonNumber((int)value);
					else if (value == null && CanSetNull)
						obj[Name] = new JsonNull();
					else
						obj[Name] = new JsonString();*/
                    OnValueChanged(component, EventArgs.Empty);
                    if (Owner != null)
                        Owner.OnPropertyChanged(Name);
                }
            }

			/// <summary>
			/// Determines if the property should be reset.
			/// </summary>
			/// <param name="component">The property.</param>
			/// <returns>Returns true if there is a value for <see cref="DefaultValue"/> and if the value of the property is not the same; otherwise false.</returns>
            public override bool ShouldSerializeValue(object component)
            {
                if (DefaultValue != null)
                {
                    JsonValue v = null;
                    if (component is JsonObject)
                        v = (JsonValue)GetValue(component);
                    else if (component is JsonValue)
                        v = (JsonValue)component;
                    else if (Owner != null)
                        v = (JsonValue)GetValue(Owner);
                    if (v != null && v.GetType() == DefaultValue.GetType())
                    {
                        return !v.Equals(DefaultValue);
                    }
                }
                return false;
            }

            #endregion Methods
        }

        #endregion Nested Types
    }
}