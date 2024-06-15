using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using prjLookday.Models;
using prjLookday.ViewModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using X.PagedList;

namespace prjLookday.Controllers
{
    [Route("[controller]")]
    public class BookingListController : Controller
    {

        private readonly lookdaysContext _context;

        public BookingListController(lookdaysContext context)
        {
            _context = context;
        }

        [HttpGet("List")]
        public async Task<IActionResult> List(int? page)
        {
            int pageNum = page ?? 1;
            int pageSize = 10;

            var pages = await _context.Bookings
                .Include(x => x.User)
                .Include(x => x.Activity)
                .Include(x => x.BookingStates)
                .Include(x => x.Payments)
                .OrderBy(x => x.BookingId)
                .Select(b => new CBookingListViewModel
                {
                    BookingId = b.BookingId,
                    BookingDate = (DateTime)b.BookingDate,
                    UserName = b.User.Username,
                    ActivityName = b.Activity.Name,
                    Price = $"${b.Price:F2}",
                    bookingStatus = b.BookingStates.States,
                    member = (int)b.Member,
                })
                .ToPagedListAsync(pageNum, pageSize);

            return View(pages);



        }

    }

}

