using AuthService.Domain.Entities;
using AuthService.Domain.Repositories;
using AuthService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AuthService.Infrastructure.Repositories
{
    public class UserRepository(AuthDbContext dbContext, ILogger<UserRepository> logger) : IUserRepository
    {
        private readonly AuthDbContext _dbContext = dbContext;
        private readonly ILogger<UserRepository> _logger = logger;

        public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            _logger.LogInformation("Fetching user by email {Email}", email);
            return await _dbContext.Users.SingleOrDefaultAsync(u => u.Email == email, ct);
        } 

        public async Task AddAsync(User user, CancellationToken ct = default)
        {
            _logger.LogInformation("Adding new user with email {Email}", user.Email);
            await _dbContext.Users.AddAsync(user, ct);
        }
            

        public Task SaveChangesAsync(CancellationToken ct = default) => _dbContext.SaveChangesAsync(ct);
    }
}
