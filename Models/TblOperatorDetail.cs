using System;
using System.Collections.Generic;

namespace LinePerformanceDashboard.Models;

public partial class TblOperatorDetail
{
    public int Oid { get; set; }

    public DateTime? ProdDate { get; set; }

    public string? LineNo { get; set; }

    public string? Name { get; set; }

    public string? ProcessName { get; set; }

    public string? AvgCycle { get; set; }

    public string? CapacityHr { get; set; }

    public string? Remark { get; set; }
}
