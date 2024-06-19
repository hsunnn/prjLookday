using Microsoft.AspNetCore.Mvc;

namespace prjLookday.Controllers
{
    public class ReviewController : SuperController
    {
        public IActionResult List()
        {
            return View();
        }
    }
}
