using StockFlow.Domain.Entities;

namespace StockFlow.Application.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
}