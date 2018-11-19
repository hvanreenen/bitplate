using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HJORM;
using HJORM.Attributes;

//using BitPlate.Domain.DataCollections;
using BitPlate.Domain.Modules;


namespace BitPlate.Domain.Autorisation
{
    [Persistent("UserGroup")]
    public class CmsSiteUserGroup : BaseUserGroup
    {
        private CmsSite _site;
        [Association("FK_Site")]
        public CmsSite Site
        {
            get
            {
                if (_site != null && !_site.IsLoaded)
                {
                    _site.Load();
                }

                return _site;
            }
            set { _site = value; }
        }

        private CmsSiteUserGroup _parentGroup;
        [Association("FK_Parent_Group")]
        public CmsSiteUserGroup ParentGroup
        {
            get
            {
                if (_parentGroup != null && !_parentGroup.IsLoaded)
                {
                    _parentGroup.Load();
                }
                return _parentGroup;
            }
            set { _parentGroup = value; }
        }
        public bool IsLeaf()
        {
            BaseCollection<CmsSiteUserGroup> subGroups = GetSubGroups();
            BaseCollection<CmsSiteUser> users = GetUsers();

            return (subGroups.Count == 0 && users.Count == 0);
        }

        public BaseCollection<CmsSiteUserGroup> GetSubGroups()
        {
            BaseCollection<CmsSiteUserGroup> subFolders = BaseCollection<CmsSiteUserGroup>.Get("FK_Parent_Group = '" + this.ID + "'");
            return subFolders;
        }

        public BaseCollection<CmsSiteUser> GetUsers()
        {
            BaseCollection<CmsSiteUser> users = BaseCollection<CmsSiteUser>.Get("FK_Folder = '" + this.ID + "'");
            return users;
        }
       
        /// <summary>
        /// Nieuwsbrief groep of webshopgroep
        /// </summary>
        public UserGroupEnum Type { get; set; }
        
        /// <summary>
        /// Is de gebruikersgroep zichtbaar als checkbox in Formuliermodule 
        /// </summary>
        public bool IsPublicVisible { get; set; }

        //public static CmsUserGroup GetOrNewUserGroupByName(string usergroupName, CmsSite site)
        //{
        //    string where = String.Format("FK_Site='{0}' AND name = '{1}' AND RoleType={2}", site.ID, usergroupName, (int)RoleEnum.NewsletterReceivers);
        //    CmsUserGroup returnValue = new CmsUserGroup();
        //    try
        //    {
        //        returnValue = (CmsUserGroup) BaseCollection<CmsUserGroup>.Get(where)[0];
        //    }
        //    catch{
        //        returnValue.Site = site;
        //        returnValue.Name = usergroupName;
        //        returnValue.RoleType = RoleEnum.NewsletterReceivers;
        //        returnValue.Save();
        //    }
        //    return returnValue;
        //}
    }
}
