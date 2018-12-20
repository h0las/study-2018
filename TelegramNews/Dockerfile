FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["TelegramNews/TelegramNews.csproj", "TelegramNews/"]
RUN dotnet restore "TelegramNews/TelegramNews.csproj"
COPY . .
WORKDIR "/src/TelegramNews"
RUN dotnet build "TelegramNews.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "TelegramNews.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "TelegramNews.dll"]