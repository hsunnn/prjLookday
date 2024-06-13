namespace prjLookdayOrder.DTO
{
    public class BookingDTO
    {
       
        //很多參數打包再一起叫做DTO(Data Transfer Object)
        //WEB API 沒有 VIEW 所以寫成DTO 取代 VIEW MODEL 　
        public int BookingId { get; set; }
        //允許DATATIME可以是 null。也允許該屬性值為空
        public DateTime? BookingDate { get; set; } 
        public decimal Price { get; set; }

        public string userDisplay { get; set; }

        public int UserID { get; set; }

        //这行代码的含义是将 ActivityName 属性初始化为 null，但同时使用 !
        //操作符表示即使它被初始化为 null，也表明在运行时不会出现空引用异常。
        // public string UserName { get; set; } = null!;
        public string? UserName { get; set; }

        public string activityDisplay { get; set; }

        public int ActivityID { get; set; }
       // public int ActivityID { get; set; }
        //public string ActivityName { get; set; } = null!;
        public string? ActivityName { get; set; } 
        public string? bookingStatus { get; set; }
        public int? member { get; set; }
    }
}
