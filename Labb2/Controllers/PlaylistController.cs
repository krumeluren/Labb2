using Labb2.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Labb2.Controllers;

[ApiController]
[Route("Playlist")]
public class PlaylistController : ControllerBase {
    private readonly RepoContext _repoContext;
    private readonly ILogger<UserController> _logger;

    public PlaylistController (ILogger<UserController> logger, RepoContext repoContext) {
        _logger = logger;
        _repoContext = repoContext;
    }

    [HttpPost("create")]
    public IActionResult CreatePlaylist (string username, string playlistName) {
        var user = _repoContext.Users
            .Where(u => u.Username == username)
            .FirstOrDefault();
        if (user == null) return NotFound($"User with name '{username}' not found.");

        var newPlaylist = new Playlist {
            Name = playlistName,
            UserId = user.Id
        };

        var response = new CreateDTO();
        _repoContext.Playlists.Add(newPlaylist);
        _repoContext.SaveChanges();

        response.IsCreated = true;
        response.Id = newPlaylist.Id;

        return Ok(newPlaylist);
    }
    public class CreateDTO {
        public bool IsCreated { get; set; }
        public int Id { get; set; }
    }

    [HttpPost("delete")]
    public IActionResult DeletePlaylist (string username, int playlistId) {
        var user = _repoContext.Users
            .Where(u => u.Username == username)
            .FirstOrDefault();

        if (user == null) return NotFound($"User with name '{username}' not found.");

        var playlist = _repoContext.Playlists
            .Where(p => p.Id == playlistId && p.UserId == user.Id)
            .FirstOrDefault();

        var response = new DeleteDTO();
        if (playlist == null) {
            response.IsDeleted = true;
            response.Message = $"Playlist with ID '{playlistId}' not found for user '{username}'.";
            return Ok(response);
        }

        _repoContext.Playlists.Remove(playlist);
        _repoContext.SaveChanges();

        response.IsDeleted = true;
        return Ok(response);
    }

    public class DeleteDTO {
        public bool IsDeleted { get; set; }
        public string Message { get; set; } = string.Empty;
    }


    [HttpPost("add")]
    public IActionResult AddSongToPlaylist (string username, int playlistId, int songId) {
        var user = _repoContext.Users
            .Where(u => u.Username == username)
            .FirstOrDefault();
        if (user == null) return NotFound($"User with name '{username}' not found.");

        var playlist = _repoContext.Playlists
            .Where(p => p.Id == playlistId && p.UserId == user.Id)
            .Include(p => p.Songs)
            .FirstOrDefault();

        if (playlist == null) return NotFound($"Playlist with ID '{playlistId}' not found for user '{username}'.");

        var song = _repoContext.Songs
            .Where(s => s.Id == songId)
            .FirstOrDefault();
        if (song == null) return NotFound($"Song with ID '{songId}' not found.");

        var response = new AddedSongFromPlaylistDTO();

        if (playlist.Songs.Any(x => x.Id == song.Id)) {
            response.IsAdded = true;
            response.Message = $"Song with ID '{songId}' already in playlist.";
            return Ok(response);
        }

        playlist.Songs.Add(song);
        _repoContext.SaveChanges();

        response.IsAdded = true;

        return Ok(response);
    }



    [HttpPost("remove")]
    public IActionResult RemoveSongFromPlaylist (string username, int playlistId, int songId) {
        var user = _repoContext.Users
            .Where(u => u.Username == username)
            .FirstOrDefault();
        if (user == null) return NotFound($"User with name '{username}' not found.");
        var playlist = _repoContext.Playlists
            .Where(p => p.Id == playlistId && p.UserId == user.Id)
            .Include(p => p.Songs)
            .FirstOrDefault();
        if (playlist == null) return NotFound($"Playlist with ID '{playlistId}' not found for user '{username}'.");

        var song = playlist.Songs
            .Where(s => s.Id == songId)
            .FirstOrDefault();

        var response = new RemovedSongFromPlaylistDTO();
        if (song == null) {
            response.IsRemoved = true;
            response.Message = $"Song with ID '{songId}' not found in playlist '{playlistId}'.";
            return Ok(response);
        }

        playlist.Songs.Remove(song);
        _repoContext.SaveChanges();

        response.IsRemoved = true;
        return Ok(response);
    }


    public class RemovedSongFromPlaylistDTO {
        public bool IsRemoved { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class AddedSongFromPlaylistDTO {
        public bool IsAdded { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
