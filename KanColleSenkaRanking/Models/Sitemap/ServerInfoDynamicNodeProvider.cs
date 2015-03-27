using KanColleSenkaRanking.Models;
using MvcSiteMapProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KanColleSenkaRanking.SiteMap
{
    public class ServerInfoDynamicNodeProvider : DynamicNodeProviderBase
    {
        private SenkaManager serverManager = DependencyResolver.Current.GetService<SenkaManager>();

        public override IEnumerable<DynamicNode> GetDynamicNodeCollection(ISiteMapNode node) {
            foreach (var server in serverManager.Servers.Values) {
                DynamicNode dynamicNode = new DynamicNode("Server" + server.ID.ToString(), server.Name);
                if (server.Enabled) {
                    dynamicNode.Description = string.Format("「{0}」の最新の戦果情報です。", server.Name);
                } else {
                    dynamicNode.Description = "このサーバの情報今はありません。";
                }
                dynamicNode.ChangeFrequency = ChangeFrequency.Daily;
                dynamicNode.RouteValues.Add("serverID", server.ID);

                yield return dynamicNode;
            }
        }
    }
}