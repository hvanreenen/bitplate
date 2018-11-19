using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BitPlate.Domain.Utils
{
    public static class Logger
    {
        public static void Log(string fileName, string msg)
        {
            try
            {
                msg = String.Format("{0:dd-MM-yyyy HH:mm:ss}: {1}", DateTime.Now, msg);
                string path = System.IO.Path.GetDirectoryName(fileName);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string file = System.IO.Path.GetFileNameWithoutExtension(fileName);
                //elke maand nieuwe
                file = file + DateTime.Now.ToString("yyyy-MM");
                string extension = Path.GetExtension(fileName);
                fileName = path + "\\" + file + extension;
                TextWriter writer = File.AppendText(fileName);
                writer.WriteLine(msg);

                writer.Close();
            }
            catch (Exception ex)
            {
            }
        }
    }
}
