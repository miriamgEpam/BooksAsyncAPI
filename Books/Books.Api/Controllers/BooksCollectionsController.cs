﻿using AutoMapper;
using Books.Api.Filters;
using Books.Api.Models;
using Books.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Books.Api.Controllers
{
    [Route("api/bookcollections")]
    [ApiController]
    [BooksResultFilter]
    public class BooksCollectionsController : ControllerBase
    {
        private readonly IBooksRepository _booksRepository;
        private readonly IMapper _mapper;

        public BooksCollectionsController(IBooksRepository booksRepository, IMapper mapper)
        {
            _booksRepository = booksRepository ?? throw new ArgumentNullException(nameof(booksRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpPost]
              public async Task<IActionResult> CreateBookCollection([FromBody] IEnumerable<BookForCreation> bookCollection)
        {
            var bookEntities = _mapper.Map<IEnumerable<Entities.Book>>(bookCollection);
            foreach (var bookEntity in bookEntities)
            {
                _booksRepository.AddBook(bookEntity);

            }
            await _booksRepository.SaveChangesAsync();
            var booksToReturn = await _booksRepository.GetBooksAsync(
                bookEntities.Select(b => b.Id).ToList());

            var bookIds = string.Join(",", booksToReturn.Select(a => a.Id));

            return CreatedAtRoute("GetBookCollection", new { bookIds }, booksToReturn);

        }

        [HttpGet("({bookIds})", Name = "GetBookCollection")]

        public async Task<IActionResult> GetBookCollection(
            [ModelBinder(BinderType =typeof(ArrayModelBinder))]
            IEnumerable<Guid> bookIds)
        {
            var bookEntites = await _booksRepository.GetBooksAsync(bookIds);
            if (bookIds.Count() != bookEntites.Count())
            {
                return NotFound(); 
            }
            return Ok(bookEntites);
        }


    }
}
