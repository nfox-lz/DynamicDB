using Newtonsoft.Json;
using System.IO;

namespace Compete.DynamicDB.Commons
{
    internal static class JsonHelper
    {
        public static T Deserialize<T>(string path)
        {
            return JsonConvert.DeserializeObject<T>(Commons.CompressionHelper.DecompressToString(File.ReadAllBytes(path)));
        }
    }
}
