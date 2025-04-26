using AutoMapper;
using Routes.Domain.ViewModels;

namespace Routes.Service.Configurations;

public class MotoristaMapper : Profile
{
    public MotoristaMapper()
    {
        #region Motorista
        CreateMap<MotoristaNovoViewModel, UsuarioNovoViewModel>().ReverseMap();
        CreateMap<UsuarioViewModel, MotoristaViewModel>().ReverseMap();
        #endregion
    }
}