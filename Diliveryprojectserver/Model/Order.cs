using System;
using System.Collections.Generic;

namespace Diliveryprojectserver.Model;

public partial class Order
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public int DriverId { get; set; }

    public string? PickupAddress { get; set; }

    public string? DeliveryAddress { get; set; }

    public DateTime? OrderDate { get; set; }

    public DateTime? DeliveryDate { get; set; }

    public int StatusId { get; set; }

    public virtual User Driver { get; set; } = null!;

    public virtual ICollection<OrderHistory> OrderHistories { get; set; } = new List<OrderHistory>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Route> Routes { get; set; } = new List<Route>();

    public virtual OrderStatus Status { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public List<OrderItem> OrderItems { get; set; }
}
