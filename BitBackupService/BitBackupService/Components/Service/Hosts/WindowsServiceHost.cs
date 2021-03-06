﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace BitplateBackupService.Components.Service.Hosts
{
    public class WindowsServiceHost : BaseServiceHost 
    {
        public static void StartHost()
        {
            ////MyWinServiceHost.Launch();
            System.ServiceProcess.ServiceBase[] ServicesToRun;

            //// More than one user Service may run within the same process. To add
            //// another service to this process, change the following line to
            //// create a second service object. For example,
            ////
            ////   ServicesToRun = New System.ServiceProcess.ServiceBase[] {new Service1(), new MySecondUserService()};
            ////
            ServicesToRun = new System.ServiceProcess.ServiceBase[] { new WindowsServiceHost() };

            System.ServiceProcess.ServiceBase.Run(ServicesToRun);
        }
    }
}
