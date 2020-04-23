using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tut3.Models
{
    public class RefreshTokenRequest
    {
        public RefreshTokenRequest(string refreshToken, string login)
        {
            RefreshToken = refreshToken;
            Login = login;
        }

        public string RefreshToken { get; set; }
        public string Login { get; set; }
    }
}
