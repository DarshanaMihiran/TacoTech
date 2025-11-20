using Microsoft.EntityFrameworkCore;
using TacoTech.UserSync.Domain.Users.Aggregates;
using TacoTech.UserSync.Domain.Users.Interfaces.Repositories;
using TacoTech.UserSync.Domain.Users.UniqueIdentifiers;
using TacoTech.UserSync.Domain.Users.ValueObjects;
using TacoTech.UserSync.Infrastructure.Users.Persistence.DBContext;
using TacoTech.UserSync.Infrastructure.Users.Persistence.Entities;

namespace TacoTech.UserSync.Infrastructure.Users.Persistence.Repositories
{
    /// <summary>
    /// EF Core implementation of <see cref="IUserRepository"/> using SQLite.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly UserSyncDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The EF Core database context.</param>
        public UserRepository(UserSyncDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<User>> GetAllAsync(CancellationToken ct = default)
        {
            var entities = await _dbContext.Users.AsNoTracking().ToListAsync(ct);
            return entities.Select(MapToDomain).ToList();
        }

        /// <inheritdoc />
        public async Task AddAsync(User user, CancellationToken ct = default)
        {
            _dbContext.Users.Add(MapToEntity(user));
            await _dbContext.SaveChangesAsync(ct);
        }

        /// <inheritdoc />
        public async Task UpdateAsync(User user, CancellationToken ct = default)
        {
            _dbContext.Users.Update(MapToEntity(user));
            await _dbContext.SaveChangesAsync(ct);
        }

        /// <summary>
        /// Maps an EF entity to a domain user.
        /// </summary>
        /// <param name="entity">The user entity.</param>
        /// <returns>The domain user.</returns>
        private static User MapToDomain(UserEntity entity) =>
            new(
                new UserId(entity.Id),
                entity.Username,
                entity.FullName,
                new Email(entity.Email),
                new City(entity.City));

        /// <summary>
        /// Maps a domain user to an EF entity.
        /// </summary>
        /// <param name="user">The domain user.</param>
        /// <returns>The user entity.</returns>
        private static UserEntity MapToEntity(User user) =>
            new()
            {
                Id = user.Id.Value,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email.Value,
                City = user.City.Value
            };
    }
}
