using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KanColleSenkaRanking.Models
{
    public class TextWithStyle
    {
        public string Value { get; set; }
        public string HtmlClass { get; set; }

        public TextWithStyle() {
        }

        public TextWithStyle(string value) {
            Value = value;
        }

        public TextWithStyle(string value, string htmlClass) {
            Value = value;
            HtmlClass = htmlClass;
        }
    }
}