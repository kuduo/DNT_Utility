using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Codeplex.Data
{
    public class DynamicXml : DynamicObject, IEnumerable
    {
        static readonly DynamicXml Null = new DynamicXml();

        #region Friendly Xml

        static readonly Regex TagRegex = new Regex(@"<(/?)(.*?)>");
        static readonly Regex AttrRegex = new Regex(@"(\w+)=""");

        static string Friendly(string txt)
        {
            var dictionary = new Dictionary<string, string>();
            Func<string, string> formatTag = tag =>
            {
                if (!dictionary.ContainsKey(tag))
                    dictionary.Add(tag, FormatText(tag));
                return dictionary[tag];
            };

            var result = TagRegex.Replace(txt, delegate(Match match)
            {
                var original = match.Result("$2");
                string worked;

                #region tag work

                if (original.Contains(" "))
                {
                    var index = original.IndexOf(" ", StringComparison.Ordinal);
                    worked = formatTag(original.Substring(0, index)) +
                             AttrRegex.Replace(original.Substring(index),
                                               attr => formatTag(attr.Result("$1")) + "=\"");
                }
                else
                {
                    worked = formatTag(original);
                }

                #endregion

                return match.Result("<$1") + worked + ">";

            });

            return result;
        }

        static string FormatText(string txt)
        {
            var sb = new StringBuilder();
            var up = true;

            foreach (var c in txt)
            {
                if (c == '_')
                {
                    up = true;
                }
                else
                {
                    if (up)
                    {
                        sb.Append(c.ToString(CultureInfo.InvariantCulture).ToUpper());
                        up = false;
                    }
                    else
                    {
                        sb.Append(c.ToString(CultureInfo.InvariantCulture).ToLower());
                    }
                }
            }

            return sb.ToString();
        }

        #endregion

        private readonly List<XElement> _elements = new List<XElement>();

        DynamicXml() { }

        public DynamicXml(string text, bool friendly = true)
        {
            try
            {
                var doc = XDocument.Parse(text);

                if (friendly)
                {
                    var formatted = doc.ToString();
                    doc = XDocument.Parse(Friendly(formatted));
                }

                _elements = new List<XElement> { doc.Root };
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                throw;
            }
        }

        protected DynamicXml(XElement element)
        {
            _elements = new List<XElement> { element };
        }

        protected DynamicXml(IEnumerable<XElement> elements)
        {
            _elements = new List<XElement>(elements);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = Null;

            switch (binder.Name)
            {
                case "Count":
                    result = _elements.Count;
                    break;
                default:
                    {
                        var items = _elements.Descendants(XName.Get(binder.Name)).ToList();

                        if (!items.Any())
                        {
                            if (_elements.Count == 1)
                            {
                                var attr = _elements[0].Attribute(XName.Get(binder.Name));

                                if (null != attr)
                                {
                                    result = attr.Value;
                                }
                            }
                        }
                        else
                        {
                            result = new DynamicXml(items);
                        }
                    }
                    break;
            }

            return true;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            var index = (int)indexes[0];

            result = new DynamicXml(_elements[index]);

            return true;
        }

        public IEnumerator GetEnumerator()
        {
            return _elements.Select(element => new DynamicXml(element)).GetEnumerator();
        }

        public override string ToString()
        {
            if (_elements.Count == 1 && !_elements[0].HasElements)
            {
                return _elements[0].Value;
            }

            return string.Join("\n", _elements);
        }

        public static implicit operator string(DynamicXml dyn)
        {
            return Null == dyn ? null : dyn.ToString();
        }

        public static implicit operator DynamicXml(string xml)
        {
            return new DynamicXml(xml);
        }
    }
}
