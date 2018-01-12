namespace TG.JSON
{
    using System.Collections;

    public sealed partial class JsonObject
    {
        /// <summary>
        /// The Enumerator used to step through the properties of a <see cref="JsonObject"/>.
        /// </summary>
        public class JsonObjectPropertyEnumerator : IEnumerator
        {
            JsonObject thisObj = null;
            string[] props = null;
            int propIdx = -1;

            /// <summary>
            /// Initializes a new instance of <see cref="JsonObjectPropertyEnumerator"/>.
            /// </summary>
            /// <param name="obj"></param>
            public JsonObjectPropertyEnumerator(JsonObject obj)
            {
                thisObj = obj;
                props = obj.PropertyNames;
            }

            /// <summary>
            /// The current value.
            /// </summary>
            public object Current
            {
                get
                {
                    if (propIdx > -1 && propIdx < props.Length)
                        return new JsonObjectPropertyValueEntry(props[propIdx], thisObj[props[propIdx]]);
                    else
                        return null;
                }
            }

            /// <summary>
            /// Moves to the next property.
            /// </summary>
            /// <returns>Returns true if the next property index is within range; otherwise false.</returns>
            public bool MoveNext()
            {

                if (propIdx + 1 < props.Length)
                {
                    propIdx++;
                    return true;
                }
                else
                    return false;
            }

            /// <summary>
            /// Resets the property index to 0;
            /// </summary>
            public void Reset()
            {
                propIdx = -1;
            }
        }

    }
}