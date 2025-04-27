namespace Movies.Models.ViewModels
{
    public class MovieWithCategoriesWithCinemasVM
    {
        public List<Movie> Movies { get; set; }
        public List<Category> Categories { get; set; }
        public List<Cinema> Cinemas { get; set; }

        public int CategoryId { get; set; }
        public int CinemaId { get; set; }

        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }

        public int TotalNumOfPages { get; set; }
    }
}
