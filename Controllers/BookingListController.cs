using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjLookday.Models;
using prjLookday.ViewModels;

namespace prjLookday.Controllers
{
    public class BookingListController : Controller
    {
        [Route("[controller]")]
        public class BookingOrderController : Controller
        {
            private readonly lookdaysContext _context;

            public BookingOrderController(lookdaysContext context)
            {
                _context = context;
            }

            public async Task<IActionResult> List()
            {
                var bookings = await _context.Bookings
                    .Include(x => x.User)
                    .Include(x => x.Activity)
                    .Include(x => x.BookingStates)
                    .Include(x => x.Payments)
                    .Select(b => new CBookingListViewModel
                    {
                        BookingId = b.BookingId,
                        BookingDate = (DateTime)b.BookingDate,
                        UserName = b.User.Username,
                        ActivityName = b.Activity.Name,
                        Price = b.Price,
                        bookingStatus = b.BookingStates.States,
                        member = (int)b.Member,
                    })
                    .ToListAsync();

                return View(bookings);
            }
        }
    }
}
