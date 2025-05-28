using Movies.Models;

public class MovieWithCategoryWithCinemaVM2
{
    public Movie Movie { get; set; } = new Movie();
    public List<Category> Categories { get; set; } = new List<Category>();
    public List<Cinema> Cinemas { get; set; } = new List<Cinema>();
    public List<Actor> Actors { get; set; } = new List<Actor>();
    public List<int> SelectedActorIds { get; set; } = new List<int>();
    public List<IFormFile> ImageFiles { get; set; } = new List<IFormFile>();
    public IFormFile TrailerFile { get; set; }
}