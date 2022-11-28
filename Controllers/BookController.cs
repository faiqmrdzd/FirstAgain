using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.DAL;
using WebAPI.DTOs.Book;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private AppDbContext _context;

        public BookController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var book = _context.Books.Where(b=>b.IsDeleted==false && b.Id==id).FirstOrDefault();
            if (book == null) return StatusCode(404, new { errorCode = 1045, message = "This book could not found" });
            return Ok(book);
        }
        [HttpGet("")]
        public IActionResult GetAll()
        {
            var books = _context.Books.Where(b => b.IsDeleted == false).ToList();
            if (books == null) return StatusCode(StatusCodes.Status404NotFound, new { errorCode = 1046, message = "This book could not found" });
            return Ok(books);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateBookDto bookDto)
        {
            if(!ModelState.IsValid) return StatusCode(StatusCodes.Status400BadRequest, new { errorCode = 1087, message = "Model is Invalid" });
            var book = new Book
            {
                Name=bookDto.Name,
                Price=bookDto.Price,
                IsDeleted=false
            };
            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(UpdateBookDto bookDto,int id)
        {
            Book dbBook = await _context.Books.Where(b => b.IsDeleted == false && b.Id == id).FirstOrDefaultAsync();
            if (dbBook == null) return NotFound();
            if (bookDto.Name == null)
            {
                bookDto.Name = dbBook.Name;
            }
            bool IsExist = dbBook.Name.Trim().ToLower() == bookDto.Name.Trim().ToLower();
            if (IsExist != false)
            {
                return StatusCode(StatusCodes.Status409Conflict, new { errorCode = 2087, message = "Multiple Name" });

            }
            if (!IsExist)
            {
                dbBook.Name=bookDto.Name;
            }
            if (bookDto.Price != 0)
            {
                dbBook.Price = bookDto.Price;
            }
            _context.Books.Update(dbBook);
            await _context.SaveChangesAsync();
            return Ok(dbBook);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var dbBook= _context.Books.Where(b => b.IsDeleted == false && b.Id == id).FirstOrDefault();
            if (dbBook == null) return NotFound();
            dbBook.IsDeleted = true;
            _context.Books.Update(dbBook);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpPatch("{id}")]
        public IActionResult Patch(int id,[FromBody] JsonPatchDocument<Book> patchDoc)
        {
            if (patchDoc != null)
            {
                var dbBook = _context.Books.Where(b => b.IsDeleted == false && b.Id == id).FirstOrDefault();
                if (dbBook == null) return NotFound();
                patchDoc.ApplyTo(dbBook, ModelState);

                if (!ModelState.IsValid) return BadRequest();

                return NoContent();
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

    }
}
