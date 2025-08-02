namespace WebAPP.Models.ViewModels
{
    public class OrderViewModel

    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public List<CartItemModel> Products { get; set; }
        public decimal GrandTotal { get; set; }
        public string PaymentMethod { get; set; }
        public decimal ShippingCost { get; set; }
        public string CouponCode { get; set; }
    }
}
