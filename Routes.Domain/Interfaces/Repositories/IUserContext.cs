namespace Routes.Domain.Interfaces.Repositories;

public interface IUserContext
{
    int UserId { get; }
    int Empresa { get; }
    string Token { get; }
}