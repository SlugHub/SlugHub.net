using System.Threading.Tasks;

namespace SlugHub.SlugStore
{
    public interface ISlugStore
    {
        Task<bool> ExistsAsync(string slug, string groupingKey);

        Task StoreAsync(Slug slug);
    }
}