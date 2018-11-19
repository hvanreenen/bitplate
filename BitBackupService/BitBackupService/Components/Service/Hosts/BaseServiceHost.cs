using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace BitplateBackupService.Components.Service.Hosts
{
    public class BaseServiceHost : System.ServiceProcess.ServiceBase
    {
        public ServiceHost serviceHost = null;

        #region "Initialize"
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public BaseServiceHost()
        {
            // This call is required by the Windows.Forms Component Designer.
            InitializeComponent();
            // TODO: Add any initialization after the InitComponent call
        }

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            // 
            // Service1
            // 
            this.ServiceName = "BitBackupService";
        }
        #endregion

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Set things in motion so your service can do its work.
        /// </summary>
        protected override void OnStart(string[] args)
        {
            // TODO: Add code here to start your service.
            if (serviceHost != null)
            {
                serviceHost.Close();
            }

            // Create a ServiceHost for the CalculatorService type and 
            // provide the base address.
            serviceHost = new ServiceHost(typeof(BackupService));

            // Open the ServiceHostBase to create listeners and start 
            // listening for messages.
            serviceHost.Open();
        }

        /// <summary>
        /// Stop this service.
        /// </summary>
        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
            if (serviceHost != null)
            {
                serviceHost.Close();
                serviceHost = null;
            }
        }
        protected override void OnContinue()
        {

        }
    }
}
