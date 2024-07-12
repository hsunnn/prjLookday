using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjLookday.Controllers;
using prjLookday.DTO;
using prjLookday.Models;
using prjLookdayOrder.DTO;

namespace prjLookdayOrder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingOrderController : SuperController
    {
        private readonly lookdaysContext _context;

        public BookingOrderController(lookdaysContext context)
        {
            _context = context;
        }

        // GET: api/BookingOrder
        // 獲取所有訂單
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingDTO>>> GetBookings()
        {
            //return await _context.Bookings.ToListAsync();
            var bookings = await _context.Bookings
            .Include(x => x.User)
            .Include(x => x.Activity)           
            .Include(x => x.BookingStates)
            .Include(x=>x.Payments)
        .Select(b => new BookingDTO
        {
            BookingId = b.BookingId,
            BookingDate = b.BookingDate, 
            userDisplay =b.User.Username,  
            activityDisplay = b.Activity.Name, 
            Price = b.Price,
            bookingStatus = b.BookingStates.States.ToString(),
            member = b.Member,
        })

        .ToListAsync();
            return Ok(bookings);
        }

        // GET: api/BookingOrder/5
        // 根據ID獲取特定訂單
        [HttpGet("ById/{id}")]
        public async Task<ActionResult<BookingDTO>> GetBooking(int id)
        {
 
            var booking = await _context.Bookings
                .Include(x=>x.User)
                .Include(x=>x.Activity)
                .Include(x=>x.BookingStates)
                .Include(x=>x.Payments)
                .Where(b => b.BookingId == id ) 
                .Select(b=> new BookingDTO
                {
                    BookingId = b.BookingId,
                    BookingDate = b.BookingDate, 
                    Price = b.Price,
                    
                    userDisplay = b.User.Username,
                    activityDisplay = b.Activity.Name,
                    bookingStatus= b.BookingStates.States.ToString(),
                    member = b.Member,
                })
             .FirstOrDefaultAsync();  

            if (booking == null)
            {
                return NotFound();
            }
            return Ok(booking);
        }

        [HttpGet("ByUsername/{username}")]
        public async Task<ActionResult<BookingDTO>> GetBookingName(string username)
        {
            var booking = await _context.Bookings
                .Include(x => x.User)
                .Include(x => x.Activity)
                .Include(x => x.BookingStates)
                .Include(x => x.Payments)
                .Where(b => EF.Functions.Like(b.User.Username, $"%{username}%"))
                .Select(b => new BookingDTO
                {
                    BookingId = b.BookingId,
                    BookingDate = b.BookingDate,  //(DateTime)(b.BookingDate)
                    Price = b.Price,
                    userDisplay = b.User.Username,
                    activityDisplay= b.Activity.Name,
                    bookingStatus = b.BookingStates.States.ToString(),
                    member = b.Member,
                })
            .ToListAsync();

            if (booking == null || !booking.Any())
            {
                return NotFound();
            }

            return Ok(booking);
        }

        [HttpPut("{userId}/{bookingId}")]
        public async Task<IActionResult> PutBooking([FromRoute] int userId, [FromRoute] int bookingId, [FromBody] BookingDTO bookingDTO)
        {
            if (bookingDTO.UserID != userId || bookingDTO.BookingId != bookingId)
            {
                bookingDTO.UserID = userId;
                bookingDTO.BookingId = bookingId;
            }

            var booking = await _context.Bookings
                .Include(x => x.User)
                .Include(x => x.Activity)
                .Include(x => x.BookingStates)
                .Include(x => x.Payments)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId && b.UserId == userId);

            if (booking == null)
            {
                return NotFound();
            }

            // 更新預訂資料
            booking.BookingDate = bookingDTO.BookingDate;
            booking.Price = bookingDTO.Price;
            booking.UserId = bookingDTO.UserID;
            // 不能直接修改 User 的 Username，需要處理這部分邏輯
            booking.ActivityId = bookingDTO.ActivityID;
            // 不能直接修改 Activity 的 ActivityName，需要處理這部分邏輯
            booking.Member = bookingDTO.member;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!BookingExists(bookingId))
                {
                    return NotFound();
                }
                else
                {
                    return StatusCode(500, "Concurrency error occurred: " + ex.Message);
                }
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, "Database error occurred: " + ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }

            return NoContent();
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(b => b.BookingId == id);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookingExistss(int id)
        {
            return _context.Bookings.Any(e => e.BookingId == id);
        }
    }
}
