using System;

namespace SteamAccCreator.Defaults
{
    public partial class Web
    {
        public const string USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:67.0) Gecko/20100101 Firefox/67.0";

        public const string STEAM_ADDRESS = "https://store.steampowered.com/";

        public const string STEAM_RESOURCE_JOIN = "join/";
        public const string STEAM_JOIN_ADDRESS = STEAM_ADDRESS + STEAM_RESOURCE_JOIN;
        public static readonly Uri STEAM_JOIN_URI = new Uri(STEAM_JOIN_ADDRESS);

        public const string STEAM_RESOURCE_LOGIN = "login/";
        public const string STEAM_LOGIN_ADDRESS = STEAM_ADDRESS + STEAM_RESOURCE_LOGIN;
        public const string STEAM_RESOURCE_RENDER_CAPTCHA = "rendercaptcha";
        public const string STEAM_RENDER_CAPTCHA_ADDRESS = STEAM_LOGIN_ADDRESS + STEAM_RESOURCE_RENDER_CAPTCHA;

        public const string STEAM_RESOURCE_AJAX_VERIFY_EMAIL = "ajaxverifyemail";
        public const string STEAM_AJAX_VERIFY_EMAIL_ADDRESS = STEAM_JOIN_ADDRESS + STEAM_RESOURCE_AJAX_VERIFY_EMAIL;
        public static readonly Uri STEAM_AJAX_VERIFY_EMAIL_URI = new Uri(STEAM_AJAX_VERIFY_EMAIL_ADDRESS);

        public const string STEAM_RESOURCE_AJAX_CHECK_VERIFIED = "ajaxcheckemailverified";
        public const string STEAM_AJAX_CHECK_VERIFIED_ADDRESS = STEAM_JOIN_ADDRESS + STEAM_RESOURCE_AJAX_CHECK_VERIFIED;
        public static readonly Uri STEAM_AJAX_CHECK_VERIFIED_URI = new Uri(STEAM_AJAX_CHECK_VERIFIED_ADDRESS);

        public const string STEAM_RESOURCE_CHECK_AVAILABLE = "checkavail";
        public const string STEAM_CHECK_AVAILABLE_ADDRESS = STEAM_JOIN_ADDRESS + STEAM_RESOURCE_CHECK_AVAILABLE;
        public static readonly Uri STEAM_CHECK_AVAILABLE_URI = new Uri(STEAM_CHECK_AVAILABLE_ADDRESS);

        public const string STEAM_RESOURCE_CHECK_AVAILABLE_PASSWORD = "checkpasswordavail";
        public const string STEAM_CHECK_AVAILABLE_PASSWORD_ADDRESS = STEAM_JOIN_ADDRESS + STEAM_RESOURCE_CHECK_AVAILABLE_PASSWORD;
        public static readonly Uri STEAM_CHECK_AVAILABLE_PASSWORD_URI = new Uri(STEAM_CHECK_AVAILABLE_PASSWORD_ADDRESS);

        public const string STEAM_RESOURCE_CREATE_ACCOUNT = "createaccount";
        public const string STEAM_CREATE_ACCOUNT_ADDRESS = STEAM_JOIN_ADDRESS + STEAM_RESOURCE_CREATE_ACCOUNT;
        public static readonly Uri STEAM_CREATE_ACCOUNT_URI = new Uri(STEAM_CREATE_ACCOUNT_ADDRESS);

        public const string STEAM_ACCOUNT_VERIFY_ADDRESS = STEAM_ADDRESS + "account/newaccountverification";
        public static readonly Uri STEAM_ACCOUNT_VERIFY_URI = new Uri(STEAM_ACCOUNT_VERIFY_ADDRESS);
    }
}
