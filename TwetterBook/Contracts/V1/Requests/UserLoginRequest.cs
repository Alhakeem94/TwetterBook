using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwetterBook.Contracts.V1.Requests
{
    public class UserLoginRequest
    {
        public string Email { get; set; }
        public string PassWord { get; set; }

    }
}
