using System;
using System.Data;
using System.IO;

using MySql.Data.MySqlClient;
using HJORM.Logging;

namespace HJORM.MySql
{
    class DataBase : HJORM.DataBase
    {
        private MySqlConnection conn;
       
        public DataBase()
        {
            if (conn == null)
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["cmsdb"].ConnectionString;
                conn = new MySqlConnection(connectionString);
            }
        }

        public DataBase(string connectionString)
        { 
            conn = new MySqlConnection(connectionString);
            
            
        }

        public override object Execute(string Sql)
        {
            Object lastInsertID = null;
            try
            {
                if (Sql != "")
                {
                   
                    SqlLogger.WriteSql(Sql);
                    MySqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = Sql;
                    cmd.CommandTimeout = (this.CommandTimeOut != -1) ? this.CommandTimeOut : cmd.CommandTimeout;
                    conn.Open();
                    //cmd.ExecuteNonQuery();
                    
                    lastInsertID = cmd.ExecuteScalar();
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("foreign key constraint fails"))
                {
                    throw new Exception("Kan niet verwijderen, want er zijn nog gerelateerde gegevens.", ex);
                }
                else if (ex.Message.Contains("Duplicate entry"))
                {
                    throw new Exception("Kan niet opslaan, want een gegeven is niet uniek. Zie de volgende melding: " + ex.Message, ex);
                }
                 throw ex;
            }
            finally
            {
                conn.Close();
            }
            if (lastInsertID != null && lastInsertID.ToString() == String.Empty) lastInsertID = 0;
            return lastInsertID;
        }

        public override object ExecuteDataReader(string Sql)
        {
            SqlLogger.WriteSql(Sql);
            MySqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = Sql;
            conn.Open();
            //cmd.ExecuteNonQuery();

            return cmd.ExecuteReader();
        }

        public override void DataReaderFinished()
        {
            conn.Close();
        }
        public override DataTable GetDataTable(string Sql)
        {
            try
            {
                SqlLogger.WriteSql(Sql);
                
                MySqlDataAdapter adapter = new MySqlDataAdapter(Sql, conn);
                
                DataSet ds = new DataSet();
                conn.Open();
                ds.EnforceConstraints = false;
                adapter.FillSchema(ds, SchemaType.Source, "table");
                adapter.Fill(ds, "table");
                conn.Close();
                return ds.Tables[0];
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }


        internal static bool CheckConnection(string connectionstring)
        {
            bool returnvalue = false;
            MySqlConnection conn = new MySqlConnection(connectionstring);
            try
            {
                conn.Open();
                returnvalue = true;
            }
            catch (Exception ex)
            {
            }
            finally
            {
                conn.Close();
                conn = null;
            }
            return returnvalue;
        }

        public string MakeTableDumbCsv(string Sql)
        {
            string output = "";
            DataTable tbl = GetDataTable(Sql);
            foreach (DataRow row in tbl.Rows)
            {
                output += writeRowCsv(row);
            }
            return output;
        }

        private string writeRowCsv(DataRow row)
        {
            string returnValue = "";
            for (int i = 0; i < row.Table.Columns.Count; i++)
            {
                if (row[i] == DBNull.Value)
                {
                    returnValue += @"\N,";
                }
                else if (row.Table.Columns[i].DataType == typeof(DateTime))
                {
                    returnValue += "'" + ((DateTime)row[i]).ToString("yyyy-MM-dd HH:mm:ss") + "',";
                }
                else if (row.Table.Columns[i].ColumnName == "Path")
                {
                    returnValue += "'" + row[i].ToString().Replace("\\", "/") + "',";
                }
                else
                {
                    //eventueel escape char voor '
                    returnValue += "'" + row[i].ToString().Replace("'", "''") + "',";
                }
            }
            returnValue = returnValue.Substring(0, returnValue.Length - 1);
            returnValue += "\r\n";
            return returnValue;
        }

        public override string MakeTableDumb(string Sql, string tableName)
        {
            string output = "";
            DataTable tbl = GetDataTable(Sql);
            
            foreach (DataRow row in tbl.Rows)
            {
                output += "INSERT INTO " + tableName + " SET ";
                output += writeRow(row);
                if (tableName == "site")
                {
                    output += "\r\n ON DUPLICATE KEY UPDATE ";
                    output += writeRow(row);
                }
                output += ";\r\n";
            }
            return output;
        }

        private string writeRow(DataRow row)
        {
            string returnValue = "";
            for (int i = 0; i < row.Table.Columns.Count; i++)
            {
                returnValue += row.Table.Columns[i].ColumnName;
                returnValue += "=";
                if (row.Table.Columns[i].ColumnName == "Price")
                {
                    string test = "";
                }
                if (row[i] == DBNull.Value)
                {
                    returnValue += @"NULL,";
                }
                else if (row.Table.Columns[i].DataType == typeof(DateTime))
                {
                    returnValue += "'" + ((DateTime)row[i]).ToString("yyyy-MM-dd HH:mm:ss") + "',";
                }

                else if (row.Table.Columns[i].DataType == typeof(Boolean))
                {
                    returnValue += row[i].ToString() + ",";
                }
                else if (row.Table.Columns[i].DataType == typeof(Int32) ||
                        row.Table.Columns[i].DataType == typeof(Int64) ||
                        row.Table.Columns[i].DataType == typeof(UInt32) ||
                        row.Table.Columns[i].DataType == typeof(UInt64))
                {
                    returnValue += row[i].ToString() + ",";
                }
                else if (row.Table.Columns[i].DataType == typeof(Double) ||
                    row.Table.Columns[i].DataType == typeof(Single))
                {
                    returnValue += row[i].ToString().Replace(",", ".") + ",";
                }
                else if (row.Table.Columns[i].ColumnName == "Path")
                {
                    returnValue += "'" + row[i].ToString().Replace("\\", "/") + "',";
                }
                else
                {
                    //eventueel escape char voor '
                    returnValue += "'" + row[i].ToString().Replace("'", "''") + "',";
                }
            }
            returnValue = returnValue.Substring(0, returnValue.Length - 1);
            //returnValue += "\r\n";
            return returnValue;
        }
    }
}
