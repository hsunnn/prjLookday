﻿using Microsoft.AspNetCore.Mvc;
using prjLookday.Models;
using prjLookday.ViewModels;
using System.Security.Permissions;

namespace prjLookday.Controllers
{
    public class UserController : SuperController
    {
        private IWebHostEnvironment _enviro = null;

        public UserController(IWebHostEnvironment enviro)
        {
            _enviro = enviro;
        }

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
        public IActionResult Create(CUserWrap userIn)
        {
            lookdaysContext db = new lookdaysContext();

            User userDb = new User();

            if (userIn.userpic != null)
            {
                string picName = Guid.NewGuid().ToString() + ".jpg";
                userDb.UserPic = picName;
                userIn.userpic.CopyTo(new FileStream(_enviro.WebRootPath + "/Images/" + picName, FileMode.Create));

            }

            userDb.Username = userIn.UserName;
            userDb.Email = userIn.Email;
            userDb.Password = userIn.Password;
            userDb.RoleId = userIn.RoleId;
            //userDb.UserPic = userIn.UserPic;


            db.Users.Add(userDb);
            db.SaveChanges();
            return RedirectToAction("List");
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
                    string picName = Guid.NewGuid().ToString() + ".jpg";
                    userDb.UserPic = picName;
                    userIn.userpic.CopyTo(new FileStream(_enviro.WebRootPath + "/Images/" + picName, FileMode.Create));

                }

                userDb.Username = userIn.UserName;
                userDb.Email = userIn.Email;
                userDb.Password = userIn.Password;
                userDb.RoleId = userIn.RoleId;
                //userDb.UserPic = userIn.userpic;

                db.SaveChanges();
            }

            return RedirectToAction("List");

        }



        public IActionResult Delete(int id)
        {
            if (id != null)
            {
                lookdaysContext db = new lookdaysContext();
                User u = db.Users.FirstOrDefault(x => x.UserId == id);
                if (u != null)
                {
                    db.Users.Remove(u);
                    db.SaveChanges();
                }

            }

            return RedirectToAction("List");
        }

    }

}
