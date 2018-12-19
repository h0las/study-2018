FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["TelegramNews.Database/TelegramNews.Database.csproj", "TelegramNews.Database/"]
RUN dotnet restore "TelegramNews.Database/TelegramNews.Database.csproj"
COPY . .
WORKDIR "/src/TelegramNews.Database"
RUN dotnet build "TelegramNews.Database.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "TelegramNews.Database.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "TelegramNews.Database.dll"]