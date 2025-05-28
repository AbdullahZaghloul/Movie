namespace Movies.Models.ViewModels
{
    public class ResetPasswordVM
    {
        public int OTP { get; set; }
        public string ApplicationUserId { get; set; }
        public string Token { get; set; }

        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

    }
}
