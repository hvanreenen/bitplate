using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using System.IO;

using HJORM;
using BitPlate.Domain;
using BitPlate.Domain.Modules;
using Ionic.Zip;
using Ionic;
using BitPlate.Domain.Utils;

namespace BitSite._bitPlate.FileManager
{
    [System.Web.Script.Services.ScriptService]
    public partial class FileService : BaseService
    {
        static string basePath = "";
        static string fileRoot = "";
        static List<HttpPostedFile> files = new List<HttpPostedFile>();
        static string CurrentFolder = "";
        protected void Page_Load(object sender, EventArgs e)
        {

            if (Request.QueryString["download"] != null)
            {
                string[] files = Request.QueryString["download"].Split(new char[] { ',' });
                string folder = Request.QueryString["downloadfolder"].Replace("//", ""); //BUG #105 FIX: .Replace("//", "");
                string FileToDownload = "";
                //string file = files[files.Length - 1];
                if (files.Length > 1 || Directory.Exists(getSiteRoot() + folder + @"\" + files[0])) //BUG #105 FIX: || Directory.Exists(getSiteRoot() + folder + @"\" + files[0])
                {
                    ZipFile zip = new ZipFile();
                    foreach (string file in files)
                    {
                        string fileOrDir = getSiteRoot() + folder + @"\" + file;
                        if (Directory.Exists(fileOrDir))
                        {
                            DirectoryInfo di = new DirectoryInfo(fileOrDir);
                            zip.AddDirectory(fileOrDir, di.Name);
                        }
                        else
                        {
                            zip.AddFile(fileOrDir, "\\");
                        }
                    }
                    string fileName = "download_" + DateTime.Now.ToShortDateString() + ".zip";
                    if (!Directory.Exists(getSiteRoot() + @"_temp\"))
                    {
                        Directory.CreateDirectory(getSiteRoot() + @"_temp\");
                    }
                    string filePath = getSiteRoot() + @"_temp\" + fileName;
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }

                    zip.Save(filePath);
                    FileToDownload = fileName;
                    folder = "_temp";
                    zip.Dispose();
                }
                else
                {
                    FileToDownload = files[files.Length - 1];
                }

                Response.Clear();
                if (FileToDownload != "")
                {
                    Response.AppendHeader("Content-Disposition", "attachment; filename=" + FileToDownload);
                    string url = getSiteRoot() + folder + "\\" + FileToDownload;
                    Response.AppendHeader("Content-Length", new FileInfo(getSiteRoot() + folder + "\\" + FileToDownload).Length.ToString());
                    Response.ContentType = "application/octet-stream";
                    //Response.TransmitFile(getSiteRoot() + folder + "\\" + file);
                    Response.WriteFile(getSiteRoot() + folder + "\\" + FileToDownload);
                    //Response.BinaryWrite(byteArray);
                }
                Response.Flush();
                Response.End();
            }
            else if (Request.Files.Count > 0)
            {
                HttpFileCollection files = Request.Files;
                string FileRecord = "<table>";
                string[] arr1 = files.AllKeys;  // This will get names of all files into a string array.
                List<BitFile> FileList = new List<BitFile>();
                for (int loop1 = 0; loop1 < arr1.Length; loop1++)
                {

                    BitFile bf = new BitFile();
                    bf.name = files[loop1].FileName;
                    bf.size = files[loop1].ContentLength;
                    bf.thumbnail_url = "";
                    bf.url = "";
                    bf.delete_url = "";
                    bf.delete_type = "DELETE";
                    FileList.Add(bf);

                    files[loop1].SaveAs(CurrentFolder + "\\" + files[loop1].FileName);
                }

                System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                string sJSON = oSerializer.Serialize(FileList);

                Response.Write(sJSON);
                //CLEAR
                //FileService.files.Clear();
                Response.Flush();
                Response.End();
            }
        }

        public class BitFile
        {
            public string name { get; set; }
            public int size { get; set; }
            public string url { get; set; }
            public string thumbnail_url { get; set; }
            public string delete_url { get; set; }
            public string delete_type { get; set; }
        }

        private string getSiteRoot()
        {
            return SessionObject.CurrentSite.Path; // AppDomain.CurrentDomain.BaseDirectory;
            //return AppDomain.CurrentDomain.BaseDirectory + "\\";
        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetImagesAndSubFolders(string folder)
        {
            BaseService.CheckLoginAndLicense();
            folder = folder.Replace("/", "\\");
            basePath = SessionObject.CurrentSite.Path;//AppDomain.CurrentDomain.BaseDirectory;
            //fileRoot = basePath + "_files\\_img";
            fileRoot = basePath + "_files";
            folder = basePath + folder;
            string html = "";
            if (folder != fileRoot)
            {
                //navigate upwards
                string parentFolder = Directory.GetParent(folder).FullName + "\\";

                parentFolder = parentFolder.Replace(basePath, "");
                if (parentFolder.EndsWith("\\"))
                {
                    parentFolder = parentFolder.Substring(0, parentFolder.Length - 1);
                }
                string relativeFolder = parentFolder.Replace("\\", "/");
                html += String.Format(@"<div class=""bitFolder"" title=""{0}"" onclick=""BITIMAGESPOPUP.getImagesAndSubFolders('{0}');"">...</div>", relativeFolder);
                //html += String.Format(@"<div class=""bitFolder"" title=""{0}"">...</div>", relativeFolder);
            }
            if (folder.EndsWith(@"\_img") && !Directory.Exists(folder))
            {                //BUG #72
                Directory.CreateDirectory(folder);
            }
            string[] subFolders = Directory.GetDirectories(folder);
            foreach (string subFolder in subFolders)
            {
                string folderName = Path.GetFileName(subFolder);
                string relativeFolder = subFolder.Replace(basePath, "");
                relativeFolder = relativeFolder.Replace("\\", "/");
                html += String.Format(@"<div class=""bitFolder"" title=""{0}"" onclick=""BITIMAGESPOPUP.getImagesAndSubFolders('{0}');"">{1}</div>", relativeFolder, folderName);
                //html += String.Format(@"<div class=""bitFolder"" title=""{0}"">{1}</div>", relativeFolder, folderName);

            }

            string[] files = Directory.GetFiles(folder, "*.jpg");
            List<string> list = new List<string>(files);
            list.AddRange(Directory.GetFiles(folder, "*.png"));
            list.AddRange(Directory.GetFiles(folder, "*.gif"));
            list.Sort();
            foreach (string file in list)
            {
                string imgUrl = file.Replace(basePath, "");
                string fileName = Path.GetFileName(file);
                imgUrl = imgUrl.Replace("\\", "/");
                html += String.Format(@"<div class=""bitImage""><img src='{0}/{1}' /></div>{2}<br/>", SessionObject.CurrentSite.DomainName, imgUrl, fileName);
            }
            return html;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetFilesAndSubFolders(string folder)
        {
            BaseService.CheckLoginAndLicense();
            folder = folder.Replace("/", "\\");
            basePath = SessionObject.CurrentSite.Path; //AppDomain.CurrentDomain.BaseDirectory;
            fileRoot = basePath + "_files";
            folder = basePath + folder;
            string html = "";
            if (folder != fileRoot)
            {
                //navigate upwards
                string parentFolder = Directory.GetParent(folder).FullName + "\\";

                parentFolder = parentFolder.Replace(basePath, "");
                if (parentFolder.EndsWith("\\"))
                {
                    parentFolder = parentFolder.Substring(0, parentFolder.Length - 1);
                }
                string relativeFolder = parentFolder.Replace("\\", "/");
                html += String.Format(@"<div class=""bitFolder"" title=""{0}"" onclick=""BITFILESPOPUP.getFilesAndSubFolders('{0}');"">...</div>", relativeFolder);
                //html += String.Format(@"<div class=""bitFolder"" title=""{0}"">...</div>", relativeFolder);
            }
            string[] subFolders = Directory.GetDirectories(folder);
            foreach (string subFolder in subFolders)
            {
                string folderName = Path.GetFileName(subFolder);
                string relativeFolder = subFolder.Replace(basePath, "");
                relativeFolder = relativeFolder.Replace("\\", "/");
                html += String.Format(@"<div class=""bitFolder"" title=""{0}"" onclick=""BITFILESPOPUP.getFilesAndSubFolders('{0}');"">{1}</div>", relativeFolder, folderName);
                //html += String.Format(@"<div class=""bitFolder"" title=""{0}"">{1}</div>", relativeFolder, folderName);

            }

            string[] files = Directory.GetFiles(folder, "*.*");
            List<string> list = new List<string>(files);
            list.Sort();
            foreach (string file in list)
            {
                string imgUrl = file.Replace(basePath, "");
                string fileName = Path.GetFileName(file);
                imgUrl = imgUrl.Replace("\\", "/");
                html += String.Format(@"<div class=""bitFile""><img src='{0}/{1}' /></div>{2}<br/>", SessionObject.CurrentSite.DomainName, imgUrl, fileName);
            }

            return html;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<TreeGridItem> GetFiles(string folder, string sort)
        {
            BaseService.CheckLoginAndLicense();
            folder = folder.Replace("/", "\\").Replace("\\\\", "\\");
            folder = (getSiteRoot() + folder).Replace("/", "\\").Replace("\\\\", "\\"); ;
            CurrentFolder = folder;
            List<CmsFile> returnFileList = new List<CmsFile>();
            List<CmsDirectory> returnDirectoryList = new List<CmsDirectory>();
            List<TreeGridItem> returnList = new List<TreeGridItem>();

            if (folder.ToLower().EndsWith("_files"))
            {
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
            }

            string[] files = Directory.GetFiles(folder);
            string[] directories = Directory.GetDirectories(folder);
            foreach (string file in files)
            {
                FileInfo fileInfoObject = new FileInfo(file);
                CmsFile fileObject = new CmsFile(fileInfoObject, SessionObject.CurrentSite, SessionObject.CurrentBitplateUser.Theme);

                returnFileList.Add(fileObject);
            }
            foreach (string directory in directories)
            {
                DirectoryInfo directoryInfoObject = new DirectoryInfo(directory);
                CmsDirectory directoryObject = new CmsDirectory(directoryInfoObject);
                returnDirectoryList.Add(directoryObject);
            }
            string sortOrder = "ASC";
            if (sort.Contains(" DESC"))
            {
                sortOrder = "DESC";
            }
            string sortField = sort.Replace(" ASC", "");
            sortField = sortField.Replace(" DESC", "");
            Sort(sortField, sortOrder, returnFileList);
            SortFolders(sortField, sortOrder, returnDirectoryList);
            //Mappen sorteren.
            returnDirectoryList.ForEach(c => returnList.Add(new TreeGridItem(c)));
            returnFileList.ForEach(c => returnList.Add(new TreeGridItem(c)));


            return returnList;
        }

        private static void Sort(string sortField, string sortOrder, List<CmsFile> returnList)
        {
            if (sortOrder == "" || sortOrder.ToUpper() == "ASC")
            {
                if (sortField == "Name")
                {
                    returnList.Sort(delegate(CmsFile f1, CmsFile f2) { return f1.Name.CompareTo(f2.Name); });
                }
                else if (sortField == "FileType")
                {
                    returnList.Sort(delegate(CmsFile f1, CmsFile f2) { return f1.FileType.CompareTo(f2.FileType); });
                }
                else if (sortField == "Volume")
                {
                    returnList.Sort(delegate(CmsFile f1, CmsFile f2) { return f1.Volume.CompareTo(f2.Volume); });
                }
                else if (sortField == "CreateDate")
                {
                    returnList.Sort(delegate(CmsFile f1, CmsFile f2) { return f1.CreateDate.CompareTo(f2.CreateDate); });
                }
                else if (sortField == "LastModifiedDate")
                {
                    returnList.Sort(delegate(CmsFile f1, CmsFile f2) { return f1.ModifiedDate.CompareTo(f2.ModifiedDate); });
                }
            }
            else
            {
                if (sortField == "Name")
                {
                    returnList.Sort(delegate(CmsFile f1, CmsFile f2) { return f2.Name.CompareTo(f1.Name); });
                }
                else if (sortField == "FileType")
                {
                    returnList.Sort(delegate(CmsFile f1, CmsFile f2) { return f2.FileType.CompareTo(f1.FileType); });
                }
                else if (sortField == "Volume")
                {
                    returnList.Sort(delegate(CmsFile f1, CmsFile f2) { return f2.Volume.CompareTo(f1.Volume); });
                }
                else if (sortField == "CreateDate")
                {
                    returnList.Sort(delegate(CmsFile f1, CmsFile f2) { return f2.CreateDate.CompareTo(f1.CreateDate); });
                }
                else if (sortField == "LastModifiedDate")
                {
                    returnList.Sort(delegate(CmsFile f1, CmsFile f2) { return f2.ModifiedDate.CompareTo(f1.ModifiedDate); });
                }
            }
        }

        private static void SortFolders(string sortField, string sortOrder, List<CmsDirectory> returnList)
        {
            if (sortOrder == "" || sortOrder.ToUpper() == "ASC")
            {
                if (sortField == "Name")
                {
                    returnList.Sort(delegate(CmsDirectory f1, CmsDirectory f2) { return f1.Name.CompareTo(f2.Name); });
                }
                else if (sortField == "CreateDate")
                {
                    returnList.Sort(delegate(CmsDirectory f1, CmsDirectory f2) { return f1.CreateDate.CompareTo(f2.CreateDate); });
                }
                else if (sortField == "LastModifiedDate")
                {
                    returnList.Sort(delegate(CmsDirectory f1, CmsDirectory f2) { return f1.ModifiedDate.CompareTo(f2.ModifiedDate); });
                }
            }
            else
            {
                if (sortField == "Name")
                {
                    returnList.Sort(delegate(CmsDirectory f1, CmsDirectory f2) { return f2.Name.CompareTo(f1.Name); });
                }
                else if (sortField == "CreateDate")
                {
                    returnList.Sort(delegate(CmsDirectory f1, CmsDirectory f2) { return f2.CreateDate.CompareTo(f1.CreateDate); });
                }
                else if (sortField == "LastModifiedDate")
                {
                    returnList.Sort(delegate(CmsDirectory f1, CmsDirectory f2) { return f2.ModifiedDate.CompareTo(f1.ModifiedDate); });
                }
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void AddFolder(string parentFolder, string folderName)
        {
            BaseService.CheckLoginAndLicense();
            if (parentFolder == "")
            {
                parentFolder = SessionObject.CurrentSite.Path + "_FILES";
            }
            else
            {
                parentFolder = (getSiteRoot() + parentFolder).Replace("/", "\\").Replace("\\\\", "\\");
            }
            Directory.CreateDirectory(parentFolder + "\\" + folderName);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void DeleteFolder(string folderName)
        {
            BaseService.CheckLoginAndLicense();
            if (folderName != "" && !folderName.EndsWith("_files") && !folderName.EndsWith("_files\\"))
            {
                folderName = getSiteRoot() + folderName;
                Directory.Delete(folderName, true);
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void DeleteFiles(string folderName, string[] files)
        {
            BaseService.CheckLoginAndLicense();
            if (folderName != "")
            {
                folderName = folderName.Replace("/", "\\").Replace("\\\\", "\\") + "\\";
                folderName = (getSiteRoot() + folderName).Replace("\\\\", "\\");
                foreach (string file in files)
                {
                    string FilePath = folderName + file;
                    if (Directory.Exists(FilePath))
                    {
                        Directory.Delete(FilePath, true);
                    }

                    if (File.Exists(FilePath))
                    {
                        File.Delete(FilePath);
                    }
                }
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void CopyFiles(string oldFolderName, string newFolderName, string[] files)
        {
            BaseService.CheckLoginAndLicense();
            if (oldFolderName != "")
            {
                oldFolderName = getSiteRoot() + oldFolderName.Replace("/", "\\").Replace("\\\\", "\\") + "\\"; ;
                newFolderName = getSiteRoot() + newFolderName.Replace("/", "\\").Replace("\\\\", "\\") + "\\"; ;
                foreach (string file in files)
                {
                    if (Directory.Exists(oldFolderName + "\\" + file)) //BUG #106 FIX: if (Directory.Exists(oldFolderName + "\\" + file)), Foldercopy
                    {
                        if (oldFolderName == newFolderName) 
                        {
                            FileHelper.CopyDirectory(oldFolderName + "\\" + file, newFolderName + "\\" + file + " (kopie)");
                        }
                        else
                        {
                            FileHelper.CopyDirectory(oldFolderName + "\\" + file, newFolderName + "\\" + file);
                        }
                        
                    }
                    else
                    {
                        FileInfo fileInfoObject = new FileInfo(oldFolderName + "\\" + file);
                        string filename = fileInfoObject.Name.Replace(fileInfoObject.Extension, "");
                        int index = 1;
                        string newFileName = String.Format("{0} ({1}){2}", filename, index, fileInfoObject.Extension);
                        while (File.Exists(newFolderName + "\\" + newFileName))
                        {
                            index++;
                            newFileName = String.Format("{0} ({1}){2}", filename, index, fileInfoObject.Extension);
                        }
                        fileInfoObject.CopyTo(newFolderName + "\\" + newFileName);
                    }
                }
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void MoveFiles(string oldFolderName, string newFolderName, string[] files)
        {
            BaseService.CheckLoginAndLicense();
            if (newFolderName != "")
            {

                oldFolderName = getSiteRoot() + oldFolderName.Replace("/", "\\").Replace("\\\\", "\\") + "\\";
                newFolderName = getSiteRoot() + newFolderName.Replace("/", "\\").Replace("\\\\", "\\") + "\\";
                string exMsg = "";
                foreach (string file in files)
                {
                    try
                    {
                        if (Directory.Exists(oldFolderName + "\\" + file)) //BUG #106 FIX: if (Directory.Exists(oldFolderName + "\\" + file)), Foldercopy
                        {
                            if (oldFolderName == newFolderName)
                            {
                                FileHelper.CopyDirectory(oldFolderName + "\\" + file, newFolderName + "\\" + file + " (kopie)");
                            }
                            else
                            {
                                FileHelper.CopyDirectory(oldFolderName + "\\" + file, newFolderName + "\\" + file);
                            }
                            FileHelper.DeleteDir(oldFolderName + "\\" + file);

                        }
                        else
                        {
                            File.Move(oldFolderName + "\\" + file, newFolderName + "\\" + file);
                        }
                    }
                    catch (IOException ex)
                    {
                        exMsg += file + ", ";
                    }
                }
                if (exMsg != "")
                {
                    exMsg = exMsg.Substring(0, exMsg.Length - 2);
                    throw new Exception("Kon sommige bestanden niet verplaatsen: " + exMsg);
                }
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void RenameFile(string folder, string oldFileName, string newFileName)
        {
            BaseService.CheckLoginAndLicense();
            if (newFileName != "")
            {
                folder = getSiteRoot() + folder.Replace("/", "\\").Replace("\\\\", "\\") + "\\";

                try
                {
                    if (!oldFileName.ToLower().EndsWith("_img")) //BUG #104 FIX: if (!oldFileName.ToLower().EndsWith("_img"))
                    {
                        if (File.Exists(folder + oldFileName))
                        {
                            File.Move(folder + oldFileName, folder + newFileName);
                        }

                        if (Directory.Exists(folder + oldFileName))
                        {
                            Directory.Move(folder + oldFileName, folder + newFileName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Kon bestand niet hernoemen.");
                }
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void RenameFolder(string CurrenFolderLocation, string NewFolderLokation)
        {
            BaseService.CheckLoginAndLicense();
            if (NewFolderLokation != "" && !NewFolderLokation.ToLower().EndsWith("_files"))
            {
                CurrenFolderLocation = (getSiteRoot() + CurrenFolderLocation).Replace("\\\\", "\\");
                NewFolderLokation = (getSiteRoot() + NewFolderLokation).Replace("\\\\", "\\");

                try
                {
                    Directory.Move(CurrenFolderLocation, NewFolderLokation);
                }
                catch (Exception ex)
                {
                    throw new Exception("Kon map niet hernoemen.");
                }

            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void SaveFiles(string folder)
        {
            BaseService.CheckLoginAndLicense();
            Dictionary<string, bool> Unzip = new Dictionary<string, bool>();

            foreach (HttpPostedFile file in FileService.files)
            {
                string filePath = getSiteRoot() + folder + "\\" + file.FileName;
                file.SaveAs(filePath);

                FileInfo fi = new FileInfo(filePath);
                if (fi.Extension == ".zip" && ZipFile.IsZipFile(filePath) && fi.Name.Contains("unzip_"))
                {
                    ZipFile zipFile = ZipFile.Read(filePath);
                    zipFile.ExtractAll(getSiteRoot() + folder);
                    zipFile.Dispose();
                }
            }
            FileService.files.Clear();
        }

        private static string treeHtml;
        private static int nodeCounter = 1;
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string BuildTree(string searchString)
        {
            BaseService.CheckLoginAndLicense();
            nodeCounter = 1;
            treeHtml = "";
            string root = SessionObject.CurrentSite.Path + "\\_files";
            buildGroup(root);
            //buildGroup(SessionObject.CurrentSite.Path );
            treeHtml += "</ul>";
            treeHtml = "<ul><li id=\"node0\"><a data-path=\"//_FILES\" href=#>\\</a>" + treeHtml + "</ul>";
            return treeHtml;
        }

        
        private static void buildGroup(string folder)
        {
            try
            {
                if (treeHtml == "")
                {
                    treeHtml += "<ul>";
                }

                string[] subFolders = System.IO.Directory.GetDirectories(folder);

                foreach (string subfolder in subFolders)
                {
                    string displayString = "", relativeFolder = "";
                    if (folder != null)
                    {
                        displayString = System.IO.Path.GetFileName(subfolder);
                        relativeFolder = subfolder.Replace(SessionObject.CurrentSite.Path, "");
                    }
                    bool subFolderHasSubFolders = (System.IO.Directory.GetDirectories(subfolder).Length > 0);
                    if (subFolderHasSubFolders)
                    {
                        //treeHtml += String.Format("<li class='treeGroup'><span class='openCloseIcon'>&#9660;</span><span class='groupIcon'><img src='{2}' alt=''/></span><span class='groupText'><a href='javascript:BITFILEMANAGEMENT.getFiles(\"{0}\")'>{1}</a></span><ul>", relativeFolder.Replace("\\", "\\\\"), displayString, _groupIcon);
                        //treeHtml += String.Format("<li class='treeGroup'><span class='openCloseIcon'>&#9660;</span><span class='groupIcon'><img src='{2}' alt=''/></span><span class='groupText'><a class='folder' id='{0}'>{1}</a></span><ul>", relativeFolder.Replace("\\", "\\\\"), displayString, "");

                        treeHtml += "<li id=\"node" + nodeCounter + "\" class=\"jstree-drop jstree-drag\"><a href=\"#\" data-path=\"" + relativeFolder.Replace("\\", "//") + "\">" + displayString + "</a><ul>";

                        buildGroup(subfolder);
                        treeHtml += "</ul></li>";
                    }
                    else
                    {
                        //treeHtml += String.Format("<li  class='treeGroup'><span class='groupIcon'><img src='{2}' alt=''/></span><span class='groupText'><a href='javascript:BITFILEMANAGEMENT.getFiles(\"{0}\")'>{1}</a></span></li>", relativeFolder.Replace("\\", "\\\\"), displayString, _groupIcon);
                        //treeHtml += String.Format("<li  class='treeGroup'><span class='groupIcon'><img src='{2}' alt=''/></span><span class='groupText'><a class='folder' id='{0}'>{1}</a></span></li>", relativeFolder.Replace("\\", "\\\\"), displayString, "");
                        treeHtml += "<li id=\"node" + nodeCounter + "\" class=\"jstree-drop jstree-drag\"><a href=\"#\" data-path=\"" + relativeFolder.Replace("\\", "//") + "\">" + displayString + "</a></li>";
                    }
                    nodeCounter++;
                }
            }
            catch
            {
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void MoveFolders(string targetFolder, string[] sourceFolders)
        {
            BaseService.CheckLoginAndLicense();
            foreach (string SourceFolder in sourceFolders)
            {
                string TargetFolder = SessionObject.CurrentSite.Path + targetFolder + SourceFolder.Substring(SourceFolder.LastIndexOf("\\"));
                string RealSourceFolder = SessionObject.CurrentSite.Path + SourceFolder;
                Directory.Move(RealSourceFolder, TargetFolder);
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string DragMoveFiles(string targetFolder, string[] sourceFiles)
        {
            BaseService.CheckLoginAndLicense();
            string errMessage = "";
            foreach (string SourceFile in sourceFiles)
            {
                try
                {
                    string TargetFolder = SessionObject.CurrentSite.Path + targetFolder + SourceFile.Substring(SourceFile.LastIndexOf("\\"));
                    string RealSourceFolder = SessionObject.CurrentSite.Path + SourceFile;
                    File.Move(RealSourceFolder, TargetFolder);
                }
                catch (Exception ex)
                {
                    errMessage = "Kon niet alle bestanden verplaatsen.";
                }

            }
            return errMessage;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public Dictionary<string, object> OpenFile(string filePath)
        {
            BaseService.CheckLoginAndLicense();

            string absoluteFilePath = SessionObject.CurrentSite.Path + '/' + filePath;
            absoluteFilePath = absoluteFilePath.Replace("../", "").Replace("/", "\\");

            string relativeFilePath = SessionObject.CurrentSite.DomainName + filePath;

            Dictionary<string, object> FileObject = new Dictionary<string, object>();
            FileInfo fi = new FileInfo(absoluteFilePath);
            FileObject.Add("Extension", fi.Extension);

            switch (fi.Extension.ToLower())
            {
                case ".html":
                case ".htm":
                case ".txt":
                case ".ini":
                case ".php":
                case ".cs":
                case ".js":
                case ".css":
                    StreamReader sr = new StreamReader(fi.FullName);
                    FileObject.Add("Value", sr.ReadToEnd());
                    sr.Close();
                    break;

                case ".jpeg":
                case ".jpg":
                case ".png":
                case ".gif":
                    FileObject.Add("Value", relativeFilePath);
                    break;

                case ".pdf":
                    FileObject.Add("Value", relativeFilePath);
                    break;
            }
            return FileObject;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void CreateThumbnails(int imageCount, int imageHeight, int imageWidth, CmsImage[] images, string currentFolder, bool cropEnabled)
        {
            BaseService.CheckLoginAndLicense();
            //string response = "";
            if (imageCount <= 0)
            {
                //WriteResponse("");
                return;
            }
            try
            {
                currentFolder = currentFolder.Replace("/", "\\");
                foreach (CmsImage file in images)
                {
                    string basePath = getSiteRoot() + currentFolder + "\\" + file.name;
                    basePath = basePath.Replace("\\\\", "\\");
                    FileInfo fi = new FileInfo(basePath);
                    string newfilename = getSiteRoot() + currentFolder + "\\" + fi.Name.Replace(fi.Extension, "") + "_thumb" + fi.Extension;
                    newfilename = newfilename.Replace("\\\\", "\\");
                    if (cropEnabled)
                    {
                        ImageHelper.CreateCroppedThumbnail(basePath, file);
                    }
                    else
                    {
                        ImageHelper.CreateThumbnail(basePath, file.width); 
                    }
                }
                //response = GetFilesTable(folder);

            }
            catch (Exception ex)
            {
                //Response.StatusCode = 500; //error
                //response = ex.Message;
            }
            finally
            {
                //WriteResponse(response);
            }


        }
    }
}