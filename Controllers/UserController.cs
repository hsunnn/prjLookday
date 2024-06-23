using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            lookdaysContext db = new lookdaysContext();

            IEnumerable<User> datas;
            if (string.IsNullOrEmpty(vm.txtKeyword))
                datas = from user in db.Users select user;
            else
                datas = db.Users.Where(r => r.Username.Contains(vm.txtKeyword) || r.Email.Contains(vm.txtKeyword));

            int pageSize = 10; // 每頁顯示的筆數
            int pageNumber = page ?? 1;
            IPagedList<User> pagedList = datas.ToPagedList(pageNumber, pageSize);

            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = pagedList.PageCount;

            return View(pagedList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CUserWrap userIn)
        {
            lookdaysContext db = new lookdaysContext();

            User userDb = new User();

            if (userIn.userpic != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    userIn.userpic.CopyTo(memoryStream);
                    userDb.UserPic = memoryStream.ToArray();
                }
            }

            userDb.Username = userIn.UserName;
            userDb.Email = userIn.Email;
            userDb.Password = HashPassword(userIn.Password);
            userDb.RoleId = userIn.RoleId;
            userDb.FPhone = userIn.FPhone;

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
            lookdaysContext db = new lookdaysContext();
            User u = db.Users.FirstOrDefault(x => x.UserId == id);

            if (u == null)
                return RedirectToAction("List");
            return View(u);
        }

        [HttpPost]
        public IActionResult Edit(CUserWrap userIn)
        {
            lookdaysContext db = new lookdaysContext();
            User userDb = db.Users.FirstOrDefault(x => x.UserId == userIn.UserId);
            if (userDb != null)
            {
                if (userIn.userpic != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        userIn.userpic.CopyTo(memoryStream);
                        userDb.UserPic = memoryStream.ToArray();
                    }
                }

                userDb.Username = userIn.UserName;
                userDb.Email = userIn.Email;
                userDb.Password = userIn.Password;
                userDb.RoleId = userIn.RoleId;
                userDb.FPhone = userIn.FPhone;

                db.SaveChanges();
            }

            return RedirectToAction("List");
        }

        public IActionResult Delete(int id)
        {
            lookdaysContext db = new lookdaysContext();
            var user = db.Users.Include(u => u.Bookings).FirstOrDefault(u => u.UserId == id);
            if (user != null)
            {
                if (user.Bookings.Any())
                {
                    return Json(new { success = false, message = "此會員已有購買紀錄，無法刪除。" });
                }
                else
                {
                    db.Users.Remove(user);
                    db.SaveChanges();
                    return Json(new { success = true, message = "會員成功刪除！" });
                }
            }

            return Json(new { success = false, message = "會員不存在。" });
        }
    }
}
