using System.Text;
using api.dtos;
using Microsoft.Extensions.Caching.Distributed;

namespace api.services;

public class ClientesService : IClienteService
{
    private readonly string? _connectionString;
    private readonly ClienteRepository _clienteRepository;
    private readonly IDistributedCache _distributedCache;

    public ClientesService(IConfiguration configuration, IDistributedCache distributedCache)
    {
        _connectionString = configuration.GetConnectionString("rinhaDb");
        _clienteRepository = new ClienteRepository(_connectionString);
        _distributedCache = distributedCache;
    }

    public async Task<IResult> Transacao(int id, TransacaoRequest? transacao)

    {
        if (transacao == null)
        {
            return Results.UnprocessableEntity();
        }

        if (transacao?.valor != Math.Floor(transacao.valor) || 
            string.IsNullOrEmpty(transacao.descricao) ||
            transacao.valor < 1 || transacao.descricao.Length > 10)
        {
            return Results.UnprocessableEntity();
        }

        if (transacao.tipo != 'd' && transacao.tipo != 'c')
        {
            return Results.UnprocessableEntity();
        }

        var entry = await _distributedCache.GetAsync(id.ToString());
        int? saldo = 0, limite = 0;

        if(entry == null) {
            (saldo, limite) = await _clienteRepository.GetCliente(id);
            
            if (saldo == null)
            {
                return Results.NotFound();
            }

            var encodedValue = Encoding.UTF8.GetBytes($"{saldo};{limite}");

            await _distributedCache.SetAsync(id.ToString(), encodedValue);
        } else 
        {
            var res = Encoding.UTF8.GetString(entry).Split(";");
            saldo = int.Parse(res[0]);
            limite = int.Parse(res[1]);
        }

        var valorInteiro = (int)transacao.valor;
        int? saldoAtual = valorInteiro + saldo;

        if (saldoAtual < -limite)
        {
            return Results.UnprocessableEntity();
        }

        await _clienteRepository.CreateTransacao(id, valorInteiro, (int)saldoAtual, transacao);
        await _distributedCache.RemoveAsync(id.ToString());

        return Results.Ok(new Cliente
        {
            limite = (int)limite,
            saldo = (int)saldoAtual
        });
    }

    public async Task<IResult> Extrato(int id)
    {

        var entry = await _distributedCache.GetAsync(id.ToString());
        int? saldo = null, limite = null;

        if(entry == null) {
            (saldo, limite) = await _clienteRepository.GetCliente(id);
            
            if (saldo == null)
            {
                return Results.NotFound();
            }

            var encodedValue = Encoding.UTF8.GetBytes($"{saldo};{limite}");

            await _distributedCache.SetAsync(id.ToString(), encodedValue);
        } else 
        {
            var res = Encoding.UTF8.GetString(entry).Split(";");
            saldo = int.Parse(res[0]);
            limite = int.Parse(res[1]);
        }

        var transacoesByCliente = await _clienteRepository.GetTransactionByUser(id);

        return Results.Ok(new TransacaoResponse
        {
            saldo = new Cliente
            {
                total = saldo,
                data_extrato = DateTime.UtcNow,
                limite = (int)limite
            },
            ultimas_transacoes = transacoesByCliente
        });
    }
}