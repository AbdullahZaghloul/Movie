using Movies.Models;

public class Actor
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Bio { get; set; }
    public string ProfilePicture { get; set; }
    public string News { get; set; }

    // ❌ REMOVE this:
    // public List<Movie> Movies { get; set; }

    // ✅ KEEP THIS:
    public List<ActorMovie> ActorMovies { get; set; }
}
