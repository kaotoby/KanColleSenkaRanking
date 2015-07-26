using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KanColleSenkaRanking.Models
{
    public class SenkaTimeInfo : IComparable<SenkaTimeInfo>
    {
        public long ID { get { return _id; } }
        public DateTime DateTime { get { return _datetime; } }

        private long _id;
        private DateTime _datetime;

        public SenkaTimeInfo(object id, object time) {
            _id = Convert.ToInt64(id);
            _datetime = Convert.ToDateTime(time);
        }

        public override string ToString() {
            return _datetime.ToString("yyyy年M月d日 H時");
        }

        public int CompareTo(SenkaTimeInfo other) {
            return this.ID.CompareTo(other.ID);
        }
    }
}