﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BitSite.UserServiceReference {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="UserServiceReference.IUserService")]
    public interface IUserService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUserService/SetUserData", ReplyAction="http://tempuri.org/IUserService/SetUserDataResponse")]
        void SetUserData(BitPlate.Domain.Autorisation.BitplateUser user, string domainName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUserService/GetUserData", ReplyAction="http://tempuri.org/IUserService/GetUserDataResponse")]
        BitPlate.Domain.Autorisation.MultiSiteUser GetUserData(BitPlate.Domain.Autorisation.BitplateUser user);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUserService/GetSiteUrls", ReplyAction="http://tempuri.org/IUserService/GetSiteUrlsResponse")]
        BitPlate.Domain.Licenses.LicensedEnvironment[] GetSiteUrls(BitPlate.Domain.Autorisation.BitplateUser user);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUserService/GenerateTempLoginKey", ReplyAction="http://tempuri.org/IUserService/GenerateTempLoginKeyResponse")]
        string GenerateTempLoginKey(string url, BitPlate.Domain.Autorisation.BitplateUser user);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUserService/CheckTempLoginKey", ReplyAction="http://tempuri.org/IUserService/CheckTempLoginKeyResponse")]
        BitPlate.Domain.Autorisation.MultiSiteUser CheckTempLoginKey(string key);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUserService/Login", ReplyAction="http://tempuri.org/IUserService/LoginResponse")]
        BitPlate.Domain.Autorisation.MultiSiteUser Login(string siteDomainName, string email, string passwordMd5);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUserService/HandShake", ReplyAction="http://tempuri.org/IUserService/HandShakeResponse")]
        string HandShake();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUserService/GetNewsItems", ReplyAction="http://tempuri.org/IUserService/GetNewsItemsResponse")]
        BitPlate.Domain.News.NewsItem[] GetNewsItems();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IUserServiceChannel : BitSite.UserServiceReference.IUserService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class UserServiceClient : System.ServiceModel.ClientBase<BitSite.UserServiceReference.IUserService>, BitSite.UserServiceReference.IUserService {
        
        public UserServiceClient() {
        }
        
        public UserServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public UserServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public UserServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public UserServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public void SetUserData(BitPlate.Domain.Autorisation.BitplateUser user, string domainName) {
            base.Channel.SetUserData(user, domainName);
        }
        
        public BitPlate.Domain.Autorisation.MultiSiteUser GetUserData(BitPlate.Domain.Autorisation.BitplateUser user) {
            return base.Channel.GetUserData(user);
        }
        
        public BitPlate.Domain.Licenses.LicensedEnvironment[] GetSiteUrls(BitPlate.Domain.Autorisation.BitplateUser user) {
            return base.Channel.GetSiteUrls(user);
        }
        
        public string GenerateTempLoginKey(string url, BitPlate.Domain.Autorisation.BitplateUser user) {
            return base.Channel.GenerateTempLoginKey(url, user);
        }
        
        public BitPlate.Domain.Autorisation.MultiSiteUser CheckTempLoginKey(string key) {
            return base.Channel.CheckTempLoginKey(key);
        }
        
        public BitPlate.Domain.Autorisation.MultiSiteUser Login(string siteDomainName, string email, string passwordMd5) {
            return base.Channel.Login(siteDomainName, email, passwordMd5);
        }
        
        public string HandShake() {
            return base.Channel.HandShake();
        }
        
        public BitPlate.Domain.News.NewsItem[] GetNewsItems() {
            return base.Channel.GetNewsItems();
        }
    }
}
