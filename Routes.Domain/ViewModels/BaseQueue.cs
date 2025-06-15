using System.Collections.Generic;
using System.Linq;

namespace Routes.Domain.ViewModels;

public class BaseQueue<T>
{
    public T Mensagem { get; set; }
    public int Retry { get; set; } = 0;
    public List<string> Erros { get; set; } = new();
    public bool Sucesso => !Erros.Any();
}