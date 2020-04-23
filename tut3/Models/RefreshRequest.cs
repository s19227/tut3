using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tut3.Models
{
    public class RefreshRequest
    {
        public string oldJWT { get; set; }
        public string refreshToken { get; set; }
    }
}
