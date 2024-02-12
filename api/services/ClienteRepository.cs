using api.dtos;
using Npgsql;

namespace api.services;

public class ClienteRepository
{
    private readonly string? _connectionString;

    public ClienteRepository(string? connectionString)
    {
        this._connectionString = connectionString;
    }
    
    public async Task<IEnumerable<Transacao>> GetTransactionByUser(int clienteId)
    {
        string query = $"SELECT valor, tipo, descricao, realizado_em FROM transacoes where cliente_id = @Id ORDER BY realizado_em DESC LIMIT 10;";

        using var connection = new NpgsqlConnection(_connectionString);
        var transacoes = new List<Transacao>();

        try 
        {
            await connection.OpenAsync();
            await using var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("Id", clienteId);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                // Process each row
                var transacao = new Transacao
                {
                    valor = (int)reader["valor"],
                    tipo = Convert.ToChar(reader["tipo"]),
                    descricao = (string)reader["descricao"],
                    realizado_em = (DateTime)reader["realizado_em"]
                };

                transacoes.Add(transacao);
            }
        }   catch (Exception e)
        {
            Console.WriteLine(e.Message, ConsoleColor.Red);
        }


        return transacoes;
    }

    public async Task CreateTransacao(int clienteId, int transacaoValor, int novoSaldo, TransacaoRequest transacao)
    {

        string commandText = @"
            CALL create_transacao_cliente(@ClienteId, @TransacaoValor, @NovoSaldo, @Tipo, @Descricao, @Data);";

        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        try
        {
            await using (var cmd = new NpgsqlCommand(commandText, connection))
            {
                cmd.Parameters.AddWithValue("ClienteId", clienteId);
                cmd.Parameters.AddWithValue("TransacaoValor", transacaoValor);
                cmd.Parameters.AddWithValue("NovoSaldo", novoSaldo);
                cmd.Parameters.AddWithValue("Tipo", transacao.tipo);
                cmd.Parameters.AddWithValue("Descricao", transacao.descricao ?? "fdf");
                cmd.Parameters.AddWithValue("Data", DateTime.Now);

                await cmd.ExecuteNonQueryAsync();
            }
        } catch (Exception e)
        {
            Console.WriteLine(e.Message, ConsoleColor.Red);
        }
    }

    public async Task<(int?, int?)> GetCliente(int clienteId)
    {
        string query = $"SELECT saldo, limite from clientes where id = @Id;";
        int? saldo = null;
        int? limite = null;

        try 
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("Id", clienteId);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                // Process each row
                limite = (int)reader["limite"];
                saldo = (int)reader["saldo"];
            }
        } catch (Exception e)
        {
            Console.WriteLine(e.Message, ConsoleColor.Red);
        }

        return (saldo, limite);
    }
}