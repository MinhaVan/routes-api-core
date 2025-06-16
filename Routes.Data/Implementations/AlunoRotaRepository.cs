using System.Threading.Tasks;
using Routes.Data.Context;
using Routes.Data.Repositories;
using Routes.Domain.Interfaces.Repositories;
using Routes.Domain.Models;

namespace Routes.Data.Implementations;

public class AlunoRotaRepository : BaseRepository<AlunoRota>, IAlunoRotaRepository
{
    private readonly APIContext _context;
    public AlunoRotaRepository(APIContext context) : base(context)
    {
        _context = context;
    }

    public async Task AtualizarStatusAsync(AlunoRota alunoRota)
    {
        _context.Update(alunoRota);
        await _context.SaveChangesAsync();
    }
}