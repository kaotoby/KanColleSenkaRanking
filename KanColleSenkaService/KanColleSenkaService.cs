using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KanColleSenkaService;
using KanColleSenkaService.Module;

namespace KanColleSenkaService
{
    class KanColleSenkaService
    {
        public Thread MainThread { get; set; }
        public bool Inprocess { get; set; }
        private Timer timer1;
        private static IList<ServerData> datalist;
        private static KanColleSenkaManager sm = new KanColleSenkaManager();

        private static readonly ILog log = LogManager.GetLogger(typeof(KanColleSenkaService).FullName);

        public KanColleSenkaService() {
            Inprocess = false;
            datalist = sm.GetServerData();
            timer1 = new Timer(TimerTick, null, 3000, 1000);
        }

        private void TimerTick(object stats) {
            if (!Inprocess) {
                var now = DateTime.Now;
                for (int i = 0; i < datalist.Count; i++) {
                    var data = datalist[i];
                    if (data.Enabled && data.NextUpdateTime <= now) {
                        MainThread = new Thread(MainFunc);
                        MainThread.Start(data);
                        break;
                    }
                }
            }
        }

        private void MainFunc(object para) {
            ServerData data = para as ServerData;
#if DEBUG
            Inprocess = true;
            if (data.Enabled) sm.ProcessServerData(data);
#else
            try {
                Inprocess = true;
                if (data.Enabled) sm.ProcessServerData(data);
            } catch (Exception ex) {
                data.NextUpdateTime = DateTime.Now.AddMinutes(20);
                data.ErrorrCount++;
                if (!(ex is WebException)) {
                    data.ErrorrCount++;
                } else {
                    log.Fatal("MainFunc Fail!", ex);
                }
            } finally {
                Inprocess = false;
            }
#endif
        }
    }
}
