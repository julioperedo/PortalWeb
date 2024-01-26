namespace PiggyBankService.Misc
{
    public class Common
    {
        public static string GetErrorMessage(Exception ex)
        {
            string message = ex.Message;
            var e1 = ex.InnerException;
            while (e1 != null)
            {
                message += Environment.NewLine + e1.Message;
                e1 = e1.InnerException;
            }
            return message;
        }
    }
}
