using System;
using System.Collections.Generic;

namespace Diliveryprojectserver.Model;

public partial class Route
{
    public int RouteId { get; set; }

    public int OrderId { get; set; }

    public int DriverId { get; set; }

    public string? StartLocation { get; set; }

    public string? EndLocation { get; set; }

    public int? EstimatedTime { get; set; }

    public double? Distance { get; set; }

    public int StatusId { get; set; }

    public virtual User Driver { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;

    public virtual ICollection<RoutePoint> RoutePoints { get; set; } = new List<RoutePoint>();

    public virtual OrderStatus Status { get; set; } = null!;
}
