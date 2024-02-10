using api.dtos;
using Npgsql;

namespace api.services;

public class ClientesService : IClienteService
{
    private readonly string? _connectionString;
    private readonly ClienteRepository _clienteRepository;

    public ClientesService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("rinhaDb");
        _clienteRepository = new ClienteRepository(_connectionString);
    }

    public async Task<IResult> Transacao(int id, TransacaoRequest? transacao)
    {
        if (transacao == null)
        {
            return Results.UnprocessableEntity();
        }

        if (string.IsNullOrEmpty(transacao.descricao) || transacao.descricao.Length > 10)
        {
            return Results.UnprocessableEntity();
        }

        if (transacao.valor <= 0)
        {
            return Results.UnprocessableEntity();
        }

        if (transacao.tipo != 'd' && transacao.tipo != 'c')
        {
            return Results.UnprocessableEntity();
        }

        (int? saldo, int? limite) = await _clienteRepository.GetCliente(id);

        if (saldo is null || limite is null)
            return Results.NotFound();

        int? saldoAtual = transacao.valor + saldo;

        if (saldoAtual < -limite)
        {
            return Results.UnprocessableEntity();
        }

        await _clienteRepository.CreateTransacao(id, transacao.valor, (int)saldoAtual, transacao);

        return Results.Ok(new Cliente
        {
            limite = (int)limite,
            saldo = (int)saldoAtual
        });
    }

    public async Task<IResult> Extrato(int id)
    {
        (int? saldo, int? limite) = await _clienteRepository.GetCliente(id);

        if (saldo is null || limite is null)
            return Results.NotFound();

        var transacoesByCliente = await _clienteRepository.GetTransactionByUser(id);

        return Results.Ok(new TransacaoResponse
        {
            saldo = new Cliente
            {
                total = saldo,
                data_extrato = DateTime.UtcNow,
                limite = limite
            },
            ultimas_transacoes = transacoesByCliente
        });
    }
}