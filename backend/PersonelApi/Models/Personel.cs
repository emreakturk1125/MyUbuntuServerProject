#nullable enable
using System;
using System.ComponentModel.DataAnnotations;

namespace PersonelApi.Models
{
    public class Personel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        [MaxLength(100)]
        public string Ad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        [MaxLength(100)]
        public string Soyad { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Departman { get; set; }

        [MaxLength(100)]
        public string? Pozisyon { get; set; }

        [MaxLength(150)]
        public string? Email { get; set; }

        [MaxLength(30)]
        public string? Telefon { get; set; }

        public DateTime IseGirisTarihi { get; set; } = DateTime.Now;

        [MaxLength(255)]
        public string? CvFileName { get; set; }

        [MaxLength(255)]
        public string? CvOriginalFileName { get; set; }
    }
}
