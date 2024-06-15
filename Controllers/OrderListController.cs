using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjLookday.Models;
using prjLookday.ViewModels;
using prjLookdayOrder.DTO;

namespace prjLookday.Controllers
{
    public class OrderListController : Controller
    {
        private readonly lookdaysContext _context;

        public OrderListController(lookdaysContext context)
        {
            _context = context;
        }
        
            //[HttpGet]
        public IActionResult OrderList(COrderListViewModel vm) //ublic async Task<ActionResult<IEnumerable<BookingDTO>>> GetBookings()
       {
            lookdaysContext db = new lookdaysContext();

            Booking bookingdb = new Booking();

            var bookings = db.Bookings
                .Include(x => x.User)
                .Include(x => x.Activity)
                .Include(x =>x.BookingStates)
                .Include(x => x.Payments)
                .Select(b => new BookingDTO
                { 
                    BookingId = b.BookingId,
                    BookingDate = b.BookingDate,
                    UserName = b.User.Username,
                    ActivityName = b.Activity.Name,
                    Price = b.Price,
                    bookingStatus = b.BookingStates.States,
                    member = b.Member,
                }
                
                .
       
        //用于异步地从可查询的数据源中检索所有元素并将它们转换为 List<T>。在需要执行数据库操作而不阻塞主线程的情况下，它特别有用，从而提高应用程序的性能和响应速度。
        //ToListAsync 通常与 await 一起使用
        .ToListAsync();
            //return Ok(bookings);

        }

        return View();


    }
    }

