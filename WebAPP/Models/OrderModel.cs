using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPP.Models
{
	public class OrderModel
	{
		public int Id { get; set; }	
		public string OrderCode { get; set; }
		public string UserName { get; set; }
		public decimal ShippingCost {  get; set; }
		public DateTime CreatedDate { get; set; }
		public int Status {  get; set; }
        public decimal GrandTotal { get; set; }
        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public AppUserModel User { get; set; }
        public ICollection<OrderDetails> OrderDetails { get; set; }
    }
}
