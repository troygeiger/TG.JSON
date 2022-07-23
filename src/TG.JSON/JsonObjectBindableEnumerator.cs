namespace TG.JSON
{
    using System.Collections;

    public sealed partial class JsonObject
    {
        /// <summary>
        /// A Enumerator used when binding to a <see cref="System.Windows.Forms.Control"/>, using the property <see cref="System.Windows.Forms.Control.DataBindings"/>.
        /// </summary>
        public class JsonObjectBindableEnumerator : IEnumerator
        {
            JsonObject thisObj = null;
            bool stepped = false;

            /// <summary>
            /// Initializes a new instance of <see cref="JsonObjectBindableEnumerator"/>.
            /// </summary>
            /// <param name="obj"></param>
            public JsonObjectBindableEnumerator(JsonObject obj)
            {
                thisObj = obj;
            }

            /// <summary>
            /// The <see cref="JsonObject"/> set during construction.
            /// </summary>
            public object Current
            {
                get
                {

                    return thisObj;

                }
            }

            /// <summary>
            /// This does nothing.
            /// </summary>
            /// <returns>Always True</returns>
            public bool MoveNext()
            {
                if (stepped)
                    return false;
                stepped = true;
                return true;
            }

            /// <summary>
            /// This does nothing.
            /// </summary>
            public void Reset()
            {
                stepped = false;
            }
        }

        
    }
}