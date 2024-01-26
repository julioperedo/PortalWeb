using System.Globalization;

namespace MobileService.Misc
{
    public class Common
    {
        public static string ToTitle(string message)
        {
            string result = "";
            if (!string.IsNullOrWhiteSpace(message))
            {
                TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
                result = myTI.ToTitleCase(message.ToLower());
            }
            return result;
        }
    }
}
