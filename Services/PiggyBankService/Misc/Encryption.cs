using System.Security.Cryptography;
using System.Text;

namespace PiggyBankService.Misc
{
    public class Encryption
    {
        private const string crypyKey = "P2#5o*gH";

        public static string Encrypt(string textToEncrypt)
        {
            DES des;
            byte[] inputByteArray;
            MemoryStream ms;
            CryptoStream cs;
            byte[] mIV = { 0x45, 0x32, 0xa5, 0x18, 0x67, 0x58, 0xac, 0xba };

            string strEncrypted;
            try
            {
                byte[] mkey = Encoding.UTF8.GetBytes(crypyKey[..8]);
                des = DES.Create();
                inputByteArray = Encoding.UTF8.GetBytes(textToEncrypt);
                ms = new MemoryStream();
                cs = new CryptoStream(ms, des.CreateEncryptor(mkey, mIV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                strEncrypted = Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception)
            {
                strEncrypted = "";
            }
            return strEncrypted;
        }

        public static string Decrypt(string textToDecrypt)
        {
            DES des;
            MemoryStream ms;
            CryptoStream cs;
            Encoding encoding;
            byte[] mIV = { 0x45, 0x32, 0xa5, 0x18, 0x67, 0x58, 0xac, 0xba };

            string strDecrypted;
            try
            {
                byte[] mkey = Encoding.UTF8.GetBytes(crypyKey[..8]);
                des = DES.Create();
                byte[] inputByteArray = Convert.FromBase64String(textToDecrypt);
                ms = new MemoryStream();
                cs = new CryptoStream(ms, des.CreateDecryptor(mkey, mIV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                encoding = Encoding.UTF8;
                strDecrypted = encoding.GetString(ms.ToArray());
            }
            catch (Exception)
            {
                strDecrypted = "";
            }
            return strDecrypted;
        }
    }
}
