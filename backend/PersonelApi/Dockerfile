# 1. Aşama: Build (Derleme)
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

# Proje dosyasını kopyala ve bağımlılıkları yükle (Cache optimizasyonu)
COPY ["PersonelApi.csproj", "./"]
RUN dotnet restore "PersonelApi.csproj"

# Tüm kaynak kodları kopyala ve projeyi publish et
COPY . .
RUN dotnet publish "PersonelApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 2. Aşama: Runtime (Çalıştırma)
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS final
WORKDIR /app

# Uygulamanın çalışacağı port ve ortam değişkenleri
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Development

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PersonelApi.dll"]

