using HJORM;
using HJORM.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitPlate.Domain.Menu
{

    public enum MenuTypeEnum { Custom, HorizontalFlat, VerticalFlat, HorizontalDropDown, VerticalDropDown, Accordion }
    [Persistent("Menu")]
    public class CmsMenu : BaseDomainSiteObject
    {
        public string LanguageCode { get; set; }

        public MenuTypeEnum Type { get; set; }

        [NonPersistent()]
        public string TypeString
        {
            get
            {
                return Type.ToString();
            }
        }

        private BaseCollection<CmsMenuItem> _allItems;
        public BaseCollection<CmsMenuItem> GetAllMenuItems()
        {
            if (_allItems == null || (_allItems != null && !_allItems.IsLoaded))
            {
                _allItems = BaseCollection<CmsMenuItem>.Get("FK_Menu='" + this.ID + "'", "OrderingNumber, Name");
                _allItems.IsLoaded = true;
            }
            return _allItems;
        }

        private BaseCollection<CmsMenuItem> _parentItems;
        public BaseCollection<CmsMenuItem> GetParentMenuItems()
        {

            if (_parentItems == null || (_parentItems != null && !_parentItems.IsLoaded))
            {
                _parentItems = BaseCollection<CmsMenuItem>.Get("FK_Menu='" + this.ID + "' AND FK_Parent_MenuItem Is Null", "OrderingNumber, Name");
                _parentItems.IsLoaded = true;
            }
            return _parentItems;

        }

        BaseCollection<CmsScript> _scripts;
        [Persistent("ScriptPerMenu")]
        [Association("FK_Menu", "FK_Script")]
        public BaseCollection<CmsScript> Scripts
        {
            get
            {
                if (_scripts == null || (_scripts != null && !_scripts.IsLoaded))
                {
                    string where = "EXISTS (SELECT * FROM ScriptPerMenu WHERE FK_Menu = '" + this.ID + "' AND Script.ID = ScriptPerMenu.FK_Script)";
                    _scripts = BaseCollection<CmsScript>.Get(where, "ScriptType, Name");
                    _scripts.IsLoaded = true;
                }

                return _scripts;
            }
            set
            {
                _scripts = value;
                _scripts.IsLoaded = true;
            }
        }

        public int GetMaxItemOrderNumber()
        {
            int returnValue = 1;
            string sql = String.Format("SELECT MAX(OrderingNumber) FROM MenuItem WHERE FK_Parent_MenuItem IS NULL AND FK_Menu = '{0}'", this.ID);
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
                string sql = String.Format("UPDATE MenuItem SET OrderingNumber = OrderingNumber - 1 WHERE OrderingNumber > {0} And OrderingNumber <= {1} AND FK_Menu = '{2}' AND FK_Parent_MenuItem IS NULL", oldOrderingNumber, newOrderingNumber, this.ID);
                DataBase.Get().Execute(sql);
            }
            else if (oldOrderingNumber > newOrderingNumber)
            {
                //nummer wordt lager: alles tussen nieuwe nummer en oude nummer 1 plek verder zetten
                string sql = String.Format("UPDATE MenuItem SET OrderingNumber = OrderingNumber + 1 WHERE OrderingNumber < {0} And OrderingNumber >= {1} AND FK_Menu = '{2}' AND FK_Parent_MenuItem IS NULL", oldOrderingNumber, newOrderingNumber, this.ID);
                DataBase.Get().Execute(sql);
            }
            item.OrderingNumber = newOrderingNumber;
            item.Save();
        }

        public void Copy(string newName)
        {
            CmsMenu newMenu = base.CreateCopy<CmsMenu>(false);
            newMenu.Name = newName;
            //if (this.HasBitplateAutorisation())
            //{
            //    foreach (Autorisation.BitplateUserGroup userGroup in this.AutorizedBitplateUserGroups)
            //    {
            //        newPage.AutorizedBitplateUserGroups.Add(userGroup);
            //    }
            //    foreach (Autorisation.BitplateUser user in this.AutorizedSBitplateUsers)
            //    {
            //        newPage.AutorizedBitplateUsers.Add(user);
            //    }
            //}
            newMenu.Save();
            foreach (CmsMenuItem item in this.GetAllMenuItems())
            {
                item.Copy(item.Name, newMenu.ID);
            }



        }
    }
}
