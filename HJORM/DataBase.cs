using System;
using System.Data;


namespace HJORM
{
    
    public abstract class DataBase
    {     
        public abstract object Execute(string Sql);

        public abstract string MakeTableDumb(string Sql, string tableName);

        public abstract DataTable GetDataTable(string Sql);

        public abstract object ExecuteDataReader(string Sql);

        public abstract void DataReaderFinished();

        public int CommandTimeOut
        {
            get { return int.Parse(System.Configuration.ConfigurationManager.AppSettings["SQLCommandTimeOut"]); }
        }
        

        /// <summary>
        /// factory method: maakt specfieke database-object aan per databasesoort
        /// </summary>
        /// <returns>DataBase</returns>
        public static DataBase Get()
        {
            //System.Configuration.ConfigurationManager.ConnectionStrings["cmsdb"].
            string providerName = System.Configuration.ConfigurationManager.ConnectionStrings["cmsdb"].ProviderName;
            if (providerName.Contains("SqlServer"))
            {
                return new SqlServer.DataBase();
            }
            else
            {
                //MySql is default database
                return new MySql.DataBase();
            }
        }
        public static DataBase Get(string connectionName)
        {
            //System.Configuration.ConfigurationManager.ConnectionStrings["cmsdb"].
            if (System.Configuration.ConfigurationManager.ConnectionStrings[connectionName] == null)
            {
                return null;
            }
            string providerName = System.Configuration.ConfigurationManager.ConnectionStrings[connectionName].ProviderName;
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;
            
            if (providerName.Contains("SqlServer"))
            {
                return null;
            }
            else
            {
                //MySql is default database
                return new MySql.DataBase(connectionString);
            }
        }
        /// <summary>
        /// Voor het geval er nog geen connectiestring is. Vanuit het instalDB script
        /// </summary>
        /// <returns></returns>
        public static DataBase Get(string providerName, string connectionString)
        {
            //System.Configuration.ConfigurationManager.ConnectionStrings["cmsdb"].
            if (providerName.Contains("SqlServer"))
            {
                return null;
            }
            else
            {
                //MySql is default database
                return new MySql.DataBase(connectionString);
            }
        }
        

        public static bool CheckConnection(string providerName, string connectionstring)
        {
            if (providerName.Contains("MySql"))
            {
                return MySql.DataBase.CheckConnection(connectionstring);
            }
            else
            {
                return false;
            }
        }

        //public abstract string CreateBackupSql(BaseObject obj);   
    }
}
