#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["CalculadoraSprint.csproj", ""]
RUN dotnet restore "CalculadoraSprint.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "CalculadoraSprint.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CalculadoraSprint.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CalculadoraSprint.dll"]
