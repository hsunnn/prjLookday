using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using prjLookday.DTO;
using prjLookday.Models;
using prjLookdayOrder.DTO;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace prjLookday.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class filterController : ControllerBase
    {
        private readonly lookdaysContext _context;
        public filterController(lookdaysContext context)
        {
            _context = context;
        }

        // GET: api/<filterController>
        [HttpGet("activitydate")]
        public async Task<ActionResult<IEnumerable<BookingFilterDTO>>> Getfilterbookings([FromQuery]DateTime startDate, [FromQuery]DateTime endDate)
        {
            
            if (startDate == default || endDate == default || startDate>endDate)
            {
                return BadRequest("無效的日期區間");
            }
            endDate = endDate.AddDays(1);
            var bookings = await _context.Bookings
                .Include(x => x.User)
                .Include(x => x.Activity)
                .Include(x => x.BookingStates)
                .Include(x => x.Payments)
                 .Where(b => b.BookingDate >= startDate && b.BookingDate <= endDate) 
                .Select(b => new BookingDTO
                {                  
                    BookingId = b.BookingId,
                    BookingDate = b.BookingDate,  //(DateTime)(b.BookingDate)
                    Price = b.Price,
                    userDisplay = b.User.Username,                 
                    activityDisplay = b.Activity.Name,                
                    bookingStatus = b.BookingStates.States.ToString(),               
                    member = b.Member,
                })
              
                .ToListAsync();

                return Ok(bookings);
        }

        [HttpGet("search")]
        public async Task<ActionResult<BookingFilterDTO>> searchbookings(string keyword)
        {
            if(keyword == default)
            {
                return BadRequest("請輸入關鍵字");
            }
            var searchbookings = await _context.Bookings
                .Include(x => x.User)
                .Include(x => x.Activity)
                .Include(x => x.BookingStates)
                .Include(x => x.Payments)
                 .Where(b => b.User.Username.Contains("keyword")||b.Activity.Name.Contains("keyword"))
                .Select(b => new BookingDTO
                {
                    BookingId = b.BookingId,
                    BookingDate = b.BookingDate,  //(DateTime)(b.BookingDate)
                    Price = b.Price,
                    userDisplay = b.User.Username,
                    activityDisplay = b.Activity.Name,
                    bookingStatus = b.BookingStates.States.ToString(),
                    member = b.Member,
                })
                .ToListAsync();

            return Ok(searchbookings);
        }
    }
}

