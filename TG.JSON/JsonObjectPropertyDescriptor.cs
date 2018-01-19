namespace TG.JSON
{
    using System;
    using System.ComponentModel;

    //public sealed partial class JsonObject
    //{

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
            JsonValue jsonComponent = component as JsonValue;
            if (jsonComponent != null)
            {
                switch (jsonComponent.ValueType)
                {
                    case JsonValueTypes.String:
                        return (string)jsonComponent;
                    case JsonValueTypes.Object:
                        JsonValue value = (component as JsonObject)[Name];
                        switch (value.ValueType)
                        {
                            case JsonValueTypes.String:
                                if (_propertyType == typeof(DateTime))
                                    return (DateTime)value;
                                else
                                    return (string)value;
                            default:
                                return value;
                        }
                    
                    default:
                        return component;
                }
            }
            //if (component is JsonObject)
            //{
            //    JsonValue v = (component as JsonObject)[Name];
            //    switch (v.ValueType)
            //    {
            //        case JsonValueTypes.String:
            //            if (_propertyType == typeof(DateTime))
            //                return (DateTime)v;
            //            else
            //                return (string)v;
            //        case JsonValueTypes.Object:
            //        case JsonValueTypes.Array:
            //            return v;
            //        case JsonValueTypes.Number:
            //            return (double)v;
            //        case JsonValueTypes.Boolean:
            //            return (bool)v;
            //        case JsonValueTypes.Binary:
            //            return (byte[])v;
            //        case JsonValueTypes.Null:
            //            return null;
            //        default:
            //            break;
            //    }
            //    return ((JsonObject)component)[Name];
            //}
            else if (Owner != null)
                return Owner[Name];

            return null;
        }

        /// <summary>
        /// Resets the value of the property with the value from <see cref="DefaultValue"/>.
        /// </summary>
        /// <param name="component">The component who's value should be reset.</param>
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
        /// A method used to set the category.
        /// </summary>
        /// <param name="category"></param>
        public void SetCategory(string category)
        {
            _category = category;
        }

        /// <summary>
        /// Sets the value of the property.
        /// </summary>
        /// <param name="component">The properties who's value should be set.</param>
        /// <param name="value">The value that should be set to the property.</param>
        public override void SetValue(object component, object value)
        {
            JsonValue jsonComponent = component as JsonValue;
            LoopNext:
            if (jsonComponent == null) return;
            switch (jsonComponent.ValueType)
            {
                case JsonValueTypes.String:
                    (jsonComponent as JsonString).Value = value as string;
                    break;
                case JsonValueTypes.Object:
                    JsonObject obj = jsonComponent as JsonObject;
                    if (value is JsonValue)
                        obj[Name] = ((JsonValue)value).Clone();
                    else
                    {
                        jsonComponent = obj[Name];
                        goto LoopNext;
                        //obj[Name] = obj.ValueFromObject(value);
                    }

                    break;
                case JsonValueTypes.Array:

                    break;
                case JsonValueTypes.Number:
                    (jsonComponent as JsonNumber).Value = Convert.ToDouble(value);
                    break;
                case JsonValueTypes.Boolean:
                    (jsonComponent as JsonBoolean).Value = Convert.ToBoolean(value);
                    break;
                case JsonValueTypes.Binary:
                    (jsonComponent as JsonBinary).Value = value as byte[];
                    break;
                default:
                    break;
            }
            OnValueChanged(component, EventArgs.Empty);

            Owner?.OnPropertyChanged(Name);

            //if (!(component is JsonObject))
            //    component = Owner;
            //if (component is JsonObject)
            //{
            //    JsonObject obj = (JsonObject)component;
            //    if (value is JsonValue)
            //        obj[Name] = ((JsonValue)value).Clone();
            //    else
            //        obj[Name] = obj.ValueFromObject(value);
            //    /*else if (value is string)
            //        obj[Name] = new JsonString((string)value);
            //    else if (value is bool)
            //        obj[Name] = new JsonBoolean((bool)value);
            //    else if (value is decimal)
            //        obj[Name] = new JsonNumber((decimal)value);
            //    else if (value is int)
            //        obj[Name] = new JsonNumber((int)value);
            //    else if (value == null && CanSetNull)
            //        obj[Name] = new JsonNull();
            //    else
            //        obj[Name] = new JsonString();*/
            //    OnValueChanged(component, EventArgs.Empty);
            //    if (Owner != null)
            //        Owner.OnPropertyChanged(Name);
            //}
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
    //}
}