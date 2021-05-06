FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/Api/OpenPOS.Identity/OpenPOS.Identity.csproj", "OpenPOS.Identity/"]
RUN dotnet restore "src/Api/OpenPOS.Identity/OpenPOS.Identity.csproj"
COPY . .
WORKDIR "/src/OpenPOS.Identity"
RUN dotnet build "OpenPOS.Identity.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OpenPOS.Identity.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OpenPOS.Identity.dll"]
