using MvcSiteMapProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KanColleSenkaRanking.Models
{
    public class ServerRankingDynamicNodeProvider : DynamicNodeProviderBase
    {
        private SenkaManager serverManager = DependencyResolver.Current.GetService<SenkaManager>();

        public override IEnumerable<DynamicNode> GetDynamicNodeCollection(ISiteMapNode node) {
            foreach (var server in serverManager.Servers.Values) {
                DynamicNode dynamicNode = new DynamicNode();
                if (server.Enabled) {
                    dynamicNode.Description = string.Format("「{0}」の最新の戦果ランキングです。", server.Name);
                } else {
                    dynamicNode.Description = "このサーバの情報今はありません。";
                }
                dynamicNode.Title = "ランキング";
                dynamicNode.ChangeFrequency = ChangeFrequency.Hourly;
                dynamicNode.UpdatePriority = UpdatePriority.High;
                dynamicNode.RouteValues.Add("serverID", server.ID);
                dynamicNode.ParentKey = "Server" + server.ID.ToString();

                yield return dynamicNode;
            }
        }
    }
}