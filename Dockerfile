#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-alpine3.10 AS base

MAINTAINER DevDotNet.Org <anton@devdotnet.org>
LABEL maintainer="DevDotNet.Org <anton@devdotnet.org>"

WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine3.10 AS build
WORKDIR /src
COPY ["Sharer-Example/Sharer-Example.csproj", "Sharer-Example/"]
COPY ["Sharer-Example/Sharer-NETStandard.csproj", "Sharer-NETStandard/"]

RUN dotnet restore "Sharer-Example/Sharer-Example.csproj"

COPY . .
WORKDIR "/src/Sharer-Example"
RUN dotnet build "Sharer-Example.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sharer-Example.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sharer-Example.dll"]