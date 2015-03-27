using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace KanColleSenkaService.Module
{
    public class SenkaData
    {
        #region Public Declarition
        /// <summary>
        /// The player ranking.
        /// </summary>
        public int Ranking { get { return int.Parse(_rawdata.api_no); } }
        /// <summary>
        /// The player level.
        /// </summary>
        public int Level { get { return int.Parse(_rawdata.api_level); } }
        /// <summary>
        /// The player nickname.
        /// </summary>
        public string PlayerName { get { return _rawdata.api_nickname; } }
        /// <summary>
        /// The player ID. (In DMM database?)
        /// </summary>
        public long PlayerID { get { return long.Parse(_rawdata.api_member_id); } }
        /// <summary>
        /// The player comment at the time.
        /// </summary>
        public string Comment { get { return _rawdata.api_comment; } }
        /// <summary>
        /// Senka (戦果) point.
        /// </summary>
        public int RankPoint { get { return int.Parse(_rawdata.api_rate); } }
        /// <summary>
        /// The number repersent rank type. (Ex. 元帥)
        /// </summary>
        public int RankType { get { return int.Parse(_rawdata.api_rank); } }
        /// <summary>
        /// The medals that player has. (Currently only 甲 medal)
        /// </summary>
        public int Medals { get { return int.Parse(_rawdata.api_medals); } }
        /// <summary>
        /// The player experience.
        /// </summary>
        public int Exp { get { return int.Parse(_rawdata.api_experience); } }
        #endregion

        #region Private Declarition
        private ApiSenkaResult _rawdata;
        #endregion

        public SenkaData(ApiSenkaResult data) {
            _rawdata = data;
        }

        public override string ToString() {
            return string.Format("[{0}:{1}][lv.{2}][{3}] {4} {5}",
                Ranking, RankPoint, Level,
                PlayerName.PadRight(12, '　'),
                Comment.PadRight(12, '　'),
                Medals == 1 ? "(甲)" : "");
        }
    }
}
