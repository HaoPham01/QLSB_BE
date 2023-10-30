using System;
using System.Collections.Generic;

namespace QLSB_APIs.Models.Entities;

public partial class Timebooking
{
    public string FieldName { get; set; } = null!;

    public int TotalBookingTime { get; set; }
}
