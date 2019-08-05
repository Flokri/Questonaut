using System;
using System.IO;

namespace Questonaut.Helper
{
    /// <summary>
    /// Convert a image stream to a byte[]
    /// </summary>
    public static class GetImageStreamAsBytes
    {
        /// <summary>
        /// Get the bytes from the stream.
        /// </summary>
        /// <param name="input"><The stream./param>
        /// <returns>The input stream as byte array.</returns>
        public static byte[] Convert(Stream input)
        {
            if (input != null)
            {
                var buffer = new byte[16 * 1024];
                using (MemoryStream ms = new MemoryStream())
                {
                    int read;
                    while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, read);
                    }
                    return ms.ToArray();
                }
            }
            return null;
        }
    }
}
