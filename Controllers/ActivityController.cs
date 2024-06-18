﻿using Microsoft.AspNetCore.Mvc;
using prjLookday.Models;
using prjLookday.ViewModels;
using System.Threading.Tasks;
using System.IO;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace prjLookday.Controllers
{
    public class ActivityController : Controller
    {
        private readonly IWebHostEnvironment _enviro;
        private readonly lookdaysContext _context;

        public ActivityController(IWebHostEnvironment enviro, lookdaysContext context)
        {
            _enviro = enviro;
            _context = context;
        }

        public IActionResult List(CKeywordViewModel vm, int? page)
        {
            var query = from activity in _context.Activities
                        join album in _context.ActivitiesAlbums
                        on activity.ActivityId equals album.ActivityId into activityAlbumGroup
                        from activityAlbum in activityAlbumGroup.DefaultIfEmpty()
                        select new CActivityAlbumViewModel
                        {
                            PhotoID = activityAlbum != null ? (int?)activityAlbum.PhotoId : null,
                            Photo = activityAlbum != null ? activityAlbum.Photo : null,
                            ActivityID = activity.ActivityId,
                            Name = activity.Name,
                            Description = activity.Description,
                            Price = (decimal)activity.Price,
                            Date = (DateOnly)activity.Date,
                            CityID = (int)activity.CityId,
                            Remaining = (int)activity.Remaining,
                            HotelID = (int)activity.HotelId
                        };

            if (!string.IsNullOrEmpty(vm.txtKeyword))
            {
                query = query.Where(r => r.Name.Contains(vm.txtKeyword) ||
                                         r.Description.Contains(vm.txtKeyword));
            }

            int pageSize = 10;
            int pageNumber = page ?? 1;
            var pagedList = query.ToPagedList(pageNumber, pageSize);

            return View(pagedList);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var activity = await _context.Activities.FindAsync(id);
            if (activity == null)
            {
                return NotFound();
            }

            var activityAlbum = await _context.ActivitiesAlbums.FirstOrDefaultAsync(a => a.ActivityId == id);
            var model = new CActivityAlbumViewModel
            {
                ActivityID = activity.ActivityId,
                Name = activity.Name,
                Description = activity.Description,
                Price = (decimal)activity.Price,
                Date = (DateOnly)activity.Date,
                CityID = (int)activity.CityId,
                Remaining = (int)activity.Remaining,
                HotelID = (int)activity.HotelId,
                Photo = activityAlbum?.Photo
            };
            return PartialView("_EditPartial", model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CActivityAlbumViewModel model)
        {
            if (ModelState.IsValid)
            {
                var activity = await _context.Activities.FindAsync(model.ActivityID);
                if (activity == null)
                {
                    return Json(new { success = false, message = "Activity not found." });
                }

                activity.Name = model.Name;
                activity.Description = model.Description;
                activity.Price = model.Price;
                activity.Date = model.Date;
                activity.CityId = model.CityID;
                activity.Remaining = model.Remaining;
                activity.HotelId = model.HotelID;

                if (model.PhotoFile != null && model.PhotoFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await model.PhotoFile.CopyToAsync(memoryStream);
                        var photo = new ActivitiesAlbum
                        {
                            ActivityId = model.ActivityID,
                            Photo = memoryStream.ToArray()
                        };

                        var existingPhoto = await _context.ActivitiesAlbums.FirstOrDefaultAsync(a => a.ActivityId == model.ActivityID);

                        if (existingPhoto != null)
                        {
                            existingPhoto.Photo = photo.Photo;
                            _context.Update(existingPhoto);
                        }
                        else
                        {
                            _context.Add(photo);
                        }
                    }
                }

                _context.Update(activity);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "活動成功更新囉!" });
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return Json(new { success = false, message = "請填寫所有必填欄位.", errors = errors });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            if (id == 0)
            {
                return Json(new { success = false, message = "Invalid activity ID." });
            }

            var activity = _context.Activities.FirstOrDefault(a => a.ActivityId == id);
            if (activity == null)
            {
                return Json(new { success = false, message = "Activity not found." });
            }

            var booking = _context.Bookings.FirstOrDefault(b => b.ActivityId == id && b.BookingStatesId == 3);
            if (booking != null)
            {
                return Json(new { success = false, message = "這個行程已有會員付款，暫時無法下架。" });
            }

            var albums = _context.ActivitiesAlbums.Where(a => a.ActivityId == id).ToList();
            _context.ActivitiesAlbums.RemoveRange(albums);

            _context.Activities.Remove(activity);
            _context.SaveChanges();

            return Json(new { success = true, message = "行程成功下架並移除圖片囉！" });
        }

        [HttpGet]
        public IActionResult Create()
        {
            return PartialView("_CreatePartial");
        }

        [HttpPost]
        public async Task<IActionResult> Create(CActivityAlbumViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var activity = new Activity
                {
                    Name = vm.Name,
                    Description = vm.Description,
                    Price = vm.Price,
                    Date = vm.Date,
                    CityId = vm.CityID,
                    Remaining = vm.Remaining,
                    HotelId = vm.HotelID
                };

                _context.Activities.Add(activity);
                await _context.SaveChangesAsync();

                if (vm.PhotoFile != null && vm.PhotoFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await vm.PhotoFile.CopyToAsync(memoryStream);
                        var photo = new ActivitiesAlbum
                        {
                            ActivityId = activity.ActivityId,
                            Photo = memoryStream.ToArray()
                        };
                        _context.ActivitiesAlbums.Add(photo);
                    }
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "活動成功新增囉!" });
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return Json(new { success = false, message = "請填寫所有必填欄位.", errors = errors });
        }
    }
}
