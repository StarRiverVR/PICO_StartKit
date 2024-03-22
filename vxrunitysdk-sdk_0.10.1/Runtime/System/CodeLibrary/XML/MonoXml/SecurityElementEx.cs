using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;

namespace com.vivo.codelibrary
{
    public static class SecurityElementEx
    {
        public static bool TryGetAttributeBool(this SecurityElement e, string key,ref bool result)
        {
            if (e == null)
            {
                return false;
            }
            var str = e.Attribute(key);
            if (str != null)
            {
                return bool.TryParse(str, out result);
            }
            return false;
        }
        public static bool TryGetAttribute(this SecurityElement e, string key, ref string result)
        {
            if (e == null)
            {
                return false;
            }
            var str = e.Attribute(key);
            if (str != null)
            {
                result = str;
                return true;
            }
            return false;
        }
        public static bool TryGetAttributeInt(this SecurityElement e, string key, ref int result)
        {
            if (e == null)
            {
                return false;
            }
            var str = e.Attribute(key);
            if (str != null)
            {
                return int.TryParse(str, out result);
            }
            return false;
        }
    }
}
