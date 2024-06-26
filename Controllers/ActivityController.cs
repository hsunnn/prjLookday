using Microsoft.AspNetCore.Mvc;
using prjLookday.Models;
using prjLookday.ViewModels;
using System.Threading.Tasks;
using System.IO;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

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

        public IActionResult List(CKeywordViewModel vm, int? page, DateTime? startDate, DateTime? endDate)
        {
            var query = from activity in _context.Activities
                        join city in _context.Cities on activity.CityId equals city.CityId
                        join album in _context.ActivitiesAlbums
                        on activity.ActivityId equals album.ActivityId into activityAlbumGroup
                        from activityAlbum in activityAlbumGroup.DefaultIfEmpty()
                        group new { activity, activityAlbum } by new
                        {
                            activity.ActivityId,
                            activity.Name,
                            activity.Description,
                            activity.Price,
                            activity.Date,
                            activity.CityId,
                            activity.City.CityName,
                            activity.Remaining,
                            activity.HotelId
                        } into activityGroup
                        select new CActivityAlbumViewModel
                        {
                            ActivityID = activityGroup.Key.ActivityId,
                            Name = activityGroup.Key.Name,
                            Description = activityGroup.Key.Description,
                            Price = (decimal)activityGroup.Key.Price,
                            Date = (DateOnly)activityGroup.Key.Date,
                            CityID = (int)activityGroup.Key.CityId,
                            CityName = activityGroup.Select(a => a.activity.City.CityName).FirstOrDefault(),
                            Remaining = (int)activityGroup.Key.Remaining,
                            HotelID = (int)activityGroup.Key.HotelId,
                            Photo = activityGroup.Select(g => g.activityAlbum.Photo).FirstOrDefault(),
                            PhotoDesc = activityGroup.Select(g => g.activityAlbum.PhotoDesc).FirstOrDefault()
                        };

            if (!string.IsNullOrEmpty(vm.txtKeyword))
            {
                query = query.Where(r => r.Name.Contains(vm.txtKeyword) ||
                                         r.Description.Contains(vm.txtKeyword));
            }

            if (startDate.HasValue)
            {
                query = query.Where(r => r.Date >= DateOnly.FromDateTime(startDate.Value));
            }

            if (endDate.HasValue)
            {
                query = query.Where(r => r.Date <= DateOnly.FromDateTime(endDate.Value));
            }

            int pageSize = 10;
            int pageNumber = page ?? 1;
            var pagedList = query.ToPagedList(pageNumber, pageSize);

            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = pagedList.PageCount;

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
                Photo = activityAlbum?.Photo,
                PhotoDesc = activityAlbum?.PhotoDesc
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

                var existingPhoto = await _context.ActivitiesAlbums.FirstOrDefaultAsync(a => a.ActivityId == model.ActivityID);

                if (model.PhotoFile != null && model.PhotoFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await model.PhotoFile.CopyToAsync(memoryStream);
                        if (existingPhoto != null)
                        {
                            existingPhoto.Photo = memoryStream.ToArray();
                        }
                        else
                        {
                            var photo = new ActivitiesAlbum
                            {
                                ActivityId = model.ActivityID,
                                Photo = memoryStream.ToArray(),
                                PhotoDesc = model.PhotoDesc
                            };
                            _context.Add(photo);
                        }
                    }
                }

                if (existingPhoto != null)
                {
                    existingPhoto.PhotoDesc = model.PhotoDesc;
                    _context.Update(existingPhoto);
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
            return PartialView("_CreatePartial", new CActivityAlbumViewModel());
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
                            Photo = memoryStream.ToArray(),
                            PhotoDesc = vm.PhotoDesc
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


        [HttpGet]
        public async Task<IActionResult> GetPhotos(int id)
        {
            var photos = await _context.ActivitiesAlbums
                .Where(a => a.ActivityId == id)
                .Select(a => new { a.Photo, a.PhotoDesc })
                .ToListAsync();

            return PartialView("_PhotoGalleryPartial", photos);
        }


        [HttpPost]
        public async Task<IActionResult> UploadPhoto(int activityId, IFormFile newPhoto, string newPhotoDesc)
        {
            if (newPhoto != null && newPhoto.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await newPhoto.CopyToAsync(memoryStream);
                    var photo = new ActivitiesAlbum
                    {
                        ActivityId = activityId,
                        Photo = memoryStream.ToArray(),
                        PhotoDesc = newPhotoDesc
                    };

                    _context.ActivitiesAlbums.Add(photo);
                    await _context.SaveChangesAsync();
                }
            }

            var photos = await _context.ActivitiesAlbums
                                       .Where(a => a.ActivityId == activityId)
                                       .Select(a => new { a.Photo, a.PhotoDesc })
                                       .ToListAsync();

            return PartialView("_PhotoGalleryPartial", photos);
        }

        [HttpGet]
        public async Task<IActionResult> GetActivityModels(int id)
        {
            var activityModels = await _context.ActivitiesModels
                .Where(m => m.ActivityId == id)
                .Select(m => new CActivityModelViewModel
                {
                    ModelId = m.ModelId,
                    ModelName = m.ModelName,
                    ModelPrice = m.ModelPrice,
                    ModelDate = m.ModelDate,
                    ModelContent = m.ModelContent,
                    ActivityId = id // 傳遞 ActivityId
                })
                .ToListAsync();

            ViewBag.ActivityId = id; // 設置 ViewBag.ActivityId
            return PartialView("_ActivityModelsPartial", activityModels);
        }


        [HttpPost]
        public async Task<IActionResult> AddActivityModel(CActivityModelViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newModel = new ActivitiesModel
                {
                    ActivityId = model.ActivityId,
                    ModelName = model.ModelName,
                    ModelPrice = model.ModelPrice,
                    ModelDate = model.ModelDate,
                    ModelContent = model.ModelContent
                };

                _context.ActivitiesModels.Add(newModel);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "活動 Model 新增成功。" });
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return Json(new { success = false, message = "請填寫所有必填欄位.", errors = errors });
        }
    }
}
