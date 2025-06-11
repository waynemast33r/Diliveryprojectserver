namespace Diliveryprojectserver.Model
{
    public class OrderUpdateDto
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public int DriverId { get; set; }
        public string? PickupAddress { get; set; }
        public string? DeliveryAddress { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public int StatusId { get; set; }
    }
}
