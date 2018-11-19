using System;
using System.Collections.Generic;

using System.Text;

using System.Reflection;
using System.Collections;

namespace HJORM
{
    
    class SqlCreator
    {
        /// <summary>
        /// factory method: maakt specfieke sqlcreator aan per database
        /// </summary>
        /// <returns>SqlCreator</returns>
        internal static SqlCreator Get()
        {
            //switch database type
            return new SqlServer.SqlCreator();
        }

        internal virtual string CreateSelectStatement(string tableName, string where, string orderby, int pageNumber, int pageSize)
        {
            string sql = @"SELECT * FROM " + tableName + "";
            
            if (where != "")
            {
                sql += @" WHERE 1=1 AND " + where;
            }
            if (orderby != "")
            {
                sql += @" ORDER BY " + orderby;
            }
            if (pageSize != 0)
            {
                pageNumber--;
                sql += @" LIMIT " + pageNumber * pageSize + @", " + pageSize;
            }
            return sql;
        }

        internal virtual string CreateSelectStatement(string tableName, string where, string orderby, int pageNumber, int pageSize, int fromRowNumber)
        {
            string sql = @"SELECT * FROM " + tableName + "";
            
            if (where != "")
            {
                sql += @" WHERE 1=1 AND " + where;
            }
            if (orderby != "")
            {
                sql += @" ORDER BY " + orderby;
            }
            if (pageSize != 0)
            {

                if (pageNumber != 0)
                {
                    pageNumber--;
                    sql += @" LIMIT " + pageNumber * pageSize + @", " + pageSize;
                }
                else
                {
                    if (fromRowNumber > 0) fromRowNumber--; //gebruikers vullen vanaf rij 1 in, in sql is dit rij 0 
                    sql += @" LIMIT " + fromRowNumber + @", " + pageSize;
                }
            }
            return sql;
        }

        internal string CreateSelectByIDStatement(BaseObject baseObject)
        {
            return CreateSelectByIDStatement(baseObject, baseObject.ID);
        }

        internal string CreateSelectByIDStatement(BaseObject baseObject, Guid id)
        {
            bool joinForInheritance = false;
            foreach (object attribute in baseObject.GetType().GetCustomAttributes(typeof(Attributes.Inheritance), false))
            {
                Attributes.Inheritance inheritanceAttribute = (Attributes.Inheritance)attribute;
                if (inheritanceAttribute.InheritanceType == HJORM.Attributes.InheritanceEnum.TwoTables)
                {
                    joinForInheritance = true;
                }
            }
            if (joinForInheritance)
            {
                return CreateSelectJoinStatement(baseObject, id);
            }
            else
            {
                return CreateSelectStatement(baseObject.TableName, "ID = '" + id.ToString() + "'", "", 0, 0);
            }
        }

        private string CreateSelectJoinStatement(BaseObject baseObject, Guid id)
        {
            string baseTableName = baseObject.GetType().BaseType.Name;
            foreach (object persitentAttribute in baseObject.GetType().BaseType.GetCustomAttributes(typeof(Attributes.Persistent), false))
            {
                baseTableName = ((Attributes.Persistent)persitentAttribute).DataBaseObject;
            }
            string subTableName = baseObject.TableName;

            string sql = "SELECT * FROM " + subTableName + " T1 INNER JOIN " + baseTableName + " T2 On T1.ID = T2.ID WHERE T1.ID='" + id + "'";

            return sql;
        }

        internal virtual string CreateInsertStatement_old(BaseObject baseObject)
        {
            string sql = "";
            Type type = baseObject.GetType();
            sql = "INSERT INTO " + baseObject.TableName + " (";
            foreach (PropertyInfo prop in type.GetProperties())
            {

                string fieldName;
                bool useProperty = AnalyseProperty(prop, out fieldName);
                if (useProperty)
                {
                    sql += fieldName + ", ";
                }
            }
            sql = sql.Substring(0, sql.Length - 2);
            sql += ") VALUES (";
            foreach (PropertyInfo prop in type.GetProperties())
            {
                string fieldName;
                bool useProperty = AnalyseProperty(prop, out fieldName);
                if (useProperty)
                {
                    sql += getSqlValue(prop, baseObject);
                }
            }
            sql = sql.Substring(0, sql.Length - 2);
            sql += ")";
            sql += "; select last_insert_id()";
            return sql;
        }

