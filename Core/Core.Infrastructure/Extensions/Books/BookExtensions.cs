using Core.Domain.Models;
using Core.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.Extensions.Books;

/// <summary>
/// Расширение на <see cref="Book"/>
/// </summary>
public static class BookExtensions
{
    /// <summary>
    /// Генерация уникального ключа для книги на основе её названия и страны издательства.
    /// </summary>
    /// <param name="book">Книга.</param>
    /// <param name="context">Контекст базы данных.</param>
    /// <param name="pendingBooks">Список книг на ожидание. Копии.</param>
    /// <returns>Уникальный ключ книги.</returns>
    public static async Task<string> GenerateBookKeyAsync(this Book book, 
        LibraryDbContext context,
        List<BookCopy> pendingBooks)
    {
        var prefix = GetBookPrefix(book);
        
        var maxNumberOfDataBase = await MaxNumberOfBooks(context, prefix);
        var maxPendingNumber = GetMaxPendingBookNumber(pendingBooks, prefix);
        var newNumber = Math.Max(maxNumberOfDataBase, maxPendingNumber);
        
        return $"{prefix}{(newNumber  + 1):D3}"; 
       
    }
    
    /// <summary>
    /// Получает префикс книги (первые буквы названия и страны)
    /// </summary>
    /// <param name="book"><see cref="Book"/>.</param>
    /// <returns></returns>
    private static string GetBookPrefix(Book book)
    {
        var titleInitial = book.Title[..1].ToUpper();
        var countryInitial = string.IsNullOrEmpty(book.Country) ? "" : book.Country.Substring(0, 1).ToUpper();
        return $"{titleInitial}{countryInitial}";
    }
    
    /// <summary>
    /// Получает максимальный числовой суффикс BookKey с заданным префиксом.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="prefix">Префикс.</param>
    /// <returns>Максимально число у книги в BookKey, формата :001</returns>
    private static async Task<int> MaxNumberOfBooks(LibraryDbContext context, string prefix)
    {
        var allKeys = await context.Books
            .Where(b => b.BookKey.StartsWith(prefix))
            .Select(b => b.BookKey.Substring(prefix.Length))
            .ToListAsync(); 

        var numbers = allKeys
            .Where(suffix => int.TryParse(suffix, out _))
            .Select(int.Parse);
        
        return numbers.Any() ? numbers.Max() : 0;
    }
    
    /// <summary>
    /// Возвращает максимальный номер среди "ожидающих" книг
    /// </summary>
    /// <param name="pendingBooks">Список книг.</param>
    /// <param name="prefix">Префикс.</param>
    /// <returns>Максимальное значение, среди ожидающих книг.</returns>
    private static int GetMaxPendingBookNumber(List<BookCopy> pendingBooks, string prefix)
    {
        return pendingBooks
            .Where(b => b.BookKey?.StartsWith(prefix) ?? false)
            .Select(b => Convert.ToInt32(b.BookKey.Substring(prefix.Length)))
            .DefaultIfEmpty(0)
            .Max();
    }
    
}