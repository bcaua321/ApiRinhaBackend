using api.dtos;
using api.services;

var builder = WebApplication.CreateSlimBuilder(args);
{
    builder.Services.ConfigureHttpJsonOptions(options =>
    {
        options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
    });

    builder.Services.AddSingleton<IClienteService, ClientesService>();
}

var app = builder.Build();
{
    app.MapPost("/clientes/{id}/transacoes", 
        async (int id, IClienteService clientesService, HttpRequest request) => {
            TransacaoRequest? transacaoRequest = null;

            try
            {
                transacaoRequest = await request.ReadFromJsonAsync<TransacaoRequest>();
            }
            catch(Exception) { }

            return await clientesService.Transacao(id, transacaoRequest);
        });

    app.MapGet("/clientes/{id}/extrato", 
        async (int id, IClienteService clientesService) => {
            return await clientesService.Extrato(id);
        });

    app.Run();
}
