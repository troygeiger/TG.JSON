using System;
using System.Collections.Generic;
using System.Text;

namespace TG.JSON
{
    /// <summary>
    /// Notifies the Serialize methods to ignore a property.
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class JsonIgnorePropertyAttribute : Attribute
    {
        readonly bool ignoreProperty;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public JsonIgnorePropertyAttribute()
        {
            ignoreProperty = true;
        }

        /// <summary>
        /// Constructor that you define whether to ignore the property or not.
        /// </summary>
        /// <param name="ignore">To ignore or not to ignore.</param>
        public JsonIgnorePropertyAttribute(bool ignore)
        {
            ignoreProperty = ignore;
        }

        /// <summary>
        /// Get whether the property should be ignored.
        /// </summary>
        public bool IgnoreProperty
        {
            get { return ignoreProperty; }
        }

    }
}
