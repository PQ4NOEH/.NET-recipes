using AlteaLabs.Core.Guards;
using System.Security.Cryptography;
using System.Text;

namespace AlteaLabs.Core.Security
{
    public class HashGenerator : IHashGenerator
    {
        public NotNulllEmptyOrWhiteSpaceString GenerateHash(NotNulllEmptyOrWhiteSpaceString data)
        {
            var dataBytes = Encoding.UTF8.GetBytes(data);
            using (var shaGenerator = new SHA1CryptoServiceProvider())
            {
                return Encoding.UTF8.GetString(shaGenerator.ComputeHash(dataBytes));
            }
        }
    }
}
