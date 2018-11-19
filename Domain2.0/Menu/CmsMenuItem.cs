using HJORM;
using HJORM.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitPlate.Domain.Menu
{
    public enum UrlTypeEnum {InternalLink, ExternalLink};
    [Persistent("MenuItem")]
    public class CmsMenuItem: BaseDomainSiteObject
    {
        public string Title { get; set; }

        public int OrderingNumber { get; set; }

        private CmsMenu _menu;
        [Association("FK_Menu")]
        public CmsMenu Menu
        {
            get
            {
                if (_menu != null && !_menu.IsLoaded)
                {
                    _menu.Load();
                }
                return _menu;
            }
            set { _menu = value; }
        }

        private CmsMenuItem _parentMenuItem;
        [Association("FK_Parent_MenuItem")]
        public CmsMenuItem ParentMenuItem
        {
            get
            {
                if (_parentMenuItem != null && !_parentMenuItem.IsLoaded)
                {
                    _parentMenuItem.Load();
                }
                return _parentMenuItem;
            }
            set { _parentMenuItem = value; }
        }

        private BaseCollection<CmsMenuItem> _subItems;
        public BaseCollection<CmsMenuItem> GetSubMenuItems()
        {

            if (_subItems == null || (_subItems != null && !_subItems.IsLoaded))
            {
                _subItems = BaseCollection<CmsMenuItem>.Get("FK_Parent_MenuItem='" + this.ID + "'", "OrderingNumber, Name");
                _subItems.IsLoaded = true;
            }
            return _subItems;

        }
        public string CompletePath { get; set; }
        public UrlTypeEnum UrlType { get; set; }
        private CmsPage _page;
        [Association("FK_Page")]
        public CmsPage Page
        {
            get
            {
                if (_page != null && !_page.IsLoaded)
                {
                    _page.Load();
                }
                return _page;
            }
            set { _page = value; }
        }

        public string Target { get; set; }
        public string CssClass { get; set; }
        public string ExternalUrl { get; set; }
        public string ImageUrl { get; set; }
        public string ImageHoverUrl { get; set; }
        public string ImageActiveUrl { get; set; }

        [NonPersistent()]
        public string Url
        {
            get
            {
                if (UrlType == UrlTypeEnum.InternalLink && Page != null)
                {
                    return Page.RelativeUrl;
                }
                else
                {
                    return ExternalUrl;
                }
            }
        }

        public override void Save()
        {
            
            CompletePath = GetCompletePath();
            base.Save();
        }

        public string GetCompletePath()
        {
            string path = this.Name;
            CmsMenuItem parent = this.ParentMenuItem;
            while (parent != null)
            {
                //path = this.ParentGroup.Name + "/" + path;
                path = parent.Name + "/" + path;
                parent = parent.ParentMenuItem;
            }
            return path;
        }

        public int GetMaxItemOrderNumber()
        {
            int returnValue = 1;
            string sql = String.Format("SELECT MAX(OrderingNumber) FROM MenuItem WHERE FK_Parent_MenuItem = '{0}'", this.ID);
            object result = DataBase.Get().Execute(sql);
            if (result != null)
            {
                returnValue = Convert.ToInt32(result);
            }
            return returnValue;
        }

        public void MoveItems(string itemId, int newOrderingNumber)
        {
            CmsMenuItem item = BaseObject.GetById<CmsMenuItem>(new Guid(itemId));
            int oldOrderingNumber = item.OrderingNumber;
            if (oldOrderingNumber < newOrderingNumber)
            {
                //nummer wordt hoger: alles tussen oude nummer en nieuwe nummer 1 plek lager zetten
                string sql = String.Format("UPDATE MenuItem SET OrderingNumber = OrderingNumber - 1 WHERE OrderingNumber > {0} And OrderingNumber <= {1} AND FK_Parent_MenuItem = '{2}'", oldOrderingNumber, newOrderingNumber, this.ID);
                DataBase.Get().Execute(sql);
            }
            else if (oldOrderingNumber > newOrderingNumber)
            {
                //nummer wordt lager: alles tussen nieuwe nummer en oude nummer 1 plek verder zetten
                string sql = String.Format("UPDATE MenuItem SET OrderingNumber = OrderingNumber + 1 WHERE OrderingNumber < {0} And OrderingNumber >= {1} AND FK_Parent_MenuItem = '{2}'", oldOrderingNumber, newOrderingNumber, this.ID);
                DataBase.Get().Execute(sql);
            }
            item.OrderingNumber = newOrderingNumber;
            item.Save();
        }

        public void Copy(string newName)
        {
            CmsMenuItem newItem = base.CreateCopy<CmsMenuItem>(false);
            newItem.Name = newName;
            newItem.Save();
        }

        internal void Copy(string newName, Guid menuId)
        {
            CmsMenuItem newItem = base.CreateCopy<CmsMenuItem>(false);
            newItem.Menu.ID = menuId;
            newItem.Name = newName;
            newItem.Save();
        }
    }
}
