using Books.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Books.Api.Controllers
{
    [Route("api/synchronousbooks")]
    [ApiController]
    public class SynchronousBookController:ControllerBase
    {
        private IBooksRepository _booksRepository;

        public SynchronousBookController(IBooksRepository booksRepository)
        {
            _booksRepository = booksRepository ?? throw new ArgumentNullException(nameof(booksRepository)); 
        }

        [HttpGet]
        public IActionResult GetBooks()
        {
            var bookEntites = _booksRepository.GetBooks();
            return Ok(bookEntites); 
        }
    }
}
