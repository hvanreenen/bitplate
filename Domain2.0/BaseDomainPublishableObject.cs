using System;
using System.Collections.Generic;
using System.Text;
using HJORM;
using HJORM.Attributes;
using BitPlate.Domain.Autorisation;
using System.IO;

namespace BitPlate.Domain
{
    /// <summary>
    /// Objecten die gepubliceerd kunnen worden, zoals Page, Template en DataItem
    /// Bevat velden om publiceren te registreren en ook virtuele methode Publish()
    /// Hiernaast bevat dit object ook de autorisatie van een object
    /// </summary>
    
    public class BaseDomainPublishableObject : BaseDomainSiteObject
    {
        public DateTime? LastPublishedDate { get; set; }
        
        public ChangeStatusEnum ChangeStatus { get; set; }
        [NonPersistent()]
        public string ChangeStatusString
        {
            get
            {
                if (LastPublishedDate == null)
                {
                    ChangeStatus = ChangeStatusEnum.New;
                }
                if (!this.IsActive)
                {
                    return base._isActiveString;
                }
                else if (ChangeStatus == ChangeStatusEnum.Published)
                {
                    return "Gepubliceerd";
                }
                if (ChangeStatus == ChangeStatusEnum.Modified)
                {
                    return "Gewijzigd";
                }
                else if (ChangeStatus == ChangeStatusEnum.Deleted)
                {
                    return "Verwijderd";
                }
                else
                {
                    return "Nieuw";
                }

            }
        }



        public virtual void SavePublishInfo()
        {
            this.LastPublishedDate = DateTime.Now;
            this.ChangeStatus = ChangeStatusEnum.Published;
            
            base.Save();
        }

