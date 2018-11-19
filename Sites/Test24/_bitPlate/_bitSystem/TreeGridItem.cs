using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BitPlate.Domain;
using BitPlate.Domain.DataCollections;
using BitPlate.Domain.Autorisation;

namespace BitSite._bitPlate
{
    public class TreeGridItem
    {
        public Guid ID { get; set; }
        public int Level { get; set; }
        public bool IsActive { get; set; }
        public bool IsLeaf { get; set; }
        public string Type { get; set; }
        public string Icon { get; set; }
        //public string TypeIconClass { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public DateTime? LastPublishedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public DateTime CreateDate { get; set; }
        //public string Email { get; set; }
        public string Url { get; set; }
        public bool IsHomePage { get; set; }
        public bool HasAutorisation { get; set; }

        public string FileType { get; set; }   //BUG #103
        public string Volume { get; set; }     //BUG #103
        public string LanguageCode { get; set; } 
        public string Field1 { get; set; }
        public string Field2 { get; set; }
        public string Field3 { get; set; }
        public string Field4 { get; set; }
        public string Field5 { get; set; }
        public ChangeStatusEnum StatusEnum { get; set; }
        public string Status { get; set; }
        public string Path { get; set; }

        public TreeGridItem()
        {
        }

        public TreeGridItem(CmsPage page)
        {
            this.ID = page.ID;
            this.Name = page.Name;
            this.Title = page.Title;
            this.IsActive = page.IsActive;
            this.HasAutorisation = page.HasAutorisation;
            this.StatusEnum = page.ChangeStatus;
            if (page.LastPublishedDate == null)
            {
                this.StatusEnum = ChangeStatusEnum.New;
            }
            this.Status = page.ChangeStatusString;
            if (!page.IsActive)
            {
                //soms is Inactive string een te lange string voor in een grid. Daarom hier versimpelen
                this.Status = "Niet actief";
            }

            this.Type = "Item";
            this.Icon = "_img/icons/item_small.png";

            if (page.IsHomePage)
            {
                this.Icon = "_img/icons/home_small.png";
            }
            this.IsHomePage = page.IsHomePage;
            if (page.Folder != null)
            {
                this.Path = page.Folder.RelativePath;
            }
            this.Url =  page.RelativeUrl;
            this.IsLeaf = true;
            this.CreateDate = page.CreateDate;
            this.Field1 = SessionObject.CurrentSite.DomainName;
            this.Field2 = page.LanguageCode;

        }

        public TreeGridItem(CmsPageFolder folder)
        {
            this.ID = folder.ID;
            this.Name = folder.Name;
            this.IsActive = folder.IsActive;
            this.Status = folder.ChangeStatusString;
            this.HasAutorisation = folder.HasAutorisation;
            if (!folder.IsActive)
            {
                //soms is Inactive string een te lange string voor in een grid. Daarom hier versimpelen
                this.Status = "Niet actief";
            }

            this.Type = "Folder";
            this.Icon = "_img/icons/folder_small.png";
            //this.TypeIconClass = "bitTableFolder";
            this.IsLeaf = folder.IsLeaf();
            this.Level = folder.RelativePath.Split(new char[] { '/' }).Length - 1;
            if (folder.ParentFolder != null)
            {
                this.Path = folder.ParentFolder.RelativePath;
            }
            this.Path = folder.RelativePath;
            this.CreateDate = folder.CreateDate;
        }

        public TreeGridItem(CmsFile file)
        {
            this.ID = file.ID;
            this.Name = file.Name;
            this.IsActive = file.IsActive;
            this.Type = "Item";
            this.Icon = file.ImageSrc; //"_img/icons/item_small.png"; 
            this.IsLeaf = true;
            this.CreateDate = file.CreateDate;
            this.LastModifiedDate = file.ModifiedDate;
            this.FileType = file.FileType;              //BUG #103
            this.Volume = file.Volume;                  //BUG #103
        }

        public TreeGridItem(CmsDirectory directory)
        {
            this.ID = directory.ID;
            this.Name = directory.Name;
            this.IsActive = directory.IsActive;
            this.Type = "Folder";
            this.Icon = "_img/icons/folder_small.png";
            this.IsLeaf = false;
            this.CreateDate = directory.CreateDate;
            this.LastModifiedDate = directory.ModifiedDate;
            this.Path = directory.FullName;
            //this.FileType = directory.FileType;
            //this.Volume = directory.Volume;
        }

        public static TreeGridItem NewItem<T>(T t) where T : BaseDomainObject, new()
        {
            TreeGridItem item = new TreeGridItem();
            item.ID = t.ID;
            item.Name = t.Name;
            item.IsActive = t.IsActive;
            item.Status = "Actief";

            if (!t.IsActive)
            {
                item.Status = "Niet actief";
            }
            item.Type = "Item";
            //item.Icon = "_img/icons/item_small.png";

            item.IsLeaf = true;
            item.CreateDate = t.CreateDate;
            return item;
        }

        public static TreeGridItem NewGroup<T>(T t) where T : BaseDomainObject, new()
        {
            TreeGridItem item = new TreeGridItem();
            item.ID = t.ID;
            item.Name = t.Name;
            item.IsActive = t.IsActive;
            item.Status = "Actief";

            if (!t.IsActive)
            {
                item.Status = "Niet actief";
            }
            item.Type = "Group";
            //item.Icon = "_img/icons/folder_small.png";
            item.CreateDate = t.CreateDate;
            item.IsLeaf = true;
            return item;
        }

        public static TreeGridItem NewPublishableItem<T>(T t) where T : BaseDomainPublishableObject, new()
        {
            TreeGridItem item = new TreeGridItem();
            item.ID = t.ID;
            item.Name = t.Name;
            item.IsActive = t.IsActive;
            item.HasAutorisation = t.HasAutorisation;
            item.Status = t.ChangeStatusString;
            if (!t.IsActive)
            {
                //soms is Inactive string een te lange string voor in een grid. Daarom hier versimpelen
                item.Status = "Niet actief";
            }

            item.Type = "Item";
            //item.Icon = "_img/icons/item_small.png";
            item.CreateDate = t.CreateDate;
            item.IsLeaf = true;
            return item;
        }
    }
}