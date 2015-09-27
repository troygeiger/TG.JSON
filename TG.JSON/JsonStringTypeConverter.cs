namespace TG.JSON
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    internal class JsonStringTypeConverter : System.ComponentModel.TypeConverter
    {
        #region Methods

        public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, Type sourceType)
        {
            if (context == null)
                return sourceType == typeof(string);
            if (sourceType == typeof(string))
                return true;
            if (context.PropertyDescriptor != null && (context.Instance is JsonObject))
            {
                if (context.PropertyDescriptor.PropertyType == typeof(JsonString) && sourceType == typeof(string))
                    return true;
            }
            else
            {
                object v = context.PropertyDescriptor.GetValue(context.Instance);
                if (v is JsonString && sourceType == typeof(string))
                    return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, Type destinationType)
        {
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string)
                return new JsonString((string)value);
            if (context.PropertyDescriptor != null)
            {
                if (context.PropertyDescriptor.PropertyType == typeof(JsonString) && value is string)
                    return new JsonString((string)value);
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (value is JsonString && destinationType == typeof(string))
                return ((JsonString)value).Value;
            else
                return base.ConvertTo(context, culture, value, destinationType);
        }

        #endregion Methods
    }
}