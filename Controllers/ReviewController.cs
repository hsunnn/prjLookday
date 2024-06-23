using Microsoft.AspNetCore.Mvc;
using prjLookday.Models;

namespace prjLookday.Controllers
{
    public class ReviewController : SuperController
    {

        private readonly IWebHostEnvironment _enviro;
        private readonly lookdaysContext _context;

        public IActionResult List()
        {
            return View();
        }
    }
}
