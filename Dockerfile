FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 5228

ENV ASPNETCORE_URLS=http://+:5228



FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["endnubedrevejr.csproj", "./"]
RUN dotnet restore "endnubedrevejr.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "endnubedrevejr.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "endnubedrevejr.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "endnubedrevejr.dll"]
