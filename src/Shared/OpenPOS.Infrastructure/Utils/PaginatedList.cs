using System.Collections.Generic;
using System.Linq;

namespace OpenPOS.Infrastructure.Utils
{
    public class PaginatedList<T> : List<T>
    {
        public PaginatedList(IEnumerable<T> items, int totalPages, int currentPage)
        {
            var collection = items.ToList();
            AddRange(collection);
            
            TotalPages = totalPages;
            CurrentPage = currentPage;
            PageSize = collection.Count;

            HasNext = CurrentPage < TotalPages;
            HasPrevious = CurrentPage > 0;
        }

        public bool HasNext { get; }
        public bool HasPrevious { get; }
        public int TotalPages { get; }
        public int CurrentPage { get; }
        public int PageSize { get; }
    }
}