using System;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Hit.Mvc
{
    /// <summary>
    /// 使用JSON.NET 进行序列化的ActionResult
    /// </summary>
    public class JsonNetResult : ActionResult
    {
        /// <summary>
        /// response.ContentEncoding
        /// </summary>
        public Encoding ContentEncoding { get; set; }
        /// <summary>
        /// response.ContentType
        /// </summary>
        public string ContentType { get; set; }
        /// <summary>
        /// 要序列号的实例
        /// </summary>
        public object Data { get; set; }
        /// <summary>
        /// 是否支持JsonP
        /// </summary>
        public bool IsJsonP { get; set; }
        /// <summary>
        /// JsonP 的 callback
        /// </summary>
        public string callback { get; set; }
        /// <summary>
        /// JSON.NET 的 SerializerSettings
        /// </summary>
        public JsonSerializerSettings SerializerSettings { get; set; }
        /// <summary>
        /// JsonTextWriter.Formatting
        /// </summary>
        public Formatting Formatting { get; set; }


        /// <summary>
        /// 通过从 System.Web.Mvc.ActionResult 类继承的自定义类型，启用对操作方法结果的处理。
        /// </summary>
        /// <param name="context">用于执行结果的上下文。上下文信息包括控制器、HTTP 内容、请求上下文和路由数据。</param>
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            HttpResponseBase response = context.HttpContext.Response;

            response.ContentType = !string.IsNullOrEmpty(ContentType)
              ? ContentType
              : "application/json";

            if (ContentEncoding != null)
                response.ContentEncoding = ContentEncoding;

            if (Data != null)
            {
                if (IsJsonP && !string.IsNullOrEmpty(callback))
                {
                    response.Output.Write(callback + "(");
                }
                JsonTextWriter writer = new JsonTextWriter(response.Output) { Formatting = Formatting };

                JsonSerializer serializer = JsonSerializer.CreateDefault(SerializerSettings);
                serializer.Serialize(writer, Data);

                writer.Flush();

                if (IsJsonP && !string.IsNullOrEmpty(callback))
                {
                    response.Output.Write(")");
                }
            }
        }
    }
}
