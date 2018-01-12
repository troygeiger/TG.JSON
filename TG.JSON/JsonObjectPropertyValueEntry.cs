namespace TG.JSON
{
    public sealed partial class JsonObject
    {
        /// <summary>
        /// A Property/Value structure returned by the <see cref="JsonObjectPropertyEnumerator"/>.
        /// </summary>
        public struct JsonObjectPropertyValueEntry
        {
            /// <summary>
            /// The name of the property.
            /// </summary>
            public string PropertyName;

            /// <summary>
            /// The value of the property.
            /// </summary>
            public JsonValue Value;

            /// <summary>
            /// A constructor used to set the parameters.
            /// </summary>
            /// <param name="property"></param>
            /// <param name="value"></param>
            public JsonObjectPropertyValueEntry(string property, JsonValue value)
            {
                PropertyName = property;
                Value = value;
            }
        }

    }
}