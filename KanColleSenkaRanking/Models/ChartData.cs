using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KanColleSenkaRanking.Models
{
    public class ChartData
    {
        public ChartJsonData[] Data { get { return _data; } }
        public string[] Lables { get { return _lables; } }
        public const int NONE = int.MaxValue;

        private ChartJsonData[] _data;
        private string[] _lables;

        public void SetData(ChartJsonData[] data, string[] lables) {
            _data = data;
            _lables = lables;
        }

        public string ToJsonString() {
            var serializer = new JsonSerializer();
            var stringWriter = new StringWriter();
            using (var writer = new JsonTextWriter(stringWriter)) {
                writer.QuoteName = false;
                serializer.Serialize(writer, this.Data);
            }
            return stringWriter.ToString();
        }

        public static double[] ValueConveter(List<int> values) {
            if (values.Count == 0) {
                return new double[0];
            } else if (values.Count == 1 && values[0] == ChartData.NONE) {
                return new double[1] { 0 };
            } else {
                double min = values.Min();
                return values.Select(v => v == ChartData.NONE ? min + 0.1 : v).ToArray();
            }
        }
    }
}