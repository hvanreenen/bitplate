using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Web.Services;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using System.IO;

using HJORM;
using BitPlate.Domain;
using BitPlate.Domain.Menu;
using BitPlate.Domain.Utils;
using BitSite._bitPlate._bitSystem;


namespace BitSite._bitPlate.Menus
{
    [System.Web.Script.Services.ScriptService]
    public partial class MenuService : BaseService
    {
        static HttpPostedFile fileToImport = null;
        protected void Page_Load(object sender, EventArgs e)
        {
           
        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public BaseCollection<CmsMenu> GetMenus(string sort)
        {
            BaseService.CheckLoginAndLicense();
            string where = String.Format("FK_Site='{0}'", SessionObject.CurrentSite.ID);
            
            //List<TreeGridItem> returnList = new List<TreeGridItem>();
            BaseCollection<CmsMenu> menus = BaseCollection<CmsMenu>.Get(where, sort);
            return menus;

            //foreach (CmsMenu menu in menus)
            //{
            //    TreeGridItem item = TreeGridItem.NewItem<CmsMenu>(menu);
            //    item.Icon = ""; //TODO laten afhangen van type
            //    item.Type = menu.TypeString;
            //    item.Status = menu.ChangeStatusString;
            //    item.LanguageCode = menu.LanguageCode;
            //    item.HasAutorisation = menu.HasAutorisation;
            //    if (searchString != null && searchString != "")
            //    {
            //        item.Name = item.Name.Replace(searchString, "<span class='highlight'>" + searchString + "</span>");
            //        //item.Title = item.Title.Replace(searchString, "<span class='highlight'>" + searchString + "</span>");
            //    }
            //    returnList.Add(item);
            //}
            //return returnList;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public CmsMenu GetMenu(string id)
        {
            BaseService.CheckLoginAndLicense();
            CmsMenu menu = null;
            if (id == null)
            {
                menu = new CmsMenu();
                menu.Site = SessionObject.CurrentSite;
                //standaard js en css voor menu toevoegen
                //wanneer gebruiker kiest voor custom menu kunnen deze weer worden weggegooid. 
                //Wat nou als iemand deze scripts verwijderd en deze niet meer gevonden kunnen worden?
                //Dat geeft het resultaat van 
                CmsScript jsScript = BaseObject.GetFirst<CmsScript>("Name='siteMenu' AND ScriptType=1");
                CmsScript cssScript = BaseObject.GetFirst<CmsScript>("Name='siteMenu' AND ScriptType=0");
                if (jsScript != null)
                {
                    menu.Scripts.Add(jsScript);
                }

                if (cssScript != null)
                {
                    menu.Scripts.Add(cssScript);
                }
            }
            else
            {
                menu = BaseObject.GetById<CmsMenu>(new Guid(id));
                //if (menu.HasAutorisation)
                //{

                //    if (!menu.IsAutorized(SessionObject.CurrentBitplateUser))
                //    {
                //        throw new Exception("U heeft geen rechten voor deze datacollectie.");
                //    }
                //}
            }
            return menu;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public CmsMenu SaveMenu(CmsMenu obj)
        {
            BaseService.CheckLoginAndLicense();
            obj.Site = SessionObject.CurrentSite;
            obj.Save();
            return obj;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void DeleteMenu(string id)
        {
            BaseService.CheckLoginAndLicense();
            CmsMenu menu = BaseObject.GetById<CmsMenu>(new Guid(id));
            //if (menu.HasAutorisation)
            //{
            //    if (!menu.IsAutorized(SessionObject.CurrentBitplateUser))
            //    {
            //        throw new Exception("U heeft geen rechten voor deze datacollectie.");
            //    }
            //}
            menu.Delete();
        }

        
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public BaseCollection<CmsMenuItem> GetMenuItems(string menuId, string parentId, string parentPath, string sort)
        {
            BaseService.CheckLoginAndLicense();
            if ((parentId == null || parentId == "") &&
                (parentPath != null && parentPath != ""))
            {
                //haal folder id op vanuit path
                //path wordt gebruikt als er vanuit de breadcrumb wordt genavigeerd
                string where = String.Format("CompletePath = '{0}'", parentPath);
                CmsMenuItem item = BaseObject.GetFirst<CmsMenuItem>(where);
                parentId = item.ID.ToString();
            }

            //if (sort == "" || sort == null)
            //{
            //    sort = "OrderingNumber";
            //}

            string whereItems = String.Format("FK_Menu = '{0}' AND FK_Parent_MenuItem='{1}'", menuId, parentId);
            if (parentId == null || parentId == "" || parentId == Guid.Empty.ToString())
            {
                whereItems = String.Format("FK_Menu = '{0}' AND FK_Parent_MenuItem Is Null", menuId);
            }

            BaseCollection<CmsMenuItem> itemslist = BaseCollection<CmsMenuItem>.Get(whereItems, "OrderingNumber");

            return itemslist;
        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public BaseCollection<BaseObject> GetAllMenuItemsLite(string menuId)
        {
            BaseService.CheckLoginAndLicense();
            string whereItems = String.Format("FK_Menu = '{0}'", menuId);
            BaseCollection<CmsMenuItem> items = BaseCollection<CmsMenuItem>.Get(whereItems, "CompletePath");
            BaseCollection<BaseObject> returnValue = new BaseCollection<BaseObject>();
            foreach (CmsMenuItem item in items)
            {
                BaseObject obj = new BaseObject();
                obj.ID = item.ID;
                obj.Name = item.CompletePath;
                returnValue.Add(obj);
            }
            return returnValue;
        }
       

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public CmsMenuItem GetMenuItem(string id, string parentId, string menuId)
        {
            BaseService.CheckLoginAndLicense();
            CmsMenuItem item = null;
            if (id == null)
            {
                item = new CmsMenuItem();
                item.Site = SessionObject.CurrentSite;
                item.Menu = new CmsMenu();
                item.Menu.ID = new Guid(menuId);
                if (parentId != null && parentId != "")
                {
                    item.ParentMenuItem = new CmsMenuItem();
                    item.ParentMenuItem.ID = new Guid(parentId);
                    item.OrderingNumber = item.ParentMenuItem.GetMaxItemOrderNumber() + 1;
                }
                else
                {
                    item.OrderingNumber = item.Menu.GetMaxItemOrderNumber() + 1;
                }
            }
            else
            {
                item = BaseObject.GetById<CmsMenuItem>(new Guid(id));
            }
            return item;
        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public CmsMenuItem SaveMenuItem(CmsMenuItem obj)
        {
            BaseService.CheckLoginAndLicense();
            obj.Site = SessionObject.CurrentSite;
            if (obj.ParentMenuItem != null && obj.ParentMenuItem.ID == Guid.Empty)
            {
                obj.ParentMenuItem = null;
            }

            obj.Save();

            return obj;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void DeleteMenuItem(string id)
        {
            BaseService.CheckLoginAndLicense();
            CmsMenuItem item = BaseObject.GetById<CmsMenuItem>(new Guid(id));
            item.Delete();
        }

        


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void UpdateOrderingNummerItem(string itemId, string parentItemId, string menuId, int newOrderingNumber)
        {
            BaseService.CheckLoginAndLicense();
            if (parentItemId != "")
            {
                CmsMenuItem item = BaseObject.GetById<CmsMenuItem>(new Guid(parentItemId));
                item.MoveItems(itemId, newOrderingNumber);
            }
            else
            {
                CmsMenu menu = BaseObject.GetById<CmsMenu>(new Guid(menuId));
                menu.MoveItems(itemId, newOrderingNumber);
            }
        }

        

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void CopyMenu(string menuId, string newName)
        {
            BaseService.CheckLoginAndLicense();
            CmsMenu menu = BaseObject.GetById<CmsMenu>(new Guid(menuId));
            menu.Copy(newName);
        }
        

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void CopyMenuItem(string itemId, string newName)
        {
            BaseService.CheckLoginAndLicense();
            CmsMenuItem item = BaseObject.GetById<CmsMenuItem>(new Guid(itemId));
            item.Copy(newName);
        }

    }

}