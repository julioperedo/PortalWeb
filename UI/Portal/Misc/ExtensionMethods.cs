namespace Portal.Misc
{
    public static class ExtensionMethods
    {
        public static string ToWebSafe(this string value)
        {
            return value?.Replace("\r", "<br />").Trim() ?? "";
        }
    }
}
