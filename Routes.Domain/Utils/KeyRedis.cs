namespace Routes.Domain.Utils;

public static class KeyRedis
{
    public static string EnviarLocalizacao(int rotaId)
    {
        return $"rota-{rotaId}-localizacao";
    }
    public static class Rotas
    {
        public const string Empresa = "rota-empresa-{0}-deletado-{1}";
    }
}