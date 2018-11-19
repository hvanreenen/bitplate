using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Data;
using System.Reflection;
using HJORM.Attributes;

namespace HJORM
{
    
    public class BaseObject
    {
        internal DataBase db;
        private SqlCreator _sqlCreator;
        private Guid _id = Guid.Empty;
        private string _tableName = String.Empty;
        private bool _isLoaded;
        private bool _isNew = false;
        public virtual string Name { get; set; }

        /*************************************************FIX ongeldige data**********************/
        private DateTime _CreateDate;
        public DateTime CreateDate {
            get
            {
                if (this._CreateDate == null)
                {
                    this._CreateDate = DateTime.Now;
                }
                return this._CreateDate;
            }
            set
            {
                this._CreateDate = value;
            }
        }
        private DateTime _ModifiedDate;
        public DateTime ModifiedDate
        {
            get
            {
                if (this._ModifiedDate == null)
                {
                    this._ModifiedDate = DateTime.Now;
                }
                return this._ModifiedDate;
            }
            set
            {
                this._ModifiedDate = value;
            }
        }
       
        public Guid ID
        {
            get { return _id; }
            set { _id = value; }
        }


        [Attributes.NonPersistent()]
        [System.Xml.Serialization.XmlIgnore()]
        public bool IsNew
        {
            get { return (_id == Guid.Empty || this._isNew); }
            //set { _isNew = value; }
            //voor deserialiseren als DataMember (WCF) private set toevoegen
            set { _isNew = value; }
        }

       

        [Attributes.NonPersistent()]
        [System.Web.Script.Serialization.ScriptIgnore]
        [System.Xml.Serialization.XmlIgnore()]
        internal string TableName
        {
            get
            {
                if (_tableName == String.Empty)
                {
                    _tableName = this.GetType().Name;
                    foreach (object attribute in this.GetType().GetCustomAttributes(typeof(Attributes.Persistent), false))
                    {
                        _tableName = ((Attributes.Persistent)attribute).DataBaseObject;
                    }

                }
                return _tableName;
            }
            set
            {
                _tableName = value;
            }
        }

        [Attributes.NonPersistent()]
        [System.Web.Script.Serialization.ScriptIgnore]
        [System.Xml.Serialization.XmlIgnore()]
        internal string ConnectionName
        {
            get
            {
                string returnValue = "cmsdb";
                foreach (object attribute in this.GetType().GetCustomAttributes(typeof(Attributes.DataConnection), false))
                {
                    returnValue = ((Attributes.DataConnection)attribute).DataBaseConnectionName;
                }
                return returnValue;
            }
        }
        [Attributes.NonPersistent()]
        [System.Web.Script.Serialization.ScriptIgnore]
        [System.Xml.Serialization.XmlIgnore()]
        public bool IsLoaded
        {
            get { return _isLoaded; }
            set { _isLoaded = value; }
        }

        public BaseObject()
        {
            db = DataBase.Get(this.ConnectionName);
            _sqlCreator = SqlCreator.Get();
        }

       
        /// <summary>
        /// Function in ORM: Hiermee kun je object ophalen uit en opslaan in een andere database dan de standaard database zoals die in de web.config staat.
        /// Vul je hier een lege string in als parameter, dan wordt de default database connectie genomen
        /// </summary>
        /// <param name="connectionString"></param>
        public void SetConnectionString(string connectionString)
        {
            db = DataBase.Get("mysql", connectionString);
        }
        //public static virtual BaseObject New()
        //{
        //    return new BaseObject();
        //}

        public virtual void Save()
        {
            this.Save(true);
        }
        public virtual void Save(bool saveChildrenAlso = true)
        {

            string sql = "";
            //als er geen database is, zoals bij remote WCF connectie, dan niks doen
            if (db == null) return;
            this.ModifiedDate = DateTime.Now;
            if (IsNew)
            {
                this.ID = (this.ID == Guid.Empty) ? Guid.NewGuid() : this.ID;
                this.CreateDate = DateTime.Now;
                sql = _sqlCreator.CreateInsertStatement(this);
                db.Execute(sql);
                //this.ID = new Guid(id.ToString());
                
            }
            else
            {
                sql = _sqlCreator.CreateUpdateStatement(this);
                db.Execute(sql);
            }

            if (saveChildrenAlso)
            {
                saveChildren();
            }
            this.IsNew = false;
        }

