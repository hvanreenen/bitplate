using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using BitSite.BitAutorisationService;
using System.ServiceModel;
using BitSite.LicServiceReference;
using BitSite.UserServiceReference;

namespace BitSite
{
    public static class BitMetaServerServicesHelper
    {
        private static UserServiceClient userServiceClient;
        private static LicServiceClient licServiceClient;

        public static AutorisationClient GetClient()
        {
            string serviceRootUrl = ConfigurationManager.AppSettings["LicenseHost"];
            string serviceUrl = serviceRootUrl + "/Autorisation.svc";
            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding(System.ServiceModel.BasicHttpSecurityMode.None);
            //binding.MaxBufferPoolSize = 2073741824; //2Gig //Int32.MaxValue;
            //binding.MaxBufferSize = 2073741824; //2Gig //Int32.MaxValue;
            //binding.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
            binding.MaxReceivedMessageSize = 10485760; //1Gig 

            //TEST EMIEL
            //binding.UseDefaultWebProxy = false;
            //binding.TransferMode = TransferMode.Buffered;
            
            System.ServiceModel.EndpointAddress endPointAddress = new System.ServiceModel.EndpointAddress(serviceUrl);

            BitSite.BitAutorisationService.AutorisationClient client = new BitSite.BitAutorisationService.AutorisationClient(binding, endPointAddress);
           
            return client;
        }


        public static UserServiceClient GetUserServiceClient()
        {
            bool foundWorkingUserService = false;
            //Al eerder verbonden geweest met de meta server? Probeer dan vorige vorige meta verbinding te gebruiken.
            if (userServiceClient != null)
            {
                try
                {
                    userServiceClient.HandShake();
                    foundWorkingUserService = true;
                }
                catch
                {
                }
            }
            if (!foundWorkingUserService)
            {
                foundWorkingUserService = FindUserServiceServer();
            }

            if (foundWorkingUserService)
            {
                return userServiceClient;
            }
            else
            {
                return null;
            }
        }

        private static bool FindUserServiceServer()
        {
            string[] serviceRootUrls = ConfigurationManager.AppSettings["LicenseHost"].Split(';');
            foreach (string serviceRootUrl in serviceRootUrls)
            {
                string serviceUrl = serviceRootUrl + "/Services/UserService.svc";
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding(System.ServiceModel.BasicHttpSecurityMode.None);

                binding.MaxReceivedMessageSize = 10485760; //1Gig 

                System.ServiceModel.EndpointAddress endPointAddress = new System.ServiceModel.EndpointAddress(serviceUrl);

                BitSite.UserServiceReference.UserServiceClient client = new BitSite.UserServiceReference.UserServiceClient(binding, endPointAddress);
                try
                {
                    client.HandShake();
                    userServiceClient = client;
                    return true;
                }
                catch
                {

                }
            }
            return false;
        }

        public static LicServiceClient GetLicenseServiceClient()
        {
            bool foundWorkingLicService = false;
            //Al eerder verbonden geweest met de meta server? Probeer dan vorige vorige meta verbinding te gebruiken.
            if (licServiceClient != null)
            {
                try
                {
                    licServiceClient.HandShake();
                    foundWorkingLicService = true;
                }
                catch
                {
                }
            }
            if (!foundWorkingLicService)
            {
                foundWorkingLicService = FindLicServiceServer();
            }

            if (foundWorkingLicService)
            {
                return licServiceClient;
            }
            else
            {
                return null;
            }
        }

        private static bool FindLicServiceServer()
        {
            string[] serviceRootUrls = ConfigurationManager.AppSettings["LicenseHost"].Split(';');
            foreach (string serviceRootUrl in serviceRootUrls)
            {
                string serviceUrl = serviceRootUrl + "/Services/LicService.svc";
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding(System.ServiceModel.BasicHttpSecurityMode.None);

                binding.MaxReceivedMessageSize = 10485760; //1Gig 

                System.ServiceModel.EndpointAddress endPointAddress = new System.ServiceModel.EndpointAddress(serviceUrl);

                BitSite.LicServiceReference.LicServiceClient client = new BitSite.LicServiceReference.LicServiceClient(binding, endPointAddress);
                try
                {
                    client.HandShake();
                    licServiceClient = client;
                    return true;
                }
                catch
                {

                }
            }
            return false;
        }

        //public static LicServiceClient GetLicenseServiceClient()
        //{
        //    string serviceRootUrl = ConfigurationManager.AppSettings["LicenseHost"];
        //    string serviceUrl = serviceRootUrl + "/Services/LicService.svc";
        //    System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding(System.ServiceModel.BasicHttpSecurityMode.None);

        //    binding.MaxReceivedMessageSize = 20485760; //2Gig 

        //    System.ServiceModel.EndpointAddress endPointAddress = new System.ServiceModel.EndpointAddress(serviceUrl);

        //    BitSite.LicServiceReference.LicServiceClient client = new BitSite.LicServiceReference.LicServiceClient(binding, endPointAddress);

        //    return client;
        //}

        //public static T GetServiceClient<T>(string name)
        //{
        //    string serviceRootUrl = ConfigurationManager.AppSettings["LicenseHost"];
        //    string serviceUrl = serviceRootUrl + "/Services/" + name;
        //    System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding(System.ServiceModel.BasicHttpSecurityMode.None);

        //    binding.MaxReceivedMessageSize = 10485760; //1Gig 

        //    System.ServiceModel.EndpointAddress endPointAddress = new System.ServiceModel.EndpointAddress(serviceUrl);

        //    T client = new T(binding, endPointAddress);

        //    return client;
        //}
    }
}