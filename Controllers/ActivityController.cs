using Microsoft.AspNetCore.Mvc;
using prjLookday.Models;
using prjLookday.ViewModels;
using X.PagedList;
using System.Linq;
using System.Diagnostics;

namespace prjLookday.Controllers
{
    public class ActivityController : SuperController
    {
        private readonly IWebHostEnvironment _enviro;

        public ActivityController(IWebHostEnvironment enviro)
        {
            _enviro = enviro;
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
    }
}
 