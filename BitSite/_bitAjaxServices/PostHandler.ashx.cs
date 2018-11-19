using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.SessionState;

namespace BitSite._bitAjaxServices
{
    /// <summary>
    /// Summary description for PostHandler
    /// </summary>
    public class PostHandler : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            string service = (context.Request.QueryString.AllKeys.Contains("service")) ? context.Request.QueryString["service"] : null;
            string method = (context.Request.QueryString.AllKeys.Contains("method")) ? context.Request.QueryString["method"] : null;

            if (service != null && service != "" && method != null && method != "")
            {
                Type type = Type.GetType(service);
                object serviceObject = Activator.CreateInstance(type);
                MethodInfo methodInfo = type.GetMethod(method);
                if (methodInfo != null && serviceObject != null)
                {
                    methodInfo.Invoke(serviceObject, new object[] { context } );
                }
                else
                {
                    throw new Exception("Unable to invoke method " + method + " in " + service);
                }
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}