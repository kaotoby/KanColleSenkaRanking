using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net;
using System.Web;

namespace KanColleSenkaService.Models
{
    public class DMMLoginHelper
    {
        private string _username;
        private string _password;
        private int _serverID;
        private HttpHelper helper;
        private static readonly ILog log = LogManager.GetLogger(typeof(DMMLoginHelper).FullName);

        private const string ServerConstPath = "/gadget/js/kcs_const.js";
        private const string ApiPath = "/kcsapi/api_auth_member/dmmlogin/";
        private const string LoginPagePath = "Sg9VTQFXDFcX";
        private const string LoginPage = "https://www.dmm.com/my/-/login/=/path=" + LoginPagePath;
        private const string LoginAjax = "https://www.dmm.com/my/-/login/ajax-get-token/";
        private const string LoginAuth = "https://www.dmm.com/my/-/login/auth/";
        private const string NetGamePage = "http://www.dmm.com/netgame/";
        private const string GamePage = NetGamePage + "social/-/gadgets/=/app_id=854854/";
        private const string GadgetPage = "http://osapi.dmm.com/gadgets/makeRequest";
        private string JSPage = "";
        private string IfreamPage = "";
        private string IfreamReferer = "";

        public DMMLoginHelper(string username, string password, int serverID) {
            _username = username;
            _password = password;
            _serverID = serverID;
            helper = new HttpHelper();
            InitializeCookies();
        }

        public void Process(out string ip, out string apiToken, out string apiStartTime) {
            string pageToken, dmmToken;
            string gadgetURL, gadgetST, gadgetGADGET;
            DMMAjaxResult formToken;

            GetPageTokens(out pageToken, out dmmToken);
            formToken = GetFormTokens(pageToken, dmmToken);
            LoginDMM(formToken);
            GetGadgetTokens(out gadgetURL, out gadgetST, out gadgetGADGET, out ip);

            Dictionary<string, string> postDic = new Dictionary<string, string>();
            Dictionary<string, string> customHeader = new Dictionary<string, string>();
            customHeader["Cache-Control"] = "no-cache";
            customHeader["Pragma"] = "no-cache";
            postDic["url"] = gadgetURL;
            postDic["httpMethod"] = "GET";
            postDic["headers"] = "";
            postDic["postData"] = "";
            postDic["authz"] = "signed";
            postDic["st"] = gadgetST;
            postDic["contentType"] = "JSON";
            postDic["numEntries"] = "3";
            postDic["getSummaries"] = "false";
            postDic["signOwner"] = "true";
            postDic["signViewer"] = "true";
            postDic["gadget"] = gadgetGADGET;
            postDic["container"] = "dmm";
            postDic["bypassSpecCache"] = "";
            postDic["getFullHeaders"] = "false";
            postDic["oauthState"] = "";
            
            string jsonResult = "";
            helper.CTRHttp(GadgetPage, IfreamReferer, postDic, customHeader, ref jsonResult);
            jsonResult = jsonResult.Substring(jsonResult.IndexOf('{'));
            string bodyData = (string)JObject.Parse(jsonResult).First.First["body"];
            JObject jsonData = JObject.Parse(bodyData.Replace("svdata=", ""));
            apiToken = (string)jsonData["api_token"];
            apiStartTime = (string)jsonData["api_starttime"];
        }

        private void GetPageTokens(out string pageToken, out string dmmToken) {
            string result = "";
            Regex regPageToken = new Regex("\"token\": \"([0-9a-f]+)\"");
            Regex regDmmToken = new Regex("\"DMM_TOKEN\", \"([0-9a-f]+)\"");
            helper.CTRHttp(LoginPage, ref result);
            pageToken = regPageToken.Match(result).Groups[1].Value;
            dmmToken = regDmmToken.Match(result).Groups[1].Value;
        }

        private DMMAjaxResult GetFormTokens(string pageToken, string dmmToken) {
            Dictionary<string, string> postDic = new Dictionary<string, string>();
            Dictionary<string, string> customHeader = new Dictionary<string, string>();
            customHeader["X-Requested-With"] = "XMLHttpRequest";
            customHeader["DMM_TOKEN"] = dmmToken;
            postDic["token"] = pageToken;

            string jsonResult = "";
            helper.CTRHttp(LoginAjax, LoginPage, postDic, customHeader, ref jsonResult);
            DMMAjaxResult ajaxResult = JsonConvert.DeserializeObject<DMMAjaxResult>(jsonResult);

            return ajaxResult;
        }

        private void LoginDMM(DMMAjaxResult formToken) {
            string loginResult = "";
            Dictionary<string, string> postDic = new Dictionary<string, string>();
            postDic["token"] = formToken.token;
            postDic["login_id"] = _username;
            postDic["save_login_id"] = "0";
            postDic["password"] = _password;
            postDic["save_password"] = "0";
            postDic["use_auto_login"] = "0";
            postDic[formToken.login_id] = _username;
            postDic[formToken.password] = _password;
            postDic["path"] = LoginPagePath;
            postDic["prompt"] = "";
            postDic["client_id"] = "";
            postDic["display"] = "";

            helper.CTRHttp(LoginAuth, LoginPage, postDic, ref loginResult);
        }

        private void GetGadgetTokens(out string url, out string st, out string gadget, out string ip) {
            string gameResult = "", jsResult = "", ipaddress, userid;
            helper.CTRHttp(GamePage, NetGamePage, ref gameResult);

            Regex ifreamReg = new Regex("name=\"game_frame\" src=\"((.+)#rpctoken=\\d+)\" width");
            Regex xmlReg = new Regex("url=(http://(.+)/gadget.xml)");
            Regex stReg = new Regex("ST\\s+: \"(.+)\"");
            Regex useridReg = new Regex("OWNER_ID\\s+: (\\d+),");
            Regex ipReg = new Regex(string.Format("ConstServerInfo.World_{0}\\s+= \"http://(.+)/\";", _serverID));
            Match ifreamMatch = ifreamReg.Match(gameResult);
            IfreamReferer = ifreamMatch.Groups[1].Value;
            IfreamPage = ifreamMatch.Groups[2].Value;
            userid = useridReg.Match(gameResult).Groups[1].Value;

            Match xmlMatch = xmlReg.Match(HttpUtility.UrlDecode(IfreamPage));
            JSPage = "http://" + xmlMatch.Groups[2].Value + ServerConstPath;
            helper.CTRHttp(JSPage, IfreamReferer, ref jsResult);
            ipaddress = ipReg.Match(jsResult).Groups[1].Value;

            ip = ipaddress;
            gadget = xmlMatch.Groups[1].Value;
            st = stReg.Match(gameResult).Groups[1].Value;
            url = string.Format("http://{0}{1}{2}/1/{3}", ipaddress, ApiPath, userid, GetTimestamp());
        }

        private void InitializeCookies() {
            Dictionary<string, Cookie> Cookies = new Dictionary<string, Cookie>();
            Cookies["cklg"] = new Cookie("cklg", "ja");

            foreach (var data in Cookies) {
                Cookie c = data.Value;
                c.Domain = "dmm.com";
                c.Expires = c.TimeStamp.AddYears(1);
                helper.Coa.Add(c);
            }
        }

        private string GetTimestamp() {
            TimeSpan timestamp = DateTime.UtcNow - new DateTime(1970, 1, 1);
            double ms = Math.Round((timestamp).TotalMilliseconds);

            return ms.ToString();
        }
    }
}
