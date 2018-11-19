using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

using BitSite._bitPlate.FileManager;

namespace BitSite._bitPlate.Dialogs
{
    public partial class Images : System.Web.UI.Page
    {
        
        //string imagesRoot = "_files\\_img";
        string imagesRoot = "_files";
        protected void Page_Load(object sender, EventArgs e)
        {
            using (FileService fileService = new FileService())
            {
                LiteralImages.Text = fileService.GetImagesAndSubFolders(imagesRoot);
            }
        }
    }
}