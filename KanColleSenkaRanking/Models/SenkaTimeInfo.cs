using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KanColleSenkaRanking.Models
{
    public class SenkaTimeInfo
    {
        public long ID { get { return _id; } }
        public DateTime Time { get { return _time; } }

        private long _id;
        private DateTime _time;

        public SenkaTimeInfo(object id, object time) {
            _id = Convert.ToInt64(id);
            _time = Convert.ToDateTime(time);
        }
    }
}