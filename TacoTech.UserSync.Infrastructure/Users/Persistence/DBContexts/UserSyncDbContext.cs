using Microsoft.EntityFrameworkCore;
using TacoTech.UserSync.Infrastructure.Users.Persistence.Entities;

namespace TacoTech.UserSync.Infrastructure.Users.Persistence.DBContext
{
    /// <summary>
    /// EF Core database context for the user synchronization module.
    /// </summary>
    public class UserSyncDbContext : DbContext
    {
        /// <summary>
        /// Gets the DbSet representing users in the local store.
        /// </summary>
        public DbSet<UserEntity> Users => Set<UserEntity>();

        /// <summary>
        /// Initializes a new instance of the <see cref="UserSyncDbContext"/> class.
        /// </summary>
        /// <param name="options">The DbContext options.</param>
        public UserSyncDbContext(DbContextOptions<UserSyncDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Configures EF Core model mappings.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserEntity>(builder =>
            {
                builder.ToTable("Users");
                builder.HasKey(x => x.Id);

                builder.Property(x => x.Id)
                       .ValueGeneratedNever();

                builder.Property(x => x.Username)
                       .HasMaxLength(50)
                       .IsRequired();

                builder.Property(x => x.FullName)
                       .HasMaxLength(100)
                       .IsRequired();

                builder.Property(x => x.Email)
                       .HasMaxLength(100)
                       .IsRequired();

                builder.Property(x => x.City)
                       .HasMaxLength(100)
                       .IsRequired();
            });
        }
    }
}
