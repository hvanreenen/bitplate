using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using HJORM;
using BitPlate.Domain;
using BitPlate.Domain.DataCollections;
using BitPlate.Domain.Autorisation;
using System.IO;
using Autofac;
using elFinder.Connector.Config;
using elFinder.Connector;
using BitPlate.Domain.Licenses;
using BitPlate.Domain.Menu;

namespace BitSite._bitPlate.Menus
{
    public partial class MenuItems : BasePage
    {
        private CmsMenu menu = null;
        
        private BaseCollection<CmsLanguage> siteLanguages = null;

        private static IContainer _container; //Elfinder container;

        protected void Page_Load(object sender, EventArgs e)
        {

            base.CheckLoginAndLicense();

            base.CheckPermissions(FunctionalityEnum.MenuManager);


            SelectMenu();

            fillMenuPages();

            siteLanguages = SessionObject.CurrentSite.Languages;

            

            
            //            LiteralJsScript.Text = String.Format(@"<script type='text/javascript'>
            //        BITIMAGESPOPUP.siteDomain = '{0}';
            //</script>", SessionObject.CurrentSite.DomainName);

            
            try
            {
                /************************************ELFINDER CONFIG*************************************/
                AppConnectorConfig elConfig = new AppConnectorConfig();
                elConfig.BaseThumbsUrl = SessionObject.CurrentSite.DomainName + "/_temp/_thumb/";
                elConfig.BaseUrl = SessionObject.CurrentSite.DomainName + "/_files/";
                elConfig.DefaultVolumeName = "LocalFileSystem";
                elConfig.DuplicateDirectoryPattern = "Copy of {0}";
                elConfig.DuplicateFilePattern = "Copy of {0}";
                elConfig.ThumbsSize = new System.Drawing.Size(48, 48);
                elConfig.UploadMaxSize = "20M";
                string BitplatePath = SessionObject.CurrentSite.Path;
                elConfig.LocalFSRootDirectoryPath = BitplatePath + "_files";
                elConfig.LocalFSThumbsDirectoryPath = BitplatePath + "_temp\\_thumb";
                elConfig.RootDirectoryName = "Root";
                elConfig.EnableAutoScaleImages = (SessionObject.CurrentSite.MaxWidthImages > 0);
                elConfig.AutoScaleImagesWidth = SessionObject.CurrentSite.MaxWidthImages;
                elFinder.Connector.Config.AppConnectorConfig.Instance = elConfig;

                if (!Directory.Exists(BitplatePath + "_temp"))
                {
                    Directory.CreateDirectory(BitplatePath + "_temp");
                }

                if (!Directory.Exists(BitplatePath + "_temp\\_thumb"))
                {
                    Directory.CreateDirectory(BitplatePath + "_temp\\_thumb");
                }
            }
            catch (Exception ex)
            {
            }

            // register IoC
            var builder = new ContainerBuilder();
            builder.RegisterElFinderConnector();
            _container = builder.Build();
            // need also to set container in elFinder module
            _container.SetAsElFinderResolver();
            /************************************END ELFINDER CONFIG*********************************/
        }

        private void SelectMenu()
        {
            string id = Request.QueryString["menuid"];
            menu = BaseObject.GetById<CmsMenu>(new Guid(id));
            //if (menu.HasAutorisation)
            //{

            //    if (!menu.IsAutorized(SessionObject.CurrentBitplateUser))
            //    {
            //        throw new Exception("U heeft geen rechten voor deze datacollectie.");
            //    }
            //}
        }

        private void fillMenuPages()
        {
            selectPages.Items.Clear();
            BaseCollection<CmsPage> pages = SessionObject.CurrentSite.Pages;
            foreach (CmsPage page in pages)
            {
                selectPages.Items.Add(new ListItem(page.RelativeUrl, page.ID.ToString()));
                
            }
        }
 

    }
}