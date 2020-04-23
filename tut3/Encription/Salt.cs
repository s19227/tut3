using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Text;

namespace tut3.Encription
{
    public static class Salt
    {
        public static bool Validate(string password, string salt, string hash) => Encript(password, salt).Equals(hash);

        public static string Encript(string password, string salt)
        {
            var valueBytes = KeyDerivation.Pbkdf2(
                                               password: password,
                                               salt: Encoding.UTF8.GetBytes(salt),
                                               prf: KeyDerivationPrf.HMACSHA512,
                                               iterationCount: 10000,
                                               numBytesRequested: 256 / 8);
            return Convert.ToBase64String(valueBytes);
        }
    }


}
