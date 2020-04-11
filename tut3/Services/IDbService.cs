using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tut3.Services
{
    public interface IDbService
    {
        bool ExistsIndexNumber(string index);
    }
}
