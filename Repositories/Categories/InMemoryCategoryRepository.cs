using GeziRotasi.API.Domain.Categories;
using GeziRotasi.API.Seed;

namespace GeziRotasi.API.Repositories.Categories
{
    public sealed class InMemoryCategoryRepository : ICategoryRepository
    {      
        private static readonly IReadOnlyList<Category> _data = CategorySeed.Build();

        public Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult(_data);

        public Task<Category?> FindBySlugAsync(string slug, CancellationToken ct = default)
            => Task.FromResult(_data.FirstOrDefault(x => x.Slug == slug));
    }
}
