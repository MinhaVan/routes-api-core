using System.Security.Cryptography;
using System.Threading.Tasks;
using Routes.Domain.Models;
using Routes.Domain.ViewModels;

namespace Routes.Domain.Interfaces.Repository;

public interface IUsuarioRepository : IBaseRepository<Usuario>
{
    Task<Usuario> LoginAsync(UsuarioLoginViewModel user);
    string ComputeHash(string input, SHA256CryptoServiceProvider algorithm);
}