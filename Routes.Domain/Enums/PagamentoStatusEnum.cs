using System.ComponentModel;

namespace Routes.Domain.Enums;

public enum PagamentoStatusEnum
{
    [Description("Novo")] NEW = 0,
    [Description("Processando")] PROCESSING = 1,
    [Description("Confirmado")] CONFIRMED = 2,
    [Description("Recebido")] RECEIVED = 3,
    [Description("Recusado")] REFUNDED = 4,
    [Description("Aguardando")] OVERDUE = 5,
    [Description("Reprovado")] REPROVED = 6,
    [Description("Aprovado")] APPROVED = 7,
    [Description("Sem informação")] UNDEFINED = 8
}