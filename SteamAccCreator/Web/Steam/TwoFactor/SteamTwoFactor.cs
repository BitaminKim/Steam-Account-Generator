using RestSharp;
using System.Collections.Generic;

namespace SteamAccCreator.Web.Steam.TwoFactor
{
    public class SteamTwoFactor : Abstractions.SteamCategoryBase
    {
        public SteamTwoFactor(SteamWebClient steam, SACModuleBase.ISACLogger logger) : base(steam, logger) { }

        private IRestResponse GetManage()
        {
            TraceMethod();

            var request = new RestRequest(SteamDefaultUrls.TF_MANAGE, Method.GET);
            request.AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            request.AddHeader("Referer", SteamDefaultUrls.TF_MANAGE);

            return Execute(request);
        }

        private SteamResponse<string> SetAuthToMail()
        {
            TraceMethod();

            var request = new RestRequest(SteamDefaultUrls.TF_MANAGE_ACTION, Method.POST);
            request.AddParameter("action", "email");
            request.AddParameter("sessionid", Steam.SessionId);
            request.AddParameter("email_authenticator_check", "on");

            request.AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Referer", SteamDefaultUrls.TF_MANAGE);

            var response = Execute(request);
            return new SteamResponse<string>(response.Content, response);
        }

        private SteamResponse<string> SetAuthToNone()
        {
            TraceMethod();

            var request = new RestRequest(SteamDefaultUrls.TF_MANAGE_ACTION, Method.POST);
            request.AddParameter("action", "none");
            request.AddParameter("sessionid", Steam.SessionId);
            request.AddParameter("none_authenticator_check", "on");

            request.AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Referer", SteamDefaultUrls.TF_MANAGE);

            var response = Execute(request);
            return new SteamResponse<string>(response.Content, response);
        }

        private SteamResponse<string> SetAuthToActuallyNone()
        {
            Logger.Trace($"{nameof(SteamTwoFactor)}::{nameof(SetAuthToActuallyNone)}()");

            var request = new RestRequest(SteamDefaultUrls.TF_MANAGE_ACTION, Method.POST);
            request.AddParameter("action", "actuallynone");
            request.AddParameter("sessionid", Steam.SessionId);

            request.AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Referer", SteamDefaultUrls.TF_MANAGE_ACTION);

            var response = Execute(request);
            return new SteamResponse<string>(response.Content, response);
        }

        public SteamResponse<bool> TurnOnByMail()
        {
            TraceMethod();

            var responses = new List<IRestResponse>();

            var requestMagage = GetManage();
            responses.Add(requestMagage);
            if (!requestMagage.IsSuccessful)
            {
                Logger.Warn("Failed to request guard manage page!");
                return new SteamResponse<bool>(false, responses);
            }

            Steam.LegitDelay();

            var requestMailAuth = SetAuthToMail();
            if (!requestMailAuth.IsSuccess)
            {
                Logger.Warn("Failed to request change guard auth type to mail!");
                return new SteamResponse<bool>(false, responses);
            }

            return new SteamResponse<bool>(true, responses);
        }

        public SteamResponse<bool> TurnOff()
        {
            TraceMethod();

            var responses = new List<IRestResponse>();

            var requestMagage = GetManage();
            responses.Add(requestMagage);
            if (!requestMagage.IsSuccessful)
            {
                Logger.Warn("Failed to request guard manage page!");
                return new SteamResponse<bool>(false, responses);
            }

            Steam.LegitDelay();

            var requestNoneAuth = SetAuthToNone();
            responses.AddRange(requestNoneAuth.HttpResponses);
            if (!requestNoneAuth.IsSuccess)
            {
                Logger.Warn("Failed to request change guard auth type to none!");
                return new SteamResponse<bool>(false, responses);
            }

            Steam.LegitDelay();

            var requestActuallyNoneAuth = SetAuthToActuallyNone();
            responses.AddRange(requestActuallyNoneAuth.HttpResponses);
            if (!requestActuallyNoneAuth.IsSuccess)
            {
                Logger.Warn("Failed to request confirmation of guard auth type none!");
                return new SteamResponse<bool>(false, responses);
            }

            return new SteamResponse<bool>(true, responses);
        }
    }
}
