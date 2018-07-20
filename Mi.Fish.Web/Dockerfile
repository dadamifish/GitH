FROM microsoft/dotnet:2.1-aspnetcore-runtime-nanoserver-sac2016 AS base
WORKDIR /app
EXPOSE 12501
EXPOSE 44326

FROM microsoft/dotnet:2.1-sdk-nanoserver-sac2016 AS build
WORKDIR /src
COPY Mi.Fish.Web/Mi.Fish.Web.csproj Mi.Fish.Web/
RUN dotnet restore Mi.Fish.Web/Mi.Fish.Web.csproj
COPY . .
WORKDIR /src/Mi.Fish.Web
RUN dotnet build Mi.Fish.Web.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Mi.Fish.Web.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Mi.Fish.Web.dll"]
