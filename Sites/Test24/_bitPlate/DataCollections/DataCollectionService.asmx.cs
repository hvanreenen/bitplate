using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Web.Services;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using System.IO;

using HJORM;
using BitPlate.Domain;
using BitPlate.Domain.DataCollections;
using BitPlate.Domain.Utils;
using BitSite._bitPlate._bitSystem;
using System.Reflection;


namespace BitSite._bitPlate.DataCollections
{
    [System.Web.Script.Services.ScriptService]
    public partial class DataCollectionService : BaseService
    {
        static HttpPostedFile fileToImport = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["download"] != null)
            {
                string id = Request.QueryString["download"];
                DataCollection collection = BaseObject.GetById<DataCollection>(new Guid(id));
                string fileName = collection.Name + ".csv";


                System.IO.MemoryStream stream = new System.IO.MemoryStream();
                System.IO.StreamWriter writer = new System.IO.StreamWriter(stream, System.Text.Encoding.Unicode);

                //if (webshopMode)
                //{
                //    ((WebshopDataCollection)dataModule.ConvertToType()).MakeDownload(writer);
                //}
                //else
                //{
                //collection.MakeDownload(writer);
                //}

                writer.Flush();
                // Convert the memory stream to an array of bytes.
                byte[] byteArray = stream.ToArray();

                // Send the file to the web browser for download.
                Response.Clear();
                Response.AppendHeader("Content-Disposition", "filename=" + fileName);
                Response.AppendHeader("Content-Length", byteArray.Length.ToString());
                Response.ContentType = "application/octet-stream";
                Response.ContentType = "";
                Response.BinaryWrite(byteArray);
                writer.Close();
            }
            else
            {
                HttpFileCollection files = Request.Files;

                string[] arr1 = files.AllKeys;  // This will get names of all files into a string array.
                for (int loop1 = 0; loop1 < arr1.Length; loop1++)
                {

                    DataCollectionService.fileToImport = files[loop1];
                }

                Response.Write("File: " + Server.HtmlEncode(DataCollectionService.fileToImport.FileName) + "<br />");
                Response.Write("  size: " + DataCollectionService.fileToImport.ContentLength + "<br />");
                Response.Write("<br />");

            }
        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<TreeGridItem> GetDataCollections(string sort, string searchString)
        {
            BaseService.CheckLoginAndLicense();
            string where = String.Format("FK_Site='{0}'", SessionObject.CurrentSite.ID);
            if (searchString != null && searchString != "")
            {
                where += String.Format("AND Name like '%{0}%'", searchString);
            }
            List<TreeGridItem> returnList = new List<TreeGridItem>();
            BaseCollection<DataCollection> collections = BaseCollection<DataCollection>.Get(where, sort);

            foreach (DataCollection collection in collections)
            {
                TreeGridItem item = TreeGridItem.NewItem<DataCollection>(collection);
                item.Icon = ""; //TODO laten afhangen van type
                item.Type = collection.TypeString;
                item.Status = collection.ChangeStatusString;
                item.LanguageCode = collection.LanguageCode;
                item.HasAutorisation = collection.HasAutorisation;
                if (searchString != null && searchString != "")
                {
                    item.Name = item.Name.Replace(searchString, "<span class='highlight'>" + searchString + "</span>");
                    //item.Title = item.Title.Replace(searchString, "<span class='highlight'>" + searchString + "</span>");
                }
                returnList.Add(item);
            }
            return returnList;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public DataCollection GetDataCollection(string id)
        {
            BaseService.CheckLoginAndLicense();
            DataCollection collection = null;
            if (id == null)
            {
                collection = new DataCollection();
                collection.Site = SessionObject.CurrentSite;
            }
            else
            {
                collection = BaseObject.GetById<DataCollection>(new Guid(id));
                if (collection.HasAutorisation)
                {

                    if (!collection.IsAutorized(SessionObject.CurrentBitplateUser))
                    {
                        throw new Exception("U heeft geen rechten voor deze datacollectie.");
                    }
                }
            }
            return collection;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public DataCollection SaveDataCollection(DataCollection obj)
        {
            BaseService.CheckLoginAndLicense();
            obj.Site = SessionObject.CurrentSite;
            obj.Save();
            return obj;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void DeleteDataCollection(string id)
        {
            BaseService.CheckLoginAndLicense();
            DataCollection collection = BaseObject.GetById<DataCollection>(new Guid(id));
            if (collection.HasAutorisation)
            {
                if (!collection.IsAutorized(SessionObject.CurrentBitplateUser))
                {
                    throw new Exception("U heeft geen rechten voor deze datacollectie.");
                }
            }
            collection.Delete();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<TreeGridItem> GetData(string datacollectionid, string groupid, string sort, string searchString, int pageNumber, int pageSize)
        {
            BaseService.CheckLoginAndLicense();
            if (sort == "" || sort == null)
            {
                sort = "Name";
            }
            string sortItems = sort;
            string sortGroups = sort;

            string whereItems = String.Format("FK_DataCollection = '{0}' AND FK_Parent_Group='{1}'", datacollectionid, groupid);
            string whereGroups = String.Format("FK_DataCollection = '{0}' AND FK_Parent_Group='{1}'", datacollectionid, groupid);
            if (groupid == null || groupid == "" || groupid == Guid.Empty.ToString())
            {
                whereItems = String.Format("FK_DataCollection = '{0}' AND FK_Parent_Group Is Null", datacollectionid);
                whereGroups = String.Format("FK_DataCollection = '{0}' AND FK_Parent_Group Is Null", datacollectionid);
            }
            if (searchString != null && searchString != "")
            {
                whereItems = String.Format("FK_DataCollection = '{0}' AND (Name like '%{1}%' OR Title like '%{1}%')", datacollectionid, searchString);
                whereGroups = String.Format("FK_DataCollection = '{0}' AND (Name like '%{1}%')", datacollectionid, searchString);

            }
            BaseCollection<DataGroup> grouplist = BaseCollection<DataGroup>.Get(whereGroups, sortGroups, pageNumber, pageSize);
            BaseCollection<DataItem> itemslist = BaseCollection<DataItem>.Get(whereItems, sortItems, pageNumber, pageSize);
            List<TreeGridItem> returnList = new List<TreeGridItem>();
            if (!(groupid == null || groupid == "" || groupid == Guid.Empty.ToString()))
            {
                //voeg de move up folder toe
                DataGroup group = BaseObject.GetById<DataGroup>(new Guid(groupid));
                TreeGridItem item = new TreeGridItem();
                if (group.ParentGroup != null)
                {
                    item = TreeGridItem.NewGroup<DataGroup>(group.ParentGroup);
                }
                else
                {
                    item.ID = Guid.Empty;
                    item.IsLeaf = false;
                    item.Type = "Group";
                }
                item.Name = "...";
                item.Status = group.ChangeStatusString;

                returnList.Add(item);
            }
            foreach (DataGroup group in grouplist)
            {
                TreeGridItem item = TreeGridItem.NewGroup<DataGroup>(group);
                item.Title = group.Title;
                item.Field1 = group.OrderNumber.ToString();
                if (searchString != null && searchString != "")
                {
                    item.Name = StringHelper.HighlightSearchResults(item.Name, searchString);
                    //item.Name = item.Name.Replace(searchString, "<span class='highlight'>" + searchString + "</span>");
                }
                item.Status = group.ChangeStatusString;
                returnList.Add(item);
            }
            foreach (DataItem dataItem in itemslist)
            {
                TreeGridItem item = TreeGridItem.NewItem<DataItem>(dataItem);
                item.Title = dataItem.Title;
                item.Field1 = dataItem.OrderNumber.ToString();
                if (searchString != null && searchString != "")
                {
                    item.Name = StringHelper.HighlightSearchResults(item.Name, searchString);
                    item.Title = StringHelper.HighlightSearchResults(item.Title, searchString);
                    //item.Name = item.Name.Replace(searchString, "<span class='highlight'>" + searchString + "</span>");
                    //item.Title = item.Title.Replace(searchString, "<span class='highlight'>" + searchString + "</span>");
                }
                item.Status = dataItem.ChangeStatusString;
                returnList.Add(item);
            }
            return returnList;
        }

        

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<TreeGridItem> GetGroups(string datacollectionid, string groupid, string groupPath, string sort, string searchString, int pageNumber, int pageSize)
        {
            BaseService.CheckLoginAndLicense();
            if ((groupid == null || groupid == "") &&
                (groupPath != null && groupPath != ""))
            {
                //haal folder id op vanuit path
                //path wordt gebruikt als er vanuit de breadcrumb wordt genavigeerd
                string where = String.Format("CompletePath = '{0}'", groupPath);
                DataGroup group = BaseObject.GetFirst<DataGroup>(where);
                groupid = group.ID.ToString();
            }
            if (sort == "" || sort == null)
            {
                sort = "OrderNumber";
            }
            string whereGroups = String.Format("FK_DataCollection = '{0}' AND FK_Parent_Group='{1}'", datacollectionid, groupid);
            if (groupid == null || groupid == "" || groupid == Guid.Empty.ToString())
            {
                whereGroups = String.Format("FK_DataCollection = '{0}' AND FK_Parent_Group Is Null", datacollectionid);
            }
            if (searchString != null && searchString != "")
            {
                whereGroups = String.Format("FK_DataCollection = '{0}' AND (Name like '%{1}%')", datacollectionid, searchString);
            }
            BaseCollection<DataGroup> grouplist = BaseCollection<DataGroup>.Get(whereGroups, sort, pageNumber, pageSize);
            List<TreeGridItem> returnList = new List<TreeGridItem>();
            if (!(groupid == null || groupid == "" || groupid == Guid.Empty.ToString()))
            {
                //voeg de move up folder toe
                DataGroup group = BaseObject.GetById<DataGroup>(new Guid(groupid));
                TreeGridItem item = new TreeGridItem();
                if (group.ParentGroup != null)
                {
                    item = TreeGridItem.NewGroup<DataGroup>(group.ParentGroup);
                    item.Path = group.ParentGroup.CompletePath;
                }
                else
                {
                    item.ID = Guid.Empty;
                    item.IsLeaf = false;
                    item.Type = "Group";
                    item.Path = "";
                }
                item.Name = "...";

                item.Status = group.ChangeStatusString;

                returnList.Add(item);
            }
            foreach (DataGroup group in grouplist)
            {
                TreeGridItem item = TreeGridItem.NewGroup<DataGroup>(group);
                item.Title = group.Title;
                item.Field1 = group.OrderNumber.ToString();
                item.Path = group.CompletePath;
                
                if (searchString != null && searchString != "")
                {
                    if (group.ParentGroup != null)
                    {
                        item.Field2 = group.ParentGroup.CompletePath;
                        item.Field3 = group.ParentGroup.ID.ToString();
                    }
                    //item.Name = item.Name.Replace(searchString, "<span class='highlight'>" + searchString + "</span>");
                    item.Name = StringHelper.HighlightSearchResults(item.Name, searchString);
                }
                item.Status = group.ChangeStatusString;
                returnList.Add(item);
            }
            return returnList;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<TreeGridItem> GetItems(string datacollectionid, string groupid, string groupPath, string sort, string searchString, int pageNumber, int pageSize)
        {
            BaseService.CheckLoginAndLicense();
            if ((groupid == null || groupid == "") &&
                (groupPath != null && groupPath != ""))
            {
                //haal folder id op vanuit path
                //path wordt gebruikt als er vanuit de breadcrumb wordt genavigeerd
                string where = String.Format("CompletePath = '{0}'", groupPath);
                DataGroup group = BaseObject.GetFirst<DataGroup>(where);
                groupid = group.ID.ToString();
            }

            if (sort == "" || sort == null)
            {
                sort = "OrderNumber";
            }

            string whereItems = String.Format("FK_DataCollection = '{0}' AND FK_Parent_Group='{1}'", datacollectionid, groupid);
            if (groupid == null || groupid == "" || groupid == Guid.Empty.ToString())
            {
                whereItems = String.Format("FK_DataCollection = '{0}' AND FK_Parent_Group Is Null", datacollectionid);
            }
            if (searchString != null && searchString != "")
            {
                whereItems = String.Format("FK_DataCollection = '{0}' AND (Name like '%{1}%' OR Title like '%{1}%')", datacollectionid, searchString);
            }
            BaseCollection<DataItem> itemslist = BaseCollection<DataItem>.Get(whereItems, sort, pageNumber, pageSize);
            List<TreeGridItem> returnList = new List<TreeGridItem>();


            foreach (DataItem dataItem in itemslist)
            {
                TreeGridItem item = TreeGridItem.NewItem<DataItem>(dataItem);
                item.Title = dataItem.Title;
                item.Field1 = dataItem.OrderNumber.ToString();
                

                if (searchString != null && searchString != "")
                {
                    if (dataItem.ParentGroup != null)
                    {
                        item.Field2 = dataItem.ParentGroup.CompletePath;
                        item.Field3 = dataItem.ParentGroup.ID.ToString();
                    }

                    //item.Name = item.Name.Replace(searchString, "<span class='highlight'>" + searchString + "</span>");
                    //item.Title = item.Title.Replace(searchString, "<span class='highlight'>" + searchString + "</span>");
                    item.Name = StringHelper.HighlightSearchResults(item.Name, searchString);
                    item.Title = StringHelper.HighlightSearchResults(item.Title, searchString);
                }
                item.Status = dataItem.ChangeStatusString;
                returnList.Add(item);
            }
            return returnList;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public int GetDataTotalCount(string datacollectionid, string groupid, string searchString)
        {
            BaseService.CheckLoginAndLicense();
            string whereItems = String.Format("FK_DataCollection = '{0}' AND FK_Parent_Group='{1}'", datacollectionid, groupid);
            string whereGroups = String.Format("FK_DataCollection = '{0}' AND FK_Parent_Group='{1}'", datacollectionid, groupid);
            if (groupid == null || groupid == "" || groupid == Guid.Empty.ToString())
            {
                whereItems = String.Format("FK_DataCollection = '{0}' AND FK_Parent_Group Is Null", datacollectionid);
                whereGroups = String.Format("FK_DataCollection = '{0}' AND FK_Parent_Group Is Null", datacollectionid);
            }
            if (searchString != null && searchString != "")
            {
                whereItems = String.Format("FK_DataCollection = '{0}' AND (Name like '%{1}%' OR Title like '%{1}%')", datacollectionid, searchString);
                whereGroups = String.Format("FK_DataCollection = '{0}' AND (Name like '%{1}%')", datacollectionid, searchString);

            }
            BaseCollection<DataGroup> grouplist = BaseCollection<DataGroup>.Get(whereGroups);
            BaseCollection<DataItem> itemslist = BaseCollection<DataItem>.Get(whereItems);
            return (grouplist.Count + itemslist.Count);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<TreeGridItem> GetAllGroups_old(string datacollectionid)
        {
            BaseService.CheckLoginAndLicense();
            string where = String.Format("FK_DataCollection = '{0}'", datacollectionid);
            BaseCollection<DataGroup> groups = BaseCollection<DataGroup>.Get(where, "CompletePath");
            List<TreeGridItem> list = new List<TreeGridItem>();
            foreach (DataGroup group in groups)
            {
                TreeGridItem item = new TreeGridItem();
                item.ID = group.ID;
                item.Name = group.CompletePath;
                list.Add(item);
            }
            return list;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public BaseCollection<DataGroup> GetAllGroups(string datacollectionid)
        {
            BaseService.CheckLoginAndLicense();
            string where = String.Format("FK_DataCollection = '{0}'", datacollectionid);
            BaseCollection<DataGroup> groups = BaseCollection<DataGroup>.Get(where, "CompletePath");
            return groups;
        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public BaseCollection<DataItem> GetAllItems(string datacollectionid)
        {
            BaseService.CheckLoginAndLicense();
            string where = String.Format("FK_DataCollection = '{0}'", datacollectionid);
            BaseCollection<DataItem> items = BaseCollection<DataItem>.Get(where, "CompletePath");
            return items;
        }
        static string treeHtml = "";
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetTreeHtml(Guid datacollectionID, string searchString)
        {
            if (Guid.Empty != datacollectionID)
            {
                BaseService.CheckLoginAndLicense();
                treeHtml = "";

                buildGroup(datacollectionID, null);

                treeHtml += "</li></ul>";
                return treeHtml;
            }
            else
            {
                return "";
            }
        }

        private static void buildGroup(Guid datacollectionID, BaseDataObject group)
        {

            DataCollection collection = BaseObject.GetById<DataCollection>(datacollectionID);
            if (treeHtml == "")
            {
                //treeHtml += "<a id='' href='javascript:BITBACKEND.loadDataCollections();'>terug</a>";
                treeHtml += String.Format("<ul><li id='{0}'><a id='{0}' class='bitTreeGroupIcon' data-groupid=''>{1}</a><ul id='treeRoot'>", collection.ID, collection.Name.ToUpper());

            }
            string displayString = "";
            if (group != null)
            {
                displayString = group.Name;
            }

            //zowel groepen als items krijgen volgorde in de tree
            //daarom union query waarbij groepen en item in 1 query staan.
            //veld title misbruiken om hier type (groep of item) in te bewaren 
            string sql = "";
            /* if (group == null)
            {
                sql = String.Format(@"SELECT ID, Name, 'Item' AS Title, OrderingNumber, DateFrom, DateTill From DataItem WHERE FK_DataCollection='{0}' AND FK_Parent_Group IS NULL 
UNION SELECT ID, Name, 'Group' AS Title, OrderingNumber, DateFrom, DateTill From DataGroup WHERE FK_DataCollection='{0}' AND FK_Parent_Group IS NULL ORDER BY OrderingNumber, Name", datacollectionID);
            }
            else
            {
                sql = String.Format(@"SELECT ID, Name, 'Item' AS Title, OrderingNumber, DateFrom, DateTill From DataItem WHERE FK_DataCollection='{0}' AND FK_Parent_Group = '{1}' 
UNION SELECT ID, Name, 'Group' AS Title, OrderingNumber, DateFrom, DateTill From DataGroup WHERE FK_DataCollection='{0}' AND FK_Parent_Group = '{1}' ORDER BY OrderingNumber, Name", datacollectionID, group.ID);

            } */

            if (group == null)
            {
                sql = String.Format(@"SELECT ID, Name, Active, 'Item' AS Title, OrderNumber, DateFrom, DateTill From DataItem WHERE FK_DataCollection='{0}' AND FK_Parent_Group IS NULL 
UNION SELECT ID, Name, Active, 'Group' AS Title, OrderNumber, DateFrom, DateTill From DataGroup WHERE FK_DataCollection='{0}' AND FK_Parent_Group IS NULL ORDER BY OrderNumber, Name", datacollectionID);
            }
            else
            {
                sql = String.Format(@"SELECT ID, Name, Active, 'Item' AS Title, OrderNumber, DateFrom, DateTill From DataItem WHERE FK_DataCollection='{0}' AND FK_Parent_Group = '{1}' 
UNION SELECT ID, Name, Active, 'Group' AS Title, OrderNumber, DateFrom, DateTill From DataGroup WHERE FK_DataCollection='{0}' AND FK_Parent_Group = '{1}' ORDER BY OrderNumber, Name", datacollectionID, group.ID);

            }

            BaseCollection<BaseDataObject> dataObjects = BaseCollection<BaseDataObject>.LoadFromSql(sql);
            foreach (BaseDataObject dataObject in dataObjects)
            {
                string inactiveClass = "";

                if (!dataObject.IsActive)
                {
                    inactiveClass = "inactive";
                }
                if (dataObject.Title == "Item")
                {
                    //treeHtml += String.Format("<li class='jstree-drag' id='{0}'><a class='bitTreeItemIcon {2}' href='#' data-groupid='{3}'>{1}</a></li>", dataObject.ID, dataObject.Name, inactiveClass, (group != null) ? group.ID.ToString() : "");
                }
                else
                {
                    if (dataObject.IsLeaf())
                    {
                        treeHtml += String.Format("<li class='jstree-drop jstree-drag' id='{0}'><a class='bitTreeGroupIcon {2}' href='#' data-groupid='{0}'>{1}</a></li>", dataObject.ID, dataObject.Name, inactiveClass);
                    }
                    else
                    {
                        treeHtml += String.Format("<li class='jstree-drop jstree-drag' id='{0}'><a class='bitTreeGroupIcon {2}' href='#' data-groupid='{0}'>{1}</a><ul>", dataObject.ID, dataObject.Name, inactiveClass);

                        buildGroup(datacollectionID, dataObject);

                        treeHtml += "</ul></li>";
                    }
                }
            }
        }

        //public string BuildTree(string datacollectionid)
        //{

        //    treeHtml = "<ul><li id=\"root\"><a href=#>ROOT</a><ul>";
        //    string where = String.Format("FK_DataCollection = '{0}' FK_Parent_Group Is Null", datacollectionid);
        //    BaseCollection<DataGroup> parentGroups = BaseCollection<DataGroup>.Get(where, "CompletePath");

        //    foreach (DataGroup group in parentGroups)
        //    {
        //        buildGroup(group);
        //    }
        //    //buildGroup(SessionObject.CurrentSite.Path );
        //    treeHtml += "</ul></ul>";
        //    //treeHtml = "<ul><li id=\"root\"><a href=#>ROOT</a>" + treeHtml + "</ul>";
        //    return treeHtml;
        //}

        //private static void buildGroup(DataGroup group)
        //{


        //        string where = String.Format("FK_Parent_Group ='{1}'", group.ID);
        //        BaseCollection<DataGroup> subGroups = BaseCollection<DataGroup>.Get(where, "CompletePath");


        //        foreach (DataGroup subGroup in subGroups)
        //        {
        //            string displayString = "";
        //            if (folder != null)
        //            {
        //                displayString = group.Name;

        //            }
        //            bool subFolderHasSubFolders = (System.IO.Directory.GetDirectories(subfolder).Length > 0);
        //            if (subFolderHasSubFolders)
        //            {
        //                //treeHtml += String.Format("<li class='treeGroup'><span class='openCloseIcon'>&#9660;</span><span class='groupIcon'><img src='{2}' alt=''/></span><span class='groupText'><a href='javascript:BITFILEMANAGEMENT.getFiles(\"{0}\")'>{1}</a></span><ul>", relativeFolder.Replace("\\", "\\\\"), displayString, _groupIcon);
        //                //treeHtml += String.Format("<li class='treeGroup'><span class='openCloseIcon'>&#9660;</span><span class='groupIcon'><img src='{2}' alt=''/></span><span class='groupText'><a class='folder' id='{0}'>{1}</a></span><ul>", relativeFolder.Replace("\\", "\\\\"), displayString, "");

        //                treeHtml += "<li class=\"jstree-drop jstree-drag\"><a href=\"#\" data-path=\"" + relativeFolder + "\">" + displayString + "</a><ul>";

        //                buildGroup(subfolder);
        //                treeHtml += "</ul></li>";
        //            }
        //            else
        //            {
        //                //treeHtml += String.Format("<li  class='treeGroup'><span class='groupIcon'><img src='{2}' alt=''/></span><span class='groupText'><a href='javascript:BITFILEMANAGEMENT.getFiles(\"{0}\")'>{1}</a></span></li>", relativeFolder.Replace("\\", "\\\\"), displayString, _groupIcon);
        //                //treeHtml += String.Format("<li  class='treeGroup'><span class='groupIcon'><img src='{2}' alt=''/></span><span class='groupText'><a class='folder' id='{0}'>{1}</a></span></li>", relativeFolder.Replace("\\", "\\\\"), displayString, "");
        //                treeHtml += "<li class=\"jstree-drop jstree-drag\"><a href=\"#\" data-path=\"" + relativeFolder + "\">" + displayString + "</a></li>";
        //            }
        //        }

        //}
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public DataItem GetItem(string id, string groupid, string datacollectionid)
        {
            BaseService.CheckLoginAndLicense();
            DataItem item = null;
            if (id == null)
            {
                item = new DataItem();
                item.Site = SessionObject.CurrentSite;
                item.DataCollection = new DataCollection();
                item.DataCollection.ID = new Guid(datacollectionid);
                if (groupid != null && groupid != "")
                {
                    item.ParentGroup = new DataGroup();
                    item.ParentGroup.ID = new Guid(groupid);
                    item.OrderNumber = item.ParentGroup.GetMaxItemOrderNumber() + 1;
                }
                else
                {
                    item.OrderNumber = item.DataCollection.GetMaxItemOrderNumber() + 1;
                }

                this.SetDefaultItemValues(item);
            }
            else
            {
                item = BaseObject.GetById<DataItem>(new Guid(id));
            }
            return item;
        }

        private void SetDefaultItemValues(DataItem item)
        {
            PropertyInfo[] properties = typeof(DataItem).GetProperties();
            foreach (DataField fld in item.DataCollection.DataItemFields)
            {
                try
                {
                    PropertyInfo currentProperty = properties.Where(c => c.Name == fld.MappingColumn).FirstOrDefault();
                    if (currentProperty != null)
                    {
                        if (currentProperty.PropertyType == typeof(string))
                        {
                            currentProperty.SetValue(item, fld.DefaultValue, null);
                        }

                        if (currentProperty.PropertyType == typeof(int))
                        {
                            int i = 0;
                            int.TryParse(fld.DefaultValue, out i);
                            currentProperty.SetValue(item, i, null);
                        }

                        if (currentProperty.PropertyType == typeof(DateTime))
                        {
                            DateTime d = DateTime.Now;
                            DateTime.TryParse(fld.DefaultValue, out d);
                            currentProperty.SetValue(item, d, null);
                        }

                        if (currentProperty.PropertyType == typeof(DataLookupValue))
                        {
                            DataLookupValue dlv = DataLookupValue.GetFirst<DataLookupValue>(" Name = '" + fld.DefaultValue + "'");
                            if (dlv != null)
                            {
                                currentProperty.SetValue(item, dlv, null);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    
                }
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public DataGroup GetGroup(string id, string parentgroupid, string datacollectionid)
        {
            BaseService.CheckLoginAndLicense();
            DataGroup group = null;
            if (id == null)
            {
                group = new DataGroup();
                group.Site = SessionObject.CurrentSite;
                group.DataCollection = new DataCollection();
                group.DataCollection.ID = new Guid(datacollectionid);
                if (parentgroupid != null && parentgroupid != "")
                {
                    group.ParentGroup = new DataGroup();
                    group.ParentGroup.ID = new Guid(parentgroupid);
                    group.OrderNumber = group.ParentGroup.GetMaxGroupOrderNumber() + 1;
                }
                else
                {
                    group.OrderNumber = group.DataCollection.GetMaxGroupOrderNumber() + 1;
                }
            }
            else
            {
                group = BaseObject.GetById<DataGroup>(new Guid(id));
            }
            return group;
        }

        /// <summary>
        /// Alleen de betreffende velden worden over het lijntje gegooid als json string
        /// Hierdoor gaat perfomance omhoog, omdat er anders veel lege velden zouden kunnen zijn.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parentgroupid"></param>
        /// <param name="datacollectionid"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetGroupAsJsonString(string id, string parentgroupid, string datacollectionid)
        {
            BaseService.CheckLoginAndLicense();
            DataGroup group = GetGroup(id, parentgroupid, datacollectionid);
            List<string> fields = new List<string>();
            fields.Add("ID");
            fields.Add("CompletePath");
            fields.Add("Active");
            fields.Add("DateTill");
            fields.Add("DateFrom");
            foreach(DataField datafield in group.DataCollection.DataGroupFields){
                fields.Add(datafield.MappingColumn);
            }
            
            string json = JSONSerializer.Serialize<DataGroup>(group, fields.ToArray());
            
            return json;
            
            
        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetItemAsJsonString(string id, string groupid, string datacollectionid)
        {
            BaseService.CheckLoginAndLicense();
            
            DataItem item = GetItem(id, groupid, datacollectionid);
            
            List<string> fields = new List<string>();
            fields.Add("ID");
            fields.Add("CompletePath");
            fields.Add("Active");
            fields.Add("DateTill");
            fields.Add("DateFrom");
            foreach (DataField datafield in item.DataCollection.DataItemFields)
            {
                fields.Add(datafield.MappingColumn);
            }

            string json = JSONSerializer.Serialize<DataItem>(item, fields.ToArray());
            
            return json;
 
            //string json = "{";
            /* json += String.Format("\"ID\" : \"{0}\", ", item.ID);
            json += String.Format("\"CreateDate\" : \"{0:yyyy-MM-ddTHH:mm:ss.000Z}\", ", item.CreateDate);
            json += String.Format("\"ModifiedDate\" : \"{0:yyyy-MM-ddTHH:mm:ss.000Z}\", ", item.ModifiedDate);
            json += String.Format("\"Name\" : \"{0}\", ", item.Name);
            json += String.Format("\"Title\" : \"{0}\", ", item.Title);
            json += String.Format("\"OrderNumber\" : {0}, ", item.OrderNumber);
            json += String.Format("\"ChangeStatusString\" : \"{0}\", ", item.ChangeStatusString);
            json += String.Format("\"Active\" : {0}, ", (int)item.Active);
            json += String.Format("\"DateFrom\" : \"{0:yyyy-MM-ddTHH:mm:ss.000Z}\", ", item.DateFrom);
            json += String.Format("\"DateTill\" : \"{0:yyyy-MM-ddTHH:mm:ss.000Z}\", ", item.DateTill);
            json += String.Format("\"IsNew\" : \"{0}\", ", item.IsNew);
            //string.format werkt niet met {'s in de string
            json += "\"DataCollection\" : {\"ID\" : \"" + datacollectionid + "\"}, ";
            if (item.ParentGroup != null)
            {
                json += "\"ParentGroup\" : {\"ID\" : \"" + groupid + "\"}, ";
            }
            json += String.Format("\"LastPublishedDate\" : \"{0:yyyy-MM-ddTHH:mm:ss.000Z}\", ", item.LastPublishedDate);
            foreach (DataField field in item.DataCollection.DataItemFields)
            {
                if (field.MappingColumn == "ImageList1" || field.MappingColumn == "FileList1")
                {
                    string jsonSubList = "";
                    BaseCollection<DataFile> files = null;
                    if (field.MappingColumn == "ImageList1")
                    {
                        files = item.ImageList1;
                    }
                    else
                    {
                        files = item.FileList1;
                    }
                    foreach (DataFile file in files)
                    {
                        jsonSubList += "{";
                        jsonSubList += String.Format("\"ID\" : \"{0}\", ", file.ID);
                        jsonSubList += String.Format("\"Name\" : \"{0}\", ", file.Name);
                        jsonSubList += String.Format("\"Url\" : \"{0}\"", file.Url);
                        jsonSubList += "}, ";
                    }
                    //laatste komma eraf
                    if (jsonSubList != "")
                    {
                        jsonSubList = jsonSubList.Substring(0, jsonSubList.Length - 2);
                    }
                    json += String.Format("\"{0}\" : [{1}], ", field.MappingColumn, jsonSubList);
                }
                else
                {
                    Object value = getFieldValue(item, field.MappingColumn);
                    if (value != null)
                    {
                        if (!json.Contains(field.MappingColumn))
                        {
                            json += String.Format("\"{0}\" : \"{1}\", ", field.MappingColumn, value);
                        }
                    }
                }
            }
            json = json.Substring(0, json.Length - 2);
            json += "}";
            return json; */
        }

        private static object getFieldValue(BaseDataObject dataObj, string mappingField)
        {


            System.Reflection.PropertyInfo prop = dataObj.GetType().GetProperty(mappingField);
            if (prop == null) return null;
            Object returnValue = prop.GetValue(dataObj, null);
            return returnValue;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public DataItem SaveItem(DataItem obj)
        {
            BaseService.CheckLoginAndLicense();
            obj.Site = SessionObject.CurrentSite;
            if (obj.ParentGroup != null && obj.ParentGroup.ID == Guid.Empty)
            {
                obj.ParentGroup = null;
            }
            if (obj.DataItemLanguages!= null) obj.DataItemLanguages.IsLoaded = true;
            obj.Save();
            //BitCaching.ClearCache();
            return obj;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void DeleteItem(string id)
        {
            BaseService.CheckLoginAndLicense();
            DataItem item = BaseObject.GetById<DataItem>(new Guid(id));
            item.Delete();
            //BitCaching.ClearCache();
            //BaseObject.DeleteById<DataItem>(new Guid(id));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public DataGroup SaveGroup(DataGroup obj)
        {
            BaseService.CheckLoginAndLicense();
            obj.Site = SessionObject.CurrentSite;
            if (obj.ParentGroup != null && obj.ParentGroup.ID == Guid.Empty)
            {
                obj.ParentGroup = null;
            }
            if (obj.DataGroupLanguages != null) obj.DataGroupLanguages.IsLoaded = true;
            obj.Save();
            //BitCaching.ClearCache();
            return obj;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void DeleteGroup(string id)
        {
            BaseService.CheckLoginAndLicense();
            //BaseObject.DeleteById<DataGroup>(new Guid(id));
            DataGroup group = BaseObject.GetById<DataGroup>(new Guid(id));
            group.Delete();
            //BitCaching.ClearCache();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void RemoveExtraImage(string fileId, string itemId)
        {
            BaseService.CheckLoginAndLicense();
            BaseObject.DeleteById<DataFile>(new Guid(fileId));
        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void SaveOrderingNummerItem_old(string type, string id, string parentid, int orderingnumber)
        {
            BaseService.CheckLoginAndLicense();
            int newOrderingNumber = orderingnumber;
            BaseDataObject dataObj;
            type = type.ToLower();
            if (type == "item")
            {
                dataObj = BaseObject.GetById<DataItem>(new Guid(id));
            }
            else
            {
                dataObj = BaseObject.GetById<DataGroup>(new Guid(id));
            }

            string oldParentID = "treeRoot";
            if (dataObj.ParentGroup != null)
            {
                oldParentID = dataObj.ParentGroup.ID.ToString();
            }

            //oude plek in de groep:
            int oldOrderingNumber = dataObj.OrderNumber;
            //dataObj.DataCollection.MoveItemsAndGroups(dataObj, oldParentID, parentid, oldOrderingNumber, newOrderingNumber);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void UpdateOrderingNummerItem(string itemId, string parentGroupId, string datacollectionId, int newOrderingNumber)
        {
            BaseService.CheckLoginAndLicense();
            if (parentGroupId != "")
            {
                DataGroup group = BaseObject.GetById<DataGroup>(new Guid(parentGroupId));
                group.MoveItems(itemId, newOrderingNumber);
            }
            else
            {
                DataCollection datacollection = BaseObject.GetById<DataCollection>(new Guid(datacollectionId));
                datacollection.MoveItems(itemId, newOrderingNumber);
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void UpdateOrderingNummerGroup(string groupId, string parentGroupId, string datacollectionId, int newOrderingNumber)
        {
            BaseService.CheckLoginAndLicense();
            if (parentGroupId != "" && parentGroupId != Guid.Empty.ToString())
            {
                DataGroup group = BaseObject.GetById<DataGroup>(new Guid(parentGroupId));
                group.MoveGroups(groupId, newOrderingNumber);
            }
            else
            {
                DataCollection datacollection = BaseObject.GetById<DataCollection>(new Guid(datacollectionId));
                datacollection.MoveGroups(groupId, newOrderingNumber);
            }
        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void ImportData(string datacollectionId)
        {
            BaseService.CheckLoginAndLicense();
            if (DataCollectionService.fileToImport == null)
            {
                throw new Exception("Geen bestand geselecteerd");
            }
            DataCollection collection = BaseObject.GetById<DataCollection>(new Guid(datacollectionId));

            System.IO.Stream stream = DataCollectionService.fileToImport.InputStream;
            System.IO.StreamReader reader = new System.IO.StreamReader(stream);

            //collection.Import(reader);
            DataCollectionService.fileToImport = null;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void CopyDataCollection(string datacollectionId, string newName)
        {
            BaseService.CheckLoginAndLicense();
            DataCollection collection = BaseObject.GetById<DataCollection>(new Guid(datacollectionId));
            collection.Copy(newName);


        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void CopyGroup(string groupId, string newName)
        {
            BaseService.CheckLoginAndLicense();
            DataGroup group = BaseObject.GetById<DataGroup>(new Guid(groupId));
            group.Copy(newName, null, null, true);
            //CopyGroup(datacollectionId, parentGroupId, group, true);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void CopyItem(string itemId, string newName)
        {
            BaseService.CheckLoginAndLicense();
            DataItem item = BaseObject.GetById<DataItem>(new Guid(itemId));
            item.Copy(newName, null, null, true);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public DataField SaveDataField(DataField obj)
        {
            BaseService.CheckLoginAndLicense();
            obj.Site = SessionObject.CurrentSite;
            obj.Save();
            return obj;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void DeleteDataField(string id)
        {
            BaseService.CheckLoginAndLicense();
            DataField field = BaseObject.GetById<DataField>(new Guid(id));
            field.Delete();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void DeleteDataLookupValue(string id)
        {
            BaseService.CheckLoginAndLicense();
            DataLookupValue lookupValue = BaseObject.GetById<DataLookupValue>(new Guid(id));
            lookupValue.Delete();
        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public BaseCollection<BaseObject> GetAllGroupsLite(string datacollectionid)
        {
            BaseService.CheckLoginAndLicense();
            BaseCollection<DataGroup> groups = GetAllGroups(datacollectionid);
            BaseCollection<BaseObject> returnValue = new BaseCollection<BaseObject>();
            foreach(DataGroup group in groups){
                BaseObject obj = new BaseObject();
                obj.ID = group.ID;
                obj.Name = group.CompletePath;
                returnValue.Add(obj);
            }
            return returnValue;
        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public BaseCollection<BaseObject> GetAllItemsLite(string datacollectionid)
        {
            BaseService.CheckLoginAndLicense();
            BaseCollection<DataItem> items = GetAllItems(datacollectionid);
            BaseCollection<BaseObject> returnValue = new BaseCollection<BaseObject>();
            foreach (DataItem item in items)
            {
                BaseObject obj = new BaseObject();
                obj.ID = item.ID;
                obj.Name = item.Name;
                returnValue.Add(obj);
            }
            return returnValue;
        }


        //private static void CopyGroup(string datacollectionId, string parentGroupId, DataGroup group, bool newName)
        //{
        //    //DataCollection collection = BaseObject.GetById<DataCollection>(new Guid(datacollectionId));

        //    DataGroup newGroup = group.CreateCopy<DataGroup>(newName);
        //    newGroup.DataCollection = new DataCollection();
        //    newGroup.DataCollection.ID = new Guid(datacollectionId);
        //    if (parentGroupId != "" && parentGroupId != null)
        //    {
        //        newGroup.ParentGroup = new DataGroup();
        //        newGroup.ParentGroup.ID = new Guid(parentGroupId);
        //    }
        //    newGroup.Save();
        //    foreach (DataGroup subgroup in group.SubGroups)
        //    {
        //        CopyGroup(datacollectionId, newGroup.ID.ToString(), subgroup, false);
        //    }
        //    foreach (DataItem item in group.Items)
        //    {
        //        CopyItem(datacollectionId, newGroup.ID.ToString(), item, false);
        //    }
        //}


        //private static void CopyItem(string datacollectionId, string parentGroupId, DataItem item, bool newName)
        //{
        //    DataItem newItem = item.CreateCopy<DataItem>(newName);
        //    newItem.DataCollection = new DataCollection();
        //    newItem.DataCollection.ID = new Guid(datacollectionId);
        //    if (parentGroupId != "")
        //    {
        //        newItem.ParentGroup = new DataGroup();
        //        newItem.ParentGroup.ID = new Guid(parentGroupId);
        //    }
        //    newItem.Save();
        //}
    }

}