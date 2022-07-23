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
    public class JsonReader : IDisposable
    {

        #region Fields
        internal delegate char ReadFunctionDelegate();
        internal delegate bool EOFDelegate();
        internal delegate long LengthDelegate();
        string jstring;
        int _position = 0;
        bool _isNull;
        StreamReader reader = null;
        ReadFunctionDelegate readFunc;
        EOFDelegate eofFunc;
        LengthDelegate lengthFunc;

        #endregion Fields

        #region Constructors

		/// <summary>
		/// Initializes a new instance of <see cref="JsonReader"/> with the provided JSON string.
		/// </summary>
		/// <param name="json">The JSON string that will be read from.</param>
        public JsonReader(string json)
        {
            StringValue = json;
            readFunc = new ReadFunctionDelegate(ReadString);
            eofFunc = new EOFDelegate(EOFString);
            lengthFunc = new LengthDelegate(LengthString);
        }

        /// <summary>
		/// Initializes a new instance of <see cref="JsonReader"/> with the provided <see cref="Stream"/>.
		/// </summary>
		/// <param name="stream">The <see cref="Stream"/> that will be read from.</param>
        public JsonReader(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            reader = new StreamReader(stream);
            readFunc = new ReadFunctionDelegate(ReadStream);
            eofFunc = new EOFDelegate(EOFStream);
            lengthFunc = new LengthDelegate(LengthStream);
        }

        /// <summary>
		/// Initializes a new instance of <see cref="JsonReader"/> with the provided <see cref="StreamReader"/>.
		/// </summary>
		/// <param name="stream">The <see cref="StreamReader"/> that will be read from.</param>
        public JsonReader (StreamReader stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            reader = stream;
            readFunc = new ReadFunctionDelegate(ReadStream);
            eofFunc = new EOFDelegate(EOFStream);
            lengthFunc = new LengthDelegate(LengthStream);
        }

        #endregion Constructors

        #region Properties

		/// <summary>
		/// Get whether the <see cref="JsonReader"/> is at the end of the JSON string.
		/// </summary>
        public bool EndOfJson
        {
            get { return eofFunc.Invoke(); }
        }

        private bool EOFString()
        {
            return Length == 0 ? true : _position == Length; 
        }

        private bool EOFStream()
        {
            return reader.EndOfStream;
        }

		/// <summary>
		/// Gets the length of the JSON string.
		/// </summary>
        public long Length
        {
            get { return lengthFunc.Invoke(); }
        }

        private long LengthString()
        {
            return jstring == null ? 0 : jstring.Length;
        }

        private long LengthStream()
        {
            return reader.BaseStream.Length;
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
        }
  //        set
  //          {
  //              if (jstring != null)
  //              {
  //                  if (value >= 0 && value < Length)
  //                      _position = value;
  //              }
  //              else if (reader != null && value >= 0 && value < Length)
  //              {
  //                  _position = value;
  //                  reader.BaseStream.Position = value;
  //              }
  //          }
  //      }

            /// <summary>
            /// Gets or Sets the JSON string value.
            /// </summary>
        public string StringValue
        {
            get
            { 
                if (reader == null)
                {
                    return jstring;
                }
                else
                {
                    long pos = reader.BaseStream.Position;
                    reader.BaseStream.Position = 0;
                    string value = reader.ReadToEnd();
                    reader.BaseStream.Position = pos;
                    return value;
                }
            }
            set
            {
                Reset();
                _isNull = string.IsNullOrEmpty(value);
                if (reader == null)
                {
                    jstring = value;
                }
                else
                {
                    MemoryStream ms = new MemoryStream();
#if !NET20 && !NET35 && !NET40
                    using (StreamWriter writer = new StreamWriter(ms, Encoding.UTF8, 1024, true))
                    {
                        writer.Write(value);
                    }
#else
                    StreamWriter writer = new StreamWriter(ms, Encoding.UTF8);
                    writer.Write(value);
#endif
                    reader.Dispose();
                    reader = new StreamReader(ms);
                }
            }
        }

        /// <summary>
        /// Gets whether the StringValue is null.
        /// </summary>
        public bool IsNull
        {
            get { return _isNull; }
        }

        /// <summary>
        /// Disposes the Underlying stream is present.
        /// </summary>
        public void Dispose()
        {
            if (reader != null)
            {
                reader.Dispose();
            }
        }

#endregion Properties

#region Methods

        private char ReadString()
        {
            if (jstring != null && !EndOfJson)
            {
                char c = jstring[_position];
                _position++;
                return c;

            }
            return '\0';
        }

        private char ReadStream()
        {
            _position++;
            return (char)reader.Read();
        }

        /// <summary>
        /// The <see cref="char"/> at the current position within the JSON string.
        /// </summary>
        /// <returns></returns>
        public char Read()
        {
            return readFunc.Invoke();
        }

		/// <summary>
		/// Sets the <see cref="StringValue"/> as null and the <see cref="Position"/> to zero.
		/// </summary>
        public void Reset()
        {
            _position = 0;
            if (reader != null)
            {
                reader.BaseStream.Position = 0;
            }
        }

#endregion Methods
    }
}