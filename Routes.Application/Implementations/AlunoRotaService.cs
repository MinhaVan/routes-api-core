using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Routes.Domain.Enums;
using Routes.Domain.Interfaces.APIs;
using Routes.Domain.Interfaces.Repository;
using Routes.Domain.Interfaces.Services;
using Routes.Domain.Models;
using Routes.Domain.ViewModels;
using Routes.Service.Exceptions;

namespace Routes.Service.Implementations;

public class AlunoRotaService(
    IPessoasAPI _pessoasAPI,
    IBaseRepository<AlunoRota> _alunoRotaRepository,
    IBaseRepository<Rota> _rotaRepository,
    IMapper _mapper) : IAlunoRotaService
{
    public async Task AdicionarAsync(AlunoRotaViewModel alunoRota)
    {
        await _alunoRotaRepository.AdicionarAsync(_mapper.Map<AlunoRota>(alunoRota));
    }

    public async Task AtualizarAsync(AlunoRotaViewModel alunoRota)
    {
        var alunoRotaExistente = await _alunoRotaRepository.BuscarUmAsync(x => x.AlunoId == alunoRota.AlunoId && x.RotaId == alunoRota.RotaId);
        if (alunoRotaExistente == null)
        {
            throw new BusinessRuleException("Aluno Rota não encontrado.");
        }

        alunoRotaExistente.Status = alunoRota.Status;
        await _alunoRotaRepository.AtualizarAsync(alunoRotaExistente);
    }

    public async Task VincularRotaAsync(int rotaId, int alunoId)
    {
        if (rotaId < 1 || alunoId < 1)
            return;

        await ValidarRotaAlunoAsync(rotaId, alunoId);

        var alunoRota = await _alunoRotaRepository.BuscarUmAsync(x =>
            x.AlunoId == alunoId &&
            x.RotaId == rotaId);

        if (alunoRota is null)
        {
            var AlunoRota = new AlunoRota
            {
                AlunoId = alunoId,
                RotaId = rotaId
            };

            await _alunoRotaRepository.AdicionarAsync(AlunoRota);
        }
        else
        {
            alunoRota.Status = StatusEntityEnum.Ativo;
            await _alunoRotaRepository.AtualizarAsync(alunoRota);
        }
    }

    public async Task DesvincularRotaAsync(int rotaId, int alunoId)
    {
        if (rotaId < 1 || alunoId < 1)
            return;

        await ValidarRotaAlunoAsync(rotaId, alunoId);

        var alunoRota = await _alunoRotaRepository.BuscarUmAsync(x => x.AlunoId == alunoId && x.RotaId == rotaId);
        _ = alunoRota ?? throw new BusinessRuleException("Aluno não estava vinculado a essa rota!");

        alunoRota.Status = StatusEntityEnum.Deletado;
        await _alunoRotaRepository.AtualizarAsync(alunoRota);
    }

    public async Task<List<AlunoRotaViewModel>> ObterRotasPorAlunoAsync(int rotaId, int? alunoId = null)
    {
        var alunosRotas = await (
            alunoId.HasValue ?
            _alunoRotaRepository.BuscarAsync(x =>
                x.RotaId == rotaId &&
                x.AlunoId == alunoId.Value) :
            _alunoRotaRepository.BuscarAsync(x =>
                x.RotaId == rotaId)
        );

        return _mapper.Map<List<AlunoRotaViewModel>>(alunosRotas);
    }

    private async Task ValidarRotaAlunoAsync(int rotaId, int alunoId)
    {
        var rotaExistente = await _rotaRepository.ObterPorIdAsync(rotaId);
        _ = rotaExistente ?? throw new BusinessRuleException("A rota especificado não existe.");

        var alunoExistente = await _pessoasAPI.ObterAlunoPorIdAsync(new List<int> { alunoId });
        _ = alunoExistente ?? throw new BusinessRuleException("O aluno especificado não existe.");
    }
}