# ---------- Build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Аввал танҳо .csproj-ҳоро нусха мекунем, то restore cache шавад
COPY PharmacyPro.sln ./
COPY src/PharmacyPro.Domain/PharmacyPro.Domain.csproj src/PharmacyPro.Domain/
COPY src/PharmacyPro.Application/PharmacyPro.Application.csproj src/PharmacyPro.Application/
COPY src/PharmacyPro.Infrastructure/PharmacyPro.Infrastructure.csproj src/PharmacyPro.Infrastructure/
COPY src/PharmacyPro.API/PharmacyPro.API.csproj src/PharmacyPro.API/
RUN dotnet restore src/PharmacyPro.API/PharmacyPro.API.csproj

# Баъд тамоми код
COPY src/ src/
RUN dotnet publish src/PharmacyPro.API/PharmacyPro.API.csproj \
    -c Release -o /app/publish --no-restore

# ---------- Runtime stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .

# Бо корбари оддӣ (на root) кор мекунад
USER app

ENTRYPOINT ["dotnet", "PharmacyPro.API.dll"]
