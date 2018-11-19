using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using HJORM;


namespace BitPlate.Domain.Utils
{

    public static class JSONSerializer
    {
        public static string Serialize<T>(BaseCollection<T> list, string[] thisFieldsOnly) where T : BaseObject, new()
        {

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new JavaScriptConverter[] { new ConverterWithFieldsSelection<T>(thisFieldsOnly) });
            string Json = serializer.Serialize(list);
            return Json;
        }

        public static string Serialize<T>(T obj, string[] thisFieldsOnly) where T : BaseObject, new()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new JavaScriptConverter[] { new ConverterWithFieldsSelection<T>(thisFieldsOnly) });
            string Json = serializer.Serialize(obj);
            return Json;
        }

        public static string Serialize(Object obj, bool RemoveNullValues = false)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer(new SimpleTypeResolver());

            string Json = serializer.Serialize(obj);
            if (RemoveNullValues)
            {
                Json = Regex.Replace(Json, "[\"][a-zA-Z0-9_]*[\"]:null[ ]*[,]?", "");
                //Json = Regex.Replace(Json, "[\"]*[][a-zA-Z0-9_]*[\"]*[]:[]*[ ]?null[]*[,]?", "");
                Json = Regex.Replace(Json, ",}", "}", RegexOptions.Singleline);
            }
            return Json;
        }


        public class ManualResolver : SimpleTypeResolver
        {
            public ManualResolver() { }
            public override Type ResolveType(string id)
            {
                return System.Web.Compilation.BuildManager.GetType(id, false);
            }
        }

        public static Object DeserializeToASPType(string Json)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer(new SimpleTypeResolver());
            string typeName = Regex.Match(Json, @"__type"":""(.*?)"",").ToString().Replace(@"__type"":""", "").Replace("\",", "");
            //Json = Regex.Replace(Json, @"""__type"":""(.*?)"",", "", RegexOptions.Singleline);
            Type objectType = Type.GetType(typeName);
            return serializer.Deserialize(Json, objectType);
        }

        public static Object Deserialize(string Json, bool ignoreASPType = false)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            if (ignoreASPType)
            {
                Json = Regex.Replace(Json, @"""__type"":""(.*?)"",", "", RegexOptions.Singleline);
            }
            return serializer.DeserializeObject(Json);
        }

        public static T Deserialize<T>(string Json, bool ignoreASPType = false)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            if (Json == null) Json = "";
            if (ignoreASPType)
            {
                Json = Regex.Replace(Json, @"""__type"":""(.*?)"",", "", RegexOptions.Singleline);
            }
            return serializer.Deserialize<T>(Json);
        }

        public static string ToJsonString(this object DataObject)
        {
            string test = JSONSerializer.Serialize(DataObject);
            return JSONSerializer.Serialize(DataObject);
        }
    }

    public class JsonResult
    {
        public bool Success { get; set; }
        public Exception Exception { get; set; }
        public string Message { get; set; }
        public object DataObject { get; set; }

        public static JsonResult CreateResult(bool Success)
        {
            return CreateResult(Success, null);
        }

        public static JsonResult CreateResult(bool Success, object DataObject)
        {
            return CreateResult(Success, null, null, DataObject);
        }

        public static JsonResult CreateResult(bool Success, string Message)
        {
            return CreateResult(Success, Message, null, null);
        }

        public static JsonResult CreateResult(bool Success, string Message, object DataObject)
        {
            return CreateResult(Success, Message, null, DataObject);
        }

        public static JsonResult CreateResult(bool Success, string Message, Exception ex, object DataObject)
        {
            JsonResult JSONResult = new JsonResult();
            JSONResult.Success = Success;
            JSONResult.Exception = ex;
            JSONResult.Message = Message;
            JSONResult.DataObject = DataObject;
            return JSONResult;
        }
    }

    /// <summary>
    /// Met deze javascript converter maak je json
    /// Je geeft een lijst mee van alleen die velden die je wilt hebben geserialiseerd
    /// Gebruik: 
    ///          var converter = new ConverterWithFieldsSelection<DataItem>(new string[] {"field1", "field2"}) });
    ///        
    /// </summary>
    /// <example>
    /// JavaScriptSerializer serializer = new JavaScriptSerializer();
    /// serializer.RegisterConverters(new JavaScriptConverter[] { new ConverterWithFieldsSelection<DataItem>(new string[] {"field1", "field2"}) });
    /// string Json = serializer.Serialize(list);
    /// </example>
    /// <typeparam name="T"></typeparam>
    public class ConverterWithFieldsSelection<T> : JavaScriptConverter where T : BaseObject, new()
    {
        string[] fields;
        public ConverterWithFieldsSelection(string[] fields)
        {
            this.fields = fields;
        }

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            throw new ApplicationException("Serializable only");
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            // create a variable we can push the serailized results to
            Dictionary<string, object> result = new Dictionary<string, object>();

            // grab the instance of the object
            T t = (T)obj;
            if (t != null)
            {
                foreach (string field in this.fields)
                {
                    // only serailize the properties we want
                    if (!result.ContainsKey("field"))
                    {
                        result.Add(field, t.GetType().GetProperty(field).GetValue(t, null));
                    }
                }
            }

            // return those results
            return result;
        }

        public override IEnumerable<Type> SupportedTypes
        {
            // let the serializer know we can accept your "MyObject" type.
            get
            {
                return new Type[] { 
                    typeof(T)
                    //, typeof(BaseCollection<T>) 
                };

            }
        }
    }
}
