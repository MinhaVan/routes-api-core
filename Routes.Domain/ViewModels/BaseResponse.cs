using System.Collections.Generic;

namespace Routes.Domain.ViewModels;

public class BaseResponse<T>
{
    public bool Sucesso { get; set; } = true;
    public T Data { get; set; }
    public string Mensagem { get; set; } = "Operação realizada com sucesso";
    public List<string> Erros { get; set; } = new List<string>();
}