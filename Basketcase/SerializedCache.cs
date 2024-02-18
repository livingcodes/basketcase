using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Text;
namespace Basketcase;
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

  public T Get<T>(str key) {
    var bytes = c.Get(key);
    if (bytes == null)
      return default(T);
    // GetString throws exception if bytes is null
    var slzd = Encoding.UTF8.GetString(bytes);
    var dslzd = JsonConvert.DeserializeObject<T>(slzd);
    return dslzd;
  }

  public void Set(str key, obj val, int sec) {
    var ops = new DistributedCacheEntryOptions() {
      AbsoluteExpiration = new DateTimeOffset(dte.UtcNow.AddSeconds(sec))
    };

    if (val == null)
      c.Set(key, null, ops);
    else {
      var jsn = JsonConvert.SerializeObject(val);
      var bytes = Encoding.UTF8.GetBytes(jsn);
      c.Set(key, bytes, ops);
    }
  }
}