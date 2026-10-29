# 📋 Personel Kayıt Sistemi - Issue Bazlı Geliştirme Planı

Bu doküman, uçtan uca geliştirilecek **Personel Kayıt Sistemi** uygulamasının adım adım yapılacak işlerini (Issue/Task formatında) içerir.

---

## 🏗️ Mimari ve Teknoloji Yığını
- **Backend:** .NET 5.0 Web API (`net5.0`)
- **ORM & Veritabanı:** Entity Framework Core 5.0 (Code First) & PostgreSQL (`Npgsql.EntityFrameworkCore.PostgreSQL`)
- **Frontend:** React
- **Klasör Yapısı (`c:\Githubproject\ServerProject`):**
  - `/backend` -> .NET 5.0 Web API Projesi
  - `/frontend` -> React Arayüz Projesi

---

## 📌 Issue Listesi (Adım Adım Görevler)

### 🔹 ISSUE-1: Proje Dizin Yapısının ve .NET 5.0 Web API İskeletinin Oluşturulması (✅ TAMAMLANDI)
- **Amaç:** Backend projesinin temelinin atılması ve gerekli NuGet paketlerinin kurulması.
- **Yapılacak İşler:**
  - [x] `backend/PersonelApi` klasöründe .NET 5.0 (`net5.0`) Web API projesinin oluşturulması.
  - [x] EF Core 5.x ve PostgreSQL bağlantısı için gerekli NuGet paketlerinin (`Npgsql.EntityFrameworkCore.PostgreSQL`, `Microsoft.EntityFrameworkCore.Design`, `Microsoft.EntityFrameworkCore.Tools` v5.x) yüklenmesi.
  - [x] React uygulamasının API ile haberleşebilmesi için `Startup.cs` içerisine **CORS** (Cross-Origin Resource Sharing) politikasının eklenmesi.

---

### 🔹 ISSUE-2: Veri Modeli (Entity), DbContext ve PostgreSQL Code-First Kurulumu (✅ TAMAMLANDI)
- **Amaç:** Veritabanı tablosuna karşılık gelecek modelin oluşturulması ve PostgreSQL bağlantısının yapılandırılması.
- **Yapılacak İşler:**
  - [x] `Models/Personel.cs` sınıfının oluşturulması:
    - `Id` (int, Primary Key)
    - `Ad` (string, Zorunlu)
    - `Soyad` (string, Zorunlu)
    - `Departman` (string)
    - `Pozisyon` (string)
    - `Email` (string)
    - `Telefon` (string)
    - `IseGirisTarihi` (DateTime)
  - [x] `Data/AppDbContext.cs` sınıfının oluşturulması ve `DbSet<Personel>` tanımlanması.
  - [x] `appsettings.json` dosyasına PostgreSQL Connection String (`DefaultConnection`) eklenmesi ve `Startup.cs` içinde `UseNpgsql` ile kaydedilmesi.
  - [x] İlk EF Core Migration'ının (`InitialCreate`) alınması ve veritabanının (`dotnet ef database update` veya başlangıçta otomatik migrate) oluşturulması.

---

### 🔹 ISSUE-3: Personel CRUD Web API Endpoint'lerinin Yazılması (✅ TAMAMLANDI)
- **Amaç:** Personel listeleme, ekleme, güncelleme ve silme işlemlerini yapacak REST API uçlarının geliştirilmesi.
- **Yapılacak İşler:**
  - [x] `Controllers/PersonellerController.cs` oluşturulması.
  - [x] `GET /api/personeller` -> Tüm personelleri listeleme.
  - [x] `GET /api/personeller/{id}` -> Tek bir personel detayını getirme.
  - [x] `POST /api/personeller` -> Yeni personel ekleme.
  - [x] `PUT /api/personeller/{id}` -> Mevcut personel bilgilerini güncelleme.
  - [x] `DELETE /api/personeller/{id}` -> Personel kaydını silme.
  - [x] Swagger üzerinden veya örnek isteklerle endpoint'lerin doğrulanması.

---

### 🔹 ISSUE-4: React Uygulamasının Kurulumu ve API Servis Katmanı (✅ TAMAMLANDI)
- **Amaç:** Frontend projesinin oluşturulması ve Backend API ile iletişim kuracak servisin yazılması.
- **Yapılacak İşler:**
  - [x] `frontend` klasöründe React uygulamasının oluşturulması.
  - [x] Temiz ve modern bir görünüm için temel stil yapısının hazırlanması.
  - [x] `src/services/personelService.js` (veya `.ts`) dosyasının oluşturularak `GET`, `POST`, `PUT`, `DELETE` API çağrılarının tanımlanması.

---

### 🔹 ISSUE-5: Tek Ekran Personel Yönetim Arayüzünün (Liste, Ekle, Güncelle, Sil) Geliştirilmesi (✅ TAMAMLANDI)
- **Amaç:** Uygulama açıldığında doğrudan personel listesinin görünmesi ve aynı ekran üzerinden tüm CRUD işlemlerinin yapılabilmesi.
- **Yapılacak İşler:**
  - [x] Ana ekranda **Personel Tablosu** bileşeninin oluşturulması (Ad, Soyad, Departman, Pozisyon, E-posta, Telefon ve İşlem Butonları).
  - [x] Aynı ekranda (üst kısımda veya açılır form/modal olarak) **Personel Ekleme / Güncelleme Formu** oluşturulması.
  - [x] Tablodaki **"Düzenle"** butonuna basıldığında seçili personelin bilgilerinin forma doldurulması ve güncellenmesi.
  - [x] Tablodaki **"Sil"** butonuna basıldığında onay alınarak personelin silinmesi ve listenin anında güncellenmesi.
  - [x] Yükleniyor (Loading) ve hata/başarı bildirim durumlarının eklenmesi.

---

### 🔹 ISSUE-6: Uçtan Uca Test ve Çalıştırma Kılavuzu (✅ TAMAMLANDI)
- **Amaç:** Hem Backend hem Frontend'in birlikte sorunsuz çalıştığının doğrulanması.
- **Yapılacak İşler:**
  - [x] PostgreSQL bağlantısının ve tablo işlemlerinin uçtan uca test edilmesi.
  - [x] Projeyi tek komutla veya adım adım nasıl ayağa kaldıracağınızı anlatan Türkçe `README.md` dokümanının oluşturulması.
