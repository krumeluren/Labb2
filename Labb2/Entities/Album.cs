using System.ComponentModel.DataAnnotations;

namespace Labb2.Entities;

public class Album {
    [Key]
    public int Id { get; set; }
    public string Title { get; set; }
    public int ReleaseYear { get; set; }

    public ICollection<Song> Songs { get; set; }
    public ICollection<Artist> Artists { get; set; } // a album can have collaborators

    public Album () {

    }
}
