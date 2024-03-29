﻿namespace TG.JSON
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// Represents a json array. Ex. { "array" : [ 1 , 2 , true, false] }
    /// </summary>
    /// <example>
    /// <code>
    ///	JsonArray myArray = new JsonArray("Hello", "World", 1, 2, 3, true);
    ///	for (int i = 0; i &lt; myArray.Count();i++)
    ///	{
    ///		Console.Write(myArray[i].ToString());
    ///	}
    /// </code>
    /// </example>
#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough()]
#endif
#if FULLNET
    [Serializable]
    [Editor(typeof(TG.JSON.Editors.JsonArrayCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))] 
#endif
    public class JsonArray : JsonValue,
    IEnumerable<JsonValue>,
    ICollection,
    System.Collections.IList,
#if !NETSTANDARD1_0
    System.Runtime.Serialization.ISerializable,
    System.Xml.Serialization.IXmlSerializable, 
#endif
    INotifyPropertyChanged
    {
        #region Fields

        List<JsonValue>.Enumerator enumerator;
        List<JsonValue> _values = new List<JsonValue>();
        static Dictionary<Type, Type> iListParamTypeCache = new Dictionary<Type, Type>();
        static Dictionary<Type, Type[]> dictionaryParamTypeCache = new Dictionary<Type, Type[]>();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes an empty <see cref="JsonArray"/>.
        /// </summary>
        public JsonArray()
        {
            enumerator = _values.GetEnumerator();
        }

#if !NETSTANDARD1_0
        /// <summary>
        /// Initializes an empty <see cref="JsonArray"/>.
        /// </summary>
        /// <param name="encryption">The <see cref="IEncryptionHandler"/> used to encrypt and decrypt values.</param>
        public JsonArray(IEncryptionHandler encryption)
        {
            GlobalEncryptionHandler = encryption;
            enumerator = _values.GetEnumerator();
        } 
#endif

        /// <summary>
        /// Initializes a new instance of <see cref="JsonArray"/> with a range of <see cref="JsonValue"/> values.
        /// </summary>
        /// <param name="range">A range of <see cref="JsonValue"/> values.</param>
        public JsonArray(IEnumerable<JsonValue> range)
            : this()
        {
            AddRange(range);
        }

#if !NETSTANDARD1_0
        /// <summary>
        /// Initializes a new instance of <see cref="JsonArray"/> with a range of <see cref="JsonValue"/> values.
        /// </summary>
        /// <param name="range">A range of <see cref="JsonValue"/> values.</param>
        /// <param name="encryption">The <see cref="IEncryptionHandler"/> used to encrypt and decrypt values.</param>
        public JsonArray(IEnumerable<JsonValue> range, IEncryptionHandler encryption)
            : this()
        {
            GlobalEncryptionHandler = encryption;
            AddRange(range);
        } 
#endif


        /// <summary>
        /// Initializes a new instance of <see cref="JsonArray"/> with a range of <see cref="JsonValue"/> values.
        /// </summary>
        /// <example>
        /// <code>
        ///		JsonArray myArray = new JsonArray("Hello", "World", 1, 2, 3, true);
        ///		Console.Write(myArray.ToString());
        /// </code>
        /// </example>
        /// <param name="range">A range of <see cref="JsonValue"/> values.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public JsonArray(params JsonValue[] range)
            : this()
        {
            AddRange(range);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="JsonArray"/> and parses the json array string provided by parameter json.
        /// </summary>
        /// <example>
        /// <code>
        ///		JsonArray myArray = new JsonArray("[1,2,3,4, \"Hello\", false]");
        ///		Console.Write(myArray.ToString());
        ///	</code>
        /// </example>
        /// <param name="json">A json array formatted string. Ex. [ 1, 2, 3, 4]</param>
        public JsonArray(string json)
            : this()
        {
            InternalParse(json);
        }

#if !NETSTANDARD1_0
        /// <summary>
        /// Initializes a new instance of <see cref="JsonArray"/> and parses the json array string provided by parameter json.
        /// </summary>
        /// <example>
        /// <code>
        ///		JsonArray myArray = new JsonArray("[1,2,3,4, \"Hello\", false]");
        ///		Console.Write(myArray.ToString());
        ///	</code>
        /// </example>
        /// <param name="json">A json array formatted string. Ex. [ 1, 2, 3, 4]</param>
        /// <param name="encryption">The <see cref="IEncryptionHandler"/> used to encrypt and decrypt values.</param>
        public JsonArray(string json, IEncryptionHandler encryption)
            : this()
        {
            GlobalEncryptionHandler = encryption;
            InternalParse(json);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="JsonArray"/> and parses using the provided <see cref="JsonReader"/>.
        /// </summary>
        /// <param name="reader">Used to read a string of json.</param>
        /// <param name="encryptionHandler">The <see cref="IEncryptionHandler"/> used to encrypt and decrypt values.</param>
        public JsonArray(JsonReader reader, IEncryptionHandler encryptionHandler)
            : this()
        {

            InternalParse(reader);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="JsonArray"/> using a source <see cref="System.IO.Stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="System.IO.Stream"/> to read from.</param>
        /// <param name="encryptionHandler">The <see cref="IEncryptionHandler"/> used to encrypt and decrypt values.</param>
        public JsonArray(System.IO.Stream stream, IEncryptionHandler encryptionHandler) : this()
        {
            GlobalEncryptionHandler = encryptionHandler;
            using (JsonReader reader = new JsonReader(stream))
            {
                InternalParse(reader);
            }
        }

        /// <summary>
        /// Initializes a new instance of <see cref="JsonArray"/> using a source <see cref="System.IO.Stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="System.IO.StreamReader"/> to read from.</param>
        /// <param name="encryptionHandler">The <see cref="IEncryptionHandler"/> used to encrypt and decrypt values.</param>
        public JsonArray(System.IO.StreamReader stream, IEncryptionHandler encryptionHandler) : this()
        {
            using (JsonReader reader = new JsonReader(stream))
            {
                InternalParse(reader);
            }
        }

#endif

        /// <summary>
        /// Initializes a new instance of <see cref="JsonArray"/> using a source <see cref="System.IO.Stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="System.IO.Stream"/> to read from.</param>
        public JsonArray(System.IO.Stream stream) : this()
        {
            using (JsonReader reader = new JsonReader(stream))
            {
                InternalParse(reader);
            }
        }

        /// <summary>
        /// Initializes a new instance of <see cref="JsonArray"/> using a source <see cref="System.IO.Stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="System.IO.StreamReader"/> to read from.</param>
        public JsonArray(System.IO.StreamReader stream) : this()
        {
            using (JsonReader reader = new JsonReader(stream))
            {
                InternalParse(reader);
            }
        }

        /// <summary>
        /// Initializes a new instance of <see cref="JsonArray"/> and parses using the provided <see cref="JsonReader"/>.
        /// </summary>
        /// <param name="reader">Used to read a string of json.</param>
        public JsonArray(JsonReader reader)
            : this()
        {
            InternalParse(reader);
        }
        
        internal JsonArray(JsonReader reader, JsonValue parent)
            : this()
        {
            SetParent(parent);
            InternalParse(reader);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="JsonArray"/> and populating with the values of an array.
        /// </summary>
        /// <param name="array">An array used to populate the new <see cref="JsonArray"/></param>
        public JsonArray(Array array)
            : this()
        {
            foreach (object item in array)
                InternalAdd(base.ValueFromObject(item));
        }

        /// <summary>
        /// Initializes a new instance of <see cref="JsonArray"/> and populating with the values of an array.
        /// </summary>
        /// <param name="enumerable">An <see cref="IEnumerable"/> used to populate the new <see cref="JsonArray"/></param>
        public JsonArray(IEnumerable enumerable)
            : this()
        {
            foreach (object item in enumerable)
            {
                InternalAdd(base.ValueFromObject(item));
            }
        }


#if !NETSTANDARD1_0
        /// <summary>
        /// Implementation for <see cref="System.Runtime.Serialization.ISerializable"/>.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        JsonArray(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : this()
        {
            //info.AddValue("Value", this.ToString());
            if (info.MemberCount > 0)
                InternalParse(info.GetString("Value"));
        } 
#endif

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the quantity of <see cref="JsonValue"/> in the array.
        /// </summary>
        public int Count
        {
            get { return _values.Count; }
        }

        /// <summary>
        /// Implemented from <see cref="System.Collections.IList"/>. Always returns false.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the <see cref="List{JsonValue}"/> collection associated with this array.
        /// </summary>
#if FULLNET
        [Editor(typeof(TG.JSON.Editors.JsonArrayCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))] 
#endif
        public List<JsonValue> Values
        {
            get { return _values; }
        }

        /// <summary>
        /// Returns <see cref="JsonValueTypes.Array"/>
        /// </summary>
        public override JsonValueTypes ValueType
        {
            get { return JsonValueTypes.Array; }
        }

        int System.Collections.ICollection.Count
        {
            get { return this.Count; }
        }

        bool System.Collections.ICollection.IsSynchronized
        {
            get { return true; }
        }

        object System.Collections.ICollection.SyncRoot
        {
            get { return null; }
        }

        bool System.Collections.IList.IsFixedSize
        {
            get
            {
                return false;
            }
        }

        bool System.Collections.IList.IsReadOnly
        {
            get { return false; }
        }

        object System.Collections.IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                if (value is JsonValue)
                    this[index] = (JsonValue)value;
            }
        }

        #endregion Properties

        #region Indexers

        /// <summary>
        /// Gets or Sets a <see cref="JsonValue"/> at the given index.
        /// </summary>
        /// <param name="index">The index at which to get or set.</param>
        /// <returns><see cref="JsonValue"/></returns>
        public JsonValue this[int index]
        {
            get { return _values[index]; }
            set
            {
                _values[index] = value;
                OnValueChanged();
            }
        }

        #endregion Indexers

        #region Methods

        /// <summary>
        /// Creates an instance of <see cref="JsonArray"/> by parsing the provided JSON array string.
        /// </summary>
        /// <param name="json">JSON array formatted string.</param>
        /// <returns><see cref="JsonArray"/></returns>
        public static JsonArray Parse(string json)
        {
            JsonArray results = new JsonArray();
            results.InternalParse(json);
            return results;
        }

        /// <summary>
        /// Adds a <see cref="JsonValue"/> to the array.
        /// </summary>
        /// <param name="obj">A <see cref="JsonValue"/> value.</param>
        public int Add(JsonValue obj)
        {
            int i = this.InternalAdd(obj);
            OnValueChanged();
            return i;
        }

        /// <summary>
        /// Adds a range of <see cref="JsonValue"/> to the array.
        /// </summary>
        /// <param name="range">A range of <see cref="JsonValue"/>.</param>
        public void AddRange(IEnumerable<JsonValue> range)
        {
            foreach (var item in range)
                this.InternalAdd(item);
        }

        /// <summary>
        /// Adds a range of <see cref="JsonValue"/> to the array.
        /// </summary>
        /// <param name="range">A range of <see cref="JsonValue"/>.</param>
        public void AddRange(params JsonValue[] range)
        {
            if (range == null)
                return;
            foreach (JsonValue item in range)
            {
                this.InternalAdd(item);
            }
            OnValueChanged();
        }

        /// <summary>
        /// Clears all values in the array.
        /// </summary>
		public void Clear()
        {
            _values.Clear();
            OnValueChanged();
        }

        /// <summary>
        /// Creates an exact copy of the <see cref="JsonArray"/>.
        /// </summary>
        /// <remarks>
        /// This method calls the <see cref="ToString"/> method and passes the JSON string to the constructor of a new <see cref="JsonArray"/>.
        /// </remarks>
        /// <returns>A new instance of <see cref="JsonArray"/>.</returns>
        public override JsonValue Clone()
        {
            return new JsonArray(this.ToString());
        }

        /// <summary>
        /// Determine whether the <see cref="JsonArray"/> contains the <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="item">The <see cref="JsonValue"/> to test if it exists in the array.</param>
        /// <returns>true if the <para name="item"/> is found in the <see cref="JsonArray"/>; otherwise false.</returns>
        public bool Contains(JsonValue item)
        {
            return _values.Contains(item);
        }

        /// <summary>
        /// Determine whether a <see cref="JsonString"/> is in the <see cref="JsonArray"/> searched using a string.
        /// </summary>
        /// <param name="value">A string to search for.</param>
        /// <returns>true if the <para name="item"/> is found in the <see cref="JsonArray"/>; otherwise false.</returns>
        public bool Contains(string value)
        {
            for (int i = 0; i < this.Count; i++)
            {
                JsonValue v = _values[i];
                if (v is JsonString)
                    if (((JsonString)v).Value == value)
                        return true;
            }
            return false;
        }

        /// <summary>
        /// Determines if a <see cref="JsonObject"/> exists within the <see cref="JsonArray"/> that has a property with a certain value.
        /// </summary>
        /// <param name="property">The property to search.</param>
        /// <param name="value">The value of the property that should match.</param>
        /// <returns></returns>
        public bool Contains(string property, JsonValue value)
        {
            JsonObject results = FindObject(property, value);
            return results != null;
        }

        /// <summary>
        /// Copies the contents of this <see cref="JsonArray"/> to a <see cref="JsonValue"/> array.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="JsonValue"/> array to copy to.</param>
        /// <param name="arrayIndex">The zero-based index in the array at which copying begins.</param>
        public void CopyTo(JsonValue[] array, int arrayIndex)
        {
            _values.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Initializes a new instance of the <paramref name="type"/> and populates with the values of this <see cref="JsonArray"/>.
        /// </summary>
        /// <param name="type">The type of collection or array to create.</param>
        /// <returns>A new instance of <paramref name="type"/>.</returns>
        public object Deserialize(Type type)
        {
            if (type.IsArray)
            {
                return DeserializeArray(type.GetElementType());
            }
            else
            {
                IList lst = (IList)Activator.CreateInstance(type);
                DeserializeInto(lst);
                return lst;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <typeparamref name="T"/> and populates with the values of this <see cref="JsonArray"/>.
        /// </summary>
        /// <typeparam name="T">The type of collection or array to create.</typeparam>
        /// <returns>A new instance of <typeparamref name="T"/>.</returns>
        public T Deserialize<T>()
        {
            return (T)Deserialize(typeof(T));
        }

        /// <summary>
        /// Initializes a new instance of the <paramref name="arrayType"/> and populates with the values of this <see cref="JsonArray"/>.
        /// </summary>
        /// <param name="arrayType">The type of collection or array to create.</param>
        /// <returns>A new instance of <paramref name="arrayType"/>.</returns>
        public object DeserializeArray(Type arrayType)
        {
            if (arrayType.IsArray)
            {
                arrayType = arrayType.GetElementType();
            }
            Array a = Array.CreateInstance(arrayType, Count);
            for (int i = 0; i < Count; i++)
            {
                JsonValue v = this[i];
                switch (v.ValueType)
                {
                    case JsonValueTypes.String:
                        if (arrayType == typeof(string))
                            a.SetValue((string)v, i);
                        else if (arrayType == typeof(char))
                            a.SetValue(Convert.ChangeType(v, typeof(char)), i);
                        break;
                    case JsonValueTypes.Object:
                        a.SetValue(((JsonObject)v).DeserializeObject(arrayType), i);
                        break;
                    case JsonValueTypes.Array:
                        break;
                    case JsonValueTypes.Number:
                        if (arrayType == typeof(short))
                            a.SetValue((short)v, i);
                        else if (arrayType == typeof(int))
                            a.SetValue((int)v, i);
                        else if (arrayType == typeof(long))
                            a.SetValue((long)v, i);
                        else if (arrayType == typeof(ushort))
                            a.SetValue((ushort)v, i);
                        else if (arrayType == typeof(uint))
                            a.SetValue((uint)v, i);
                        else if (arrayType == typeof(ulong))
                            a.SetValue((ulong)v, i);
                        else if (arrayType == typeof(float))
                            a.SetValue((float)v, i);
                        else if (arrayType == typeof(double))
                            a.SetValue((double)v, i);
                        else if (arrayType == typeof(decimal) || arrayType == typeof(object))
                            a.SetValue((decimal)v, i);
                        else if (arrayType == typeof(byte))
                            a.SetValue((byte)v, i);
                        break;
                    case JsonValueTypes.Boolean:
                        if (arrayType == typeof(bool) || arrayType == typeof(object))
                            a.SetValue((bool)v, i);
                        break;
                    case JsonValueTypes.Binary:
                        break;
                    case JsonValueTypes.Null:
                        break;
                    default:
                        break;
                }
            }

            return a;
        }

        private Type GetListAddType(Type iListType)
        {
            if (iListParamTypeCache.ContainsKey(iListType))
            {
                return iListParamTypeCache[iListType];
            }
            MethodInfo addMethod = null;
#if FULLNET
            addMethod = iListType.GetMethod("Add");
#else
            foreach (MethodInfo item in iListType.GetRuntimeMethods())
            {
                if (item.Name == "Add")
                {
                    addMethod = item;
                    break;
                }
            }
#endif
            if (addMethod == null) return null;

            ParameterInfo[] parameters = addMethod.GetParameters();
            if (parameters.Length < 1)
                return null;
            Type paramType = parameters[0].ParameterType;
            iListParamTypeCache.Add(iListType, paramType);
            return paramType;
        }

        private Type[] GetDictionaryAddType(Type dictType)
        {
            if (dictionaryParamTypeCache.ContainsKey(dictType))
            {
                return dictionaryParamTypeCache[dictType];
            }
            MethodInfo addMethod = null;
#if FULLNET
            addMethod = dictType.GetMethod("Add");
#else
            foreach (MethodInfo item in dictType.GetRuntimeMethods())
            {
                if (item.Name == "Add")
                {
                    addMethod = item;
                    break;
                }
            }
#endif
            if (addMethod == null) return null;

            ParameterInfo[] parameters = addMethod.GetParameters();
            if (parameters.Length != 2)
                return null;
            Type[] types = new Type[2];
            types[0] = parameters[0].ParameterType;
            types[1] = parameters[1].ParameterType;
            dictionaryParamTypeCache.Add(dictType, types);
            return types;
        }

        /// <summary>
        /// Initializes a new instance of the <typeparamref name="T"/> and populates with the values of this <see cref="JsonArray"/>.
        /// </summary>
        /// <typeparam name="T">The type of collection or array to create.</typeparam>
        /// <returns>A new instance of <typeparamref name="T"/>.</returns>
        public T DeserializeArray<T>()
        {
            return (T)DeserializeArray(typeof(T));
        }

        /// <summary>
        /// Populates the <paramref name="lst"/> with the values of this <see cref="JsonArray"/>.
        /// </summary>
        /// <param name="lst">The <see cref="IList"/> to populate.</param>
        public void DeserializeInto(IList lst)
        {
            if (lst == null)
                return;
            
            Type paramType = GetListAddType(lst.GetType());
            

            for (int i = 0; i < Count; i++)
            {
                JsonValue v = this[i];
                try
                {
                    switch (v.ValueType)
                    {
                        case JsonValueTypes.Object:
                            lst.Add(((JsonObject)v).DeserializeObject(paramType));
                            break;
                        case JsonValueTypes.Array:
                            throw new NotImplementedException("Deserializing array values has not been implemented.");
                        case JsonValueTypes.String:
                        case JsonValueTypes.Number:
                        case JsonValueTypes.Boolean:
                            lst.Add(Convert.ChangeType(v, paramType));
                            break;
                        case JsonValueTypes.Binary:
                            throw new NotImplementedException("Deserializing Binary values has not been implemented.");
                        case JsonValueTypes.Null:
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception)
                {

                }
            }
        }

        /// <summary>
        /// Populates the <paramref name="dictionary"/> with the values of this <see cref="JsonArray"/>.
        /// </summary>
        /// <param name="dictionary">The <see cref="IDictionary"/> to populate.</param>
        public void DeserializeInto(IDictionary dictionary)
        {
            if (dictionary == null)
                return;

            Type[] types = GetDictionaryAddType(dictionary.GetType());
            if (types == null) return;
            Type keyType = types[0];
            Type valueType = types[1];

            

            for (int i = 0; i < Count; i++)
            {
                JsonValue v = this[i];
                try
                {
                    if (v.ValueType == JsonValueTypes.Object)
                    {
                        JsonObject obj = (JsonObject)v;
                        if (obj.ContainsProperty("Key") && obj.ContainsProperty("Value"))
                        {
                            JsonValue jKey = obj["Key"];
                            JsonValue jValue = obj["Value"];
                            object key = null, value = null;
                            if (keyType == typeof(string))
                                key = (string)jKey;
                            else if (keyType == typeof(byte))
                                key = (byte)jKey;
                            else if (keyType == typeof(short))
                                key = (short)jKey;
                            else if (keyType == typeof(int))
                                key = (int)jKey;
                            else if (keyType == typeof(long))
                                key = (long)jKey;
                            else if (keyType == typeof(ushort))
                                key = (ushort)jKey;
                            else if (keyType == typeof(uint))
                                key = (uint)jKey;
                            else if (keyType == typeof(ulong))
                                key = (ulong)jKey;
                            else if (keyType == typeof(float))
                                key = (float)jKey;
                            else if (keyType == typeof(double))
                                key = (double)jKey;
                            else if (keyType == typeof(decimal))
                                key = (decimal)jKey;
                            else if (jKey.ValueType == JsonValueTypes.Object)
                                key = ((JsonObject)jKey).DeserializeObject(keyType);

                            if (key == null)
                                continue;

                            if (jValue.ValueType == JsonValueTypes.Object)
                                value = ((JsonObject)jValue).DeserializeObject(valueType);
                            else if (jValue.ValueType == JsonValueTypes.Array)
                                value = ((JsonArray)jValue).Deserialize(valueType);
                            else if (jValue.ValueType == JsonValueTypes.Binary && valueType == typeof(byte[]))
                                value = (byte[])((JsonBinary)jValue).Value;
                            else if (valueType == typeof(string))
                                value = (string)jValue;
                            else if (valueType == typeof(byte))
                                value = (byte)jValue;
                            else if (valueType == typeof(short))
                                value = (short)jValue;
                            else if (valueType == typeof(int))
                                value = (int)jValue;
                            else if (valueType == typeof(long))
                                value = (long)jValue;
                            else if (valueType == typeof(ushort))
                                value = (ushort)jValue;
                            else if (valueType == typeof(uint))
                                value = (uint)jValue;
                            else if (valueType == typeof(ulong))
                                value = (ulong)jValue;
                            else if (valueType == typeof(float))
                                value = (float)jValue;
                            else if (valueType == typeof(double))
                                value = (double)jValue;
                            else if (valueType == typeof(decimal))
                                value = (decimal)jValue;
                            else if (valueType == typeof(bool))
                                value = (bool)jValue;

                            if (value == null && !IsNullable(valueType))
                                continue;
                            dictionary.Add(key, value);
                        }
                    }
                }
                catch (Exception)
                {

                }
            }
        }

        /// <summary>
        /// Searches the array for a <see cref="JsonObject"/> with a property with matching <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="property">The property to match within a <see cref="JsonObject"/>.</param>
        /// <param name="value">The property's value to match within a <see cref="JsonObject"/>.</param>
        /// <returns>A <see cref="JsonObject"/> with a matching property and value; otherwise null.</returns>
        public JsonObject FindObject(string property, JsonValue value)
        {
            int index = IndexOf(property, value);
            if (index == -1)
                return null;
            else
                return this[index] as JsonObject;
        }

        /// <summary>
        /// Searches the array for all <see cref="JsonObject"/> with a property with matching <see cref="JsonValue"/>.
        /// </summary>
		/// <param name="property">The property to match within a <see cref="JsonObject"/>.</param>
		/// <param name="value">The property's value to match within a <see cref="JsonObject"/>.</param>
		/// <returns>A <see cref="JsonObject"/> array with a matching property and value; otherwise empty.</returns>
        public JsonObject[] FindAllObjects(string property, JsonValue value)
        {
            List<JsonObject> objs = new List<JsonObject>();
            if (value == null)
                value = new JsonNull();
            int index = -1;
            while ((index = IndexOf(property, value, index + 1)) > -1)
                objs.Add(this[index] as JsonObject);
            return objs.ToArray();
        }

        /// <summary>
        /// Returns an enumerator that iterates through <see cref="JsonValue"/>s.
        /// </summary>
        public IEnumerator<JsonValue> GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        /// <summary>
        /// Searches for the specified <see cref="JsonObject"/> and returns the zero-based index of the first occurrence within the entire <see cref="JsonArray"/>.
        /// </summary>
        /// <param name="property">The property name within a <see cref="JsonObject"/>. Cannot be null.</param>
        /// <param name="value">The value of the property that should match.</param>
        /// <returns>The zero-based index of the matching <see cref="JsonObject"/>; otherwise -1 is returned.</returns>
        public int IndexOf(string property, JsonValue value)
        {
            return IndexOf(property, value, 0);
        }

        /// <summary>
        /// Searches for the specified <see cref="JsonObject"/> and returns the zero-based index of the first occurrence within the entire <see cref="JsonArray"/>.
        /// </summary>
        /// <param name="property">The property name within a <see cref="JsonObject"/>. Cannot be null.</param>
        /// <param name="value">The value of the property that should match.</param>
        /// <param name="indexStart">The index to begin searching from.</param>
        /// <returns>The zero-based index of the matching <see cref="JsonObject"/>; otherwise -1 is returned.</returns>
        public int IndexOf(string property, JsonValue value, int indexStart)
        {
            if (value == null)
                value = new JsonNull();
            if (string.IsNullOrEmpty(property))
                throw new ArgumentNullException("property");
            for (int i = indexStart; i < Count; i++)
            {
                JsonObject item = this[i] as JsonObject;
                if (item != null && item.ContainsProperty(property))
                {
                    JsonValue prop = item[property];
                    if (prop.ValueType == value.ValueType)
                    {
                        switch (value.ValueType)
                        {
                            case JsonValueTypes.String:
                                if ((prop as JsonString).Value == (value as JsonString).Value)
                                    return i;
                                break;
                            case JsonValueTypes.Number:
                                if ((prop as JsonNumber).Value == (value as JsonNumber).Value)
                                    return i;
                                break;
                            case JsonValueTypes.Boolean:
                                if ((prop as JsonBoolean).Value == (value as JsonBoolean).Value)
                                    return i;
                                break;
                            case JsonValueTypes.Binary:
                            case JsonValueTypes.Array:
                            case JsonValueTypes.Object:
                                if (prop.ToString() == value.ToString())
                                    return i;
                                break;
                            case JsonValueTypes.Null:
                                return i;
                            default:
                                break;
                        }
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// Inserts a <see cref="JsonValue"/> at the given index.
        /// </summary>
        /// <param name="index">The index to which the value should be inserted.</param>
        /// <param name="value">The value to insert.</param>
        public void Insert(int index, JsonValue value)
        {
            _values.Insert(index, value);
            OnValueChanged();
        }

        /// <summary>
        /// Reads JSON from a <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to read from.</param>
        public void Read(System.IO.Stream stream)
        {
            using (JsonReader reader = new JsonReader(stream))
            {
                InternalParse(reader);
            }
        }

#if !NETSTANDARD1_0
        /// <summary>
        /// Reads JSON from a file.
        /// </summary>
        /// <param name="path">The path to read from.</param>
        public void Read(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                Read(fs);
            }
        }
#endif

        /// <summary>
        /// Removes a <see cref="JsonValue"/> from the array.
        /// </summary>
        /// <param name="obj">A <see cref="JsonValue"/> to remove.</param>
        public void Remove(JsonValue obj)
        {
            _values.Remove(obj);
            OnValueChanged();
        }

        /// <summary>
        /// Removes a <see cref="JsonValue"/> from the array at the given index.
        /// </summary>
        /// <param name="index">The zero-based index at which to remove from.</param>
        public void RemoveAt(int index)
        {
            _values.RemoveAt(index);
            OnValueChanged();
        }

        /// <summary>
        /// Reverses the <see cref="JsonArray"/> with last to first/first to last.
        /// </summary>
        public void Reverse()
        {
            _values.Reverse();
            OnValueChanged();
        }

        /// <summary>
        /// Converts (Serializes) an <see cref="IEnumerable"/> object and all contained objects' properties to this <see cref="JsonArray"/>. Properties are converted to an <see cref="JsonValue"/> of a compatible type.
        /// NOTE: A new instance of <see cref="JsonArray"/> is not created.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="maxDepth">The maximum property dept to drill down to. This also prevents an endless loop if a property references to a point that could return to itself.</param>
        /// <param name="ignoreProperties">A params string array of property names that should be ignored.</param>
        /// <returns>The current instance of <see cref="JsonArray"/> populated with the serialized values of <paramref name="obj"/>. A new instance of <see cref="JsonArray"/> is not created.</returns>
        public JsonArray SerializeObject(object obj, int maxDepth, params string[] ignoreProperties)
        {
            return SerializeObject(obj, maxDepth, false, false, ignoreProperties);
        }


        /// <summary>
        /// Converts (Serializes) an <see cref="IEnumerable"/> object and all contained objects' properties to this <see cref="JsonArray"/>. Properties are converted to an <see cref="JsonValue"/> of a compatible type.
        /// NOTE: A new instance of <see cref="JsonArray"/> is not created.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="maxDepth">The maximum property dept to drill down to. This also prevents an endless loop if a property references to a point that could return to itself.</param>
        /// <param name="includeAttributes">If True, the AttributeTable will be populated with the object's attributes.</param>
        /// <param name="includeTypeInformation">If True, a _type property will be added with the full Type.AssemblyQualifiedName.</param>
        /// <param name="ignoreProperties">A params string array of property names that should be ignored.</param>
        /// <returns>The current instance of <see cref="JsonArray"/> populated with the serialized values of <paramref name="obj"/>. A new instance of <see cref="JsonArray"/> is not created.</returns>
        public JsonArray SerializeObject(object obj, int maxDepth, bool includeAttributes, bool includeTypeInformation, params string[] ignoreProperties)
        {
            return SerializeObject(obj, new Serialization.JsonSerializationOptions(maxDepth, includeAttributes, includeTypeInformation, ignoreProperties, null));
        }

        /// <summary>
        /// Converts (Serializes) an <see cref="IEnumerable"/> object and all contained objects' properties to this <see cref="JsonArray"/>. Properties are converted to an <see cref="JsonValue"/> of a compatible type.
        /// NOTE: A new instance of <see cref="JsonArray"/> is not created.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="serializationOptions">The options to use for serialization.</param>
        /// <returns>The current instance of <see cref="JsonArray"/> populated with the serialized values of <paramref name="obj"/>. A new instance of <see cref="JsonArray"/> is not created.</returns>
        public JsonArray SerializeObject(object obj, Serialization.JsonSerializationOptions serializationOptions)
        {
#if FULLNET || NETSTANDARD2_0
            if (!typeof(System.Collections.IEnumerable).IsAssignableFrom(obj.GetType()))
                return this; 
#else
            if (!typeof(System.Collections.IEnumerable).GetTypeInfo().IsAssignableFrom(obj.GetType().GetTypeInfo()))
                return this;
#endif
            System.Collections.IEnumerator enumer = ((System.Collections.IEnumerable)obj).GetEnumerator();
            while (enumer.MoveNext())
                this.Add(base.ValueFromObject(enumer.Current, serializationOptions));
            return this;
        }

        /// <summary>
        /// Sorts the values of the entire <see cref="JsonArray"/> using the specified <see cref="Comparison{JsonValue}"/>
        /// </summary>
        /// <param name="comparison"></param>
        public void Sort(Comparison<JsonValue> comparison)
        {
            _values.Sort(comparison);
            OnValueChanged();
        }

        void System.Collections.ICollection.CopyTo(Array array, int index)
        {
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        int System.Collections.IList.Add(object value)
        {
            if (value is JsonValue)
            {
                this.Add((JsonValue)value);
                return 1;
            }
            else
                return 0;
        }

        void System.Collections.IList.Clear()
        {
            this.Clear();
        }

        bool System.Collections.IList.Contains(object value)
        {
            if (value is JsonValue)
                return this.Contains((JsonValue)value);
            else
                return false;
        }

        int System.Collections.IList.IndexOf(object value)
        {
            if (value is JsonValue)
                return _values.IndexOf((JsonValue)value);
            else
                return -1;
        }

        void System.Collections.IList.Insert(int index, object value)
        {
            if (value is JsonValue)
                _values.Insert(index, (JsonValue)value);
        }

        void System.Collections.IList.Remove(object value)
        {
            if (value is JsonValue)
                this.Remove((JsonValue)value);
        }

        void System.Collections.IList.RemoveAt(int index)
        {
            _values.RemoveAt(index);
            OnValueChanged();
        }

#if !NETSTANDARD1_0
        void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            info.AddValue("Value", ToString());
            //if (info.MemberCount > 0)
            //InternalParse(info.GetString("Value"));
        }

        System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema()
        {
            return null;
        }

        void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
        {
            InternalParse(reader.ReadContentAsString());
        }

        void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteCData(this.ToString());
        } 
#endif

        /// <summary>
        /// Generates a JSON formatted array string. Ex. [ \"Hello\" , 1 ]
        /// </summary>
        /// <returns>JSON formatted array string.</returns>
        public override string ToString() 
        {
            return this.ToString(Formatting.Compressed);
        }
        
        internal override void InternalWrite(StreamWriter writer, Formatting format, int depth)
        {
            switch (format)
            {
                case Formatting.JavascriptCompressed:
                case Formatting.Compressed:
                    writer.Write("[");
                    for (int i = 0; i < _values.Count; i++)
                    {
                        if (i > 0) writer.Write(",");
                        _values[i].InternalWrite(writer, format, depth);
                    }
                    writer.Write("]");
                    break;
                case Formatting.Spaces:
                    writer.Write("[ ");
                    for (int i = 0; i < _values.Count; i++)
                    {
                        if (i > 0) writer.Write(" , ");
                        _values[i].InternalWrite(writer, format, depth);
                    }
                    writer.Write(" ]");
                    break;
                case Formatting.JavascriptIndented:
                case Formatting.Indented:
                    writer.Write("[");
                    int nextDepth = depth + 1;
                    string root = GenerateIndents(depth);
                    string indent = GenerateIndents(nextDepth);
                    int count = 0;
                    for (int i = 0; i < _values.Count; i++)
                    {
                        writer.WriteLine(i > 0 ? "," : "");
                        writer.Write(indent);
                        _values[i].InternalWrite(writer, format, nextDepth);
                        count++;
                    }
                    if (count > 0)
                    {
                        writer.WriteLine();
                        writer.Write($"{root}]");
                    }
                    else
                    {
                        writer.Write("]");
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Writes to a <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to write to.</param>
        /// <param name="formatting">How the output should be formatted.</param>
        public void Write(Stream stream, Formatting formatting)
        {
            using (StreamWriter writer = new StreamWriter(stream))
            {
                InternalWrite(writer, formatting, 0);
            }
        }

        /// <summary>
        /// Writes to a <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to write to.</param>
        public void Write(Stream stream)
        {
            Write(stream, Formatting.Compressed);
        }

#if !NETSTANDARD1_X
        /// <summary>
        /// Writes to a <see cref="Stream"/>.
        /// </summary>
        /// <param name="path">The file path to write to.</param>
        /// <param name="formatting">How the output should be formatted.</param>
        public void Write(string path, Formatting formatting)
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                InternalWrite(writer, formatting, 0);
            }
        }

        /// <summary>
        /// Writes to a <see cref="Stream"/>.
        /// </summary>
        /// <param name="path">The file path to write to.</param>
        public void Write(string path)
        {
            Write(path, Formatting.Compressed);
        } 
#endif

        /*
		/// <summary>
		/// Generates a JSON formatted array string. Ex. [ \"Hello\" , 1 ]
		/// </summary>
		/// <param name="compress">true removes all non-string spaces from the output. false spaces each item in the array.</param>
		/// <returns>JSON formatted array string.</returns>
		public override string ToString(bool compress)
		{

			StringBuilder sb = new StringBuilder();
			sb.Append("[");
			if (!compress)
				sb.Append(" ");
			for (int i = 0; i < _objects.Count; i++)
			{
				string v = _objects[i].ToString(compress);
				if (i == 0)
					sb.Append(v);
				else
				{
					if (compress)
						sb.Append("," + v);
					else
						sb.Append(" , " + v);
				}
			}
			if (!compress)
				sb.Append(" ");
			sb.Append("]");

			return sb.ToString();
		}

		 */
        private static JsonValue ValueFromString(string value)
        {
            decimal d = 0;
            bool b = false;
            if (string.IsNullOrEmpty(value) || value == "null")
                return new JsonNull();
            else if (bool.TryParse(value, out b))
                return new JsonBoolean(b);
            else if (decimal.TryParse(value, out d))
                return new JsonNumber(d);
            else
                return new JsonNull();
        }

        /// <summary>
        /// Used internally to add a <see cref="JsonValue"/> to the array without ValueChanged event.
        /// </summary>
        /// <param name="obj">A <see cref="JsonValue"/> value.</param>
        private int InternalAdd(JsonValue obj)
        {
            if (obj == null)
                obj = new JsonNull();
            obj.SetParent(this);
            int i = this.Count;
            _values.Add(obj);
            return i;
        }

        private void InternalParse(string json)
        {
            if (string.IsNullOrEmpty(json))
                return;
            json = json.Trim();
            if (!json.StartsWith("[") && !json.EndsWith("]"))
                return;
            InternalParse(new JsonReader(json));
        }

        private void InternalParse(JsonReader reader)
        {
            bool inString = false;
            bool inEsc = false;
            char chr;
            StringBuffer buffer = new StringBuffer();

            while (!reader.EndOfJson)
            {
                chr = reader.Read();
                if (inEsc)
                {
#region Handle Escape Char
                    char echr;
                    switch (chr)
                    {
                        case 'r':
                            echr = '\r';
                            break;
                        case 'n':
                            echr = '\n';
                            break;
                        case 'f':
                            echr = '\f';
                            break;
                        case 'b':
                            echr = '\b';
                            break;
                        case 't':
                            echr = '\t';
                            break;
                        default:
                            echr = chr;
                            break;
                    }
                    buffer.Add(echr);
                    inEsc = false;
                    continue;
#endregion Handle Escape Char
                }
                switch (chr)
                {
                    case '\\':
                        if (inString)
                            inEsc = true;
                        break;
                    case '"':
                        inString = !inString;
                        if (!inString)
                        {
                            string value = buffer.Dump();
                            if (value.StartsWith("base64"))
                                this.InternalAdd(new JsonBinary(value));
                            else
                                this.InternalAdd(new JsonString(value));
                        }
                        break;
                    case '[':
                        if (inString)
                            buffer.Add(chr);
                        else if (reader.Position > 1)
                        {
                            this.InternalAdd(new JsonArray(reader, this));
                        }
                        break;
                    case ']':
                        if (inString)
                            buffer.Add(chr);
                        else
                        {
                            if (buffer.Length > 0)
                                this.InternalAdd(ValueFromString(buffer.Dump()));
                            return;
                        }
                        break;
                    case ',':
                        if (inString)
                            buffer.Add(chr);
                        else if (buffer.Length > 0)
                            this.InternalAdd(ValueFromString(buffer.Dump()));
                        break;
                    case '{':
                        if (inString)
                            buffer.Add(chr);
                        else
                        {
                            this.InternalAdd(new JsonObject(reader, this));
                        }
                        break;
                    case '\r':
                    case '\n':
                    case '\t':
                        break;
                    case ' ':
                        if (inString)
                            buffer.Add(chr);
                        break;
                    default:
                        //if (inString)
                        buffer.Add(chr);
                        break;
                }
            }
        }

        private bool IsNullable(Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

#endregion Methods
    }
}