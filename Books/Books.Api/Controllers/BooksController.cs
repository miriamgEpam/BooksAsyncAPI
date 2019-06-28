using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Books.Api.Filters;
using Books.Api.Models;
using Books.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Books.Api.Controllers
{
    [Route("api/books")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private IBooksRepository _booksRepository;
        private readonly IMapper _mapper;

        public BooksController(IBooksRepository booksRepository, IMapper mapper)

        {
            _booksRepository = booksRepository ?? throw new ArgumentNullException(nameof(booksRepository));

            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        [BookResultFilter]
        public async Task<IActionResult> GetBooks()
        {
            var booksEntities = await _booksRepository.GetBooksAsync();
            return Ok(booksEntities);
        }

        [HttpGet]
        [BookWithCoversResultFilter]
        [Route("{id}", Name = "GetBook")]
        public async Task<IActionResult> GetBook(Guid id)
        {
            var bookEntity = await _booksRepository.GetBookAsync(id);
            if (bookEntity == null)
            {
                return NotFound();
            }

            //var bookCover = await _booksRepository.GetBookCoverAsync("dummycover");
            var bookCovers = await _booksRepository.GetBookCoversAsync(id);

            return Ok((bookEntity,  bookCovers));
        }

        [HttpPost]
        [BookResultFilter]
        public async Task<IActionResult> CreateBook([FromBody]BookForCreation book)
        {
            var bookEntity = _mapper.Map<Entities.Book>(book); 
            _booksRepository.AddBook(bookEntity);
            await _booksRepository.SaveChangesAsync();
            await _booksRepository.GetBookAsync(bookEntity.Id);
            return CreatedAtRoute("GetBook", new { id =bookEntity.Id }, bookEntity);

        }
    }
}