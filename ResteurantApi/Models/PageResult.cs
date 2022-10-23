using System;
using System.Collections.Generic;

namespace ResteurantApi.Models
{
    //klasa generyczna
    public class PageResult<T>
    {
        public List<T> Items { get; set; }
        public int totalPages { get; set; }
        public int ItemsFrom { get; set; }
        public int ItemsTo { get; set; }
        public int TotalItemsFound { get; set; }

        public PageResult(List<T> items, int totalCount, int pageSize, int pageNumber)
        {
            //liczymy strony zeby sie dobrze wyswietlaly
            Items = items;
            TotalItemsFound = totalCount;
            ItemsFrom = pageSize * (pageNumber - 1) + 1;
            ItemsTo = ItemsFrom + pageSize - 1;
            totalPages = (int)Math.Ceiling(totalCount / (double) pageSize);
        }
    }
}
