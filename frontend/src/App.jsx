import { useState, useEffect } from "react";
import { personelService } from "./services/personelService";

const initialFormState = {
  ad: "",
  soyad: "",
  departman: "",
  pozisyon: "",
  email: "",
  telefon: "",
  iseGirisTarihi: new Date().toISOString().split("T")[0],
};

function App() {
  const [personeller, setPersoneller] = useState([]);
  const [formData, setFormData] = useState(initialFormState);
  const [duzenlenenId, setDuzenlenenId] = useState(null);
  const [yukleniyor, setYukleniyor] = useState(false);
  const [islemYapiliyor, setIslemYapiliyor] = useState(false);
  const [hataMesaji, setHataMesaji] = useState("");
  const [basariMesaji, setBasariMesaji] = useState("");

  // Sayfa açıldığında doğrudan personel listesini yükle
  useEffect(() => {
    personelleriGetir();
  }, []);

  const bildirimGoster = (mesaj, tip = "basari") => {
    if (tip === "hata") {
      setHataMesaji(mesaj);
      setBasariMesaji("");
    } else {
      setBasariMesaji(mesaj);
      setHataMesaji("");
    }
    setTimeout(() => {
      setHataMesaji("");
      setBasariMesaji("");
    }, 4000);
  };

  const personelleriGetir = async () => {
    try {
      setYukleniyor(true);
      const data = await personelService.getAll();
      setPersoneller(data);
    } catch (err) {
      bildirimGoster(
        "Personel listesi alınamadı. Backend API'nin (http://localhost:5000) çalıştığından emin olun.",
        "hata"
      );
    } finally {
      setYukleniyor(false);
    }
  };

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const formuSifirla = () => {
    setFormData(initialFormState);
    setDuzenlenenId(null);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!formData.ad.trim() || !formData.soyad.trim()) {
      bildirimGoster("Ad ve Soyad alanları zorunludur.", "hata");
      return;
    }

    try {
      setIslemYapiliyor(true);
      if (duzenlenenId) {
        // Güncelleme işlemi
        await personelService.update(duzenlenenId, formData);
        bildirimGoster("Personel bilgileri başarıyla güncellendi.");
      } else {
        // Yeni ekleme işlemi
        await personelService.create(formData);
        bildirimGoster("Yeni personel başarıyla eklendi.");
      }
      formuSifirla();
      await personelleriGetir();
    } catch (err) {
      bildirimGoster(err.message || "İşlem sırasında bir hata oluştu.", "hata");
    } finally {
      setIslemYapiliyor(false);
    }
  };

  const handleDuzenleSec = (personel) => {
    setDuzenlenenId(personel.id);
    setFormData({
      ad: personel.ad || "",
      soyad: personel.soyad || "",
      departman: personel.departman || "",
      pozisyon: personel.pozisyon || "",
      email: personel.email || "",
      telefon: personel.telefon || "",
      iseGirisTarihi: personel.iseGirisTarihi
        ? personel.iseGirisTarihi.split("T")[0]
        : new Date().toISOString().split("T")[0],
    });
    window.scrollTo({ top: 0, behavior: "smooth" });
  };

  const handleSil = async (id, adSoyad) => {
    const onay = window.confirm(
      `${adSoyad} isimli personeli silmek istediğinize emin misiniz?`
    );
    if (!onay) return;

    try {
      await personelService.remove(id);
      if (duzenlenenId === id) {
        formuSifirla();
      }
      bildirimGoster("Personel kaydı silindi.");
      await personelleriGetir();
    } catch (err) {
      bildirimGoster(err.message || "Silme işlemi başarısız oldu.", "hata");
    }
  };

  return (
    <div className="container">
      {/* Üst Başlık */}
      <header className="header">
        <h1>👥 Personel Kayıt Sistemi</h1>
        <span className="badge">Toplam: {personeller.length} Personel</span>
      </header>

      {/* Bildirim Mesajları */}
      {hataMesaji && <div className="alert alert-error">{hataMesaji}</div>}
      {basariMesaji && <div className="alert alert-success">{basariMesaji}</div>}

      {/* Personel Ekleme / Güncelleme Formu */}
      <section className="card">
        <h2 className="card-title">
          {duzenlenenId
            ? `✏️ Personel Güncelle (#${duzenlenenId})`
            : "➕ Yeni Personel Ekle"}
        </h2>

        <form onSubmit={handleSubmit}>
          <div className="form-grid">
            <div className="form-group">
              <label htmlFor="ad">Ad *</label>
              <input
                id="ad"
                name="ad"
                type="text"
                placeholder="Örn: Ahmet"
                value={formData.ad}
                onChange={handleInputChange}
                required
              />
            </div>

            <div className="form-group">
              <label htmlFor="soyad">Soyad *</label>
              <input
                id="soyad"
                name="soyad"
                type="text"
                placeholder="Örn: Yılmaz"
                value={formData.soyad}
                onChange={handleInputChange}
                required
              />
            </div>

            <div className="form-group">
              <label htmlFor="departman">Departman</label>
              <input
                id="departman"
                name="departman"
                type="text"
                placeholder="Örn: Yazılım Geliştirme"
                value={formData.departman}
                onChange={handleInputChange}
              />
            </div>

            <div className="form-group">
              <label htmlFor="pozisyon">Pozisyon</label>
              <input
                id="pozisyon"
                name="pozisyon"
                type="text"
                placeholder="Örn: Kıdemli Uzman"
                value={formData.pozisyon}
                onChange={handleInputChange}
              />
            </div>

            <div className="form-group">
              <label htmlFor="email">E-Posta</label>
              <input
                id="email"
                name="email"
                type="email"
                placeholder="ornek@sirket.com"
                value={formData.email}
                onChange={handleInputChange}
              />
            </div>

            <div className="form-group">
              <label htmlFor="telefon">Telefon</label>
              <input
                id="telefon"
                name="telefon"
                type="text"
                placeholder="0555 000 00 00"
                value={formData.telefon}
                onChange={handleInputChange}
              />
            </div>

            <div className="form-group">
              <label htmlFor="iseGirisTarihi">İşe Giriş Tarihi</label>
              <input
                id="iseGirisTarihi"
                name="iseGirisTarihi"
                type="date"
                value={formData.iseGirisTarihi}
                onChange={handleInputChange}
              />
            </div>
          </div>

          <div className="form-actions">
            {duzenlenenId && (
              <button
                type="button"
                className="btn btn-secondary"
                onClick={formuSifirla}
                disabled={islemYapiliyor}
              >
                İptal Et
              </button>
            )}
            <button
              type="submit"
              className="btn btn-primary"
              disabled={islemYapiliyor}
            >
              {islemYapiliyor
                ? "Kaydediliyor..."
                : duzenlenenId
                ? "Güncelle"
                : "Personel Ekle"}
            </button>
          </div>
        </form>
      </section>

      {/* Doğrudan Açılan Personel Listesi Tablosu */}
      <section className="card">
        <div
          style={{
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center",
            marginBottom: "1rem",
            paddingBottom: "0.5rem",
            borderBottom: "1px solid var(--border)",
          }}
        >
          <h2 style={{ fontSize: "1.1rem", fontWeight: 600 }}>
            📋 Personel Listesi
          </h2>
          <button
            type="button"
            className="btn btn-secondary"
            onClick={personelleriGetir}
            disabled={yukleniyor}
            style={{ padding: "0.4rem 0.85rem", fontSize: "0.82rem" }}
          >
            {yukleniyor ? "Yükleniyor..." : "🔄 Yenile"}
          </button>
        </div>

        {yukleniyor ? (
          <div className="empty-state">Personel listesi yükleniyor...</div>
        ) : personeller.length === 0 ? (
          <div className="empty-state">
            Henüz kayıtlı personel bulunmuyor. Yukarıdaki formdan ilk personeli
            ekleyebilirsiniz.
          </div>
        ) : (
          <div className="table-wrapper">
            <table>
              <thead>
                <tr>
                  <th>#ID</th>
                  <th>Ad Soyad</th>
                  <th>Departman</th>
                  <th>Pozisyon</th>
                  <th>E-Posta</th>
                  <th>Telefon</th>
                  <th>İşe Giriş</th>
                  <th>İşlemler</th>
                </tr>
              </thead>
              <tbody>
                {personeller.map((p) => (
                  <tr key={p.id}>
                    <td>{p.id}</td>
                    <td style={{ fontWeight: 600 }}>
                      {p.ad} {p.soyad}
                    </td>
                    <td>{p.departman || "-"}</td>
                    <td>{p.pozisyon || "-"}</td>
                    <td>{p.email || "-"}</td>
                    <td>{p.telefon || "-"}</td>
                    <td>
                      {p.iseGirisTarihi
                        ? new Date(p.iseGirisTarihi).toLocaleDateString("tr-TR")
                        : "-"}
                    </td>
                    <td>
                      <div className="actions-cell">
                        <button
                          type="button"
                          className="btn btn-edit"
                          onClick={() => handleDuzenleSec(p)}
                        >
                          Düzenle
                        </button>
                        <button
                          type="button"
                          className="btn btn-delete"
                          onClick={() => handleSil(p.id, `${p.ad} ${p.soyad}`)}
                        >
                          Sil
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </section>
    </div>
  );
}

export default App;
