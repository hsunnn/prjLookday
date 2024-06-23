using Microsoft.AspNetCore.Mvc;
using prjLookday.Models;
using prjLookday.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace prjLookday.Controllers
{
    public class ReviewController : SuperController
    {

        private readonly IWebHostEnvironment _enviro;
        private readonly lookdaysContext _context;

        public ReviewController(lookdaysContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> List(CKeywordViewModel vm, int? page, string ratingFilter)
        {
            var query = from activity in _context.Activities
                        join album in _context.ActivitiesAlbums
                        on activity.ActivityId equals album.ActivityId into activityAlbumGroup
                        from activityAlbum in activityAlbumGroup.DefaultIfEmpty()
                        join review in _context.Reviews
                        on activity.ActivityId equals review.ActivityId into reviewGroup
                        group new { activity, activityAlbum, reviewGroup } by new
                        {
                            activity.ActivityId,
                            activity.Name,
                            activity.Description
                        } into activityGroup
                        select new CAvgReviewViewModel
                        {
                            ActivityID = activityGroup.Key.ActivityId,
                            Name = activityGroup.Key.Name,
                            Description = activityGroup.Key.Description,
                            Photo = activityGroup.Select(g => g.activityAlbum.Photo).FirstOrDefault(),
                            AverageRating = (double)(activityGroup.SelectMany(g => g.reviewGroup).Any() ?
                                            activityGroup.SelectMany(g => g.reviewGroup).Average(r => r.Rating) : 0)
                        };

            if (!string.IsNullOrEmpty(vm.txtKeyword))
            {
                query = query.Where(r => r.Name.Contains(vm.txtKeyword) ||
                                         r.Description.Contains(vm.txtKeyword));
            }

            if (!string.IsNullOrEmpty(ratingFilter))
            {
                if (ratingFilter == "4andAbove")
                {
                    query = query.Where(r => r.AverageRating >= 4).OrderByDescending(r => r.AverageRating);
                }
                else if (ratingFilter == "0to3_9")
                {
                    query = query.Where(r => r.AverageRating < 4).OrderByDescending(r => r.AverageRating);
                }
            }
            else
            {
                query = query.OrderByDescending(r => r.AverageRating);
            }

            int pageSize = 10;
            int pageNumber = page ?? 1;
            var pagedList = await query.ToPagedListAsync(pageNumber, pageSize);

            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = pagedList.PageCount;
            ViewBag.txtKeyword = vm.txtKeyword;
            ViewBag.RatingFilter = ratingFilter;

            return View(pagedList);
        }


        [HttpGet]
        public async Task<IActionResult> GetReviews(int id)
        {
            var reviews = await _context.Reviews
                .Where(r => r.ActivityId == id)
                .Select(r => new ReviewViewModel
                {
                    UserName = r.User.Username,
                    UserEmail = r.User.Email,
                    Comment = r.Comment,
                    Rating = r.Rating.HasValue ? r.Rating.Value : 0,
                    UserPic = r.User.UserPic
                })
                .ToListAsync();

            return PartialView("_ReviewPartialView", reviews);
        }


        [HttpPost]
        public async Task<IActionResult> DeleteReviews([FromBody] List<int> reviewIds)
        {
            if (reviewIds == null || !reviewIds.Any())
            {
                return Json(new { success = false, message = "沒有選中的評論。" });
            }

            var reviews = await _context.Reviews.Where(r => reviewIds.Contains(r.ReviewId)).ToListAsync();
            _context.Reviews.RemoveRange(reviews);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "選中的評論已刪除。" });
        }

    }
}
