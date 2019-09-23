namespace SteamAccCreator.Web
{
    public class ErrorMessages
    {
        public class Steam
        {
            public const string UNKNOWN = "Steam has disallowed your IP making this account";
            public const string PROBABLY_IP_BAN = "Your IP probably banned by Steam.";
            public const string TRASH_MAIL = "Disposable mail address detected. This mail domain was banned.";
            public const string WRONG_CAPTCHA = "Wrong captcha... Error";
            public const string EMAIL_ERROR = "Email error";
            public const string SIMILIAR_MAIL = "This e-mail address must be different from your own.";
            public const string INVALID_MAIL = "Please enter a valid email address.";
            public const string REGISTRATION = "There was an error with your registration, please try again.";
            public const string TIMEOUT = "You've waited too long to verify your email. Please try creating your account and verifying your email again.";
            public const string MAIL_UNVERIFIED = "Trying to verify mail...";
            public const string MAIL_UNVERIFIED_HAND_MODE = "Waiting for mail confirmation...";
            public const string ALIAS_UNAVAILABLE = "Alias (login) already in use!";
            public const string PASSWORD_UNSAFE = "Password not safe enough!";
            public const string ACCOUNT_SEEMS_CREATED = "Account seems to be created but something was broken...";
        }

        public class Mail
        {
            public const string NO_FREE_DOMAIN = "Cannot get free domain! Use custom domain or try again (later)?..";
            public const string SERVICE_ERROR = "Mail service probably broken. You can try again later...";
        }

        public const string HTTP_FAILED = "HTTP Request failed";
    }
}