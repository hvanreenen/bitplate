using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HJORM;
using HJORM.Attributes;
using System.IO;
using BitPlate.Domain.Utils;
using System.Web;

namespace BitPlate.Domain
{
    
    [Persistent("PageFolder")]
    public class CmsPageFolder : BaseDomainPublishableObject
    {
        public string RelativePath { get; set; }

        [NonPersistent()]
        public string AbsolutePath
        {
            get
            {
                string relativePath = GetCompletePath();
                if (Site != null)
                {
                    string absolutePath = Site.Path;
                    if (relativePath != null)
                    {
                        absolutePath += "\\" + relativePath.Replace("/", "\\");
                    }
                    return absolutePath;
                }
                else
                {
                    return null;
                }
            }
        }



        private CmsPageFolder _parentFolder;
        [Association("FK_Parent_Folder")]
        public CmsPageFolder ParentFolder
        {
            get
            {
                if (_parentFolder != null && !_parentFolder.IsLoaded)
                {
                    _parentFolder.Load();
                }
                return _parentFolder;
            }
            set { _parentFolder = value; }
        }


        [NonPersistent()]
        public override bool IsActive
        {
            get
            {
                bool returnValue = base.IsActive;
                if (ParentFolder != null && !ParentFolder.IsActive)
                {
                    returnValue = false;
                    _isActiveString += " (overgenomen van folder)";
                }
                return returnValue;
            }
        }

        public bool IsLeaf()
        {
            BaseCollection<CmsPageFolder> subFolders = GetSubFolders();
            BaseCollection<CmsPage> pages = GetPages();

            return (subFolders.Count == 0 && pages.Count == 0);
        }

        public override void Save()
        {
            if (IsNew && WebSessionHelper.CurrentLicense != null && WebSessionHelper.CurrentLicense.HasMaxNumberExceeded("PageFolder"))
            {
                throw new Exception("Kan niet opslaan: licentie heeft maximale aantal folders (" + WebSessionHelper.CurrentLicense.MaxNumberOfPageFolders + ") overschreden.");
            }
            Validate();
            RelativePath = GetCompletePath();
            //unique check
            //workaraound voor mysql: uniek is combi van naam, site en folder
            //als folder leeg is (NULL), gaat uniqueconstraint niet af in mySql
            //hierom in c# checken
            if (this.ParentFolder == null && ID != Guid.Empty)
            {
                string sql = String.Format("FK_Site='{0}' AND FK_Parent_Folder IS NULL AND name = '{1}' AND ID != '{2}'", Site.ID, this.Name, this.ID);
                CmsPageFolder checkFolder = BaseObject.GetFirst<CmsPageFolder>(sql);
                if (checkFolder != null)
                {
                    throw new Exception("Naam is niet uniek.");
                }
            }
            CmsPageFolder currentFolder = BaseObject.GetById<CmsPageFolder>(this.ID);
            if (currentFolder != null)
            {
                currentFolder.DeleteCurrentFolderLocation();
            }
            base.Save();
            
            //bij naamswijziging, moeten alle paden in subfolders en pagina's worden geupdate
            UpdateRelativePathAndUnpublishedItemsInSubFoldersAndPages();
            UnpublishedItem.Set(this, "Folder");
        }

        public void DeleteCurrentFolderLocation()
        {
            FileHelper.DeleteDir(this.AbsolutePath);
        }

        public void UpdateRelativePathAndUnpublishedItemsInSubFoldersAndPages()
        {
            BaseCollection<CmsPageFolder> subFolders = GetSubFolders();
            foreach (CmsPageFolder subfolder in subFolders)
            {
                subfolder.Save();
                UnpublishedItem.Set(subfolder, "Folder");
                subfolder.UpdateRelativePathAndUnpublishedItemsInSubFoldersAndPages();
            }
            BaseCollection<CmsPage> pages = GetPages();
            foreach (CmsPage page in pages)
            {
                page.Save();
                UnpublishedItem.Set(page, "Page");
            }
        }

        private void Validate()
        {
            bool valid = true;
            //controleer of parent path niet naar zichzelf verwijst
            CmsPageFolder parent = ParentFolder;
            while (valid && parent != null)
            {
                if (parent.Equals(this))
                {
                    valid = false;
                }
                parent = parent.ParentFolder;
            }
            if (!valid)
            {
                throw new Exception("Folder mag niet onder zichzelf vallen.");
            }
        }

