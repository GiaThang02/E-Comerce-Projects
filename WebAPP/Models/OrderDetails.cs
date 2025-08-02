using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPP.Models
{
	public class OrderDetails
	{
		public int Id { get; set; }
		public string UserName { get; set; }
		public string OrderCode {  get; set; }
		public int ProductId {  get; set; }
		public decimal Price { get; set; }
		public int Quantity {  get; set; }
        public decimal GrandTotal { get; set; }

        [ForeignKey("ProductId")]
		public ProductModel Products { get; set; }
        public int? OrderId { get; set; }  
        [ForeignKey("OrderId")]
        public OrderModel Order { get; set; }
		public ProductModel ProductModel { get; set; }

	}
}
