using System;
using System.Collections.Generic;

namespace Diliveryprojectserver.Model;

public partial class OrderHistory
{
    public int HistoryId { get; set; }

    public int OrderId { get; set; }

    public int StatusId { get; set; }

    public DateTime? ChangeDate { get; set; }

    public int? ChangedBy { get; set; }

    public virtual User? ChangedByNavigation { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual OrderStatus Status { get; set; } = null!;
}
