using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwetterBook.Domain;

namespace TwetterBook.Services
{
    public interface IIdentityService
    {
        Task<AuthinticationResult> CreateAsync(string email, string passWord);
        Task<AuthinticationResult> LoginAsync(string email, string passWord);
    }
}
