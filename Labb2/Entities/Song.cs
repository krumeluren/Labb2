using System.ComponentModel.DataAnnotations;

namespace Labb2.Entities;

public class Song {
    [Key]
    public int Id { get; set; }
    public string Title { get; set; }

    // seconds 
    public int Duration { get; set; }

    public int AlbumId { get; set; }
    public Album Album { get; set; }

    public ICollection<Artist> Artists { get; set; } // a song can have collaborators
    public ICollection<Playlist> Playlists { get; set; }

    public Song () {

    }
}
