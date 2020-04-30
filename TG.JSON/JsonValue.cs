using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace TG.JSON
{

    /// <summary>
    /// TG.JSON is an easy to use library for interacting with JSON in .NET. TG.JSON was developed
    /// to quickly create JSON object and arrays with as few lines of code as possible.
    /// All JsonValue objects can be directly cast to or from a value type, such as string, int, etc. Ex. JsonValue v = 1; JsonValue b = true;
    /// </summary>
    /// <remarks>
    /// <listheader>Features</listheader>
    /// <list type="bullet">
    /// 	<item>Fast parsing and outputting ToString.</item>
    /// 	<item>Easy to initialize JsonObjects and JsonArrays.</item>
    /// 	<item>Cast JsonValues directly to value types.</item>
    /// 	<item>Serialize object into JsonObjects. Multiple object could be serialize into a single JsonObject.</item>
    /// 	<item>De-serialize into any type with matching properties.</item>
    /// 	<item>Very small file size.</item>
    /// 	<item>ToString output can be in several formats and with indentation.</item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code>
    ///    JsonObject obj = new JsonObject(
    ///        "name", "John Doe",
    ///        "age", 32,
    ///        "isMarried", true,
    ///        "notes", null
    ///        );
    ///    if ((bool)obj["isMarried"])
    ///    {
    ///        Console.WriteLine((string)obj["name"]);
    ///        Console.WriteLine((int)obj["age"]);
    ///    }
    ///    Console.WriteLine(obj.ToString());
    ///
    ///    // Values can be directly set to JsonValues.
    ///    JsonValue v = 1;
    ///    JsonValue b = true;
    /// </code>
    /// </example>
    internal class NamespaceDoc { }

    #region Enumerations

    /// <summary>
    /// This represents the format of a <see cref="JsonValue"/> when outputting from <seealso cref="JsonValue.ToString(Formatting)"/>.
    /// </summary>
    public enum Formatting
    {
        /// <summary>
        /// All values in-line. No whitespace or carriage returns.
        /// </summary>
        Compressed,

        /// <summary>
        /// All values are separated by spaces. Ex. [ "Hello" , "World" ]
        /// </summary>
        Spaces,

        /// <summary>
        /// All values are indented in a hierarchical format using the parent <see cref="JsonValue"/>'s <seealso cref="JsonValue.IndentString"/> value.
        /// </summary>
        Indented,

        /// <summary>
        /// Values are output as an in-line javascript value.
        /// </summary>
        JavascriptCompressed,

        /// <summary>
        /// Values are output as in indented javascript value using the parent <see cref="JsonValue"/>'s <seealso cref="JsonValue.IndentString"/> value.
        /// </summary>
        JavascriptIndented
    }

    /// <summary>
    /// Specified a value type for a <see cref="TG.JSON.JsonValue"/>.
    /// </summary>
    public enum JsonValueTypes
    {
        /// <summary>
        /// <see cref="JsonString"/>
        /// </summary>
        String,
        /// <summary>
        /// <see cref="JsonObject"/>
        /// </summary>
        Object,
        /// <summary>
        /// <see cref="JsonArray"/>
        /// </summary>
        Array,
        /// <summary>
        /// <see cref="JsonNumber"/>
        /// </summary>
        Number,
        /// <summary>
        /// <see cref="JsonBoolean"/>
        /// </summary>
        Boolean,
        /// <summary>
        /// <see cref="JsonBinary"/>
        /// </summary>
        Binary,
        /// <summary>
        /// <see cref="JsonNull"/>
        /// </summary>
        Null
    }

    #endregion Enumerations


    /// <summary>
    /// An abstract class that represents all json values.
    /// </summary>
    /// <remarks>
    /// Also contains operators to convert primitive values to <see cref="JsonValue"/>.
    /// </remarks>
#if !DEBUG
	[System.Diagnostics.DebuggerStepThrough()]
#endif
    public abstract class JsonValue
#if !NETSTANDARD1_0
        : IConvertible
#endif
    {
        #region Fields

        /// <summary>
        /// The mime type application/json
        /// </summary>
        public const string MIME_Type = "application/json";
        private JsonValue _parent = null;
        private static JsonNull generic = null;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="JsonValue"/>.
        /// </summary>
        public JsonValue()
        {
            this.IndentString = "\t";
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// Event that is called when a value has changed.
        /// </summary>
        public event EventHandler ValueChanged;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets or Sets the string to use for indentation formatting. Default is \t.
        /// </summary>
        public string IndentString
        {
            get;
            set;
        }

#if !NETSTANDARD1_0

        /// <summary>
        /// The <see cref="IEncryptionHandler"/> to use for encrypting values.
        /// </summary>
        public IEncryptionHandler GlobalEncryptionHandler { get; set; }

        /// <summary>
        /// Gets if the <see cref="GlobalEncryptionHandler"/> property is not null.
        /// </summary>
        public bool HasGlobalEncryptionHandler
        {
            get
            {
                return GlobalEncryptionHandler != null;
            }
        }

        /// <summary>
        /// Gets or Sets the encryption key to use for encrypting values.
        /// </summary>
        [Obsolete("To enable custom IEncryptionHandler, this property is being obsoleted.")]
        public string GlobalEncryptionKeyString
        {
            get
            {
                return GlobalEncryptionHandler?.EncryptionKeyAsString();
            }
            set
            {
                GlobalEncryptionHandler = new EncryptionHandler(value);
            }
        }

        /// <summary>
        /// Gets or Sets the encryption key to use for encrypting values.
        /// </summary>
        [Obsolete("To enable custom IEncryptionHandler, this property is being obsoleted.")]
        public byte[] GlobalEncryptionKeyBytes
        {
            get
            {
                return GlobalEncryptionHandler?.EncryptionKey;
            }
            set
            {
                GlobalEncryptionHandler = new EncryptionHandler(value);
            }
        }
#endif

        /// <summary>
        /// States that events, such as ValueChanged, should not be fired.
        /// </summary>
        public bool SuspendEvents
        {
            get;
            set;
        }

        /// <summary>
        /// The type of JSON value the inheriting class represents.
        /// </summary>
        public abstract JsonValueTypes ValueType
        {
            get;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// An operator used to convert a value to string.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator string(JsonValue value)
        {
            if (value is JsonNull || value == null)
                return null;
            else if (value.ValueType == JsonValueTypes.String)
                return (value as JsonString).Value;
            else
                return value.ToString();
        }

        /// <summary>
        /// An operator used to convert a value to boolean.
        /// </summary>
        /// <remarks>
        ///	<list type="bullet">
        ///		<item>
        ///			<term><see cref="JsonString"/></term>
        ///			<description>Parses string values "true" or "false".</description>
        ///		</item>
        ///		<item>
        ///			<term><see cref="JsonNumber"/></term>
        ///			<description>Returns true if value is greater than zero.</description>
        ///		</item>
        ///		<item>
        ///			<term><see cref="JsonBoolean"/></term>
        ///			<description>Returns the underlying boolean value.</description>
        ///		</item>
        ///		<item>Otherwise returns false.</item>
        /// </list>
        /// </remarks>
        /// <param name="value">The <see cref="JsonValue"/> to convert. </param>
        public static implicit operator bool(JsonValue value)
        {
            if (value == null)
                return false;
            switch (value.ValueType)
            {
                case JsonValueTypes.String:
                    bool v = false;
                    bool.TryParse((value as JsonString).Value, out v);
                    return v;
                case JsonValueTypes.Number:
                    return (value as JsonNumber).Value > 0;
                case JsonValueTypes.Boolean:
                    return (value as JsonBoolean).Value;
                default:
                    return false;
            }
        }

        /// <summary>
        /// An operator used to convert a value to <see cref="float"/>.
        /// </summary>
        /// <remarks>
        ///		<list type="bullet">
        ///			<item>
        ///				<term><see cref="JsonString"/></term>
        ///				<description>Parses the underlying string to <see cref="float"/>. Returns -1 if value could not be parsed.</description>
        ///			</item>
        ///			<item>
        ///				<term><see cref="JsonNumber"/></term>
        ///				<description>Converts the underlying <see cref="double"/> value to <see cref="float"/>.</description>
        ///			</item>
        ///			<item>
        ///				<term><see cref="JsonBoolean"/></term>
        ///				<description>Returns a value of one if the underlying value is true; otherwise zero.</description>
        ///			</item>
        ///			<item>Otherwise returns -1.</item>
        ///		</list>
        /// </remarks>
        /// <param name="value">The <see cref="JsonValue"/> to convert.</param>
        public static implicit operator float(JsonValue value)
        {
            if (value == null)
                return -1;
            switch (value.ValueType)
            {
                case JsonValueTypes.String:
                    float v = -1;
                    float.TryParse((value as JsonString).Value, out v);
                    return v;
                case JsonValueTypes.Number:
                    return Convert.ToSingle((value as JsonNumber).Value);
                case JsonValueTypes.Boolean:
                    return (value as JsonBoolean).Value ? 1 : 0;
                default:
                    return -1;
            }
        }

        /// <summary>
        /// An operator used to convert a value to <see cref="double"/>.
        /// </summary>
        /// <remarks>
        ///		<list type="bullet">
        ///			<item>
        ///				<term><see cref="JsonString"/></term>
        ///				<description>Parses the underlying string to <see cref="double"/>. Returns -1 if value could not be parsed.</description>
        ///			</item>
        ///			<item>
        ///				<term><see cref="JsonNumber"/></term>
        ///				<description>Returns the underlying <see cref="double"/> value.</description>
        ///			</item>
        ///			<item>
        ///				<term><see cref="JsonBoolean"/></term>
        ///				<description>Returns a value of one if the underlying value is true; otherwise zero.</description>
        ///			</item>
        ///			<item>Otherwise returns -1.</item>
        ///		</list>
        /// </remarks>
        /// <param name="value">The <see cref="JsonValue"/> to convert.</param>
        public static implicit operator double(JsonValue value)
        {
            if (value == null)
                return -1;
            switch (value.ValueType)
            {
                case JsonValueTypes.String:
                    double v = -1;
                    double.TryParse((value as JsonString).Value, out v);
                    return v;
                case JsonValueTypes.Number:
                    return Convert.ToDouble((value as JsonNumber).Value);
                case JsonValueTypes.Boolean:
                    return (value as JsonBoolean).Value ? 1 : 0;
                default:
                    return -1;
            }
        }

        /// <summary>
        /// An operator used to convert a value to <see cref="decimal"/>.
        /// </summary>
        /// <remarks>
        ///		<list type="bullet">
        ///			<item>
        ///				<term><see cref="JsonString"/></term>
        ///				<description>Parses the underlying string to <see cref="decimal"/>. Returns -1 if value could not be parsed.</description>
        ///			</item>
        ///			<item>
        ///				<term><see cref="JsonNumber"/></term>
        ///				<description>Converts the underlying <see cref="double"/> value to <see cref="decimal"/>.</description>
        ///			</item>
        ///			<item>
        ///				<term><see cref="JsonBoolean"/></term>
        ///				<description>Returns a value of one if the underlying value is true; otherwise zero.</description>
        ///			</item>
        ///			<item>Otherwise returns -1.</item>
        ///		</list>
        /// </remarks>
        /// <param name="value">The <see cref="JsonValue"/> to convert.</param>
        public static implicit operator decimal(JsonValue value)
        {
            if (value == null)
                return -1;
            switch (value.ValueType)
            {
                case JsonValueTypes.String:
                    decimal v = -1;
                    decimal.TryParse((value as JsonString).Value, out v);
                    return v;
                case JsonValueTypes.Number:
                    return (value as JsonNumber).Value;
                case JsonValueTypes.Boolean:
                    return (value as JsonBoolean).Value ? 1 : 0;
                default:
                    return -1;
            }
        }

        /// <summary>
        /// An operator used to convert a value to <see cref="int"/>.
        /// </summary>
        /// <remarks>
        ///		<list type="bullet">
        ///			<item>
        ///				<term><see cref="JsonString"/></term>
        ///				<description>Parses the underlying string to <see cref="int"/>. Returns -1 if value could not be parsed.</description>
        ///			</item>
        ///			<item>
        ///				<term><see cref="JsonNumber"/></term>
        ///				<description>Converts the underlying <see cref="double"/> value to <see cref="int"/>.</description>
        ///			</item>
        ///			<item>
        ///				<term><see cref="JsonBoolean"/></term>
        ///				<description>Returns a value of one if the underlying value is true; otherwise zero.</description>
        ///			</item>
        ///			<item>Otherwise returns -1.</item>
        ///		</list>
        /// </remarks>
        /// <param name="value">The <see cref="JsonValue"/> to convert.</param>
        public static implicit operator int(JsonValue value)
        {
            if (value == null)
                return -1;
            switch (value.ValueType)
            {
                case JsonValueTypes.String:
                    int v = -1;
                    int.TryParse((value as JsonString).Value, out v);
                    return v;
                case JsonValueTypes.Number:
                    return Convert.ToInt32((value as JsonNumber).Value);
                case JsonValueTypes.Boolean:
                    return (value as JsonBoolean).Value ? 1 : 0;
                default:
                    return -1;
            }
        }

        /// <summary>
        /// An operator used to convert a value to <see cref="short"/>.
        /// </summary>
        /// <remarks>
        ///		<list type="bullet">
        ///			<item>
        ///				<term><see cref="JsonString"/></term>
        ///				<description>Parses the underlying string to <see cref="short"/>. Returns -1 if value could not be parsed.</description>
        ///			</item>
        ///			<item>
        ///				<term><see cref="JsonNumber"/></term>
        ///				<description>Converts the underlying <see cref="double"/> value to <see cref="short"/>.</description>
        ///			</item>
        ///			<item>
        ///				<term><see cref="JsonBoolean"/></term>
        ///				<description>Returns a value of one if the underlying value is true; otherwise zero.</description>
        ///			</item>
        ///			<item>Otherwise returns -1.</item>
        ///		</list>
        /// </remarks>
        /// <param name="value">The <see cref="JsonValue"/> to convert.</param>
        public static implicit operator short(JsonValue value)
        {
            if (value == null)
                return -1;
            switch (value.ValueType)
            {
                case JsonValueTypes.String:
                    short v = -1;
                    short.TryParse((value as JsonString).Value, out v);
                    return v;
                case JsonValueTypes.Number:
                    return Convert.ToInt16((value as JsonNumber).Value);
                case JsonValueTypes.Boolean:
                    return (value as JsonBoolean).Value ? (short)1 : (short)0;
                default:
                    return -1;
            }
        }

        /// <summary>
        /// An operator used to convert a value to <see cref="long"/>.
        /// </summary>
        /// <remarks>
        ///		<list type="bullet">
        ///			<item>
        ///				<term><see cref="JsonString"/></term>
        ///				<description>Parses the underlying string to <see cref="long"/>. Returns -1 if value could not be parsed.</description>
        ///			</item>
        ///			<item>
        ///				<term><see cref="JsonNumber"/></term>
        ///				<description>Converts the underlying <see cref="double"/> value to <see cref="long"/>.</description>
        ///			</item>
        ///			<item>
        ///				<term><see cref="JsonBoolean"/></term>
        ///				<description>Returns a value of one if the underlying value is true; otherwise zero.</description>
        ///			</item>
        ///			<item>Otherwise returns -1.</item>
        ///		</list>
        /// </remarks>
        /// <param name="value">The <see cref="JsonValue"/> to convert.</param>
        public static implicit operator long(JsonValue value)
        {
            if (value == null)
                return -1;
            switch (value.ValueType)
            {
                case JsonValueTypes.String:
                    long v = -1;
                    long.TryParse((value as JsonString).Value, out v);
                    return v;
                case JsonValueTypes.Number:
                    return Convert.ToInt64((value as JsonNumber).Value);
                case JsonValueTypes.Boolean:
                    return (value as JsonBoolean).Value ? 1 : 0;
                default:
                    return -1;
            }
        }

        /// <summary>
        /// An operator used to convert a value to <see cref="byte"/>.
        /// </summary>
        /// <remarks>
        ///		<list type="bullet">
        ///			<item>
        ///				<term><see cref="JsonString"/></term>
        ///				<description>Parses the underlying string to <see cref="float"/>. Returns 0 if value could not be parsed.</description>
        ///			</item>
        ///			<item>
        ///				<term><see cref="JsonNumber"/></term>
        ///				<description>Converts the underlying <see cref="double"/> value to <see cref="byte"/>. If <see cref="JsonNumber.Value"/> is greater than 255, <see cref="byte.MaxValue"/> is returned.</description>
        ///			</item>
        ///			<item>
        ///				<term><see cref="JsonBoolean"/></term>
        ///				<description>Returns a value of one if the underlying value is true; otherwise zero.</description>
        ///			</item>
        ///			<item>Otherwise returns -1.</item>
        ///		</list>
        /// </remarks>
        /// <param name="value">The <see cref="JsonValue"/> to convert.</param>
        public static implicit operator byte(JsonValue value)
        {
            if (value == null)
                return 0;
            switch (value.ValueType)
            {
                case JsonValueTypes.String:
                    byte b = 0;
                    byte.TryParse((value as JsonString).Value, out b);
                    return b;
                case JsonValueTypes.Number:
                    JsonNumber n = value as JsonNumber;
                    if (n.Value >= 0 && n.Value < 255)
                        return Convert.ToByte(n.Value);
                    if (n.Value > 255)
                        return byte.MaxValue;
                    return byte.MinValue;
                case JsonValueTypes.Boolean:
                    return Convert.ToByte((value as JsonBoolean).Value);
                case JsonValueTypes.Binary:
                    JsonBinary bin = value as JsonBinary;
                    if (bin.Value.Length > 0)
                        return bin.Value[0];
                    else
                        return byte.MinValue;
                default:
                    return byte.MinValue;
            }
        }

        /// <summary>
        /// An operator used to convert a value to <see cref="ushort"/>.
        /// </summary>
        /// <remarks>
        ///		<list type="bullet">
        ///			<item>
        ///				<term><see cref="JsonString"/></term>
        ///				<description>Parses the underlying string to <see cref="ushort"/>. Returns 0 if value could not be parsed.</description>
        ///			</item>
        ///			<item>
        ///				<term><see cref="JsonNumber"/></term>
        ///				<description>Converts the underlying <see cref="double"/> value to <see cref="ushort"/>.</description>
        ///			</item>
        ///			<item>
        ///				<term><see cref="JsonBoolean"/></term>
        ///				<description>Returns a value of one if the underlying value is true; otherwise zero.</description>
        ///			</item>
        ///			<item>Otherwise returns 0.</item>
        ///		</list>
        /// </remarks>
        /// <param name="value">The <see cref="JsonValue"/> to convert.</param>
        public static implicit operator ushort(JsonValue value)
        {
            if (value == null)
                return 0;
            switch (value.ValueType)
            {
                case JsonValueTypes.String:
                    ushort v = 0;
                    ushort.TryParse((value as JsonString).Value, out v);
                    return v;
                case JsonValueTypes.Number:
                    return Convert.ToUInt16((value as JsonNumber).Value);
                case JsonValueTypes.Boolean:
                    return (ushort)((value as JsonBoolean).Value ? 1 : 0);
                default:
                    return 0;
            }
        }

        /// <summary>
        /// An operator used to convert a value to <see cref="uint"/>.
        /// </summary>
        /// <remarks>
        ///		<list type="bullet">
        ///			<item>
        ///				<term><see cref="JsonString"/></term>
        ///				<description>Parses the underlying string to <see cref="uint"/>. Returns 0 if value could not be parsed.</description>
        ///			</item>
        ///			<item>
        ///				<term><see cref="JsonNumber"/></term>
        ///				<description>Converts the underlying <see cref="double"/> value to <see cref="uint"/>.</description>
        ///			</item>
        ///			<item>
        ///				<term><see cref="JsonBoolean"/></term>
        ///				<description>Returns a value of one if the underlying value is true; otherwise zero.</description>
        ///			</item>
        ///			<item>Otherwise returns 0.</item>
        ///		</list>
        /// </remarks>
        /// <param name="value">The <see cref="JsonValue"/> to convert.</param>
        public static implicit operator uint(JsonValue value)
        {
            if (value == null)
                return 0;
            switch (value.ValueType)
            {
                case JsonValueTypes.String:
                    uint v = 0;
                    uint.TryParse((value as JsonString).Value, out v);
                    return v;
                case JsonValueTypes.Number:
                    return Convert.ToUInt32((value as JsonNumber).Value);
                case JsonValueTypes.Boolean:
                    return (uint)((value as JsonBoolean).Value ? 1 : 0);
                default:
                    return 0;
            }
        }

        /// <summary>
        /// An operator used to convert a value to <see cref="ulong"/>.
        /// </summary>
        /// <remarks>
        ///		<list type="bullet">
        ///			<item>
        ///				<term><see cref="JsonString"/></term>
        ///				<description>Parses the underlying string to <see cref="ulong"/>. Returns 0 if value could not be parsed.</description>
        ///			</item>
        ///			<item>
        ///				<term><see cref="JsonNumber"/></term>
        ///				<description>Converts the underlying <see cref="double"/> value to <see cref="ulong"/>.</description>
        ///			</item>
        ///			<item>
        ///				<term><see cref="JsonBoolean"/></term>
        ///				<description>Returns a value of one if the underlying value is true; otherwise zero.</description>
        ///			</item>
        ///			<item>Otherwise returns 0.</item>
        ///		</list>
        /// </remarks>
        /// <param name="value">The <see cref="JsonValue"/> to convert.</param>
        public static implicit operator ulong(JsonValue value)
        {
            if (value == null)
                return 0;
            switch (value.ValueType)
            {
                case JsonValueTypes.String:
                    ulong v = 0;
                    ulong.TryParse((value as JsonString).Value, out v);
                    return v;
                case JsonValueTypes.Number:
                    return Convert.ToUInt64((value as JsonNumber).Value);
                case JsonValueTypes.Boolean:
                    return (ushort)((value as JsonBoolean).Value ? 1 : 0);
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Returns the underlying byte array.
        /// </summary>
        /// <param name="value">A <see cref="JsonBinary"/>. If value is not a <see cref="JsonBinary"/>, null is returned.</param>
        public static implicit operator byte[] (JsonValue value)
        {
            if (value != null && value.ValueType == JsonValueTypes.Binary)
                return (value as JsonBinary).Value;
            else
                return null;
        }

        /// <summary>
        /// Parses a date string value from a <see cref="JsonString"/> to a <see cref="DateTime"/>.
        /// </summary>
        /// <param name="value">A <see cref="JsonString"/> with a date value. If value is not a <see cref="JsonString"/> or the value could not be parsed, <see cref="DateTime.UtcNow"/> is returned.</param>
        public static implicit operator DateTime(JsonValue value)
        {
            if (value != null && value.ValueType == JsonValueTypes.String)
            {
                DateTime time;
                DateTime.TryParse((string)value, System.Globalization.CultureInfo.InvariantCulture
                                  , System.Globalization.DateTimeStyles.RoundtripKind, out time);
                return time;

            }
            return DateTime.UtcNow;
        }

        /// <summary>
        /// Parses a date string value from a <see cref="JsonString"/> to a nullable <see cref="DateTime"/>.
        /// </summary>
        /// <param name="value">A <see cref="JsonString"/> with a date value. If value is not a <see cref="JsonString"/> or the value could not be parsed, null is returned.</param>
        public static implicit operator DateTime? (JsonValue value)
        {
            if (value != null && value.ValueType == JsonValueTypes.String)
            {
                DateTime time;
                if (DateTime.TryParse((string)value, System.Globalization.CultureInfo.InvariantCulture
                                  , System.Globalization.DateTimeStyles.RoundtripKind, out time))
                    return time;
            }
            return null;
        }

        /// <summary>
        /// Implicitly converts a string value to a relevant <see cref="JsonValue"/>.
        /// </summary>
        /// <remarks>
        ///		<list type="bullet">
        ///			<item>If the value is null, <see cref="JsonNull"/> is returned.</item>
        ///			<item>If the value starts with '[' and ends with ']', the value is parsed to <see cref="JsonArray"/>.</item>
        ///			<item>If the value starts with '{' and ends with '}', the value is parsed to <see cref="JsonObject"/>.</item>
        ///			<item>Otherwise <see cref="JsonString"/> is returned.</item>
        ///		</list>
        /// </remarks>
        /// <param name="value">A string value to convert.</param>
        public static implicit operator JsonValue(string value)
        {
            if (value == null)
                return new JsonNull();

            else if (value.StartsWith("[") && value.EndsWith("]"))
                return new JsonArray(value);
            else if (value.StartsWith("{") && value.EndsWith("}"))
                return new JsonObject(value);
            return new JsonString(value);
        }

#if !NETSTANDARD1_X
        /// <summary>
        /// Implicitly returns a <see cref="JsonNull"/> if the value is a <see cref="DBNull"/>.
        /// </summary>
        /// <param name="value">A <see cref="DBNull"/> value.</param>
        public static implicit operator JsonValue(DBNull value)
        {
            return new JsonNull();
        }
#endif

        /// <summary>
        /// Implicitly converts a <see cref="short"/> to <see cref="JsonNumber"/>.
        /// </summary>
        /// <param name="value">The value to convert to <see cref="JsonNumber"/>.</param>
        /// <example>
        /// <code>JsonValue v = 5;</code>
        /// </example>
        public static implicit operator JsonValue(short value)
        {
            return new JsonNumber(value);
        }

        /// <summary>
        /// Implicitly converts a <see cref="int"/> to <see cref="JsonNumber"/>.
        /// </summary>
        /// <param name="value">The value to convert to <see cref="JsonNumber"/>.</param>
        /// <example>
        /// <code>JsonValue v = 5;</code>
        /// </example>
        public static implicit operator JsonValue(int value)
        {
            return new JsonNumber(value);
        }

        /// <summary>
        /// Implicitly converts a <see cref="long"/> to <see cref="JsonNumber"/>.
        /// </summary>
        /// <param name="value">The value to convert to <see cref="JsonNumber"/>.</param>
        /// <example>
        /// <code>JsonValue v = 5;</code>
        /// </example>
        public static implicit operator JsonValue(long value)
        {
            return new JsonNumber(value);
        }

        /// <summary>
        /// Implicitly converts a <see cref="decimal"/> to <see cref="JsonNumber"/>.
        /// </summary>
        /// <param name="value">The value to convert to <see cref="JsonNumber"/>.</param>
        /// <example>
        /// <code>JsonValue v = 5;</code>
        /// </example>
        public static implicit operator JsonValue(decimal value)
        {
            return new JsonNumber(value);
        }

        /// <summary>
        /// Implicitly converts a <see cref="float"/> to <see cref="JsonNumber"/>.
        /// </summary>
        /// <param name="value">The value to convert to <see cref="JsonNumber"/>.</param>
        /// <example>
        /// <code>JsonValue v = 5;</code>
        /// </example>
        public static implicit operator JsonValue(float value)
        {
            return new JsonNumber(value);
        }

        /// <summary>
        /// Implicitly converts a <see cref="double"/> to <see cref="JsonNumber"/>.
        /// </summary>
        /// <param name="value">The value to convert to <see cref="JsonNumber"/>.</param>
        /// <example>
        /// <code>JsonValue v = 5;</code>
        /// </example>
        public static implicit operator JsonValue(double value)
        {
            return new JsonNumber(value);
        }

        /// <summary>
        /// Implicitly converts a <see cref="byte"/> to <see cref="JsonNumber"/>.
        /// </summary>
        /// <param name="value">The value to convert to <see cref="JsonNumber"/>.</param>
        /// <example>
        /// <code>JsonValue v = 5;</code>
        /// </example>
        public static implicit operator JsonValue(byte value)
        {
            return new JsonNumber(Convert.ToDouble(value));
        }

        /// <summary>
        /// Implicitly converts a <see cref="byte"/> to <see cref="JsonBoolean"/>.
        /// </summary>
        /// <param name="value">The value to convert to <see cref="JsonBoolean"/>.</param>
        /// <example>
        /// <code>JsonValue v = true;</code>
        /// </example>
        public static implicit operator JsonValue(bool value)
        {
            return new JsonBoolean(value);
        }

        /// <summary>
        /// Implicitly converts a <see cref="ushort"/> to <see cref="JsonNumber"/>.
        /// </summary>
        /// <param name="value">The value to convert to <see cref="JsonNumber"/>.</param>
        /// <example>
        /// <code>JsonValue v = 5;</code>
        /// </example>
        public static implicit operator JsonValue(ushort value)
        {
            return new JsonNumber(Convert.ToDouble(value));
        }

        /// <summary>
        /// Implicitly converts a <see cref="uint"/> to <see cref="JsonNumber"/>.
        /// </summary>
        /// <param name="value">The value to convert to <see cref="JsonNumber"/>.</param>
        /// <example>
        /// <code>JsonValue v = 5;</code>
        /// </example>
        public static implicit operator JsonValue(uint value)
        {
            return new JsonNumber(Convert.ToDouble(value));
        }

        /// <summary>
        /// Implicitly converts a <see cref="ulong"/> to <see cref="JsonNumber"/>.
        /// </summary>
        /// <param name="value">The value to convert to <see cref="JsonNumber"/>.</param>
        /// <example>
        /// <code>JsonValue v = 5;</code>
        /// </example>
        public static implicit operator JsonValue(ulong value)
        {
            return new JsonNumber(Convert.ToDouble(value));
        }

        /// <summary>
        /// Implicitly converts a <see cref="byte"/> array to <see cref="JsonBinary"/>.
        /// </summary>
        /// <param name="value">The value to convert to <see cref="JsonBinary"/>.</param>
        /// <example>
        /// <code>JsonValue v = 5;</code>
        /// </example>
        public static implicit operator JsonValue(byte[] value)
        {
            return new JsonBinary(value);
        }

        /// <summary>
        /// Implicitly converts a <see cref="DateTime"/> to a <see cref="JsonString"/> as a Round-trip date/time pattern.
        /// </summary>
        /// <param name="value">The value to convert to <see cref="JsonNumber"/>.</param>
        /// <example>
        /// <code>
        /// JsonValue v = DateTime.Now;
        /// </code>
        /// </example>
        public static implicit operator JsonValue(DateTime value)
        {
            return new JsonString(value.ToString("o"));
        }

        /// <summary>
        /// Tests if two values are equal.
        /// </summary>
        /// <param name="left">The value on the left side of the operator.</param>
        /// <param name="right">The value on the right side of the operator.</param>
        /// <returns>Returns true if the values are equal; otherwise false.</returns>
        public static bool operator ==(JsonValue left, JsonValue right)
        {
            bool l = JsonNull.IsNull(left), r = JsonNull.IsNull(right);
            if (l && r)
                return true;
            if ((l && !r) || (r && !l))
                return false;
            if (left.ValueType == right.ValueType)
            {
                switch (left.ValueType)
                {
                    case JsonValueTypes.String:
                        return (left as JsonString).Value == (right as JsonString).Value;
                    case JsonValueTypes.Object:
                    case JsonValueTypes.Array:
                    case JsonValueTypes.Binary:
                        return left.ToString() == right.ToString();
                    case JsonValueTypes.Number:
                        return (left as JsonNumber).Value == (right as JsonNumber).Value;
                    case JsonValueTypes.Boolean:
                        return (left as JsonBoolean).Value == (right as JsonBoolean).Value;
                    case JsonValueTypes.Null:
                        return true;
                    default:
                        break;
                }
            }
            return false;
        }

        /// <summary>
        /// Tests if two values are not equal.
        /// </summary>
        /// <param name="left">The value on the left side of the operator.</param>
        /// <param name="right">The value on the right side of the operator.</param>
        /// <returns>Returns true if the values are not equal; otherwise false.</returns>
        public static bool operator !=(JsonValue left, JsonValue right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Returns the hash code for the current object.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Determines if an object is equal the current object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>True if equal; otherwise false.</returns>
        public override bool Equals(object obj)
        {
            JsonValue value = obj as JsonValue;
            if (value == null) return false;

            return this == value;
        }

        /// <summary>
        /// Determine if a <see cref="JsonValue"/> is null.
        /// </summary>
        /// <param name="value">A value to determine.</param>
        /// <returns>Returns true if <paramref name="value"/> is null or is a <see cref="JsonNull"/></returns>
        public static bool IsNull(JsonValue value)
        {
            if ((object)value == null)
                return true;
            JsonNull n = value as JsonNull;
            if ((object)n != null)
                return true;
            return false;
        }

        /// <summary>
        /// Tries to parse a string of json and determine is <see cref="JsonValue"/> type.
        /// </summary>
        /// <param name="json">A string of json that should be parsed.</param>
        /// <param name="value">The output <see cref="JsonValue"/> created if successful. If not successful, a <see cref="JsonNull"/> se set to <paramref name="value"/>.</param>
        /// <returns>Returns true if <see cref="JsonValue"/> was successfully created; otherwise false is returned.</returns>
        public static bool TryParse(string json, out JsonValue value)
        {
            if (string.IsNullOrEmpty(json))
            {
                value = new JsonNull();
                return true;
            }
            if (json.StartsWith("{", StringComparison.CurrentCulture) && json.EndsWith("}", StringComparison.Ordinal))
            {
                value = new JsonObject(json);
                return true;
            }
            if (json.StartsWith("[") && json.EndsWith("]"))
            {
                value = new JsonArray(json);
                return true;
            }
            if (json.StartsWith("\"") && json.EndsWith("\""))
            {
                json = json.Substring(1, json.Length - 2);
                if (json.StartsWith("base64:"))
                    value = new JsonBinary(json);
                else
                    value = new JsonString(json);
                return true;
            }
            decimal v;
            bool bv;
            if (decimal.TryParse(json, out v))
            {
                value = new JsonNumber(v);
                return true;
            }
            if (bool.TryParse(json, out bv))
            {
                value = new JsonBoolean(bv);
                return true;
            }
            value = null;
            return false;
        }

        /// <summary>
        /// Creates an exact copy of the JsonValue.
        /// </summary>
        /// <returns></returns>
        public abstract JsonValue Clone();

        /// <summary>
        /// Gets the JsonValue's Parent.
        /// </summary>
        /// <returns>Parent <see cref="JsonValue"/></returns>
        public JsonValue GetParent()
        {
            return _parent;
        }

        /// <summary>
        /// Iterates up the Parent chain to retrieve the top most Parent.
        /// </summary>
        /// <returns><see cref="TG.JSON.JsonValue"/></returns>
        public JsonValue GetRoot()
        {
            JsonValue p = this.GetParent();
            while (p != null)
            {
                if (p._parent == null)
                    break;
                else
                    p = p.GetParent();
            }
            return p;
        }

#if !NETSTANDARD1_0
        /// <summary>
        /// Searches for an available <see cref="IEncryptionHandler"/>.
        /// </summary>
        /// <returns></returns>
        internal IEncryptionHandler GetAvailableEncryptionHandler()
        {
            if (GlobalEncryptionHandler != null)
            {
                return GlobalEncryptionHandler;
            }
            else
            {
                JsonValue p = this.GetParent();
                while (p != null)
                {
                    if (p.HasGlobalEncryptionHandler)
                    {
                        return p.GlobalEncryptionHandler;
                    }
                    else if (p._parent != null)
                    {
                        p = p.GetParent();
                        if (p.HasGlobalEncryptionHandler)
                        {
                            return p.GlobalEncryptionHandler;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

            }
            return null;
        }
#endif

        internal void SetParent(JsonValue parent)
        {
            _parent = parent;
        }

        /// <summary>
        /// Generates a JSON formatted array string. Ex. [ \"Hello\" , 1 ]
        /// </summary>
        /// <param name="format">Determines the format of the outputted JSON.</param>
        /// <returns>JSON formatted string.</returns>
        public virtual string ToString(Formatting format)
        {
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(ms))
                {
                    InternalWrite(writer, format, 0);
                    writer.Flush();
                    System.Text.UTF8Encoding encoding = new UTF8Encoding(false);
                    string value = encoding.GetString(ms.ToArray(), 0, (int)ms.Length);
                    
                    encoding = null;
                    return value;
                }

            }
        }
        

#if !NETSTANDARD1_0
        /// <summary>
        /// Generates a JSON formatted array string. Ex. [ \"Hello\" , 1 ]
        /// </summary>
        /// <returns>JSON formatted string.</returns>
        public override string ToString()
        {
            return this.ToString(Formatting.Compressed);
        }
#endif

        /// <summary>
        /// Returns the equivalent <see cref="JsonValue"/> from the specified object <paramref name="obj"/>.
        /// </summary>
        /// <remarks>
        /// If <paramref name="obj"/> cannot be matched with a <see cref="JsonNull"/>, <see cref="JsonBoolean"/>,
        /// <see cref="JsonNumber"/> or a <see cref="JsonString"/>; the <seealso cref="JsonObject.SerializeObject(object)"/> method is called.
        /// </remarks>
        /// <param name="obj">The object to be converted to a <see cref="JsonValue"/>.</param>
        /// <returns>A new <see cref="JsonValue"/> based on <paramref name="obj"/>.</returns>
        public JsonValue ValueFromObject(object obj)
        {
            return ValueFromObject(obj, int.MaxValue);
        }

        /// <summary>
        /// Returns the equivalent <see cref="JsonValue"/> from the specified object <paramref name="obj"/>.
        /// </summary>
        /// <remarks>
        /// If <paramref name="obj"/> cannot be matched with a <see cref="JsonNull"/>, <see cref="JsonBoolean"/>,
        /// <see cref="JsonNumber"/> or a <see cref="JsonString"/>; the <seealso cref="JsonObject.SerializeObject(object)"/> method is called.
        /// </remarks>
        /// <param name="obj">The object to be converted to a <see cref="JsonValue"/>.</param>
        /// <param name="maxDepth">The maximum depth to serialize if method <see cref="JsonObject.SerializeObject(object, int, string[])"/> needs to be called.</param>
        /// <param name="ignoreProperties">Property names that should be ignored if method <see cref="JsonObject.SerializeObject(object, int, string[])"/> needs to be called.</param>
        /// <returns>A new <see cref="JsonValue"/> based on <paramref name="obj"/>.</returns>
        public JsonValue ValueFromObject(object obj, int maxDepth, params string[] ignoreProperties)
        {
            return ValueFromObject(obj, maxDepth, false, false, ignoreProperties);
        }

        /// <summary>
        /// Returns the equivalent <see cref="JsonValue"/> from the specified object <paramref name="obj"/>.
        /// </summary>
        /// <remarks>
        /// If <paramref name="obj"/> cannot be matched with a <see cref="JsonNull"/>, <see cref="JsonBoolean"/>,
        /// <see cref="JsonNumber"/> or a <see cref="JsonString"/>; the <seealso cref="JsonObject.SerializeObject(object)"/> method is called.
        /// </remarks>
        /// <param name="obj">The object to be converted to a <see cref="JsonValue"/>.</param>
        /// <param name="maxDepth">The maximum depth to serialize if method <see cref="JsonObject.SerializeObject(object, int, string[])"/> needs to be called.</param>
        /// <param name="includeAttributes">If True, the AttributeTable will be populated with the object's attributes.</param>
        /// <param name="includeTypeInformation">If True, a _type property will be added with the full Type.AssemblyQualifiedName.</param>
        /// <param name="ignoreProperties">Property names that should be ignored if method <see cref="JsonObject.SerializeObject(object, int, string[])"/> needs to be called.</param>
        /// <returns>A new <see cref="JsonValue"/> based on <paramref name="obj"/>.</returns>
        public JsonValue ValueFromObject(object obj, int maxDepth, bool includeAttributes, bool includeTypeInformation, params string[] ignoreProperties)
        {
            return ValueFromObject(obj, new Serialization.JsonSerializationOptions(maxDepth, includeAttributes, includeTypeInformation, ignoreProperties, null));
        }

        /// <summary>
        /// Returns the equivalent <see cref="JsonValue"/> from the specified object <paramref name="obj"/>.
        /// </summary>
        /// <remarks>
        /// If <paramref name="obj"/> cannot be matched with a <see cref="JsonNull"/>, <see cref="JsonBoolean"/>,
        /// <see cref="JsonNumber"/> or a <see cref="JsonString"/>; the <seealso cref="JsonObject.SerializeObject(object)"/> method is called.
        /// </remarks>
        /// <param name="obj">The object to be converted to a <see cref="JsonValue"/>.</param>
        /// <param name="serializationOptions">The options to use when serializing.</param>
        /// <returns>A new <see cref="JsonValue"/> based on <paramref name="obj"/>.</returns>
        public JsonValue ValueFromObject(object obj, Serialization.JsonSerializationOptions serializationOptions)
        {
            if (serializationOptions == null)
            {
                serializationOptions = new Serialization.JsonSerializationOptions();
            }

            if (obj is string)
                return (string)obj;
            if (obj is bool)
                return new JsonBoolean((bool)obj);
#if !NETSTANDARD1_X
            if (obj == null || obj is DBNull)
                return new JsonNull();
#endif
            if (obj is DateTime)
                return (DateTime)obj;
            if (obj is JsonValue)
                return ((JsonValue)obj).Clone();
            if (obj is byte[])
                return (byte[])obj;
            if (obj is System.Collections.IEnumerable && serializationOptions.CurrentDepth > 0)
            {
                serializationOptions.CurrentDepth--;
                //return new JsonArray().SerializeObject(obj, maxDepth - 1, includeAttributes, includeTypeInformation, ignoreProperties);
                JsonArray array = new JsonArray();
                array.SerializeObject(obj, serializationOptions);
                serializationOptions.CurrentDepth++;
                return array;
            }

            if (obj is short)
                return (short)obj;
            if (obj is int)
                return (int)obj;
            if (obj is long)
                return (long)obj;
            if (obj is ushort)
                return (ushort)obj;
            if (obj is uint)
                return (uint)obj;
            if (obj is ulong)
                return (ulong)obj;
            if (obj is decimal)
                return (decimal)obj;
            if (obj is float)
                return (float)obj;
            if (obj is double)
                return (double)obj;
            if (obj is byte)
                return (byte)obj;
            if (obj is Enum)
                return obj.ToString();
#if !NETSTANDARD1_X
            if (obj is System.Drawing.Color)
            {
                System.Drawing.Color color = (System.Drawing.Color)obj;
                if (color.IsNamedColor)
                {
                    return new JsonObject("color", color.Name);
                }
                else
                {
                    return new JsonObject("color", $"#{color.ToArgb().ToString("X8")}");
                }
            }
#endif
            if (obj != null && serializationOptions.CurrentDepth > 0)
            {
                serializationOptions.CurrentDepth--;
                JsonObject jo = new JsonObject();
                jo.SerializeObject(obj, serializationOptions);
                serializationOptions.CurrentDepth++;
                return jo;
            }

            return new JsonNull();
        }

        /// <summary>
        /// Returns the equivalent <see cref="JsonValue"/> from the specified object <paramref name="obj"/>.
        /// </summary>
        /// <remarks>
        /// If <paramref name="obj"/> cannot be matched with a <see cref="JsonNull"/>, <see cref="JsonBoolean"/>,
        /// <see cref="JsonNumber"/> or a <see cref="JsonString"/>; the <seealso cref="JsonObject.SerializeObject(object)"/> method is called.
        /// </remarks>
        /// <param name="obj">The object to be converted to a <see cref="JsonValue"/>.</param>
        /// <returns>A new <see cref="JsonValue"/> based on <paramref name="obj"/>.</returns>
        public static JsonValue GetValueFromObject(object obj)
        {
            return GetValueFromObject(obj, int.MaxValue);
        }

        /// <summary>
        /// Returns the equivalent <see cref="JsonValue"/> from the specified object <paramref name="obj"/>.
        /// </summary>
        /// <remarks>
        /// If <paramref name="obj"/> cannot be matched with a <see cref="JsonNull"/>, <see cref="JsonBoolean"/>,
        /// <see cref="JsonNumber"/> or a <see cref="JsonString"/>; the <seealso cref="JsonObject.SerializeObject(object)"/> method is called.
        /// </remarks>
        /// <param name="obj">The object to be converted to a <see cref="JsonValue"/>.</param>
        /// <param name="maxDepth">The maximum depth to serialize if method <see cref="JsonObject.SerializeObject(object, int, string[])"/> needs to be called.</param>
        /// <param name="ignoreProperties">Property names that should be ignored if method <see cref="JsonObject.SerializeObject(object, int, string[])"/> needs to be called.</param>
        /// <returns>A new <see cref="JsonValue"/> based on <paramref name="obj"/>.</returns>
        public static JsonValue GetValueFromObject(object obj, int maxDepth, params string[] ignoreProperties)
        {
            if (generic == null)
                generic = new JsonNull();
            return generic.ValueFromObject(obj, maxDepth, ignoreProperties);
        }
        

        internal abstract void InternalWrite(System.IO.StreamWriter writer, Formatting format, int depth);

        
        /// <summary>
        /// Executes the <see cref="JsonValue.ValueChanged"/> event.
        /// </summary>
        protected virtual void OnValueChanged()
        {
            if (ValueChanged != null && !SuspendEvents)
                ValueChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Generates an indent string using the root IndentString property multiplied by the indents parameter.
        /// </summary>
        /// <remarks>
        /// This method retrieves the parent value's <see cref="JsonValue.IndentString"/> and multiplies that by the <paramref name="indents"/> argument.
        /// </remarks>
        /// <param name="indents">Number of indents to generate.</param>
        /// <returns>Indent String</returns>
        protected virtual string GenerateIndents(int indents)
        {
            string s = "";
            JsonValue root = GetRoot() ?? this;
            string indent = root.IndentString;
            for (int i = 0; i < indents; i++)
                s += indent;
            return s;
        }

        #endregion Methods

#if !NETSTANDARD1_0
        #region IConvertable Implementation

        /// <summary>
        /// Implementation of IConvertable. See <see cref="IConvertible"/>.
        /// </summary>
        /// <returns><see cref="TypeCode"/></returns>
        TypeCode IConvertible.GetTypeCode()
        {

            switch (ValueType)
            {
                case JsonValueTypes.String:
                    return TypeCode.String;
                case JsonValueTypes.Object:
                    return TypeCode.Object;
                case JsonValueTypes.Array:
                    return TypeCode.Object;
                case JsonValueTypes.Number:
                    return TypeCode.Decimal;
                case JsonValueTypes.Boolean:
                    return TypeCode.Boolean;
                case JsonValueTypes.Binary:
                    return TypeCode.Byte;
                case JsonValueTypes.Null:
                    return TypeCode.Empty;
                default:
                    return TypeCode.Empty;
            }
        }

        /// <summary>
        /// Implementation of IConvertable. See <see cref="IConvertible"/>.
        /// </summary>
        /// <returns><see cref="bool"/></returns>
        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            return (bool)this;
        }

        /// <summary>
        /// Implementation of IConvertable. See <see cref="IConvertible"/>.
        /// </summary>
        /// <returns><see cref="char"/></returns>
        char IConvertible.ToChar(IFormatProvider provider)
        {
            return char.MinValue;
        }

        /// <summary>
        /// Implementation of IConvertable. See <see cref="IConvertible"/>.
        /// </summary>
        /// <returns><see cref="sbyte"/></returns>
        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            return sbyte.MinValue;
        }

        /// <summary>
        /// Implementation of IConvertable. See <see cref="IConvertible"/>.
        /// </summary>
        /// <returns><see cref="byte"/></returns>
        byte IConvertible.ToByte(IFormatProvider provider)
        {
            return (byte)this;
        }

        /// <summary>
        /// Implementation of IConvertable. See <see cref="IConvertible"/>.
        /// </summary>
        /// <returns><see cref="System.Int16"/></returns>
        short IConvertible.ToInt16(IFormatProvider provider)
        {
            return (short)this;
        }

        /// <summary>
        /// Implementation of IConvertable. See <see cref="IConvertible"/>.
        /// </summary>
        /// <returns><see cref="System.UInt16"/></returns>
        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            return (ushort)this;
        }

        /// <summary>
        /// Implementation of IConvertable. See <see cref="IConvertible"/>.
        /// </summary>
        /// <returns><see cref="System.Int32"/></returns>
        int IConvertible.ToInt32(IFormatProvider provider)
        {
            return (int)this;
        }

        /// <summary>
        /// Implementation of IConvertable. See <see cref="IConvertible"/>.
        /// </summary>
        /// <returns><see cref="System.UInt32"/></returns>
        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            return (uint)this;
        }

        /// <summary>
        /// Implementation of IConvertable. See <see cref="IConvertible"/>.
        /// </summary>
        /// <returns><see cref="System.Int64"/></returns>
        long IConvertible.ToInt64(IFormatProvider provider)
        {
            return (long)this;
        }

        /// <summary>
        /// Implementation of IConvertable. See <see cref="IConvertible"/>.
        /// </summary>
        /// <returns><see cref="System.UInt64"/></returns>
        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            return (ulong)this;
        }

        /// <summary>
        /// Implementation of IConvertable. See <see cref="IConvertible"/>.
        /// </summary>
        /// <returns><see cref="System.Single"/></returns>
        float IConvertible.ToSingle(IFormatProvider provider)
        {
            return (float)this;
        }

        /// <summary>
        /// Implementation of IConvertable. See <see cref="IConvertible"/>.
        /// </summary>
        /// <returns><see cref="System.Double"/></returns>
        double IConvertible.ToDouble(IFormatProvider provider)
        {
            return (double)this;
        }

        /// <summary>
        /// Implementation of IConvertable. See <see cref="IConvertible"/>.
        /// </summary>
        /// <returns><see cref="System.Decimal"/></returns>
        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            return (decimal)this;
        }

        /// <summary>
        /// Implementation of IConvertable. See <see cref="IConvertible"/>.
        /// </summary>
        /// <returns><see cref="System.DateTime"/></returns>
        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            return (DateTime)this;
        }

        /// <summary>
        /// Implementation of IConvertable. See <see cref="IConvertible"/>.
        /// </summary>
        /// <returns><see cref="System.String"/></returns>
        string IConvertible.ToString(IFormatProvider provider)
        {
            return (string)this;
        }

        /// <summary>
        /// Implementation of IConvertable. See <see cref="IConvertible"/>.
        /// </summary>
        /// <returns><see cref="System.Object"/></returns>
        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            var ntype = Nullable.GetUnderlyingType(conversionType);
            if (ntype != null && this.ValueType != JsonValueTypes.Null)
            {
                return Convert.ChangeType(this, ntype);
            }
#if NETSTANDARD1_3
            else if (conversionType.GetTypeInfo().IsEnum)
#else
            else if (conversionType.IsEnum)
#endif
            {
                return Enum.Parse(conversionType, (string)this);
            }
            return null;
        }

        #endregion IConvertable Implementation
#endif

    }
}