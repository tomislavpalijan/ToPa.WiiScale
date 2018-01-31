using System;
using System.IO;
using Newtonsoft.Json;

namespace WiiScale.Logic.UI.Helper.Serializer
{
    public static class JsonSerializer
    {
        public static void SerializeObject<T>(T serializebleObject, string filepath)
        {
            var direct = Path.GetDirectoryName(filepath);

            if (String.IsNullOrEmpty(direct))
                return;

            if (!Directory.Exists(direct))
                Directory.CreateDirectory(direct);

            using (var fs = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (var sw = new StreamWriter(fs))
                {
                    sw.Write(JsonConvert.SerializeObject(serializebleObject));
                }
            }

        }

        public static T DeserializeObject<T>(string filepath)
        {
            string rawString;
            using (var sr = File.OpenText(filepath))
            {
                rawString = sr.ReadToEnd();
            }
            return JsonConvert.DeserializeObject<T>(rawString);
        }
    }
}