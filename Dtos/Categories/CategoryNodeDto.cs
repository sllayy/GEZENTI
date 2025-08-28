namespace GeziRotasi.API.Dtos.Categories
{
    public sealed record CategoryNodeDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = default!;
        public string Slug { get; init; } = default!;
        public List<CategoryNodeDto> Children { get; init; } = new();
    }
}
