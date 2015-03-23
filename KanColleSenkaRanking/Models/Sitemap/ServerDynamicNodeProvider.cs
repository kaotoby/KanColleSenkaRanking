using MvcSiteMapProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KanColleSenkaRanking.Models
{
    public class ServerDynamicNodeProvider : DynamicNodeProviderBase
    {
        private SenkaManager serverManager = DependencyResolver.Current.GetService<SenkaManager>();

        public override IEnumerable<DynamicNode> GetDynamicNodeCollection(ISiteMapNode node) {
            foreach (var server in serverManager.Servers.Values) {
                DynamicNode dynamicNode = new DynamicNode("Server" + server.ID.ToString(), server.Name);
                if (server.Enabled) {
                    dynamicNode.Description = string.Format("{0} {1}の戦果データ。", server.LastUpdateTimeString, server.Name);
                } else {
                    dynamicNode.Description = "このサーバの情報今はありません。";
                }
                dynamicNode.ChangeFrequency = ChangeFrequency.Hourly;
                dynamicNode.UpdatePriority = UpdatePriority.High;
                dynamicNode.RouteValues.Add("serverID", server.ID);

                yield return dynamicNode;
            }
        }
    }
}