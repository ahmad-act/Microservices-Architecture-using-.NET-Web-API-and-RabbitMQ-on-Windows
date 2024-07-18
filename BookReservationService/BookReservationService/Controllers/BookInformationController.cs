using AutoMapper;
using BookReservationService.BusinessLayer;
using BookReservationService.DTOs;
using BookReservationService.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Swashbuckle.AspNetCore.Annotations;

namespace BookReservationService.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    [Produces("application/json")]
    public class BookReservationController : Controller
    {
        private readonly ILogger<object> _logger;
        private readonly IBookReservationBL _bookInfoBL;


        public BookReservationController(ILogger<object> logger, IBookReservationBL bookBL)
        {
            _logger = logger;
            _bookInfoBL = bookBL;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get all book informations")]
        [SwaggerResponse(200, "Successfully retrieved book informations", typeof(List<BookReservation>))]
        [SwaggerResponse(404, "Book Informations not found")]
        [SwaggerResponse(500, "Internal Server Error")]
        public async Task<IActionResult> GetBookReservations()
        {
            Log.Information("Entered: GetBookReservations()");

            try
            {
                List<BookReservationDisplayDto>? bookInformationsDisplayDto = await _bookInfoBL.GetBookReservations();

                if (bookInformationsDisplayDto == null || bookInformationsDisplayDto.Count == 0)
                {
                    return NotFound("Book Informations not found");
                }

                return Ok(bookInformationsDisplayDto);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to complete GetBookReservations(). \nHTTP status code: 500 \nError: {Message}", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get a book information by ID")]
        [SwaggerResponse(200, "Successfully retrieved the book information", typeof(BookReservation))]
        [SwaggerResponse(404, "Book not found")]
        [SwaggerResponse(500, "Internal Server Error")]
        public async Task<IActionResult> GetBookReservation(int id)
        {
            Log.Information("Entered: GetBookReservation(int id)");

            try
            {
                var bookInformationDisplayDto = await _bookInfoBL.GetBookReservation(id);

                if (bookInformationDisplayDto == null)
                {
                    return NotFound("Book not found");
                }

                return Ok(bookInformationDisplayDto);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to complete GetBookReservation(). \nHTTP status code: 500 \nError: {Message}", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create a new book information")]
        [SwaggerResponse(201, "Successfully created the book information", typeof(BookReservation))]
        [SwaggerResponse(400, "Invalid book data")]
        [SwaggerResponse(500, "Internal server error occurred")]
        public async Task<IActionResult> CreateBookReservations([FromBody] BookReservationUpdateDto bookInformationUpdateDto)
        {
            Log.Information("Entered: CreateBookReservations([FromBody] BookReservationUpdateDto bookInformationUpdateDto)");

            try
            {
                if (bookInformationUpdateDto == null)
                {
                    return BadRequest("Invalid book data");
                }

                BookReservationDisplayDto? createdBookReservation = await _bookInfoBL.CreateBookReservation(bookInformationUpdateDto);

                if (createdBookReservation == null)
                {
                    return BadRequest("Book information is not created.");
                }

                return Ok(createdBookReservation);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to complete CreateBookReservations(). \nHTTP status code: 500 \nError: {Message}", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Update a book information by ID")]
        [SwaggerResponse(200, "Successfully updated the book information")]
        [SwaggerResponse(404, "Book not found")]
        [SwaggerResponse(400, "Invalid book information data")]
        [SwaggerResponse(500, "Internal Server Error")]
        public async Task<IActionResult> UpdateBookReservations(int id, [FromBody] BookReservationUpdateDto bookInformationUpdateDto)
        {
            Log.Information("Entered: UpdateBookReservations(int id, [FromBody] BookReservationUpdateDto bookInformationUpdateDto)");

            try
            {
                if (bookInformationUpdateDto == null)
                {
                    return BadRequest("Invalid book data");
                }

                var existingBook = await _bookInfoBL.UpdateBookReservation(id, bookInformationUpdateDto);

                if (existingBook == null)
                {
                    return NotFound("Book not found");
                }

                return Ok(existingBook);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to complete GetBookReservations(). \nHTTP status code: 500 \nError: {Message}", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete a book information by ID")]
        [SwaggerResponse(204, "Successfully deleted the book information", typeof(BookReservation))]
        [SwaggerResponse(404, "Book not found")]
        [SwaggerResponse(500, "Internal Server Error")]
        public async Task<IActionResult> DeleteBookReservations(int id)
        {
            Log.Information("Entered: DeleteBookReservations(int id)");

            try
            {
                string msg = await _bookInfoBL.DeleteBookReservation(id);

                if (!string.IsNullOrEmpty(msg))
                {
                    return NotFound(msg);
                }

                return Ok("Successfully deleted the book information");
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to complete DeleteBookReservations(). \nHTTP status code: 500 \nError: {Message}", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search book information by Title")]
        [SwaggerResponse(200, "Successfully searched the book informations", typeof(List<BookReservation>))]
        [SwaggerResponse(204, "The search did not yield any results")]
        [SwaggerResponse(400, "Please enter search data")]
        [SwaggerResponse(404, "No book informations found matching the search term")]
        [SwaggerResponse(500, "Internal Server Error")]
        public async Task<IActionResult> SearchBookReservations(string? searchTerm)
        {
            Log.Information("Entered: SearchBookReservations(string? searchTerm)");

            try
            {

                if (string.IsNullOrEmpty(searchTerm))
                {
                    return BadRequest("Please enter search data");
                }

                var bookInformations = await _bookInfoBL.SearchBookReservations(searchTerm);

                if (bookInformations == null || bookInformations.Count == 0)
                {
                    return NotFound("No book informations found matching the search term");
                }

                return Ok(bookInformations);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to complete SearchBooks(). \nHTTP status code: 500 \nError: {Message}", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
    }
}
