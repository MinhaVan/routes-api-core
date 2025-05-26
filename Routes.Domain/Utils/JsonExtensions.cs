using Newtonsoft.Json;

namespace Routes.Data.Utils;

public static class JsonExtensions
{
    public static string ToJson(this object obj, Formatting formatting = Formatting.None) // Formatting.Indented
    {
        return JsonConvert.SerializeObject(obj, formatting, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        });
    }

    public static T FromJson<T>(this string json)
    {
        return JsonConvert.DeserializeObject<T>(json);
    }
}
