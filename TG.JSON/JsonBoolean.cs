using System;
using System.Collections.Generic;
using System.Text;

namespace TG.JSON
{

#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough()]
#endif
    /// <summary>
    /// Represents a json boolean value.
    /// </summary>
    [System.ComponentModel.TypeConverter(typeof(System.ComponentModel.BooleanConverter))]
    public class JsonBoolean : JsonValue
    {
        #region Fields

        bool _value = false;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="JsonBoolean"/> with a value of false.
        /// </summary>
        public JsonBoolean()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="JsonBoolean"/> with a value specified by the value parameter.
        /// </summary>
        /// <param name="value">The value to initialize as.</param>
        public JsonBoolean(bool value)
        {
            this.Value = value;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or Sets the bool value of this <see cref="TG.JSON.JsonBoolean"/>
        /// </summary>
        public bool Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnValueChanged();
            }
        }

        /// <summary>
        /// Returns <see cref="JsonValueTypes.Boolean"/>.
        /// </summary>
        public override JsonValueTypes ValueType
        {
            get { return JsonValueTypes.Boolean; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Returns the bool value from <see cref="JsonBoolean.Value"/>
        /// </summary>
        /// <example><code>
        /// JsonBoolean jb = true;
        /// bool b = jb;
        /// </code></example>
        /// <param name="value">The value to cast.</param>
        public static implicit operator bool(JsonBoolean value)
        {
            return value.Value;
        }

        /// <summary>
        /// Returns a new instance of <see cref="JsonBoolean"/> with the value specified by <paramref name="value"/>.
        /// </summary>
        /// <example><code>
        /// JsonBoolean jb = true;
        /// bool b = jb;
        /// </code></example>
        /// <param name="value">The value to initialize the new <see cref="JsonBoolean"/> with.</param>
        public static implicit operator JsonBoolean(bool value)
        {
            return new JsonBoolean(value);
        }

        /// <summary>
        /// Performs a not equals operation on argument "left" <see cref="JsonBoolean.Value"/> and argument "right" <see cref="JsonBoolean.Value"/>.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>Returns true if the <see cref="JsonBoolean.Value"/>s are not equal; otherwise false.</returns>
        public static bool operator !=(JsonBoolean left, JsonBoolean right)
        {
            return left?.Value != right?.Value;
        }

        /// <summary>
        /// Performs a equals operation on argument "left" <see cref="JsonBoolean.Value"/> and argument "right" <see cref="JsonBoolean.Value"/>.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>Returns true if the <see cref="JsonBoolean.Value"/>s are equal; otherwise false.</returns>
        public static bool operator ==(JsonBoolean left, JsonBoolean right)
        {
            return left?.Value == right?.Value;
        }

        /// <summary>
        /// Creates a new instance of <see cref="JsonBoolean"/> with an exact copy of it's value.
        /// </summary>
        public override JsonValue Clone()
        {
            return new JsonBoolean(this.Value);
        }

        /// <summary>
        /// Compares an object value to this <see cref="JsonBoolean.Value"/>.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
#if NETSTANDARD1_0
        public bool Equals(object obj)
#else
        public override bool Equals(object obj)
#endif
        {
            if (obj is bool)
                return this.Value == (bool)obj;
            else if (obj is JsonBoolean)
                return this.Value == ((JsonBoolean)obj).Value;
            return base.Equals(obj);
        }

        /// <summary>Serves as a hash function for a particular type. </summary>
        /// <returns>A hash code for the current <see cref="JsonBoolean" />.</returns>
#if NETSTANDARD1_0
        public int GetHashCode()
#else
        public override int GetHashCode()
#endif
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Converts the value of this instance to its equivalent string representation (either "true" or "false").
        /// </summary>
#if NETSTANDARD1_0
        public string ToString()
#else
        public override string ToString() 
#endif
        {
            return this.Value.ToString().ToLower();
        }

        internal override string InternalToString(Formatting format, int depth)
        {
            return this.ToString();
        }

        #endregion Methods
    }
}