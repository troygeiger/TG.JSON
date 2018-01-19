namespace TG.JSON
{
    using System;
    using System.Collections.Generic;
    using System.Text;


    /// <summary>
    /// Represents a string json value.
    /// </summary>
    /// <remarks>
    ///	Does not escape or unescape Unicode.
    /// </remarks>
#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough()]
#endif
    //[System.ComponentModel.TypeConverter(typeof(JsonStringTypeConverter))]
    [System.ComponentModel.TypeConverter(typeof(System.ComponentModel.StringConverter))]
    public class JsonString : JsonValue
    {
        #region Fields

        string _value;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="JsonString"/> class with an empty value.
        /// </summary>
        public JsonString()
        {
            this.Value = "";
        }

        /// <summary>
        /// Initialize a new instance of <see cref="JsonString"/> with the value specified by <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The <see cref="string"/> value to be set to the new instance's value.</param>
        public JsonString(string value)
        {
            this.Value = value;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Check to see if the value contains a <see cref="DateTime"/> value.
        /// </summary>
        public bool ContainsDateTime
        {
            get
            {
                DateTime d;
                return DateTime.TryParse(Value, out d);
            }
        }

        /// <summary>
        /// Indicate whether the value should be encrypted when calling <see cref="ToString"/>.
        /// </summary>
        public bool EncryptValue { get; set; }

        /// <summary>
        /// Gets or Sets the string value of the current instance.
        /// </summary>
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (value != null && value.StartsWith("crypto:"))
                {
                    EncryptValue = true;
                    _value = DecryptString(value);
                }
                else
                {
                    _value = value;
                }

                OnValueChanged();
            }
        }

        /// <summary>
        /// Returns <see cref="JsonValueTypes.String"/>.
        /// </summary>
		public override JsonValueTypes ValueType
        {
            get { return JsonValueTypes.String; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Creates a new instance of <see cref="JsonString"/> with an exact copy of it's value.
        /// </summary>
        public override JsonValue Clone()
        {
            return new JsonString(this.Value) {
                EncryptValue = this.EncryptValue,
                GlobalEncryptionHandler = this.GlobalEncryptionHandler
            };
        }

        /// <summary>
        /// Determines whether the specified<see cref= "System.Object" /> is equal to the current<see cref="JsonString" />.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="System.Object" /> is equal to the current <see cref="JsonString" />; otherwise, false.
        /// </returns>
        /// <param name="obj">
        /// The <see cref="System.Object" /> to compare with the current <see cref="JsonString" />.
        /// </param>
		public override bool Equals(object obj)
        {
            if (obj is JsonString)
                return this.Value == ((JsonString)obj).Value;
            else if (obj is string)
                return this.Value == (string)obj;
            return base.Equals(obj);
        }

        /// <summary>Serves as a hash function for a particular type. </summary>
        /// <returns>A hash code for the current <see cref="JsonString" />.</returns>
		public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Returns the <see cref="string"/> value.
        /// </summary>
        public override string ToString()
        {
            return _value;
        }

        internal string Escape(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                value = value.Replace("\\", "\\\\");
                value = value.Replace("\r", "\\r");
                value = value.Replace("\n", "\\n");
                value = value.Replace("\"", "\\\"");
                value = value.Replace("\t", "\\t");
                //value = value.Replace("/", "\\/");
            }
            return value;
        }

        internal override string InternalToString(Formatting format, int depth)
        {
            return string.Format("\"{0}\"", Escape(EncryptValue ? EncryptString(_value) : _value));
        }

        internal string Unescape(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                value = value.Replace("\\r", "\r");
                value = value.Replace("\\n", "\n");
                value = value.Replace("\\\"", "\"");
                value = value.Replace("\\t", "\t");
                value = value.Replace("\\\\", "\\");
                //value = value.Replace("\\/", "/");
            }
            return value;
        }

        internal string EncryptString(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                IEncryptionHandler handler = this.GetAvailableEncryptionHandler();
                if (handler != null)
                {
                    return string.Concat("crypto:", handler.EncryptBase64(value));
                }
            }
            return "crypto:";
        }

        internal string DecryptString(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                IEncryptionHandler handler = this.GetAvailableEncryptionHandler();
                if (handler != null)
                {
                    if (value.StartsWith("crypto:"))
                    {
                        return handler.DecryptBase64(value.Substring(7));
                    }
                    else
                    {
                        return handler.DecryptBase64(value);
                    }
                }
            }
            return value;
        }

        #endregion Methods
    }
}