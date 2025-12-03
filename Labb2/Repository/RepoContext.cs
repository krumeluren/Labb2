using Labb2.Entities;
using Microsoft.EntityFrameworkCore;
public class RepoContext : DbContext {
    public RepoContext (DbContextOptions options) : base(options) { }

    public DbSet<Artist> Artists { get; set; }
    public DbSet<Album> Albums { get; set; }
    public DbSet<Song> Songs { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Playlist> Playlists { get; set; }
}
