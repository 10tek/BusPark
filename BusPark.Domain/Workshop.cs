using System;
using System.Collections.Generic;
using System.Text;

namespace BusPark.Domain
{
    public class Workshop : Entity
    {
        public Guid MechanicId { get; set; }
        public Guid BusId { get; set; }
        public bool isComplete { get; set; } = false;
    }
}
