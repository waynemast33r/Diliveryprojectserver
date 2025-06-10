using System;
using System.Collections.Generic;

namespace Diliveryprojectserver.Model;

public partial class User
{
    public int UserId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? PasswordHash { get; set; }

    public int? RoleId { get; set; }

    public virtual ICollection<ActionLog> ActionLogs { get; set; } = new List<ActionLog>();

    public virtual ICollection<Order> OrderDrivers { get; set; } = new List<Order>();

    public virtual ICollection<OrderHistory> OrderHistories { get; set; } = new List<OrderHistory>();

    public virtual ICollection<Order> OrderUsers { get; set; } = new List<Order>();

    public virtual Role? Role { get; set; }

    public virtual ICollection<Route> Routes { get; set; } = new List<Route>();

    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
