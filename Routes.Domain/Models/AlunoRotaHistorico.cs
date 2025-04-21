using System;

namespace Routes.Domain.Models
{
    public class AlunoRotaHistorico : Entity
    {
        public int RotaHistoricoId { get; set; }
        public int AlunoId { get; set; }
        public DateTime DataRealizacao { get; set; }
        public bool EntrouNaVan { get; set; }
        public string Observacao { get; set; }
        //
        public virtual RotaHistorico RotaHistorico { get; set; }
        public virtual Aluno Aluno { get; set; }
    }
}