        public void SaveLastPublishInfo(string tableName, string fileName)
        {
            fileName = fileName.Replace(@"\", @"\\");
            string sql = String.Format("UPDATE {0} SET ChangeStatus={1}, ModifiedDate=NOW() WHERE ID='{2}'", tableName, (int)ChangeStatusEnum.Modified,  this.ID);
            DataBase.Get().Execute(sql);
        }
        
        public bool HasAutorisation {get; set;}

        public bool HasBitplateAutorisation()
        {
            bool returnvalue = false;
            if (HasAutorisation)
            {
                returnvalue = (AutorizedBitplateUserGroups.Count > 0 || AutorizedBitplateUsers.Count > 0);
            }
            return returnvalue;
        }

        public bool HasSiteAutorisation()
        {
            bool returnvalue = false;
            if (HasAutorisation)
            {
                returnvalue = (AutorizedSiteUserGroups.Count > 0 || AutorizedSiteUsers.Count > 0);
            }
            return returnvalue;
        }

        private BaseCollection<Autorisation.BitplateUserGroup> _autorizedBitplateUserGroups;
        [System.Xml.Serialization.XmlIgnore()]
        [Association("FK_Object", "FK_BitplateUserGroup")]
        [Persistent("UserGroupObjectPermission")]
        public BaseCollection<Autorisation.BitplateUserGroup> AutorizedBitplateUserGroups {
            get
            {
                if (HasAutorisation && _autorizedBitplateUserGroups == null)
                {
                    string where = "EXISTS (SELECT * FROM UserGroupObjectPermission WHERE FK_Object = '" + this.ID + "' AND UserGroupObjectPermission.FK_BitplateUserGroup = BitplateUserGroup.ID)";
                    _autorizedBitplateUserGroups = BaseCollection<Autorisation.BitplateUserGroup>.Get(where, "Name");
                }
                return _autorizedBitplateUserGroups;
            }
            set
            {
                _autorizedBitplateUserGroups = value;
            }
        }

        private BaseCollection<Autorisation.BitplateUser> _autorizedBitplateUsers;
        [System.Xml.Serialization.XmlIgnore()]
        [Association("FK_Object", "FK_BitplateUser")]
        [Persistent("UserObjectPermission")]
        public BaseCollection<Autorisation.BitplateUser> AutorizedBitplateUsers
        {
            get
            {
                if (HasAutorisation && _autorizedBitplateUsers == null)
                {
                    string where = "EXISTS (SELECT * FROM UserObjectPermission WHERE FK_Object = '" + this.ID + "' AND UserObjectPermission.FK_BitplateUser = BitplateUser.ID)";
                    _autorizedBitplateUsers = BaseCollection<Autorisation.BitplateUser>.Get(where, "Name");
                }
                return _autorizedBitplateUsers;
            }
            set
            {
                _autorizedBitplateUsers = value;
            }
        }

        private BaseCollection<Autorisation.SiteUserGroup> _autorizedSiteUserGroups;
        [System.Xml.Serialization.XmlIgnore()]
        [Association("FK_Object", "FK_SiteUserGroup")]
        [Persistent("UserGroupObjectPermission")]
        public BaseCollection<Autorisation.SiteUserGroup> AutorizedSiteUserGroups
        {
            get
            {
                if (HasAutorisation && _autorizedSiteUserGroups == null)
                {
                    string where = "EXISTS (SELECT * FROM UserGroupObjectPermission WHERE FK_Object = '" + this.ID + "' AND UserGroupObjectPermission.FK_SiteUserGroup = SiteUserGroup.ID)";
                    _autorizedSiteUserGroups = BaseCollection<Autorisation.SiteUserGroup>.Get(where, "Name");
                }
                return _autorizedSiteUserGroups;
            }
            set
            {
                _autorizedSiteUserGroups = value;
            }
        }

        private BaseCollection<Autorisation.SiteUser> _autorizedSiteUsers;
        [System.Xml.Serialization.XmlIgnore()]
        [Association("FK_Object", "FK_SiteUser")]
        [Persistent("UserObjectPermission")]
        public BaseCollection<Autorisation.SiteUser> AutorizedSiteUsers
        {
            get
            {
                if (HasAutorisation && _autorizedSiteUsers == null)
                {
                    string where = "EXISTS (SELECT * FROM UserObjectPermission WHERE FK_Object = '" + this.ID + "' AND UserObjectPermission.FK_SiteUser = SiteUser.ID)";
                    _autorizedSiteUsers = BaseCollection<Autorisation.SiteUser>.Get(where, "Name");
                }
                return _autorizedSiteUsers;
            }
            set
            {
                _autorizedSiteUsers = value;
            }
        }

        //private BaseCollection<Autorisation.ObjectPermission> _objectPermissions;
        //[System.Xml.Serialization.XmlIgnore()]
        //[Association("FK_Object")]
        //[Persistent("ObjectPermission")]
        //public BaseCollection<Autorisation.ObjectPermission> ObjectPermissions
        //{
        //    get
        //    {
        //        if (HasAutorisation && _objectPermissions == null)
        //        {
        //            string where = "FK_Object = '" + this.ID + "'";
        //            _objectPermissions = BaseCollection<Autorisation.ObjectPermission>.Get(where, "Name");
        //        }
        //        return _objectPermissions;
        //    }
        //    set
        //    {
        //        _objectPermissions = value;
        //    }
        //}

        public override void Save()
        {
            this.ChangeStatus = ChangeStatusEnum.Modified;
            
            if (this.IsNew)
            {
                this.ChangeStatus = ChangeStatusEnum.New;
            }
            //als er iets is gevuld bij AutorisedUsers of UserGroups dan HasAutorisation = false
            if (HasAutorisation && AutorizedBitplateUsers.Count == 0 && AutorizedBitplateUserGroups.Count == 0 && AutorizedSiteUsers.Count == 0 && AutorizedSiteUserGroups.Count == 0)
            {
                HasAutorisation = false;
            }
            

            base.Save();
        }
        /// <summary>
        /// lijst van BitplateUserGroups en BitPlateUsers samengevoegd in 1 lijst
        /// Dit zorgt ook voor 1 caal aan server
        /// Later kan er hier dan nog onderscheid worden gemaakt tussen AllowView, AllowEdit, AllowConfig enz.
        /// </summary>
        /// <returns></returns>
        //public BaseCollection<ObjectPermission> GetObjectPermissions4LicenseServer()
        //{

        //    BaseCollection<ObjectPermission> objPermissions = new BaseCollection<ObjectPermission>();
        //    if (this.HasAutorisation && this.AutorizedBitplateUserGroups != null && this.AutorizedBitplateUserGroups.Length > 0)
        //    {
        //        foreach (BitplateUserGroup userGroup in this.AutorizedBitplateUserGroups)
        //        {
        //            ObjectPermission objPermission = new ObjectPermission();
        //            objPermission.FK_Site = this.Site.ID;
        //            objPermission.FK_Object = this.ID;
        //            objPermission.FK_UserGroup = userGroup.ID;
        //            objPermission.ObjectType = this.GetType().ToString();
        //            objPermission.PermissionType = 1;
        //            objPermissions.Add(objPermission);
        //        }

        //    }
        //    if (this.HasAutorisation && this.AutorizedBitplateUsers != null && this.AutorizedBitplateUsers.Length > 0)
        //    {
        //        foreach (BitplateUser user in this.AutorizedBitplateUsers)
        //        {
        //            ObjectPermission objPermission = new ObjectPermission();
        //            objPermission.FK_Object = this.ID;
        //            objPermission.FK_User = user.ID;
        //            objPermission.ObjectType = this.GetType().ToString();
        //            objPermission.PermissionType = 2;
        //            objPermissions.Add(objPermission);
        //        }

        //    }
        //    return objPermissions;
        //}

        public bool IsAutorized(Autorisation.SiteUser user)
        {
            bool returnValue = true;
            if (HasAutorisation)
            {
                returnValue = false;
                if (user != null)
                {
                    foreach (Autorisation.SiteUserGroup userGroup in AutorizedSiteUserGroups)
                    {
                        if (user.UserGroups.Contains(userGroup))
                        {
                            returnValue = true;
                            break;
                        }
                    }
                    if (returnValue == false)
                    {
                        //kijk of gebruiker als individuele gebruik is toegevoegd aan object
                        foreach (Autorisation.SiteUser authUser in AutorizedSiteUsers)
                        {
                            if (authUser.Equals(user))
                            {
                                returnValue = true;
                                break;
                            }
                        }
                    }
                }
            }
            return returnValue;
        }

        public bool IsAutorized(Autorisation.BitplateUser user)
        {
            bool returnValue = true;
            if (user.GetUserType() == UserTypeEnum.Developers)
            {
                return true;
            }
            if (HasAutorisation)
            {
                returnValue = false;
                foreach (Autorisation.BitplateUserGroup userGroup in AutorizedBitplateUserGroups)
                {
                    if (user.UserGroups.Contains(userGroup))
                    {
                        returnValue = true;
                        break;
                    }
                }
                if (returnValue == false)
                {
                    //kijk of gebruiker als individuele gebruik is toegevoegd aan object
                    foreach (Autorisation.BitplateUser authUser in AutorizedBitplateUsers)
                    {
                        if (authUser.Equals(user))
                        {
                            returnValue = true;
                            break;
                        }
                    }
                }
            }
            return returnValue;
        }

        public override void FillObject(System.Data.DataRow dataRow)
        {
            base.FillObject(dataRow);
            if (dataRow.Table.Columns.Contains("LastPublishedDate")) this.LastPublishedDate = HJORM.DataConverter.ToNullableDateTime(dataRow["LastPublishedDate"]);
            if (dataRow.Table.Columns.Contains("ChangeStatus")) this.ChangeStatus = dataRow["ChangeStatus"] == DBNull.Value ? ChangeStatusEnum.New : (ChangeStatusEnum)Convert.ToInt32(dataRow["ChangeStatus"]);
            if (dataRow.Table.Columns.Contains("HasAutorisation")) this.HasAutorisation = HJORM.DataConverter.ToBoolean(dataRow["HasAutorisation"]);
            //if (this.HasBitplateAutorisation())
            //{
            //    if (!this.IsAutorized(Utils.WebSessionHelper.CurrentBitplateUser))
            //    {
            //        throw new Exception("U heeft geen rechten op deze data.");
            //    }
            //}
        }

    }
}
