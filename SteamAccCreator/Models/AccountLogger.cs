using SteamAccCreator.Web;
using System;

namespace SteamAccCreator.Models
{
    public class AccountLogger : SACModuleBase.ISACLogger
    {
        private Account Account { get; }

        public AccountLogger(Account account)
        {
            Account = account;
        }

        private string LogAccInfo => $"{{{nameof(Account)};login={Account.Login};mail={Account.Mail}}}";

        public void Trace(string message)
            => Logger.Trace($"{LogAccInfo}: {message}");

        public void Info(string message)
            => Logger.Info($"{LogAccInfo}: {message}");

        public void Debug(string message)
            => Logger.Debug($"{LogAccInfo}: {message}");

        public void Warn(string message)
            => Logger.Warn($"{LogAccInfo}: {message}");
        public void Warn(string message, Exception exception)
            => Logger.Warn($"{LogAccInfo}: {message}", exception);

        public void Error(string message, Exception exception)
            => Logger.Error($"{LogAccInfo}: {message}", exception);

        public void Fatal(string message, Exception exception)
            => Logger.Fatal($"{LogAccInfo}: {message}", exception);
    }
}
