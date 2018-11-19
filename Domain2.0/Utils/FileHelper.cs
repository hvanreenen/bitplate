using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace BitPlate.Domain.Utils
{
    public static class FileHelper
    {
        public static FileInfo[] GetFilesOnServer(string path)
        {

            DirectoryInfo dir = new DirectoryInfo(path);

            return dir.GetFiles();


        }

        public static bool FileExist(string filename)
        {
            return File.Exists(filename);
        }

        public static bool DirectoryExist(string directoryname)
        {
            return Directory.Exists(directoryname);
        }

        public static void WriteFile(string fileName, string content)
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            
            TextWriter writer = File.CreateText(fileName);
            writer.Write(content);
            writer.Close();
            writer.Dispose(); //Posible FIX
        }

        public static string ReadFile(string fileName)
        {
            string content;
            TextReader reader = File.OpenText(fileName);
            content = reader.ReadToEnd();
            reader.Close();
            reader.Dispose();
            return content;
        }

        public static string CleanFileName(string fileName)
        {
            fileName = fileName.Replace(" ", "_");
            fileName = fileName.Replace("/", "");
            fileName = fileName.Replace("\\", "");
            fileName = fileName.Replace("?", "");
            fileName = fileName.Replace(":", "");
            fileName = fileName.Replace("<", "");
            fileName = fileName.Replace(">", "");
            fileName = fileName.Replace("&", "");
            fileName = fileName.Replace("\"", "");
            fileName = fileName.Replace("'", "");
            fileName = fileName.Replace(",", "");
            return fileName;
        }

        public static string GetRelativePath(string path)
        {
            string sitePath = AppDomain.CurrentDomain.BaseDirectory;

            string relativepath = path.Replace(AppDomain.CurrentDomain.BaseDirectory, "");
            relativepath = relativepath.Replace("\\", "/");
            return "../" + relativepath;
        }
        public static string GetIcon(FileInfo file)
        {
            string iconSrc = "../app_themes/theme1/img/file.jpg";
            if (file.Extension.ToLower() == ".pdf")
                iconSrc = "../app_themes/theme1/img/pdf.jpg";
            else if (file.Extension.ToLower() == ".doc")
                iconSrc = "../app_themes/theme1/img/doc.jpg";
            else if (file.Extension.ToLower() == ".xls")
                iconSrc = "../app_themes/theme1/img/xls.jpg";

            return iconSrc;
        }

        public static bool IsImage(FileInfo file)
        {
            return (file.Extension.ToLower() == ".jpg" ||
                file.Extension.ToLower() == ".gif" ||
                file.Extension.ToLower() == ".png");
        }

        public static bool IsImage(string filename)
        {
            return (filename.ToLower().EndsWith(".jpg") ||
                filename.ToLower().EndsWith(".gif") ||
                filename.ToLower().EndsWith(".png"));
        }

        public static void CopyDirectory(string source, string destination)
        {
            CopyDirectory(new DirectoryInfo(source), new DirectoryInfo(destination));
        }

        public static void CopyDirectory(DirectoryInfo source, DirectoryInfo destination)
        {
            if (!destination.Exists)
            {
                destination.Create();
            }

            // Copy all files.
            FileInfo[] files = source.GetFiles();
            foreach (FileInfo file in files)
            {
                file.CopyTo(Path.Combine(destination.FullName,
                    file.Name), true);
            }

            // Process subdirectories.
            DirectoryInfo[] dirs = source.GetDirectories();
            foreach (DirectoryInfo dir in dirs)
            {
                // Get destination directory.
                string destinationDir = Path.Combine(destination.FullName, dir.Name);

                // Call CopyDirectory() recursively.
                CopyDirectory(dir, new DirectoryInfo(destinationDir));
            }
        }

        public static void DeleteFile(string fileName)
        {
            if(File.Exists(fileName)){
                File.Delete(fileName);
            }
        }

        public static void DeleteFilesFromDirectory(string directoryPath, string pattern, bool includeSubdirectories)
        {
            if (!Directory.Exists(directoryPath))
            {
                return;
            }
            string[] files;
            if (!string.IsNullOrEmpty(pattern))
            {
                files = Directory.GetFiles(directoryPath, pattern);
            }
            else
            {
                files = Directory.GetFiles(directoryPath);
            }

            foreach (string file in files)
            {
                File.Delete(file);
            }

            if (includeSubdirectories)
            {
                string[] directories = Directory.GetDirectories(directoryPath);
                foreach (string dir in directories)
                {
                    DeleteFilesFromDirectory(dir, pattern, includeSubdirectories);
                }
            }
        }

        public static void EmptySiteDirectory(string directorypath, bool deleteFiles = false)
        {
            DirectoryInfo directory = new DirectoryInfo(directorypath);
            foreach (System.IO.FileInfo file in directory.GetFiles()) file.Delete();
            foreach (System.IO.DirectoryInfo subDirectory in directory.GetDirectories())
            {
                if (subDirectory.Name.ToLower() != "_files" || deleteFiles)
                {
                    if (!subDirectory.Name.ToLower().Contains("aspnet_client"))
                    {
                        subDirectory.Delete(true);
                    }
                }
            }
        }

        public static void CreateDirectory(string Path)
        {
            Directory.CreateDirectory(Path);
        }

        public static void DeleteDir(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }
        public static void CopyDir(string sourceFolder, string destFolder)
        {
            CopyDir(sourceFolder, destFolder, "*.*");
        }
        public static void CopyDir(string sourceFolder, string destFolder, string searchPattern, bool isReadOnly = false, bool includingSubDirectories = true)
        {
            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);
            if (!Directory.Exists(sourceFolder))
                Directory.CreateDirectory(sourceFolder);

            string[] files = DirectoryExtensions.GetFiles(sourceFolder, searchPattern, SearchOption.TopDirectoryOnly);//  Directory.GetFiles(, searchPattern);
            foreach (string file in files)
            {
                string name = Path.GetFileName(file);
                string dest = Path.Combine(destFolder, name);
                if (File.Exists(dest))
                {
                    FileInfo fi = new FileInfo(dest);
                    fi.IsReadOnly = isReadOnly;
                    fi.Refresh();
                }
                FileInfo ffi = new FileInfo(file);
                ffi.IsReadOnly = isReadOnly;
                ffi.Refresh();
                File.Copy(file, dest, true);
            }
            if (includingSubDirectories)
            {
                string[] folders = Directory.GetDirectories(sourceFolder);
                foreach (string folder in folders)
                {
                    string name = Path.GetFileName(folder);
                    string dest = Path.Combine(destFolder, name);
                    if (Directory.Exists(dest))
                    {
                        DirectoryInfo di = new DirectoryInfo(dest);
                        di.Attributes &= (isReadOnly) ? FileAttributes.ReadOnly : ~FileAttributes.ReadOnly;
                        di.Refresh();
                    }
                    DirectoryInfo fdi = new DirectoryInfo(folder);
                    fdi.Attributes &= (isReadOnly) ? FileAttributes.ReadOnly : ~FileAttributes.ReadOnly;
                    fdi.Refresh();
                    CopyDir(folder, dest);
                }
            }
        }

        public static void CopyFile(string sourceFile, string destFile, bool isReadOnly = false)
        {
            FileInfo fileInfo = new FileInfo(sourceFile);
            if (fileInfo.Exists)
            {
                fileInfo.IsReadOnly = isReadOnly;
                fileInfo.Refresh();
                fileInfo.CopyTo(destFile, true);
            }
        }
    }
}
