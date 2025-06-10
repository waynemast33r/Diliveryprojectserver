using System;
using System.Collections.Generic;

namespace Diliveryprojectserver.Model;

public partial class ActionLog
{
    public int LogId { get; set; }

    public int UserId { get; set; }

    public string? ActionDescription { get; set; }

    public DateTime? ActionDate { get; set; }

    public virtual User User { get; set; } = null!;
}
