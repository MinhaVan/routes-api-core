using AutoMapper;
using Routes.Domain.Models;
using Routes.Domain.ViewModels.Rota;

namespace Routes.Service.Configurations;

public class AjusteAlunoRotaMapper : Profile
{
    public AjusteAlunoRotaMapper()
    {
        #region RotaAjusteEndereco
        CreateMap<RotaAjusteEnderecoViewModel, AjusteAlunoRota>().ReverseMap();
        #endregion
    }
}