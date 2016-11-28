namespace Hit.Mvc
{
    /// <summary>
    /// JSON数据接口数据格式
    /// </summary>
    public class JSON_Data
    {
        /// <summary>
        /// 成功或失败
        /// </summary>
        public bool success { get; set; }

        /// <summary>
        /// 代码 0 正常;负数 不可预期异常;正数 预期中的异常
        /// </summary>
        public int code { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public object msg { get; set; }

        /// <summary>
        /// 结果
        /// </summary>
        public object result { get; set; }

    }
    /// <summary>
    /// 列表数据格式
    /// </summary>
    public class JSON_Data_List
    {
        /// <summary>
        /// 总行数
        /// </summary>
        public int total { get; set; }

        /// <summary>
        /// 列表数据
        /// </summary>
        public object data { get; set; }

    }
}