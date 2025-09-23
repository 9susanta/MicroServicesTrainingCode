using AutoMapper;
using CustomerManagementMicroService.Application;
using CustomerManagementMicroService.Domain;
using CustomerManagementMicroService.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Numerics;
using System.Xml.Linq;

namespace ConsoleApp23
{

    internal class Program
    {
        public static IMapper map = null;
        public static string connectionstringDB = "Data Source=DESKTOP-ILFSBH1\\SQLEXPRESS;Initial Catalog=CustomerDB;Integrated Security=True;Trust Server Certificate=True";

        public static string connectionstringAudit = "Data Source=DESKTOP-ILFSBH1\\SQLEXPRESS;Initial Catalog=CustomerAuditTrail;Integrated Security=True;Trust Server Certificate=True";
        public static List<IEventRecord> EventDB = new List<IEventRecord>();
        static CreateCustomerCommand Create()
        {
           
            CreateCustomerCommand c = new CreateCustomerCommand();
            c.guid = Guid.NewGuid();
            Console.WriteLine("Enter country");
            c.Country = Console.ReadLine();
            Console.WriteLine("Enter Name");
            c.Name = Console.ReadLine();
            Console.WriteLine("Enter Amount");
            Money m1 = new Money(Convert.ToDecimal(Console.ReadLine()));

            c.money = m1;
            return c;
        }
        static  UpdateCustomerCommand Update(IMediator m)
        {
            Console.WriteLine("Enter Id tp update");
            int id = Convert.ToInt16(Console.ReadLine());
            GetCustomerByIdCommand s = new GetCustomerByIdCommand(id);
           
            var t = m.Send(s).Result;
            Console.WriteLine($"CustomerId: {t.Id}, Name: {t.Name} ,  Country: {t.Country}");

            UpdateCustomerCommand c = new UpdateCustomerCommand();
            c.Id = t.Id;
            c.guid = t.guid;
            Console.WriteLine("Enter country");
            c.Country = Console.ReadLine();
            Console.WriteLine("Enter Name");
            c.Name = Console.ReadLine();
            return c;
        }
        static void Main(string[] args)
        {
           
           
            var config = new MapperConfiguration(cfg =>
                    {
                        cfg.AddProfile<CustomerDtotoCustomer>();
                        cfg.AddProfile<CustomerCommandtoCustomer>();
                        cfg.AddProfile<CustomerCommandtoCustomerEvent>();
                        cfg.AddProfile<UpdateCommandtoCustomer>();
                        cfg.AddProfile<CustomertoCustomer>();
                        cfg.AddProfile<UpdateCommandtoCustomerEvent>();
                    });



            // Step 2: Create Mapper instance
            map = config.CreateMapper();

            var services = new ServiceCollection();
            services.AddLogging();

            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssemblyContaining(typeof(Program)));
            
            var provider = services.BuildServiceProvider();
            var mediator = provider.GetRequiredService<IMediator>();
           
            string cont= "";
            while (cont != "N")
            {
                Console.WriteLine("A - Add , U - update , D - Delete , N to exit");
                cont = Console.ReadLine();
                Console.Clear();
                if(cont == "A")
                {
                  var cmd=  Create();
                  cmd.Orders.Add(new Order() { Item="Shoes",Amount=100.23m});
                  cmd.Orders.Add(new Order() { Item = "Shirst", Amount = 200 });

                    mediator.Send(cmd);
                }
                else if (cont == "U")
                {
                    var cmd = Update(mediator);
                    mediator.Send(cmd);
                }
                else
                {
                    return;
                }
              



            }
            IEventStore<IEvent> eventDb = new InmemoryEventStore();
            foreach (var item in eventDb.GetEvents())
            {
                Console.WriteLine(item.guid + item.eventData + "\n");
            }

            //GetCustomerByIdCommand g = new GetCustomerByIdCommand(10);
            //mediator.Send(g);

            Console.ReadLine();
        }
    }
   
}
