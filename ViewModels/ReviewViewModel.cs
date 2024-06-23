namespace prjLookday.ViewModels
{
    public class ReviewViewModel
    {

        public int ReviewId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string Comment { get; set; }
        public double Rating { get; set; }

        public byte[] UserPic { get; set; }
    }
}
