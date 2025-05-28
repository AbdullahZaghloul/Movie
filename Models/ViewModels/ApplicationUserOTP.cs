namespace Movies.Models.ViewModels
{
    public class ApplicationUserOTP
    {
        public int Id { get; set; }
        public int OTP { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public DateTime RealseDate { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}
