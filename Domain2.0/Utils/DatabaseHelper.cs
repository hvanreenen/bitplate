using HJORM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BitPlate.Domain.Utils
{




    public class DatabaseHelper
    {
        public class DatabaseTableDefinition
        {
            public string Name { get; set; }
            public bool isManyToManyLinkTable { get; set; }
            public string ParentTableName { get; set; }
            public string ForeignKey { get; set; }
        }

        public List<DatabaseTableDefinition> _dataCollectionTables;

        public List<DatabaseTableDefinition> DataCollectionTables
        {
            get
            {
                if (this._dataCollectionTables == null)
                {
                    _dataCollectionTables = new List<DatabaseTableDefinition>();
                    _dataCollectionTables.Add(new DatabaseTableDefinition() { Name = "datacollection", isManyToManyLinkTable = false });
                    _dataCollectionTables.Add(new DatabaseTableDefinition() { Name = "datafield", isManyToManyLinkTable = false });
                    //_dataCollectionTables.Add(new DatabaseTableDefinition() { Name = "datafield_live", isForeignKeyTable = false });
                    _dataCollectionTables.Add(new DatabaseTableDefinition() { Name = "datafile", isManyToManyLinkTable = false });
                    //_dataCollectionTables.Add(new DatabaseTableDefinition() { Name = "datafile_live", isForeignKeyTable = false });
                    _dataCollectionTables.Add(new DatabaseTableDefinition() { Name = "datagroup", isManyToManyLinkTable = false });
                    //_dataCollectionTables.Add(new DatabaseTableDefinition() { Name = "datagroup_live", isForeignKeyTable = false });
                    _dataCollectionTables.Add(new DatabaseTableDefinition() { Name = "datagrouplanguage", isManyToManyLinkTable = false });
                    _dataCollectionTables.Add(new DatabaseTableDefinition() { Name = "dataitem", isManyToManyLinkTable = false });
                    //_dataCollectionTables.Add(new DatabaseTableDefinition() { Name = "dataitem_live", isForeignKeyTable = false });
                    _dataCollectionTables.Add(new DatabaseTableDefinition() { Name = "dataitemlanguage", isManyToManyLinkTable = false });
                    _dataCollectionTables.Add(new DatabaseTableDefinition() { Name = "datalookupvalue", isManyToManyLinkTable = false });
                    //_dataCollectionTables.Add(new DatabaseTableDefinition() { Name = "datalookupvalue_live", isForeignKeyTable = false });
                    _dataCollectionTables.Add(new DatabaseTableDefinition() { Name = "datalookupvalueperitem", isManyToManyLinkTable = true, ParentTableName = "datalookupvalue", ForeignKey = "FK_lookupvalue" });
                    //_dataCollectionTables.Add(new DatabaseTableDefinition() { Name = "datatext", isManyToManyLinkTable = false });
                }
                return this._dataCollectionTables;
            }
        }

        public List<DatabaseTableDefinition> _siteTables;

        public List<DatabaseTableDefinition> SiteTables
        {
            get
            {
                if (_siteTables == null)
                {
                    _siteTables = new List<DatabaseTableDefinition>();
                    //_siteTables.Add(new DatabaseTableDefinition() { Name = "backup", isManyToManyLinkTable = false });

                    //_siteTables.Add(new DatabaseTableDefinition() { Name = "directory", isManyToManyLinkTable = false });
                    //_siteTables.Add(new DatabaseTableDefinition() { Name = "directoryautorisationitem", isManyToManyLinkTable = false });
                    //_siteTables.Add(new DatabaseTableDefinition() { Name = "drilldownmodule", isForeignKeyTable = false });
                    _siteTables.Add(new DatabaseTableDefinition() { Name = "eventlog", isManyToManyLinkTable = false });
                    //_siteTables.Add(new DatabaseTableDefinition() { Name = "importdefinition", isForeignKeyTable = false });
                    //_siteTables.Add(new DatabaseTableDefinition() { Name = "importdefinitiongroups", isForeignKeyTable = false });
                    _siteTables.Add(new DatabaseTableDefinition() { Name = "language", isManyToManyLinkTable = false });
                    _siteTables.Add(new DatabaseTableDefinition() { Name = "module", isManyToManyLinkTable = false });
                    //_siteTables.Add(new DatabaseTableDefinition() { Name = "modulelayout", isManyToManyLinkTable = false });
                    _siteTables.Add(new DatabaseTableDefinition() { Name = "modulenavigationaction", isManyToManyLinkTable = false });
                    _siteTables.Add(new DatabaseTableDefinition() { Name = "moduleperpage", isManyToManyLinkTable = true, ParentTableName="page", ForeignKey="FK_Page" });
                    _siteTables.Add(new DatabaseTableDefinition() { Name = "newsletter", isManyToManyLinkTable = false });
                    //_tables.Add(new DatabaseTableDefinition() { Name = "newsletterconfig", isForeignKeyTable = false });
                    _siteTables.Add(new DatabaseTableDefinition() { Name = "newslettergroup", isManyToManyLinkTable = false });
                    _siteTables.Add(new DatabaseTableDefinition() { Name = "newslettergroupsubscriber", isManyToManyLinkTable = false });
                    _siteTables.Add(new DatabaseTableDefinition() { Name = "newsletterlayout", isManyToManyLinkTable = false });
                    _siteTables.Add(new DatabaseTableDefinition() { Name = "newsletterlayoutcontainer", isManyToManyLinkTable = false });
                    _siteTables.Add(new DatabaseTableDefinition() { Name = "newslettermailing", isManyToManyLinkTable = false });
                    _siteTables.Add(new DatabaseTableDefinition() { Name = "newslettermailingstatistics", isManyToManyLinkTable = false });
                    _siteTables.Add(new DatabaseTableDefinition() { Name = "newsletterpergroup", isManyToManyLinkTable = true, ParentTableName = "newsletter", ForeignKey = "FK_newsletter" });
                    _siteTables.Add(new DatabaseTableDefinition() { Name = "newslettersubscriber", isManyToManyLinkTable = false });
                    //_siteTables.Add(new DatabaseTableDefinition() { Name = "objectpermission", isManyToManyLinkTable = false });
                    _siteTables.Add(new DatabaseTableDefinition() { Name = "page", isManyToManyLinkTable = false });
                    _siteTables.Add(new DatabaseTableDefinition() { Name = "pagefolder", isManyToManyLinkTable = false });
                    _siteTables.Add(new DatabaseTableDefinition() { Name = "script", isManyToManyLinkTable = false });

                    _siteTables.Add(new DatabaseTableDefinition() { Name = "scriptperpage", isManyToManyLinkTable = true, ParentTableName = "page", ForeignKey = "FK_page" });
                    ////_tables.Add("scriptpersite");
                    _siteTables.Add(new DatabaseTableDefinition() { Name = "scriptpertemplate", isManyToManyLinkTable = true, ParentTableName = "template", ForeignKey = "FK_template" });
                    _siteTables.Add(new DatabaseTableDefinition() { Name = "searchindex", isManyToManyLinkTable = false });
                    _siteTables.Add(new DatabaseTableDefinition() { Name = "site", isManyToManyLinkTable = false });
                    _siteTables.Add(new DatabaseTableDefinition() { Name = "siteenvironment", isManyToManyLinkTable = false });
                    _siteTables.Add(new DatabaseTableDefinition() { Name = "template", isManyToManyLinkTable = false });
                    //_siteTables.Add(new DatabaseTableDefinition() { Name = "unpublisheditem", isManyToManyLinkTable = false });
                    _siteTables.Add(new DatabaseTableDefinition() { Name = "siteuser", isManyToManyLinkTable = false });
                    _siteTables.Add(new DatabaseTableDefinition() { Name = "siteusergroup", isManyToManyLinkTable = false });
                    _siteTables.Add(new DatabaseTableDefinition() { Name = "siteusergroupperuser", isManyToManyLinkTable = true, ParentTableName = "siteusergroup", ForeignKey = "FK_usergroup" });
                    _siteTables.Add(new DatabaseTableDefinition() { Name = "userobjectpermission", isManyToManyLinkTable = true, ParentTableName = "siteuser", ForeignKey = "FK_siteuser" });
                    _siteTables.Add(new DatabaseTableDefinition() { Name = "usergroupobjectpermission", isManyToManyLinkTable = true, ParentTableName = "siteusergroup", ForeignKey = "FK_siteusergroup" });

                    _siteTables.Add(new DatabaseTableDefinition() { Name = "bitplateusergroup", isManyToManyLinkTable = false});
                    _siteTables.Add(new DatabaseTableDefinition() { Name = "bitplateuser", isManyToManyLinkTable = false });
                    _siteTables.Add(new DatabaseTableDefinition() { Name = "bitplateusergroupperuser", isManyToManyLinkTable = true, ParentTableName = "bitplateusergroup", ForeignKey = "FK_usergroup"});
                    //_siteTables.Add(new DatabaseTableDefinition() { Name = "userinterestin", isManyToManyLinkTable = false });

                }
                return _siteTables;
            }


        }

        public void CloneEnvironmentTables(CmsSite site, CmsSiteEnvironment currentEnvorinment, CmsSiteEnvironment newEnvironment)
        {
            foreach (DatabaseTableDefinition table in this.SiteTables)
            {
                string sql = String.Format(@"CREATE TABLE IF NOT EXISTS {0}." + table.Name + " LIKE {1}." + table.Name + ";", newEnvironment.DatabaseName, currentEnvorinment.DatabaseName, site.ID);
                DataBase.Get().Execute(sql);
            }
        }

        //public void ClearSiteEnvironmentTables(CmsSite site, CmsSiteEnvironment newEnvironment)
        //{
        //    DataBase.Get().Execute("SET foreign_key_checks = 0;");
        //    foreach (DatabaseTableDefinition table in this.SiteTables)
        //    {
        //        if (!table.isManyToManyLinkTable)
        //        {

        //            string sql = String.Format(@"DELETE FROM {0}." + table.Name + " WHERE FK_Site='{1}';", newEnvironment.DataBaseName, site.ID);
        //            DataBase.Get().Execute(sql);

        //        }
        //        else
        //        {
        //            string sql = String.Format(@"DELETE FROM {0}." + table.Name + " WHERE FK_Site='{1}';", newEnvironment.DataBaseName, site.ID);
        //            DataBase.Get().Execute(sql);
        //        }
        //    }
        //    DataBase.Get().Execute("SET foreign_key_checks = 1;");
        //}

        private string[] GetAllTables()
        {
            DataBase database = DataBase.Get();
            DataTable dt = database.GetDataTable("SHOW TABLES");
            List<string> tableCollection = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                if (row[0].ToString() != "")
                {
                    tableCollection.Add(row[0].ToString());
                }
            }
            return tableCollection.ToArray();
        }

        public void DropSiteEnvironmentTables(CmsSite site, CmsSiteEnvironment newEnvironment)
        {

            //bool ValidQuery = false;
            //string sql = "SET FOREIGN_KEY_CHECKS = 0;\r\nDROP TABLE IF EXISTS ";
            //foreach (string tableName in this.GetAllTables())
            //{
            //    if (!tableName.ToLower().StartsWith("data"))
            //    {
            //        sql += newEnvironment.DatabaseName + "." + tableName + ",\r\n";
            //        ValidQuery = true;
            //    }
            //}
            //sql = sql.Substring(0, sql.Length - 3);
            //sql += ";\r\nSET FOREIGN_KEY_CHECKS = 1;";
            //if (ValidQuery)
            //{
            //    DataBase.Get().Execute(sql);
            //}

            DataBase.Get().Execute("SET foreign_key_checks = 0;");
            foreach (DatabaseTableDefinition table in this.SiteTables)
            {
                string sql = String.Format(@"DROP TABLE IF EXISTS {0}." + table.Name + ";", newEnvironment.DatabaseName, site.ID);
                DataBase.Get().Execute(sql);
            }
            DataBase.Get().Execute("SET foreign_key_checks = 1;");
        }

        public void InsertEnvironmentDataInTables(CmsSite site, CmsSiteEnvironment currentEnvorinment, CmsSiteEnvironment newEnvironment)
        {
            DataBase.Get().Execute("SET foreign_key_checks = 0;");
            string sql;
            foreach (DatabaseTableDefinition tableDef in this.SiteTables)
            {
                if (tableDef.Name == "eventlog") continue;
                if (!tableDef.isManyToManyLinkTable)
                {
                    if (tableDef.Name == "site")
                    {
                        sql = String.Format(@"INSERT INTO {0}." + tableDef.Name + " SELECT * FROM {1}." + tableDef.Name + " WHERE ID='{2}';", newEnvironment.DatabaseName, currentEnvorinment.DatabaseName, site.ID);
                        DataBase.Get().Execute(sql);
                    }
                    else
                    {
                        sql = String.Format(@"INSERT INTO {0}." + tableDef.Name + " SELECT * FROM {1}." + tableDef.Name + " WHERE FK_Site='{2}';", newEnvironment.DatabaseName, currentEnvorinment.DatabaseName, site.ID);
                        DataBase.Get().Execute(sql);
                    }
                }
                else
                {
                    sql = String.Format(@"INSERT INTO {0}.{2} SELECT * FROM {1}.{2} WHERE EXISTS(SELECT * FROM {1}.{3} WHERE {1}.{3}.FK_Site='{5}' AND {1}.{3}.ID = {1}.{2}.{4})",
                        newEnvironment.DatabaseName, currentEnvorinment.DatabaseName, tableDef.Name, tableDef.ParentTableName, tableDef.ForeignKey, site.ID);
                    DataBase.Get().Execute(sql);
                }
            }

            ////modules references
            //sql = String.Format(@"INSERT INTO {0}.moduleperpage SELECT * FROM {1}.moduleperpage WHERE EXISTS(SELECT * FROM {1}.page WHERE {1}.page.FK_Site='{2}' AND {1}.page.ID = {1}.moduleperpage.FK_Page)", newEnvironment.DataBaseName, currentEnvorinment.DataBaseName, site.ID);
            //DataBase.Get().Execute(sql);
            ////scripts references
            ////per template
            //sql = String.Format(@"INSERT INTO {0}.scriptpertemplate SELECT * FROM {1}.scriptpertemplate WHERE EXISTS(SELECT * FROM {1}.template WHERE {1}.template.FK_Site='{2}' AND {1}.template.ID = {1}.scriptpertemplate.FK_Template)", newEnvironment.DataBaseName, currentEnvorinment.DataBaseName, site.ID);
            //DataBase.Get().Execute(sql);
            ////per page
            //sql = String.Format(@"INSERT INTO {0}.scriptperpage SELECT * FROM {1}.scriptperpage WHERE EXISTS(SELECT * FROM {1}.page WHERE {1}.page.FK_Site='{2}' AND {1}.page.ID = {1}.scriptperpage.FK_Page)", newEnvironment.DataBaseName, currentEnvorinment.DataBaseName, site.ID);
            //DataBase.Get().Execute(sql);
            ////Newsletters
            //sql = String.Format(@"INSERT INTO {0}.newsletterpergroup SELECT * FROM {1}.newsletterpergroup WHERE EXISTS(SELECT * FROM {1}.newsletter WHERE {1}.newsletter.FK_Site='{2}' AND {1}.newsletter.ID = {1}.newsletterpergroup.FK_Newsletter)", newEnvironment.DataBaseName, currentEnvorinment.DataBaseName, site.ID);
            //DataBase.Get().Execute(sql);

            DataBase.Get().Execute("SET foreign_key_checks = 1;");
        }

        public void CloneDataCollectionEnvironmentTables(CmsSite site, CmsSiteEnvironment currentEnvorinment, CmsSiteEnvironment newEnvironment)
        {
            foreach (DatabaseTableDefinition table in this.DataCollectionTables)
            {
                string sql = String.Format(@"CREATE TABLE IF NOT EXISTS {0}." + table.Name + " LIKE {1}." + table.Name + ";", newEnvironment.DatabaseName, currentEnvorinment.DatabaseName, site.ID);
                DataBase.Get().Execute(sql);
            }
        }

        public void ClearDataCollectionEnvironmentTables(CmsSite site, CmsSiteEnvironment newEnvironment)
        {
            DataBase.Get().Execute("SET foreign_key_checks = 0;");
            foreach (DatabaseTableDefinition table in this.DataCollectionTables)
            {
                try
                {
                    string sql = String.Format(@"DELETE FROM {0}." + table.Name + " WHERE FK_Site='{1}';", newEnvironment.DatabaseName, site.ID);
                    DataBase.Get().Execute(sql);
                }
                catch
                {
                    //NIET ELKE TABEL HEEFT FK_SITE.
                }
            }
            DataBase.Get().Execute("SET foreign_key_checks = 1;");
        }

        public void DropDataCollectionEnvironmentTables(CmsSite site, CmsSiteEnvironment newEnvironment)
        {
            //bool ValidQuery = false;
            //string sql = "SET FOREIGN_KEY_CHECKS = 0;\r\nDROP TABLE IF EXISTS ";
            //foreach (string tableName in this.GetAllTables())
            //{
            //    if (tableName.ToLower().StartsWith("data"))
            //    {
            //        sql += newEnvironment.DatabaseName + "." + tableName + ",\r\n";
            //        ValidQuery = true;
            //    }
            //}
            //sql = sql.Substring(0, sql.Length - 3);
            //sql += ";\r\nSET FOREIGN_KEY_CHECKS = 1;";
            //if (ValidQuery)
            //{
            //    DataBase.Get().Execute(sql);
            //}

            DataBase.Get().Execute("SET foreign_key_checks = 0;");
            foreach (DatabaseTableDefinition table in this.DataCollectionTables)
            {
                string sql = String.Format(@"DROP TABLE IF EXISTS {0}." + table.Name + ";", newEnvironment.DatabaseName, site.ID);
                DataBase.Get().Execute(sql);
            }
            DataBase.Get().Execute("SET foreign_key_checks = 1;");
        }

        public void InsertEnvironmentDataCollectionInTables(CmsSite site, CmsSiteEnvironment currentEnvorinment, CmsSiteEnvironment newEnvironment)
        {
            DataBase.Get().Execute("SET foreign_key_checks = 0;");
            string sql;
            foreach (DatabaseTableDefinition table in this.DataCollectionTables)
            {
                if (!table.isManyToManyLinkTable)
                {
                    sql = String.Format(@"INSERT INTO {0}." + table.Name + " SELECT * FROM {1}." + table.Name + " WHERE FK_Site='{2}';", newEnvironment.DatabaseName, currentEnvorinment.DatabaseName, site.ID);
                    DataBase.Get().Execute(sql);
                }
            }

            //lookupvalues
            sql = String.Format(@"INSERT INTO {0}.datalookupvalueperitem SELECT * FROM {1}.datalookupvalueperitem WHERE EXISTS(SELECT * FROM {1}.datalookupvalue WHERE {1}.datalookupvalue.FK_Site='{2}' AND {1}.datalookupvalue.ID = {1}.datalookupvalueperitem.FK_LookupValue)", newEnvironment.DatabaseName, currentEnvorinment.DatabaseName, site.ID);
            DataBase.Get().Execute(sql);

            DataBase.Get().Execute("SET foreign_key_checks = 1;");
        }
    }
}
