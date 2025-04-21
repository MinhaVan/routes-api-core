using System;
using System.Collections.Generic;
using Routes.Domain.Enums;

namespace Routes.Domain.Models
{
    public class RotaHistorico : Entity
    {
        public int RotaId { get; set; }
        public TipoRotaEnum TipoRota { get; set; }
        public DateTime DataRealizacao { get; set; }
        public DateTime? DataFim { get; set; }
        public bool EmAndamento { get; set; }
        //
        public virtual Rota Rota { get; set; }
        public virtual List<AlunoRotaHistorico> AlunoRotaHistorico { get; set; }
        public virtual List<LocalizacaoTrajeto> LocalizacaoTrajeto { get; set; }
    }
}