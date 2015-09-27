namespace TG.JSON
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

	/// <summary>
	/// The JsonReader is used to step through a JSON string when parsing.
	/// </summary>
	[System.Diagnostics.DebuggerStepThrough]
    public class JsonReader
    {
        #region Fields

        string jstring;
        int _position = 0;

        #endregion Fields

        #region Constructors

		/// <summary>
		/// Initializes a new instance of <see cref="JsonReader"/> with the provided JSON string.
		/// </summary>
		/// <param name="json">The JSON string that will be read from.</param>
        public JsonReader(string json)
        {
            jstring = json;
        }

        #endregion Constructors

        #region Properties

		/// <summary>
		/// Get whether the <see cref="JsonReader"/> is at the end of the JSON string.
		/// </summary>
        public bool EndOfJson
        {
            get { return Length == 0 ? true : _position == Length; }
        }

		/// <summary>
		/// Gets the length of the JSON string.
		/// </summary>
        public int Length
        {
            get { return jstring == null ? 0 : jstring.Length; }
        }

		/// <summary>
		/// Gets or Sets the current position the <see cref="JsonReader"/> is reading from within the JSON string.
		/// </summary>
        public int Position
        {
            get
            {
                return _position;
            }
            set
            {
                if (jstring != null)
                {
                    if (value >= 0 && value < Length)
                        _position = value;
                }
            }
        }

		/// <summary>
		/// Gets or Sets the JSON string value.
		/// </summary>
        public string StringValue
        {
            get { return jstring; }
            set
            {
                Reset();
                jstring = value;
            }
        }

        #endregion Properties

        #region Methods

		/// <summary>
		/// The <see cref="char"/> at the current position within the JSON string.
		/// </summary>
		/// <returns></returns>
        public char Read()
        {
            if (jstring != null && !EndOfJson)
            {
                char c = jstring[_position];
                _position++;
                return c;

            }
            return '\0';
        }

		/// <summary>
		/// Sets the <see cref="StringValue"/> as null and the <see cref="Position"/> to zero.
		/// </summary>
        public void Reset()
        {
            jstring = null;
            _position = 0;
        }

        #endregion Methods
    }
}