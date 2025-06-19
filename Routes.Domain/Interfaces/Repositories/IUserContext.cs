namespace Routes.Domain.Interfaces.Repositories;

public interface IUserContext
{
    int UserId { get; }
    string Token { get; }
}