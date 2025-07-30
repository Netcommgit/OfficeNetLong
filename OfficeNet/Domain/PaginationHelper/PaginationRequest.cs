namespace OfficeNet.Domain.PaginationModel
{
    public class PaginationRequest
    {
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        //public string? SortColumn { get; set; }  // Optional
        //public string? SortDirection { get; set; } = "asc"; // "asc" or "desc"
    }
}
