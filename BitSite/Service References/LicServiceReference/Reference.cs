﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18052
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BitSite.LicServiceReference {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="LicServiceReference.ILicService")]
    public interface ILicService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILicService/GetLicense", ReplyAction="http://tempuri.org/ILicService/GetLicenseResponse")]
        BitPlate.Domain.Licenses.LicenseFile GetLicense(string licenseCode, string serverName, string path, string domainName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILicService/HandShake", ReplyAction="http://tempuri.org/ILicService/HandShakeResponse")]
        string HandShake();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ILicServiceChannel : BitSite.LicServiceReference.ILicService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class LicServiceClient : System.ServiceModel.ClientBase<BitSite.LicServiceReference.ILicService>, BitSite.LicServiceReference.ILicService {
        
        public LicServiceClient() {
        }
        
        public LicServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public LicServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public LicServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public LicServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public BitPlate.Domain.Licenses.LicenseFile GetLicense(string licenseCode, string serverName, string path, string domainName) {
            return base.Channel.GetLicense(licenseCode, serverName, path, domainName);
        }
        
        public string HandShake() {
            return base.Channel.HandShake();
        }
    }
}
