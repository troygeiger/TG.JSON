namespace TG.JSON
{
    using System.Collections;

    public sealed partial class JsonObject
    {
        /// <summary>
        /// Determines the type of <see cref="IEnumerator"/> to use when GetEnumerator is called.
        /// </summary>
        public enum JsonObjectEnumeratorTypes
        {
            /// <summary>
            /// Indicates that <see cref="JsonObjectBindableEnumerator"/> should be used. 
            /// </summary>
            Bindable,
            /// <summary>
            /// Indicates that the <see cref="IEnumerator"/> should return a 
            /// </summary>
            Property
        }

    }
}