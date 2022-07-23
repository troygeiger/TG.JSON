using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TG.JSON
{

#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough()]
#endif
    /// <summary>
    /// This class represents a byte array. When calling the ToString method, the array is converted to a base64 string beginning with "base64:"./>
    /// </summary>
    /// <remarks>
    /// Although JSON does not officially specify a binary type, this class was created for convenience of storing binary data.
    /// </remarks>
	public sealed class JsonBinary : JsonValue
	{
		#region Constructors

        /// <summary>
        /// Initializes a new empty instance of <see cref="JsonBinary"/>.
        /// </summary>
        public JsonBinary() { }

        /// <summary>
        /// Initializes a new instance of <see cref="JsonBinary"/> and converts the specified base64 string to a byte array.
        /// </summary>
        /// <param name="value">A base64 string starting with "base64:"</param>
		public JsonBinary(string value)
		{
			if (value != null && (value.StartsWith("\"base64:") || value.StartsWith("base64:")))
			{
				value = value.Trim('"');
				this.Value = Convert.FromBase64String(value.Substring(7));
			}
		}

        /// <summary>
        /// Initialize a new instance of <see cref="JsonBinary"/> and populating it with the specified byte array.
        /// </summary>
        /// <param name="value">An array of bytes to populate to the new <see cref="JsonBinary"/>.</param>
		public JsonBinary(byte[] value)
		{
			this.Value = value;
		}

		#endregion Constructors

		#region Properties

        /// <summary>
        /// Get or set the byte array value.
        /// </summary>
		public byte[] Value
		{
			get;
			set;
		}

        /// <summary>
        /// Returns <see cref="JsonValueTypes.Binary"/>
        /// </summary>
		public override JsonValueTypes ValueType
		{
			get { return JsonValueTypes.Binary; }
		}

        #endregion Properties

        #region Methods

        /// <summary>
        /// Creates a new instance of <see cref="JsonBinary"/> with an exact copy of it's value.
        /// </summary>
        public override JsonValue Clone()
		{
			return new JsonBinary(this.Value);
		}

        internal override void InternalWrite(StreamWriter writer, Formatting format, int depth)
        {
            if (Value != null && Value.Length > 0)
                writer.Write(string.Concat("\"base64:",
                    Convert.ToBase64String(this.Value),
                "\""));
            else
                writer.Write("\"base64:\"");
        }

   
		#endregion Methods
	}
}