using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using HJORM;
using BitPlate.Domain.Modules;
using BitSite._bitPlate.EditPage.ModuleConfig;

using System.Web.UI;
using System.ComponentModel;
using BitPlate.Domain.Utils;
using BitSite._bitPlate._bitSystem;

namespace BitSite._bitPlate.EditPage.Modules
{
    public abstract class BaseModuleUserControl : System.Web.UI.UserControl
    {
        protected Label LabelMsg;
        protected BaseModule module;
        public Guid ModuleID { get; set; }
        public string NavigationString { get; set; }
        public string SettingsString { get; set; } //extra settings
        public bool HasAutorisation { get; set; }
        public string SiteUserGroups { get; set; }
        public string SiteUsers { get; set; }
        public string BitplateUserGroups { get; set; }
        public string BitplateUsers { get; set; }

        public Dictionary<string, object> Settings { get; set; }
        public Dictionary<string, ModuleNavigationActionLite> NavigationActions { get; set; }

        //**************************DIT IS EEN CACHING TEST ******************************/
        //Deze code wordt uigevoerd voor het page_load event.
        //protected override void OnLoad(EventArgs e)
        //{
        //    /* Is het niet een gecachte pagina voer dan de page_load functie uit. */
        //    //if (!BitCaching.CachedPage)
        //    //{
        //    //    base.OnLoad(e);
        //    //}
        //}
        //***********************************EINDE***********************************/

        /// <summary>
        /// Laadt moduleobject uit database
        /// </summary>
        public void LoadModuleObject()
        {
            this.module = BaseObject.GetById<BaseModule>(this.ModuleID);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            try
            {
                base.Render(writer);
            }
            catch (Exception ex)
            {
                writer.Write("Er is een fout in deze module");
            }
        }


        protected virtual void Load(object sender, EventArgs e)
        {
            this.LabelMsg = (Label)this.FindControl("LabelMsg" + this.ModuleID.ToString("N"));
            if (this.LabelMsg == null) this.LabelMsg = new Label();
        }
        
        protected void LoadNavigationActions()
        {
            if (NavigationString != null && NavigationString != "" && NavigationActions == null)
            {
                NavigationActions = (Dictionary<string, ModuleNavigationActionLite>) JSONSerializer.Deserialize<Dictionary<string, ModuleNavigationActionLite>>(NavigationString);
            }
        }

        protected ModuleNavigationActionLite GetNavigationActionByTagName(string key)
        {
            ModuleNavigationActionLite returnValue = null;
            if (NavigationActions == null)
            {
                LoadNavigationActions();
            }
            if (NavigationActions != null && NavigationActions.ContainsKey(key))
            {
                returnValue = NavigationActions[key];
            }
            return returnValue;
        }

        protected ModuleNavigationActionLite GetFirstNavigationAction()
        {
            ModuleNavigationActionLite returnValue = null;
            if (NavigationActions == null)
            {
                LoadNavigationActions();
            }
            if (NavigationActions != null && NavigationActions.Count > 0)
            {
                returnValue = NavigationActions.First().Value;
            }
            return returnValue;
        }
        protected void LoadSettings()
        {

            if (SettingsString != null && SettingsString != "" && Settings == null)
            {
                Settings = (Dictionary<string, object>)JSONSerializer.Deserialize<Dictionary<string, object>>(SettingsString);
            }
            else
            {
                if (this.Settings == null)
                {
                    Settings = new Dictionary<string, object>();
                }
            }
        }

        internal T getSetting<T>(string key)
        {
            T returnValue = default(T);
            if (Settings == null)
            {
                LoadSettings();
            }
            if (Settings != null && Settings.ContainsKey(key))
            {
                object value = Settings[key];
                if (value != null && value.ToString() != "")
                {
                    if (typeof(T).IsEnum)
                    {
                        //returnValue = T.TryParse( (T)Settings[key]);
                        returnValue = (T)Convert.ChangeType(value, Enum.GetUnderlyingType(typeof(T)));
                    }
                    else if (typeof(T) == typeof(Guid))
                    {
                        returnValue = (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(value.ToString());

                        //returnValue = new Guid(value.ToString()); // (T)new Guid(value.ToString()); // Guid.TryParse(value.ToString(), out returnValue);
                    }
                    else
                    {
                        returnValue = (T)Convert.ChangeType(value, typeof(T)); //T.TryParse( (T)Settings[key];
                    }
                }
            }
            return returnValue;
        }

        /// <summary>
        /// Finds a Control recursively. Note finds the first match and exists
        /// </summary>
        /// <param name="ContainerCtl"></param>
        /// <param name="IdToFind"></param>
        /// <returns></returns>
        public Control FindControlRecursive(Control root, string id)
        {
            if (root.ID == id)
                return root;

            foreach (Control Ctl in root.Controls)
            {
                Control FoundCtl = FindControlRecursive(Ctl, id);
                if (FoundCtl != null)
                    return FoundCtl;
            }

            return null;
        }

        private List<Control> _controls;

        public List<Control> GetAllControls(Control root)
        {
            this._controls = new List<Control>();
            FindAllControls(root);
            return this._controls;
        }

        private void FindAllControls(Control root)
        {
            foreach (Control Ctl in root.Controls)
            {
                FindAllControls(Ctl);
                this._controls.Add(Ctl);
            }
        }

        

        

        protected bool CheckAutorisation()
        {
             bool isAutorized = true;
            if (HasAutorisation)
            {
                isAutorized = false;
                bool editMode = false;
                if (Request.QueryString["mode"] != null && Request.QueryString["mode"].ToLower() == "edit")
                {
                    editMode = true;
                }
                if (editMode)
                {
                    if (SessionObject.CurrentBitplateUser != null)
                    {
                        isAutorized = SessionObject.CurrentBitplateUser.IsAutorized(BitplateUserGroups, BitplateUsers);
                    }
                }
                else 
                {
                    if (SessionObject.CurrentBitSiteUser != null)
                    {
                        isAutorized = SessionObject.CurrentBitSiteUser.IsAutorized(SiteUserGroups, SiteUsers);
                    }
                }
                //Developers mogen altijd alles zien
                if (SessionObject.CurrentBitplateUser != null && SessionObject.CurrentBitplateUser.GetSuperUserType() == BitPlate.Domain.Autorisation.SuperUserTypeEnum.Developers)
                {
                    isAutorized = true;
                }
                this.Visible = isAutorized;
                //this.IsUserAutorized = isAutorized;
            }
            return isAutorized;
        }

        /// <summary>
        /// Module wordt vanuit andere module aangeroepen met ajax call (MS-updatepanels)
        /// Bij NavigateAction van type ShowDetailsInModules
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public virtual void Reload(BaseModuleUserControl sender, NavigationParameterObject args = null)
        {
            this.CheckAutorisation();
        }

        

        internal string GetLanguageCodeFromMasterPage()
        {
            DefaultMaster masterPage = (DefaultMaster)this.Page.Master;
            if (masterPage != null)
            {
                return masterPage.LanguageCode;
            }
            else return "";
        }
    }
}