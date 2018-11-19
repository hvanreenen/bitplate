using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HJORM;
using HJORM.Attributes;

namespace BitPlate.Domain.Autorisation
{
    
    public class ObjectPermission: BaseDomainObject
    {
        public Guid FK_Site { get; set; }
        public Guid FK_Object { get; set; }
        public Guid FK_User { get; set; }
        public Guid FK_UserGroup { get; set; }
        
        public String ObjectType { get; set; }
        //PermissionType = BitPlateUserGroup, BitPlateUser, SiteUserGroup, SiteUser
        public int PermissionType { get; set; }
    }
}