        private void saveChildren()
        {

            Type type = this.GetType();
            foreach (PropertyInfo prop in type.GetProperties())
            {
                if (prop.PropertyType.FullName.Contains("BaseCollection"))
                {
                    bool persist = true;
                    foreach (object attribute in prop.GetCustomAttributes(typeof(Attributes.NonPersistent), false))
                    {
                        persist = false;
                    }
                    if (persist)
                    {
                        string tableName = "";
                        string foreignObjectName = "";
                        string thisForeignKey = "";
                        string collectionForeignKey = "";

                        foreach (object attribute in prop.GetCustomAttributes(typeof(Attributes.Persistent), false))
                        {
                            tableName = ((Attributes.Persistent)attribute).DataBaseObject;
                        }
                        foreach (object attribute in prop.GetCustomAttributes(typeof(Attributes.Association), false))
                        {
                            foreignObjectName = ((Attributes.Association)attribute).ForeignObjectName;
                            thisForeignKey = ((Attributes.Association)attribute).ForeignKey;
                            collectionForeignKey = ((Attributes.Association)attribute).CollectionForeignKey;
                        }
                        if (tableName != "" && thisForeignKey != "" && collectionForeignKey != "")
                        {
                            //MANY TO MANY
                            object value;
                            try
                            {
                                value = prop.GetValue(this, null);
                            }
                            catch (Exception ex)
                            {
                                throw new Exception("ORM child read exception ('" + prop.Name + "').") { Source = ex.Source, HelpLink = ex.HelpLink };
                            }

                            //eerst alle weggooien
                            string sql = string.Format("DELETE FROM {0} WHERE {1} = '{2}' AND {3} IS NOT NULL;",
                                    tableName, thisForeignKey, this.ID, collectionForeignKey);
                            db.Execute(sql);

                            if (value != null)
                            {
                                IEnumerable list = (IEnumerable)value;
                                foreach (BaseObject obj in list)
                                {
                                    sql = string.Format("INSERT INTO {0} (ID, {1}, {2}) Values(UUID(), '{3}','{4}')",
                                        tableName, thisForeignKey, collectionForeignKey, this.ID, obj.ID);
                                    db.Execute(sql);
                                }
                                PropertyInfo isLoadedProp = value.GetType().GetProperty("IsLoaded");
                                isLoadedProp.SetValue(value, false, null);
                            }
                        }
                        else
                        {
                            //ONE TO MANY
                            object value = prop.GetValue(this, null);
                            if (value != null)
                            {

                                IEnumerable list = (IEnumerable)value;
                                foreach (BaseObject obj in list)
                                {
                                    //als er in de childeren een verwijzing is naar de parent dan wordt die hier gezet 
                                    PropertyInfo parentObject = obj.GetType().GetProperty(foreignObjectName);
                                    if (parentObject == null)
                                    {
                                        parentObject = obj.GetType().GetProperty(this.GetType().Name);
                                    }
                                    if (parentObject == null)
                                    {
                                        parentObject = obj.GetType().GetProperty(this.GetType().BaseType.Name);
                                    }
                                    if (parentObject != null)
                                    {
                                        parentObject.SetValue(obj, this, null);
                                    }
                                    //kijk of clild-object een ref naar Site heeft
                                    PropertyInfo siteParentObject = obj.GetType().GetProperty("Site");
                                    if (siteParentObject != null)
                                    {
                                        //zo ja, kijk of die is gevuld. Zo nee, vullen met site uit parent
                                        if (siteParentObject.GetValue(obj, null) == null)
                                        {
                                            Object site = this.GetType().GetProperty("Site").GetValue(this, null);
                                            siteParentObject.SetValue(obj, site, null);
                                        }
                                    }
                                    obj.Save(saveChildrenAlso: false);
                                }
                            }
                        }
                    }
                }
            }
        }

