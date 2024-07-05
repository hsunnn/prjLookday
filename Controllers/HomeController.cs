using Microsoft.AspNetCore.Mvc;
using prjLookday.Models;
using prjLookday.ViewModels;
using System.Diagnostics;
using System.Text.Json;
using Activity = System.Diagnostics.Activity;

namespace prjLookday.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOGIN_MEMBER))
                return View();
            return RedirectToAction("Login");
        }

        public IActionResult Privacy()
        {
            return View();
        }


        public IActionResult Login()
        {
            return View();
        }

        public IActionResult PowerBi()
        {
            return View();
        }

        public IActionResult Logout(CLoginViewModel vm)
        {
            HttpContext.Session.Remove(CDictionary.SK_LOGIN_MEMBER);
            return RedirectToAction("Login");
        }

        [HttpPost]
        public IActionResult Login(CLoginViewModel vm)
        {
            User user = (new lookdaysContext()).Users.FirstOrDefault(
                t => t.Username.Equals(vm.txtAccount) && t.Password.Equals(vm.txtPassword));
            if (user != null && user.Password.Equals(vm.txtPassword))
            {
                string json = JsonSerializer.Serialize(user);
                HttpContext.Session.SetString(CDictionary.SK_LOGIN_MEMBER, json);
                return RedirectToAction("Index");
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
