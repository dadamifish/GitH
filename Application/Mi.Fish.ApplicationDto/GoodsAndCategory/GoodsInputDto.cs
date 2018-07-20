namespace Mi.Fish.ApplicationDto
{
    using System.ComponentModel.DataAnnotations;
    public class CategoryInputDto : BaseInputDto
    {
        public string ParentNo { get; set; }
        public int Level { get; set; }
    }

    public class BaseInputDto
    {
        public string Token { get; set; }
    }

    public class GoodsInputDto : BaseInputDto
    {
        public string Category { get; set; }
        public string Type { get; set; } = "1";
    }

    public class OrderDetailInputDto : BaseInputDto
    {

        [Required(ErrorMessage = "订单编号不能为空")]
        public string OrderNo { get; set; }
    }
}
