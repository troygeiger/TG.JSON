﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TG.JSON.Serialization
{
    /// <summary>
    /// Represents the options that will be used during serialization.
    /// </summary>
    public class JsonSerializationOptions
    {
        int _maxDepth = int.MaxValue;

        /// <summary>
        /// Creates a new instance of <see cref="JsonSerializationOptions"/>.
        /// </summary>
        public JsonSerializationOptions()
        {

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
        {
            MaxDepth = maxDepth;
            IncludeAttributes = includeAttributes;
            IncludeTypeInformation = includeTypeInformation;
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