using System.ComponentModel.DataAnnotations;

namespace Labb2.Entities;

public class User {
    [Key]
    public int Id { get; set; }
    public string Username { get; set; }

    public ICollection<Playlist> Playlists { get; set; }

    public User () {

    }
}
