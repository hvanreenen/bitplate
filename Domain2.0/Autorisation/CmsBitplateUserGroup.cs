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
    //Deze moeten in een apart User Database komen
    [Persistent("UserGroup")]
    public class CmsBitplateUserGroup : BaseUserGroup
    {

        private CmsBitplateUserGroup _parentGroup;
        [Association("FK_Parent_Group")]
        public CmsBitplateUserGroup ParentGroup
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
            BaseCollection<CmsBitplateUserGroup> subGroups = GetSubGroups();
            BaseCollection<CmsBitplateUser> users = GetUsers();

            return (subGroups.Count == 0 && users.Count == 0);
        }

        public BaseCollection<CmsBitplateUserGroup> GetSubGroups()
        {
            BaseCollection<CmsBitplateUserGroup> subFolders = BaseCollection<CmsBitplateUserGroup>.Get("FK_Parent_Group = '" + this.ID + "'");
            return subFolders;
        }

        public BaseCollection<CmsBitplateUser> GetUsers()
        {
            BaseCollection<CmsBitplateUser> users = BaseCollection<CmsBitplateUser>.Get("FK_Folder = '" + this.ID + "'");
            return users;
        }
        
    }
}
