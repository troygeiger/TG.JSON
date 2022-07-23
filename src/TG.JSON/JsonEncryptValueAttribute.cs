using System;
using System.Collections.Generic;
using System.Text;

namespace TG.JSON
{
    /// <summary>
    /// This attribute instructs the serializer to encrypt the string value.
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class JsonEncryptValueAttribute : Attribute
    {
       
    }
}
