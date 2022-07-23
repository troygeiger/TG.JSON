#if FULLNET || NETSTANDARD2_0

namespace TG.JSON
{
    /// <summary>
    /// Allows a <see cref="JsonArray"/> to be bound to controls, such as a <see cref="System.Windows.Forms.DataGridView"/> or <see cref="System.Windows.Forms.BindingNavigator"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// var jarray = new JsonArray(
    ///     new JsonObject("hello", "world"),
    ///     new JsonObject("hello", "23212")
    ///     );
    ///
    /// var binder = new JsonArrayBindingSource(jarray, typeof(JsonObject));
    /// BindingSource bs = new BindingSource();
    /// bs.DataSource = binder;
    /// bindingNavigator1.BindingSource = bs;
    /// textBox1.DataBindings.Add("Text", bs, "hello", true, DataSourceUpdateMode.OnPropertyChanged);
    /// </code>
    /// </example>
    public class JsonArrayBindingSource : JsonArrayBindingSource<JsonValue>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="JsonArrayBindingSource"/> with a source <see cref="JsonArray"/>, the type of <see cref="JsonValue"/> that is contained in the array and the properties that should be used or available.
        /// </summary>
        /// <param name="sourceArray">The source array containing values.</param>
        /// <param name="prototype">The <see cref="JsonValue"/> used as a prototype for new values.</param>
        public JsonArrayBindingSource(JsonArray sourceArray, JsonValue prototype)
            : base(sourceArray, prototype)
        { }


        #endregion Constructors
    }
}
#endif