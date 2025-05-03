using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Movies.Models;

public class Movie
{
    public int Id { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    [ValidateNever]
    public List<string> ImgUrl { get; set; } = [];
    [ValidateNever]
    public string TrailerUrl { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public MovieStatus MovieStatus { get; set; }

    public int CinemaId { get; set; }
    public int CategoryId { get; set; }
    [ValidateNever]
    public Cinema Cinema { get; set; }
    [ValidateNever]
    public Category Category { get; set; }

    // ❌ REMOVE this:
    // public List<Actor> Actors { get; set; }

    // ✅ KEEP THIS:
    [ValidateNever]
    public List<ActorMovie> ActorMovies { get; set; }
}
