using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjLookday.Models;
using prjLookday.ViewModels;

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
