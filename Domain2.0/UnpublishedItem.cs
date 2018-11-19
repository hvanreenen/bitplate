using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HJORM;
using HJORM.Attributes;
using BitPlate.Domain.Utils;

namespace BitPlate.Domain
{
    public enum ChangeStatusEnum { Published, Modified, New, Deleted, PublishError }

    /// <summary>
    /// Dit wordt een lijst van alle ongepubliceerde items
    /// Dat kunnen zijn pagina's, templates, folders en....
    /// Er zit enige redundantie in met de objecten zelf. 
    /// In de objecten zelf wordt namelijk ook bijgehouden wat de publiceer status is en wat de laatste publiceer datum is.
    /// Dit om minder database calls te hebben....
    /// </summary>

    
    public class UnpublishedItem : BaseDomainSiteObject
    {
        [NonPersistent()]
        public bool Checked = true;
        public string Type { get; set; }
        public Guid FK_Object { get; set; }
        public ChangeStatusEnum ChangeStatus { get; set; }
        [NonPersistent()]
        public string ChangeStatusString
        {
            get {
                //Name() method van enum = extension method in Utils.TypeExtensions.cs
                return ChangeStatus.Name();
            }
        }
        public string UserName { get; set; }
        public Guid FK_User { get; set; }
        /// <summary>
        /// Laatste publiceerdatum van het Object waar het om gaat
        /// Dit veld is redundant met LastPublishedDate in het Object
        /// Omwille van performance hier dubbel opgenomen
        /// </summary>
        public DateTime? LastPublishedDate  { get; set; }
        public string Actions { get; set; }
        
        //public static void Set(BaseDomainPublishableObject baseObj, string Type)
        //{
        //    if (isNew)
        //    {
        //        Set(baseObj, Type, ChangeStatusEnum.New);
        //    }
        //    else
        //    {
        //        Set(baseObj, Type, ChangeStatusEnum.Modified);
        //    }
        //}
        public static void Set(BaseDomainPublishableObject baseObj, string Type)
        {
            Set(baseObj, Type, false);
        }

        public static void Set(BaseDomainPublishableObject baseObj, string Type, bool alsoUpdateObject)
        {
            if (baseObj == null) return;
            if (baseObj.LastPublishedDate == null)
            {
                Set(baseObj, Type, ChangeStatusEnum.New, alsoUpdateObject);
            }
            else
            {
                Set(baseObj, Type, ChangeStatusEnum.Modified, alsoUpdateObject);
            }
        }

        public static void Set(BaseDomainPublishableObject baseObj, string Type, ChangeStatusEnum ChangeStatus)
        {
            Set(baseObj, Type, ChangeStatus, false);
        }

        public static void Set(BaseDomainPublishableObject baseObj, string Type, ChangeStatusEnum ChangeStatus, bool alsoUpdateObject){
            //even uitgezet
            return;
            if (!(Type == "Folder" || Type == "Page" || Type == "Module" || Type == "Template" || Type == "Script" || Type == "DataCollection" || Type == "DataGroup" || Type == "DataItem"))
            {
                throw new Exception("Geen geldige type voor UnpublishedItem");
            }


            UnpublishedItem item = new UnpublishedItem();
            //try to find
            string where = String.Format("FK_Object = '{0}'", baseObj.ID);
            BaseCollection<UnpublishedItem> items = BaseCollection<UnpublishedItem>.Get(where);
            if (items.Count > 0)
            {
                item = items[0];
            }
            else
            {
                //nieuwe
                
            }
            if (baseObj.LastPublishedDate == null && ChangeStatus == ChangeStatusEnum.Deleted)
            {
                item.Delete();
            }
            else
            {
                item.LastPublishedDate = baseObj.LastPublishedDate;
                item.ChangeStatus = ChangeStatus;
                item.Site = baseObj.Site;
                item.Name = baseObj.Name;
                item.FK_Object = baseObj.ID;
                item.Type = Type;
                item.ModifiedDate = DateTime.Now;
                if (Utils.WebSessionHelper.CurrentBitplateUser != null)
                {
                    item.UserName = Utils.WebSessionHelper.CurrentBitplateUser.Email;
                    item.FK_User = Utils.WebSessionHelper.CurrentBitplateUser.ID;
                }
                if (ChangeStatus == ChangeStatusEnum.Deleted)
                {
                    item.Actions += String.Format("Verwijderd door {0} op {1:dd-MM-yyyy HH:mm}; ", item.UserName, DateTime.Now);
                }
                else
                {
                    item.Actions += String.Format("Bewaard door {0} op {1:dd-MM-yyyy HH:mm}; ", item.UserName, DateTime.Now);
                }
                item.Save();
            }
            if (alsoUpdateObject && ChangeStatus == ChangeStatusEnum.Modified)
            {
                //in de objecten wordt change-status ook bewaard (redundant (omwilen van minder db-calls)
                //hier wordt die gezet
                //dit wordt aangeroepen bij wijziging van een module op een pagina.
                string sql = String.Format("UPDATE {0} SET ChangeStatus={1}, ModifiedDate=NOW() WHERE ID='{2}'", Type,  (int)ChangeStatusEnum.Modified, baseObj.ID);
                DataBase.Get().Execute(sql);
            }

            
        }
        /// <summary>
        /// Try Delete
        /// Als die bestaat: weggooien
        /// </summary>
        /// <param name="fk_object"></param>
        public static void DeleteByFK_Object(Guid fk_object)
        {
            UnpublishedItem item = null;
            //try to find
            string where = String.Format("FK_Object = '{0}'", fk_object);
            BaseCollection<UnpublishedItem> items = BaseCollection<UnpublishedItem>.Get(where);
            if (items.Count > 0)
            {
                item = items[0];
                item.Delete();
            }
        }
        /// <summary>
        /// Van objecten die zijn weggegooid uit de database wordt hier de live versie weggegooid
        /// </summary>

        public void DeleteLiveObject()
        {
            if (Type == "Template")
            {
                string fileName = this.Site.Path + this.Name + ".Master";
                Utils.FileHelper.DeleteFile(fileName);
            }
            else if (Type == "PageFolder")
            {
                string path = this.Site.Path  + this.Name;
                Utils.FileHelper.DeleteFile(path);
            }
            else if (Type == "Page")
            {
                string fileName = this.Site.Path+ this.Name + ".aspx";
                Utils.FileHelper.DeleteFile(fileName);
            }
            else if (Type == "Script")
            {
                string fileName = this.Site.Path  + this.Name;
                Utils.FileHelper.DeleteFile(fileName);
            }
        }




        internal static void WriteError(Guid fk_object, Exception ex)
        {
            try
            {
                string where = String.Format("FK_Object = '{0}'", fk_object);
                UnpublishedItem item = BaseObject.GetFirst<UnpublishedItem>(where);
                if (item != null)
                {
                    item.ChangeStatus = ChangeStatusEnum.PublishError;
                    item.Actions += "Fout tijdens publiceren: " + ex.Message + "; ";
                    item.Save();
                }
            }
            catch (Exception e)
            {
            }
        }
    }
}
