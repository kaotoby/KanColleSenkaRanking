using KanColleSenkaRanking.Models;
using MvcSiteMapProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KanColleSenkaRanking.SiteMap
{
    public class PlayerDynamicNodeProvider : DynamicNodeProviderBase
    {
        private SenkaManager serverManager = DependencyResolver.Current.GetService<SenkaManager>();

        public override IEnumerable<DynamicNode> GetDynamicNodeCollection(ISiteMapNode node) {
            foreach (var server in serverManager.Servers.Values) {
                IList<SenkaData> playerList = server.GetFullPlayerList();
                foreach (var player in playerList) {
                    DynamicNode dynamicNode = new DynamicNode();
                    dynamicNode.Title = player.PlayerName;
                    dynamicNode.Description = string.Format("{0} ({1})の戦果データ。", player.PlayerName, server.Name);

                    dynamicNode.ChangeFrequency = ChangeFrequency.Daily;
                    dynamicNode.UpdatePriority = UpdatePriority.Absolute_040; //Below Normal
                    dynamicNode.RouteValues.Add("playerID", player.PlayerID);
                    dynamicNode.ParentKey = "Server" + server.ID.ToString();

                    yield return dynamicNode;
                }
            }
        }
    }
}