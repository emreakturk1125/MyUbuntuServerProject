# 👥 Personel Kayıt Sistemi (React + .NET 5.0 Web API + EF Core Code-First + PostgreSQL)

Bu proje, **React** ön yüzü, **.NET Core 5.0 Web API** arka ucu ve **PostgreSQL** veritabanı (EF Core 5.0 Code-First) kullanılarak geliştirilmiş tek ekranlı uçtan uca bir **Personel Kayıt (CRUD) Sistemi** uygulamasıdır.

---

## 📂 Proje Dizin Yapısı

```text
c:\Githubproject\ServerProject
├── backend/
│   ├── PersonelApi.sln                 # Visual Studio Solution dosyası
│   └── PersonelApi/                    # .NET 5.0 Web API Projesi
│       ├── Controllers/PersonellerController.cs
│       ├── Data/AppDbContext.cs
│       ├── Migrations/                 # EF Core Code-First Migration dosyaları
│       ├── Models/Personel.cs
│       ├── Startup.cs
│       └── appsettings.json
├── frontend/                           # React (Vite) Arayüz Projesi
│   ├── src/
│   │   ├── services/personelService.js # REST API İstemci Katmanı
│   │   ├── App.jsx                     # Tek Ekran Personel Liste + Ekle/Güncelle/Sil Arayüzü
│   │   └── index.css                   # Stil Dosyası
├── docker-compose.yml                  # PostgreSQL 15 Docker yapılandırması
├── PROJE_GOREVLERI.md                  # Issue bazlı tamamlanan görev listesi
└── README.md                           # Kurulum ve Çalıştırma Kılavuzu
```

---

## 🚀 Adım Adım Çalıştırma Kılavuzu

### 1. PostgreSQL Veritabanını Başlatma (Docker)
Ana dizinde terminali açıp aşağıdaki komutu çalıştırın:
```powershell
docker compose up -d
```
- **Host:** `localhost`
- **Port:** `5432`
- **Database:** `PersonelDb`
- **Username:** `postgres`
- **Password:** `postgres`

---

### 2. Backend (.NET 5.0 Web API) Çalıştırma
`backend/PersonelApi.sln` dosyasını **Visual Studio** ile açıp **F5**'e basarak veya terminalden şu komutla çalıştırabilirsiniz:
```powershell
cd backend/PersonelApi
dotnet run
```
- **API Adresi:** `http://localhost:5000/api/personeller`
- **Swagger Arayüzü:** `http://localhost:5000/swagger`
- **Not:** Uygulama ayağa kalkarken `Program.cs` içerisindeki `context.Database.Migrate()` sayesinde veritabanı ve `Personeller` tablosu otomatik olarak oluşturulur/güncellenir. Visual Studio **Package Manager Console** üzerinden manuel çalıştırmak için `Update-Database` komutunu kullanabilirsiniz.

---

### 3. Frontend (React) Uygulamasını Çalıştırma
Yeni bir terminal açıp `frontend` klasörüne gidin ve geliştirme sunucusunu başlatın:
```powershell
cd frontend
npm install
npm run dev
```
- Tarayıcınızdan konsolda belirtilen adrese (örn. `http://localhost:5173` veya `http://localhost:5175`) giderek doğrudan personel listesini görüntüleyebilir, yeni personel ekleyebilir, mevcut kayıtları düzenleyebilir ve silebilirsiniz.
