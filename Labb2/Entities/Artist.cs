namespace Labb2.Entities;
public class Artist {
    public int Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<Album> Albums { get; set; }
    public IEnumerable<Song> Songs { get; set; }
}

public class Song {

    public int Id { get; set; }
    public string Title { get; set; }
    public int Duration { get; set; }
    public int AlbumId { get; set; }

    public IEnumerable<Artist> Artists { get; set; }
}

public class Album {

    public int Id { get; set; }
    public string Title { get; set; }
    public int ReleaseYear { get; set; }
    public IEnumerable<Song> Songs { get; set; }
    public IEnumerable<Artist> Artists { get; set; }

}


public class User {
}

public class Playlist {
}