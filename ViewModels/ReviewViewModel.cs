namespace prjLookday.ViewModels
{
    public class ReviewViewModel
    {
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }

        public byte[] UserPic { get; set; }
    }
}
