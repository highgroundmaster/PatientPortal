using System.Security.Cryptography;
using System.Text;

namespace PatientPortal.Helpers
{
    public static class UserPasswordHelper  
    {  
        public static string GenerateSalt(int maximumSaltLength) //length of salt    
        {
            var salt = new byte[maximumSaltLength];
            using (var random = new RNGCryptoServiceProvider())
            {
                random.GetNonZeroBytes(salt);
            }

            return Convert.ToBase64String(salt); ;
        }  
        public static string HashPassword(string pass, string salt) //hash password with SHA256
        {  
            byte[] saltBytes = Encoding.Unicode.GetBytes(salt);
            byte[] plainText = Encoding.Unicode.GetBytes(pass);
            byte[] hash = new byte[saltBytes.Length + plainText.Length];

            HashAlgorithm algorithm = HashAlgorithm.Create("SHA2");

            System.Buffer.BlockCopy(saltBytes, 0, hash, 0, saltBytes.Length);  
            System.Buffer.BlockCopy(plainText, 0, hash, saltBytes.Length, plainText.Length);
            return Convert.ToBase64String(algorithm.ComputeHash(hash));

        }  
        public static string HashPasswordMd5(string pass) //Encrypt using MD5    
        {  
            Byte[] originalBytes;  
            Byte[] encodedBytes;  
            MD5 md5;  
            //Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)    
            md5 = new MD5CryptoServiceProvider();  
            originalBytes = ASCIIEncoding.Default.GetBytes(pass);  
            encodedBytes = md5.ComputeHash(originalBytes);  
            //Convert encoded bytes back to a 'readable' string    
            return BitConverter.ToString(encodedBytes);  
        }  
        public static string base64Encode(string sData) // Encode byte password to Base64 String
        {  
            try  
            {  
                byte[] encData_byte = new byte[sData.Length];  
                encData_byte = System.Text.Encoding.UTF8.GetBytes(sData);  
                string encodedData = Convert.ToBase64String(encData_byte);  
                return encodedData;  
            }  
            catch (Exception ex)  
            {  
                throw new Exception("Error in base64Encode" + ex.Message);  
            }  
        }  
        public static string base64Decode(string sData) //Decode Base64 String to Bytes Passw
        {  
            try  
            {  
                var encoder = new System.Text.UTF8Encoding();  
                System.Text.Decoder utf8Decode = encoder.GetDecoder();  
                byte[] todecodeByte = Convert.FromBase64String(sData);  
                int charCount = utf8Decode.GetCharCount(todecodeByte, 0, todecodeByte.Length);  
                char[] decodedChar = new char[charCount];  
                utf8Decode.GetChars(todecodeByte, 0, todecodeByte.Length, decodedChar, 0);  
                string result = new String(decodedChar);  
                return result;  
            }  
            catch (Exception ex)  
            {  
                throw new Exception("Error in base64Decode" + ex.Message);  
            }  
        }  

    }

}  