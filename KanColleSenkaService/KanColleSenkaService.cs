using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KanColleSenkaService;
using KanColleSenkaService.Models;

namespace KanColleSenkaService
{
    class KanColleSenkaService
    {
        public bool Inprocess { get; set; }

        private Timer timer1;
        private KanColleSenkaManager sm;

        private static readonly ILog log = LogManager.GetLogger(typeof(KanColleSenkaService).FullName);

        public KanColleSenkaService() {
            Inprocess = false;
            sm = new KanColleSenkaManager();
            timer1 = new Timer(TimerTick, null, 3000, 10000);
        }

        private void TimerTick(object stats) {
            if (!Inprocess) {
                var now = DateTime.Now;
                var updates = sm.Servers.Where(data => data.Enabled && data.NextUpdateTime <= now);
                sm.UpdateDateInfo();
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
