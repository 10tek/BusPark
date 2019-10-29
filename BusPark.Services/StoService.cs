using BusPark.DataAccess;
using BusPark.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusPark.Services
{
    public class StoService
    {
        private BusParkContext context;

        public StoService(BusParkContext context)
        {
            this.context = context;
        }

        public void SetBus(Bus bus)
        {
            var busStatus = context.BusStatuses.GetAll().SingleOrDefault(x => x.Id == bus.BusStatus).Status;
            if (busStatus == "Отправлен на ремонт" || busStatus == "На ходу") return;
            bus.BusStatus = context.BusStatuses.GetAll().SingleOrDefault(x => x.Status == "Отправлен на ремонт").Id;
            context.Buses.Update(bus);
        }

        public void RepairBus(Bus bus)
        {
            var busStatus = context.BusStatuses.GetAll().SingleOrDefault(x => x.Id == bus.BusStatus).Status;
            if (busStatus != "Отправлен на ремонт") return;
            bus.BusStatus = context.BusStatuses.GetAll().SingleOrDefault(x => x.Status == "Ремонтируется").Id;
            context.Buses.Update(bus);
            var mechanics = context.Mechanics.GetAll();
            var workshops = context.Workshops.GetAll();
            Mechanic busMechanic = null;
            foreach (var mechanic in mechanics)
            {
                if (CheckMechanic(mechanic))
                {
                    busMechanic = mechanic;
                }
            }
            if (busMechanic is null)
            {
                Console.WriteLine("Свободных механиков нет!");
                return;
            }
            context.Workshops.Add(new Workshop
            {
                BusId = bus.Id,
                MechanicId = busMechanic.Id,
                isComplete = false
            });
        }

        public void CompliteRepair(Bus bus)
        {
            var busStatus = context.BusStatuses.GetAll().SingleOrDefault(x => x.Id == bus.BusStatus).Status;
            if (busStatus == "На ходу") return;
            bus.BusStatus = context.BusStatuses.GetAll().SingleOrDefault(x => x.Status == "На ходу").Id;

            var workshop = context.Workshops.GetAll().SingleOrDefault(x => x.BusId == bus.Id && x.isComplete == false);
            if(workshop is null)
            {
                Console.WriteLine("Такого автобуса нет на ремонте!");
                return;
            }
            workshop.isComplete = true;
            context.Workshops.Update(workshop);
        }
       
        public void BreakBus(Bus bus)
        {
            var busStatus = context.BusStatuses.GetAll().SingleOrDefault(x => x.Id == bus.BusStatus).Status;
            if (busStatus != "На ходу") return;
            bus.BusStatus = context.BusStatuses.GetAll().SingleOrDefault(x => x.Status == "Сломан").Id;
            context.Buses.Update(bus);
        }

        private bool CheckMechanic(Mechanic mechanic)
        {
            var currentRepairMechanics = context.Workshops.GetAll().Where(repair => repair.MechanicId == mechanic.Id).ToList();
            if (currentRepairMechanics.Count != 0)
            {
                foreach (var mechanicq in currentRepairMechanics)
                {
                    if (mechanicq.isComplete == false)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
