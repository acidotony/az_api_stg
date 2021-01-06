#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /etc/ssl/openssl.cnf
WORKDIR /src
COPY ["Edenred.AR.Api.Storage/Edenred.AR.Api.Storage.csproj", "Edenred.AR.Api.Storage/"]
#RUN dotnet restore "Edenred.AR.Api.Storage/Edenred.AR.Api.Storage.csproj"
COPY . .
WORKDIR "/src/Edenred.AR.Api.Storage"
RUN dotnet build "Edenred.AR.Api.Storage.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Edenred.AR.Api.Storage.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Edenred.AR.Api.Storage.dll"]