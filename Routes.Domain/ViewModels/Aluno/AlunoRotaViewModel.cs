using Routes.Domain.Enums;

namespace Routes.Domain.ViewModels;

public class AlunoRotaViewModel
{
    public int AlunoId { get; set; }
    public int RotaId { get; set; }
    public StatusEntityEnum Status { get; set; }
}