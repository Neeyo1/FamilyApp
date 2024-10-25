namespace API.Helpers;

public class AssignmentParams : PaginationParams
{
    public string? Name { get; set; }
    public string? CreatedBy { get; set; }
    public int MinUsers { get; set; } = 1;
    public int MaxUsers { get; set; } = 10;
    public string Status { get; set; } = "all";
    public string OrderBy { get; set; } = "oldest";
}
