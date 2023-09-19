using CS322_eTickets.Models;

namespace CS322_eTickets.Data.ViewModels
{
    public class MoviePagination
    {
        public IEnumerable<Movie> Movies { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
    }
}
