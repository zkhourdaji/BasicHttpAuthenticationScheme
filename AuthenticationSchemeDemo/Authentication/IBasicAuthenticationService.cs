using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationSchemeDemo.Authentication
{
    // will be injected into the authentication handler by DI
    public interface IBasicAuthenticationService
    {
        Task<bool> IsValidUserAsync(string user, string password);

        bool StoreBasicCredentials(string username, string password);
        bool StoreBasicCredentials(string base64UserNameAndPassword);

    }

}
