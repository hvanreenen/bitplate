using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using HJORM;
using HJORM.Attributes;
using BitPlate.Domain.Autorisation;

namespace BitPlate.Domain.Licenses
{
    //[DataContract]
    [Flags]

    public enum FunctionalityEnum
    {
        [EnumMember]
        SiteContentManagement = 1000,
        [EnumMember]
        FileManager = 1100,
        [EnumMember]
        FileManagerUpload = 1110,
        [EnumMember]
        FileManagerDownload = 1120,
        [EnumMember]
        FileManagerAddFolder = 1140,
        [EnumMember]
        FileManagerRename = 1150,
        [EnumMember]
        FileManagerRemove = 1160,
        [EnumMember]
        FileManagerCreateThumbnail = 1170,
        [EnumMember]
        FileManagerCCP = 1180,

        [EnumMember]
        Pages = 1200,
        [EnumMember]
        PagesCreate = 1210,
        [EnumMember]
        PagesDelete = 1220,
        [EnumMember]
        PagesEdit = 1230,
        [EnumMember]
        PagesEditModuleAdd = 1231,
        [EnumMember]
        PagesEditModuleRemove = 1232,
        [EnumMember]
        PagesEditModuleConfig = 1233,
        [EnumMember]
        PagesEditModuleEdit = 1234,
        [EnumMember]
        PagesEditModuleMove = 1235,
        [EnumMember]
        PagesConfig = 1240,
        [EnumMember]
        PagesCreateFolder = 1250,

        [EnumMember]
        Publish = 1300,

        [EnumMember]
        DataCollections = 1400,
        [EnumMember]
        DataCollectionsCreate = 1410,
        [EnumMember]
        DataCollectionsEditData = 1420,
        [EnumMember]
        DataCollectionsConfig = 1430,
        [EnumMember]
        DataCollectionsRemove = 1440,
        [EnumMember]
        DataCollectionDataCreate = 1421,
        [EnumMember]
        DataCollectionDataEdit = 1422,
        [EnumMember]
        DataCollectionDataRemove = 1423,

        [EnumMember]
        MenuManager = 1500,
        [EnumMember]
        Modules = 1600,
        [EnumMember]
        ModulesGeneral = 1610,
        [EnumMember]
        ModuleHTML = 1611,
        [EnumMember]
        ModuleSearch = 1612,
        [EnumMember]
        ModuleSearchResults = 1613,
        [EnumMember]
        ModuleInputForm = 1614,
        //[EnumMember]
        //ModulePopup = 1615,
        //[EnumMember]
        //ModuleMedia = 1616,
        //[EnumMember]
        //ModuleLink = 1617,
        //[EnumMember]
        //ModuleBreadCrump = 1618,
        //[EnumMember]
        //ModulesGeneralExtra = 1620,
        [EnumMember]
        ModulesDataCollections = 1630,
        //[EnumMember]
        //ModuleDataMainGroups = 1631,
        [EnumMember]
        ModuleDataGroups = 1632,
        [EnumMember]
        ModuleDataItems = 1633,
        [EnumMember]
        ModuleDataGroupDetails = 1634,
        [EnumMember]
        ModuleDataItemDetails = 1635,
        [EnumMember]
        ModuleDataTree = 1636,
        [EnumMember]
        ModuleDataBreadCrumb = 1637,
        [EnumMember]
        ModuleDataGoogleMaps = 1638,
        [EnumMember]
        ModuleDataFilter = 1639,
        [EnumMember]
        ModulesWebshop = 1660,
        [EnumMember]
        ModuleWebshopProducts = 1661,
        [EnumMember]
        ModuleWebshopProductDetails = 1662,
        [EnumMember]
        ModuleWebshopCard = 1663,
        [EnumMember]
        ModuleWebshopOrderForm = 1664,
        [EnumMember]
        ModuleWebshopCheckout = 1665,
        [EnumMember]
        ModuleWebshopInvoice = 1666,
        [EnumMember]
        ModuleWebshopPayment = 1667,

