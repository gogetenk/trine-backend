#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
ENV ASPNETCORE_URLS=http://+:5000
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["NuGet.config", ""]
COPY ["src/WebApi/WebApi.csproj", "src/WebApi/"]
COPY ["src/Dal.AzureStorageImpl/Dal.AzureStorageImpl.csproj", "src/Dal.AzureStorageImpl/"]
COPY ["src/Assistance.Operational.Dal/Dal.csproj", "src/Assistance.Operational.Dal/"]
COPY ["src/Model/Model.csproj", "src/Model/"]
COPY ["src/Dto/Dto.csproj", "src/Dto/"]
COPY ["src/Assistance.Operational.Dal.SendGridImpl/Dal.SendGridImpl.csproj", "src/Assistance.Operational.Dal.SendGridImpl/"]
COPY ["src/Assistance.Operational.Dal.MongoImpl/Dal.MongoImpl.csproj", "src/Assistance.Operational.Dal.MongoImpl/"]
COPY ["src/Bll.Impl/Bll.Impl.csproj", "src/Bll.Impl/"]
COPY ["src/Bll/Bll.csproj", "src/Bll/"]
COPY ["src/Assistance.Operational.Dal.AzureNotificationImpl/Dal.AzureNotificationImpl.csproj", "src/Assistance.Operational.Dal.AzureNotificationImpl/"]
COPY ["src/Dal.DiscordImpl/Dal.DiscordImpl.csproj", "src/Dal.DiscordImpl/"]

RUN dotnet restore "src/WebApi/WebApi.csproj"
COPY . .
RUN dotnet build "src/WebApi/WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/WebApi/WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Assistance.Operational.WebApi.dll"]

# heroku uses the following
CMD ASPNETCORE_URLS=http://*:$PORT dotnet Assistance.Operational.WebApi.dll