<h1 align="center">Api para a <a href="https://github.com/zanfranceschi/rinha-de-backend-2024-q1/">Rinha de backend</a></h1>

<p align="center">Um exemplo de API feita para a rinha com asp.net core com AOT nativo. </p>

### 🛠 Tecnologias

- ASP.NET Core (Native AOT)
- Nginx
- PostgreSQL

### 🧐 Por que AOT nativo com asp.net core?

<p align="center">
  <img width="460" height="300" src="https://github.com/bcaua321/ApiRinhaBackend/assets/67557512/8b7502f5-2844-4c91-ad47-6d901d89be1f">
</p>

<p>Com AOT nativo, produz uma imagem menor, menos uso de memória e mais rápido ao iniciar.</p>


#### 🚩 🚩 🚩 Observação 
<p>Para criar uma API no asp.net core com suporte ao AOT, será necessário realizar algumas alterações no código, 
recomendo dar uma olhada <a href="https://learn.microsoft.com/en-us/aspnet/core/fundamentals/native-aot?view=aspnetcore-8.0">aqui</a>.</p>

<p>Para criar uma imagem com suporte, recomendo dar uma olhada <a href="https://github.com/dotnet/dotnet-docker/blob/main/samples/releasesapi/">aqui</a>.</p>

<p>Tive muito problemas para iniciar o build da aplicação, não consegui fazer mais otimizações para o build da imagem, só consegui setar ServerGarbageCollection para false na api.csproj</p>
