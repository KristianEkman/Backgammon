using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Dto.account
{
    public enum LocalAccountStatus
    {
        Success,
        InvalidLogin, //Username or password is wrong.
        EmailExists        
    }
}
