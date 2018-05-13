using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace TG.JSON
{

    /// <summary>
    /// Represents a json number value.
    /// </summary>
    /// <example>
    /// <code>
    /// JsonNumber a = 1.5f;
    /// float af = a + 5;
    /// //af = 6.5
    /// 
    /// JsonNumber b = new JsonNumber(5);
    /// </code>
    /// </example>
    /// <seealso cref="JsonValue"/>
#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough()]
#endif
    [System.ComponentModel.TypeConverter(typeof(System.ComponentModel.DecimalConverter))] 
    public class JsonNumber : JsonValue
    {
        #region Fields
        static CultureInfo en_US = new CultureInfo("en-US");
        double _value = 0;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="JsonNumber"/> with a value of zero.
        /// </summary>
        public JsonNumber()
        {
            this.Value = 0;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="JsonNumber"/> with a value provided by the value parameter.
        /// </summary>
        /// <param name="value">The decimal value to set the new instance. This get converted to a double.</param>
        public JsonNumber(decimal value)
        {
            this.Value = Convert.ToDouble(value);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="JsonNumber"/> with a value provided by the value parameter.
        /// </summary>
        /// <param name="value">The short value to set the new instance. This get converted to a double.</param>
        public JsonNumber(short value)
        {
            this.Value = Convert.ToDouble(value);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="JsonNumber"/> with a value provided by the value parameter.
        /// </summary>
        /// <param name="value">The int value to set the new instance. This get converted to a double.</param>
        public JsonNumber(int value)
        {
            this.Value = Convert.ToDouble(value);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="JsonNumber"/> with a value provided by the value parameter.
        /// </summary>
        /// <param name="value">The long value to set the new instance. This get converted to a double.</param>
        public JsonNumber(long value)
        {
            this.Value = Convert.ToDouble(value);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="JsonNumber"/> with a value provided by the value parameter.
        /// </summary>
        /// <param name="value">The float value to set the new instance. This get converted to a double.</param>
        public JsonNumber(float value)
        {
            this.Value = Convert.ToDouble(value);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="JsonNumber"/> with a value provided by the value parameter.
        /// </summary>
        /// <param name="value">The double value to set the new instance.</param>
        public JsonNumber(double value)
        {
            this.Value = value;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or Sets the double value associated with this <see cref="JsonNumber"/>.
        /// </summary>
        public double Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnValueChanged();
            }
        }

        /// <summary>
        /// Always returns <see cref="JsonValueTypes.Number"/>.
        /// </summary>
		public override JsonValueTypes ValueType
        {
            get { return JsonValueTypes.Number; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Implicitly casts a <see cref="JsonNumber"/> to a <see cref="decimal"/> value.
        /// </summary>
        /// <example><code>
        /// JsonNumber n = new JsonNumber(5);
        /// decimal d = n;
        /// </code></example>
        /// <param name="value">The value to be cast.</param>
        public static implicit operator decimal (JsonNumber value)
        {
            return Convert.ToDecimal(value.Value);
        }

        /// <summary>
        /// Implicitly casts a <see cref="JsonNumber"/> to a <see cref="int"/> value.
        /// </summary>
        /// <example><code>
        /// JsonNumber n = new JsonNumber(5);
        /// decimal d = n;
        /// </code></example>
        /// <param name="value">The value to be cast.</param>
        public static implicit operator int (JsonNumber value)
        {
            return Convert.ToInt32(value.Value);
        }

        /// <summary>
        /// Implicitly casts a <see cref="JsonNumber"/> to a <see cref="long"/> value.
        /// </summary>
        /// <example><code>
        /// JsonNumber n = new JsonNumber(5);
        /// long d = n;
        /// </code></example>
        /// <param name="value">The value to be cast.</param>
        public static implicit operator long (JsonNumber value)
        {
            return Convert.ToInt64(value.Value);
        }

        /// <summary>
        /// Implicitly casts a <see cref="JsonNumber"/> to a <see cref="float"/> value.
        /// </summary>
        /// <example><code>
        /// JsonNumber n = new JsonNumber(5.3f);
        /// float d = n;
        /// </code></example>
        /// <param name="value">The value to be cast.</param>
        public static implicit operator float (JsonNumber value)
        {
            return Convert.ToSingle(value.Value);
        }

        /// <summary>
        /// Implicitly casts a <see cref="decimal"/> to a <see cref="JsonNumber"/> value.
        /// </summary>
        /// <example><code>
        /// decimal d = 5;
        /// JsonNumber n = d;
        /// </code></example>
        /// <param name="value">The value to be cast.</param>
        public static implicit operator JsonNumber(decimal value)
        {
            return new JsonNumber(value);
        }

        /// <summary>
        /// Implicitly casts a <see cref="double"/> to a <see cref="JsonNumber"/> value.
        /// </summary>
        /// <example><code>
        /// double d = 5;
        /// JsonNumber n = d;
        /// </code></example>
        /// <param name="value">The value to be cast.</param>
        public static implicit operator JsonNumber(double value)
        {
            return new JsonNumber(value);
        }

        /// <summary>
        /// Implicitly casts a <see cref="int"/> to a <see cref="JsonNumber"/> value.
        /// </summary>
        /// <example><code>
        /// int d = 5;
        /// JsonNumber n = d;
        /// </code></example>
        /// <param name="value">The value to be cast.</param>
        public static implicit operator JsonNumber(int value)
        {
            return new JsonNumber(Convert.ToDecimal(value));
        }

        /// <summary>
        /// Implicitly casts a <see cref="long"/> to a <see cref="JsonNumber"/> value.
        /// </summary>
        /// <example><code>
        /// long d = 5;
        /// JsonNumber n = d;
        /// </code></example>
        /// <param name="value">The value to be cast.</param>
        public static implicit operator JsonNumber(long value)
        {
            return new JsonNumber(value);
        }

        /// <summary>
        /// Implicitly casts a <see cref="float"/> to a <see cref="JsonNumber"/> value.
        /// </summary>
        /// <example><code>
        /// float d = 5;
        /// JsonNumber n = d;
        /// </code></example>
        /// <param name="value">The value to be cast.</param>
        public static implicit operator JsonNumber(float value)
        {
            return new JsonNumber(Convert.ToDecimal(value));
        }

        /// <summary>
        /// Performs a equals operation between to <see cref="JsonNumber"/>.
        /// </summary>
        /// <param name="left">The left side of the operation.</param>
        /// <param name="right">The right side of the operation</param>
        /// <example><code>
        /// JsonNumber l = 2;
        /// JsonNumber r = 4;
        /// Console.Write(l == r);
        /// //Output = false;
        /// </code></example>
        /// <returns>Returns true if the left <see cref="JsonNumber.Value"/> does not equal the right <see cref="JsonNumber.Value"/>; otherwise false;</returns>
        public static bool operator ==(JsonNumber left, JsonNumber right)
        {
            return left.Value == right.Value;
        }

        /// <summary>
        /// Performs a not equals operation between to <see cref="JsonNumber"/>.
        /// </summary>
        /// <param name="left">The left side of the operation.</param>
        /// <param name="right">The right side of the operation</param>
        /// <example><code>
        /// JsonNumber l = 2;
        /// JsonNumber r = 4;
        /// Console.Write(l != r);
        /// //Output = true;
        /// </code></example>
        /// <returns>Returns true if the left <see cref="JsonNumber.Value"/> does not equal the right <see cref="JsonNumber.Value"/>; otherwise false;</returns>
        public static bool operator !=(JsonNumber left, JsonNumber right)
        {
            return left.Value != right.Value;
        }

        /// <summary>
        /// Multiplies the left and right values.
        /// </summary>
        /// <param name="left">The left side of the multiplication equation.</param>
        /// <param name="right">The right side of the multiplication equation.</param>
        /// <returns>Returns the results of multiplying the left and right parameters.</returns>
        /// <example><code>
        /// JsonNumber l = 2;
        /// JsonNumber r = 4;
        /// Console.Write(l * r);
        /// //Output = 8;
        /// </code></example>
        public static JsonNumber operator *(JsonNumber left, JsonNumber right)
        {
            return new JsonNumber(left.Value * right.Value);
        }

        /// <summary>
        /// Adds the left and right values.
        /// </summary>
        /// <param name="left">The left side of the addition equation.</param>
        /// <param name="right">The right side of the addition equation.</param>
        /// <returns>Returns the results of adding the left and right parameters.</returns>
        /// <example><code>
        /// JsonNumber l = 2;
        /// JsonNumber r = 4;
        /// Console.Write(l + r);
        /// //Output = 6;
        /// </code></example>
        public static JsonNumber operator +(JsonNumber left, JsonNumber right)
        {
            return new JsonNumber(left.Value + right.Value);
        }

        /// <summary>
        /// Subtracts the left and right values.
        /// </summary>
        /// <param name="left">The left side of the subtraction equation.</param>
        /// <param name="right">The right side of the subtraction equation.</param>
        /// <returns>Returns the results of subtracting the left and right parameters.</returns>
        /// <example><code>
        /// JsonNumber l = 2;
        /// JsonNumber r = 4;
        /// Console.Write(l - r);
        /// //Output = -2;
        /// </code></example>
        public static JsonNumber operator -(JsonNumber left, JsonNumber right)
        {
            return new JsonNumber(left.Value - right.Value);
        }

        /// <summary>
        /// Divides the left and right values.
        /// </summary>
        /// <param name="left">The left side of the division equation.</param>
        /// <param name="right">The right side of the division equation.</param>
        /// <returns>Returns the results of dividing the left and right parameters.</returns>
        /// <example><code>
        /// JsonNumber l = 4;
        /// JsonNumber r = 2;
        /// Console.Write(l * r);
        /// //Output = 2;
        /// </code></example>
        public static JsonNumber operator /(JsonNumber left, JsonNumber right)
        {
            return new JsonNumber(left.Value / right.Value);
        }

        /// <summary>
        /// Creates a new instance of <see cref="JsonNumber"/> with an exact copy of it's value.
        /// </summary>
		public override JsonValue Clone()
        {
            return new JsonNumber(this.Value);
        }

        /// <summary>
        /// Determines whether the specified<see cref= "System.Object" /> is equal to the current<see cref="JsonNumber" />.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="System.Object" /> is equal to the current <see cref="JsonNumber" />; otherwise, false.
        /// </returns>
        /// <param name="obj">
        /// The <see cref="System.Object" /> to compare with the current <see cref="JsonNumber" />.
        /// </param>
#if NETSTANDARD1_0
        public bool Equals(object obj)
#else
        public override bool Equals(object obj) 
#endif
        {
            if (obj is JsonNumber)
                return this.Value == ((JsonNumber)obj).Value;
            else if (obj is decimal)
                return this.Value == (double)obj;
            else if (obj is int)
                return this.Value == (int)obj;
            return base.Equals(obj);
        }

        /// <summary>Serves as a hash function for a particular type. </summary>
        /// <returns>A hash code for the current <see cref="JsonNumber" />.</returns>
#if NETSTANDARD1_0
        public int GetHashCode()
#else
        public override int GetHashCode() 
#endif
        {
            return base.GetHashCode();
        }

        /// <summary>Converts the numeric value of this instance to its equivalent string representation.</summary>
        /// <returns>The string representation of the value of this instance.</returns>
#if NETSTANDARD1_0
        public string ToString()
#else
        public override string ToString() 
#endif
        {
            return this.Value.ToString(en_US);
        }

        internal override string InternalToString(Formatting format, int depth)
        {
            return this.ToString();
        }

        #endregion Methods
    }
}