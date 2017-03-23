using System.Collections.Generic;

namespace UnitySocial
{
namespace Tools
{
    public static class DictionaryExtensions
    {
        public static bool TryGetValue<T>(this Dictionary<string, object> dict, string key, out T value)
        {
            if (dict != null && dict.ContainsKey(key))
            {
                if (dict[key].GetType() == typeof(T))
                {
                    value = (T) dict[key];
                    return true;
                }
                else
                {
                    //Debug.Log("Trying to get " + typeof(T).ToString() + " but the item is " + dict[key].GetType().ToString() + ", trying to convert");

                    try
                    {
                        value = (T) System.Convert.ChangeType(dict[key], typeof(T));
                        return true;
                    }
                    catch
                    {
                    }
                }
            }
            value = default(T);
            return false;
        }

        public static Dictionary<string, object> JsonToDictionary(string json)
        {
            #if UNITY_SOCIAL
            if (json != null && json.Length > 0)
            {
                return Json.Deserialize(json) as Dictionary<string, object>;
            }
            #endif
            return null;
        }

        public static string DictionaryToJson(object o)
        {
            #if UNITY_SOCIAL
            return Json.Serialize(o);
            #else
            return null;
            #endif
        }
    }
}
}