        public virtual void Delete()
        {
            //als er geen database is, zoals bij remote WCF connectie, dan niks doen
            if (db == null) return;
            string sql = "";
            if (!IsNew)
            {
                sql = _sqlCreator.CreateDeleteStatement(this);
            }
            db.Execute(sql);

        }

        public static void DeleteById<T>(Guid id, string connectionString = "") where T : BaseObject, new()
        {
            T t = new T();
            t.ID = id;
            if (connectionString != "")
            {
                t.db = DataBase.Get("mysql", connectionString);
            }
            t.Load();
            t.Delete();
        }

        public static T GetById<T>(Guid id) where T : BaseObject, new()
        {
            return GetById<T>(id, "");
        }

        public static T GetById<T>(Guid id, string connectionString = "") where T : BaseObject, new()
        {
            return GetById<T>(id, "", connectionString);
        }

        public static T GetById<T>(Guid id, string tableName, string connectionString = "") where T : BaseObject, new()
        {
            T t = new T();
            t.ID = id;
            if (tableName != "")
            {
                t.TableName = tableName;
            }
            if (connectionString != "")
            {
                t.db = DataBase.Get("mysql", connectionString);
            }
            //t.DatabaseName = databaseName;
            t.Load();
            if (t.IsLoaded)
            {
                return t;
            }
            else
            {
                return null;
            }
        }
        public static T GetFirst<T>(string where) where T : BaseObject, new()
        {
            return GetFirst<T>(where, "");
        }
        public static T GetFirst<T>(string where, string orderby) where T : BaseObject, new()
        {
            return GetFirst<T>(where, orderby, "");
        }

        public static T GetFirst<T>(string where , string orderby, string tableName) where T : BaseObject, new()
        {
            BaseCollection<T> collection = BaseCollection<T>.Get(where, orderby, tableName: tableName);
            if (collection.Count > 0)
            {
                return collection[0];
            }
            else
            {
                return null;
            }
        }

        public virtual void Load()
        {
            if (this.ID == Guid.Empty)
            {
                LoadNew();
            }
            else
            {
                //als er geen database is, zoals bij remote WCF connectie, dan niks doen
                if (db == null) return;
                Type type = this.GetType();
                

                string sql = _sqlCreator.CreateSelectByIDStatement(this, this.ID);
                DataRowCollection rows = db.GetDataTable(sql).Rows;
                if (rows.Count == 1)
                {
                    DataRow dataRow = rows[0];

                    FillObject(type, dataRow);
                }
            }
        }

