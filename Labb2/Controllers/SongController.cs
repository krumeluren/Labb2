using Labb2.DTOS;
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

        var response = new SongDTO(song.Id, song.Title, song.Duration);
        return Ok(response);
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

        var response = new SongCreateDTO(newSong.Id);
        return Ok(response);
    }

    public class SongCreateDTO {
        public int Id { get; set; }
        public SongCreateDTO (int id) {
            Id = id;
        }
    }

    [HttpPost("delete")]
    public IActionResult DeleteSong (int id) {
        var song = _repoContext.Songs
            .Where(s => s.Id == id)
            .FirstOrDefault();

        var response = new SongDeletedDTO();

        if (song == null) {
            response.IsDeleted = true;
            return Ok(response);
        }

        _repoContext.Songs.Remove(song);
        _repoContext.SaveChanges();

        response.IsDeleted = true;
        return Ok(response);
    }

    public class SongDeletedDTO {
        public bool IsDeleted { get; set; }
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

        return Ok(new SongDTO(song.Id, song.Title, song.Duration));
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

        var response = new AddArtistDTO();

        if (song.Artists.Contains(artistToAdd)) {
            response.Message = $"Artist with ID '{artistId}' is already associated with song ID '{id}'.";
            response.IsAdded = true;

            return Ok(response);
        }

        song.Artists.Add(artistToAdd);
        _repoContext.SaveChanges();

        response.IsAdded = true;
        return Ok(response);
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

        var response = new RemoveArtistDTO();

        if (!song.Artists.Contains(artistToRemove)) {
            response.IsRemoved = true;
            response.Message = $"Artist with ID '{artistId}' is not associated with song ID '{id}'.";
            return Ok(response);
        }

        song.Artists.Remove(artistToRemove);
        _repoContext.SaveChanges();

        response.IsRemoved = true;
        return Ok(response);
    }

    public class AddArtistDTO {
        public bool IsAdded { get; set; } = false;
        public string Message { get; set; } = string.Empty;
    }

    public class RemoveArtistDTO {
        public bool IsRemoved { get; set; } = false;
        public string Message { get; set; } = string.Empty;
    }
}