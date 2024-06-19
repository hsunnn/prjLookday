﻿using Microsoft.AspNetCore.Mvc;
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
        private readonly LookdaysContext _context;
        public filterController(LookdaysContext context)
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

            var bookings = await _context.Bookings
                .Include(x => x.User)
                .Include(x => x.Activity)
                .Include(x => x.BookingStates)
                .Include(x => x.Payments)
                 .Where(b => b.BookingDate >= startDate && b.BookingDate <= endDate) 
                .Select(b => new BookingDTO
                {
                    //BookingId = b.BookingId,
                    //BookingDate = b.BookingDate,
                    //ActivityName = b.Activity.Name,
                    //ActivityDate = (DateOnly)b.Activity.Date,   //b.Activity.Date
                    //bookingStatus = b.BookingStates.States
                    BookingId = b.BookingId,
                    BookingDate = b.BookingDate,  //(DateTime)(b.BookingDate)
                    Price = b.Price,
                    userDisplay = b.User.Username,
                    //UserID = b.UserId,
                    //UserName = b.User.Username,
                    activityDisplay = b.Activity.Name,
                    //ActivityID = b.ActivityId,
                    //ActivityName = b.Activity.Name,
                    bookingStatus = b.BookingStates.States.ToString(),
                    //remaining = $"{b.Member}/{b.Activity.Remaining}"
                    member = b.Member,
                })
              
                .ToListAsync();

                return Ok(bookings);
        }
    }



    // GET: api/<filterController>
    ////[HttpGet("activitydate/{activitystart}")]
    ////    public async Task<ActionResult<IEnumerable<BookingFilterDTO>>> Getfilterbookings(DateTime BookingDate, DateOnly ActivityDate)
    ////    {
    ////        //DateTime dateTime = DateTime.Now;

    ////        // 將DateTime轉換為DateTimeOffset
    ////        DateTimeOffset dateTimeOffset = new DateTimeOffset(BookingDate);

    ////        // 獲取Unix時間戳 ToUnixTimeSeconds() 方法，該方法返回從 1970 年 1 月 1 日的午夜（UTC）到指定時間的秒數。
    ////        long unixTimeSeconds = dateTimeOffset.ToUnixTimeSeconds();
    ////        //-------------
    ////        // 假設有一個DateOnly對象
    ////        //DateOnly dateOnly = DateOnly.FromDateTime(DateTime.Now);

    ////        // 將DateOnly轉換為DateTime（假設時間為午夜）//ToDateTime(TimeOnly.MinValue) 方法將 ActivityDate 與指定的時間部分（這裡是午夜）組合成完整的 DateTime 物件。
    ////        DateTime dateTime = ActivityDate.ToDateTime(TimeOnly.MinValue);

    ////        // 將DateTime轉換為DateTimeOffset
    ////        DateTimeOffset dateTimeOffset2 = new DateTimeOffset(dateTime, TimeSpan.Zero);

    ////        // 獲取Unix時間戳
    ////        long unixTimeSeconds2 = dateTimeOffset.ToUnixTimeSeconds();

    ////        //---------------------------------------------------------------------------

    ////        if (string.IsNullOrEmpty(BookingDate.ToString()) || string.IsNullOrEmpty(ActivityDate.ToString()) || unixTimeSeconds < unixTimeSeconds2)
    ////        {
    ////            return BadRequest("查無此日期區間資料");
    ////        }

    ////        var bookings = await _context.Bookings
    ////            .Include(x => x.User)
    ////            .Include(x => x.Activity)
    ////            .Include(x => x.BookingStates)
    ////            .Include(x => x.Payments)
    ////             .Where(b => b.BookingDate >= BookingDate && unixTimeSeconds <= unixTimeSeconds2) //&& b.BookingDate <= ActivityDate
    ////            .Select(b => new BookingFilterDTO
    ////            {
    ////                BookingId = b.BookingId,
    ////                BookingDate = b.BookingDate,
    ////                ActivityName = b.Activity.Name,
    ////                ActivityDate = (DateOnly)b.Activity.Date,   //b.Activity.Date
    ////                bookingStatus = b.BookingStates.States
    ////            })
    ////            .ToListAsync();
    ////        return Ok(bookings);
    ////    }

    ////    // return new string[] { "value1", "value2" };
    ////}
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
