namespace TG.JSON
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.ComponentModel;

    /// <summary>
    /// Defines a property when adding a <see cref="JsonValue"/> to a <see cref="JsonObject"/>.
    /// </summary>
    /// <remarks>
    /// This can be used in place of a string property name. It includes properties used to populate the <see cref="JsonObject.AttributesTable"/>.
    /// Using this class helps continue the effort of defining JSON data in as few lines of code as possible.
    /// </remarks>
    public class JsonPropertyDefinition
    {
      
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="JsonPropertyDefinition"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        public JsonPropertyDefinition(string propertyName)
            : this(propertyName, null, null, null, true, false)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="JsonPropertyDefinition"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="browsable">Can the property be viewed in a control such as PropertyGrid.</param>
        /// <param name="readOnly">Can the property be edited?</param>
        public JsonPropertyDefinition(string propertyName, bool browsable, bool readOnly)
            : this(propertyName, null, null, null, browsable, readOnly)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="JsonPropertyDefinition"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="category">A category the property belongs.</param>
        /// <param name="description">A description of the property.</param>
        /// <param name="defaultValue">The default value if the property.</param>
        /// <param name="browsable">Can the property be viewed in a control such as PropertyGrid.</param>
        /// <param name="readOnly">Can the property be edited?</param>
        public JsonPropertyDefinition(string propertyName, string category, string description, JsonValue defaultValue, bool browsable, bool readOnly)
        {
            PropertyName = propertyName;
            Category = category;
            Description = description;
            DefaultValue = defaultValue;
            Browsable = browsable;
            ReadOnly = readOnly;
        }

        /// <summary>
        /// Creates a new instance of <see cref="JsonPropertyDefinition"/>.
        /// </summary>
        /// <param name="property"></param>
        internal JsonPropertyDefinition(System.Reflection.PropertyInfo property) : this(property.Name)
        {
            foreach (var att in property.GetCustomAttributes(true))
            {
                Type t = att.GetType();
                if (t == typeof(CategoryAttribute))
                    Category = (att as CategoryAttribute).Category;
                else if (t == typeof(DescriptionAttribute))
                    Description = (att as DescriptionAttribute).Description;
                else if (t == typeof(DefaultValueAttribute))
                    try
                    {
                        DefaultValue = (JsonValue)(att as DefaultValueAttribute).Value;
                    }
                    catch (Exception)
                    { }
                else if (t == typeof(BrowsableAttribute))
                    Browsable = (att as BrowsableAttribute).Browsable;
                else if (t == typeof(ReadOnlyAttribute))
                    ReadOnly = (att as ReadOnlyAttribute).IsReadOnly;

            }
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Can the property be viewed in a control such as PropertyGrid.
        /// </summary>
        public bool Browsable
        {
            get; set;
        }

        /// <summary>
        /// A category the property belongs.
        /// </summary>
        public string Category
        {
            get; set;
        }

        /// <summary>
        /// The default value if the property.
        /// </summary>
        public JsonValue DefaultValue
        {
            get; set;
        }

        /// <summary>
        /// A description of the property.
        /// </summary>
        public string Description
        {
            get; set;
        }

        /// <summary>
        /// Returns true if one or more of the attribute properties are not their default value. 
        /// </summary>
        public bool HasAttributes
        {
            get
            {
                return !string.IsNullOrEmpty(Category)
                    | !string.IsNullOrEmpty(Description)
                    | DefaultValue != null
                    | Browsable != true
                    | ReadOnly != false;
            }
        }

        /// <summary>
        /// The name of the property.
        /// </summary>
        public string PropertyName
        {
            get; set;
        }

        /// <summary>
        /// Can the property be edited?
        /// </summary>
        public bool ReadOnly
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Creates a new instance of <see cref="JsonPropertyDefinition"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        public static JsonPropertyDefinition Create(string propertyName)
        {
            return new JsonPropertyDefinition(propertyName);
        }

        /// <summary>
        /// Creates a new instance of <see cref="JsonPropertyDefinition"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="browsable">Can the property be viewed in a control such as PropertyGrid.</param>
        /// <param name="readOnly">Can the property be edited?</param>
        public static JsonPropertyDefinition Create(string propertyName, bool browsable, bool readOnly)
        {
            return new JsonPropertyDefinition(propertyName, browsable, readOnly);
        }

        /// <summary>
        /// Creates a new instance of <see cref="JsonPropertyDefinition"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="category">A category the property belongs.</param>
        /// <param name="description">A description of the property.</param>
        /// <param name="defaultValue">The default value if the property.</param>
        /// <param name="browsable">Can the property be viewed in a control such as PropertyGrid.</param>
        /// <param name="readOnly">Can the property be edited?</param>
        public static JsonPropertyDefinition Create(string propertyName, string category, string description, JsonValue defaultValue, bool browsable, bool readOnly)
        {
            return new JsonPropertyDefinition(propertyName, category, description, defaultValue, browsable, readOnly);
        }

        /// <summary>
        /// Creates an attribute definition that can be inserted into the <see cref="JsonObject.AttributesTable"/>.
        /// </summary>
        /// <returns>A new attribute entry as <see cref="JsonObject"/>.</returns>
        internal JsonObject CreateAttributeEntry()
        {
            JsonObject e = new JsonObject();
            if (!string.IsNullOrEmpty(Category))
                e.Add("Category", Category);
            if (!string.IsNullOrEmpty(Description))
                e.Add("Description", Description);
            if (DefaultValue != null)
                e.Add("DefaultValue", DefaultValue);
            if (!Browsable)
                e.Add("Browsable", false);
            if (ReadOnly)
                e.Add("ReadOnly", true);
            return e;
        }

        /// <summary>
        /// Creates an attribute definition and then inserts it into the <see cref="JsonObject.AttributesTable"/>.
        /// </summary>
        /// <param name="attributesTable">An <see cref="JsonObject.AttributesTable"/> to add the entry to.</param>
        /// <returns>A new attribute entry as <see cref="JsonObject"/>.</returns>
        internal JsonObject CreateAttributeEntry(JsonObject attributesTable)
        {
            JsonObject att = CreateAttributeEntry();
            if (att.Count > 0 && attributesTable != null)
                attributesTable[PropertyName] = att;
            return att;
        }

        #endregion Methods
    }

    /// <summary>
    /// Indicates that a private property should be serialized. Also used to rename a property when serializing and deserializing.
    /// </summary>
    /// <example>
    /// <code>
    ///		public class TestClass
    ///		{
    ///		    // This property will not serialize since it is not decorated with the <see cref="JsonPropertyAttribute"/>.
    ///		    private string DontShowMe { get; set; }
    ///         
    ///		    [JsonProperty]
    ///			private string SerializeMe { get; set; }
    ///			
    ///         [JsonProperty("NewName")]
    ///         public string OldName { get; set; }
    ///		}
    ///	</code>
    /// </example>
    [System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class JsonPropertyAttribute : Attribute
    {
        readonly string _jsonProperty;

        /// <summary>
        /// Creates a new instance of <see cref="JsonPropertyAttribute"/>.
        /// </summary>
        public JsonPropertyAttribute()
        {
            this._jsonProperty = null;
        }

        /// <summary>
        /// Creates a new instance of <see cref="JsonPropertyAttribute"/>.
        /// </summary>
        /// <param name="jsonProperty">The name of the JSON property to be matched.</param>
        public JsonPropertyAttribute(string jsonProperty)
        {
            this._jsonProperty = jsonProperty;
        }

        /// <summary>
        /// Gets the JSON property name to be matched.
        /// </summary>
        public string JsonPropertyName
        {
            get { return _jsonProperty; }
        }
        
        /// <summary>
        /// Gets whether there is a property name set.
        /// </summary>
        public bool HasNameOverride
        {
            get { return !string.IsNullOrEmpty(_jsonProperty); }
        }
    }
}