using System.Text.Json;
using System.Text.Json.Serialization;

namespace api.dtos;

// Preferi já usar snake_case (má prática no C#, não faça isso em casa)
public class Cliente
{

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Decimal limite { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? data_extrato { get; set; } = null;
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? saldo { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? total { get; set; }
}

public class Transacao 
{
    public int valor { get; set; }
    public char tipo { get; set; }
    public string? descricao { get; set; }
    public DateTime realizado_em { get; set; }
}

public class TransacaoRequest
{
    public decimal valor { get; set; } 
    public char tipo { get; set; }
    public string? descricao { get; set; }
}

public class TransacaoResponse
{
    public Cliente? saldo { get; set; } 
    public IEnumerable<Transacao>? ultimas_transacoes { get; set; } 
}

// AOT no .NET nao tem suporte a Reflection, 
// por isso para serializar as requests/responses tem que usar isso
[JsonSerializable(typeof(Cliente))]
[JsonSerializable(typeof(Transacao))]
[JsonSerializable(typeof(TransacaoRequest))]
[JsonSerializable(typeof(TransacaoResponse))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}