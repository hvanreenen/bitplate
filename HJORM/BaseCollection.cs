using System;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.Serialization;

using System.Text;
using System.Reflection;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace HJORM
{
    
    public class BaseCollection<T> : List<T>, IEnumerable where T : BaseObject, new()
    {
        public string Where = "";
        public string OrderBy = "";
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int FromRowNumber { get; set; }
        //voor andere conncetiestrings dan default
        public string ConnectionString { get; set; }

        public string TableName { get; set; }
        public bool IsLoaded { get; set; }
        private SqlCreator _sqlCreator;

        public static BaseCollection<T> Get() 
        {
            return BaseCollection<T>.Get("", "");
        }

        public static BaseCollection<T> Get(string where) 
        {
            return BaseCollection<T>.Get(where, "");
        }

        public static BaseCollection<T> Get(string where, string orderby)
        {
            BaseCollection<T> returnValue = new BaseCollection<T>();
            returnValue.Where = where;
            returnValue.OrderBy = orderby;
            returnValue.reload();

            return returnValue;
        }

        //public static BaseCollection<T> Get(string where, string orderby, int pageNumber, int pageSize)
        //{
        //    BaseCollection<T> returnValue = new BaseCollection<T>();
        //    returnValue.Where = where;
        //    returnValue.OrderBy = orderby;
        //    returnValue.PageSize = pageSize;
        //    returnValue.PageNumber = pageNumber;
        //    returnValue.Reload();

        //    return returnValue;
        //}

        //public static BaseCollection<T> Get(string where, string orderby, int pageNumber, int pageSize, int FromRowNumber)
        //{
        //    BaseCollection<T> returnValue = new BaseCollection<T>();
        //    returnValue.Where = where;
        //    returnValue.OrderBy = orderby;
        //    returnValue.PageSize = pageSize;
        //    returnValue.FromRowNumber = FromRowNumber;
        //    returnValue.Reload();

        //    return returnValue;
        //}

        public static BaseCollection<T> Get(string where, string orderby, int pageNumber = 0, int pageSize = 0, int FromRowNumber = 0, string tableName = "", string connectionString = "")
        {
            BaseCollection<T> returnValue = new BaseCollection<T>();
            returnValue.Where = where;
            returnValue.OrderBy = orderby.Trim();
            returnValue.PageSize = pageSize;
            returnValue.FromRowNumber = FromRowNumber;
            returnValue.TableName = tableName;
            returnValue.ConnectionString = connectionString;
            returnValue.reload();

            return returnValue;
        }

        private void reload()
        {
            this.Clear();
            
            Type type = typeof(T);
            T t = new T();
            string tableName = t.TableName;
            if (this.TableName != null && this.TableName != "")
            {
                tableName = this.TableName;
            }
            string connectionName = t.ConnectionName;
            DataBase db = DataBase.Get(connectionName);
            if (this.ConnectionString != "" && this.ConnectionString != null)
            {
                db = DataBase.Get("mysql", this.ConnectionString);
            }

            //als er geen database is, zoals bij remote WCF connectie, dan niks doen
            if (db == null) return;
           
            string sql = SqlCreator.Get().CreateSelectStatement(tableName, Where, OrderBy, PageNumber, PageSize, FromRowNumber);
            sql = sql.Replace("\\", "\\\\");
            DataTable dataTable = db.GetDataTable(sql);

            foreach (DataRow dataRow in dataTable.Rows)
            {
                t = new T();
                t.db = db;
                t.FillObject(type, dataRow);

                this.Add(t);
            }
        }

        public static BaseCollection<T> LoadFromSql(string sql)
        {
            BaseCollection<T> returnValue = new BaseCollection<T>();
            returnValue.Clear();
            DataBase db = DataBase.Get();
            DataTable dataTable = db.GetDataTable(sql);
            Type type = typeof(T);
            foreach (DataRow dataRow in dataTable.Rows)
            {
                T t = new T();
                t.FillObject(type, dataRow);

                returnValue.Add(t);
            }
            return returnValue;
        }

        public void OrderByFieldName(string Name)
        {
            if (OrderBy == Name)
            {
                OrderBy = Name + " DESC";
            }
            else
            {
                OrderBy = Name;
            }
            //_orderby = Name;
            reload();
        }

        public void OrderByFieldName(string Name, string direction)
        {
            string[] multipleFieldNames = Name.Split(new char[] { ',' });
            foreach (string fieldName in multipleFieldNames)
            {
                OrderBy += fieldName + " " + direction + ", ";
            }
            OrderBy = OrderBy.Substring(0, OrderBy.Length -2);
            
            //_orderby = Name;
            reload();
        }

        

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        //public T GetByID(Guid id)
        //{
        //    //2_DO caching in collection, nu is er een extra call naar de database
        //    T t = new T();
        //    t.ID = id;
        //    t.Load();
        //    return t;
        //}

        public void DeleteAll()
        {
            foreach(T t in this){
                t.Delete();
            }
            base.Clear();
        }

        //public T GetFirst()
        //{
        //    foreach (T t in this.Values)
        //    {
        //        return t;
        //    }
        //    return default(T);  
        //}
        //public new  void Clear()
        //{
        //    DeleteAll();
        //}

        //public T GetByName(string name)
        //{
        //    //2_DO caching in collection, nu is er een extra call naar de database
        //    T t = new T();
        //    t.Load(id);
        //    return t;
        //}

        public virtual string CreateBackupSQL()
        {
            _sqlCreator = SqlCreator.Get();
            string sql = _sqlCreator.CreateBackupStatement(this);
            return sql;
        }

        public List<BaseObject> ToList()
        {
            List<BaseObject> list = new List<BaseObject>();
            foreach (T t in this)
            {
                list.Add(t);
            }
            return list;
        }

        public Func<T, T> CreateNewStatement(string fields)
        {
            // input parameter "o"
            var xParameter = Expression.Parameter(typeof(T), "o");

            // new statement "new Data()"
            var xNew = Expression.New(typeof(T));

            // create initializers
            var bindings = fields.Split(',').Select(o => o.Trim())
                .Select(o =>
                {

                    // property "Field1"
                    var mi = typeof(T).GetProperty(o);

                    // original value "o.Field1"
                    var xOriginal = Expression.Property(xParameter, mi);

                    // set value "Field1 = o.Field1"
                    return Expression.Bind(mi, xOriginal);
                }
            );

            // initialization "new Data { Field1 = o.Field1, Field2 = o.Field2 }"
            var xInit = Expression.MemberInit(xNew, bindings);

            // expression "o => new Data { Field1 = o.Field1, Field2 = o.Field2 }"
            var lambda = Expression.Lambda<Func<T, T>>(xInit, xParameter);

            // compile to Func<Data, Data>
            return lambda.Compile();
        }

        //public void add(Guid guid)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
