namespace Hit.Mvc
{
    public class JSON_Data
    {
        /// <summary>
        /// 成功或失败
        /// </summary>
        public bool success { get; set; }


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