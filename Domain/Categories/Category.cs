namespace GeziRotasi.API.Domain.Categories
{
    public class Category
    {
        // EF Core için parametresiz ctor
        public Category() { }

        // Domain katmanında kolay kullanım için ctor
        public Category(int id, string name, string slug, int? parentId, string path, int depth)
        {
            Id = id;
            Name = name;
            Slug = slug;
            ParentId = parentId;
            Path = path;
            Depth = depth;
        }

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public int? ParentId { get; set; }
        public string Path { get; set; } = string.Empty;
        public int Depth { get; set; }
    }
}
