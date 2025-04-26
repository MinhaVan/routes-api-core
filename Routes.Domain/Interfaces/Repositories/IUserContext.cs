namespace Routes.Domain.Interfaces.Repository;

public interface IUserContext
{
    int UserId { get; }
    int Empresa { get; }
    string Token { get; }
}