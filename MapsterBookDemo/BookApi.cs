using Mapster;
using MapsterBookDemo.Application.DTOs;
using MapsterBookDemo.Domain.Models;
using MapsterBookDemo.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Net;

namespace MapsterBookDemo;


public class BookApi
{
    private readonly ILogger _logger;
    private readonly AppDbContext _context;


    public BookApi(ILoggerFactory loggerFactory, AppDbContext context)
    {
        _logger = loggerFactory.CreateLogger<BookApi>();
        _context = context;
    }

    [Function(nameof(GetById))]
    [OpenApiOperation(operationId: "GetById", tags: new[] { "Book" }, Summary = "GetBookById summary", Description = "GetBookById description.", Visibility = OpenApiVisibilityType.Important)]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
    [OpenApiParameter(name: "bookId", In = ParameterLocation.Query, Required = true, Type = typeof(int), Description = "The **BookId** parameter")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(BookDto), Description = "The OK response")]
    public async Task<IActionResult> GetById([HttpTrigger(AuthorizationLevel.Function, "get", Route = "books/{bookId:int}")] HttpRequestData req, int bookId)
    {
        _logger.LogInformation("---> {FunctionName} function processed a request.", nameof(GetById));

        try
        {
            var bookDto = await _context.Books
                .Where(b => b.Id == bookId)
                .ProjectToType<BookDto>()
                .FirstOrDefaultAsync();

            if (bookDto == null)
            {
                _logger.LogInformation($"---> Book not found. {bookId}");
                return new NotFoundResult();
            }

            _logger.LogInformation($"---> Found book. {bookDto}");

            return new OkObjectResult(bookDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting book");
            return new BadRequestResult();
        }
    }

    [Function(nameof(GetAll))]
    [OpenApiOperation(operationId: "GetAll", tags: new[] { "Book" }, Summary = "GetAllBooks summary", Description = "GetAllBooks description.", Visibility = OpenApiVisibilityType.Important)]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<Book>), Description = "The OK response")]
    public async Task<IActionResult> GetAll([HttpTrigger(AuthorizationLevel.Function, "get", Route = "books")] HttpRequestData req)
    {
        _logger.LogInformation("---> {FunctionName} function processed a request.", nameof(GetAll));

        try
        {
            var bookResult = await _context.Books
                .ProjectToType<BookDto>()
                .ToListAsync();

            if (bookResult.Count() == 0)
            {
                return new NotFoundResult();
            }

            _logger.LogInformation($"---> Found {bookResult.Count()} books");

            // TODO: Add BookReadDto and Mapster it 
            return new OkObjectResult(bookResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting books");
            return new BadRequestResult();
        }
    }

    [Function(nameof(AddBook))]
    [OpenApiOperation(operationId: "AddBook", tags: new[] { "Book" }, Summary = "Add Book summary", Description = "Add Book description.", Visibility = OpenApiVisibilityType.Important)]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(BookDto), Required = true, Description = "Add bookDto.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, contentType: "application/json", bodyType: typeof(Book), Description = "The OK response")]
    public async Task<IActionResult> AddBook([HttpTrigger(AuthorizationLevel.Function, "post", Route = "books")] HttpRequestData req)
    {
        _logger.LogInformation("---> {FunctionName} function processed a request.", nameof(AddBook));

        try
        {
            var bookDto = await req.ReadFromJsonAsync<BookDto>();
            var book = bookDto.Adapt<Book>();

            _logger.LogInformation($"---> Adding new book. {book}");

            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();

            //return new CreatedResult($"/api/books/{book.Id}", book);
            //return new CreatedResult($"/api/books/{book.Id}", book.Adapt<BookDto>());
            return new OkObjectResult(book.Adapt<BookDto>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding book");
            return new BadRequestResult();
        }
    }

    [Function(nameof(Update))]
    [OpenApiOperation(operationId: "Update", tags: new[] { "Book" }, Summary = "UpdateBook summary", Description = "UpdateBook description.", Visibility = OpenApiVisibilityType.Important)]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
    [OpenApiParameter(name: "BookId", In = ParameterLocation.Query, Required = true, Type = typeof(int), Description = "The **BookId** parameter")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, contentType: "application/json", bodyType: typeof(Book), Description = "The OK response")]
    public async Task<IActionResult> Update([HttpTrigger(AuthorizationLevel.Function, "put", Route = "books/{bookId:int}")] HttpRequestData req, int bookId)
    {
        _logger.LogInformation("---> {FunctionName} function processed a request.", nameof(Update));

        try
        {
            var bookDto = await req.ReadFromJsonAsync<BookDto>();
            if (bookDto == null)
            {
                return new BadRequestResult();
            }

            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == bookId);

            if (book == null)
            {
                return new NotFoundResult();
            }
            _logger.LogInformation($"---> Found book. {book}");

            bookDto.ToModel(book);

            await _context.SaveChangesAsync();

            return new OkObjectResult(book.Adapt<BookDto>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting book");
            return new BadRequestResult();
        }
    }
}
