namespace Labb2.DTOS;

public class PlaylistDTO {
    public int Id { get; }
    public string Name { get; }
    public List<SongDTO> Songs { get; set; } = new();

    public PlaylistDTO (int id, string name) {
        Id = id;
        Name = name;
    }
}

public class SongDTO {
    public int Id { get; }
    public string Title { get; }
    public int Duration { get; }
    public SongDTO (int id, string title, int duration) {
        Id = id;
        Title = title;
        Duration = duration;
    }

}

public class ArtistDTO {
    public int Id { get; }
    public string Name { get; }
}

public class AlbumDTO {
    public int Id { get; }
    public string Title { get; }
    public int ReleaseYear { get; }

    public AlbumDTO (int id, string title, int releaseYear) {
        Id = id;
        Title = title;
        ReleaseYear = releaseYear;
    }
}


public class UserDTO {
    public string Username { get; }
    public int Id { get; }
    public UserDTO (string username, int id) {
        Username = username;
        Id = id;
    }
}