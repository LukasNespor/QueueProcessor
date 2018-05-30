FROM microsoft/dotnet:2.0-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.0-sdk AS build
WORKDIR /src
COPY QueueProcessor/QueueProcessor.csproj QueueProcessor/
RUN dotnet restore QueueProcessor/QueueProcessor.csproj
COPY . .
WORKDIR /src/QueueProcessor
RUN dotnet build QueueProcessor.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish QueueProcessor.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "QueueProcessor.dll"]