        public virtual void LoadNew()
        {
        }
        public virtual void FillObject(DataRow dataRow)
        {
            if (dataRow.Table.Columns.Contains("ID")) this.ID = new Guid(dataRow["ID"].ToString());
            if (dataRow.Table.Columns.Contains("Name")) this.Name = dataRow["Name"].ToString();
            if (dataRow.Table.Columns.Contains("CreateDate")) this.CreateDate = DataConverter.ToDateTime(dataRow["CreateDate"]);
            if (dataRow.Table.Columns.Contains("ModifiedDate")) this.ModifiedDate = DataConverter.ToDateTime(dataRow["ModifiedDate"]);

            /* this.ID = new Guid(dataRow["ID"].ToString());
            this.Name = dataRow["Name"].ToString();
            this.CreateDate = DataConverter.ToDateTime(dataRow["CreateDate"]);
            this.ModifiedDate = DataConverter.ToDateTime(dataRow["ModifiedDate"]); */
            
        }
        public virtual void FillObject(Type type, DataRow dataRow)
        {
            FillObject(dataRow);
            if (IsLoaded) return;
            foreach (PropertyInfo prop in type.GetProperties())
            {
                bool nonPersitent = false;
                foreach (object attribute in prop.GetCustomAttributes(typeof(Attributes.NonPersistent), false))
                {
                    nonPersitent = true;
                }
                if (!nonPersitent)
                {

                    string fieldName = prop.Name;
                    foreach (object attribute in prop.GetCustomAttributes(typeof(Attributes.Persistent), false))
                    {
                        fieldName = ((Attributes.Persistent)attribute).DataBaseObject;
                    }

                    if (isBaseObject(prop.PropertyType))
                    {
                        string fkName = ""; // "FK_" + prop.Name;
                        foreach (object attribute in prop.GetCustomAttributes(typeof(Attributes.Association), false))
                        {
                            fkName = ((Attributes.Association)attribute).ForeignKey;
                        }

                        if (dataRow.Table.Columns.Contains(fkName) && (dataRow[fkName] != System.DBNull.Value) && dataRow[fkName].ToString() != "")
                        {
                            try
                            {
                                Guid fk = new Guid(dataRow[fkName].ToString());
                                //alleen de id zetten van het gerelateerde object.
                                //Later kan die (lazy) worden geladen


                                object initObject = getInitBaseObject(fk, prop.PropertyType);
                                prop.SetValue(this, initObject, null);
                                //if (baseobjectValue != System.DBNull.Value)
                                //{
                                //    prop.SetValue(this, baseobjectValue, null);
                                //}
                            }
                            catch { }
                        }
                    }
                    else if (dataRow.Table.Columns.Contains(fieldName))
                    {
                        object value = dataRow[fieldName];
                        if (prop.PropertyType == typeof(System.Boolean))
                        {
                            value = (value.ToString() == "1" || value.ToString() == "True") ? true : false;
                        }
                        else if (prop.PropertyType == typeof(System.Guid))
                        {
                            if (value != System.DBNull.Value)
                            {
                                value = new Guid(value.ToString());
                            }
                        }
                        else if (prop.PropertyType.IsArray)
                        {
                            string[] values = value.ToString().Split(new char[] { ',' });
                            if (prop.PropertyType == typeof(System.String[]))
                            {
                                value = values;
                            }
                            else if (prop.PropertyType == typeof(System.Int32[]))
                            {
                                int[] ints = values.Where(x => x != "").Select(x => int.Parse(x)).ToArray();
                                value = ints;
                            }
                        }
                        //else if (prop.PropertyType == typeof(System.String[]))
                        //{
                        //    value = value.ToString().Split(new char[] { ',' });
                        //}
                        if (value != System.DBNull.Value)
                        {
                            try
                            {
                                prop.SetValue(this, value, null);
                            }
                            catch (Exception ex)
                            {
                                Logging.SqlLogger.WriteSql(ex.ToString());
                            }
                            
                        }
                    }
                }
            }
            IsLoaded = true;
        }

        private BaseObject getInitBaseObject(Guid fk, Type type)
        {
            Assembly assembly = type.Assembly;
            BaseObject obj = (BaseObject)assembly.CreateInstance(type.FullName);
            obj.ID = fk;
            obj.IsLoaded = false;
            //obj.Load();
            return obj;
        }

        private BaseObject loadBaseObject(Guid fk, Type type)
        {
            Assembly assembly = type.Assembly;
            BaseObject obj = (BaseObject)assembly.CreateInstance(type.FullName);
            obj.ID = fk;
            obj.Load();
            return obj;
        }

        public static T LoadBaseObjectLazy<T>(BaseObject owner, string fk) where T : BaseObject, new()
        {
            DataBase db = DataBase.Get();

            T t = new T();


            string sql = "SELECT T1.* FROM " + t.TableName + " AS T1 INNER JOIN " + owner.TableName + " AS T2 ON T2." + fk + " = T1.ID WHERE T2.ID = " + owner.ID;
            //Assembly assembly = type.Assembly;
            //BaseObject obj = (BaseObject)assembly.CreateInstance(type.FullName);
            //obj.Load(fk);
            //return obj;

            if (db.GetDataTable(sql).Rows.Count == 1)
            {
                DataRow dataRow = db.GetDataTable(sql).Rows[0];

                t.FillObject(t.GetType(), dataRow);
                return t;
            }
            else
            {
                return null;
            }


        }

