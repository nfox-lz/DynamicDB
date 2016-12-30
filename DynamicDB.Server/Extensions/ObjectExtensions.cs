using Newtonsoft.Json;
using System.IO;

namespace Compete.DynamicDB.Extensions
{
    internal static class ObjectExtensions
    {
        public static void SaveAs(this object obj, string path)
        {
            File.WriteAllBytes(path, Commons.CompressionHelper.Compress(JsonConvert.SerializeObject(obj)));
        }
    }
}
