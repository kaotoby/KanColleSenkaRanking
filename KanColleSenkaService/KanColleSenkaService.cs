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
        public bool Inprocess { get; set; }
        private Timer timer1;
        private static IList<ServerData> datalist;
        private static KanColleSenkaManager sm = new KanColleSenkaManager();

        private static readonly ILog log = LogManager.GetLogger(typeof(KanColleSenkaService).FullName);

        public KanColleSenkaService() {
            Inprocess = false;
            datalist = sm.GetServerData();
            timer1 = new Timer(TimerTick, null, 3000, 10000);
        }

        private void TimerTick(object stats) {
            if (!Inprocess) {
                var now = DateTime.Now;
                var updates = datalist.Where(data => data.Enabled && data.NextUpdateTime <= now);
                Parallel.ForEach(updates, data =>
                {
#if DEBUG
                    Inprocess = true;
                    sm.ProcessServerData(data);
#else
                    try {
                        Inprocess = true;
                        sm.ProcessServerData(data);
                    } catch (Exception ex) {
                        data.ErrorrCount++;
                        data.NextUpdateTime = DateTime.Now.AddMinutes(5 * data.ErrorrCount);
                        if (!(ex is WebException)) {
                            log.Fatal("MainFunc Fail!", ex);
                        }
                    }
#endif
                });
                Inprocess = false;
            }
        }
    }
}
