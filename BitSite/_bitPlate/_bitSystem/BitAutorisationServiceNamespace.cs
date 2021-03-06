﻿using BitPlate.Domain.Autorisation;
using HJORM;
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17626
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;

namespace BitSite.BitAutorisationService
{


    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName = "ServiceReference1.IAutorisation")]
    public interface IAutorisation
    {

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IAutorisation/Login", ReplyAction = "http://tempuri.org/IAutorisation/LoginResponse")]
        BitPlate.Domain.Autorisation.BitplateUser Login(string Email, string MD5Password, System.Guid SiteId);

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IAutorisation/SendNewPassword", ReplyAction = "http://tempuri.org/IAutorisation/SendNewPasswordResponse")]
        bool SendNewPassword(string Email);

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IAutorisation/ChangePassword", ReplyAction = "http://tempuri.org/IAutorisation/ChangePasswordResponse")]
        bool ChangePassword(BitPlate.Domain.Autorisation.BitplateUser currentUser, string MD5CurrentPassword, string MD5NewPassword);

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IAutorisation/ChangeSite", ReplyAction = "http://tempuri.org/IAutorisation/ChangeSiteResponse")]
        BitPlate.Domain.Autorisation.BitplateUser ChangeSite(BitPlate.Domain.Autorisation.BitplateUser currentUser, System.Guid SiteId);

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IAutorisation/GetUsers", ReplyAction = "http://tempuri.org/IAutorisation/GetUsersResponse")]
        BitPlate.Domain.Autorisation.BitplateUser[] GetUsers(BitPlate.Domain.Autorisation.BitplateUser currentUser, bool onlyCurrentSite, string sort, int pageNumber, int pageSize, string searchString);

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IAutorisation/GetUser", ReplyAction = "http://tempuri.org/IAutorisation/GetUserResponse")]
        BitPlate.Domain.Autorisation.BitplateUser GetUser(System.Guid id);

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IAutorisation/SaveUser", ReplyAction = "http://tempuri.org/IAutorisation/SaveUserResponse")]
        void SaveUser(BitPlate.Domain.Autorisation.BitplateUser user);

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IAutorisation/DeleteUser", ReplyAction = "http://tempuri.org/IAutorisation/DeleteUserResponse")]
        void DeleteUser(System.Guid id);

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IAutorisation/GetUserGroups", ReplyAction = "http://tempuri.org/IAutorisation/GetUserGroupsResponse")]
        BitPlate.Domain.Autorisation.BitplateUserGroup[] GetUserGroups(BitPlate.Domain.Autorisation.BitplateUser currentUser, bool onlyCurrentSite, string sort, int pageNumber, int pageSize, string searchString);

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IAutorisation/GetUserGroup", ReplyAction = "http://tempuri.org/IAutorisation/GetUserGroupResponse")]
        BitPlate.Domain.Autorisation.BitplateUserGroup GetUserGroup(System.Guid id);

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IAutorisation/SaveUserGroup", ReplyAction = "http://tempuri.org/IAutorisation/SaveUserGroupResponse")]
        void SaveUserGroup(BitPlate.Domain.Autorisation.BitplateUserGroup userGroup);

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IAutorisation/DeleteUserGroup", ReplyAction = "http://tempuri.org/IAutorisation/DeleteUserGroupResponse")]
        void DeleteUserGroup(System.Guid id);

        //[System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IAutorisation/GetSites", ReplyAction = "http://tempuri.org/IAutorisation/GetSitesResponse")]
        //BitPlate.Domain.Autorisation.LicenseSite[] GetSites(BitPlate.Domain.Autorisation.BitplateUser currentUser, string sort);

        //[System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IAutorisation/GetSite", ReplyAction = "http://tempuri.org/IAutorisation/GetSiteResponse")]
        //BitPlate.Domain.Autorisation.LicenseSite GetSite(System.Guid id);

        //[System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IAutorisation/SaveSite", ReplyAction = "http://tempuri.org/IAutorisation/SaveSiteResponse")]
        //Guid SaveSite(BitPlate.Domain.Autorisation.LicenseSite site);

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IAutorisation/DeleteSite", ReplyAction = "http://tempuri.org/IAutorisation/DeleteSiteResponse")]
        void DeleteSite(System.Guid id);

        //[System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IAutorisation/GetLicenses", ReplyAction = "http://tempuri.org/IAutorisation/GetLicensesResponse")]
        //BitPlate.Domain.Autorisation.License[] GetLicenses(BitPlate.Domain.Autorisation.BitplateUser currentUser, string companyId, string sort, string searchString);

        //[System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IAutorisation/GetLicense", ReplyAction = "http://tempuri.org/IAutorisation/GetLicenseResponse")]
        //BitPlate.Domain.Autorisation.License GetLicense(System.Guid id);

        //[System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IAutorisation/LoadLicense", ReplyAction = "http://tempuri.org/IAutorisation/LoadLicenseResponse")]
        //BitPlate.Domain.Autorisation.License LoadLicense(string licenseCode, string serverName, string domainName);

        //[System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IAutorisation/SaveLicense", ReplyAction = "http://tempuri.org/IAutorisation/SaveLicenseResponse")]
        //void SaveLicense(BitPlate.Domain.Autorisation.License license);

        //[System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IAutorisation/DeleteLicense", ReplyAction = "http://tempuri.org/IAutorisation/DeleteLicenseResponse")]
        //void DeleteLicense(System.Guid id);

        //[System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IAutorisation/GetDefaultValuesForLicenseType", ReplyAction = "http://tempuri.org/IAutorisation/GetDefaultValuesForLicenseTypeResponse")]
        //string[] GetDefaultValuesForLicenseType(BitPlate.Domain.Autorisation.BitplateUser currentUser, int licenceType);

        //[System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IAutorisation/GetCompanies", ReplyAction = "http://tempuri.org/IAutorisation/GetCompaniesResponse")]
        //BitPlate.Domain.Autorisation.Company[] GetCompanies(BitPlate.Domain.Autorisation.BitplateUser currentUser, string resellerId, string sort);

        //[System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IAutorisation/GetResellers", ReplyAction = "http://tempuri.org/IAutorisation/GetResellersResponse")]
        //BitPlate.Domain.Autorisation.Company[] GetResellers(BitPlate.Domain.Autorisation.BitplateUser currentUser, string sort);

        //[System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IAutorisation/GetCompany", ReplyAction = "http://tempuri.org/IAutorisation/GetCompanyResponse")]
        //BitPlate.Domain.Autorisation.Company GetCompany(System.Guid id);

        //[System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IAutorisation/SaveCompany", ReplyAction = "http://tempuri.org/IAutorisation/SaveCompanyResponse")]
        //void SaveCompany(BitPlate.Domain.Autorisation.Company license);

        //[System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IAutorisation/DeleteCompany", ReplyAction = "http://tempuri.org/IAutorisation/DeleteCompanyResponse")]
        //void DeleteCompany(System.Guid id);

        //[System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IAutorisation/GetFunctionalities", ReplyAction = "http://tempuri.org/IAutorisation/GetFunctionalitiesResponse")]
        //BitPlate.Domain.Autorisation.Functionality[] GetFunctionalities(BitPlate.Domain.Autorisation.BitplateUser currentUser);

        //[System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IAutorisation/GetDefaultFunctionalitiesForUserGroup", ReplyAction = "http://tempuri.org/IAutorisation/GetDefaultFunctionalitiesForUserGroupResponse")]
        //BitPlate.Domain.Autorisation.Functionality[] GetDefaultFunctionalitiesForUserGroup(BitPlate.Domain.Autorisation.BitplateUser currentUser, string name);

        //[System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IAutorisation/GetDefaultFunctionalitiesForLicenseType", ReplyAction = "http://tempuri.org/IAutorisation/GetDefaultFunctionalitiesForLicenseTypeResponse")]
        //BitPlate.Domain.Autorisation.Functionality[] GetDefaultFunctionalitiesForLicenseType(BitPlate.Domain.Autorisation.BitplateUser currentUser, int licenseType);

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IAutorisation/GetUsersByObjectPermission", ReplyAction = "http://tempuri.org/IAutorisation/GetUsersByObjectPermissionResponse")]
        BitPlate.Domain.Autorisation.BitplateUser[] GetUsersByObjectPermission(System.Guid FK_Object);

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IAutorisation/GetUserGroupsByObjectPermission", ReplyAction = "http://tempuri.org/IAutorisation/GetUserGroupsByObjectPermissionResponse")]
        BitPlate.Domain.Autorisation.BitplateUserGroup[] GetUserGroupsByObjectPermission(System.Guid FK_Object);

        /* [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IAutorisation/SaveObjectPermissions", ReplyAction = "http://tempuri.org/IAutorisation/SaveObjectPermissionsResponse")]
        void SaveObjectPermissions(BitPlate.Domain.Autorisation.ObjectPermission[] objectPermissions); */

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IAutorisation/SaveObjectPermissions", ReplyAction = "http://tempuri.org/IAutorisation/SaveObjectPermissionsResponse")]
        void SaveObjectPermissions(BaseCollection<ObjectPermission> objectPermissions);
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IAutorisationChannel : BitSite.BitAutorisationService.IAutorisation, System.ServiceModel.IClientChannel
    {
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class AutorisationClient : System.ServiceModel.ClientBase<BitSite.BitAutorisationService.IAutorisation>, BitSite.BitAutorisationService.IAutorisation
    {

        public AutorisationClient()
        {
        }

        public AutorisationClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        public AutorisationClient(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public AutorisationClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public AutorisationClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        public BitPlate.Domain.Autorisation.BitplateUser Login(string Email, string MD5Password, System.Guid SiteId)
        {
            return base.Channel.Login(Email, MD5Password, SiteId);
        }

        public bool SendNewPassword(string Email)
        {
            return base.Channel.SendNewPassword(Email);
        }

        public bool ChangePassword(BitPlate.Domain.Autorisation.BitplateUser currentUser, string MD5CurrentPassword, string MD5NewPassword)
        {
            return base.Channel.ChangePassword(currentUser, MD5CurrentPassword, MD5NewPassword);
        }

        public BitPlate.Domain.Autorisation.BitplateUser ChangeSite(BitPlate.Domain.Autorisation.BitplateUser currentUser, System.Guid SiteId)
        {
            return base.Channel.ChangeSite(currentUser, SiteId);
        }

        public BitPlate.Domain.Autorisation.BitplateUser[] GetUsers(BitPlate.Domain.Autorisation.BitplateUser currentUser, bool onlyCurrentSite, string sort, int pageNumber, int pageSize, string searchString)
        {
            return base.Channel.GetUsers(currentUser, onlyCurrentSite, sort, pageNumber, pageSize, searchString);
        }

        public BitPlate.Domain.Autorisation.BitplateUser GetUser(System.Guid id)
        {
            return base.Channel.GetUser(id);
        }

        public void SaveUser(BitPlate.Domain.Autorisation.BitplateUser user)
        {
            base.Channel.SaveUser(user);
        }

        public void DeleteUser(System.Guid id)
        {
            base.Channel.DeleteUser(id);
        }

        public BitPlate.Domain.Autorisation.BitplateUserGroup[] GetUserGroups(BitPlate.Domain.Autorisation.BitplateUser currentUser, bool onlyCurrentSite, string sort, int pageNumber, int pageSize, string searchString)
        {
            return base.Channel.GetUserGroups(currentUser, onlyCurrentSite, sort, pageNumber, pageSize, searchString);
        }

        public BitPlate.Domain.Autorisation.BitplateUserGroup GetUserGroup(System.Guid id)
        {
            return base.Channel.GetUserGroup(id);
        }

        public void SaveUserGroup(BitPlate.Domain.Autorisation.BitplateUserGroup userGroup)
        {
            base.Channel.SaveUserGroup(userGroup);
        }

        public void DeleteUserGroup(System.Guid id)
        {
            base.Channel.DeleteUserGroup(id);
        }

        //public BitPlate.Domain.Autorisation.LicenseSite[] GetSites(BitPlate.Domain.Autorisation.BitplateUser currentUser, string sort)
        //{
        //    return base.Channel.GetSites(currentUser, sort);
        //}

        //public BitPlate.Domain.Autorisation.LicenseSite GetSite(System.Guid id)
        //{
        //    return base.Channel.GetSite(id);
        //}

        //public Guid SaveSite(BitPlate.Domain.Autorisation.LicenseSite site)
        //{
        //    return base.Channel.SaveSite(site);
        //}

        public void DeleteSite(System.Guid id)
        {
            base.Channel.DeleteSite(id);
        }

        //public BitPlate.Domain.Autorisation.License[] GetLicenses(BitPlate.Domain.Autorisation.BitplateUser currentUser, string companyId, string sort, string searchString)
        //{
        //    return base.Channel.GetLicenses(currentUser, companyId, sort, searchString);
        //}

        //public BitPlate.Domain.Autorisation.License GetLicense(System.Guid id)
        //{
        //    return base.Channel.GetLicense(id);
        //}

        //public BitPlate.Domain.Autorisation.License LoadLicense(string licenseCode, string serverName, string domainName)
        //{
        //    return base.Channel.LoadLicense(licenseCode, serverName, domainName);
        //}

        //public void SaveLicense(BitPlate.Domain.Autorisation.License license)
        //{
        //    base.Channel.SaveLicense(license);
        //}

        //public void DeleteLicense(System.Guid id)
        //{
        //    base.Channel.DeleteLicense(id);
        //}

        //public string[] GetDefaultValuesForLicenseType(BitPlate.Domain.Autorisation.BitplateUser currentUser, int licenceType)
        //{
        //    return base.Channel.GetDefaultValuesForLicenseType(currentUser, licenceType);
        //}

        //public BitPlate.Domain.Autorisation.Company[] GetCompanies(BitPlate.Domain.Autorisation.BitplateUser currentUser, string resellerId, string sort)
        //{
        //    return base.Channel.GetCompanies(currentUser, resellerId, sort);
        //}

        //public BitPlate.Domain.Autorisation.Company[] GetResellers(BitPlate.Domain.Autorisation.BitplateUser currentUser, string sort)
        //{
        //    return base.Channel.GetResellers(currentUser, sort);
        //}

        //public BitPlate.Domain.Autorisation.Company GetCompany(System.Guid id)
        //{
        //    return base.Channel.GetCompany(id);
        //}

        //public void SaveCompany(BitPlate.Domain.Autorisation.Company license)
        //{
        //    base.Channel.SaveCompany(license);
        //}

        //public void DeleteCompany(System.Guid id)
        //{
        //    base.Channel.DeleteCompany(id);
        //}

        //public BitPlate.Domain.Autorisation.Functionality[] GetFunctionalities(BitPlate.Domain.Autorisation.BitplateUser currentUser)
        //{
        //    return base.Channel.GetFunctionalities(currentUser);
        //}

        //public BitPlate.Domain.Autorisation.Functionality[] GetDefaultFunctionalitiesForUserGroup(BitPlate.Domain.Autorisation.BitplateUser currentUser, string name)
        //{
        //    return base.Channel.GetDefaultFunctionalitiesForUserGroup(currentUser, name);
        //}

        //public BitPlate.Domain.Autorisation.Functionality[] GetDefaultFunctionalitiesForLicenseType(BitPlate.Domain.Autorisation.BitplateUser currentUser, int licenseType)
        //{
        //    return base.Channel.GetDefaultFunctionalitiesForLicenseType(currentUser, licenseType);
        //}

        public BitPlate.Domain.Autorisation.BitplateUser[] GetUsersByObjectPermission(System.Guid FK_Object)
        {
            return base.Channel.GetUsersByObjectPermission(FK_Object);
        }

        public BitPlate.Domain.Autorisation.BitplateUserGroup[] GetUserGroupsByObjectPermission(System.Guid FK_Object)
        {
            return base.Channel.GetUserGroupsByObjectPermission(FK_Object);
        }

        public void SaveObjectPermissions(HJORM.BaseCollection<BitPlate.Domain.Autorisation.ObjectPermission> objectPermissions)
        {
            base.Channel.SaveObjectPermissions(objectPermissions);
        }
    }
}
