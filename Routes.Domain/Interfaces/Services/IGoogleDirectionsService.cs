using System.Collections.Generic;
using System.Threading.Tasks;
using Routes.Domain.ViewModels;
using Routes.Domain.ViewModels.Rota;

namespace Routes.Domain.Interfaces.Services;

public interface IGoogleDirectionsService
{
    Task<Marcador> ObterMarcadorAsync(string endereco);
    Task<BaseResponse<List<Marcador>>> ObterRotaIdealAsync(Marcador origem, Marcador destino, List<Marcador> pontosIntermediarios);
}