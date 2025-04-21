namespace Routes.Domain.Models
{
    public class LocalizacaoTrajeto : Entity
    {
        public int RotaId { get; set; }
        public int RotaHistoricoId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        //
        public virtual RotaHistorico RotaHistorico { get; set; }
    }
}