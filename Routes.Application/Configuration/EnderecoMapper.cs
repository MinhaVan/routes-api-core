using AutoMapper;
using Routes.Domain.Models;
using Routes.Domain.ViewModels;

namespace Routes.Service.Configurations;

public class EnderecoMapper : Profile
{
    public EnderecoMapper()
    {
        CreateMap<EnderecoAdicionarViewModel, Endereco>().ReverseMap();
        CreateMap<EnderecoAtualizarViewModel, Endereco>().ReverseMap();
        CreateMap<EnderecoViewModel, Endereco>().ReverseMap();
    }
}