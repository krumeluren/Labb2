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
        var album = _repoContext.Albums
            .Include(a => a.Artists)
            .Include(x => x.Songs);
        return Ok(album);
    }

    [HttpGet("get")]
    public IActionResult GetAlbumByTitle (string title) {
        var album = _repoContext.Albums
            .Where(a => a.Title == title)
            .Include(a => a.Artists)
            .Include(a => a.Songs)
            .FirstOrDefault();
        if (album == null) return NotFound($"Album with title '{title}' not found.");
        return Ok(album);
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
        if (album.Artists.Contains(artistToAdd)) {
            return Conflict($"Artist with ID '{artistId}' is already associated with album ID '{id}'.");
        }
        album.Artists.Add(artistToAdd);
        _repoContext.SaveChanges();
        return Ok($"Artist with ID '{artistId}' added to album ID '{id}'.");
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
        if (!album.Artists.Contains(artistToRemove)) {
            return NotFound($"Artist with ID '{artist}' is not associated with album ID '{id}'.");
        }
        album.Artists.Remove(artistToRemove);
        _repoContext.SaveChanges();
        return Ok($"Artist with ID '{artist}' removed from album ID '{id}'.");
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
        if (album.Songs.Contains(songToAdd)) {
            return Conflict($"Song with ID '{songId}' is already associated with album ID '{id}'.");
        }
        album.Songs.Add(songToAdd);
        _repoContext.SaveChanges();
        return Ok($"Song with ID '{songId}' added to album ID '{id}'.");
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
        if (!album.Songs.Contains(songToRemove)) {
            return NotFound($"Song with ID '{songId}' is not associated with album ID '{id}'.");
        }
        album.Songs.Remove(songToRemove);
        _repoContext.SaveChanges();
        return Ok($"Song with ID '{songId}' removed from album ID '{id}'.");
    }
}