        public string GetCompletePath()
        {
            //hack om er zeker van te zijn dat parentfolder laatste uit database is.
            //Dat kan niet, omdat je anders de parentFolder nooit kan wijzigen;)
            /* if (!IsNew)
            {
                CmsPageFolder dummy = BaseObject.GetById<CmsPageFolder>(this.ID);

                this.ParentFolder = dummy.ParentFolder;
                //this.Load();
            } */
            //einde hack
            string path = this.Name;
            CmsPageFolder parentFolder = this.ParentFolder;
            while (parentFolder != null)
            {
                path = parentFolder.Name + "/" + path;
                parentFolder = parentFolder.ParentFolder;
            }
            return path;
        }


        public BaseCollection<CmsPageFolder> GetSubFolders()
        {
            BaseCollection<CmsPageFolder> subFolders = BaseCollection<CmsPageFolder>.Get("FK_Parent_Folder = '" + this.ID + "'");
            return subFolders;
        }

        public BaseCollection<CmsPage> GetPages()
        {
            BaseCollection<CmsPage> pages = BaseCollection<CmsPage>.Get("FK_Folder = '" + this.ID + "'", "Name");
            return pages;
        }

        public override void Delete()
        {
            
            base.Delete();
            UnpublishedItem.Set(this, "Folder", ChangeStatusEnum.Deleted);

        }

        public CmsPageFolder Copy(string newName, Guid? parentFolderID)
        {
            CmsPageFolder newFolder = this.CreateCopy<CmsPageFolder>(false);
            if (newName != "")
            {
                newFolder.Name = newName;
            }
            newFolder.LastPublishedDate = null;
            
            if (parentFolderID.HasValue)
            {
                newFolder.ParentFolder = new CmsPageFolder();
                newFolder.ParentFolder.ID = parentFolderID.Value;
            }
            if (this.HasAutorisation)
            {
                foreach (Autorisation.SiteUserGroup userGroup in this.AutorizedSiteUserGroups)
                {
                    newFolder.AutorizedSiteUserGroups.Add(userGroup);
                }
                foreach (Autorisation.SiteUser user in this.AutorizedSiteUsers)
                {
                    newFolder.AutorizedSiteUsers.Add(user);
                }
            }
            newFolder.Save();

            foreach (CmsPageFolder subFolder in this.GetSubFolders())
            {
                CmsPageFolder newSubFolder = subFolder.Copy(subFolder.Name, newFolder.ID);

            }
            foreach (CmsPage page in this.GetPages())
            {
                CmsPage newPage = page.Copy(page.Name, newFolder.ID);

            }
            return newFolder;
        }

        public CmsPage GetHomePage()
        {
            CmsPage homePage;
            BaseCollection<CmsPage> pagesInFolder;
            ////IS HET DE DOCUMENTROOT? ZOEK DAN DE PAGINAS
            //pagesInFolder = BaseCollection<CmsPage>.Get("FK_Site = '" + this.Site.ID.ToString() + "' AND FK_Folder IS NULL");

            pagesInFolder = BaseCollection<CmsPage>.Get("FK_Folder = '" + this.ID.ToString() + "'");
            homePage = pagesInFolder.Where(c => c.IsHomePage).FirstOrDefault();

            if (homePage == null)
            {
                homePage = pagesInFolder.Where(c => c.Name.ToLower() == "default").FirstOrDefault();
            }

            if (homePage == null)
            {
                homePage = pagesInFolder.Where(c => c.Name.ToLower() == "index").FirstOrDefault();
            }

            if (homePage == null)
            {
                homePage = pagesInFolder.FirstOrDefault();
            }

            return homePage;
        }

        public override void FillObject(System.Data.DataRow dataRow)
        {
            base.FillObject(dataRow);
            if (dataRow["FK_Parent_Folder"] != DBNull.Value)
            {
                this.ParentFolder = new CmsPageFolder();
                this.ParentFolder.ID = DataConverter.ToGuid(dataRow["FK_Parent_Folder"]);
            }
            this.RelativePath = dataRow["RelativePath"].ToString();
            this.IsLoaded = true;
        }
    }
}
