using System.Collections.Generic;

namespace Mi.Fish.ApplicationDto
{
    public class CategoryDTO
    {
        public int Level { get; set; }
        /// <summary>
        /// 分类序号
        /// </summary>
        public string Item_Clsno { get; set; }
        /// <summary>
        /// 分类名称
        /// </summary>
        public string Item_Clsname { get; set; }
    }

    public class CategoryOutPut
    {
        public int Level { get; set; }
        /// <summary>
        /// 分类序号
        /// </summary>
        public string CategoryNo { get; set; }
        /// <summary>
        /// 分类名称
        /// </summary>
        public string CategoryName { get; set; }
    }

    /// <summary>
    /// 所选择分类
    /// </summary>
    public class CategoryDetailOutPut
    {
        /// <summary>
        /// 顶层分类，只有level=0情况返回
        /// </summary>
        public List<CategoryOutPut> TopLevelCategory { get; set; } = new List<CategoryOutPut>();
        /// <summary>
        /// 第二层分类
        /// </summary>
        public List<CategoryOutPut> SecendLevelCategory { get; set; } = new List<CategoryOutPut>();
        /// <summary>
        /// 第三层分类
        /// </summary>
        public List<CategoryOutPut> ThirdLevelCategory { get; set; } = new List<CategoryOutPut>();
        /// <summary>
        /// 当前默认选择分类
        /// </summary>
        public CategoryOutPut CurrentSelected { get; set; } = new CategoryOutPut();
    }
}
