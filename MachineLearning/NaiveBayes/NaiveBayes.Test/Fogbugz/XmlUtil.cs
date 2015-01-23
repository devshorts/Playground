using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace NaiveBayes.Test.Fogbugz
{

    public static class FxExtensions
    {
        public static dynamic AsFluent(this XElement element)
        {
            return new FxElement(element);
        }

        public static dynamic AsFluent(this XDocument document)
        {
            return new FxDocument(document);
        }
    }

    public class FxDocument
    {
        private readonly XDocument _document;
        public FxDocument(XDocument document)
        {
            _document = document;
        }

        public FxElement Root
        {
            get
            {
                return new FxElement(_document.Root);
            }
        }
    }

    public class FxElements : DynamicObject
    {
        public static readonly FxElements Null = new FxElements(null);

        public readonly XElement[] Elements;
        public FxElements(XElement[] elements)
        {
            Elements = elements;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (Elements == null || !Elements.Any())
            {
                result = this;
                return true;
            }

            var elements = Elements.SelectMany(e => e.Elements(binder.Name)).ToArray();
            result = new FxElements(elements);

            return true;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            if (indexes.Count() != 1)
            {
                result = null;
                return false;
            }

            if (indexes[0] is int)
            {
                var elementIdx = (int)indexes[0];
                if (Elements.Count() > elementIdx)
                {
                    result = new FxElement(Elements[elementIdx]);
                    return true;
                }

                result = new FxElement(null);
                return true;
            }

            if (Elements == null || !Elements.Any())
            {
                result = new string[0];
                return true;
            }

            var attributeName = indexes[0] as string;

            var attributes = from element in Elements
                             let fxElement = new FxElement(element)
                             let value = fxElement.GetAttribute(attributeName)
                             where value != null
                             select value;

            result = attributes.ToArray();
            return true;
        }


        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            if (Elements == null || !Elements.Any())
            {
                result = new string[0];
                return true;
            }

            if (args.Count() > 0)
            {
                var predicate = args[0] as Func<dynamic, bool>;
                if (predicate != null)
                {
                    result = Elements.SelectMany(e => e.Elements(binder.Name))
                        .Select(child => new FxElement(child))
                        .Where(predicate)
                        .ToArray();
                    return true;
                }
                result = null;
                return false;
            }

            var values = from element in Elements.Select(e => new FxElement(e))
                         let value = element.GetValue(binder.Name)
                         where value != null
                         select value;


            result = values.ToArray();
            return true;
        }
    }

    public class FxElement : DynamicObject
    {
        public static readonly FxElement Null = new FxElement(null);

        public readonly XElement Elements;
        public FxElement(XElement element)
        {
            Elements = element;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = GetChildren(binder.Name);
            return true;
        }

        public object GetChildren(string name)
        {
            if (Elements == null)
            {
                return Null;
            }

            XElement[] elements = Elements.Elements(name).ToArray();

            return (elements == null || !elements.Any())
                       ? FxElements.Null
                       : new FxElements(elements);
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            if (indexes.Count() != 1)
            {
                result = null;
                return false;
            }

            var attributeName = indexes[0] as String;
            result = GetAttribute(attributeName);

            return true;
        }

        public string GetAttribute(string name)
        {
            string result;
            if (Elements == null)
            {
                result = null;
            }
            else
            {
                XAttribute attribute = Elements.Attribute(name);
                result = attribute != null ? attribute.Value : null;
            }
            return result;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            if (Elements == null)
            {
                result = Null;
                return true;
            }

            if (args.Count() == 0)
            {
                result = GetValue(binder.Name);
            }
            else
            {
                result = GetChildren(binder.Name);
            }

            return true;
        }

        public string GetValue(string name)
        {
            if (Elements != null)
            {
                XElement child = Elements.Element(name);
                if (child != null)
                {
                    return child.Value;
                }
            }

            return null;
        }
    }
}
