using System.Threading.Tasks;

namespace SlugHub
{
    public interface ISlugGenerator
    {
        Task<string> GenerateSlugAsync(string text, string[] uniquifiers = null);

        Task<string> GenerateSlugAsync(string text, string groupingKey, string[] uniquifiers = null);
    }
}