using System.Text.RegularExpressions;

namespace Assets._Scripts.Util
{
    public static class FormUtil
    {
        public static bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email.Trim(),
                @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
                RegexOptions.IgnoreCase);
        }

        public static bool IsPasswordValid(string value)
        {
            return value != null && value.Trim().Length < 6 || value.Trim().Length > 12 ? false : true;
        }

        public static bool IsPasswordRepeatValid(string value0, string value1)
        {
            return value0 == value1;
        }

        public static bool IsNotEmpty(string value)
        {
            return value != null && value.Trim().Length > 0;
        }

        public static bool IsValidName(string value)
        {
            return value != null && value.Trim().Length > 0 && Regex.IsMatch(value, @"^[a-zA-Z0-9\s]*$");
        }

        public static bool IsValidCel(string value)
        {
            return value != null && value.Trim().Length == 11;
        }

        public static bool IsValidCode(string value)
        {
            return value != null && value.Trim().Length == 5;
        }
    }
}
