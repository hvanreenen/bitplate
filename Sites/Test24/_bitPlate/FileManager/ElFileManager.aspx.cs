using Autofac;
using elFinder.Connector;
using elFinder.Connector.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BitPlate.Domain;
using System.Diagnostics;

namespace BitSite._bitPlate.FileManager
{
    public partial class ElFileManager : BasePage
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
            CmsSiteEnvironment CurrentEnvironment = SessionObject.CurrentSite.CurrentWorkingEnvironment;
            string Url = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
            elConfig.BaseThumbsUrl = Url + "/_temp/_thumb/";
            elConfig.BaseUrl = Url + "/_files/";
            elConfig.DefaultVolumeName = "LocalFileSystem";
            elConfig.DuplicateDirectoryPattern = "Copy of {0}";
            elConfig.DuplicateFilePattern = "Copy of {0}";
            elConfig.ThumbsSize = new System.Drawing.Size(48, 48);
            elConfig.UploadMaxSize = "128M";
            string BitplatePath = (!SessionObject.CurrentSite.Path.EndsWith("\\")) ? SessionObject.CurrentSite.Path + "\\" : SessionObject.CurrentSite.Path;
            elConfig.LocalFSRootDirectoryPath = BitplatePath + "_files";
            elConfig.LocalFSThumbsDirectoryPath = BitplatePath + "_temp\\_thumb";
            elConfig.RootDirectoryName = "Root";

            if (!Directory.Exists(BitplatePath + "_temp"))
            {
                Directory.CreateDirectory(BitplatePath + "_temp");
            }

            if (!Directory.Exists(BitplatePath + "_temp\\_thumb"))
            {
                Directory.CreateDirectory(BitplatePath + "_temp\\_thumb");
            }

            if (!Directory.Exists(BitplatePath + "_files"))
            {
                Directory.CreateDirectory(BitplatePath + "_files");
            }

            /* if (!Directory.Exists(BitplatePath + "_css"))
            {
                Directory.CreateDirectory(BitplatePath + "_css");
            }

            if (!Directory.Exists(BitplatePath + "_css\\_moduleStyles"))
            {
                Directory.CreateDirectory(BitplatePath + "_css\\_moduleStyles");
            }

            if (!Directory.Exists(BitplatePath + "_js"))
            {
                Directory.CreateDirectory(BitplatePath + "_js");
            }

            if (!Directory.Exists(BitplatePath + "_js\\moduleScripts"))
            {
                Directory.CreateDirectory(BitplatePath + "_js\\moduleScripts");
            }

            if (!Directory.Exists(BitplatePath + "_files\\_css"))
            {
                //Directory.CreateDirectory(BitplatePath + "_files");
                Process.Start("mklink /D \"" + BitplatePath + "_files\\_css\" \"" + BitplatePath + "\\_css\"");
            } */
            //Als MaxWithImages groter is dan 0 dan activeer autoscaler.
            elConfig.EnableAutoScaleImages = (SessionObject.CurrentSite.MaxWidthImages > 0);
            elConfig.AutoScaleImagesWidth = SessionObject.CurrentSite.MaxWidthImages;
            elFinder.Connector.Config.AppConnectorConfig.Instance = elConfig;

            // register IoC
            var builder = new ContainerBuilder();
            builder.RegisterElFinderConnector();
            _container = builder.Build();
            // need also to set container in elFinder module
            _container.SetAsElFinderResolver();

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
        }
    }
}