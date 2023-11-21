using MapsterBookDemo.Application.Interfaces;
using MapsterBookDemo.Domain.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace MapsterBookDemo;

public class BookController
{
    private readonly ILogger _logger;
    private readonly IRepository<Book, int> _repo;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public BookController(ILoggerFactory loggerFactory, IRepository<Book, int> repo, JsonSerializerOptions jsonSerializerOptions)
    {
        _logger = loggerFactory.CreateLogger<BookController>();
        _repo = repo;
        _jsonSerializerOptions = jsonSerializerOptions;
    }

    [Function("BookController")]
    public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        var books = await _repo.GetAllAsync();
        var booksJson = JsonSerializer.Serialize(books, _jsonSerializerOptions);

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json; charset=utf-8");

        response.WriteString(booksJson);
        return response;
    }
}
