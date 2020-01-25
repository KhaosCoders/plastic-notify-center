using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace PlasticNotifyCenter.Utils
{
    /// <summary>
    /// Helps with JSON related tasks
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// Converts a object to a Json-Object string
        /// </summary>
        /// <param name="obj">Object</param>
        public static async Task<string> StringifyAsync(object obj)
        {
            using MemoryStream ms = new MemoryStream();
            await JsonSerializer.SerializeAsync(ms, obj, obj.GetType());
            ms.Seek(0, SeekOrigin.Begin);

            return await new StreamReader(ms).ReadToEndAsync();
        }
    }
}