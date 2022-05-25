using System;
using System.Collections.Generic;

namespace PatientPortal.Models
{
    public partial class Swap
    {
        public ulong SwapId { get; set; }
        public ulong PatientId { get; set; }
        public ulong DonorId { get; set; }

        public virtual Donor Donor { get; set; } = null!;
        public virtual Patient Patient { get; set; } = null!;
    }
}
