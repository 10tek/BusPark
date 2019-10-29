using System;
using System.Collections.Generic;
using System.Text;

namespace BusPark.Domain
{
    public class Bus : Entity
    {
        public string BusNumber { get; set; }
        public Guid BusStatus { get; set; }
    }
}
