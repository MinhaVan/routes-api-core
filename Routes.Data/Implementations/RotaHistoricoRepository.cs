using System.Linq;
using System.Threading.Tasks;
using Routes.Data.Context;
using Routes.Data.Repositories;
using Routes.Domain.Interfaces.Repositories;
using Routes.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Routes.Data.Implementations;

public class RotaHistoricoRepository : BaseRepository<RotaHistorico>, IRotaHistoricoRepository
{
    private readonly APIContext _context;
    public RotaHistoricoRepository(APIContext context) : base(context)
    {
        _context = context;
    }

    public async Task<RotaHistorico> ObterUltimoTrajetoAsync(int rotaId)
    {
        return await _context.RotaHistoricos
            .Include(x => x.Rota)
            .Include(x => x.Rota.Veiculo)
            .Include(x => x.Rota.AlunoRotas)
            .Where(x => x.RotaId == rotaId)
            .OrderByDescending(x => x.DataFim)
            .FirstOrDefaultAsync();
    }
}