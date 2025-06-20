using System;
using System.Collections.Generic;
using System.Linq;
using Routes.Domain.ViewModels.Rota;

namespace Routes.Domain.Utils;

public static class MarcadorExtensions
{
    public static List<Marcador> OrdenarComDependencias(this List<Marcador> marcadores)
    {
        var resultado = new List<Marcador>();
        var visitado = new HashSet<Guid>();
        var emProcesso = new HashSet<Guid>();

        void Visitar(Marcador marcador)
        {
            if (visitado.Contains(marcador.IdTemporario)) return;
            if (emProcesso.Contains(marcador.IdTemporario))
                throw new Exception("Ciclo detectado nas dependências de marcadores.");

            emProcesso.Add(marcador.IdTemporario);

            foreach (var prereqId in marcador.Prerequisitos)
            {
                var prereq = marcadores.FirstOrDefault(m => m.IdTemporario == prereqId);
                if (prereq != null) Visitar(prereq);
            }

            emProcesso.Remove(marcador.IdTemporario);
            visitado.Add(marcador.IdTemporario);
            resultado.Add(marcador);
        }

        foreach (var marcador in marcadores)
        {
            Visitar(marcador);
        }

        return resultado;
    }
}