        internal virtual string CreateInsertStatement(BaseObject baseObject)
        {

            string sql = "";
            Type type = baseObject.GetType();
            bool twoTableInsert = false; //1 op 1 relatie in het geval van inheritance
            bool parentTableInsert = false;

            foreach (object attribute in type.GetCustomAttributes(typeof(Attributes.Inheritance), false))
            {
                Attributes.Inheritance inheritanceAttribute = (Attributes.Inheritance)attribute;
                if (inheritanceAttribute.InheritanceType == HJORM.Attributes.InheritanceEnum.TwoTables)
                {
                    twoTableInsert = true;
                }
                else if (inheritanceAttribute.InheritanceType == HJORM.Attributes.InheritanceEnum.ParentTable)
                {
                    parentTableInsert = true;
                }
            }
            if (twoTableInsert)
            {
                //elke in eigen tabel opslaan
                //dus twee keer een insert doen

                //base table
                string baseTableName = type.BaseType.Name;
                foreach (object persitentAttribute in type.BaseType.GetCustomAttributes(typeof(Attributes.Persistent), false))
                {
                    baseTableName = ((Attributes.Persistent)persitentAttribute).DataBaseObject;
                }

                string sqlBase = "INSERT INTO " + baseTableName + " SET ";
                
                foreach (PropertyInfo prop in type.BaseType.GetProperties())
                {
                    string fieldName;
                    bool useProperty = AnalyseProperty(prop, out fieldName);
                    if (useProperty)
                    {
                        sqlBase += fieldName + " = ";
                        sqlBase += getSqlValue(prop, baseObject);
                    }
                }
                sqlBase = sqlBase.Substring(0, sqlBase.Length - 2);
                sqlBase += "; ";

                //child table
                string sqlSub = "INSERT INTO " + baseObject.TableName + " SET ID = '" + baseObject.ID.ToString() + "', ";
                
                foreach (PropertyInfo prop in type.GetProperties())
                {
                    //alleen de props als de basetype ze niet heeft, behalve voor site
                    if (type.BaseType.GetProperty(prop.Name) == null || prop.Name == "Site")
                    {
                        string fieldName;
                        bool useProperty = AnalyseProperty(prop, out fieldName);
                        if (useProperty)
                        {
                            sqlSub += fieldName + " = ";
                            sqlSub += getSqlValue(prop, baseObject);
                        }
                    }
                }
                sqlSub = sqlSub.Substring(0, sqlSub.Length - 2);
                sql = sqlBase + sqlSub + "; select last_insert_id();";
            }
            else
            {
                string tablename = baseObject.TableName;
                foreach (object persitentAttribute in type.GetCustomAttributes(typeof(Attributes.Persistent), false))
                {
                    tablename = ((Attributes.Persistent)persitentAttribute).DataBaseObject;
                }
                if (parentTableInsert)
                {
                    tablename = type.BaseType.Name;
                    foreach (object persitentAttribute in type.BaseType.GetCustomAttributes(typeof(Attributes.Persistent), false))
                    {
                        tablename = ((Attributes.Persistent)persitentAttribute).DataBaseObject;
                    }
                }
                //geldt voor mySQL, gedaan omdat de volgorde van de eerste keer props (fieldnames) en de tweede keer (values) verschilde
                sql = "INSERT INTO " + tablename + " SET ";
                foreach (PropertyInfo prop in type.GetProperties())
                {
                    string fieldName;
                    bool useProperty = AnalyseProperty(prop, out fieldName);
                    if (useProperty)
                    {
                        sql += fieldName + " = ";
                        sql += getSqlValue(prop, baseObject);
                    }
                }
                sql = sql.Substring(0, sql.Length - 2);
                sql += "; select last_insert_id();";
            }

            return sql;
        }
        internal string CreateUpdateStatement(BaseObject baseObject)
        {
            string sql = "";
            Type type = baseObject.GetType();
            bool twoTables = false; //1 op 1 relatie in het geval van inheritance
            bool parentTableInsert = false;

            foreach (object attribute in type.GetCustomAttributes(typeof(Attributes.Inheritance), false))
            {
                Attributes.Inheritance inheritanceAttribute = (Attributes.Inheritance)attribute;
                if (inheritanceAttribute.InheritanceType == HJORM.Attributes.InheritanceEnum.TwoTables)
                {
                    twoTables = true;
                }
                else if (inheritanceAttribute.InheritanceType == HJORM.Attributes.InheritanceEnum.ParentTable)
                {
                    parentTableInsert = true;
                }
            }
            if (twoTables)
            {
                //elke in eigen tabel opslaan
                //dus twee keer een insert doen

                //base table
                string baseTableName = type.BaseType.Name;
                foreach (object persitentAttribute in type.BaseType.GetCustomAttributes(typeof(Attributes.Persistent), false))
                {
                    baseTableName = ((Attributes.Persistent)persitentAttribute).DataBaseObject;
                }

                string sqlBase = "UPDATE " + baseTableName + " SET ";
                foreach (PropertyInfo prop in type.BaseType.GetProperties())
                {
                    string fieldName;
                    bool useProperty = AnalyseProperty(prop, out fieldName);
                    if (useProperty)
                    {
                        sqlBase += fieldName + " = ";
                        sqlBase += getSqlValue(prop, baseObject);
                    }
                }
                sqlBase = sqlBase.Substring(0, sqlBase.Length - 2);
                sqlBase += " WHERE ID = '" + baseObject.ID + "'"; //id van base en sub zijn gelijk
                sqlBase += "; ";

                //child table
                string sqlSub = "UPDATE " + baseObject.TableName + " SET  ";
                foreach (PropertyInfo prop in type.GetProperties())
                {
                    //allen de props als de basetype ze niet heeft
                    if (type.BaseType.GetProperty(prop.Name) == null)
                    {
                        string fieldName;
                        bool useProperty = AnalyseProperty(prop, out fieldName);
                        if (useProperty)
                        {
                            sqlSub += fieldName + " = ";
                            sqlSub += getSqlValue(prop, baseObject);
                        }
                    }
                }
                sqlSub = sqlSub.Substring(0, sqlSub.Length - 2);
                sqlSub += " WHERE ID = '" + baseObject.ID + "'";

                sql = sqlBase + sqlSub;
            }
            else
            {

                sql = "UPDATE " + baseObject.TableName + " SET ";
                foreach (PropertyInfo prop in type.GetProperties())
                {
                    string fieldName;
                    bool useProperty = AnalyseProperty(prop, out fieldName);
                    if (useProperty)
                    {
                        sql += fieldName + " = ";
                        sql += getSqlValue(prop, baseObject);
                    }
                }
                sql = sql.Substring(0, sql.Length - 2);
                sql += " WHERE ID = '" + baseObject.ID + "'";
            }
            return sql;
        }


