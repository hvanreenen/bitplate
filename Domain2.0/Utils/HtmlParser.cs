using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace BitPlate.Domain.Utils
{


    public class HtmlParser
    {

        public  HtmlDocument document;

        public static HtmlDocument Parse(string html)
        {
            HtmlParser parser = new HtmlParser();

            Thread t = new Thread(parser.TParseMain);
            t.SetApartmentState(ApartmentState.STA);
            t.Start((object)html);
            t.Join();
            return parser.document;
        }

        public void TParseMain(object html)
        {
            WebBrowser wbc = new WebBrowser();
            wbc.DocumentText = "";    
            HtmlDocument doc = wbc.Document.OpenNew(true);
            doc.Write((string)html);
            

            document = doc;
            //this.ReturnString = doc.Body.InnerHtml + " do here something";
            return;
        }
    }
}