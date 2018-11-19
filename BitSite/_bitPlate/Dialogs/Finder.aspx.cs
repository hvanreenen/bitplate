using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

using BitSite._bitPlate.bitAjaxServices;
using elFinder.Connector.Config;
using elFinder.Connector;
using Autofac;

namespace BitSite._bitPlate.bitDetails
{
    public partial class ElFiles : BasePage
    {
        private static IContainer _container;

        protected void Page_Load(object sender, EventArgs e)
        {
            base.CheckLoginAndLicense();

            /* <elFinder apiVersion="2.0" 
             * localFSRootDirectoryPath="C:\Users\emiel\Documents\Visual Studio 2012\Projects\Bitplate\Source\bitplate2.0\BitSite\_files" 
             * localFSThumbsDirectoryPath="C:\Users\emiel\Documents\Visual Studio 2012\Projects\Bitplate\Source\bitplate2.0\BitSite\_files\thumbs\"
             * rootDirectoryName="Root" 
             * uploadMaxSize="20M"
          duplicateFilePattern="Copy of {0}"
          duplicateDirectoryPattern="Copy of {0}"
            thumbsSize="48,48"
          defaultVolumeName="LocalFileSystem" 
             * baseUrl="http://localhost:49922/_files/" 
             * baseThumbsUrl="http://localhost:49922/_files/thumbs/">
  </elFinder>*/
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

            // register IoC
            var builder = new ContainerBuilder();
            builder.RegisterElFinderConnector();
            _container = builder.Build();
            // need also to set container in elFinder module
            _container.SetAsElFinderResolver();
        }
    }
}