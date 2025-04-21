using System;
using Routes.Domain.Enums;

namespace Routes.Domain.Models;

public abstract class Entity
{
    public int Id { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime DataAlteracao { get; set; }
    public StatusEntityEnum Status { get; set; }
}