using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;

namespace HJORM.Logging
{
    public static class SqlLogger
    {
        //public static bool UseLogging = false;
        public static void WriteSql(string sql)
        {
            try
            {
                bool useSqlLogging = (ConfigurationManager.AppSettings["UseSQLLogging"] == "true");
                if (useSqlLogging)
                {
                    string fileName = "SQL_log.txt";
                    TextWriter writer = File.AppendText(AppDomain.CurrentDomain.BaseDirectory + fileName);
                    writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff"));
                    writer.Write(sql + "\r\n");
                    writer.WriteLine("===============================================");

                    writer.Close();
                }
            }
            catch (Exception ex)
            {
                //SKIP log while in file is used by other process.
            }
        }
    }
}
