using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RequiredProperties
{
    public static class AttributesCache
    {
        private static Dictionary<Type, List<string>> _attributes = new Dictionary<Type, List<string>>();

        public static List<string> GetAttributes(Type t)
        {
            List<string> attributes;
            if (_attributes.TryGetValue(t, out attributes))
            {
                return attributes;
            }

            var properties = t.GetProperties();

            var attr = properties.Select(p => new {property = p, attr = p.CustomAttributes})
                                       .Where(p => p.attr.Any(a => a.AttributeType.Name == "RequiredAttribute"))
                                       .Select(p => p.property.Name)
                                       .ToList();

            _attributes[t] = attr;

            return _attributes[t];
        } 
    }
}
