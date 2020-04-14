using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryBooks.Infrastructure
{
    public interface IDynamoDbContext<T> : IDisposable where T : class
    {
        Task<IEnumerable<T>> GetAsync();
        Task<IEnumerable<T>> GetAsync(ScanFilter scanConditions);
        Task<T> GetByIdAsync(string id);
        Task SaveAsync(T item);
        Task DeleteByIdAsync(T item);
        int GetCount();
        Task DeleteByIdAsync(string id);
    }
}
