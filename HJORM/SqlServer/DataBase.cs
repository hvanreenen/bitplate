using System;
using System.Collections.Generic;

using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace HJORM.SqlServer
{
    class DataBase : HJORM.DataBase
    {
        private SqlConnection conn;

        public DataBase()
        {
            if (conn == null)
            {
                conn = new SqlConnection(@"Data Source=LAPTOPHJ\SQLEXPRESS;Initial Catalog=TestDB;Integrated Security=True");
            }
        }

        public override object Execute(string Sql)
        {
            if (Sql != "")
            {
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = Sql;
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            //2_DO @@Indentity after insert
            return null;
        }

        public override DataTable GetDataTable(string Sql)
        {
            SqlDataAdapter adapter = new SqlDataAdapter(Sql, conn);

            DataSet ds = new DataSet();
            conn.Open();
            adapter.FillSchema(ds, SchemaType.Source, "table");
            adapter.Fill(ds, "table");
            conn.Close();
            return ds.Tables[0];
        }
        public override string MakeTableDumb(string Sql, string tableName)
        {
            return "";
        }


        public override object ExecuteDataReader(string Sql)
        {
            throw new NotImplementedException();
        }

        public override void DataReaderFinished()
        {
            throw new NotImplementedException();
        }
    }
}
