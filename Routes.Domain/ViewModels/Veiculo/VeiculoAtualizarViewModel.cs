using Routes.Domain.Enums;

namespace Routes.Domain.ViewModels;

public class VeiculoAtualizarViewModel
{
    public int Id { get; set; }
    public int Ano { get; set; }
    public int AnoModelo { get; set; }
    public string Cor { get; set; }
    public string Marca { get; set; }
    public string Modelo { get; set; }
    public string Placa { get; set; }
    public TipoVeiculoEnum TipoVeiculo { get; set; }
}