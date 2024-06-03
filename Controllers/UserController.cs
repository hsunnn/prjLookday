using Microsoft.AspNetCore.Mvc;
using prjLookday.Models;
using prjLookday.ViewModels;

namespace prjLookday.Controllers
{
    public class UserController : SuperController
    {

        public IActionResult List(CKeywordViewModel vm)
        {
            lookdaysContext db = new lookdaysContext();

            IEnumerable<User> datas = null;
            if (string.IsNullOrEmpty(vm.txtKeyword))
                datas = from user in db.Users
                        select user;
            else
                datas = db.Users.Where(r => r.Username.Contains(vm.txtKeyword) ||
                    r.Email.Contains(vm.txtKeyword));
            return View(datas);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(User u)
        {
            lookdaysContext db = new lookdaysContext();
            db.Users.Add(u);
            db.SaveChanges();
            return RedirectToAction("List");
        }


        public ActionResult Edit(int? id) 
        {
            if (id == null)
                return RedirectToAction("List");
            lookdaysContext db = new lookdaysContext();
            User u = db.Users.FirstOrDefault(x => x.UserId == id);

            if (u == null)
                return RedirectToAction("List");
            return View(u);
        }
    }
}
