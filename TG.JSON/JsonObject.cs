using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Reflection;
using System.IO;

namespace TG.JSON
{

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
#if !NETSTANDARD1_X
    [TypeConverter(typeof(ComponentConverter))]
#endif
#if !NETSTANDARD1_0
    [Serializable]
#endif
    public sealed partial class JsonObject : JsonValue, INotifyPropertyChanged, IXmlSerializable, IEnumerable
#if !NETSTANDARD1_0
        , ISerializable
#endif
#if !NETSTANDARD1_X
        , ICustomTypeDescriptor
#endif
    {
        #region Fields

        //Type myType;
        Dictionary<string, JsonValue> propertyValue = new Dictionary<string, JsonValue>();
        static Dictionary<Type, PropertyInfoEx[]> propertyCache = new Dictionary<Type, PropertyInfoEx[]>();
        static object locker = new object();
#if CAN_DYNAMIC
        private DynamicObjectHandler dynObjHandler = null;
#endif

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public JsonObject()
        {
            //myType = GetType();
        }

#if !NETSTANDARD1_0
        /// <summary>
        /// Initialize a new instance of <see cref="JsonObject"/> with an <see cref="IEncryptionHandler"/>.
        /// </summary>
        /// <param name="encryption">The <see cref="IEncryptionHandler"/> to use for encryption.</param>
        public JsonObject(IEncryptionHandler encryption) : this()
        {
            this.GlobalEncryptionHandler = encryption;
        }
#endif

        /// <summary>
        /// Initializes a new instance of <see cref="JsonObject"/> that parses the specified JSON string.
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

#if !NETSTANDARD1_0
        /// <summary>
        /// Initializes a new instance of <see cref="JsonObject"/> that parses the specified JSON string.
        /// </summary>
        /// <param name="json">JSON Object formatted string. Ex. { "Hello" : "World" } </param>
        /// <param name="encryption">The <see cref="IEncryptionHandler"/> to use for encryption.</param>
        public JsonObject(string json, IEncryptionHandler encryption) : this()
        {
            this.GlobalEncryptionHandler = encryption;
            this.InternalParser(json);
        }
#endif

        /// <summary>
        /// Initializes a new instance of <see cref="JsonObject"/> that is populated with an initial property name and string value.
        /// </summary>
        /// <param name="property">A unique property name used for the initial entry.</param>
        /// <param name="value">A <see cref="System.String"/> value for the initial entry.</param>
        /// <example>
        ///	<code>
        ///	JsonObject json = new JsonObject("Hello", "World");
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
        ///		"IAMNUMBER", 1);
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

#if !NETSTANDARD1_0
        /// <summary>
        /// Initializes a new instance of <see cref="JsonObject"/> that serializes the specified object.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="encryption">The <see cref="IEncryptionHandler"/> used to encrypt and decrypt values.</param>
        public JsonObject(object obj, IEncryptionHandler encryption)
            : this()
        {
            GlobalEncryptionHandler = encryption;
            SerializeObject(obj);
        }
#endif

        /// <summary>
        /// Initializes a new instance of <see cref="JsonObject"/> that serializes the specified object.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="maxDepth">The maximum property depth that should be serialized.</param>
        /// <param name="ignoreProperties">Property names that should be ignored when serializing.</param>
        /// <param name="includeAttributes">If True, the AttributeTable will be populated with the object's attributes.</param>
        /// <param name="includeTypeInformation">If True, a _type property will be added with the full Type.AssemblyQualifiedName.</param>
        public JsonObject(object obj, int maxDepth, bool includeAttributes, bool includeTypeInformation, params string[] ignoreProperties)
            : this()
        {
            SerializeObject(obj, maxDepth, includeAttributes, includeTypeInformation, ignoreProperties);
        }

#if !NETSTANDARD1_0
        /// <summary>
        /// Initializes a new instance of <see cref="JsonObject"/> that serializes the specified object.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="maxDepth">The maximum property depth that should be serialized.</param>
        /// <param name="ignoreProperties">Property names that should be ignored when serializing.</param>
        /// <param name="includeAttributes">If True, the AttributeTable will be populated with the object's attributes.</param>
        /// <param name="includeTypeInformation">If True, a _type property will be added with the full Type.AssemblyQualifiedName.</param>
        /// <param name="encryption">The <see cref="IEncryptionHandler"/> used to encrypt and decrypt values.</param>
        public JsonObject(object obj, int maxDepth, bool includeAttributes, bool includeTypeInformation, IEncryptionHandler encryption, params string[] ignoreProperties)
            : this()
        {
            GlobalEncryptionHandler = encryption;
            SerializeObject(obj, maxDepth, includeAttributes, includeTypeInformation, ignoreProperties);
        }
#endif

#if !NETSTANDARD1_0
        /// <summary>
        /// Initializes a new instance of <see cref="JsonObject"/> that serializes the specified object.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="encryption">The <see cref="IEncryptionHandler"/> used to encrypt and decrypt values.</param>
        /// <param name="serializationOptions">The options to use for serialization.</param>
        public JsonObject(object obj, IEncryptionHandler encryption, Serialization.JsonSerializationOptions serializationOptions)
            : this()
        {
            GlobalEncryptionHandler = encryption;
            SerializeObject(obj, serializationOptions);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="JsonObject"/> using the specified <see cref="JsonReader"/> to parse a JSON string.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> that will be used to parse a JSON string.</param>
        /// <param name="encryptionHandler">The <see cref="IEncryptionHandler"/> used to encrypt and decrypt values.</param>
        public JsonObject(JsonReader reader, IEncryptionHandler encryptionHandler)
            : this()
        {
            GlobalEncryptionHandler = encryptionHandler;
            InternalParser(reader);
        }

#endif

        /// <summary>
        /// Initializes a new instance of <see cref="JsonObject"/> using the specified <see cref="JsonReader"/> to parse a JSON string.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> that will be used to parse a JSON string.</param>
        public JsonObject(JsonReader reader) : this()
        {
            InternalParser(reader);
        }

        internal JsonObject(JsonReader reader, JsonValue parent): this()
        {
            SetParent(parent);
            InternalParser(reader);
        }

#if !NETSTANDARD1_0
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
#endif

        /// <summary>
        /// Initializes a new instance of <see cref="JsonObject"/> using a source <see cref="System.IO.Stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="System.IO.Stream"/> to read from.</param>
        public JsonObject(System.IO.Stream stream) : this()
        {
            using (JsonReader reader = new JsonReader(stream))
            {
                InternalParser(reader);
            }
        }

        /// <summary>
        /// Initializes a new instance of <see cref="JsonObject"/> using a source <see cref="System.IO.StreamReader"/>.
        /// </summary>
        /// <param name="stream">The <see cref="System.IO.StreamReader"/> to read from.</param>
        public JsonObject(System.IO.StreamReader stream) : this()
        {
            using (JsonReader reader = new JsonReader(stream))
            {
                InternalParser(reader);
            }
        }

#if !NETSTANDARD1_0
        /// <summary>
        /// Initializes a new instance of <see cref="JsonObject"/> using a source <see cref="System.IO.Stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="System.IO.Stream"/> to read from.</param>
        /// <param name="encryptionHandler">The <see cref="IEncryptionHandler"/> used to encrypt and decrypt values.</param>
        public JsonObject(System.IO.Stream stream, IEncryptionHandler encryptionHandler)
            : this()
        {
            GlobalEncryptionHandler = encryptionHandler;
            using (JsonReader reader = new JsonReader(stream))
            {
                InternalParser(reader);
            }
        }

        /// <summary>
        /// Initializes a new instance of <see cref="JsonObject"/> using a source <see cref="System.IO.StreamReader"/>.
        /// </summary>
        /// <param name="stream">The <see cref="System.IO.StreamReader"/> to read from.</param>
        /// <param name="encryptionHandler">The <see cref="IEncryptionHandler"/> used to encrypt and decrypt values.</param>
        public JsonObject(System.IO.StreamReader stream, IEncryptionHandler encryptionHandler)
            : this()
        {
            GlobalEncryptionHandler = encryptionHandler;
            using (JsonReader reader = new JsonReader(stream))
            {
                InternalParser(reader);
            }
        }
#endif

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
                JsonObject attTable = this["_attributesTable"] as JsonObject;
                if (attTable == null)
                {
                    attTable = new JsonObject();
                    this.internalAdd("_attributesTable", attTable);
                }
                return attTable;
            }
            set
            {
                this["_attributesTable"] = value;
            }
        }

