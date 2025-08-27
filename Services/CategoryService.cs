using GeziRotasi.API.Dtos.Categories;
using GeziRotasi.API.Repositories.Categories;

namespace GeziRotasi.API.Services
{
    public sealed class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository repo;

        public CategoryService(ICategoryRepository repo) => this.repo = repo;

        public async Task<IReadOnlyList<CategoryFlatDto>> GetFlatAsync(CancellationToken ct = default)
        {
            var all = await repo.GetAllAsync(ct);
            return all.Select(c => new CategoryFlatDto(c.Id, c.Name, c.Slug, c.ParentId, c.Path, c.Depth))
                      .OrderBy(static c => c.Path)
                      .ToList();
        }

        public async Task<IReadOnlyList<CategoryNodeDto>> GetTreeAsync(CancellationToken ct = default)
        {
            var all = (await repo.GetAllAsync(ct)).ToList();
            var lookup = all.ToDictionary(x => x.Id, x => new CategoryNodeDto
            {
                Id = x.Id,
                Name = x.Name,
                Slug = x.Slug,
                Children = new()
            });

            var roots = new List<CategoryNodeDto>();
            foreach (var c in all)
            {
                var node = lookup[c.Id];
                if (c.ParentId is null) roots.Add(node);
                else lookup[c.ParentId.Value].Children.Add(node);
            }
            return roots;
        }
    }
}
