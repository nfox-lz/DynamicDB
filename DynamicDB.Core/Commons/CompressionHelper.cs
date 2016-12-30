using System.IO;
using System.IO.Compression;
using System.Text;

namespace Compete.DynamicDB.Commons
{
    public static class CompressionHelper
    {
        private const string EncodingName = "UTF-8";

        private const ulong BufferSize = 1048576L;

        public static byte[] Compress(byte[] data)
        {
            using (var stream = new MemoryStream())
                try
                {
                    using (GZipStream zipStream = new GZipStream(stream, CompressionLevel.Fastest, true))
                        try
                        {
                            zipStream.Write(data, 0, data.Length);
                        }
                        finally
                        {
                            zipStream.Close();
                        }

                    var result = new byte[stream.Length];
                    stream.Position = 0;
                    stream.Read(result, 0, result.Length);

                    return result;
                }
                finally
                {
                    stream.Close();
                }
        }

        public static byte[] Compress(string data)
        {
            return Compress(Encoding.GetEncoding(EncodingName).GetBytes(data));
        }

        public static byte[] Decompress(byte[] data)
        {
            using (var stream = new MemoryStream(data))
                try
                {
                    using (GZipStream zipStream = new GZipStream(stream, CompressionMode.Decompress))
                        try
                        {
                            using (MemoryStream outBuffer = new MemoryStream())
                            {
                                byte[] buffer = new byte[BufferSize];
                                int count = zipStream.Read(buffer, 0, buffer.Length);
                                while (count > 0)
                                {
                                    outBuffer.Write(buffer, 0, count);
                                    count = zipStream.Read(buffer, 0, buffer.Length);
                                }
                                var result = outBuffer.ToArray();
                                outBuffer.Close();

                                return result;
                            }
                        }
                        finally
                        {
                            zipStream.Close();
                        }
                }
                finally
                {
                    stream.Close();
                }
        }

        public static string DecompressToString(byte[] data)
        {
            return Encoding.GetEncoding(EncodingName).GetString(Decompress(data));
        }
    }
}
