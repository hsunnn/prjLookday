namespace prjLookday.ViewModels
{
    public class CActivityAlbumViewModel
    {
        public int? PhotoID { get; set; }
        public byte[] Photo { get; set; }  
        public int ActivityID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string PhotoDesc {  get; set; }
        public decimal Price { get; set; }
        public DateOnly Date { get; set; }
        public string CityName { get; set; }
        public int CityID { get; set; }
        public int Remaining { get; set; }
        public int HotelID { get; set; }

        public int PriceInt => (int)Price;

        public IFormFile PhotoFile { get; set; }
    }
}
