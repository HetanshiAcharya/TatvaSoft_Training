using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
    public class PaginatedViewModel<t>
#pragma warning restore CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
    {
        public string SortColumn { get; set; }
        public bool SortOrder { get; set; }
        public string value { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 2;
       public List<t> list { get; set; }
    }
}
