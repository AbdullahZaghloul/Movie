namespace Movies.Models.ViewModels
{
    public class MovieWithCategoryWithCinemaVM2
    {
        public Movie Movie { get; set; }
        public List<Category> Categories { get; set; }
        public List<Cinema> Cinemas { get; set; }

        public List<IFormFile> ImageFiles { get; set; }
        public IFormFile Trailer { get; set; }
    }
}
