using RestSharp;
using SACModuleBase;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SteamAccCreator.Web.Steam.Abstractions
{
    public abstract class SteamCategoryBase
    {
        internal SteamWebClient Steam;
        internal RestClient HttpClient => Steam.HttpClient;

        internal ISACLogger Logger { get; }

        internal SteamCategoryBase(SteamWebClient steam, ISACLogger logger)
        {
            Steam = steam;
            Logger = logger;
        }

        internal IRestResponse Execute(IRestRequest request)
            => HttpClient.Execute(request);

        internal void TraceMethod()
        {
            var stack = new StackTrace();

            var whoCallMe = stack.GetFrame(1);
            var method = whoCallMe.GetMethod();
            var parameters = method.GetParameters();

            if (parameters.Count() > 0)
            {
                var paramsList = new List<string>();
                foreach (var p in parameters)
                {
                    paramsList.Add($"{(p.IsOut ? "out " : "")}{p.Name}");
                }
                Logger.Trace($"{GetType().Name}::{method.Name}({string.Join(", ", paramsList)})");
            }
            else
                Logger.Trace($"{GetType().Name}::{method.Name}()");
        }
    }
}
