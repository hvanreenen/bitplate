using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HJORM.Attributes;
namespace BitPlate.Domain.Modules
{
    
    [Persistent("Module")]
    public class CustomModule : BaseDataModule
    {

        public CustomModule(): base()
        {
        }
        public CustomModule(string type): this()
        {
            if(!type.EndsWith("Module")){
                type += "Module";
            }
            Type = type;
            //onderstaande waardes uit externe file halen op basis van type
            LoadPropsFromXmlFile(type);
            //Name = type;
//            this.Tags.Add(new Tag("{textName}", "<input type='text' id='textName'/>"));
//            this.Tags.Add(new Tag("{submitButton}", "<button onclick='javascript:HELLOWORLDMODULES.submit();'>", true, "</button>"));
//            ContentSamples.Add(@"voer hier je naam in: {textName}<br/>
//{submitButton}");
//            this.IncludeScripts.Add("HELLOWORLDMODULES.js");
//            this.IncludeScripts.Add("testmap\\testScript.js");
//            this.TabPages.Add("tabPage1");
//            this.TabPages.Add("tabPage2");

            //TagReplaces.Add("{textName}", "<input type='text' id='textName'/>");

        }

        public void LoadPropsFromXmlFile(string type)
        {
            //string modulesXmlFile = String.Format("{0}\\_bitPlate\\_bitModules\\AllModules.xml", AppDomain.CurrentDomain.BaseDirectory, type);

            //List<ModuleDefinition> moduleList = new List<ModuleDefinition>();
            ////ModuleDefinition def = new ModuleDefinition();
            ////def.ModuleType = "SearchModule";
            ////def.XmlFileLocation = "SearchModules\\SearchModule";
            ////def.MenuFolder = "Zoek";
            ////def.MenuName = "Zoek";
            ////moduleList.Add(def);

            //System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(moduleList.GetType());
            ////System.IO.StreamWriter wr = System.IO.File.CreateText(modulesXmlFile);
            ////x.Serialize(wr, moduleList);
            
            //System.IO.StreamReader reader = System.IO.File.OpenText(modulesXmlFile);
            //moduleList = (List<ModuleDefinition>)x.Deserialize(reader);
            //reader.Close();
            List<ModuleDefinition> moduleList = Utils.WebSessionHelper.AvailableModules;
            string moduleXmlFile = "";
            foreach (ModuleDefinition modDef in moduleList)
            {
                if (modDef.ModuleType == type)
                {
                    moduleXmlFile = String.Format("{0}\\_bitPlate\\_bitModules\\{1}", AppDomain.CurrentDomain.BaseDirectory, modDef.XmlFileLocation);
                    break;
                }
            }
            //string moduleXmlFile = String.Format("{0}\\_bitPlate\\_bitModules\\{1}\\{1}.xml", AppDomain.CurrentDomain.BaseDirectory, type);
            if (System.IO.File.Exists(moduleXmlFile))
            {
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(this.GetType());
                System.IO.StreamReader reader = System.IO.File.OpenText(moduleXmlFile);
                CustomModule temp = (CustomModule)serializer.Deserialize(reader);
                this.Name = temp.Name;
                this.Tags = temp.Tags;
                this.ContentSamples = temp.ContentSamples;
                this.IncludeScripts = temp.IncludeScripts;
                this.UseInNewsletter = temp.UseInNewsletter;
                this.ConfigPageUrl = temp.ConfigPageUrl;
                this.ConfigPageStandardElements = temp.ConfigPageStandardElements;
                reader.Close();
            }
        }

        public override string ToString(ModeEnum mode)
        {
            string moduleStartHTML = getModuleStartHTML(mode);
            //string moduleStartHTML = "<div>";
            string moduleContentHTML = Content;
            string moduleEndHTML = " </div>";

            foreach (Tag tag in this.Tags)
            {
                string replaceValue = tag.ReplaceValue;
                if(replaceValue.Contains("{moduleid}")){
                    replaceValue = replaceValue.Replace("{moduleid}", "{0}");
                }
                if(replaceValue.Contains("{0}")){
                    //vervang door module id
                    moduleContentHTML = moduleContentHTML.Replace(tag.Name, String.Format(tag.ReplaceValue, this.ID));
                }
                else{
                    moduleContentHTML = moduleContentHTML.Replace(tag.Name, tag.ReplaceValue);
                }
                if (tag.HasCloseTag)
                {
                    moduleContentHTML = moduleContentHTML.Replace(tag.CloseTag, tag.ReplaceValueCloseTag);
                }
            
            }
            //2do onderstaande replaces uit externe file halen op basis van type
            //moduleContentHTML = moduleContentHTML.Replace("{textName}", "<input type='text' id='textName'/>");
            //moduleContentHTML = moduleContentHTML.Replace("{submitButton}", "<button onclick='javascript:HELLOWORLDMODULES.submit();'>OK</button>");

            return moduleStartHTML + moduleContentHTML + moduleEndHTML;

        }
    }
}