        /// <summary>
        /// Get a string array of all property names.
        /// </summary>
        public string[] PropertyNames
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
                    foreach (var cur in PropertyNames)
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

#if CAN_DYNAMIC

        /// <summary>
        /// Dynamic representation of the properties.
        /// </summary>
        public dynamic Properties
        {
            get
            {
                if (dynObjHandler == null)
                    dynObjHandler = new DynamicObjectHandler(this);
                return dynObjHandler;
            }
        }
#endif

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

                return new JsonNull();
            }
            set
            {
                if (propertyValue.ContainsKey(property))
                {
                    if (value == null)
                        value = new JsonNull();
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

#if !NETSTANDARD1_0
        /// <summary>
        /// A static method for creating a JsonObject using provided JSON object string.
        /// </summary>
        /// <param name="json">A JSON object string</param>
        /// <param name="encryption">The <see cref="IEncryptionHandler"/> used to encrypt and decrypt values.</param>
        /// <returns>A new instance of <see cref="JsonObject"/> based on the parsed JSON string.</returns>
        /// <example>
        /// <code>
        /// class MyClass
        /// {
        ///		static void Main()
        ///		{
        ///		    EncryptionHandler enc = new EncryptionHandler("EncryptMe");
        ///			JsonObject json = JsonObject.Parse("{ \"Hello\" : \"World\" , \"IsAwesome\" : true }", enc);
        ///			Console.Write(json.ToString(false));
        ///		}
        /// }
        /// </code>
        /// </example>
        public static JsonObject Parse(string json, IEncryptionHandler encryption)
        {
            return new JsonObject(json, encryption);
        }
#endif

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
        /// Adds a new entry to the current <see cref="JsonObject"/>'s collection.
        /// </summary>
        /// <param name="propertyDefinition">The definition of the property.</param>
        /// <param name="value">The value to add to the property.</param>
        /// <returns>Returns this <see cref="JsonObject"/>.</returns>
        /// <example>
        /// <code>
        /// JsonObject json = New JsonObject();
        /// json.Add(new JsonPropertyDefinition("Hello", true, false), new JsonArray("[ 1, 2, true, false]"));
        /// json.Add("IsAwesome", true);
        ///
        /// //This method returns itself. This makes possible in-line calling.
        /// JsonObject j = new JsonObject("hello", "world");
        /// j.Remove("hello").Add("quote", "Everything is Awesome!");
        /// </code>
        /// </example>
        public JsonObject Add(JsonPropertyDefinition propertyDefinition, JsonValue value)
        {
            this.internalAdd(propertyDefinition.PropertyName, value);
            propertyDefinition.CreateAttributeEntry(AttributesTable);
            OnValueChanged();
            OnPropertyChanged(propertyDefinition.PropertyName);
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
            object p;
            for (int i = 0; i < propertiesValues.Length; i += 2)
            {
                p = propertiesValues[i];
                if ((p as string) != null)
                    this.internalAdd((string)p, ValueFromObject(propertiesValues[i + 1]));
                else if ((p as JsonPropertyDefinition) != null)
                {
                    var pDef = p as JsonPropertyDefinition;
                    this.internalAdd(pDef.PropertyName, ValueFromObject(propertiesValues[i + 1]));
                    pDef.CreateAttributeEntry(AttributesTable);
                }
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
            return new JsonObject(this.ToString()
#if !NETSTANDARD1_0 
                , GlobalEncryptionHandler
#endif
                );
        }

        /// <summary>
        /// Determines if the provided property exists in the current JsonObject's collection.
        /// </summary>
        /// <param name="property">A <see cref="string"/> property to search for. This is case sensitive.</param>
        /// <returns>Returns true if the property exists; otherwise false.</returns>
        public bool ContainsProperty(string property)
        {
            if (string.IsNullOrEmpty(property))
                return false;
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
#if !NETSTANDARD1_X
            if (type == typeof(System.Drawing.Color))
            {
                if (this.ContainsProperty("color"))
                {
                    string clr = (string)this["color"];
                    if (clr.StartsWith("#"))
                    {
                        obj = System.Drawing.Color.FromArgb(int.Parse(clr.Replace("#", ""), System.Globalization.NumberStyles.HexNumber));
                    }
                    else
                    {
                        obj = System.Drawing.Color.FromName(clr);
                    }
                }
                else
                    obj = System.Drawing.Color.Black;
            }
            else
#endif
            {
                if (this.ContainsProperty("_type"))
                {
                    Regex rex = new Regex(@"^([^,]+),\s([^,]+),\sVersion=(\d{1,}\.\d{1,}\.\d{1,}\.\d{1,}),\sCulture=(\w+),\sPublicKeyToken=(\w+)");
                    Match m = rex.Match((string)this["_type"]);
                    if (m.Success)
                    {
#if FULLNET
                        obj = Activator.CreateInstance(m.Groups[2].Value, m.Groups[1].Value).Unwrap();
#else
                        obj = Activator.CreateInstance(Type.GetType(m.Value));
#endif
                        type = obj.GetType();
                    }
                    else
                    {
                        obj = Activator.CreateInstance(type);
                    }
                }
                else
                    obj = Activator.CreateInstance(type);
                foreach (PropertyInfoEx property in GetTypeProperties(type))
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
        /// Returns a <see cref="JsonPropertyDefinition"/> of a property.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>Returns the <see cref="JsonPropertyDefinition"/>, if the property exists within the <see cref="JsonObject"/>; else null is returned.</returns>
        public JsonPropertyDefinition GetPropertyDefinition(string propertyName)
        {
            if (!this.ContainsProperty(propertyName))
                return null;
            JsonObject att = AttributesTable[propertyName] as JsonObject;
            if (att == null)
                return new JsonPropertyDefinition(propertyName);
            else
                return new JsonPropertyDefinition(
                    propertyName,
                    (string)att["Category"],
                    (string)att["Description"],
                    att["DefaultValue"],
                    (bool)att["Browsable"],
                    (bool)att["ReadOnly"]
                   );
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="JsonObject"/>'s property/value.
        /// </summary>
		public IEnumerator GetEnumerator()
        {
            switch (EnumeratorType)
            {
                case JsonObjectEnumeratorTypes.Bindable:
                    return new JsonObjectBindableEnumerator(this);
                case JsonObjectEnumeratorTypes.Property:
                    return new JsonObjectPropertyEnumerator(this);
                default:
                    return propertyValue.GetEnumerator();
            }

        }

        /// <summary>
        /// Selects the type of <see cref="IEnumerator"/> returned by the <see cref="IEnumerable.GetEnumerator"/> method.
        /// </summary>
        public JsonObjectEnumeratorTypes EnumeratorType { get; set; }

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

        /// <summary>
        /// Get the value of a property as the specified type.
        /// </summary>
        /// <typeparam name="T">The type of value to return.</typeparam>
        /// <param name="property">The name of the property</param>
        /// <returns>T</returns>
        public T GetValueAs<T>(string property)
        {
            try
            {
                return (T)Convert.ChangeType(this[property], typeof(T));
            }
            catch (Exception)
            {

            }
            return default(T);
        }

#if !NETSTANDARD1_X
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

#endif
#if !NETSTANDARD1_0
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Value", this.ToString());
        }
#endif

        System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
        {
            InternalParser(reader.ReadContentAsString());
        }

        void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteCData(this.ToString());
        }

        /// <summary>
        /// Reads JSON from a <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to read from.</param>
        public void Read(System.IO.Stream stream)
        {
            using (JsonReader reader = new JsonReader(stream))
            {
                InternalParser(reader);
            }
        }

#if !NETSTANDARD1_0
        /// <summary>
        /// Reads JSON from a file.
        /// </summary>
        /// <param name="path">The path to read from.</param>
        public void Read(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                Read(fs);
            }
        } 
#endif

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
        /// <param name="includeAttributes">If True, the AttributeTable will be populated with the object's attributes.</param>
        /// <returns>The current instance of <see cref="JsonObject"/>. (Returns itself)</returns>
		public JsonObject SerializeObject(object obj, bool includeAttributes)
        {
            return SerializeObject(obj, int.MaxValue, includeAttributes);
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
            return SerializeObject(obj, maxDepth, false, false, ignoreProperties);
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
        /// <param name="includeAttributes">If True, the AttributeTable will be populated with the object's attributes.</param>
        /// <param name="ignoreProperties">Property names that should be ignored when serializing.</param>
        /// <returns>The current instance of <see cref="JsonObject"/>. (Returns itself)</returns>
		public JsonObject SerializeObject(object obj, int maxDepth, bool includeAttributes, params string[] ignoreProperties)
        {
            return SerializeObject(obj, maxDepth, includeAttributes, false, ignoreProperties);
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
        /// <param name="includeAttributes">If True, the AttributeTable will be populated with the object's attributes.</param>
        /// <param name="includeTypeInformation">If True, a _type property will be added with the full Type.AssemblyQualifiedName.</param>
        /// <returns>The current instance of <see cref="JsonObject"/>. (Returns itself)</returns>
		public JsonObject SerializeObject(object obj, int maxDepth, bool includeAttributes, bool includeTypeInformation, params string[] ignoreProperties)
        {
            return SerializeObject(obj, new Serialization.JsonSerializationOptions(maxDepth, includeAttributes, includeTypeInformation, ignoreProperties, null));
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
        /// <param name="serializationOptions">The options to user when serializing.</param>
        /// <returns>The current instance of <see cref="JsonObject"/>. (Returns itself)</returns>
        public JsonObject SerializeObject(object obj, Serialization.JsonSerializationOptions serializationOptions)
        {
            if (obj == null || serializationOptions == null)
                return this;

            foreach (PropertyInfoEx property in GetTypeProperties(obj.GetType()))
            {
                if (serializationOptions.IgnoreProperties.Contains(property.Name) || property.IgnoreProperty || !property.CanRead
                    || (!property.IsPublic && property.JsonProperty == null))
                    continue;
                if ((serializationOptions.CurrentDepth == serializationOptions.MaxDepth || serializationOptions.ApplySelectedPropertiesOnChildren)
                    && serializationOptions.SelectedProperties.Count > 0 && !serializationOptions.SelectedProperties.Contains(property.Name))
                    continue;
                try
                {
                    object pval = property.GetValue(obj, null);

                    if (property.PropertyType == typeof(object))
                    {

                        JsonValue value;
                        if (property.EncryptValue && pval != null)
                        {
                            value = new JsonString(pval?.ToString()) { EncryptValue = true };
                        }
                        else
                        {
                            value = ValueFromObject(pval, serializationOptions);
                        }
                        if (value.ValueType == JsonValueTypes.Object)
                        {
                            ((JsonObject)value).internalAdd("_type", pval.GetType().AssemblyQualifiedName);
                        }

                        this.internalAdd(property.Name, value);
                    }
                    else
                    {
                        JsonValue value;
                        if (property.EncryptValue && pval != null && !(pval is System.Collections.IEnumerable && property.PropertyType != typeof(string)))
                        {
                            value = new JsonString(pval?.ToString()) { EncryptValue = true };

                        }
                        else
                        {
                            value = ValueFromObject(pval, serializationOptions);
                        }
                        this.internalAdd(property.Name, value);
                    }
                    if (serializationOptions.IncludeTypeInformation)
                    {
                        this.internalAdd("_type", obj.GetType().AssemblyQualifiedName);
                    }
                    if (serializationOptions.IncludeAttributes)
                    {
                        JsonObject table = new JsonObject();
                        foreach (Attribute att in property.Info.GetCustomAttributes(true))
                        {
                            Type t = att.GetType();
                            if (t.Namespace == "TG.JSON") continue;
                            table[t.Name.Replace("Attribute", "")] = ValueFromObject(att, 1, "TypeId");

                        }
                        if (table.Count > 0)
                        {
                            AttributesTable[property.Name] = table;
                        }
                        //JsonPropertyDefinition pdef = new JsonPropertyDefinition(property.Info);
                        //pdef.CreateAttributeEntry(AttributesTable);
                    }
                }
                catch (Exception)
                {
                }
            }
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
        [Obsolete("Use SerializeObject(object obj, bool includeAttributes) instead.")]
        public JsonObject SerializeObjectWithAttributes(object obj)
        {
            return SerializeObjectWithAttributes(obj, int.MaxValue);
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
        [Obsolete("Use SerializeObject(object obj, int maxDepth, bool includeAttributes, params string[] ignoreProperties) instead.")]
        public JsonObject SerializeObjectWithAttributes(object obj, int maxDepth, params string[] ignoreProperties)
        {
            if (obj == null)
                return this;
            List<string> ignore = new List<string>(ignoreProperties);

            foreach (PropertyInfoEx property in GetTypeProperties(obj.GetType()))
            {
                if (ignore.Contains(property.Name) || property.IgnoreProperty || !property.CanRead
                    || (!property.IsPublic && property.JsonProperty == null))
                    continue;

                try
                {
                    object pval = property.GetValue(obj, null);

                    if (property.PropertyType == typeof(object))
                    {

                        JsonValue value;
                        if (property.EncryptValue && pval != null)
                        {
                            value = new JsonString(pval?.ToString()) { EncryptValue = true };
                        }
                        else
                        {
                            value = ValueFromObject(pval, maxDepth, ignoreProperties);
                        }
                        if (value.ValueType == JsonValueTypes.Object)
                        {
                            ((JsonObject)value).internalAdd("_type", pval.GetType().AssemblyQualifiedName);
                        }

                        this.internalAdd(property.Name, value);
                    }
                    else
                    {
                        JsonValue value;
                        if (property.EncryptValue && pval != null && !(pval is System.Collections.IEnumerable))
                        {
                            value = new JsonString(pval?.ToString()) { EncryptValue = true };

                        }
                        else
                        {
                            value = ValueFromObject(pval, maxDepth, ignoreProperties);
                        }
                        this.internalAdd(property.Name, value);
                    }
                    JsonPropertyDefinition pdef = new JsonPropertyDefinition(property.Info);
                    pdef.CreateAttributeEntry(AttributesTable);
                }
                catch (Exception)
                {
                }
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
            SetPropertyAttributes(property, category, description, defaultValue, true, false);
        }

        /// <summary>
        /// Sets the property details for the given property to the _attributesTable <see cref="JsonObject"/>.
        /// </summary>
        /// <param name="property">The name of the property that should have its details set.</param>
        /// <param name="category">The Category of the property.</param>
        /// <param name="description">The Description of the property.</param>
        /// <param name="defaultValue">The DefaultValue of the property.</param>
        /// <param name="browsable">Should the property be visible?</param>
        /// <param name="readOnly">Is the property read only.</param>
        public void SetPropertyAttributes(string property, string category, string description, JsonValue defaultValue, bool browsable, bool readOnly)
        {
            if (string.IsNullOrEmpty(property))
                return;
            JsonObject table = AttributesTable;
            JsonObject details = table[property] as JsonObject ?? new JsonObject();

            details["Category"] = category;
            details["Description"] = description;
            details["DefaultValue"] = defaultValue;
            details["Browsable"] = browsable;
            details["ReadOnly"] = readOnly;
            table[property] = details;
        }

#if !NETSTANDARD1_X
        /// <summary>
        /// Sets a TypeConverter, for the given property, to the _attributesTable and is utilized in the <see cref="JsonObjectPropertyDescriptor"/>.
        /// </summary>
        /// <param name="property">The name of the property to associate with the <see cref="TypeConverter"/>.</param>
        /// <param name="converterType">The <see cref="Type"/> for the <see cref="TypeConverter"/>.</param>
        public void SetPropertyTypeConverter(string property, Type converterType)
        {
            if (string.IsNullOrEmpty(property) || converterType == null)
                return;
            JsonObject details = AttributesTable[property] as JsonObject ?? new JsonObject();
            details["TypeConverter"] = new JsonObject("ConverterTypeName", converterType.AssemblyQualifiedName);
            AttributesTable[property] = details; 
        }
#endif

        /// <summary>
        /// Sets the property details for the given property to the _attributesTable <see cref="JsonObject"/>.
        /// </summary>
        /// <param name="property">The name of the property that should have its details set.</param>
        /// <param name="category">The Category of the property.</param>
        /// <param name="description">The Description of the property.</param>
        /// <param name="defaultValue">The DefaultValue of the property.</param>
        /// <param name="browsable">Should the property be visible?</param>
        /// <param name="readOnly">Is the property read only.</param>
        /// <param name="displayName">Sets the display name.</param>
        public void SetPropertyAttributes(string property, string category, string description, JsonValue defaultValue, bool browsable, bool readOnly, string displayName)
        {
            if (string.IsNullOrEmpty(property))
                return;
            JsonObject table = AttributesTable;
            JsonObject details = table[property] as JsonObject ?? new JsonObject();

            details["Category"] = category;
            details["Description"] = description;
            details["DefaultValue"] = defaultValue;
            details["Browsable"] = browsable;
            details["ReadOnly"] = readOnly;
            details["DisplayName"] = displayName;
            table[property] = details;
        }

        /// <summary>
        /// Sets the property details for the given property to the _attributesTable <see cref="JsonObject"/>.
        /// </summary>
        /// <param name="propertyDefinition">The definition to add to the <see cref="JsonObject.AttributesTable"/>.</param>
        public void SetPropertyAttributes(JsonPropertyDefinition propertyDefinition)
        {
            if (propertyDefinition != null)
                propertyDefinition.CreateAttributeEntry(AttributesTable);
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
            k.AddRange(this.PropertyNames);
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

        /// <summary>
        /// Writes to a <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to write to.</param>
        /// <param name="formatting">How the output should be formatted.</param>
        public void Write(Stream stream, Formatting formatting)
        {
            using (StreamWriter writer = new StreamWriter(stream))
            {
                InternalWrite(writer, formatting, 0);
            }
        }

        /// <summary>
        /// Writes to a <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to write to.</param>
        public void Write(Stream stream)
        {
            Write(stream, Formatting.Compressed);
        }

#if !NETSTANDARD1_X
        /// <summary>
        /// Writes to a <see cref="Stream"/>.
        /// </summary>
        /// <param name="path">The file path to write to.</param>
        /// <param name="formatting">How the output should be formatted.</param>
        public void Write(string path, Formatting formatting)
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                InternalWrite(writer, formatting, 0);
            }
        } 


        /// <summary>
        /// Writes to a <see cref="Stream"/>.
        /// </summary>
        /// <param name="path">The file path to write to.</param>
        public void Write(string path)
        {
            Write(path, Formatting.Compressed);
        }

#endif

        internal JsonObject internalAdd(string property, JsonValue value)
        {
            if (value == null)
                value = new JsonNull();
            value.SetParent(this);
            //key = Unescape(key);
            if (!propertyValue.ContainsKey(property))
                propertyValue.Add(property, value);
            else
                propertyValue[property] = value;
            return this;
        }

        internal override void InternalWrite(StreamWriter writer, Formatting format, int depth)
        {
            int count = 0;
            int nextDepth = depth + 1;
            string root = null;
            string indent = null;
            switch (format)
            {
                case Formatting.Compressed:
                    writer.Write("{");
                    foreach (string key in propertyValue.Keys)
                    {
                        writer.Write($"{(count > 0 ? "," : "")}\"{key}\":");
                        propertyValue[key].InternalWrite(writer, format, depth);
                        count++;
                    }
                    writer.Write("}");
                    break;
                case Formatting.Spaces:
                    writer.Write("{ ");
                    foreach (string key in propertyValue.Keys)
                    {
                        writer.Write($"{(count > 0 ? " , " : "")}\"{key}\" : ");
                        propertyValue[key].InternalWrite(writer, format, depth);
                        count++;
                    }
                    writer.Write(" }");
                    break;
                case Formatting.Indented:
                    writer.Write("{");
                    
                    root = GenerateIndents(depth);
                    indent = GenerateIndents(nextDepth);
                    foreach (string key in propertyValue.Keys)
                    {
                        writer.WriteLine(count > 0 ? "," : "");
                        writer.Write($"{indent}\"{key}\": ");
                        propertyValue[key].InternalWrite(writer, format, nextDepth);
                        count++;
                    }
                    writer.WriteLine();
                    writer.Write($"{root}}}");
                    break;
                case Formatting.JavascriptCompressed:
                    writer.Write("{");
                    foreach (string key in propertyValue.Keys)
                    {
                        writer.Write($"{(count > 0 ? "," : "")}{key}:");
                        propertyValue[key].InternalWrite(writer, format, depth);
                        count++;
                    }
                    writer.Write("}");
                    break;
                case Formatting.JavascriptIndented:
                    writer.Write("{");
                   
                    root = GenerateIndents(depth);
                    indent = GenerateIndents(nextDepth);
                    foreach (string key in propertyValue.Keys)
                    {
                        writer.WriteLine(count > 0 ? "," : "");
                        writer.Write($"{indent}{key}: ");
                        propertyValue[key].InternalWrite(writer, format, nextDepth);
                        count++;
                    }
                    writer.WriteLine();
                    writer.Write($"{root}}}");
                    break;
                default:
                    break;
            }
        }

        internal void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null)
                return;
            var eventArgs = new PropertyChangedEventArgs(propertyName);
            PropertyChanged(this, eventArgs);
        }
        static Dictionary<Type, PropertyInfo[]> cache = new Dictionary<Type, PropertyInfo[]>();
        private void DeserializeJsonValueInto(PropertyInfoEx property, object obj, JsonValue value)
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
                        case JsonValueTypes.Object:
                            if (!property.CanWrite)
                            {
                                object o2 = property.GetValue(obj, null);
                                JsonObject jo = (JsonObject)value;

                                foreach (PropertyInfoEx property2 in GetTypeProperties(o2.GetType()))
                                    if (jo.ContainsProperty(property2.Name))
                                        DeserializeJsonValueInto(property2, o2, jo[property2.Name]);
                            }
                            else
                                property.SetValue(obj, ((JsonObject)value).DeserializeObject(property.PropertyType), null);
                            break;
                        case JsonValueTypes.Array:
                            if (!property.CanWrite)
                            {
#if NETSTANDARD1_X
                                if (typeof(System.Collections.IList).GetTypeInfo().IsAssignableFrom(property.PropertyType.GetTypeInfo()))
                                    ((JsonArray)value).DeserializeInto((IList)property.GetValue(obj, null));
                                else if (typeof(System.Collections.IDictionary).GetTypeInfo().IsAssignableFrom(property.PropertyType.GetTypeInfo()))
                                    ((JsonArray)value).DeserializeInto((IDictionary)property.GetValue(obj, null));
#else
                                if (typeof(System.Collections.IList).IsAssignableFrom(property.PropertyType))
                                    ((JsonArray)value).DeserializeInto((IList)property.GetValue(obj, null));
                                else if (typeof(System.Collections.IDictionary).IsAssignableFrom(property.PropertyType))
                                    ((JsonArray)value).DeserializeInto((IDictionary)property.GetValue(obj, null));
#endif
                            }
                            else
                                property.SetValue(obj, ((JsonArray)value).Deserialize(property.PropertyType), null);
                            break;
                        default:
                            property.SetValue(obj, Convert.ChangeType(value, property.PropertyType), null);
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

#if !NETSTANDARD1_X

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
            foreach (string key in this.PropertyNames)
            {
                List<Attribute> attributes = new List<Attribute>();
                if (key == dt)
                    continue;
                JsonObject pd = details[key] as JsonObject;
                string category = null;
                string desc = null;
                JsonValue defaultValue = null;
                bool readOnly = false;
                bool browsable = true;

                if (pd != null)
                {
                    JsonValue att = pd["Category"];
                    category = att.ValueType == JsonValueTypes.Object ? (string)(att as JsonObject)["Category"] : (string)pd["Category"];

                    att = pd["Description"];
                    desc = att.ValueType == JsonValueTypes.Object ? (string)(att as JsonObject)["Description"] : (string)pd["Description"];

                    att = pd["DefaultValue"];
                    defaultValue = att.ValueType == JsonValueTypes.Object ? (att as JsonObject)["Value"] : pd["DefaultValue"];
                    if (pd.ContainsProperty("Browsable"))
                    {
                        att = pd["Browsable"];
                        browsable = att.ValueType == JsonValueTypes.Object ? (bool)(att as JsonObject)["Browsable"] : (bool)pd["Browsable"];
                    }

                    if (pd.ContainsProperty("ReadOnly"))
                    {
                        att = pd["ReadOnly"];
                        readOnly = att.ValueType == JsonValueTypes.Object ? (bool)(att as JsonObject)["IsReadOnly"] : (bool)pd["ReadOnly"];
                    }

                    if (pd.ContainsProperty("TypeConverter"))
                    {
                        att = pd["TypeConverter"];
                        JsonObject tcObj = (JsonObject)att;
                        attributes.Add(new TypeConverterAttribute(tcObj["ConverterTypeName"]));
                    }
                }

                if (key.StartsWith("_"))
                    browsable = false;
                if (key.EndsWith("_"))
                    readOnly = true;
                JsonValue value = this[key];
                Type ptype;
                switch (value.ValueType)
                {
                    case JsonValueTypes.String:
                        if ((value as JsonString).ContainsDateTime)
                            ptype = typeof(DateTime);
                        else
                            ptype = typeof(string);
                        break;
                    case JsonValueTypes.Number:
                        ptype = typeof(double);
                        break;
                    case JsonValueTypes.Boolean:
                        ptype = typeof(bool);
                        break;
                    case JsonValueTypes.Binary:
                        ptype = typeof(byte[]);
                        break;
                    case JsonValueTypes.Object:
                    case JsonValueTypes.Array:
                    case JsonValueTypes.Null:
                    default:
                        ptype = value.GetType();
                        break;
                }
                attributes.Add(new BrowsableAttribute(browsable));
                attributes.Add(new ReadOnlyAttribute(readOnly));


                props.Add(new JsonObjectPropertyDescriptor(key, ptype, category, desc, readOnly, attributes.ToArray())
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

#endif

        private void InternalParser(string json)
        {
            if (string.IsNullOrEmpty(json))
                return;
            json = json.Trim();
            if (!json.StartsWith("{") && !json.EndsWith("}"))
                return;
            InternalParser(new JsonReader(json));
        }

        private void InternalParser(JsonReader reader)
        {
            string key = null;
            //bool inKey = false;
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
                        else
                        {
                            if (key != null)
                            {
                                this.internalAdd(key, new JsonObject(reader, this));
                            }
                        }
                        break;
                    case '}':
                        if (inString)
                            buffer.Add(chr);
                        else
                        {
                            if (key != null && buffer.Length > 0)
                            {
                                this.internalAdd(key, ValueFromString(buffer.Dump()));
                            }
                            return;
                        }
                        break;
                    case ':':
                        if (!inString)
                        {
                            key = buffer.Dump();
                            if (key == null)
                            {
                                throw new Exception($"Unexpected property key at character index {reader.Position}.");
                            }
                        }
                        else
                            buffer.Add(chr);
                        break;
                    case '"':
                        inString = !inString;
                        if (!inString && key != null)
                        {
                            string value = buffer.Dump();
                            if (value.StartsWith("base64:"))
                                this.internalAdd(key, new JsonBinary(value));
                            else
                            {
                                JsonString jstring = new JsonString();
                                this.internalAdd(key, jstring);
                                jstring.Value = value;
                            }
                        }
                        break;
                    case ',':
                        if (inString)
                            buffer.Add(chr);
                        else
                        {
                            if (key != null && buffer.Length > 0)
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
                        else if (key != null)
                        {
                            this.internalAdd(key, new JsonArray(reader, this));
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

        private PropertyInfoEx[] GetTypeProperties(Type type)
        {
#if !NETSTANDARD1_X
            lock (locker)
            {
                if (propertyCache.ContainsKey(type))
                    return propertyCache[type];
                List<PropertyInfoEx> info = new List<PropertyInfoEx>();

                foreach (PropertyInfo item in type.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    info.Add(new PropertyInfoEx(item, false));
                }

                foreach (PropertyInfo item in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    info.Add(new PropertyInfoEx(item, true));
                }
                PropertyInfoEx[] ex = info.ToArray();
                propertyCache.Add(type, ex);
                return ex;
            }
#else
            return new PropertyInfoEx[0];
#endif
        }

        internal class PropertyInfoEx
        {

            public PropertyInfoEx(PropertyInfo info, bool isPublic)
            {
                Info = info;
                IsPublic = isPublic;
                ReadAttributes();
            }

            private void ReadAttributes()
            {
                foreach (object att in Info.GetCustomAttributes(false))
                {
                    if (JsonProperty == null)
                    {
                        JsonProperty = att as JsonPropertyAttribute;
                    }
                    if (att is JsonIgnorePropertyAttribute)
                    {
                        IgnoreProperty = true;
                    }
                    else if (att is JsonEncryptValueAttribute)
                    {
                        EncryptValue = true;
                    }
                }
            }

            internal void SetValue(object obj, object value, object[] index)
            {
                Info.SetValue(obj, value, index);
            }

            internal object GetValue(object obj, object[] index)
            {
                return Info.GetValue(obj, index);
            }

            public PropertyInfo Info { get; private set; }

            public bool IsPublic { get; private set; }

            public bool EncryptValue { get; set; }

            public bool IgnoreProperty { get; set; }

            public JsonPropertyAttribute JsonProperty { get; set; }

            public bool HasNameOverride
            {
                get
                {
                    return JsonProperty != null && JsonProperty.HasNameOverride;
                }
            }

            public string Name
            {
                get { return HasNameOverride ? JsonProperty.JsonPropertyName : Info.Name; }
            }
            public Type PropertyType
            {
                get { return Info.PropertyType; }
            }
            public bool CanWrite
            {
                get { return Info.CanWrite; }
            }

            public bool CanRead
            {
                get { return Info.CanRead; }
            }
        }

        /// <summary>
        /// Navigate through nested <see cref="JsonObject"/>s using a forward slash separated path string.
        /// </summary>
        /// <param name="path">Properties separated with a forward slash "/".</param>
        /// <returns>Returns the <see cref="JsonValue"/> at the end of the path; else returns null.</returns>
        public JsonValue Navigate(string path)
        {
            return Navigate(path, false);
        }

        /// <summary>
        /// Navigate through nested <see cref="JsonObject"/>s using a forward slash separated path string.
        /// </summary>
        /// <param name="path">Properties separated with a forward slash "/".</param>
        /// <param name="createIfNotExists">Determines if a <see cref="JsonObject"/> should be created if part of the path does not exist.</param>
        /// <returns>Returns the <see cref="JsonValue"/> at the end of the path; else returns null.</returns>
        public JsonValue Navigate(string path, bool createIfNotExists)
        {
            if (string.IsNullOrEmpty(path))
                return null;
            var properties = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (properties.Length == 0)
                return null;
            JsonValue result = this;

            foreach (string prop in properties)
            {
                if (result != null)
                {
                    if (result.ValueType == JsonValueTypes.Object)
                    {
                        JsonValue v = (result as JsonObject)[prop];
                        if (v == null)
                        {
                            if (createIfNotExists)
                            {
                                v = new JsonObject();
                                (result as JsonObject).Add(prop, v);
                                result = v;
                            }
                            else
                                return null;
                        }
                        else
                            result = v;
                    }
                    else
                        return null;
                }
                else
                    return null;

            }
            return result;
        }

#endregion Methods
    }
}