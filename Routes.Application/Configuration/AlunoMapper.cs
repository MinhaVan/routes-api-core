using AutoMapper;
using Routes.Domain.ViewModels;
using Routes.Domain.Models;

namespace Routes.Service.Configurations;

public class AlunoMapper : Profile
{
    public AlunoMapper()
    {
        CreateMap<AlunoViewModel, Aluno>().ReverseMap();
        CreateMap<AlunoAdicionarViewModel, Aluno>().ReverseMap();
        CreateMap<AlunoRotaViewModel, AlunoRota>().ReverseMap();
        CreateMap<AlunoDetalheViewModel, Aluno>().ReverseMap();
    }
}