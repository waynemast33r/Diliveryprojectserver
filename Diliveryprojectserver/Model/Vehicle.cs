using System;
using System.Collections.Generic;

namespace Diliveryprojectserver.Model;

public partial class Vehicle
{
    public int VehicleId { get; set; }

    public int DriverId { get; set; }

    public string? VehicleNumber { get; set; }

    public string? Model { get; set; }

    public double? Capacity { get; set; }

    public virtual User Driver { get; set; } = null!;
}
