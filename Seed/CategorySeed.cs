using GeziRotasi.API.Domain.Categories;

namespace GeziRotasi.API.Seed
{
    public static class CategorySeed
    {
        private static string Slugify(string s) =>
            s.Trim().ToLowerInvariant()
             .Replace('ı', 'i').Replace('ğ', 'g').Replace('ü', 'u')
             .Replace('ş', 's').Replace('ö', 'o').Replace('ç', 'c')
             .Replace(" ", "-");

        public static IReadOnlyList<Category> Build()
        {
            var list = new List<Category>();
            int id = 1;

            Category Add(string name, string? parentSlug = null)
            {
                var slug = Slugify(name);

                Category? parent = null;
                if (!string.IsNullOrWhiteSpace(parentSlug))
                {
                    var parentSlugNorm = Slugify(parentSlug!);
                    parent = list.FirstOrDefault(x => x.Slug == parentSlugNorm);

                    if (parent is null)
                    {
                        throw new InvalidOperationException(
                            $"Seed error: parent slug '{parentSlug}' (norm='{parentSlugNorm}') not found for '{name}'. " +
                            "Rootları önce eklediğinden ve parent adını doğru yazdığından emin ol.");
                    }
                }

                var path = parent is null ? slug : $"{parent.Path}/{slug}";
                var depth = parent is null ? 0 : parent.Depth + 1;
                var cat = new Category(id++, name, slug, parent?.Id, path, depth);
                list.Add(cat);
                return cat;
            }

            Add("yemek");
            Add("turistik");
            Add("kahve");
            Add("eğlence");

            Add("kebap", "yemek");
            Add("kahvaltı", "yemek");
            Add("vegan", "yemek");
            Add("balık", "yemek");
            Add("fast food", "yemek");

            Add("müze", "turistik");
            Add("tarihî", "turistik");
            Add("park", "turistik");
            Add("saray", "turistik");
            Add("cami", "turistik");

            Add("3. nesil", "kahve");
            Add("pastane", "kahve");

            Add("akvaryum", "eğlence");
            Add("tema parkı", "eğlence");

            return list;
        }
    }
}
