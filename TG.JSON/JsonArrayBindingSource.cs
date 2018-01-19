namespace TG.JSON
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text;

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
    public class JsonArrayBindingSource : IBindingList, ICancelAddNew, ITypedList
    {
        #region Fields

        int newPos = -1;
        PropertyDescriptorCollection props = null;
        //Type vtype;
        JsonArray _array;
        JsonValue _prototype = null;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="JsonArrayBindingSource"/> with a source <see cref="JsonArray"/> and the type of <see cref="JsonValue"/> that is contained in the array.
        /// </summary>
        /// <param name="sourceArray">The source array containing values.</param>
        /// <param name="valueType">The type of <see cref="JsonValue"/> the <see cref="JsonArray"/> contains.</param>
        /// <remarks>
        /// If the source <see cref="JsonArray"/> is empty, use the constructor with the constructor <see cref="JsonArrayBindingSource.JsonArrayBindingSource(JsonArray, Type, IEnumerable{JsonObjectPropertyDescriptor})"/> to define the properties that should be available.
        /// </remarks>
        [Obsolete("Depreciated! Use JsonArrayBindingSource(JsonArray sourceArray, JsonValue prototype).")]
        public JsonArrayBindingSource(JsonArray sourceArray, Type valueType)
        {
            throw new NotImplementedException("This constructor is obsolete. Use JsonArrayBindingSource(JsonArray sourceArray, JsonValue prototype).");
            //_array = sourceArray;
            //vtype = valueType;

            //if (sourceArray.Count > 0)
            //    ReadProperties();
            //else
            //{

            //    if (valueType == typeof(JsonString) || valueType == typeof(JsonNumber) || valueType == typeof(JsonBoolean))
            //    {
            //        PropertyDescriptorCollection c = TypeDescriptor.GetProperties(valueType);
            //        PropertyDescriptor[] ca = new PropertyDescriptor[1];
            //        foreach (PropertyDescriptor item in c)
            //        {
            //            if (item.Name == "Value")
            //            {
            //                ca[0] = item;
            //                props = new PropertyDescriptorCollection(ca);
            //                break;
            //            }
            //        }
            //    }
            //    else
            //        props = TypeDescriptor.GetProperties(valueType);
            //}
        }

        /// <summary>
        /// Initializes a new instance of <see cref="JsonArrayBindingSource"/> with a source <see cref="JsonArray"/>, the type of <see cref="JsonValue"/> that is contained in the array and the properties that should be used or available.
        /// </summary>
        /// <param name="sourceArray">The source array containing values.</param>
        /// <param name="valueType">The type of <see cref="JsonValue"/> the <see cref="JsonArray"/> contains.</param>
        /// <param name="properties">A list of properties that should be used or available.</param>
        [Obsolete("Depreciated! Use JsonArrayBindingSource(JsonArray sourceArray, JsonValue prototype).")]
        public JsonArrayBindingSource(JsonArray sourceArray, Type valueType, IEnumerable<JsonObjectPropertyDescriptor> properties)
        {
            throw new NotImplementedException("This constructor is obsolete. Use JsonArrayBindingSource(JsonArray sourceArray, JsonValue prototype).");
            //List<JsonObjectPropertyDescriptor> p = new List<JsonObjectPropertyDescriptor>();
            //if (properties != null)
            //{
            //    foreach (JsonObjectPropertyDescriptor item in properties)
            //    {
            //        item.CanSetNull = false;
            //        p.Add(item);
            //    }
            //}
            //props = new PropertyDescriptorCollection(p.ToArray());
            //_array = sourceArray;
            //vtype = valueType;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="JsonArrayBindingSource"/> with a source <see cref="JsonArray"/>, the type of <see cref="JsonValue"/> that is contained in the array and the properties that should be used or available.
        /// </summary>
        /// <param name="sourceArray">The source array containing values.</param>
        /// <param name="prototype">The <see cref="JsonValue"/> used as a prototype for new values.</param>
        public JsonArrayBindingSource(JsonArray sourceArray, JsonValue prototype)
        {

            _array = sourceArray ?? throw new ArgumentNullException("sourceArray");
            _prototype = prototype ?? throw new ArgumentNullException("prototype");

            switch (prototype.ValueType)
            {
                case JsonValueTypes.Object:
                    props = TypeDescriptor.GetProperties(prototype);
                    break;
                case JsonValueTypes.Array:
                    throw new NotImplementedException("JsonArray types have not been implemented.");
                case JsonValueTypes.String:
                    props = new PropertyDescriptorCollection(new PropertyDescriptor[]
                    { new JsonObjectPropertyDescriptor("Value", typeof(JsonString), "Values", null, false, true) });
                    break;
                case JsonValueTypes.Number:
                    props = new PropertyDescriptorCollection(new PropertyDescriptor[]
                    { new JsonObjectPropertyDescriptor("Value", typeof(double), "Values", null, false, true) });
                    break;
                case JsonValueTypes.Boolean:
                    props = new PropertyDescriptorCollection(new PropertyDescriptor[]
                    { new JsonObjectPropertyDescriptor("Value",typeof(bool), "Values", null, false, true) });
                    break;
                case JsonValueTypes.Binary:
                    props = new PropertyDescriptorCollection(new PropertyDescriptor[]
                    { new JsonObjectPropertyDescriptor("Value", typeof(JsonBinary), "Values", null, false, true) });
                    break;
                case JsonValueTypes.Null:
                    props = new PropertyDescriptorCollection(new PropertyDescriptor[]
                    { new JsonObjectPropertyDescriptor("Value", typeof(JsonNull), "Values", null, false, true) });
                    break;
                default:
                    break;
            }
        }


        #endregion Constructors

        #region Events

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public event ListChangedEventHandler ListChanged;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets whether the binding source is allowed to edit. Always returns true.
        /// </summary>
        public bool AllowEdit
        {
            get { return true; }
        }

        /// <summary>
        /// Gets whether the binding source is allowed to add new items. Always returns true.
        /// </summary>
        public bool AllowNew
        {
            get { return true; }
        }

        /// <summary>
        /// Gets whether the binding source is allowed to remove items. Always returns true.
        /// </summary>
        public bool AllowRemove
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the item count of the source array.
        /// </summary>
        public int Count
        {
            get { return List.Count; }
        }

        /// <summary>
        /// Gets if the binding source is a fixed size. Always returns false.
        /// </summary>
        public bool IsFixedSize
        {
            get { return false; }
        }

        /// <summary>
        /// Gets whether the binding source should be read only. Always returns false.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Gets whether the souce array is sorted. Always returns true.
        /// </summary>
        public bool IsSorted
        {
            get { return true; }
        }

        /// <summary>
        /// Gets whether the binding source is syncronized. Always returns true.
        /// </summary>
        public bool IsSynchronized
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the source <see cref="JsonArray"/>
        /// </summary>
        public JsonArray List
        {
            get
            {
                return _array;
            }
        }

        /// <summary>
        /// Gets the current position in the source <see cref="JsonArray"/>.
        /// </summary>
        public int Position
        {
            get; private set;
        }

        /// <summary>
        /// Gets the sort direction. Always returns Ascending.
        /// </summary>
        public ListSortDirection SortDirection
        {
            get { return ListSortDirection.Ascending; }
        }

        /// <summary>
        /// Not used. Returns null.
        /// </summary>
        public PropertyDescriptor SortProperty
        {
            get { return null; }
        }

        /// <summary>
        /// Gets whether the binding source supports change notifications. Always returns true.
        /// </summary>
        public bool SupportsChangeNotification
        {
            get { return true; }
        }

        /// <summary>
        /// Gets whether the binding source supports searching. Always returns false.
        /// </summary>
        public bool SupportsSearching
        {
            get { return false; }
        }

        /// <summary>
        /// Gets whether the binding source supports sorting. Always returns false.
        /// </summary>
        public bool SupportsSorting
        {
            get { return false; }
        }

        /// <summary>
        /// Not used. Always returns null.
        /// </summary>
        public object SyncRoot
        {
            get { return null; }
        }

        #endregion Properties

        #region Indexers

        /// <summary>
        /// Gets or Sets a <see cref="JsonValue"/> at the specified index.
        /// </summary>
        public object this[int index]
        {
            get
            {
                Position = index;
                object obj = this.List[index];
                if (obj is JsonNull)
                    return "";
                return this.List[index];
            }
            set
            {
                Position = index;
                if (value is JsonValue)
                {
                    List[index] = (JsonValue)value;
                    OnListChanged(ListChangedType.ItemChanged, index);
                }
            }
        }

        #endregion Indexers

        #region Methods

        /// <summary>
        /// Adds a value to the source <see cref="JsonArray"/>.
        /// </summary>
        /// <param name="value">The value to add to the array.</param>
        /// <returns>Returns the index of the newly added value within the array.</returns>
        public int Add(object value)
        {
            if (value is JsonValue)
            {
                int i = List.Add((JsonValue)value);
                OnListChanged(ListChangedType.ItemAdded, i);
                return i;
            }
            else
                return -1;
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="property"></param>
        public void AddIndex(PropertyDescriptor property)
        {
        }

        /// <summary>
        /// Creates a new instance of the specified type, provided by the constructor.
        /// </summary>
        /// <returns>Returns the newly created instance.</returns>
        public object AddNew()
        {
            

            JsonValue newValue = _prototype.Clone();
            newPos = List.Add(newValue);
            this.Position = newPos;
            if (newPos != -1)
                this.OnListChanged(ListChangedType.ItemAdded, newPos);


            //if (vtype == typeof(JsonString))
            //    newValue = new JsonString();
            //else if (vtype == typeof(JsonNumber))
            //    newValue = new JsonNumber();
            //else if (vtype == typeof(JsonBoolean))
            //    newValue = new JsonBoolean();
            //else if (vtype == typeof(JsonObject))
            //{
            //    JsonObject obj = new JsonObject();
            //    foreach (PropertyDescriptor prop in props)
            //    {
            //        var constr = prop.PropertyType.GetConstructors();
            //        obj.Add(prop.Name, (JsonValue)Activator.CreateInstance(prop.PropertyType));
            //    }

            //    newValue = obj;
            //}
            //if (newValue != null)
            //{
            //    newPos = List.Add(newValue);
            //    this.Position = newPos;
            //}
            return newValue;
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="direction"></param>
        public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
        {
        }

        /// <summary>
        /// Cancels the newly created item.
        /// </summary>
        /// <param name="itemIndex">The index of the new item that should be deleted.</param>
        public void CancelNew(int itemIndex)
        {
            if (this.newPos >= 0 && this.newPos == this.Position)
            {
                this.RemoveAt(this.newPos);
                this.newPos = -1;
                return;
            }
        }

        /// <summary>
        /// Clears all items from the source <see cref="JsonArray"/>.
        /// </summary>
        public void Clear()
        {
            List.Clear();
            Position = -1;
            newPos = -1;
            OnListChanged(ListChangedType.Reset, -1);
        }

        /// <summary>
        /// Determines if the array contains a value.
        /// </summary>
        /// <param name="value">The value to test if contained in the array.</param>
        /// <returns>Returns true if value is contained in the array; otherwise false.</returns>
        public bool Contains(object value)
        {
            if (value is JsonValue)
                return List.Contains((JsonValue)value);
            else
                return false;
        }

        /// <summary>
        /// Not Implemented
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        public void CopyTo(Array array, int index)
        {
            //List.CopyTo(array, index);
        }

        /// <summary>
        /// Commits a pending new item to the collection.
        /// </summary>
        /// <param name="itemIndex">The index of the item that was previously added to the collection.</param>
        public void EndNew(int itemIndex)
        {
            if (this.newPos >= 0 && this.newPos == this.Position)
            {
                this.newPos = -1;
                return;
            }
        }

        /// <summary>
        /// Not Implemented
        /// </summary>
        /// <param name="property"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public int Find(PropertyDescriptor property, object key)
        {
            return -1;
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An System.Collections.IEnumerator object that can be used to iterate through the collection.</returns>
        public System.Collections.IEnumerator GetEnumerator()
        {
            return List.GetEnumerator();
        }


        /// <summary>
        /// Returns the System.ComponentModel.PropertyDescriptorCollection that represents the properties on each item used to bind data.
        /// </summary>
        /// <param name="listAccessors">An array of System.ComponentModel.PropertyDescriptor objects to find in the collection as bindable. This can be null.</param>
        /// <returns>The System.ComponentModel.PropertyDescriptorCollection that represents the properties on each item used to bind data.</returns>
        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            return props;
        }

        /// <summary>
        /// Returns the name of the list.
        /// </summary>
        /// <param name="listAccessors">An array of System.ComponentModel.PropertyDescriptor objects, for which the list name is returned. This can be null.</param>
        /// <returns>The name of the list.</returns>
        public string GetListName(PropertyDescriptor[] listAccessors)
        {
            return "List";
        }

        /// <summary>
        /// Determines the index of a specific item in the System.Collections.IList.
        /// </summary>
        /// <param name="value">The System.Object to locate in the System.Collections.IList.</param>
        /// <returns>The index of value if found in the list; otherwise, -1.</returns>
        public int IndexOf(object value)
        {
            if (value is JsonValue)
                return List.Values.IndexOf((JsonValue)value);
            else
                return -1;
        }

        /// <summary>
        /// Inserts an item to the System.Collections.IList at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which value should be inserted.</param>
        /// <param name="value">The System.Object to insert into the System.Collections.IList.</param>
        public void Insert(int index, object value)
        {
            if (value is JsonValue)
            {
                List.Values.Insert(index, (JsonValue)value);
                OnListChanged(ListChangedType.ItemAdded, index);
            }
        }

        /// <summary>
        /// Invokes the <see cref="ListChanged"/> event.
        /// </summary>
        /// <param name="listChangeType">Specifies how the list changed.</param>
        /// <param name="newIndex">The index of the changed item.</param>
        public virtual void OnListChanged(ListChangedType listChangeType, int newIndex)
        {
            if (ListChanged != null)
                ListChanged(this, new ListChangedEventArgs(listChangeType, newIndex));
        }

        /// <summary>
        /// Removes a value from the list.
        /// </summary>
        /// <param name="value">The value to remove.</param>
        public void Remove(object value)
        {
            int i = this.IndexOf(value);
            if (value is JsonValue)
                List.Remove((JsonValue)value);
            if (i != -1)
                OnListChanged(ListChangedType.ItemDeleted, i);
        }

        /// <summary>
        /// Removes a value from the list at the given index.
        /// </summary>
        /// <param name="index">The index to remove from.</param>
        public void RemoveAt(int index)
        {
            List.RemoveAt(index);
            OnListChanged(ListChangedType.ItemDeleted, index);
        }

        /// <summary>
        /// Not Implemented.
        /// </summary>
        /// <param name="property"></param>
        public void RemoveIndex(PropertyDescriptor property)
        {
        }

        /// <summary>
        /// Not Implemented
        /// </summary>
        public void RemoveSort()
        {
        }

        //private void ReadProperties()
        //{
        //    bool propsSet = false;
        //    foreach (JsonValue item in _array)
        //    {
        //        if (item.GetType() != vtype)
        //            throw new Exception("A value is not of the specified type.");
        //        if (!propsSet)
        //        {
        //            props = TypeDescriptor.GetProperties(item);
        //            propsSet = true;
        //        }
        //        else
        //        {
        //            PropertyDescriptorCollection thisProps = TypeDescriptor.GetProperties(item);
        //            if (thisProps.Count != props.Count)
        //                throw new Exception("Properties Mismatch");
        //            List<string> names = new List<string>();
        //            foreach (PropertyDescriptor prop in thisProps)
        //                names.Add(prop.Name);
        //            foreach (PropertyDescriptor prop in props)
        //            {
        //                names.Remove(prop.Name);
        //            }
        //            if (names.Count > 0)
        //                throw new Exception("Properties Mismatch");
        //        }

        //    }

        //    if (props != null)
        //    {
        //        foreach (PropertyDescriptor prop in props)
        //        {
        //            if (prop is JsonObjectPropertyDescriptor)
        //            {
        //                JsonObjectPropertyDescriptor jpd = (JsonObjectPropertyDescriptor)prop;
        //                jpd.CanSetNull = false;
        //            }
        //        }
        //    }
        //}

        #endregion Methods
    }
}