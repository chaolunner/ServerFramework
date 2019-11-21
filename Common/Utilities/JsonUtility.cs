using Newtonsoft.Json;
using System;

namespace Common
{
#if !UNITY
    public static class JsonUtility
    {
        public static T FromJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static object FromJson(string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type);
        }

        public static string ToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
#endif
}