        internal virtual string CreateDeleteStatement(BaseObject baseObject)
        {
            return "DELETE FROM " + baseObject.TableName + " WHERE ID =  '" + baseObject.ID.ToString() + "'";
        }

        /// <summary>
        /// Kijk of property persistent moet worden en bepaal de database fieldname 
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        private bool AnalyseProperty(PropertyInfo prop, out string fieldName )
        {
            bool useProperty = true;
            fieldName = prop.Name;
            //if (prop.PropertyType.IsGenericType)
            //{
            //    useProperty = false;
            //}

            foreach (object attribute in prop.GetCustomAttributes(false))
            {
                if (attribute.GetType() == typeof(Attributes.NonPersistent))
                {
                    useProperty = false;
                }
                else if (attribute.GetType() == typeof(Attributes.Association))
                {
                    fieldName = ((Attributes.Association)attribute).ForeignKey;
                }
                if (attribute.GetType() == typeof(Attributes.Persistent))
                {
                    fieldName = ((Attributes.Persistent)attribute).DataBaseObject;
                }
            }
            if (fieldName == "")
            {
                useProperty = false;
            }
            if (prop.PropertyType.FullName.Contains("BaseCollection"))
            {
                //collecties worden bij savechildren gedaan
                useProperty = false;
            }
            return useProperty;
        }

