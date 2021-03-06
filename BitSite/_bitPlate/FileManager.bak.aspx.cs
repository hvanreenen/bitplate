﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BitPlate.Domain.Autorisation;
using BitSite._bitPlate.bitMasterPages;

namespace BitSite._bitPlate
{
    public partial class FileManager : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.CheckLoginAndLicense();
            base.CheckPermissions(BitPlate.Domain.Autorisation.FunctionalityEnum.FileManager);

            if (Request.QueryString["Referer"] != null)
            {
                this.BackLink.HRef = Request.QueryString["Referer"];
                if (Request.QueryString["Template"] != null)
                {
                    this.BackLink.HRef += "?Template";
                }
                if (Request.QueryString["id"] != null)
                {
                    this.BackLink.HRef += "#" + Request.QueryString["id"];
                }
            }

            tumbnailWith.Value = SessionObject.CurrentSite.MaxWidthThumbnails.ToString();


            if (!SessionObject.HasPermission(FunctionalityEnum.FileManagerUpload))
            {
                liFileUpload.Disabled = true;
                aFileUpload.HRef = "#";
            }
            if (!SessionObject.HasPermission(FunctionalityEnum.FileManagerDownload))
            {
                liFileDownload.Disabled = true;
                aFileDownload.HRef = "#";
            }
            if (!SessionObject.HasPermission(FunctionalityEnum.FileManagerRename))
            {
                liFileRename.Disabled = true;
                aFileRename.HRef = "#";
            }
            if (!SessionObject.HasPermission(FunctionalityEnum.FileManagerCreateThumbnail))
            {
                liCreateThumb.Disabled = true;
                aCreateThumb.HRef = "#";
            }
            if (!SessionObject.HasPermission(FunctionalityEnum.FileManagerCCP))
            {
                liFileCut.Disabled = true;
                aFileCut.HRef = "#";
                liFileCopy.Disabled = true;
                aFileCopy.HRef = "#";
                liFilePaste.Disabled = true;
                aFilePaste.HRef = "#";
            }

        }
    }
}