#if CAN_DYNAMIC

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace TG.JSON
{
    internal class DynamicObjectHandler : System.Dynamic.DynamicObject
    {
        
        public DynamicObjectHandler(JsonObject obj)
        {
            Owner = obj;
        }

        public JsonObject Owner { get; set; }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return Owner.PropertyNames;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            try
            {
                Owner[binder.Name] = Owner.ValueFromObject(value);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            try
            {
                result = Owner[binder.Name];
                return true;
            }
            catch (Exception)
            {
                
            }
            result = null;
            return false;
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            return base.TryConvert(binder, out result);
        }

        public override bool TryDeleteMember(DeleteMemberBinder binder)
        {
            return base.TryDeleteMember(binder);
        }
        
    }
}
#endif