using AuthenticationSchemeDemo.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationSchemeDemo.Controllers
{
    public class AccountsController : Controller
    {
        private readonly IBasicAuthenticationService _authenticationService;
        private readonly BasicAuthenticationHandler _authenticationHandler;

        public AccountsController(BasicAuthenticationHandler authenticationHandler, 
                                    IBasicAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
            _authenticationHandler = authenticationHandler;
        }


        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Signup(string username, string password)
        {
            _authenticationService.StoreBasicCredentials(username, password);
            return Content("Registered");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<IActionResult> LoginAsync(string username, string password)
        {
            if (await _authenticationService.IsValidUserAsync(username, password))
            {
                return Content("You are not logged in " + username);
            }
            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            return null;
        }
    }
}