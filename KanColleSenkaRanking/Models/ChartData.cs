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
    }
}