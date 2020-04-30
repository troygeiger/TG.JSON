using System;
using System.Collections.Generic;
using System.Text;

namespace TG.JSON.Serialization
{
    /// <summary>
    /// Represents the options that will be used during serialization.
    /// </summary>
    public class JsonSerializationOptions
    {
        int _maxDepth;

        /// <summary>
        /// Creates a new instance of <see cref="JsonSerializationOptions"/>.
        /// </summary>
        public JsonSerializationOptions()
        {
            MaxDepth = int.MaxValue;
        }

        /// <summary>
        /// Creates a new instance of <see cref="JsonSerializationOptions"/>.
        /// </summary>
        /// <param name="maxDepth">The maximum depth to drill down when serializing.</param>
        /// <param name="includeAttributes">Include property attributes when serializing.</param>
        /// <param name="includeTypeInformation">If True, a property of "_type" will be added containing the fully qualified name of the object.</param>
        public JsonSerializationOptions(int maxDepth, bool includeAttributes, bool includeTypeInformation)
        {
            MaxDepth = maxDepth;
            IncludeAttributes = includeAttributes;
            IncludeTypeInformation = includeTypeInformation;
        }

        /// <summary>
        /// Creates a new instance of <see cref="JsonSerializationOptions"/>.
        /// </summary>
        /// <param name="maxDepth">The maximum depth to drill down when serializing.</param>
        /// <param name="includeAttributes">Include property attributes when serializing.</param>
        /// <param name="includeTypeInformation">If True, a property of "_type" will be added containing the fully qualified name of the object.</param>
        /// <param name="ignoreProperties">The properties that should be ignored when serializing.</param>
        /// <param name="selectedProperties">Property names added to this list will be the only properties to be serialized; unless specified in the <see cref="IgnoreProperties"/> list.</param>
        public JsonSerializationOptions(int maxDepth, bool includeAttributes, bool includeTypeInformation, string[] ignoreProperties, string[] selectedProperties)
            : this(maxDepth, includeAttributes, includeTypeInformation)
        {
            if (ignoreProperties != null)
            {
                IgnoreProperties.AddRange(ignoreProperties);
            }

            if (selectedProperties != null)
            {
                SelectedProperties.AddRange(selectedProperties); 
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="JsonSerializationOptions"/>.
        /// </summary>
        /// <param name="maxDepth">The maximum depth to drill down when serializing.</param>
        /// <param name="applySelectedPropertiesOnChildren">Get or Set whether child objects will be evaluated against the <see cref="SelectedProperties"/> and <see cref="IgnoreProperties"/> when serializing.</param>
        /// <param name="includeAttributes">Include property attributes when serializing.</param>
        /// <param name="includeTypeInformation">If True, a property of "_type" will be added containing the fully qualified name of the object.</param>
        public JsonSerializationOptions(int maxDepth, bool applySelectedPropertiesOnChildren, bool includeAttributes, bool includeTypeInformation)
            : this(maxDepth, includeAttributes, includeTypeInformation)
        {
            ApplySelectedPropertiesOnChildren = applySelectedPropertiesOnChildren;
        }


        /// <summary>
        /// Creates a new instance of <see cref="JsonSerializationOptions"/>.
        /// </summary>
        /// <param name="maxDepth">The maximum depth to drill down when serializing.</param>
        /// <param name="applySelectedPropertiesOnChildren">Get or Set whether child objects will be evaluated against the <see cref="SelectedProperties"/> and <see cref="IgnoreProperties"/> when serializing.</param>
        /// <param name="includeAttributes">Include property attributes when serializing.</param>
        /// <param name="includeTypeInformation">If True, a property of "_type" will be added containing the fully qualified name of the object.</param>
        /// <param name="ignoreProperties">The properties that should be ignored when serializing.</param>
        /// <param name="selectedProperties">Property names added to this list will be the only properties to be serialized; unless specified in the <see cref="IgnoreProperties"/> list.</param>
        public JsonSerializationOptions(int maxDepth, bool applySelectedPropertiesOnChildren, bool includeAttributes, bool includeTypeInformation, string[] ignoreProperties, string[] selectedProperties)
            : this(maxDepth, includeAttributes, includeTypeInformation, ignoreProperties, selectedProperties)
        {
            ApplySelectedPropertiesOnChildren = applySelectedPropertiesOnChildren;
        }



        /// <summary>
        /// The maximum depth to drill down when serializing.
        /// </summary>
        public int MaxDepth
        {
            get { return _maxDepth; }
            set
            {
                _maxDepth = value;
                CurrentDepth = value;
            }
        }


        internal int CurrentDepth { get; set; }

        /// <summary>
        /// Get or Set whether child objects will be evaluated against the <see cref="SelectedProperties"/> when serializing.
        /// </summary>
        public bool ApplySelectedPropertiesOnChildren { get; set; } = false;

        /// <summary>
        /// Include property attributes when serializing.
        /// </summary>
        public bool IncludeAttributes { get; set; } = false;

        /// <summary>
        /// If True, a property of "_type" will be added containing the fully qualified name of the object.
        /// </summary>
        public bool IncludeTypeInformation { get; set; } = false;

        /// <summary>
        /// Add property names that should be ignored when serializing.
        /// </summary>
        public List<string> IgnoreProperties { get; } = new List<string>();

        /// <summary>
        /// Property names added to this list will be the only properties to be serialized; unless specified in the <see cref="IgnoreProperties"/> list.
        /// </summary>
        public List<string> SelectedProperties { get; } = new List<string>();
    }
}
