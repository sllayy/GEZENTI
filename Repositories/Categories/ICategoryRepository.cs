using GeziRotasi.API.Domain.Categories;

namespace GeziRotasi.API.Repositories.Categories
{
    public interface ICategoryRepository
    {
        Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken ct = default);
        Task<Category?> FindBySlugAsync(string slug, CancellationToken ct = default);
    }
}
