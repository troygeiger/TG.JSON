namespace TG.JSON
{
    using System;
    using System.Collections.Generic;
    using System.Text;

	/// <summary>
	/// The StringBuffer class is used to store an array of <see cref="char"/> when parsing a JSON string.
	/// </summary>
    [System.Diagnostics.DebuggerStepThrough]
    internal class StringBuffer
    {
        #region Fields

        char[] buffer;
        int initSize = 0;
        int position = 0;

        #endregion Fields

        #region Constructors

        public StringBuffer()
            : this(0)
        {
        }

        public StringBuffer(int initialSize)
        {
            buffer = new char[initialSize];
            initSize = initialSize;
        }

        #endregion Constructors

        #region Properties

        public int Length
        {
            get; private set;
        }

        #endregion Properties

        #region Methods

        public void Add(char chr)
        {
            EnsureSize(1);
            buffer[position] = chr;
            position++;
            Length++;
        }

        public string Dump()
        {
            string s = this.ToString();
            this.Reset();
            return s;
        }

        public void Reset()
        {
            buffer = new char[initSize];
            Length = 0;
            position = 0;
        }

        public override string ToString()
        {
            if (this.Length > 0)
                return new string(buffer, 0, this.Length);
            else
                return string.Empty;
        }

        private void EnsureSize(int plus)
        {
            if (position + plus >= buffer.Length)
            {
                int num = buffer.Length == 0 ? 16 : buffer.Length * 2;
                char[] copy = new char[num];
                Array.Copy(buffer, copy, buffer.Length);
                buffer = copy;
            }
        }

        #endregion Methods
    }
}