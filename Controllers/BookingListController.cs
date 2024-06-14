using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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


        [HttpGet("SimpleList")]
        public async Task<IActionResult> SimpleList()
        {
            try
            {
                var bookings = await _context.Bookings.Take(10).Select(b => new
                {
                    b.BookingId,
                    b.BookingDate
                }).ToListAsync();
                return View(bookings);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception Message:{ex.Message}");
                Console.WriteLine($"Stack Trace:{ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.GetType().FullName}");
                    Console.WriteLine($"Inner Exception Message: {ex.InnerException.Message}");
                    Console.WriteLine($"Inner Exception Stack Trace: {ex.InnerException.StackTrace}");
                }
                return StatusCode(500, "Internal server error");
            }
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
                        Price = $"${b.Price:F2}",
                        bookingStatus = b.BookingStates.States,
                        member = (int)b.Member,
                    })
                    .ToListAsync();

                return View(bookings);
            }
      
        }

    }

