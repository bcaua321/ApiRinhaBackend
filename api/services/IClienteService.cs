using api.dtos;

namespace api.services
{
    public interface IClienteService
    {
        Task<IResult> Transacao(int id, TransacaoRequest? transacao);
        Task<IResult> Extrato(int id);
    }
}