using Microsoft.AspNetCore.Mvc;
using prjLookday.Models;
using prjLookday.ViewModels;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using X.PagedList;

namespace prjLookday.Controllers
{
    public class UserController : SuperController
    {
        private readonly IWebHostEnvironment _enviro;

        public UserController(IWebHostEnvironment enviro)
        {
            _enviro = enviro;
        }

        public IActionResult List(CKeywordViewModel vm, int? page)
        {
            LookdaysContext db = new LookdaysContext();

            IEnumerable<User> datas;
            if (string.IsNullOrEmpty(vm.txtKeyword))
                datas = from user in db.Users select user;
            else
                datas = db.Users.Where(r => r.Username.Contains(vm.txtKeyword) || r.Email.Contains(vm.txtKeyword));

            int pageSize = 10; // 每頁顯示的記錄數
            int pageNumber = page ?? 1; // 當前頁碼，默認為第1頁
            IPagedList<User> pagedList = datas.ToPagedList(pageNumber, pageSize);

            return View(pagedList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CUserWrap userIn)
        {
            LookdaysContext db = new LookdaysContext();

            User userDb = new User();

            if (userIn.userpic != null)
            {
                string picName = Guid.NewGuid().ToString() + ".jpg";
                //userDb.UserPic = picName;
                userIn.userpic.CopyTo(new FileStream(_enviro.WebRootPath + "/Images/" + picName, FileMode.Create));
            }

            userDb.Username = userIn.UserName;
            userDb.Email = userIn.Email;
            userDb.Password = HashPassword(userIn.Password);
            userDb.RoleId = userIn.RoleId;

            db.Users.Add(userDb);
            db.SaveChanges();
            return RedirectToAction("List");
        }

        private string HashPassword(string pwd)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(pwd));
                var hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                return hash;
            }
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
                return RedirectToAction("List");
            LookdaysContext db = new LookdaysContext();
            User u = db.Users.FirstOrDefault(x => x.UserId == id);

            if (u == null)
                return RedirectToAction("List");
            return View(u);
        }

        [HttpPost]
        public IActionResult Edit(CUserWrap userIn)
        {
            LookdaysContext db = new LookdaysContext();
            User userDb = db.Users.FirstOrDefault(x => x.UserId == userIn.UserId);
            if (userDb != null)
            {
                if (userIn.userpic != null)
                {
                    string picName = Guid.NewGuid().ToString() + ".jpg";
                //    userDb.UserPic = picName;
                    userIn.userpic.CopyTo(new FileStream(_enviro.WebRootPath + "/Images/" + picName, FileMode.Create));
                }

                userDb.Username = userIn.UserName;
                userDb.Email = userIn.Email;
                userDb.Password = userIn.Password;
                userDb.RoleId = userIn.RoleId;

                db.SaveChanges();
            }

            return RedirectToAction("List");
        }

        public IActionResult Delete(int id)
        {
            LookdaysContext db = new LookdaysContext();
            User u = db.Users.FirstOrDefault(x => x.UserId == id);
            if (u != null)
            {
                db.Users.Remove(u);
                db.SaveChanges();
            }

            return RedirectToAction("List");
        }
    }
}
