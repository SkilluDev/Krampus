using System.Text;

namespace KrampUtils {
    public static class MoreString {

        public static string ToPrettyCase(this string str) {
            str = str
            .Replace("_", " ")
            .Replace("  ", " ")
            .Replace("  ", " ")
            .Trim();

            var sb = new StringBuilder();

            for (int i = 0; i < str.Length; i++) {
                if (i == 0) {
                    sb.Append(str[i].ToString().ToUpper()[0]);
                } else {
                    if (string.IsNullOrWhiteSpace(str[i - 1].ToString())) {
                        sb.Append(str[i].ToString().ToUpper()[0]);
                    } else {
                        sb.Append(str[i].ToString().ToLower()[0]);
                    }
                }
            }

            return sb.ToString();
        }
    }
}