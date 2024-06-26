using Microsoft.AspNetCore.Mvc;

namespace prjLookday.Controllers
{
    public class ChatViewController : Controller
    {
        public IActionResult ChatView()
        {
            return View();
        }
    }
}
