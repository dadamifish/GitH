namespace Mi.Fish.ApplicationDto
{
    using System.ComponentModel.DataAnnotations;
    public class OrderRefundInput:BaseInputDto
    {
        [Required(ErrorMessage = "订单编号不能为空")]
        public string OrderNo { get; set; }
        public bool IsConfirm { get; set; } 
    }
}
