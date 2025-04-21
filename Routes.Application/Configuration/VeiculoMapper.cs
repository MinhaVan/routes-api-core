using AutoMapper;
using Routes.Domain.ViewModels;
using Routes.Domain.Models;

namespace Routes.Service.Configurations;

public class VeiculoMapper : Profile
{
    public VeiculoMapper()
    {
        CreateMap<VeiculoViewModel, Veiculo>().ReverseMap();
        CreateMap<VeiculoAdicionarViewModel, Veiculo>().ReverseMap();
        CreateMap<VeiculoAtualizarViewModel, Veiculo>().ReverseMap();
    }
}