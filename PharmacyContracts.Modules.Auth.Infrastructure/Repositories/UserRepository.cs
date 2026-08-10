// Repositories/UserRepository.cs
using Microsoft.EntityFrameworkCore;
using PharmacyContracts.Modules.Auth.Application.Interfaces;
using PharmacyContracts.Modules.Auth.Domain.Entities;
using PharmacyContracts.Modules.Auth.Infrastructure.Data;

namespace PharmacyContracts.Modules.Auth.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AuthDbContext _context;
    public UserRepository(AuthDbContext context) => _context = context;

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => _context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public Task<List<User>> GetAllAsync(CancellationToken cancellationToken = default)
        => _context.Users.OrderByDescending(u => u.CreatedAt).ToListAsync(cancellationToken);

    public async Task AddAsync(User entity, CancellationToken cancellationToken = default)
        => await _context.Users.AddAsync(entity, cancellationToken);

    public void Update(User entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Users.Update(entity);
    }

    public void Remove(User entity) => _context.Users.Remove(entity);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);
}