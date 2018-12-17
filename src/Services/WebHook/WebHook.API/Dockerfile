FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["src/Services/WebHook/WebHook.API/WebHook.API.csproj", "src/Services/WebHook/WebHook.API/"]
RUN dotnet restore "src/Services/WebHook/WebHook.API/WebHook.API.csproj"
COPY . .
WORKDIR "/src/src/Services/WebHook/WebHook.API"
RUN dotnet build "WebHook.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "WebHook.API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WebHook.API.dll"]