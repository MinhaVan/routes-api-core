namespace Routes.Domain.ViewModels;

public class AlunoRotaOnlineViewModel
{
    public AlunoViewModel Aluno { get; set; }
    public RotaViewModel Rota { get; set; }
    public bool IsOnline { get; set; }
}

