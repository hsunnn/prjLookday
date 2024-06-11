using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingFilterDTO>>> Getfilterbookings(DateTime? BookingDate, DateOnly? ActivityDate)
        {

            if (!BookingDate.HasValue || !ActivityDate.HasValue) //|| BookingDate > ActivityDate
            {
                return BadRequest("查無此日期區間資料");
            }

            var bookings = await _context.Bookings
                .Include(x => x.User)
                .Include(x => x.Activity)
                .Include(x => x.BookingStates)
                .Include(x => x.Payments)
                 .Where(b => b.BookingDate >= BookingDate) //&& b.BookingDate <= ActivityDate
                .Select(b => new BookingFilterDTO
                {
                    BookingId = b.BookingId,
                    BookingDate = b.BookingDate,
                   //ActivityName = b.Activity.ActivityName,
                   //ActivityDate = b.Activity.Date,   //b.Activity.Date
                    bookingStatus = b.BookingStates.ToString()
                })
                .ToListAsync();
            return Ok(bookings);
        }

         //return new string[] { "value1", "value2" };
    }

    // GET api/<filterController>/5
    //[HttpGet("{id}")]
    //public string Get(int id)
    //{
    //    return "value";
    //}

    // POST api/<filterController>
    //[HttpPost]
    //public void Post([FromBody] string value)
    //{
    //}

    // PUT api/<filterController>/5
    //[HttpPut("{id}")]
    //public void Put(int id, [FromBody] string value)
    //{
    //}

    // DELETE api/<filterController>/5
    //[HttpDelete("{id}")]
    //public void Delete(int id)
    //{
    //}
}

