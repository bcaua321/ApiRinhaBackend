using api.dtos;
using api.services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateSlimBuilder(args);
{
    builder.Services.ConfigureHttpJsonOptions(options =>
    {
        options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
    });

    builder.Services.AddDistributedMemoryCache();
    builder.Services.AddSingleton<IClienteService, ClientesService>();
}

var app = builder.Build();
{
    app.MapPost("/clientes/{id}/transacoes", 
        async ([FromRoute]int id, [FromBody]TransacaoRequest transacaoRequest,  [FromServices]IClienteService clientesService) => {
            return await clientesService.Transacao(id, transacaoRequest);
        });

    app.MapGet("/clientes/{id}/extrato", 
        async ([FromRoute]int id, [FromServices]IClienteService clientesService) => {
            return await clientesService.Extrato(id);
        });

    app.Run();
}
