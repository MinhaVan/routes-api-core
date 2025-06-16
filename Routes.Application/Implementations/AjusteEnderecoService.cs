using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Routes.Domain.Enums;
using Routes.Domain.Interfaces.Repositories;
using Routes.Domain.Interfaces.Services;
using Routes.Domain.Models;
using Routes.Domain.ViewModels;
using Routes.Domain.ViewModels.Rota;
using Routes.Service.Exceptions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Routes.Domain.Interfaces.APIs;

namespace Routes.Service.Implementations;

public class AjusteEnderecoService(
    IMapper _mapper,
    IPessoasAPI _pessoasAPI,
    ILogger<AjusteEnderecoService> _logger,
    IBaseRepository<AjusteAlunoRota> _ajusteAlunoRotaRepository) : IAjusteEnderecoService
{

    public async Task<List<RotaAjusteEnderecoViewModel>> ObterAjusteEnderecoAsync(int AlunoId, int rotaId)
    {
        var now = DateTime.UtcNow.Date;
        var response = await _ajusteAlunoRotaRepository.BuscarAsync(x =>
            x.Status == StatusEntityEnum.Ativo &&
            x.AlunoId == AlunoId && x.RotaId == rotaId &&
            x.Data >= now,
            x => x.EnderecoDestino,
            x => x.EnderecoPartida,
            x => x.EnderecoRetorno);

        return _mapper.Map<List<RotaAjusteEnderecoViewModel>>(response.OrderBy(x => x.Data));
    }

    public async Task AdicionarAjusteEnderecoAsync(RotaAdicionarAjusteEnderecoViewModel alterarEnderecoViewModel)
    {
        try
        {
            var aluno = await _pessoasAPI.ObterAlunoPorIdAsync(new List<int> { alterarEnderecoViewModel.AlunoId });
            if (aluno is null)
            {
                _logger.LogInformation("[AdicionarAjusteEnderecoAsync] Aluno com identificador {0} não encontrado.", alterarEnderecoViewModel.AlunoId);
                throw new BusinessRuleException("Aluno não encontrado.");
            }

            var ajusteAlunoRotaDataBase = await _ajusteAlunoRotaRepository.BuscarAsync(
                x => x.AlunoId == alterarEnderecoViewModel.AlunoId &&
                     x.RotaId == alterarEnderecoViewModel.RotaId &&
                     x.Data.Date == alterarEnderecoViewModel.Data.Date);

            if (ajusteAlunoRotaDataBase is not null && ajusteAlunoRotaDataBase.Any(x => x.Status == StatusEntityEnum.Ativo))
            {
                _logger.LogInformation("[AdicionarAjusteEnderecoAsync] Já existe um ajuste para esse dia.");
                throw new BusinessRuleException("Já existe um ajuste para esse dia. Case queira, favor editar!");
            }

            // Logica para caso o usuário DELETOU um ajuste e está adicionando um novo para o mesmo aluno/rota/dia
            AjusteAlunoRota ajusteAlunoRota = null;
            if (ajusteAlunoRotaDataBase.Count() == 1 && ajusteAlunoRotaDataBase.Any(x => x.Status == StatusEntityEnum.Deletado))
            {
                ajusteAlunoRota = ajusteAlunoRotaDataBase.First();
                ajusteAlunoRota.Data = alterarEnderecoViewModel.Data;
                ajusteAlunoRota.NovoEnderecoDestinoId = alterarEnderecoViewModel.EnderecoDestinoId;
                ajusteAlunoRota.NovoEnderecoRetornoId = alterarEnderecoViewModel.EnderecoRetornoId;
                ajusteAlunoRota.NovoEnderecoPartidaId = alterarEnderecoViewModel.EnderecoPartidaId;
                ajusteAlunoRota.Status = StatusEntityEnum.Ativo;

                await _ajusteAlunoRotaRepository.AtualizarAsync(ajusteAlunoRota);
            }
            else
            {
                ajusteAlunoRota = new AjusteAlunoRota
                {
                    Data = alterarEnderecoViewModel.Data,
                    NovoEnderecoDestinoId = alterarEnderecoViewModel.EnderecoDestinoId,
                    NovoEnderecoRetornoId = alterarEnderecoViewModel.EnderecoRetornoId,
                    NovoEnderecoPartidaId = alterarEnderecoViewModel.EnderecoPartidaId,
                    AlunoId = alterarEnderecoViewModel.AlunoId,
                    RotaId = alterarEnderecoViewModel.RotaId
                };
                await _ajusteAlunoRotaRepository.AdicionarAsync(ajusteAlunoRota);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[AdicionarAjusteEnderecoAsync] Ocorreu um erro ao tentar adicionar um ajuste na rota. Data: {0}", JsonConvert.SerializeObject(alterarEnderecoViewModel, Formatting.None));
            throw;
        }
    }

    public async Task AlterarAjusteEnderecoAsync(RotaAlterarAjusteEnderecoViewModel alterarAjusteEnderecoViewModel)
    {
        try
        {
            var ajusteAlunoRota = await _ajusteAlunoRotaRepository.BuscarUmAsync(x => x.Id == alterarAjusteEnderecoViewModel.Id);
            if (ajusteAlunoRota is null)
            {
                _logger.LogInformation("[AlterarEnderecoAsync] Ajuste com os identificador {0} não encontrado.", alterarAjusteEnderecoViewModel.Id);
                throw new BusinessRuleException("Ajuste não encontrado.");
            }

            ajusteAlunoRota.Data = alterarAjusteEnderecoViewModel.Data;
            ajusteAlunoRota.NovoEnderecoDestinoId = alterarAjusteEnderecoViewModel.EnderecoDestinoId;
            ajusteAlunoRota.NovoEnderecoRetornoId = alterarAjusteEnderecoViewModel.EnderecoRetornoId;
            ajusteAlunoRota.NovoEnderecoPartidaId = alterarAjusteEnderecoViewModel.EnderecoPartidaId;
            ajusteAlunoRota.Status = alterarAjusteEnderecoViewModel.Deletado ? StatusEntityEnum.Deletado : StatusEntityEnum.Ativo;

            await _ajusteAlunoRotaRepository.AtualizarAsync(ajusteAlunoRota);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[AlterarEnderecoAsync] Ocorreu um erro ao tentar atualizar um ajuste na rota. Data: {0}", JsonConvert.SerializeObject(alterarAjusteEnderecoViewModel, Formatting.None));
            throw;
        }
    }
}