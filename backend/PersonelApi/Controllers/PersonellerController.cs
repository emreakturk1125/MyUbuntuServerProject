using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PersonelApi.Data;
using PersonelApi.Models;
using PersonelApi.Services;

namespace PersonelApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonellerController : ControllerBase
    {
        private const long MaxCvFileSize = 10 * 1024 * 1024; // 10 MB
        private static readonly string[] AllowedCvExtensions = { ".pdf", ".doc", ".docx" };

        private readonly AppDbContext _context;
        private readonly IStorageService _storageService;
        private readonly ILogger<PersonellerController> _logger;

        public PersonellerController(
            AppDbContext context,
            IStorageService storageService,
            ILogger<PersonellerController> logger)
        {
            _context = context;
            _storageService = storageService;
            _logger = logger;
        }

        // GET: api/personeller
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Personel>>> GetPersoneller()
        {
            try
            {
                return await _context.Personeller
                    .OrderByDescending(p => p.Id)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel listesi getirilirken hata oluştu.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Personel listesi alınırken sunucu hatası oluştu." });
            }
        }

        // GET: api/personeller/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Personel>> GetPersonel(int id)
        {
            try
            {
                var personel = await _context.Personeller.FindAsync(id);

                if (personel == null)
                {
                    return NotFound(new { message = "Personel bulunamadı." });
                }

                return personel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel detayı getirilirken hata oluştu. PersonelId: {PersonelId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Personel bilgisi alınırken sunucu hatası oluştu." });
            }
        }

        // POST: api/personeller
        [HttpPost]
        public async Task<ActionResult<Personel>> CreatePersonel(Personel personel)
        {
            try
            {
                _context.Personeller.Add(personel);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetPersonel), new { id = personel.Id }, personel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Yeni personel oluşturulurken hata oluştu.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Personel eklenirken sunucu hatası oluştu." });
            }
        }

        // PUT: api/personeller/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePersonel(int id, Personel personel)
        {
            if (id != personel.Id)
            {
                return BadRequest(new { message = "Gönderilen ID ile personel ID eşleşmiyor." });
            }

            try
            {
                var mevcutPersonel = await _context.Personeller.FindAsync(id);
                if (mevcutPersonel == null)
                {
                    return NotFound(new { message = "Güncellenecek personel bulunamadı." });
                }

                mevcutPersonel.Ad = personel.Ad;
                mevcutPersonel.Soyad = personel.Soyad;
                mevcutPersonel.Departman = personel.Departman;
                mevcutPersonel.Pozisyon = personel.Pozisyon;
                mevcutPersonel.Email = personel.Email;
                mevcutPersonel.Telefon = personel.Telefon;
                mevcutPersonel.IseGirisTarihi = personel.IseGirisTarihi;

                if (!string.IsNullOrWhiteSpace(personel.CvFileName))
                {
                    mevcutPersonel.CvFileName = personel.CvFileName;
                }

                if (!string.IsNullOrWhiteSpace(personel.CvOriginalFileName))
                {
                    mevcutPersonel.CvOriginalFileName = personel.CvOriginalFileName;
                }

                await _context.SaveChangesAsync();

                return Ok(mevcutPersonel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel güncellenirken hata oluştu. PersonelId: {PersonelId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Personel güncellenirken sunucu hatası oluştu." });
            }
        }

        // DELETE: api/personeller/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePersonel(int id)
        {
            try
            {
                var personel = await _context.Personeller.FindAsync(id);
                if (personel == null)
                {
                    return NotFound(new { message = "Silinecek personel bulunamadı." });
                }

                if (!string.IsNullOrWhiteSpace(personel.CvFileName))
                {
                    await _storageService.DeleteFileAsync(personel.CvFileName);
                }

                _context.Personeller.Remove(personel);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel silinirken hata oluştu. PersonelId: {PersonelId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Personel silinirken sunucu hatası oluştu." });
            }
        }

        // POST: api/personeller/5/cv
        [HttpPost("{id}/cv")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(MaxCvFileSize)]
        public async Task<IActionResult> UploadCv(int id, IFormFile file)
        {
            try
            {
                if (file == null && Request.HasFormContentType && Request.Form.Files.Count > 0)
                {
                    file = Request.Form.Files[0];
                }

                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "Lütfen yüklenecek bir CV dosyası seçiniz." });
                }

                if (file.Length > MaxCvFileSize)
                {
                    return BadRequest(new { message = "CV dosya boyutu en fazla 10 MB olabilir." });
                }

                var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
                if (string.IsNullOrWhiteSpace(extension) || !AllowedCvExtensions.Contains(extension))
                {
                    return BadRequest(new { message = "Geçersiz dosya formatı. Sadece .pdf, .doc ve .docx uzantılı dosyalar yüklenebilir." });
                }

                var personel = await _context.Personeller.FindAsync(id);
                if (personel == null)
                {
                    return NotFound(new { message = "Personel bulunamadı." });
                }

                // Eski bir CV varsa önce MinIO'dan sil
                if (!string.IsNullOrWhiteSpace(personel.CvFileName))
                {
                    var oldFileDeleted = await _storageService.DeleteFileAsync(personel.CvFileName);
                    if (!oldFileDeleted)
                    {
                        _logger.LogWarning(
                            "Personelin eski CV dosyası MinIO'dan silinemedi, yeni dosya yükleme işlemine devam ediliyor. PersonelId: {PersonelId}, OldCvFileName: {OldCvFileName}",
                            id, personel.CvFileName);
                    }
                }

                // Yeni CV dosyasını MinIO'ya yükle
                var uploadedFileName = await _storageService.UploadFileAsync(file, "cv");

                personel.CvFileName = uploadedFileName;
                personel.CvOriginalFileName = Path.GetFileName(file.FileName);

                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Personel CV dosyası başarıyla yüklendi ve güncellendi. PersonelId: {PersonelId}, CvFileName: {CvFileName}, CvOriginalFileName: {CvOriginalFileName}",
                    id, personel.CvFileName, personel.CvOriginalFileName);

                return Ok(personel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel CV yükleme işlemi sırasında hata oluştu. PersonelId: {PersonelId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "CV dosyası yüklenirken bir hata oluştu." });
            }
        }

        // GET: api/personeller/5/cv/download
        [HttpGet("{id}/cv/download")]
        public async Task<IActionResult> DownloadCv(int id)
        {
            try
            {
                var personel = await _context.Personeller.FindAsync(id);
                if (personel == null)
                {
                    return NotFound(new { message = "Personel bulunamadı." });
                }

                if (string.IsNullOrWhiteSpace(personel.CvFileName))
                {
                    return NotFound(new { message = "Bu personele ait yüklü bir CV dosyası bulunamadı." });
                }

                var fileStream = await _storageService.GetFileAsync(personel.CvFileName);
                var downloadFileName = !string.IsNullOrWhiteSpace(personel.CvOriginalFileName)
                    ? personel.CvOriginalFileName
                    : Path.GetFileName(personel.CvFileName);

                return File(fileStream, "application/octet-stream", downloadFileName);
            }
            catch (FileNotFoundException fnfEx)
            {
                _logger.LogWarning(fnfEx, "MinIO üzerinde personel CV dosyası bulunamadı. PersonelId: {PersonelId}", id);
                return NotFound(new { message = "CV dosyası depolama alanında (MinIO) bulunamadı." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel CV indirme işlemi sırasında hata oluştu. PersonelId: {PersonelId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "CV dosyası indirilirken bir hata oluştu." });
            }
        }

        // DELETE: api/personeller/5/cv
        [HttpDelete("{id}/cv")]
        public async Task<IActionResult> DeleteCv(int id)
        {
            try
            {
                var personel = await _context.Personeller.FindAsync(id);
                if (personel == null)
                {
                    return NotFound(new { message = "Personel bulunamadı." });
                }

                if (string.IsNullOrWhiteSpace(personel.CvFileName))
                {
                    return NotFound(new { message = "Bu personele ait silinecek bir CV dosyası bulunamadı." });
                }

                var fileToDelete = personel.CvFileName;
                var isDeleted = await _storageService.DeleteFileAsync(fileToDelete);
                if (!isDeleted)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { message = "CV dosyası MinIO üzerinden silinirken bir hata oluştu." });
                }

                personel.CvFileName = null;
                personel.CvOriginalFileName = null;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Personel CV dosyası başarıyla silindi. PersonelId: {PersonelId}, DeletedFile: {DeletedFile}", id, fileToDelete);

                return Ok(new { message = "CV dosyası başarıyla silindi.", personel });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel CV silme işlemi sırasında hata oluştu. PersonelId: {PersonelId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "CV dosyası silinirken bir hata oluştu." });
            }
        }
    }
}