        private string getSqlValue(PropertyInfo prop, BaseObject baseObject)
        {
            string sql;
            object value = prop.GetValue(baseObject, null);

            if (value == null)
            {
                sql = "null, ";
            }
            else if (value.GetType() == typeof(DateTime))
            {
                sql = "'" + Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss") + "', ";
            }
            else if (value.GetType() == typeof(int) ||
                value.GetType() == typeof(long))
            {
                sql = value.ToString() + ", ";
            }
            else if (value.GetType() == typeof(double))
            {
                sql = value.ToString().Replace(",", ".") + ", ";
            }
            else if (value.GetType() == typeof(bool))
            {
                if ((bool)value)
                {
                    sql = "1, ";
                }
                else
                {
                    sql = "0, ";
                }
            }
            else if (IsBaseObject(value.GetType()))
            {
                sql = "'" + ((BaseObject)value).ID.ToString() + "', ";
            }
            else if (IsEnumType(value.GetType()))
            {
                sql = (int)value + ", ";
            }
            else if (value.GetType().IsArray)
            {
                string valuesString = "";
                IEnumerable arrayValues = value as IEnumerable;
                foreach (object item in arrayValues)
                {
                    valuesString += item + ",";
                }
                sql = "'" + valuesString + "', ";
            }
            //else if (value.GetType() == typeof(System.String[]))
            //{
            //    System.String[] values = (System.String[])value;
            //    string valuesString = "";
            //    foreach (string item in values)
            //    {
            //        valuesString += item + ",";
            //    }
            //    sql = "'" + valuesString + "', ";
            //}
            else
            {
                //escape caracter voor ' in mysql
                sql = "'" + value.ToString().Replace("'", "''") + "', ";
                if (sql.Contains(@"\"))
                {
                    //mySql needs \\ to enter \
                    sql = sql.Replace(@"\", @"\\");
                }
            }
            return sql;
        }

        private bool IsEnumType(Type type)
        {
            return (type.BaseType == typeof(System.Enum));
        }

        private bool IsBaseObject(Type type)
        {
            if (type == typeof(BaseObject))
            {
                return true;
            }
            else if (type == typeof(object))
            {
                return false;
            }
            else
            {
                return IsBaseObject(type.BaseType);
            }
        }

        internal string CreateBackupStatement<T>(BaseCollection<T> collection) where T : BaseObject, new()
        {
            string sql = "";
            Type type = typeof(T);
            string tablename = new T().TableName;
            foreach (object persitentAttribute in type.GetCustomAttributes(typeof(Attributes.Persistent), false))
            {
                tablename = ((Attributes.Persistent)persitentAttribute).DataBaseObject;
            }

            //geldt voor mySQL, gedaan omdat de volgorde van de eerste keer props (fieldnames) en de tweede keer (values) verschilde
            sql = "INSERT INTO " + tablename + "(";
            foreach (PropertyInfo prop in type.GetProperties())
            {
                string fieldName;
                bool useProperty = AnalyseProperty(prop, out fieldName);
                if (useProperty)
                {
                    sql += fieldName + ",";
                }
            }
            sql = sql.Substring(0, sql.Length - 1);
            sql += ") VALUES (";
            foreach (T t in collection)
            {
                sql += " (";
                foreach (PropertyInfo prop in type.GetProperties())
                {
                    string fieldName;
                    bool useProperty = AnalyseProperty(prop, out fieldName);
                    if (useProperty)
                    {
                        sql += getSqlValue(prop, t);// +",";
                    }
                }
                sql = sql.Substring(0, sql.Length - 2);
                sql += @") , 
";
            }
            sql = sql.Substring(0, sql.Length - 5);
            sql += @");

";
            return sql;
        }


        internal string CreateBackupStatement(BaseObject baseObject, string FKName, string FKParameterName)
        {
            string sql = "";
            Type type = baseObject.GetType();
            string tablename = baseObject.TableName;
            foreach (object persitentAttribute in type.GetCustomAttributes(typeof(Attributes.Persistent), false))
            {
                tablename = ((Attributes.Persistent)persitentAttribute).DataBaseObject;
            }

            //geldt voor mySQL, gedaan omdat de volgorde van de eerste keer props (fieldnames) en de tweede keer (values) verschilde
            sql = "INSERT INTO " + tablename + " SET ";
            foreach (PropertyInfo prop in type.GetProperties())
            {
                string fieldName;
                bool useProperty = AnalyseProperty(prop, out fieldName);
                if (useProperty)
                {
                    if (fieldName == FKName)
                    {
                        sql += FKName + " = " + FKParameterName + ", ";
                    }
                    else
                    {
                        sql += fieldName + " = ";
                        sql += getSqlValue(prop, baseObject);
                    }
                }
            }
            sql = sql.Substring(0, sql.Length - 2);
            sql += @";
";
            return sql;
        }
    }
}
