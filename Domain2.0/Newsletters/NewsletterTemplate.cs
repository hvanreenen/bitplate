using BitPlate.Domain.Utils;
using HJORM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HJORM.Attributes;
namespace BitPlate.Domain.Newsletters
{
    [Persistent("Template")]
    public class NewsletterTemplate: CmsTemplate
    {
        //public override void Publish()
        //{
        //    //maak er een master page van.
        //    string html = this.getPublishHtml();

        //    //save
        //    string fileName = this.Site.Path  + "\\" + this.Name + ".Master";
        //    if (!Directory.Exists(this.Site.Path + "\\Newsletters\\")) //BUG FIX
        //    {
        //        Directory.CreateDirectory(this.Site.Path + "\\Newsletters\\");
        //    }
        //    fileName = this.Site.Path + "\\Newsletters\\" + this.Name + ".Master";
        //    FileHelper.WriteFile(fileName, html);

        //    if (LastPublishedFileName != null && LastPublishedFileName != fileName)
        //    {
        //        //naam is gewijzigd
        //        //dan oude master weggooien
        //        FileHelper.DeleteFile(LastPublishedFileName);
        //        //een alle pagina's opnieuw publiceren, zodat er verwijzing is naar juiste master.
        //        BaseCollection<CmsPage> pages = BaseCollection<CmsPage>.Get("FK_Layout = '" + this.ID + "'");
        //        foreach (CmsPage page in pages)
        //        {
        //            page.Publish();
        //        }
        //    }
        //    base.SaveLastPublishInfo("Template", fileName);

        //    //base.SavePublishInfo();
        //}
    }
}
