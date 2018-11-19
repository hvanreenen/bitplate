using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BitPlate.Domain
{
    
    public class CmsFile : BaseDomainSiteObject
    {
        public CmsFile()
        {
        }
        public CmsFile(FileInfo fileInfoObject, CmsSite site, string theme)
        {
            this.Site = site;
            this.Name = fileInfoObject.Name;
            this.ModifiedDate = fileInfoObject.LastWriteTime;
            this.CreateDate = fileInfoObject.CreationTime;
            this.FileType = fileInfoObject.Extension;
            this.Volume = getVolumeString(fileInfoObject.Length);
            this.ImageSrc = getImageSrc(fileInfoObject, theme); //@HJ Hoe krijg je hier het theme?
            
        }

        private string getImageSrc(FileInfo fileInfoObject, string theme)
        {
            string imageSrc = "";

            if (Utils.ImageHelper.IsImage(fileInfoObject.Name))
            {
                imageSrc = fileInfoObject.FullName;
                
                imageSrc = imageSrc.Replace(Site.Path, "");
                imageSrc = imageSrc.Replace("\\", "/");
                //System.Web.HttpContext.Current.Request.Url
                imageSrc = Site.DomainName + "/" + imageSrc;
            }
            else
            {
                //afhankelijk van file type
                if (fileInfoObject.Extension == ".pdf")
                {
                    imageSrc = "_themes/" + theme + "/images/fileTypes/pdf.gif";
                }
                else if (fileInfoObject.Extension == ".doc" || fileInfoObject.Extension == ".docx")
                {
                    imageSrc = "_themes/" + theme + "/images/fileTypes/doc.gif";
                }
                else if (fileInfoObject.Extension == ".mp3")
                {
                    imageSrc = "_themes/" + theme + "/images/fileTypes/mp3.gif";
                }
                else if (fileInfoObject.Extension == ".xls" || fileInfoObject.Extension == ".xlsx")
                {
                    imageSrc = "_themes/" + theme + "/images/fileTypes/xls.gif";
                }
                else if (fileInfoObject.Extension == ".zip")
                {
                    imageSrc = "_themes/" + theme + "/images/fileTypes/zip.gif";
                }
                else if (fileInfoObject.Extension == ".css" || fileInfoObject.Extension == ".js" || fileInfoObject.Extension == ".txt")
                {
                    imageSrc = "_themes/" + theme + "/images/fileTypes/txt.gif";
                }
                else
                {
                    imageSrc = "_themes/" + theme + "/images/fileTypes/file.gif";
                }

            }
            return imageSrc;
        }

        public string FileType { get; set; }
        public string Volume { get; set; }
        public string ImageSrc { get; set; }

        public override void Save()
        {
        }
        public override void Delete()
        {
        }

        private string getVolumeString(double length)
        {
            string[] sizes = { "b", "kB", "MB", "GB" };

            int order = 0;
            while (length >= 1024 && order + 1 < sizes.Length)
            {
                order++;
                length = length / 1024;
            }
            return String.Format("{0:0.##}{1}", length, sizes[order]);
        }
    }


}
