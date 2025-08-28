namespace GeziRotasi.API.Domain.Categories
{
    public sealed record Category(
    int Id,
    string Name,
    string Slug,
    int? ParentId,
    string Path,   
    int Depth      
);
}
