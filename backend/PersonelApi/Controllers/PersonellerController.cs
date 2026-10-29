using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonelApi.Data;
using PersonelApi.Models;

namespace PersonelApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonellerController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PersonellerController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/personeller
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Personel>>> GetPersoneller()
        {
            return await _context.Personeller
                .OrderByDescending(p => p.Id)
                .ToListAsync();
        }

        // GET: api/personeller/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Personel>> GetPersonel(int id)
        {
            var personel = await _context.Personeller.FindAsync(id);

            if (personel == null)
            {
                return NotFound(new { message = "Personel bulunamadı." });
            }

            return personel;
        }

        // POST: api/personeller
        [HttpPost]
        public async Task<ActionResult<Personel>> CreatePersonel(Personel personel)
        {
            _context.Personeller.Add(personel);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPersonel), new { id = personel.Id }, personel);
        }

        // PUT: api/personeller/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePersonel(int id, Personel personel)
        {
            if (id != personel.Id)
            {
                return BadRequest(new { message = "Gönderilen ID ile personel ID eşleşmiyor." });
            }

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

            await _context.SaveChangesAsync();

            return Ok(mevcutPersonel);
        }

        // DELETE: api/personeller/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePersonel(int id)
        {
            var personel = await _context.Personeller.FindAsync(id);
            if (personel == null)
            {
                return NotFound(new { message = "Silinecek personel bulunamadı." });
            }

            _context.Personeller.Remove(personel);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
