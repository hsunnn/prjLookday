﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using prjLookday.Models;
using prjLookday.ViewModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using X.PagedList;
using X.PagedList.Mvc.Core;

namespace prjLookday.Controllers
{
    [Route("[controller]")]
    public class BookingListController : SuperController
    {

        private readonly LookdaysContext _context;

        public BookingListController(LookdaysContext context)
        {
            _context = context;
        }

        [HttpGet("List")]
        public async Task<IActionResult> List(int page=1)
        {
            int pageSize =20;

            var pages = await _context.Bookings
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
                .OrderBy(x => x.BookingId)
                .Where(b => b.bookingStatus == "已完成付款")
                .ToPagedListAsync(page, pageSize);

            return View(pages);
        }

    }

}
