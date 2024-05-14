using Assets._Scripts.Manager.Language.Attributes;


namespace Assets._Scripts.Manager.Language.Token
{
    public static class LanguageManagerToken
    {
        public static class common
        {
            public static string error_token { get { return LanguageManager.Instance.GetTranslation("common", "error_token"); } }
            [LanguageManagerIgnoreToken()]
            public static string error_token_en_US { get { return LanguageManager.Instance.GetTranslation("en_US", "common", "error_token"); } }
            [LanguageManagerIgnoreToken()]
            public static string error_token_pt_BR { get { return LanguageManager.Instance.GetTranslation("pt_BR", "common", "error_token"); } }
            public static string accept_token { get { return LanguageManager.Instance.GetTranslation("common", "accept_token"); } }
            [LanguageManagerIgnoreToken()]
            public static string accept_token_en_US { get { return LanguageManager.Instance.GetTranslation("en_US", "common", "accept_token"); } }
            [LanguageManagerIgnoreToken()]
            public static string accept_token_pt_BR { get { return LanguageManager.Instance.GetTranslation("pt_BR", "common", "accept_token"); } }
            public static string close_token { get { return LanguageManager.Instance.GetTranslation("common", "close_token"); } }
            [LanguageManagerIgnoreToken()]
            public static string close_token_en_US { get { return LanguageManager.Instance.GetTranslation("en_US", "common", "close_token"); } }
            [LanguageManagerIgnoreToken()]
            public static string close_token_pt_BR { get { return LanguageManager.Instance.GetTranslation("pt_BR", "common", "close_token"); } }
        }

        public static class splash
        {
            public static string message_0 { get { return LanguageManager.Instance.GetTranslation("splash", "message_0"); } }
            [LanguageManagerIgnoreToken()]
            public static string message_0_en_US { get { return LanguageManager.Instance.GetTranslation("en_US", "splash", "message_0"); } }
            [LanguageManagerIgnoreToken()]
            public static string message_0_pt_BR { get { return LanguageManager.Instance.GetTranslation("pt_BR", "splash", "message_0"); } }
            public static string message_1 { get { return LanguageManager.Instance.GetTranslation("splash", "message_1"); } }
            [LanguageManagerIgnoreToken()]
            public static string message_1_en_US { get { return LanguageManager.Instance.GetTranslation("en_US", "splash", "message_1"); } }
            [LanguageManagerIgnoreToken()]
            public static string message_1_pt_BR { get { return LanguageManager.Instance.GetTranslation("pt_BR", "splash", "message_1"); } }
        }
    }
}