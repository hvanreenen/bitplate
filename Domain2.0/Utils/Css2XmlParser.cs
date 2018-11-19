using System;
using System.Collections.Generic;
using System.Text;

namespace BitPlate.Domain.Utils
{
    class Css2XmlParser
    {
        public static string ParseCss2Xml(string css)
        {
            css = RemoveRemarks(css);
            string xml = "";
            xml += "<Styles>\r\n";
            string[] styles = css.Split(new char[] { '}' });

            foreach (string style in styles)
            {
                xml += ParseStyle(style);
            }

            xml += "</Styles>\r\n";
            return xml;
        }

        private static string RemoveRemarks(string css)
        {
            string newcss = css;
            int posStart = css.IndexOf("/*");
            int posEnd = 0;
            if (posStart > 0)
            {
                 posEnd = css.IndexOf("*/", posStart);
            }
            while (posStart > 0)
            {
                string newcss1 = newcss.Substring(0, posStart);
                string newcss2 = newcss.Substring(posEnd + 2, newcss.Length - posEnd -2);
                newcss = newcss1 + newcss2;
                posStart = newcss.IndexOf("/*");
                if (posStart > 0)
                {
                    posEnd = newcss.IndexOf("*/", posStart);
                }
            }
            return newcss;

        }

        private static string ParseStyle(string style)
        {
            string xml = "";
            if (style.Trim() != "")
            {
                //int posStyleNameStart = 0;
                int posStyleNameEnd = style.IndexOf("{");
                if (posStyleNameEnd <= 0)
                {
                    return xml;
                }
                string styleName = style.Substring(0, posStyleNameEnd).Trim();

                style = style.Replace(styleName, "");
                style = style.Replace("{", "");
                style = style.Replace("}", "");
                string[] attributes = style.Split(new char[] { ';' });

                //style kan meerdere namen hebben --> gescheiden door ,
                string[] styleNames = styleName.Split(new char[] { ',' });
                foreach (string s in styleNames)
                {
                    string elementName = "div";
                    if (styleName.Contains("#")) //id's doen we niet
                    {
                        return xml;
                    }
                    if (styleName.Contains(":")) //classes als :hover en :link doen we niet
                    { 
                        return xml; 
                    }
                    if (!styleName.Contains(".")) //style is geen class maar een element
                    {
                        return xml; 
                        //styleName = s;
                        //elementName = styleName;
                    }
                    else
                    {
                        //haal element uit class --> deel na de spatie
                        int posSpace = s.IndexOf(" ");
                        if (posSpace > 0)
                        {
                            styleName = s.Substring(0, posSpace).Replace(".", "").Trim();
                            elementName = s.Substring(posSpace, s.Length - posSpace).Trim();
                        }
                        else if (s.Split(new char[] { '.' }).Length > 1)
                        {
                            //of deel voor de punt ??
                            elementName = s.Split(new char[] { '.' })[0].Trim();
                            styleName = s.Split(new char[] { '.' })[1].Replace(".", "").Trim();
                        }  
                    }
                    if (elementName == "") { elementName = "div";  }
                    if (elementName == "body") { return xml; } //body doen we niet

                    


                    xml += string.Format("<Style name=\"{0}\" element=\"{1}\" >\r\n", styleName, elementName);
                    xml += string.Format("<Attribute name=\"class\" value=\"{0}\" />\r\n", styleName);

                    

                    //foreach (string attribute in attributes)
                    //{
                    //    xml += ParseAttribute(attribute);
                    //}
                    xml += "</Style>\r\n";
                }
            }
            return xml;

        }

        private static object ParseAttribute(string attribute)
        {
            string xml = "";
            if (attribute.Trim() != "")
            {
                string[] namevalue = attribute.Split(new char[] { ':' });

                string name = namevalue[0].Trim();
                string value = namevalue[1].Trim();

                xml = string.Format("\t<Style name=\"{0}\" value=\"{1}\" />\r\n", name, value);
            }
            return xml;
        }
    }
}
