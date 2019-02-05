using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace AuthenticationSchemeDemo.Authentication
{
    public class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
    {
        readonly private IBasicAuthenticationService _authenticationService;

        public BasicAuthenticationHandler(
                IOptionsMonitor<BasicAuthenticationOptions> options,
                ILoggerFactory logger,
                UrlEncoder encoder,
                ISystemClock clock,
                IBasicAuthenticationService authenticaitonService)
                : base(options, logger, encoder, clock)
        {
            _authenticationService = authenticaitonService;
        }

        
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // return no result if authorization header is not in the request
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.NoResult();
            }
            // no result if the authentication header is not valid
            if (!AuthenticationHeaderValue.TryParse(Request.Headers["Authorization"], out AuthenticationHeaderValue headerValue))
            {
                return AuthenticateResult.NoResult();
            }
            // make sure scheme is "Basic" if not, then return no result
            if (!"Basic".Equals(headerValue.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                return AuthenticateResult.NoResult();
            }
            // decode the user name and password from the header value
            byte[] headerValueBytes = Convert.FromBase64String(headerValue.Parameter);
            string userAndPassword = Encoding.UTF8.GetString(headerValueBytes);
            // user name and password will be seperated by a colon
            string[] parts = userAndPassword.Split(":");
            if (parts.Length != 2)
            {
                return AuthenticateResult.Fail("Invalid Basic authentication header");
            }
            string user = parts[0];
            string password = parts[1];

            bool isValidUser = await _authenticationService.IsValidUserAsync(user, password);

            if (!isValidUser)
            {
                return AuthenticateResult.Fail("Invalid username or password");
            }
            // only claim is their username
            Claim[] claims = new Claim[] { new Claim(ClaimTypes.Name, user) };
            // create an identity with the username claim
            // Scheme is the authentication scheme, its a property of the authentication handler class
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            // create a principal with the user identity
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            // we need the authentication ticket to pass to the authenticationResult.Success method
            AuthenticationTicket ticket = new AuthenticationTicket(principal, Scheme.Name);
            return AuthenticateResult.Success(ticket);

        }

        /// <summary>
        /// Handle unauthenticated requests.
        /// If the user tries to navigate to an action method that is protected by the authorize attribute decorator
        /// without being authenticated, it will call this method.
        /// When the browser sees the WWW-Authenticate header in the 401 request it knows were using Basic http scheme,
        /// It will then show a pop up prompting the user to sign in.
        /// The browser then puts an Authorize header in the request to the server with the username and password joined with a colon
        /// and Base64 encoded.
        /// Then the handle authenticate method gets called.
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            // standard header the server is required to return when authentication is needed
            Response.Headers["WWW-Authenticate"] = $"Basic realm=\"{Options.Realm}\", charset=\"UTF-8\"";
            await base.HandleChallengeAsync(properties);
        }
    }
}
