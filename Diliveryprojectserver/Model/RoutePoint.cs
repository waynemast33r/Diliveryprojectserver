using System;
using System.Collections.Generic;

namespace Diliveryprojectserver.Model;

public partial class RoutePoint
{
    public int PointId { get; set; }

    public int RouteId { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public string? Description { get; set; }

    public virtual Route Route { get; set; } = null!;
}
