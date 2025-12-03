using Labb2.DTOS;
using Labb2.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Labb2.Controllers;

[ApiController]
[Route("Album")]
public class AlbumController : ControllerBase {
    private readonly RepoContext _repoContext;
    private readonly ILogger<AlbumController> _logger;
    public AlbumController (ILogger<AlbumController> logger, RepoContext repoContext) {
        _logger = logger;
        _repoContext = repoContext;
    }
    [HttpGet("get/all")]
    public IActionResult GetAlbums () {
        var albums = _repoContext.Albums
            .Include(a => a.Artists)
            .Include(x => x.Songs);

        var result = new GetAllDto();

        foreach (var album in albums) {
            var getDto = CreateGetDto(album);
            result.Albums.Add(getDto);
        }

        return Ok(result);
    }

    [HttpGet("get")]
    public IActionResult GetAlbumByTitle (string title) {
        var album = _repoContext.Albums
            .Where(a => a.Title == title)
            .Include(a => a.Artists)
            .Include(a => a.Songs)
            .FirstOrDefault();
        if (album == null) return NotFound($"Album with title '{title}' not found.");

        var result = CreateGetDto(album);
        return Ok(result);
    }

    private GetDto CreateGetDto (Album album) {
        var result = new GetDto();

        result.Title = album.Title;
        result.ReleaseYear = album.ReleaseYear;
        result.Artists = album.Artists.Select(x => new ArtistDTO(x.Id, x.Name)).ToList();
        result.Songs = album.Songs.Select(x => new SongDTO(x.Id, x.Title, x.Duration)).ToList();
        return result;
    }

    public class GetAllDto {
        public List<GetDto> Albums { get; set; } = new List<GetDto>();
    }

    public class GetDto {
        public string Title { get; set; } = string.Empty;
        public int ReleaseYear { get; set; }
        public List<ArtistDTO> Artists { get; set; } = new List<ArtistDTO>();
        public List<SongDTO> Songs { get; set; } = new List<SongDTO>();
    }



    [HttpPost("create")]
    public IActionResult AddAlbum (int artistId, string albumTitle, int releaseYear) {
        var artist = _repoContext.Artists
            .Where(a => a.Id == artistId)
            .FirstOrDefault();
        if (artist == null) return NotFound($"Artist with id '{artistId}' not found.");
        var newAlbum = new Album {
            Title = albumTitle,
            ReleaseYear = releaseYear,
            Artists = new List<Artist> { artist }
        };
        _repoContext.Albums.Add(newAlbum);
        _repoContext.SaveChanges();
        return Ok(newAlbum.Id);
    }

    [HttpDelete("delete")]
    public IActionResult DeleteAlbum (int albumId) {
        var album = _repoContext.Albums
            .Where(a => a.Id == albumId)
            .FirstOrDefault();

        var result = new DeleteAlbumDto();
        if (album == null) {
            result.IsDeleted = true;
            result.Message = $"Album with ID '{albumId}' not found.";
            return Ok(result);
        }

        _repoContext.Remove(album);
        _repoContext.SaveChanges();

        result.IsDeleted = true;
        return Ok(result);
    }

    public class DeleteAlbumDto {
        public bool IsDeleted { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    [HttpPost("add/artist")]
    public IActionResult AddArtistToAlbum (int id, int artistId) {
        var album = _repoContext.Albums
            .Where(a => a.Id == id)
            .Include(a => a.Artists)
            .FirstOrDefault();
        if (album == null) return NotFound($"Album with ID '{id}' not found.");
        var artistToAdd = _repoContext.Artists
            .Where(a => a.Id == artistId)
            .FirstOrDefault();
        if (artistToAdd == null) return NotFound($"Artist with ID '{artistId}' not found.");


        var result = new ArtistAddedDto();
        if (album.Artists.Contains(artistToAdd)) {
            result.IsAdded = true;
            result.Message = $"Artist with ID '{artistId}' is already associated with album ID '{id}'.";
            return Ok(result);
        }

        album.Artists.Add(artistToAdd);
        _repoContext.SaveChanges();

        result.IsAdded = true;
        return Ok(result);
    }

    public class ArtistAddedDto {
        public bool IsAdded { get; set; }
        public string Message { get; set; } = string.Empty;
    }
    public class ArtistRemovedDto {
        public bool IsRemoved { get; set; }
        public string Message { get; set; } = string.Empty;
    }


    [HttpDelete("remove/artist")]
    public IActionResult RemoveArtistFromAlbum (int id, int artist) {
        var album = _repoContext.Albums
            .Where(a => a.Id == id)
            .Include(a => a.Artists)
            .FirstOrDefault();
        if (album == null) return NotFound($"Album with ID '{id}' not found.");
        var artistToRemove = _repoContext.Artists
            .Where(a => a.Id == artist)
            .FirstOrDefault();
        if (artistToRemove == null) return NotFound($"Artist with ID '{artist}' not found.");


        var result = new ArtistRemovedDto();

        if (!album.Artists.Contains(artistToRemove)) {
            result.IsRemoved = true;
            result.Message = $"Artist with ID '{artist}' is not associated with album ID '{id}'.";
            return Ok(result);
        }

        album.Artists.Remove(artistToRemove);
        _repoContext.SaveChanges();

        result.IsRemoved = true;
        return Ok(result);
    }

    [HttpPost("add/song")]
    public IActionResult AddSongToAlbum (int id, int songId) {
        var album = _repoContext.Albums
            .Where(a => a.Id == id)
            .Include(a => a.Songs)
            .FirstOrDefault();
        if (album == null) return NotFound($"Album with ID '{id}' not found.");
        var songToAdd = _repoContext.Songs
            .Where(s => s.Id == songId)
            .FirstOrDefault();
        if (songToAdd == null) return NotFound($"Song with ID '{songId}' not found.");

        var result = new AddSongDto();

        if (album.Songs.Contains(songToAdd)) {
            result.IsAdded = true;
            result.Message = $"Song with ID '{songId}' is already associated with album ID '{id}'.";
            return Conflict(result);
        }

        album.Songs.Add(songToAdd);
        _repoContext.SaveChanges();

        result.IsAdded = true;

        return Ok(result);
    }

    public class AddSongDto {
        public bool IsAdded { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class RemoveSongDto {
        public bool IsRemoved { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    [HttpDelete("remove/song")]
    public IActionResult RemoveSongFromAlbum (int id, int songId) {
        var album = _repoContext.Albums
            .Where(a => a.Id == id)
            .Include(a => a.Songs)
            .FirstOrDefault();
        if (album == null) return NotFound($"Album with ID '{id}' not found.");
        var songToRemove = _repoContext.Songs
            .Where(s => s.Id == songId)
            .FirstOrDefault();
        if (songToRemove == null) return NotFound($"Song with ID '{songId}' not found.");

        var result = new RemoveSongDto();
        if (!album.Songs.Contains(songToRemove)) {
            result.IsRemoved = true;
            result.Message = $"Song with ID '{songId}' is not associated with album ID '{id}'.";
            return Ok(result);
        }

        album.Songs.Remove(songToRemove);
        _repoContext.SaveChanges();

        result.IsRemoved = true;
        return Ok(result);
    }
}

