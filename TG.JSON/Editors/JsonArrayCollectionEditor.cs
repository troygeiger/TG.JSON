using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace TG.JSON.Editors
{
    internal class JsonArrayCollectionEditor : System.ComponentModel.Design.CollectionEditor
    {
        
        public JsonArrayCollectionEditor(Type type): base(type)
        { }

        protected override Type[] CreateNewItemTypes()
        {
            var assem = Assembly.GetExecutingAssembly();
            var jsonVal = typeof(JsonValue);
            List<Type> results = new List<Type>();
            foreach (Type t in assem.GetTypes())
                if (!t.IsAbstract && t.IsSubclassOf(jsonVal))
                    results.Add(t);
            return results.ToArray();//base.CreateNewItemTypes();
        }

        protected override string GetDisplayText(object value)
        {
            return base.GetDisplayText(value.GetType().Name);
        }

        protected override object CreateInstance(Type itemType)
        {
            //if (itemType == typeof(JsonObject))
            //{
            //    object o = base.CreateInstance(itemType);
            //    (o as JsonObject).ShowProperties = true;
            //    return o;
            //}
            //else
                return base.CreateInstance(itemType);
        }

    }
}
