using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace ShopNursery.Utils
{
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session,string key,IEnumerable<T> value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));  
        }

        public static IEnumerable<T>? Get<T>(this ISession session,string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonConvert.DeserializeObject<IEnumerable<T>>(value);
        }
    }
}
