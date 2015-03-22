using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace KanColleSenkaService
{
    public partial class MainService : ServiceBase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MainService).FullName);
        private static KanColleSenkaService Kan { get; set; }

        public MainService() {
            InitializeComponent();
        }

        public void OnDebug() {
            OnStart(null);
        }

        protected override void OnStart(string[] args) {
            log.Info("[Started] Kan Colle Senka Service");
            Kan = new KanColleSenkaService();
        }

        protected override void OnStop() {
            Kan.MainThread.Abort();
            log.Warn("[Stopped] Kan Colle Senka Service");
        }
    }
}
