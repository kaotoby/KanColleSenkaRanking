using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace KanColleSenkaRanking.Models
{
    public class DevStateData
    {
        public string Name { get { return _name; } }
        public TextWithStyle TypeText { get { return TypeConveter(_type); } }
        public TextWithStyle StateText { get { return StateConveter(_state); } }
        public string StartDateString { get { return DateConveter(_startDate); } }
        public string EndDateString { get { return DateConveter(_endDate); } }

        private string _type;
        private string _name;
        private StateType _state;
        private DateTime _startDate;
        private DateTime _endDate;

        private static string DirPath = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
        private const string csvFile = "DevState.csv";

        public DevStateData(string csvLine) {
            var data = csvLine.Split(',');
            _type = data[0];
            _name = data[1];
            _state = (StateType)Enum.Parse(typeof(StateType), data[2]);
            if (data[3] == "") {
                _startDate = DateTime.MaxValue;
            } else {
                _startDate = DateTime.ParseExact(data[3], "M/d/yyyy", null);
            }
            if (data[4] == "") {
                _endDate = DateTime.MaxValue;
            } else {
                _endDate = DateTime.ParseExact(data[4], "M/d/yyyy", null);
            }
        }

        public static IList<DevStateData> GetFromFile() {
            List<DevStateData> dataset = new List<DevStateData>();
            string path = Path.Combine(DirPath, csvFile);
            foreach (var line in File.ReadAllLines(path).Skip(1)) {
                dataset.Add(new DevStateData(line));
            }
            dataset = dataset.OrderBy(data => data._state)
                .ThenBy(data => data._startDate)
                .ThenBy(data => data._type)
                .ThenBy(data => data._endDate)
                .ToList();
            return dataset;
        }

        private TextWithStyle TypeConveter(string type) {
            TextWithStyle vws = new TextWithStyle(type);
            if (type.Contains("Database")) {
                vws.HtmlClass = "bcl-red";
            } else if (type.Contains("Front")) {
                vws.HtmlClass = "bcl-yellow";
            } else if (type.Contains("Back")) {
                vws.HtmlClass = "bcl-blue";
            } else if (type.Contains("Other")) {
                vws.HtmlClass = "bcl-gray";
            }
            return vws;
        }

        private TextWithStyle StateConveter(StateType state) {
            TextWithStyle vws = new TextWithStyle(state.ToString());
            if (state == StateType.Done) {
                vws.HtmlClass = "cl-green";
            } else if (state == StateType.InProgress) {
                vws.HtmlClass = "cl-blue font-bold";
            } else if (state == StateType.Planned) {
                vws.HtmlClass = "cl-gray";
            }
            return vws;
        }

        private string DateConveter(DateTime date) {
            if (date == DateTime.MaxValue) {
                return "";
            } else {
                return date.ToString("yyyy/M/d");
            }
        }

        public enum StateType
        {
            Done,
            InProgress,
            Planned
        }
    }
}