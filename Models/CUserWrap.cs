namespace prjLookday.Models
{
    public class CUserWrap
    {
        private User _user;

        public CUserWrap()
        {
            if (_user == null)
                _user = new User();
        }

        public User user
        {
            get { return _user; }
            set { _user = value; }
        }

        public int UserId
        {
            get { return _user.UserId; } 
            set { _user.UserId = value; }
        }

        public string UserName
        {
            get { return _user.Username; }
            set { _user.Username = value;}
        }

        public string Email
        {
            get { return _user.Email; }
            set { _user.Email = value; }
        }

        public string Password
        { 
            get { return _user.Password; } 
            set { _user.Password = value; } 
        }

        public int? Preferences
        { 
            get { return _user.Preferences; }
            set { _user.Preferences = value; } 
        }

        public int RoleId
        {
            get { return _user.RoleId; }
            set { _user.RoleId = value; }
        }

        public byte[]? Userpic
        {
            get { return _user.UserPic; }
            set { _user.UserPic = value; }
        }

        public string? FPhone { get; set; }

        public IFormFile userpic { get; set; }
    }

}
