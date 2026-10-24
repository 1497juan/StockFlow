using Microsoft.EntityFrameworkCore;
using StockFlow.Application.Repositories;
using StockFlow.Domain.Entities;
using StockFlow.Infrastructure.Persistence;

namespace StockFlow.Infrastructure.Repositories;

public class UserRepository: IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _context.Users.AsNoTracking().FirstOrDefaultAsync(
                user => user.Username == username,
                cancellationToken);
    }
}