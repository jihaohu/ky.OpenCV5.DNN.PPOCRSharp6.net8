using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ky.OpenCV5.DNN.PPOCRSharp6.PaddleOCRSharp
{
    /// <summary>
    /// 简单的 JSON 辅助工具，封装对 Newtonsoft.Json 的调用以统一反序列化配置（TypeNameHandling.Auto）。
    /// </summary>
    public class JsonHelper
    {
        /// <summary>
        /// 将 JSON 字符串反序列化为指定类型对象。
        /// </summary>
        /// <typeparam name="T">目标类型。</typeparam>
        /// <param name="json">JSON 文本。</param>
        /// <returns>反序列化后的对象。</returns>
        public static T DeserializeObject<T>(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return default(T);
            }
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };
            return (T)(object)JsonConvert.DeserializeObject(json, typeof(T), settings);
        }
    }
}
