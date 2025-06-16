using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Routes.Data.Context;
using Routes.Data.Repositories;
using Routes.Domain.Enums;
using Routes.Domain.Interfaces.Repositories;
using Routes.Domain.Models;

namespace Routes.Data.Implementations;

public class MotoristaRotaRepository : BaseRepository<MotoristaRota>, IMotoristaRotaRepository
{
    private readonly APIContext _context;
    public MotoristaRotaRepository(APIContext context) : base(context)
    {
        _context = context;
    }

    public async Task AtualizarStatusAsync(int motoristaId, int rotaId, StatusEntityEnum status)
    {
        var motoristaRota = await _context.MotoristaRotas
            .FirstOrDefaultAsync(mr => mr.MotoristaId == motoristaId && mr.RotaId == rotaId);

        if (motoristaRota != null)
        {
            motoristaRota.Status = status;
            _context.Update(motoristaRota);
            await _context.SaveChangesAsync();
        }
        else
        {
            throw new Exception("MotoristaRota não foi encontrado!");
        }
    }
}