        [EnumMember]
        ModulesAuth = 1680,
        [EnumMember]
        ModuleAuthLogin = 1681,
        [EnumMember]
        ModuleAuthLoginData = 1682,
        [EnumMember]
        ModuleAuthLoginStatus = 1683,



        [EnumMember]
        SiteManagement = 2000,
        [EnumMember]
        Templates = 2100,
        [EnumMember]
        TemplatesCreate = 2110,
        [EnumMember]
        TemplatesDelete = 2120,
        [EnumMember]
        TemplatesEdit = 2130,
        [EnumMember]
        TemplatesConfig = 2140,
        [EnumMember]
        Scripts = 2200,
        [EnumMember]
        ScriptsCreate = 2210,
        [EnumMember]
        ScriptsDelete = 2220,
        [EnumMember]
        ScriptsEdit = 2230,
        [EnumMember]
        ScriptsConfig = 2240,
        [EnumMember]
        SiteConfig = 2300,
        [EnumMember]
        Backups = 2400,
        [EnumMember]
        EventLog = 2500,
        [EnumMember]
        Stylesheets = 2600,
        [EnumMember]
        StylesheetsCreate = 2610,
        [EnumMember]
        StylesheetsDelete = 2620,
        [EnumMember]
        StylesheetsEdit = 2630,
        [EnumMember]
        StylesheetsConfig = 2640,
        [EnumMember]
        MultipleLanguages = 2700,

        [EnumMember]
        NewsLetters = 3000,
        [EnumMember]
        NewsLettersOverview = 3100,
        [EnumMember]
        NewsLetterSubscriptions = 3200,
        [EnumMember]
        NewsLetterSettings = 3300,
        [EnumMember]
        NewsLetterStats = 3400,
        [EnumMember]
        NewsletterModules = 3500,
        [EnumMember]
        NewsletterModulesSubscribe = 3510,
        [EnumMember]
        NewsletterModulesUnsubscribe = 3520,
        [EnumMember]
        NewsletterModulesOptin = 3530,

        [EnumMember]
        Webshop = 4000,
        [EnumMember]
        WebshopProducts = 4100,
        [EnumMember]
        WebshopOrders = 4200,
        [EnumMember]
        WebshopUsers = 4300,
        [EnumMember]
        WebshopSettings = 4400,

        [EnumMember]
        ServerManagement = 5000,
        [EnumMember]
        ServerSites = 5100,
        [EnumMember]
        ServerUsers = 5200,
        [EnumMember]
        ServerBackups = 5300,
        [EnumMember]
        ServerEventLog = 5400,

        [EnumMember]
        UserManagement = 6000,
        [EnumMember]
        Users = 6100,
        [EnumMember]
        UsersCreate = 6110,
        [EnumMember]
        UsersConfig = 6120,
        [EnumMember]
        UsersRemove = 6130,
        [EnumMember]
        UserGroups = 6200,
        [EnumMember]
        UserGroupsCreate = 6210,
        [EnumMember]
        UserGroupsConfig = 6220,
        [EnumMember]
        UserGroupsRemove = 6230,
        [EnumMember]
        UserRights = 6300,

        [EnumMember]
        SiteUserManagement = 7000,
        [EnumMember]
        SiteUsers = 7100,
        [EnumMember]
        SiteUsersCreate = 7110,
        [EnumMember]
        SiteUsersConfig = 7120,
        [EnumMember]
        SiteUsersRemove = 7130,
        [EnumMember]
        SiteUserGroups = 7200,
        [EnumMember]
        SiteUserGroupsCreate = 7210,
        [EnumMember]
        SiteUserGroupsConfig = 7220,
        [EnumMember]
        SiteUserGroupsRemove = 7230,
        [EnumMember]
        SiteUserRights = 7300,
        
        [EnumMember]
        LicenseManagement = 9000,
        //[EnumMember]
        //LicenseResellers = 9100,
        //[EnumMember]
        //LicenseCompanies = 9200,
        //[EnumMember]
        //Licenses = 9300,
        //[EnumMember]
        //LicenseSites = 9400,

    }
}