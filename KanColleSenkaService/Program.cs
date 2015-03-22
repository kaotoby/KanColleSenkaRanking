using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KanColleSenkaService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main() {
            ServicePointManager.DefaultConnectionLimit = 1000;

            DirectoryInfo appBase = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            while (appBase.Name != "KanColleSenkaRanking") appBase = appBase.Parent;
            string path = appBase.FullName;
#if DEBUG
            path = Path.Combine(path, "KanColleSenkaRanking");
#endif

            AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(path, "App_Data"));
            XmlConfigurator.ConfigureAndWatch(new FileInfo(Path.Combine(path, "Log4Net.config")));

#if DEBUG
            MainService _service = new MainService();
            _service.OnDebug();
            Thread.Sleep(Timeout.Infinite);
#else
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new MainService() 
            };
            ServiceBase.Run(ServicesToRun);
#endif
        }
    }
}
