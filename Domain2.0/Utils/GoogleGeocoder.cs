using System;
using System.Web;
using System.Configuration;

using System.Net;

using System.IO;
using System.Globalization;

using HJORM;
using BitPlate.Domain;
using BitPlate.Domain.DataCollections;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace BitPlate.Domain.Utils
{
    public class GPoint
    {
        public double Lat { get; set; }
        public double Long { get; set; }

        public GPoint()
        {
        }

        public GPoint(double latitude, double longitude)
        {
            Lat = latitude;
            Long = longitude;
        }
    }


    [XmlRoot(ElementName = "GeocodeResponse", Namespace = "")]
    public class GeocodeResponse
    {
        [XmlElement(ElementName = "status")]
        public GeocodeResponseStatusCode Status;
        [XmlElement(ElementName = "result")]
        public List<GeocodeResponseResult> Results;
    }
    [XmlType(Namespace = "")]
    public class GeocodeResponseResult
    {
        [XmlElement(ElementName = "type")]
        public List<string> Types;
        [XmlElement(ElementName = "formatted_address")]
        public string FormattedAddress;
        [XmlElement(ElementName = "address_component")]
        public List<GeocodeResponseAddressComponent> AddressComponents;
        [XmlElement(ElementName = "geometry")]
        public GeocodeResponseResultGeometry Geometry;
    }
    [XmlType(Namespace = "")]
    public class GeocodeResponseAddressComponent
    {
        [XmlElement(ElementName = "long_name")]
        public string LongName;
        [XmlElement(ElementName = "short_name")]
        public string ShortName;
        [XmlElement(ElementName = "type")]
        public List<string> Types;
    }
    [XmlType(Namespace = "")]
    public class GeocodeResponseResultGeometry
    {
        [XmlElement(ElementName = "location")]
        public Location Location;
        [XmlElement(ElementName = "location_type")]
        public GeocodeResponseResultGeometryLocationType LocationType;
        [XmlElement(ElementName = "viewport")]
        public GeocodeResponseResultGeometryViewport Viewport;
    }
    [XmlType(Namespace = "")]
    public class GeocodeResponseResultGeometryViewport
    {
        [XmlElement(ElementName = "southwest")]
        public Location Southwest;
        [XmlElement(ElementName = "northeast")]
        public Location Northeast;
    }
    public enum GeocodeResponseStatusCode
    {
        OK,
        ZERO_RESULTS,
        OVER_QUERY_LIMIT,
        REQUEST_DENIED,
        INVALID_REQUEST,
    }
    public enum GeocodeResponseResultGeometryLocationType
    {
        ROOFTOP,
        RANGE_INTERPOLATED,
        GEOMETRIC_CENTER,
        APPROXIMATE,
    }
    [XmlType(Namespace = "")]
    public class Location
    {
        [XmlElement(ElementName = "lat")]
        public string Lat;
        [XmlElement(ElementName = "lng")]
        public string Lng;
    }

    public class GoogleGeocoder
    {


        public static GPoint GetLatLng(string address, string postalCode, string city, string country, string googlMapsKey)
        {
            if (postalCode == null) postalCode = "";
            if (city == null) city = "";
            if (country == null) country = "";
            if (country == "")
            {
                return new GPoint();
            }
            if (city == "" && postalCode == "")
            {
                return new GPoint();
            }
            string addressQuery = address + ", " + postalCode + ", " + city + ", " + country;
            return GetLatLng(addressQuery, googlMapsKey);
        }

        public static GPoint GetLatLng(string completeAddress, string googlMapsKey)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(GeocodeResponse));
                WebClient c = new WebClient();
                byte[] response = c.DownloadData("http://maps.googleapis.com/maps/api/geocode/xml?address=" + completeAddress + "&sensor=true");
                MemoryStream ms = new MemoryStream(response);
                GeocodeResponse geocodeResponse = (GeocodeResponse)xs.Deserialize(ms);
                NumberFormatInfo provider = new NumberFormatInfo();
                provider.NumberDecimalSeparator = ".";
                double latidude = Convert.ToDouble(geocodeResponse.Results[0].Geometry.Location.Lat, provider);
                double longitude = Convert.ToDouble(geocodeResponse.Results[0].Geometry.Location.Lng, provider);
                return new GPoint(latidude, longitude);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Adres ({0}) niet gevonden in Googlemaps. ", completeAddress), ex);
            }
        }

       
        public static bool IsWithinDistance(DataItem item, double searchFromLat, double searchFromLong, int searchRadius, out double distance)
        {
            bool returnValue = false;

            double R = 6371; // km
            double dLat = toRad(searchFromLat - item.Latitude);
            double dLon = toRad(searchFromLong - item.Longitude);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(toRad(item.Latitude)) * Math.Cos(toRad(searchFromLat)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double d = R * c;
            //double d2 = DistanceBetweenPlaces( this.Longitude,this.Latitude, searchFromLong, searchFromLat);
            //double distanceX = this.Latitude - searchFromLat;
            //double distanceY = this.Longitude - searchFromLong;

            //double distance = Math.Sqrt((distanceX * distanceX) + (distanceY * distanceY));
            returnValue = (d < searchRadius);
            distance = Math.Round(d, 1);
            return returnValue;
        }

        private static double toRad(double number)
        {
            return number * Math.PI / 180;
        }

        

    }
}
