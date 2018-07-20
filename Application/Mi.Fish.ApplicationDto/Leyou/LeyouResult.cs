namespace Mi.Fish.ApplicationDto
{
    /// <summary>
    /// 
    /// </summary>
    public class LeyouResult<TData>
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// 提示信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 返回数据
        /// </summary>
        public virtual TData Data { get; set; }
    }
}
