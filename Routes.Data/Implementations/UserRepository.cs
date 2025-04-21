using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Routes.Data.Context;
using Routes.Domain.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;
using Routes.Domain.Models;
using Routes.Data.Repositories;
using Routes.Domain.ViewModels;
using Routes.Domain.Enums;

namespace Routes.Data.Implementations
{
    public class UsuarioRepository : BaseRepository<Usuario>, IUsuarioRepository
    {
        private readonly APIContext _ctx;

        public UsuarioRepository(APIContext context) : base(context)
        {
            _ctx = context;
        }

        public async Task<Usuario> LoginAsync(UsuarioLoginViewModel user)
        {
            var query = _ctx.Usuarios.Where(x =>
                x.EmpresaId == user.EmpresaId &&
                x.Senha.Equals(user.Senha) &&
                (x.CPF.Equals(user.CPF) || x.Email.Equals(user.Email))
            );

            if (user.IsMotorista)
            {
                query = query.Where(x => x.Perfil == PerfilEnum.Motorista);
            }

            return await query.FirstOrDefaultAsync();
        }

        public string ComputeHash(string input, SHA256CryptoServiceProvider algorithm)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashedBytes = algorithm.ComputeHash(inputBytes);
            return BitConverter.ToString(hashedBytes);
        }
    }
}