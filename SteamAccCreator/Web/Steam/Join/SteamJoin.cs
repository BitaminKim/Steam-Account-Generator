using HtmlAgilityPack;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SteamAccCreator.Web.Steam.Join
{
    public class SteamJoin : Abstractions.SteamCategoryBase
    {
        public SteamJoin(SteamWebClient steam, SACModuleBase.ISACLogger logger) : base(steam, logger) { }

        private string g_strRedirectURL = string.Empty;
        private static readonly Regex SteamProfileRegex = new Regex(@"\/profiles\/(\d+)", RegexOptions.IgnoreCase);

        /// <summary>
        /// Will be initial request
        /// </summary>
        public SteamResponse<bool> GetJoinPage()
        {
            TraceMethod();

            // it's probably stupid... but it's can be legit to steam

            var requestStore = new RestRequest(SteamDefaultUrls.STORE_BASE, Method.GET);
            requestStore.AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            var responseStore = Execute(requestStore);
            if (!responseStore.IsSuccessful)
                return new SteamResponse<bool>(false, responseStore);

            Logger.Debug("Trying to find login link...");

            var storeHtml = new HtmlDocument();
            storeHtml.LoadHtml(responseStore.Content);

            var loginUrl = SteamDefaultUrls.LOGIN;
            var nodesStore = storeHtml?.DocumentNode?.SelectNodes("//a[@class='global_action_link']");
            if (nodesStore != null)
            {
                foreach (var node in nodesStore)
                {
                    if (node == null)
                        continue;
                    if (node.NodeType != HtmlNodeType.Element)
                        continue;

                    if (!node.Attributes.Any(x => x.Name.ToLower() == "href"))
                        continue;

                    var url = node.GetAttributeValue("href", SteamDefaultUrls.LOGIN);
                    if (!url.ToLower().Contains("/login"))
                        continue;

                    loginUrl = url;
                    break;
                }
            }

            Logger.Debug($"Login link is: {loginUrl}");

            var responsesList = new List<IRestResponse>();
            responsesList.Add(responseStore);

            Steam.LegitDelay();

            Logger.Debug("Requesting login link...");

            var loginRequest = new RestRequest(loginUrl, Method.GET);
            loginRequest.AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            var responseLogin = Execute(loginRequest);
            responsesList.Add(responseLogin);
            if (!responseLogin.IsSuccessful)
            {
                Logger.Warn("Failed to request login link!");
                return new SteamResponse<bool>(false, responsesList);
            }

            Logger.Debug("Trying to find join link...");

            var joinUrl = SteamDefaultUrls.JOIN;
            var nodesJoin = storeHtml?.DocumentNode?.SelectNodes("//a[@class='btnv6_blue_hoverfade btn_medium']");
            if (nodesJoin != null)
            {
                foreach (var node in nodesJoin)
                {
                    if (node == null)
                        continue;
                    if (node.NodeType != HtmlNodeType.Element)
                        continue;

                    if (!node.Attributes.Any(x => x.Name.ToLower() == "href"))
                        continue;

                    var url = node.GetAttributeValue("href", SteamDefaultUrls.LOGIN);
                    if (!url.ToLower().Contains("/join"))
                        continue;

                    joinUrl = url;
                    break;
                }
            }

            Logger.Debug($"Join link is: {joinUrl}");

            Steam.LegitDelay();

            Logger.Debug("Requesting join link...");

            var requestJoin = new RestRequest(joinUrl, Method.GET);
            requestJoin.AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            var responseJoin = HttpClient.Execute(requestJoin);
            responsesList.Add(responseJoin);
            if (!responseJoin.IsSuccessful)
            {
                Logger.Warn("Failed to request join link!");
                return new SteamResponse<bool>(false, responsesList);
            }

            // check if any steam related links on page
            var isSuccessJoin = Regex.IsMatch(responseJoin.Content ?? "", @"\.steampowered\.com|akamaihd\.net|steamcommunity\.com", RegexOptions.IgnoreCase);
            if (isSuccessJoin)
            {
                // get redirect link
                // to be legit?
                var redirectUrl = Regex.Match(responseJoin.Content ?? "", "var\\s+g_strRedirectURL\\s+?\\=\\s+?\\\"([^\"]+)\\\";", RegexOptions.IgnoreCase);
                if (redirectUrl.Success)
                {
                    g_strRedirectURL = Regex.Replace(redirectUrl.Groups[1].Value, "\\\\/", "/"); // escape \/ in url
                    Logger.Debug($"{nameof(g_strRedirectURL)} now is: {g_strRedirectURL}");
                }
                else
                {
                    Logger.Warn($"{nameof(g_strRedirectURL)} not found on join page. Using default (='{SteamDefaultUrls.STORE_BASE}')");
                    g_strRedirectURL = SteamDefaultUrls.STORE_BASE;
                }
            }
            else
                Logger.Warn("Cannot find anything related to Steam on join page!");

            return new SteamResponse<bool>(responseJoin.IsSuccessful, responsesList);
        }

        public SteamResponse<Steam.Models.SteamCaptcha> RefreshCaptcha(int count = 1)
            => Steam.GetCaptcha(SteamDefaultUrls.JOIN_REFRESH_CAPTCHA);

        public SteamResponse<Models.VerifyMailResponse> VerifyEmail(string email, string captchaGid, string captchaSolution)
        {
            TraceMethod();

            var request = new RestRequest(SteamDefaultUrls.AJAX_VERIFY_EMAIL, Method.POST);
            request.AddParameter("email", email);
            request.AddParameter("captchagid", captchaGid);
            request.AddParameter("captcha_text", captchaSolution);

            request.AddHeader("Accept", "*/*");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
            request.AddHeader("X-Requested-With", "XMLHttpRequest");

            return Steam.Execute<Models.VerifyMailResponse>(request);
        }

        public SteamResponse<string> CheckEmailVerified(Models.VerifyMailResponse verifyMailResponse)
            => CheckEmailVerified(verifyMailResponse?.CreationId);
        public SteamResponse<string> CheckEmailVerified(string creationId)
        {
            TraceMethod();

            if (string.IsNullOrEmpty(creationId))
            {
                Logger.Warn($"{nameof(creationId)} is empty!");
                return new SteamResponse<string>();
            }

            var request = new RestRequest(SteamDefaultUrls.AJAX_CHECK_VERIFIED, Method.POST);
            request.AddParameter("creationid", creationId);

            request.AddHeader("Accept", "*/*");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
            request.AddHeader("X-Requested-With", "XMLHttpRequest");

            var response = Execute(request);
            return new SteamResponse<string>(response.Content, response);
        }

        public SteamResponse<Models.LoginAvailableResponse> CheckLogin(string accountName, int count = 1)
        {
            TraceMethod();

            var request = new RestRequest(SteamDefaultUrls.CHECK_AVAILABLE, Method.POST);
            request.AddParameter("accountname", accountName);
            request.AddParameter("count", count);

            request.AddHeader("Accept", "text/javascript, text/html, application/xml, text/xml, */*");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
            request.AddHeader("X-Prototype-Version", "1.7");
            request.AddHeader("X-Requested-With", "XMLHttpRequest");

            return Steam.Execute<Models.LoginAvailableResponse>(request);
        }

        public SteamResponse<Models.PasswordAvailableResponse> CheckPassword(string password, string accountName, int count = 1)
        {
            TraceMethod();

            var request = new RestRequest(SteamDefaultUrls.CHECK_AVAILABLE_PASSWORD, Method.POST);
            request.AddParameter("password", password);
            request.AddParameter("accountname", accountName);
            request.AddParameter("count", count);

            request.AddHeader("Accept", "text/javascript, text/html, application/xml, text/xml, */*");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
            request.AddHeader("X-Prototype-Version", "1.7");
            request.AddHeader("X-Requested-With", "XMLHttpRequest");

            return Steam.Execute<Models.PasswordAvailableResponse>(request);
        }
        public SteamResponse<Models.CreateAccountResponse> CreateAccount(string accountName, string password, Models.VerifyMailResponse verifyMailResponse,
            int count = 1, int lt = 0, int embeddedAppId = 0)
            => CreateAccount(accountName, password, verifyMailResponse.CreationId, count, lt, embeddedAppId);
        public SteamResponse<Models.CreateAccountResponse> CreateAccount(string accountName, string password, string creationId,
            int count = 1, int lt = 0, int embeddedAppId = 0)
        {
            TraceMethod();

            var requestCreate = new RestRequest(SteamDefaultUrls.CREATE_ACCOUNT, Method.POST);
            requestCreate.AddParameter("accountname", accountName);
            requestCreate.AddParameter("password", password);
            requestCreate.AddParameter("count", count);
            requestCreate.AddParameter("lt", lt);
            requestCreate.AddParameter("creation_sessionid", creationId);
            requestCreate.AddParameter("embedded_appid", embeddedAppId);

            requestCreate.AddHeader("Accept", "text/javascript, text/html, application/xml, text/xml, */*");
            requestCreate.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
            requestCreate.AddHeader("X-Prototype-Version", "1.7");
            requestCreate.AddHeader("X-Requested-With", "XMLHttpRequest");

            return Steam.Execute<Models.CreateAccountResponse>(requestCreate);
        }

        public SteamResponse<bool> CallRedirectUrl(string creationId)
        {
            TraceMethod();

            if (string.IsNullOrEmpty(g_strRedirectURL))
            {
                Logger.Warn($"{nameof(g_strRedirectURL)} is empty!");
                return new SteamResponse<bool>(new Exception($"{nameof(g_strRedirectURL)} is empty!"));
            }

            var _strRedirectURL = $"{g_strRedirectURL}{((g_strRedirectURL.IndexOf('?') > 0) ? '&' : '?')}creationid={creationId}";
            var request = new RestRequest(_strRedirectURL, Method.GET);
            request.AddHeader("Accept", "text/javascript, text/html, application/xml, text/xml, */*");
            var response = HttpClient.Execute(request);
            if (!response.IsSuccessful)
            {
                Logger.Warn($"Failed to request {nameof(g_strRedirectURL)} ({g_strRedirectURL})!");
                return new SteamResponse<bool>(false, response);
            }

            // check if any steam related links on page
            var isSuccess = Regex.IsMatch(response.Content ?? "", @"\.steampowered\.com|akamaihd\.net|steamcommunity\.com", RegexOptions.IgnoreCase);
            if (!isSuccess)
            {
                Logger.Warn($"Failed to find on {nameof(g_strRedirectURL)} anything related to Steam!");
                return new SteamResponse<bool>(false, response);
            }

            var profileIdRegex = SteamProfileRegex.Match(response.Content ?? "");
            if (profileIdRegex.Success)
                Steam.SteamId = long.Parse(profileIdRegex.Groups[1].Value);
            else
                Logger.Warn("Failed to find Steam ID!");

            return new SteamResponse<bool>(true, response);
        }
    }
}
