namespace Mi.Fish.ApplicationDto
{
    ///<summary>
    ///搜索商品入参
    ///</summary>
    public class GoodsSearchInput:BaseInputDto
    {
        ///<summary>
        ///商品编号或者商品名称，支持模糊搜索
        ///</summary>
        public string Key { get; set; }
    }
}
