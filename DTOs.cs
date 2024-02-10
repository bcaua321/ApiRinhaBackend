using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace api.dtos;

public class Cliente
{
    public Cliente() {}
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? limite { get; set; }

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
    public int valor { get; set; } 
    public char tipo { get; set; }
    public string? descricao { get; set; }
}

public class TransacaoResponse
{
    public Cliente? saldo { get; set; } 
    public IEnumerable<Transacao>? ultimas_transacoes { get; set; } 
}

// AOT in .NET dont have support for Reflection, so we have to use this
[JsonSerializable(typeof(Cliente))]
[JsonSerializable(typeof(Transacao))]
[JsonSerializable(typeof(TransacaoRequest))]
[JsonSerializable(typeof(TransacaoResponse))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}