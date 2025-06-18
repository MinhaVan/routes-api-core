using System.Diagnostics.CodeAnalysis;

namespace Routes.Domain.Utils;

[ExcludeFromCodeCoverage]
public static class KeyRedis
{
    public static string EnviarLocalizacao(int rotaId)
    {
        return $"rota-{rotaId}-localizacao";
    }

    public static class Veiculos
    {
        public static string Veiculo(int id) => $"veiculo-{id}";
        public static string VeiculosEmpresa(int empresa, bool deletado) => $"veiculos-empresa-{empresa}-deletado-{deletado}";
    }

    public static class Rotas
    {
        public const string Empresa = "rota-empresa-{0}-deletado-{1}";
    }
}