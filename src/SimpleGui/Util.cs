using Newtonsoft.Json;
using System.IO;
using System.Numerics;

namespace SimpleGui
{
    internal static class Util
    {
        public static Vector4 ToVector4(this System.Drawing.Color color)
        {
            return new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
        }

        public static string GetJsonText<T>(this T obj)
        {
            var json = JsonConvert.SerializeObject(obj, new JsonSerializerSettings()
            {
                Formatting = Newtonsoft.Json.Formatting.Indented
            });
            return json;
        }

        public static T LoadFromJsonFile<T>(string filename)
        {
            T result = default(T);
            if (File.Exists(filename))
            {
                var contents = File.ReadAllText(filename);
                result = JsonConvert.DeserializeObject<T>(contents);
            }
            return result;
        }

        public static void SaveToJsonFile<T>(this T obj, string filename)
        {
            File.WriteAllText(filename, obj.GetJsonText());
        }
    }
}
