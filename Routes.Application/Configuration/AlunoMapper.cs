using AutoMapper;
using Routes.Domain.Models;
using Routes.Domain.ViewModels;

namespace Routes.Service.Configurations;

public class AlunoMapper : Profile
{
    public AlunoMapper()
    {
        CreateMap<AlunoDetalheViewModel, AlunoViewModel>().ReverseMap();
        CreateMap<EnderecoAtualizarViewModel, Endereco>().ReverseMap();
        CreateMap<EnderecoViewModel, Endereco>().ReverseMap();
    }
}