namespace TG.JSON
{
	using System;
	using System.Collections.Generic;
	using System.Text;

#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough()]
#endif
	/// <summary>
	/// Represents a null json value.
	/// </summary>
	//[System.ComponentModel.TypeConverter(typeof(System.ComponentModel.NullableConverter))]
	public class JsonNull : JsonValue
	{
		#region Properties

        /// <summary>
        /// Returns <see cref="JsonValueTypes.Null"/>
        /// </summary>
		public override JsonValueTypes ValueType
		{
			get { return JsonValueTypes.Null; }
		}
        
		#endregion Properties

		#region Methods

        /// <summary>
        /// This always returns null.
        /// </summary>
        /// <param name="value"></param>
		public static explicit operator string(JsonNull value)
		{
			return null;
		}

        /// <summary>
        /// Creates a new instance of <see cref="JsonNull"/>.
        /// </summary>
		public override JsonValue Clone()
		{
			return new JsonNull();
		}

        /// <summary>
        /// Returns string "null".
        /// </summary>
        /// <returns>"null"</returns>
#if NETSTANDARD1_0
        public string ToString()
#else
        public override string ToString() 
#endif
        {
			return "null";
		}

		internal override string InternalToString(Formatting format, int depth)
		{
			return this.ToString();
		}

		#endregion Methods
	}
}