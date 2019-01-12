using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Text;

namespace Basketcase
{
    // todo: could try storing serialized json in IMemoryCache instead of I DistributedCache; wouldn't need to convert to byte array
    /// <summary>Serializes cached object to json then to bytes. 
    /// If object changes outside cache, the cached version does not change.</summary>
    public class SerializedCached : ICache
    {
        public SerializedCached(
            IDistributedCache cache
        ) {
            c = cache;
        }
        IDistributedCache c;

        public T Get<T>(string key) {
            var bytes = c.Get(key);
            if (bytes == null)
                return default(T);
            // GetString throws exception if bytes is null
            var serialized = Encoding.UTF8.GetString(bytes);
            var deserialized = JsonConvert.DeserializeObject<T>(serialized);
            return deserialized;
        }

        public void Set(string key, object value, int seconds) {
            var options = new DistributedCacheEntryOptions() {
                AbsoluteExpiration = new DateTimeOffset(DateTime.UtcNow.AddSeconds(seconds))
            };

            if (value == null)
                c.Set(key, null, options);
            else {
                var json = JsonConvert.SerializeObject(value);
                var bytes = Encoding.UTF8.GetBytes(json);
                c.Set(key, bytes, options);
            }
        }
    }
}
