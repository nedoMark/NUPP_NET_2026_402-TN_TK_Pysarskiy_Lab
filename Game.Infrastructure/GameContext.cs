using Game.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
namespace Game.Infrastructure;
public class GameContext : DbContext
{
    public GameContext(DbContextOptions<GameContext> options) : base(options) { }
    public DbSet<CharacterModel> Characters => Set<CharacterModel>();
    public DbSet<PlayerModel> Players => Set<PlayerModel>();
    public DbSet<EnemyModel> Enemies => Set<EnemyModel>();
    public DbSet<PlayerProfileModel> Profiles => Set<PlayerProfileModel>();
    public DbSet<ItemModel> Items => Set<ItemModel>();
    public DbSet<WeaponModel> Weapons => Set<WeaponModel>();
    public DbSet<TagModel> Tags => Set<TagModel>();
    protected override void OnModelCreating(ModelBuilder model)
    {
        model.Entity<CharacterModel>().UseTptMappingStrategy().ToTable("Characters");
        model.Entity<PlayerModel>().ToTable("Players");
        model.Entity<EnemyModel>().ToTable("Enemies");
        model.Entity<PlayerModel>().HasOne(x => x.Profile).WithOne(x => x.Player).HasForeignKey<PlayerProfileModel>(x => x.PlayerId).OnDelete(DeleteBehavior.Cascade);
        model.Entity<CharacterModel>().HasMany(x => x.Items).WithOne(x => x.Character).HasForeignKey(x => x.CharacterId).OnDelete(DeleteBehavior.Cascade);
        model.Entity<WeaponModel>().HasMany(x => x.Tags).WithMany(x => x.Weapons).UsingEntity(j => j.ToTable("WeaponTags"));
    }
}
public class GameContextFactory : IDesignTimeDbContextFactory<GameContext>
{
    public GameContext CreateDbContext(string[] args) => new(new DbContextOptionsBuilder<GameContext>().UseSqlite("Data Source=game.db").Options);
}
