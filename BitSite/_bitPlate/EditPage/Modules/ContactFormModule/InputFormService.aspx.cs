using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using BitPlate.Domain;
using BitPlate.Domain.Modules;
using BitPlate.Domain.Utils;
using BitPlate.Domain.DataCollections;
using System.Text.RegularExpressions;

namespace BitSite.EditPage.Modules
{
    public partial class InputFormService : System.Web.UI.Page
    {
        private static BaseModule module;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.Form.AllKeys.Any())
            {
                Dictionary<string, string> Values = new Dictionary<string, string>();
                foreach (string FormValueKey in Request.Form.AllKeys)
                {
                    Values.Add(FormValueKey, Request[FormValueKey]);
                }
                Dictionary<string, object> ReturnValue = HandlePost(Values);
                if (ReturnValue.Count > 0)
                {
                    ReturnValue.Add("success", false);
                }
                else
                {
                    //if (module.NavigationType == NavigationTypeEnum.NavigateToPage)
                    //{
                    //    ReturnValue.Add("DrillDownUrl", module.DrillDownPage.RelativeUrl);
                    //}
                    ReturnValue.Add("success", true);
                }
                string JsonReturnValue = JSONSerializer.Serialize(ReturnValue);
                Response.Write(JsonReturnValue);
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static Dictionary<string, object> SubmitAjaxForm(string obj)
        { 
            Dictionary<string, string> Values = new Dictionary<string, string>();
            string[] controls = obj.Split('&');
            foreach (string control in controls) {
                string[] tmp = control.Split('=');
                Values.Add(tmp[0], HttpUtility.HtmlDecode(tmp[1]));
            }

            Dictionary<string, object> ReturnValue = HandlePost(Values);
            if (ReturnValue.Count > 0)
            {
                ReturnValue.Add("success", false);
            }
            else
            {
                //if (module.NavigationType == NavigationTypeEnum.NavigateToPage){
                //    ReturnValue.Add("DrillDownUrl", module.DrillDownPage.RelativeUrl);
                //}
                
                ReturnValue.Add("success", true);
            }
            
            return ReturnValue;
        }

        private static Dictionary<string, object> HandlePost(Dictionary<string, string> Values)
        {
            module = BaseModule.GetById<BaseModule>(new Guid(Values["bitFormId"]));
            Dictionary<string, object> Settings = JSONSerializer.Deserialize<Dictionary<string, object>>(module.SettingsJsonString);
            Dictionary<string, object> ValidationFieldsObject = (Dictionary<string, object>)Settings["validation"];
            List<DataInputValidation> Validations = new List<DataInputValidation>();
            foreach (KeyValuePair<string, object> ValidationFieldObject in ValidationFieldsObject)
            {
                Dictionary<string, object> tmp = (Dictionary<string, object>)ValidationFieldObject.Value;
                DataInputValidation div = new DataInputValidation();
                div.Required = (bool)tmp["Required"];
                div.DataType = (DataTypeEnum)Enum.Parse(typeof(DataTypeEnum), (string)tmp["DataType"]);
                div.Name = (string)tmp["Name"];
                div.ErrorMessage = (string)tmp["ErrorMessage"];
                Validations.Add(div);
            }
            Dictionary<string, object> returnValue = new Dictionary<string, object>();
            foreach (DataInputValidation Validation in Validations)
            {
                KeyValuePair<string, string> ValueToValidate = Values.Where(c => c.Key == Validation.Name).FirstOrDefault();
                if (Validation.ErrorMessage == "") {
                    Validation.ErrorMessage = "Vul hier uw " + Validation.Name + " in.";
                }
                if (!ValueToValidate.Equals(null))
                {
                    if (ValueToValidate.Value != "")
                    {
                        switch (Validation.DataType)
                        {
                            case DataTypeEnum.Number:
                                try
                                {
                                    double.Parse(ValueToValidate.Value);
                                }
                                catch (Exception ex)
                                {
                                    returnValue.Add(Validation.Name, Validation.ErrorMessage);
                                }
                                break;

                            case DataTypeEnum.Email:
                                if (!Regex.Match(ValueToValidate.Value, @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$").Success) {
                                    returnValue.Add(Validation.Name, Validation.ErrorMessage);
                                }
                                break;
                        }
                    }
                    else
                    {
                        if (Validation.Required)
                        {
                            returnValue.Add(Validation.Name, Validation.ErrorMessage);
                        }
                    }
                }
                else
                {
                    if (Validation.Required) {
                        returnValue.Add(Validation.Name, Validation.ErrorMessage);
                    }
                    
                }
            }
            if (!(returnValue.Count > 0))
            {
                //TODO Settings amken
                bool SendEmail = (bool)module.Settings["isEmailEnabled"]; ;
                if (SendEmail)
                {
                    string Template = module.Settings["EmailTemplate"].ToString().Trim();
                    string emailValues = "";
                    foreach (KeyValuePair<string, string> keyval in Values)
                    {
                        emailValues += keyval.Key + " = " + keyval.Value + "<br />";
                        Template = Template.Replace("{" + keyval.Key + "}", keyval.Value);
                    }

                    Template = Template.Replace("{values}", emailValues);
                    EmailManager.SendMail(module.Settings["EmailFrom"].ToString(), module.Settings["EmailTo"].ToString(), module.Settings["EmailSubject"].ToString(), Template, true);
                    //send mail
                }

                if ((bool)module.Settings["isDataCollectieEnabled"])
                {
                   // DataCollection dc = module.DataCollection;
                   // DataItem di = new DataItem();
                   // di.DataCollection = dc;
                   //// DataGroup dg = DataGroup.GetById<DataGroup>(Guid.Parse(module.SelectGroup));
                   // //di.ParentGroup = dg;

                   // foreach (DataField dfl in dc.DataItemFields)
                   // {
                   //     if (Values.ContainsKey(dfl.Name))
                   //     {

                   //         if (dfl.MappingColumn != "Name")
                   //         {
                   //             typeof(DataItem).GetProperty(dfl.MappingColumn).SetValue(di, Values[dfl.Name], null);
                   //         }                           
                   //     }
                   // }
                   // di.Name = DateTime.Now.ToString();
                   // di.Save();
                }
                //if (module.DrillDownUrl != "")
                //{
                //    returnValue.Add("DrillDownUrl", module.DrillDownUrl);
                //}
            }
            
            return returnValue;
        }
    }

    public enum DataTypeEnum
    {
        Text,
        Number,
        Email
    }

    public class DataInputValidation
    {
        public string Name { get; set; }
        public bool Required { get; set; }
        public DataTypeEnum DataType { get; set; }
        public string ErrorMessage { get; set; }
    }
}