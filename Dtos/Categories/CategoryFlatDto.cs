namespace GeziRotasi.API.Dtos.Categories
{
    public sealed record CategoryFlatDto(
        int Id,
        string Name,
        string Slug,
        int? ParentId,
        string Path,
        int Depth
    );
}
