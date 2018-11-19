using BitPlate.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitPlate.Domain.Modules
{
    public static class ModuleLoader
    {
        public static BaseModule Load(string typeString)
        {
            BaseModule module;
            if (typeString == "HtmlModule" || typeString == "Html")
            {
                module = new BaseModule();
                module.Type = "HtmlModule";
                module.Content = "<p>&nbsp;</p>";
                
            }
            else
            {
                ModuleDefinition moduleDefinition = GetModuleDefinition(typeString);
                if (typeString == "GroupListModule")
                {
                    module = new Data.GroupListModule();
                }
                else if (typeString == "ItemListModule")
                {
                    module = new Data.ItemListModule();
                }
                else if (typeString == "GroupDetailsModule")
                {
                    module = new Data.GroupDetailsModule();
                }
                else if (typeString == "ItemDetailsModule")
                {
                    module = new Data.ItemDetailsModule();
                }
                else if (typeString == "DataBreadCrumbModule")
                {
                    module = new Data.DataBreadCrumbModule();
                }
                else if (typeString == "TreeViewModule")
                {
                    module = new Data.TreeViewModule();
                }
                else if (typeString == "SearchModule")
                {
                    module = new Search.SearchModule();
                }
                else if (typeString == "SearchResultsModule")
                {
                    module = new Search.SearchResultsModule();
                }
                else if (typeString == "LoginModule")
                {
                    module = new Auth.LoginModule();
                }
                else if (typeString == "MyProfileModule")
                {
                    module = new Auth.MyProfileModule();
                }
                else if (typeString == "LoginStatusModule")
                {
                    module = new Auth.LoginStatusModule();
                }
                else if (typeString == "ContactFormModule")
                {
                    module = new ContactForm.ContactFormModule();
                }
                else if (typeString == "OptInModule")
                {
                    module = new Newsletter.OptInModule();
                }
                else if (typeString == "SubscribeModule")
                {
                    module = new Newsletter.SubscribeModule();
                }
                else if (typeString == "UnsubscribeModule")
                {
                    module = new Newsletter.UnsubscribeModule();
                }
                else
                {
                    Type type = Type.GetType(moduleDefinition.ModuleClass);
                    module = (BaseModule)System.Activator.CreateInstance(type);
                    if (module == null) return null;
                }
                
                module.Type = typeString;
                module.Name = moduleDefinition.FriendlyName;
                module.ConfigPageUrl = moduleDefinition.ConfigPageUrl;
                
                string moduleXmlFile = moduleDefinition.XmlFileLocation;
                if (moduleXmlFile != null && moduleXmlFile != "")
                {
                    if (!System.IO.File.Exists(moduleXmlFile))
                    {
                        if (WebSessionHelper.CurrentSite != null)
                        {
                            moduleXmlFile = WebSessionHelper.CurrentSite.Path + "_bitplate\\EditPage\\Modules\\" + moduleXmlFile;
                        }
                    }
                    if (System.IO.File.Exists(moduleXmlFile))
                    {
                        System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(BaseModule));
                        System.IO.StreamReader reader = System.IO.File.OpenText(moduleXmlFile);
                        BaseModule moduleFromXml = (BaseModule)serializer.Deserialize(reader);
                        //overschrijven
                        if (moduleFromXml.ContentSamples != null && moduleFromXml.ContentSamples.Count > 0)
                        {
                            module.ContentSamples = moduleFromXml.ContentSamples;
                        }
                        if (moduleFromXml.IncludeScripts != null && moduleFromXml.IncludeScripts.Count > 0)
                        {
                            module.IncludeScripts = moduleFromXml.IncludeScripts;
                        }
                        if (moduleFromXml.ConfigPageUrl != null)
                        {
                            module.ConfigPageUrl = moduleFromXml.ConfigPageUrl;
                        }
                        
                        reader.Close();
                    }
                }
            }
            return module;

        }

        private static ModuleDefinition GetModuleDefinition(string type)
        {
            List<ModuleDefinition> moduleList = Utils.WebSessionHelper.AvailableModules;

            ModuleDefinition moduleDefinition = null;
            foreach (ModuleDefinition modDef in moduleList)
            {
                if (modDef.ModuleType == type)
                {
                    moduleDefinition = modDef;
                    //moduleXmlFile = String.Format("{0}\\_bitPlate\\_bitModules\\{1}", AppDomain.CurrentDomain.BaseDirectory, modDef.XmlFileLocation);
                    break;
                }
            }
            return moduleDefinition;
        }
    }

}
