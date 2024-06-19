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
        private readonly LookdaysContext _context;

        public BookingOrderController(LookdaysContext context)
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
            //.Include 和資料表有直接的關聯
            .Include(x => x.BookingStates)
            .Include(x=>x.Payments)
            //.ThenInclude 有兩層的關聯
            //.ThenInclude(x=>x.)
        .Select(b => new BookingDTO
        {
            BookingId = b.BookingId,
            BookingDate = b.BookingDate,  //(DateTime)(b.BookingDate)
            //UserName= b.User.Username,
            userDisplay =b.User.Username,  //$"編號:{b.UserId}|姓名:{b.User.Username}"
            activityDisplay = b.Activity.Name,  //$"編號:{b.ActivityId}{b.Activity.Name}"
            Price = b.Price,
            bookingStatus = b.BookingStates.States.ToString(),
            member = b.Member,
            //remaining = $"{b.Member}/{b.Activity.Remaining}",

            //PaymentDate = b.Payments.ToString(),
        })
        //用于异步地从可查询的数据源中检索所有元素并将它们转换为 List<T>。在需要执行数据库操作而不阻塞主线程的情况下，它特别有用，从而提高应用程序的性能和响应速度。
        //ToListAsync 通常与 await 一起使用
        .ToListAsync();
            return Ok(bookings);
        }

        // GET: api/BookingOrder/5
        // 根據ID獲取特定訂單
        [HttpGet("ById/{id}")]
        public async Task<ActionResult<BookingDTO>> GetBooking(int id)
        {
            //FindAsync 是用于异步检索数据源中实体的方法
            //var booking = await _context.Bookings.FindAsync(id);

            var booking = await _context.Bookings
                .Include(x=>x.User)
                .Include(x=>x.Activity)
                .Include(x=>x.BookingStates)
                .Include(x=>x.Payments)
                .Where(b => b.BookingId == id ) 
                .Select(b=> new BookingDTO
                {
                    BookingId = b.BookingId,
                    BookingDate = b.BookingDate,  //(DateTime)(b.BookingDate)
                    Price = b.Price,
                    
                    //activityDisplay = b.Activity.Name,
                    //UserID = b.UserId,
                    userDisplay = b.User.Username,
                    //ActivityID = b.ActivityId,
                    activityDisplay = b.Activity.Name,
                    bookingStatus= b.BookingStates.States.ToString(),
                    member = b.Member,
                    //remaining = $"{b.Member}/{b.Activity.Remaining}"
                })
             // 使用FirstOrDefaultAsync獲取單筆資料而不是ToListAsync獲取列表
             .FirstOrDefaultAsync();  

            if (booking == null)
            {
                return NotFound();
            }

            return Ok(booking);
        }
        //透過用戶姓名搜尋
        [HttpGet("ByUsername/{username}")]
        public async Task<ActionResult<BookingDTO>> GetBookingName(string username)
        {
            //FindAsync 是用于异步检索数据源中实体的方法
            //var booking = await _context.Bookings.FindAsync(id);

            var booking = await _context.Bookings
                .Include(x => x.User)
                .Include(x => x.Activity)
                .Include(x => x.BookingStates)
                .Include(x => x.Payments)
                .Where(b => b.User.Username == username)
                .Select(b => new BookingDTO
                {
                    BookingId = b.BookingId,
                    BookingDate = b.BookingDate,  //(DateTime)(b.BookingDate)
                    Price = b.Price,
                    userDisplay = b.User.Username,
                    //UserID = b.UserId,
                    //UserName = b.User.Username,
                    activityDisplay= b.Activity.Name,
                    //ActivityID = b.ActivityId,
                    //ActivityName = b.Activity.Name,
                    bookingStatus = b.BookingStates.States.ToString(),
                    //remaining = $"{b.Member}/{b.Activity.Remaining}"
                    member = b.Member,
                })
            //通过指定条件查找第一个符合条件的元素，如果没有找到符合条件的元素，则返回默认值（例如 null）
            //.FirstOrDefaultAsync();
            .ToListAsync();

            if (booking == null || !booking.Any())
            {
                return NotFound();
            }

            return Ok(booking);
        }


        // PUT: api/BookingOrder/5    // 更新特定ID的訂單
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        [HttpPut("{userId}/{bookingId}")]
        public async Task<IActionResult> PutBooking([FromRoute] int userId, [FromRoute] int bookingId, [FromBody] BookingDTO bookingDTO)
        {
            //if (id != bookingDTO.BookingId)
            //{
            //    //BadRequest 是 HTTP 状态码 400，表示客户端发送的请求有错误，服务器无法理解或处理
            //    return BadRequest("沒有此筆訂單資料");
            //}

            // 檢查 userId 和 bookingId
            if (bookingDTO.UserID != userId || bookingDTO.BookingId != bookingId)
            {
                // 如果不相同，將 bookingDTO 的對應屬性設置為路由參數
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
            // booking.User.Username = bookingDTO.UserName;
            booking.ActivityId = bookingDTO.ActivityID;
            // 不能直接修改 Activity 的 ActivityName，需要處理這部分邏輯
            // booking.Activity.ActivityName = bookingDTO.ActivityName;
            // booking.BookingStatesId = int.Parse(bookingDTO.bookingStatus);
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

        // POST: api/BookingOrder   // 創建新的訂單
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<IEnumerable<BookingDTO>>> PostBooking([FromBody] BookingFilterDTO filter)     //[FromBody] 不能有key
        //{
        //    // 初始化查詢，包括相關的實體
        //    var query = _context.Bookings
        // .Include(x => x.User)           // 包括與預訂相關的 User 實體
        // .Include(x => x.Activity)       // 包括與預訂相關的 Activity 實體
        // .Include(x => x.BookingStates)  // 包括與預訂相關的 BookingStates 實體
        // .Include(x => x.Payments)       // 包括與預訂相關的 Payments 實體
        // .AsQueryable();                 // 轉換為 IQueryable 以便進一步過濾


        //    // 如果 StartDate 過濾條件有值，則添加條件從該日期開始過濾預訂
        //    if (filter.BookingDate.HasValue)
        //    {
        //        //"大於或等於"
        //        query = query.Where(b => b.BookingDate >= filter.BookingDate.Value);
        //    }
        //    // 如果 ActivityDate 過濾條件有值，則添加條件到該日期結束過濾預訂
        //    if (filter.ActivityDate.HasValue)
        //    {
        //        //"小於或等於"
        //       // query = query.Where(b => b.BookingDate <= filter.ActivityDate.Value);
        //    }


        //    // 將過濾後的預訂投影為 BookingDTO 對象並異步執行查詢
        //    var bookings = await query
        //        .Select(b => new BookingDTO
        //        {
        //            BookingId = b.BookingId,                         // 映射 BookingId
        //            BookingDate = b.BookingDate,                     // 映射 BookingDate  (DateTime)(b.BookingDate)
        //            Price = b.Price,                                 // 映射 Price
        //            UserID = b.UserId,                               // 映射 UserID
        //            UserName = b.User.Username,                      // 映射 UserName
        //            ActivityID = b.ActivityId,                       // 映射 ActivityID
        //            ActivityName = b.Activity.Name,                  // 映射 ActivityName
        //            bookingStatus = b.BookingStatesId.ToString(),    // 映射 bookingStatus
        //            member = b.Member,                               // 映射 member
        //            // PaymentDate = b.Payments.ToString(),
        //        })
        //        .ToListAsync();    // 異步檢索 BookingDTO 列表                              
                                   
        //    return Ok(bookings);   // 返回包含預訂列表的 OK 響應
        //}

        // DELETE: api/BookingOrder/5  // 刪除特定ID的訂單
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

        // 檢查特定ID的訂單是否存在
        private bool BookingExistss(int id)
        {
            return _context.Bookings.Any(e => e.BookingId == id);
        }
    }
}
