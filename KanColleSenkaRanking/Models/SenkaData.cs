using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KanColleSenkaRanking.Models
{
    public class SenkaData
    {
        #region Public Declarition
        /// <summary>
        /// The time that data collected.
        /// </summary>
        public DateTime Date { get { return _date; } }
        /// <summary>
        /// The time string that data collected.
        /// </summary>
        public string DateString { get { return _date.ToString("yyyy年M月d日 H時"); } }
        /// <summary>
        /// The player ranking.
        /// </summary>
        public int Ranking { get { return _ranking; } }
        /// <summary>
        /// The player level.
        /// </summary>
        public int Level { get { return _level; } }
        /// <summary>
        /// The player nickname.
        /// </summary>
        public string PlayerName { get { return _playerName; } }
        /// <summary>
        /// The player ID. (In DMM database?)
        /// </summary>
        public long PlayerID { get { return _playerID; } }
        /// <summary>
        /// The player comment at the time.
        /// </summary>
        public string Comment { get { return _comment; } }
        /// <summary>
        /// Senka (戦果) point.
        /// </summary>
        public int RankPoint { get { return _rankPoint; } }
        /// <summary>
        /// The full name of the rank type.
        /// </summary>
        public string RankName { get { return _rankName; } }
        /// <summary>
        /// The medals that player has. (Currently only 甲 medal)
        /// </summary>
        public int Medals { get { return _medals; } }
        /// <summary>
        /// The delta value of ranking.
        /// </summary>
        public TextWithStyle RankingDelta { get { return RankingDeltaConverter(_rankingDelta); } }
        /// <summary>
        /// The delta value of rank point.
        /// </summary>
        public TextWithStyle RankPointDelta { get { return RankPointDeltaConverter(_rankPointDelta); } }
        #endregion

        #region Private Declarition
        private DateTime _date;
        private int _ranking;
        private int _level;
        private string _playerName;
        private long _playerID;
        private string _comment;
        private int _rankPoint;
        private string _rankName;
        private int _medals;
        private int? _rankingDelta;
        private int? _rankPointDelta;
        #endregion

        public SenkaData(Dictionary<string, object> data) {
            _date = Convert.ToDateTime(data["Date"]);
            _ranking = Convert.ToInt32(data["Ranking"]);
            _level = Convert.ToInt32(data["Level"]);
            _playerName = Convert.ToString(data["PlayerName"]);
            _playerID = Convert.ToInt64(data["PlayerID"]);
            _comment = Convert.ToString(data["Comment"]);
            _rankPoint = Convert.ToInt32(data["RankPoint"]);
            _rankName = Convert.ToString(data["RankName"]);
            _medals = Convert.ToInt32(data["Medals"]);
        }

        public void SetDelta(SenkaData compareData) {
            _rankingDelta = compareData._ranking - this._ranking;
            _rankPointDelta = this._rankPoint - compareData._rankPoint;
        }

        private TextWithStyle RankingDeltaConverter(int? delta) {
            TextWithStyle data = new TextWithStyle();
            if (delta.HasValue) {
                int value = delta.Value;
                data.Value = string.Format("({0}{1})", value >= 0 ? "\u2191" : "\u2193", Math.Abs(value));
                if (value == 0) {
                    data.HtmlClass = "cl-gray";
                } else if (value < 0) {
                    data.HtmlClass = "cl-red";
                } else {
                    data.HtmlClass = "cl-green";
                }
            }
            return data;
        }

        private TextWithStyle RankPointDeltaConverter(int? delta) {
            TextWithStyle data = new TextWithStyle();
            if (delta.HasValue) {
                int value = delta.Value;
                data.Value = string.Format("(+{0})", value);
                if (value < 50) {
                    data.HtmlClass = "cl-gray";
                } else if (value < 100) {
                    data.HtmlClass = "cl-green";
                } else if (value < 150) {
                    data.HtmlClass = "cl-blue";
                } else {
                    data.HtmlClass = "cl-blue font-bold";
                }
            }
            return data;
        }
    }
}