using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace BitplateBackupService.Components.Service.Hosts
{
    public class ConsoleServiceHost: BaseServiceHost
    {
        public static void StartHost(string[] args)
        {
            ConsoleServiceHost bitBackupService = new ConsoleServiceHost();
            try
            {
                Console.WriteLine("De " + bitBackupService.ServiceName + "-service wordt gestart.");
                bitBackupService.OnStart(args);
                Console.WriteLine("De " + bitBackupService.ServiceName + "-service is gestart in command line.");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
                Console.ReadLine();
            }
        }
    }
}
