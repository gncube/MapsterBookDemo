using MapsterBookDemo.Application.Interfaces;
using MapsterBookDemo.Domain.Models;
using MapsterBookDemo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MapsterBookDemo.Infrastructure.Repositories;
public class BookRepository : IRepository<Book, int>
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<BookRepository> _logger;

    public BookRepository(AppDbContext dbContext, ILogger<BookRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
        _dbContext.Database.EnsureCreatedAsync();
    }

    public async Task<IEnumerable<Book>> GetAllAsync()
    {
        return await _dbContext.Books.ToListAsync();
    }

    public async Task<Book> GetAsync(int id)
    {
        var foundBook = await _dbContext.Books.FirstOrDefaultAsync(x => x.Id == id);
        if (foundBook == null)
        {
            _logger.LogInformation($"Book not found. {id}");
            return null;
        }
        return foundBook;
    }

    public async Task<Book> AddAsync(Book book)
    {
        var bookExists = await _dbContext.Books.AnyAsync(x => x.Id == book.Id);
        if (bookExists)
        {
            _logger.LogInformation($"Book with id {book.Id} already exists.");
        }

        var addedBook = await _dbContext.Books.AddAsync(book);
        await _dbContext.SaveChangesAsync();
        return addedBook.Entity;
    }

    public async Task<bool> UpdateAsync(Book book)
    {
        var bookExists = _dbContext.Books.Any(x => x.Id == book.Id);
        if (!bookExists)
        {
            _logger.LogInformation($"Book with id {book.Id} does NOT exists.");
        }

        _dbContext.Books.Update(book);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public Task<bool> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}
