using Labb2.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Labb2.Controllers;

[ApiController]
[Route("Artist")]
public class ArtistController : ControllerBase {
    private readonly RepoContext _repoContext;
    private readonly ILogger<ArtistController> _logger;
    public ArtistController (ILogger<ArtistController> logger, RepoContext repoContext) {
        _logger = logger;
        _repoContext = repoContext;
    }
    [HttpGet("all")]
    public IActionResult GetAllArtists () {
        var artists = _repoContext.Artists
            .Include(a => a.Albums)
            .Include(a => a.Songs)
            .ToList();
        return Ok(artists);
    }
    [HttpGet("get")]
    public IActionResult GetArtistByName (string name) {
        var artist = _repoContext.Artists
            .Where(a => a.Name == name)
            .Include(a => a.Albums)
            .Include(a => a.Songs)
            .FirstOrDefault();
        if (artist == null) return NotFound($"Artist with name '{name}' not found.");

        return Ok(artist);
    }

    [HttpPost("add")]
    public IActionResult AddArtist (string name) {
        var existingArtist = _repoContext.Artists
            .Where(a => a.Name == name)
            .FirstOrDefault();
        if (existingArtist != null) return Conflict($"Artist with name '{name}' already exists.");

        var newArtist = new Artist {
            Name = name
        };
        _repoContext.Artists.Add(newArtist);
        _repoContext.SaveChanges();
        return Ok(newArtist);
    }

    [HttpDelete("delete")]
    public IActionResult DeleteArtist (string name) {
        var artist = _repoContext.Artists
            .Where(a => a.Name == name)
            .FirstOrDefault();
        if (artist == null) return NotFound($"Artist with name '{name}' not found.");

        _repoContext.Artists.Remove(artist);
        _repoContext.SaveChanges();
        return Ok($"Artist with name '{name}' deleted successfully.");
    }
}
