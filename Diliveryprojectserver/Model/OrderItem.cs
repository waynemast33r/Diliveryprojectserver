namespace Diliveryprojectserver.Model
{
    public class OrderItem
    {
        public int OrderItemID { get; set; }
        public int OrderID { get; set; }
        public string ItemName { get; set; } = string.Empty; // Значение по умолчанию
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
        public Order? Order { get; set; }
    }
}
