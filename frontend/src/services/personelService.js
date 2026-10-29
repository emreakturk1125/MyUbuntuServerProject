const API_BASE_URL = "http://localhost:5000/api/personeller";

export const personelService = {
  // Tüm personelleri getir
  async getAll() {
    const response = await fetch(API_BASE_URL);
    if (!response.ok) {
      throw new Error("Personel listesi alınırken bir hata oluştu.");
    }
    return await response.json();
  },

  // Tek bir personeli ID ile getir
  async getById(id) {
    const response = await fetch(`${API_BASE_URL}/${id}`);
    if (!response.ok) {
      throw new Error("Personel bilgisi alınamadı.");
    }
    return await response.json();
  },

  // Yeni personel ekle
  async create(personel) {
    const response = await fetch(API_BASE_URL, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(personel),
    });
    if (!response.ok) {
      throw new Error("Personel eklenirken bir hata oluştu.");
    }
    return await response.json();
  },

  // Mevcut personeli güncelle
  async update(id, personel) {
    const response = await fetch(`${API_BASE_URL}/${id}`, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ ...personel, id }),
    });
    if (!response.ok) {
      throw new Error("Personel güncellenirken bir hata oluştu.");
    }
    return await response.json();
  },

  // Personel sil
  async remove(id) {
    const response = await fetch(`${API_BASE_URL}/${id}`, {
      method: "DELETE",
    });
    if (!response.ok) {
      throw new Error("Personel silinirken bir hata oluştu.");
    }
    return true;
  },
};
