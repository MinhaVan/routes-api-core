using AutoMapper;
using Routes.Domain.ViewModels;
using Routes.Domain.Models;

namespace Routes.Service.Configurations;

public class MotoristaMapper : Profile
{
    public MotoristaMapper()
    {
        #region Motorista
        CreateMap<MotoristaNovoViewModel, UsuarioNovoViewModel>().ReverseMap();
        CreateMap<MotoristaNovoViewModel, Motorista>().ReverseMap();
        CreateMap<UsuarioViewModel, MotoristaViewModel>().ReverseMap();
        CreateMap<Motorista, MotoristaViewModel>().ReverseMap();
        CreateMap<Usuario, MotoristaViewModel>().ReverseMap();
        #endregion
    }
}