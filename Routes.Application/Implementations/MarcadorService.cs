using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Routes.Domain.Enums;
using Routes.Domain.Interfaces.APIs;
using Routes.Domain.Interfaces.Repositories;
using Routes.Domain.Interfaces.Services;
using Routes.Domain.Models;
using Routes.Domain.ViewModels;
using Routes.Domain.ViewModels.Rota;

namespace Routes.Application.Implementations;

public class MarcadorService(
    IPessoasAPI _pessoasAPI,
    IBaseRepository<Endereco> _enderecoRepository,
    IBaseRepository<Rota> _rotaRepository,
    IBaseRepository<OrdemTrajeto> _ordemTrajetoRepository,
    IBaseRepository<AjusteAlunoRota> _ajusteAlunoRotaRepository,
    IMapper _mapper) : IMarcadorService
{
    public async Task<List<Marcador>> ObterTodosMarcadoresParaRotasAsync(int rotaId)
    {
        var marcadores = new List<Marcador>();

        var ordemTrajeto = await _ordemTrajetoRepository.BuscarUmAsync(x => x.RotaId == rotaId, z => z.Marcadores);
        if (ordemTrajeto is not null)
        {
            var enderecosIds = ordemTrajeto.Marcadores.Select(x => x.EnderecoId).Distinct().ToList();
            var enderecos = await _enderecoRepository.BuscarAsync(x => enderecosIds.Contains(x.Id));

            foreach (var m in ordemTrajeto.Marcadores.OrderBy(x => x.Ordem))
            {
                var marcadorViewModel = new Marcador
                {
                    EnderecoId = m.EnderecoId,
                    Id = m.Id,
                    Latitude = m.Latitude,
                    Longitude = m.Longitude,
                    Ordem = m.Ordem,
                    Titulo = enderecos.FirstOrDefault(x => x.Id == m.EnderecoId)?.ObterEndereco() ?? "Sem Endereço",
                    TipoMarcador = m.TipoMarcador
                };
                marcadores.Add(marcadorViewModel);
            }
        }
        else
        {
            var rota = await _rotaRepository.BuscarUmAsync(x => x.Id == rotaId, z => z.AlunoRotas.Where(x => x.Status == StatusEntityEnum.Ativo));

            var alunosId = rota.AlunoRotas.Select(x => x.AlunoId).ToList();
            var alunosResponse = await _pessoasAPI.ObterAlunoPorIdAsync(alunosId);
            var alunos = alunosResponse.Data;
            var now = DateTime.UtcNow;

            var ajusteAlunoRota = await _ajusteAlunoRotaRepository
                .BuscarAsync(x => alunosId.Contains(x.AlunoId) && x.Data.Date == now.Date && x.RotaId == rotaId,
                    z => z.EnderecoDestino,
                    z => z.EnderecoPartida,
                    z => z.EnderecoRetorno);

            marcadores.AddRange(ObterMarcadorPorRotaDirecao(alunos, rota.TipoRota, rotaId));
        }

        return marcadores;
    }

    public List<Marcador> ObterMarcadorPorRotaDirecao(
        IEnumerable<AlunoViewModel> alunos,
        TipoRotaEnum tipoRota,
        int rotaId,
        IEnumerable<AjusteAlunoRota> ajusteAlunoRota = null)
    {
        ajusteAlunoRota ??= new List<AjusteAlunoRota>();
        var marcadoresPartidas = new Dictionary<string, Marcador>();
        var marcadoresDestinos = new Dictionary<string, Marcador>();
        var now = DateTime.Now;
        var index = 0;

        foreach (var aluno in alunos)
        {
            var ajuste = ajusteAlunoRota.FirstOrDefault(x => x.AlunoId == aluno.Id && x.Data.Date == now.Date && x.RotaId == rotaId);

            void AddMarcador(Dictionary<string, Marcador> dict, string chave, int? enderecoId, string titulo, TipoMarcadorEnum tipoMarcador, double lat, double lng)
            {
                if (!dict.TryGetValue(chave, out var marcador))
                {
                    marcador = new Marcador
                    {
                        Ordem = index++,
                        Titulo = titulo,
                        TipoMarcador = tipoMarcador,
                        Latitude = lat,
                        Longitude = lng,
                        EnderecoId = enderecoId,
                        Aluno = _mapper.Map<AlunoViewModel>(aluno),
                        Alunos = new List<AlunoViewModel> { _mapper.Map<AlunoViewModel>(aluno) }
                    };
                    dict[chave] = marcador;
                }
                else
                {
                    marcador.Alunos.Add(_mapper.Map<AlunoViewModel>(aluno));
                }
            }

            if (tipoRota == TipoRotaEnum.Ida)
            {
                var latPartida = ajuste?.EnderecoPartida?.Latitude ?? aluno.EnderecoPartida.Latitude;
                var lngPartida = ajuste?.EnderecoPartida?.Longitude ?? aluno.EnderecoPartida.Longitude;
                var chavePartida = $"{latPartida},{lngPartida}";
                AddMarcador(marcadoresPartidas, chavePartida, aluno.EnderecoPartidaId, aluno.EnderecoPartida.ObterEndereco(), TipoMarcadorEnum.Partida, latPartida, lngPartida);

                var latDestino = ajuste?.EnderecoDestino?.Latitude ?? aluno.EnderecoDestino.Latitude;
                var lngDestino = ajuste?.EnderecoDestino?.Longitude ?? aluno.EnderecoDestino.Longitude;
                var chaveDestino = $"{latDestino},{lngDestino}";
                AddMarcador(marcadoresDestinos, chaveDestino, aluno.EnderecoDestinoId, aluno.EnderecoDestino.ObterEndereco(), TipoMarcadorEnum.Destino, latDestino, lngDestino);
            }
            else
            {
                var latInicioRetorno = ajuste?.EnderecoDestino?.Latitude ?? aluno.EnderecoDestino.Latitude;
                var lngInicioRetorno = ajuste?.EnderecoDestino?.Longitude ?? aluno.EnderecoDestino.Longitude;
                var chaveInicioRetorno = $"{latInicioRetorno},{lngInicioRetorno}";
                AddMarcador(marcadoresPartidas, chaveInicioRetorno, aluno.EnderecoDestinoId, aluno.EnderecoDestino.ObterEndereco(), TipoMarcadorEnum.InicioRetorno, latInicioRetorno, lngInicioRetorno);

                if (aluno.EnderecoRetorno != null && aluno.EnderecoRetornoId.HasValue)
                {
                    var latRetorno = aluno.EnderecoRetorno.Latitude;
                    var lngRetorno = aluno.EnderecoRetorno.Longitude;
                    var chaveRetorno = $"{latRetorno},{lngRetorno}";
                    AddMarcador(marcadoresDestinos, chaveRetorno, aluno.EnderecoRetornoId.Value, aluno.EnderecoRetorno.ObterEndereco(), TipoMarcadorEnum.Retorno, latRetorno, lngRetorno);
                }
            }
        }

        foreach (var (key, marcadorDestino) in marcadoresDestinos)
        {
            if (marcadoresPartidas.TryGetValue(key, out var marcadorExistente))
            {
                marcadorExistente.Alunos.AddRange(marcadorDestino.Alunos);
            }
            else
            {
                marcadoresPartidas[key] = marcadorDestino;
            }
        }

        return marcadoresPartidas.Values.ToList();
    }
}