using GeziRotasi.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace GeziRotasi.API.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public sealed class CategoriesController : ControllerBase
    {
        private readonly ICategoryService service;

        public CategoriesController(ICategoryService service) => this.service = service;

        /// <summary> Kategori ağacı (root → children...). </summary>
        [HttpGet]
        public async Task<IActionResult> GetTree(CancellationToken ct)
            => Ok(await service.GetTreeAsync(ct));

        /// <summary> Düz liste (id, name, slug, parentId, path, depth). </summary>
        [HttpGet("flat")]
        public async Task<IActionResult> GetFlat(CancellationToken ct)
            => Ok(await service.GetFlatAsync(ct));
    }
}
