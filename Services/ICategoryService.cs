using GeziRotasi.API.Dtos.Categories;

namespace GeziRotasi.API.Services
{
    public interface ICategoryService
    {
        Task<IReadOnlyList<CategoryNodeDto>> GetTreeAsync(CancellationToken ct = default);
        Task<IReadOnlyList<CategoryFlatDto>> GetFlatAsync(CancellationToken ct = default);
    }
}
