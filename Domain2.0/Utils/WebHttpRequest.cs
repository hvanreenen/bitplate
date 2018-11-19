using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace BitPlate.Domain.Utils
{
    public class WebHttpRequest
    {
        public enum HttpMethod {
            GET,
            POST
        }

        public string URL { get; set; }
        private string _contentType = "application/x-www-form-urlencoded";
        public string ContentType { 
            get{
            return this._contentType;
            } 
            set {
                this._contentType = value;
            }
        }
        private HttpMethod _method = HttpMethod.POST;
        public HttpMethod Method
        {
            get
            {
                return this._method;
            }
            set
            {
                this._method = value;
            }
        }
        public byte[] Data { get; set; }
        public string proxy { get; set; }

        public WebHttpRequest(string URL, byte[] Data = null, string Proxy = null)
        {
            this.URL = URL;
            this.Data = Data;
            this.proxy = Proxy;
        }

        public void SetDataObject(Object obj)
        {
            string JsonObj = JSONSerializer.Serialize(obj);
            this.Data = UTF8.Encode(JsonObj);
        }

        public string GetResponse()
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(this.URL);
            req.Method = this._method.ToString();
            req.ContentType = this._contentType;
            req.ContentLength = this.Data.Length;
            req.Proxy = new WebProxy(proxy, true);
            req.CookieContainer = new CookieContainer(); 

            Stream reqst = req.GetRequestStream(); 
            reqst.Write(this.Data, 0, this.Data.Length);
            reqst.Flush();
            reqst.Close();

            HttpWebResponse res = (HttpWebResponse)req.GetResponse();

            Stream resst = res.GetResponseStream();
            StreamReader sr = new StreamReader(resst);
            return sr.ReadToEnd();
        }
    }
}
