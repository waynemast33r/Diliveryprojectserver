using System;
using System.Collections.Generic;

namespace Diliveryprojectserver.Model;

public partial class OrderStatus
{
    public int StatusId { get; set; }

    public string? StatusName { get; set; }

    public virtual ICollection<OrderHistory> OrderHistories { get; set; } = new List<OrderHistory>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Route> Routes { get; set; } = new List<Route>();
}
