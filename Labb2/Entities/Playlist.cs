using System.ComponentModel.DataAnnotations;

namespace Labb2.Entities;

public class Playlist {
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    public ICollection<Song> Songs { get; set; }

    public Playlist () {

    }
}