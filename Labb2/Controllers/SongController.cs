using Labb2.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Labb2.Controllers;

[ApiController]
[Route("Song")]
public class SongController : ControllerBase {
    private readonly RepoContext _repoContext;
    private readonly ILogger<SongController> _logger;
    public SongController (ILogger<SongController> logger, RepoContext repoContext) {
        _logger = logger;
        _repoContext = repoContext;
    }
    [HttpGet("get")]
    public IActionResult GetSongByTitle (string title) {
        var song = _repoContext.Songs
            .Where(s => s.Title == title)
            .Include(s => s.Artists)
            .Include(s => s.Album)
            .FirstOrDefault();
        if (song == null) return NotFound($"Song with title '{title}' not found.");
        return Ok(song);
    }

    [HttpPost("create")]
    public IActionResult AddSong (string title, int duration, int albumId) {
        var album = _repoContext.Albums
            .Where(a => a.Id == albumId)
            .FirstOrDefault();
        if (album == null) return NotFound($"Album with ID '{albumId}' not found.");
        var newSong = new Song {
            Title = title,
            Duration = duration,
            AlbumId = albumId
        };
        _repoContext.Songs.Add(newSong);
        _repoContext.SaveChanges();
        return Ok(newSong);
    }
    [HttpPost("delete")]
    public IActionResult DeleteSong (int id) {
        var song = _repoContext.Songs
            .Where(s => s.Id == id)
            .FirstOrDefault();
        if (song == null) return NotFound($"Song with ID '{id}' not found.");
        _repoContext.Songs.Remove(song);
        _repoContext.SaveChanges();
        return Ok($"Song with ID '{id}' deleted successfully.");
    }

    [HttpPost("update")]
    public IActionResult UpdateSong (int id, string? title, int? duration, int? albumId) {
        var song = _repoContext.Songs
            .Where(s => s.Id == id)
            .FirstOrDefault();
        if (song == null) return NotFound($"Song with ID '{id}' not found.");
        if (title != null) song.Title = title;
        if (duration != null) song.Duration = duration.Value;
        if (albumId != null) {
            var album = _repoContext.Albums
                .Where(a => a.Id == albumId)
                .FirstOrDefault();
            if (album == null) return NotFound($"Album with ID '{albumId}' not found.");
            song.AlbumId = albumId.Value;
        }
        _repoContext.SaveChanges();
        return Ok(song);
    }

    [HttpPost("add/artist")]
    public IActionResult AddArtistToSong (int id, int artistId) {
        var song = _repoContext.Songs
            .Where(s => s.Id == id)
            .Include(s => s.Artists)
            .FirstOrDefault();
        if (song == null) return NotFound($"Song with ID '{id}' not found.");
        var artistToAdd = _repoContext.Artists
            .Where(a => a.Id == artistId)
            .FirstOrDefault();
        if (artistToAdd == null) return NotFound($"Artist with ID '{artistId}' not found.");
        if (song.Artists.Contains(artistToAdd)) {
            return Conflict($"Artist with ID '{artistId}' is already associated with song ID '{id}'.");
        }
        song.Artists.Add(artistToAdd);
        _repoContext.SaveChanges();
        return Ok($"Artist with ID '{artistId}' added to song ID '{id}'.");
    }


    [HttpDelete("remove/artist")]
    public IActionResult RemoveArtistFromSong (int id, int artistId) {
        var song = _repoContext.Songs
            .Where(s => s.Id == id)
            .Include(s => s.Artists)
            .FirstOrDefault();
        if (song == null) return NotFound($"Song with ID '{id}' not found.");
        var artistToRemove = _repoContext.Artists
            .Where(a => a.Id == artistId)
            .FirstOrDefault();
        if (artistToRemove == null) return NotFound($"Artist with ID '{artistId}' not found.");
        if (!song.Artists.Contains(artistToRemove)) {
            return NotFound($"Artist with ID '{artistId}' is not associated with song ID '{id}'.");
        }
        song.Artists.Remove(artistToRemove);
        _repoContext.SaveChanges();
        return Ok($"Artist with ID '{artistId}' removed from song ID '{id}'.");
    }
}