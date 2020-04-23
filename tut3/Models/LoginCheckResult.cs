using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tut3.Models
{
    public class LoginCheckResult
    {
        public LoginCheckResult(List<string> roles, string refreshToken)
        {
            Roles = roles;
            this.RefreshToken = refreshToken;
        }

        public List<string> Roles { get; set; }
        public string RefreshToken { get; set; }
    }
}
