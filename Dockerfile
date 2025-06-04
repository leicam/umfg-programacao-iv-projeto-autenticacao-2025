# Acesse https://aka.ms/customizecontainer para saber como personalizar seu contêiner de depuração e como o Visual Studio usa este Dockerfile para criar suas imagens para uma depuração mais rápida.

# Esta fase é usada durante a execução no VS no modo rápido (Padrão para a configuração de Depuração)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Esta fase é usada para compilar o projeto de serviço
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["umfgcloud.autenticacao.webapi/umfgcloud.autenticacao.webapi.csproj", "umfgcloud.autenticacao.webapi/"]
COPY ["umfgcloud.apresentacao.identityRepositorio/umfgcloud.apresentacao.identityRepositorio.csproj", "umfgcloud.apresentacao.identityRepositorio/"]
COPY ["umfgcloud.autenticacao.aplicacao/umfgcloud.autenticacao.aplicacao.csproj", "umfgcloud.autenticacao.aplicacao/"]
COPY ["umfgcloud.autenticacao.dominio/umfgcloud.autenticacao.dominio.csproj", "umfgcloud.autenticacao.dominio/"]
RUN dotnet restore "./umfgcloud.autenticacao.webapi/umfgcloud.autenticacao.webapi.csproj"
COPY . .
WORKDIR "/src/umfgcloud.autenticacao.webapi"
RUN dotnet build "./umfgcloud.autenticacao.webapi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Esta fase é usada para publicar o projeto de serviço a ser copiado para a fase final
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./umfgcloud.autenticacao.webapi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Esta fase é usada na produção ou quando executada no VS no modo normal (padrão quando não está usando a configuração de Depuração)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Defina o ambiente aqui (por exemplo, Development, Staging ou Production)
ENV ASPNETCORE_ENVIRONMENT=Staging

# Configura a porta do ASP.NET Core para usar o $PORT do Heroku e executa a API
CMD ASPNETCORE_URLS="http://*:$PORT" dotnet umfgcloud.autenticacao.webapi.dll