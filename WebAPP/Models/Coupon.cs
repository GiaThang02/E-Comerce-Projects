namespace WebAPP.Models.Repository
{
    public class Coupon
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public decimal DiscountPercentage { get; set; } // Đổi tên thuộc tính để rõ ràng
        public DateTime ExpirationDate { get; set; }
        public bool IsActive { get; set; }
    }
}