        private bool isBaseObject(Type type)
        {
            if (type == typeof(BaseObject))
            {
                return true;
            }
            else if (type == typeof(object))
            {
                return false;
            }
            else if (type.BaseType == null) //voor interfaces
            {
                return false;
            }
            else
            {
                return isBaseObject(type.BaseType);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            else
            {
                return this.ID == ((BaseObject)obj).ID;
            }
        }

        public override string ToString()
        {
            return this.GetType().Name + " ID=" + ID;
        }

        public virtual string CreateBackupSQL()
        {
            string sql = _sqlCreator.CreateBackupStatement(this, "", "");
            return sql;
        }

        public virtual string CreateBackupSQL(string FKName, string FKParameterName)
        {
            string sql = _sqlCreator.CreateBackupStatement(this, FKName, FKParameterName);
            return sql;
        }

        public virtual T CreateCopy<T>() where T : BaseObject, new()
        {
            return CreateCopy<T>(true);
        }

        //public virtual T Copy<T>(bool newName) where T : BaseObject, new()
        //{
        //    return Copy<T>(true, false, false);
        //}

        public virtual T CreateCopy<T>(bool newName) where T : BaseObject, new()
        {
            T t = new T();
            Type type = this.GetType();
            foreach (PropertyInfo prop in type.GetProperties())
            {
                object value = prop.GetValue(this, null);
                if (prop.CanWrite)
                {
                    if (!prop.PropertyType.FullName.Contains("BaseCollection"))
                    {
                        prop.SetValue(t, value, null);
                    }
                }

            }
            t.ID = Guid.Empty;
            t.CreateDate = DateTime.Now;
            if (newName)
            {
                t.Name = getNewName(this);
            }
            //if (save)
            //{
            //    t.Save();
            //}
            //if (copyChildren)
            //{
            //    copyAndSaveChildren(this, t);
            //}
            return t;
        }

        //private void copyAndSaveChildren(BaseObject baseObj, BaseObject newParentObj)
        //{
        //    Type type = baseObj.GetType();
        //    foreach (PropertyInfo prop in type.GetProperties())
        //    {
        //        if (prop.PropertyType.FullName.Contains("BaseCollection"))
        //        {
        //            object value = prop.GetValue(baseObj, null);
        //            IEnumerable list = (IEnumerable)value;
        //            foreach (BaseObject obj in list)
        //            {
        //                MethodInfo method = obj.GetType().GetMethod("Copy", new Type[] { typeof(bool), typeof(bool), typeof(bool) });
        //                MethodInfo genericMethod = method.MakeGenericMethod(new Type[] { obj.GetType() });
        //                BaseObject newObj = (BaseObject)genericMethod.Invoke(obj, new object[] { false, false, false });

        //                newObj = SetNewAssociatedParentObjects(newObj, newParentObj);


        //                newObj.Save();
        //                copyAndSaveChildren(obj, newObj);
        //            }
        //        }
        //    }
        //}

        //private BaseObject SetNewAssociatedParentObjects(BaseObject newObj, BaseObject newParentObj)
        //{
        //    Type type = newObj.GetType();
        //    foreach (PropertyInfo prop in type.GetProperties())
        //    {
        //        string parentPropertyName = "";
        //        foreach (object attribute in prop.GetCustomAttributes(typeof(Attributes.Association), false))
        //        {
        //            parentPropertyName = ((Attributes.Association)attribute).ParentPropertyName;
        //        }
        //        if (parentPropertyName != "")
        //        {
        //            //als er in de childeren een verwijzing is naar de parent dan wordt die hier gezet 
        //            PropertyInfo parentObject = newObj.GetType().GetProperty(parentPropertyName);

        //            if (parentObject != null)
        //            {
        //                parentObject.SetValue(newObj, newParentObj, null);
        //            }
        //        }
        //    }
        //    return newObj;
        //}

        private string getNewName(BaseObject obj)
        {
            string newName = "";
            string name = obj.Name;
            //name = name.Replace(" (kopie)", "");
            //for (int i = 1; i < 100; i++)
            //{
            //    name = name.Replace(" (kopie" + i + ")", "");
            //}
            newName = name + " (kopie)";
            string sql = String.Format("Select * from {0} where name='{1}'", obj._tableName, newName);
            object values = this.db.Execute(sql);
            int index = 1;
            while (values != null)
            {
                index++;
                newName = name + " (kopie " + index + ")";
                sql = String.Format("Select * from {0} where name='{1}'", obj._tableName, newName);
                values = this.db.Execute(sql);
            }
            return newName;
        }
    }
}
