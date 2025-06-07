using System.Collections.Generic;
using System.Threading.Tasks;
using Routes.Domain.Models;
using Routes.Domain.ViewModels.Rota;

namespace Routes.Domain.Interfaces.Services;

public interface IOrdemTrajetoService
{
    Task SalvarOrdemDoTrajetoAsync(int rotaId, List<Marcador> marcadoresOrdenados);
    Task AtualizarOuCriarOrdemTrajeto(OrdemTrajeto ordemTrajeto, int rotaId, List<Marcador> rotaIdeal);
}