using System;
using System.Collections;
using Microsoft.SPOT;

namespace NetDuinoPlusHelloWorld.Utils
{
    public static class StringUtils
    {
        public static String Replace(this string item, string search, string replacement)
        {
            var t = item.Split(search.ToCharArray());

            var s = "";
            foreach (var i in t)
            {
                if (i != String.Empty)
                {
                    s += replacement + i;
                }
            }
            return s;
        }
        
        public static String FormatAsCommaList(this object[] items)
        {
            if(items == null)
            {
                return "";
            }
            var l = items.Length;
            string s = "";
            int count = 0;
            foreach( var item in items)
            {
                s += item;
                if(count < items.Length - 1)
                {
                    s += ", ";
                }
                count++;
            }
            return s;
        }
    }
}
