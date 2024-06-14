using Microsoft.AspNetCore.Mvc;
using prjLookday.Models;
using prjLookday.ViewModels;
using X.PagedList;
using System.Linq;
using System.Diagnostics;
using System;

namespace prjLookday.Controllers
{
    public class ActivityController : SuperController
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
            lookdaysContext db = new lookdaysContext();

            var query = from activity in db.Activities
                        join album in db.ActivitiesAlbums
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

        // GET: /Activity/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var activity = await _context.Activities.FindAsync(id);
            if (activity == null)
            {
                return NotFound();
            }
            var model = new CActivityAlbumViewModel
            {
                ActivityID = activity.ActivityId,
                Name = activity.Name,
                Description = activity.Description,
                Price = (decimal)activity.Price,
                Date = (DateOnly)activity.Date,
                CityID = (int)activity.ActivityId,
                Remaining = (int)activity.Remaining,
                HotelID = (int)activity.HotelId
            };
            return PartialView("_EditPartial", model);  // 改用 _EditPartial
        }

        // POST: /Activity/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(CActivityAlbumViewModel model)
        {
            if (ModelState.IsValid)
            {
                var activity = await _context.Activities.FindAsync(model.ActivityID);
                if (activity == null)
                {
                    return NotFound();
                }
                activity.Name = model.Name;
                activity.Description = model.Description;
                activity.Price = model.Price;
                activity.Date = model.Date;
                activity.CityId = model.CityID;
                activity.Remaining = model.Remaining;
                activity.HotelId = model.HotelID;

                _context.Update(activity);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return PartialView("_EditPartial", model);  // 改用 _EditPartial
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            if (id == 0)
            {
                return Json(new { success = false, message = "Invalid activity ID."});
            }

            using (lookdaysContext db = new lookdaysContext())
            {
                var activity = db.Activities.FirstOrDefault(a => a.ActivityId == id);
                if (activity == null)
                {
                    return Json(new { success = false, message = "Activity not found."});
                }

                var booking = db.Bookings.FirstOrDefault(b => b.ActivityId == id && b.BookingStatesId == 3);
                if (booking != null)
                {
                    return Json(new { success = false, message = "這個行程已有會員付款，暫時無法下架。" });
                }

                var albums = db.ActivitiesAlbums.Where(a => a.ActivityId == id).ToList();
                db.ActivitiesAlbums.RemoveRange(albums);//要避免SqlException: DELETE 陳述式與 REFERENCE 條件約束衝突

                db.Activities.Remove(activity);
                db.SaveChanges();

                return Json(new { success = true, message = "行程成功下架並移除圖片囉！" });
            }
        }
    }
}
 