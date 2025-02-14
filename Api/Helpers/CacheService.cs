using System.Text.Json;

namespace Api.Helpers;

class CacheService<T>(ILogger logger, string filePath)
{
    private static JsonSerializerOptions JsonSerializerOptions => new() { WriteIndented = true };

    // Asynchronously writes the cache to a file
    public async Task WriteCacheAsync(IEnumerable<T> data)
    {
        try
        {
            await using FileStream createStream = File.Create(filePath);
            await JsonSerializer.SerializeAsync(createStream, data, JsonSerializerOptions);
            logger.LogInformation("✅ Cache saved successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError("❌ Error saving cache: {}", ex.Message);
        }
    }

    // Asynchronously reads the cache from a file
    public async Task<List<T>> ReadCacheAsync()
    {
        try
        {
            if (!File.Exists(filePath))
            {
                logger.LogInformation("⚠️ No cache found.");
                return [];
            }

            await using FileStream openStream = File.OpenRead(filePath);
            return await JsonSerializer.DeserializeAsync<List<T>>(openStream) ?? [];
        }
        catch (Exception ex)
        {
            logger.LogError("❌ Error reading cache: {}", ex.Message);
            return [];
        }
    }
}