using BusPark.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusPark.DataAccess
{
    public class BusParkContext : IDisposable
    {
        public Repository<Bus> Buses { get; set; }
        public Repository<BusStatus> BusStatuses { get; set; }
        public Repository<Mechanic> Mechanics { get; set; }
        public Repository<Workshop> Workshops { get; set; }

        public BusParkContext(string connectionString, string providerInvariantName)
        {
            Buses = new Repository<Bus>(connectionString, providerInvariantName);
            BusStatuses = new Repository<BusStatus>(connectionString, providerInvariantName);
            Mechanics = new Repository<Mechanic>(connectionString, providerInvariantName);
            Workshops = new Repository<Workshop>(connectionString, providerInvariantName);
        }

        public void Dispose()
        {
            Buses.Dispose();
            BusStatuses.Dispose();
            Mechanics.Dispose();
            Workshops.Dispose();
        }
    }
}
