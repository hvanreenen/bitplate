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
using System.Linq.Expressions;
using BitSite._bitPlate;

namespace BitSite._bitAjaxServices
{
    [System.Web.Script.Services.ScriptService]
    public partial class GoogleMapsService : BaseService
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// We moeten de items als jsonstring oversturen, want anders is de performance niet goed.
        /// Items bevat veel velden en parent objecten. deze kunnen we er uit laten door zelf een string op te bouwen
        /// 
        /// </summary>
        /// <param name="googleMapsKey"></param>
        /// <param name="datacollectionID"></param>
        /// <param name="geoPoint"></param>
        /// <param name="zoomLevel"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetPlacesAsJsonString(string googleMapsKey, string datacollectionID, GPoint geoPoint, int zoomLevel, string navigationUrl)
        {
            //public static IEnumerable<DataItem> GetPlacesAsJsonString(string googleMapsKey, string datacollectionID, GPoint geoPoint, int zoomLevel)
            //{
            string traceInfo = "";
            try
            {
                int searchRadius = convertToRadius(zoomLevel);
                traceInfo = "1";
                string where = GetWhere(datacollectionID);
                traceInfo = "2";
                IEnumerable<DataItem> items = BaseCollection<DataItem>.Get(where);
                traceInfo = "3";
                //if (items.Count > 500)
                //{
                //    throw new Exception("Er zijn teveel items gevonden die voldoen aan de selectie. Maak de selectie kleiner aub.");
                //}
                BaseCollection<DataItem> locations = new BaseCollection<DataItem>();

                foreach (DataItem item in items)
                {

                    double distance = 0;
                    if (GoogleGeocoder.IsWithinDistance(item, geoPoint.Lat, geoPoint.Long, searchRadius, out distance))
                    {
                        traceInfo = "4";
                        item.Distance = distance;
                        //RewriteUrl=DrillDownUrl van de location wordt ipv de {RewriteUrl} gezet in js. 
                        item.RewriteUrl = item.GetRewriteUrl(navigationUrl, "I");
                        traceInfo = "5";
                        locations.Add(item);
                    }

                }
                //sorteer de lijst op afstand
                locations.Sort(delegate(DataItem loc1, DataItem loc2) { return loc1.Distance.CompareTo(loc2.Distance); });
                //return locations;
                traceInfo = "6";

                string json = ConvertToJson(locations);
                return json;

            }
            catch (Exception ex)
            {
                traceInfo += ex.Message;
            }
            return traceInfo;

            //return locations.Select(locations.CreateNewStatement("Name, Title")); 
        }

        private string ConvertToJson(BaseCollection<DataItem> locations)
        {
            string traceInfo = "a";
            try
            {
                if (locations.Count == 0)
                {
                    return "[]";
                }
                //Object sublist = locations.Select(item => new { item.Name, item.Title, item.Latitude, item.Longitude, item.Distance });
                List<string> fieldsToSerialize = new List<string>(new string[] { "ID", "Distance", "RewriteUrl" });
                traceInfo = "b";
                foreach (DataField field in locations[0].DataCollection.DataItemFields)
                {
                    fieldsToSerialize.Add(field.MappingColumn);

                }
                traceInfo = "d";
                string json = JSONSerializer.Serialize<DataItem>(locations, fieldsToSerialize.ToArray());
                traceInfo = "d";
                return json;
            }
            catch (Exception ex)
            {
                traceInfo += ex.Message;
            }
            return traceInfo;
            /*string json = "[";
            foreach (DataItem item in locations)
            {
                json += "{";
                json += String.Format("\"ID\" : \"{0}\", ", item.ID);
                json += String.Format("\"CreateDate\" : \"{0:yyyy-MM-ddTHH:mm:ss.000Z}\", ", item.CreateDate);
                json += String.Format("\"ModifiedDate\" : \"{0:yyyy-MM-ddTHH:mm:ss.000Z}\", ", item.ModifiedDate);
                json += String.Format("\"Name\" : \"{0}\", ", item.Name);
                json += String.Format("\"Title\" : \"{0}\", ", item.Title);
                //json += String.Format("\"OrderNumber\" : {0}, ", item.OrderNumber);
                //json += String.Format("\"ChangeStatusString\" : \"{0}\", ", item.ChangeStatusString);
                //json += String.Format("\"Active\" : {0}, ", (int)item.Active);
                //json += String.Format("\"DateFrom\" : \"{0:yyyy-MM-ddTHH:mm:ss.000Z}\", ", item.DateFrom);
                //json += String.Format("\"DateTill\" : \"{0:yyyy-MM-ddTHH:mm:ss.000Z}\", ", item.DateTill);
                //json += String.Format("\"IsNew\" : \"{0}\", ", item.IsNew);
                ////string.format werkt niet met {'s in de string
                //json += "\"DataCollection\" : {\"ID\" : \"" + datacollectionid + "\"}, ";
                if (item.ParentGroup != null)
                {
                    json += "\"ParentGroup\" : {\"ID\" : \"" + item.ParentGroup.ID.ToString() + "\"}, ";
                    json += "\"ParentGroup\" : {\"Name\" : \"" + item.ParentGroup.Name + "\"}, ";
                    json += "\"ParentGroup\" : {\"Title\" : \"" + item.ParentGroup.Title + "\"}, ";
                }
                //json += String.Format("\"LastPublishedDate\" : \"{0:yyyy-MM-ddTHH:mm:ss.000Z}\", ", item.LastPublishedDate);
                foreach (DataField field in item.DataCollection.DataItemFields)
                {
                    if (field.MappingColumn == "ImageList1" || field.MappingColumn == "FileList1")
                    {
                        //doe niks
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
                json += "},";
            }
            //laatste komma eraf
            json = json.Remove(json.Length - 1, 1);
            json += "]";
            return json; 
             */
        }





        private int convertToRadius(int zoomLevel)
        {
            if (zoomLevel == 13)
            {
                return 3;
            }
            else if (zoomLevel == 12)
            {
                return 5;
            }
            else if (zoomLevel == 11)
            {
                return 10;
            }
            else if (zoomLevel == 10)
            {
                return 20;
            }
            else if (zoomLevel == 9)
            {
                return 50;
            }
            else if (zoomLevel == 8)
            {
                return 100;
            }
            else if (zoomLevel == 7)
            {
                return 200;
            }
            else
            {
                return 500;
            }

        }

        private string GetWhere(string datacollection_id)
        {
            string where = String.Format(" FK_DataCollection = '{0}' ", datacollection_id);
            return where;
        }



    }

}