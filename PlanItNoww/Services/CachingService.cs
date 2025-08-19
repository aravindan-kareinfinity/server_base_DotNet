using Microsoft.Extensions.Options;
using PlanItNoww.Utils;
using StackExchange.Redis;
using System.Text.Json;

namespace PlanItNoww.Services
{
    public interface ICachingService
    {
        Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiry = null);
        Task<T> GetAsync<T>(string key);
        Task<bool> DeleteAsync(string key);
        Task<bool> ExistsAsync(string key);
        Task<bool> SetExpiryAsync(string key, TimeSpan expiry);
        Task<TimeSpan?> GetTimeToLiveAsync(string key);
        Task<long> IncrementAsync(string key, long value = 1);
        Task<double> IncrementAsync(string key, double value = 1);
        Task<bool> SetHashAsync(string key, string field, string value);
        Task<string> GetHashAsync(string key, string field);
        Task<Dictionary<string, string>> GetHashAllAsync(string key);
        Task<bool> DeleteHashAsync(string key, string field);
    }

    public class CachingService : ICachingService
    {
        private readonly ApplicationEnvironment _config;
        private readonly ILogger<CachingService> _logger;
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public CachingService(IOptions<ApplicationEnvironment> config, ILogger<CachingService> logger)
        {
            _config = config.Value;
            _logger = logger;
            
            try
            {
                var redisConfig = _config.caching.redis;
                var connectionString = redisConfig.connectionString ?? "localhost:6379";
                
                _redis = ConnectionMultiplexer.Connect(connectionString);
                _database = _redis.GetDatabase(redisConfig.database);
                
                _logger.LogInformation("Redis connection established successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect to Redis");
                throw;
            }
        }

        public async Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            try
            {
                var jsonValue = JsonSerializer.Serialize(value);
                var result = await _database.StringSetAsync(key, jsonValue, expiry);
                
                if (result)
                {
                    _logger.LogDebug("Successfully cached value for key: {Key}", key);
                }
                else
                {
                    _logger.LogWarning("Failed to cache value for key: {Key}", key);
                }
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to set cache value for key: {Key}", key);
                return false;
            }
        }

        public async Task<T> GetAsync<T>(string key)
        {
            try
            {
                var value = await _database.StringGetAsync(key);
                
                if (value.HasValue)
                {
                    var result = JsonSerializer.Deserialize<T>(value);
                    _logger.LogDebug("Successfully retrieved cached value for key: {Key}", key);
                    return result;
                }
                
                _logger.LogDebug("Cache miss for key: {Key}", key);
                return default(T);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get cache value for key: {Key}", key);
                return default(T);
            }
        }

        public async Task<bool> DeleteAsync(string key)
        {
            try
            {
                var result = await _database.KeyDeleteAsync(key);
                
                if (result)
                {
                    _logger.LogDebug("Successfully deleted cache key: {Key}", key);
                }
                else
                {
                    _logger.LogDebug("Cache key not found for deletion: {Key}", key);
                }
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete cache key: {Key}", key);
                return false;
            }
        }

        public async Task<bool> ExistsAsync(string key)
        {
            try
            {
                var result = await _database.KeyExistsAsync(key);
                _logger.LogDebug("Cache key existence check for {Key}: {Exists}", key, result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check cache key existence: {Key}", key);
                return false;
            }
        }

        public async Task<bool> SetExpiryAsync(string key, TimeSpan expiry)
        {
            try
            {
                var result = await _database.KeyExpireAsync(key, expiry);
                
                if (result)
                {
                    _logger.LogDebug("Successfully set expiry for cache key: {Key}, Expiry: {Expiry}", key, expiry);
                }
                else
                {
                    _logger.LogWarning("Failed to set expiry for cache key: {Key}", key);
                }
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to set expiry for cache key: {Key}", key);
                return false;
            }
        }

        public async Task<TimeSpan?> GetTimeToLiveAsync(string key)
        {
            try
            {
                var ttl = await _database.KeyTimeToLiveAsync(key);
                
                if (ttl.HasValue)
                {
                    _logger.LogDebug("TTL for cache key {Key}: {TTL}", key, ttl.Value);
                }
                else
                {
                    _logger.LogDebug("No TTL set for cache key: {Key}", key);
                }
                
                return ttl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get TTL for cache key: {Key}", key);
                return null;
            }
        }

        public async Task<long> IncrementAsync(string key, long value = 1)
        {
            try
            {
                var result = await _database.StringIncrementAsync(key, value);
                _logger.LogDebug("Incremented cache key {Key} by {Value}, new value: {Result}", key, value, result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to increment cache key: {Key}", key);
                return 0;
            }
        }

        public async Task<double> IncrementAsync(string key, double value = 1)
        {
            try
            {
                var result = await _database.StringIncrementAsync(key, value);
                _logger.LogDebug("Incremented cache key {Key} by {Value}, new value: {Result}", key, value, result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to increment cache key: {Key}", key);
                return 0;
            }
        }

        public async Task<bool> SetHashAsync(string key, string field, string value)
        {
            try
            {
                var result = await _database.HashSetAsync(key, field, value);
                
                if (result)
                {
                    _logger.LogDebug("Successfully set hash field {Field} for key: {Key}", field, key);
                }
                else
                {
                    _logger.LogDebug("Hash field {Field} already exists for key: {Key}", field, key);
                }
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to set hash field {Field} for key: {Key}", field, key);
                return false;
            }
        }

        public async Task<string> GetHashAsync(string key, string field)
        {
            try
            {
                var value = await _database.HashGetAsync(key, field);
                
                if (value.HasValue)
                {
                    _logger.LogDebug("Successfully retrieved hash field {Field} for key: {Key}", field, key);
                    return value;
                }
                
                _logger.LogDebug("Hash field {Field} not found for key: {Key}", field, key);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get hash field {Field} for key: {Key}", field, key);
                return null;
            }
        }

        public async Task<Dictionary<string, string>> GetHashAllAsync(string key)
        {
            try
            {
                var hashEntries = await _database.HashGetAllAsync(key);
                var result = new Dictionary<string, string>();
                
                foreach (var entry in hashEntries)
                {
                    result[entry.Name] = entry.Value;
                }
                
                _logger.LogDebug("Successfully retrieved all hash fields for key: {Key}, Count: {Count}", key, result.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all hash fields for key: {Key}", key);
                return new Dictionary<string, string>();
            }
        }

        public async Task<bool> DeleteHashAsync(string key, string field)
        {
            try
            {
                var result = await _database.HashDeleteAsync(key, field);
                
                if (result)
                {
                    _logger.LogDebug("Successfully deleted hash field {Field} for key: {Key}", field, key);
                }
                else
                {
                    _logger.LogDebug("Hash field {Field} not found for deletion from key: {Key}", field, key);
                }
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete hash field {Field} for key: {Key}", field, key);
                return false;
            }
        }

        public void Dispose()
        {
            _redis?.Close();
            _redis?.Dispose();
        }
    }
}
