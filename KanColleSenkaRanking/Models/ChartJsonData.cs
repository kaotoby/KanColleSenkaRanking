using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KanColleSenkaRanking.Models
{
    public class ChartJsonData
    {
        public double[] value { get; set; }
        public string name { get; set; }

        public ChartJsonData() {
        }

        public ChartJsonData(string _name) {
            name = _name;
        }

        public void SetValue(List<int> values) {
            value = ChartData.ValueConveter(values);
        }
    }
}