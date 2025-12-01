using System.ComponentModel.DataAnnotations;

namespace Labb2.Entities;
public class Artist {
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }

    public ICollection<Album> Albums { get; set; }
    public ICollection<Song> Songs { get; set; }

    public Artist () {

    }
}

public class Song {
    [Key]
    public int Id { get; set; }
    public string Title { get; set; }

    // seconds 
    public int Duration { get; set; }

    public int AlbumId { get; set; }
    public Album Album { get; set; }

    public ICollection<Artist> Artists { get; set; }
    public ICollection<Playlist> Playlists { get; set; }

    public Song () {

    }
}

public class Album {
    [Key]
    public int Id { get; set; }
    public string Title { get; set; }
    public int ReleaseYear { get; set; }

    public ICollection<Song> Songs { get; set; }
    public ICollection<Artist> Artists { get; set; }

    public Album () {

    }
}

public class User {
    [Key]
    public int Id { get; set; }
    public string Username { get; set; }

    public ICollection<Playlist> Playlists { get; set; }

    public User () {

    }
}

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