using BusPark.DataAccess;
using BusPark.Domain;
using BusPark.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BusPark.UI
{
    public class BusParkUI : IDisposable
    {
        private readonly string ConnectionString;
        private readonly string ProviderName;
        private BusParkContext context;
        private StoService stoService;

        public BusParkUI()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true);
            IConfigurationRoot configurationRoot = builder.Build();
            ConnectionString = configurationRoot.GetConnectionString("DebugConnectionString");
            ProviderName = configurationRoot.GetSection("AppConfig").GetChildren().Single(item => item.Key == "ProviderName").Value;
            context = new BusParkContext(ConnectionString, ProviderName);
            stoService = new StoService(context);
        }

        public void Action()
        {
            FillTables();
            var isExit = false;
            while (!isExit)
            {
                Console.WriteLine("1 - посмотреть все автобусы");
                Console.WriteLine("2 - Отправить автобус на ремонт");
                Console.WriteLine("3 - Брать механику автобус на ремонт");
                Console.WriteLine("4 - Завершить ремонт автобуса");
                Console.WriteLine("5 - Сломать автобус");
                Console.WriteLine("0 - Выход");
                if (int.TryParse(Console.ReadLine(), out var menu) && menu > 0 && menu <= 4)
                {
                    switch (menu)
                    {
                        case 0: isExit = true; break;
                        case 1: ShowBuses(); break;
                        case 2: SetBus(); break;
                        case 3: RepairBus(); break;
                        case 4: CompliteRepair();break;
                        case 5: BreakBus(); break;
                    }
                }
            }
        }

        private void SetBus()
        {
            Console.WriteLine("Введите номер автобуса");
            var bus = context.Buses.GetAll().SingleOrDefault(x => x.BusNumber == Console.ReadLine());
            if (bus is null)
            {
                Console.WriteLine("Некорректный выбор");
                return;
            }
            stoService.SetBus(bus);
        }

        private void RepairBus()
        {
            Console.WriteLine("Введите номер автобуса");
            var bus = context.Buses.GetAll().SingleOrDefault(x => x.BusNumber == Console.ReadLine());
            if (bus is null)
            {
                Console.WriteLine("Некорректный выбор");
                return;
            }
            stoService.RepairBus(bus);
        }

        private void CompliteRepair()
        {
            Console.WriteLine("Введите номер автобуса");
            var bus = context.Buses.GetAll().SingleOrDefault(x => x.BusNumber == Console.ReadLine());
            if (bus is null)
            {
                Console.WriteLine("Некорректный выбор");
                return;
            }
            stoService.CompliteRepair(bus);
        }

        private void BreakBus()
        {
            Console.WriteLine("Введите номер автобуса");
            var bus = context.Buses.GetAll().SingleOrDefault(x => x.BusNumber == Console.ReadLine());
            if (bus is null)
            {
                Console.WriteLine("Некорректный выбор");
                return;
            }
            stoService.BreakBus(bus);
        }

        private void ShowBuses()
        {
            var buses = context.Buses.GetAll().ToList();
            var busStatuses = context.BusStatuses.GetAll();
            foreach (var bus in buses)
            {
                Console.WriteLine($"bus number: {bus.BusNumber}");
                Console.WriteLine($"bus status: {busStatuses.SingleOrDefault(x => x.Id == bus.BusStatus).Status}\n");
            }
        }

        private void FillTables()
        {
            var firstBusStatus = new BusStatus
            {
                Status = "Отправлен на ремонт"
            };
            var secondBusStatus = new BusStatus
            {
                Status = "На ходу"
            };
            var thirdBusStatus = new BusStatus
            {
                Status = "Ремонтируется"
            }; 
            var fourthBusStatus = new BusStatus
            {
                Status = "Сломан"
            };

            var firstMechanic = new Mechanic
            {
                FullName = "Naruto"
            };
            var secondMechanic = new Mechanic
            {
                FullName = "Sasuke"
            };
            var thirdMechanic = new Mechanic
            {
                FullName = "Jiraya"
            };

            var firstBus = new Bus
            {
                BusNumber = "1111",
                BusStatus = secondBusStatus.Id
            };
            var secondBus = new Bus
            {
                BusNumber = "2222",
                BusStatus = secondBusStatus.Id
            };
            var thirdBus = new Bus
            {
                BusNumber = "3333",
                BusStatus = secondBusStatus.Id
            };

            context.BusStatuses.Add(firstBusStatus);
            context.BusStatuses.Add(secondBusStatus);
            context.BusStatuses.Add(thirdBusStatus);
            context.BusStatuses.Add(fourthBusStatus);

            context.Mechanics.Add(firstMechanic);
            context.Mechanics.Add(secondMechanic);
            context.Mechanics.Add(thirdMechanic);

            context.Buses.Add(firstBus);
            context.Buses.Add(secondBus);
            context.Buses.Add(thirdBus);
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}
