using ConsoleApp23;
using CustomerManagementMicroService.Domain;
using CustomerManagementMicroService.Infrastructure;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerManagementMicroService.Application
{
    public interface ICommand
    {
        // Create, Update , Delete
    }
    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        void Handle(TCommand command);
    }
    public class CreateCustomerCommand : IRequest<Customer>
    {
        public int Id { get; set; } // DB repository local DB
        public Guid guid { get; set; } // cross deployment
        public string Name { get; set; }
        public string Country { get; set; }
        public Money money { get; set; }
        public DateTime CreateDateTime { get; set; }
        public int ClientId { get; set; }
        public List<Order> Orders { get; set; }
        public CreateCustomerCommand()
        {
            Orders = new List<Order>();
        }
    }
    public class UpdateCustomerCommand : IRequest<Customer>
    {
        public Guid guid { get; set; } // cross deployment
        public int Id { get; set; } // DB repository local DB
        public string Name { get; set; }
        public string Country { get; set; }
        public List<Order> Orders { get; set; }

    }
    public class CreateCustomerHandler : IRequestHandler<CreateCustomerCommand, Customer>
    {
        public Task<Customer> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {

            //  1. command translate in to enrity // aggregate
            var cust = Program.map.Map<Customer>(request);
            IEventStore<IEvent> eventDb = new SqlServerEventDb();

            //ag.IsValid();
            // 2. Check are you valid ( invariants)
            //cust.Validate();

            // 3. Calling Repository
            foreach (var temp in request.Orders)
            {
                cust.AddOrders(temp);
            }


            Irepository<Customer> irepository = new CustomerRepository(); // RDBMS
            irepository.Save(cust); // Locally

            // 4. publish the event Queue , API
            foreach (var e in cust.getEvents())
            {
                if (e is CreateCustomerEvent)
                {
                    var eve = Program.map.Map<CreateCustomerEvent>(request);
                    eventDb.AppendEvent(eve); // Audit trail
                }
                if (e is CreateOrderEvent)
                {
                    var eve = Program.map.Map<CreateOrderEvent>(request);
                    eventDb.AppendEvent(eve); // Audit trail
                    // Microservice
                }

            }

            return null;
        }
    }
    public class UpdateCustomerHandler : IRequestHandler<UpdateCustomerCommand, Customer>
    {
        public Task<Customer> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {

            //  1. command translate in to enrity // aggregate
            var cust = Program.map.Map<Customer>(request); // Shallow cloining
            IEventStore<IEvent> eventDb = new SqlServerEventDb();
            foreach(var temp in request.Orders)
            {
                cust.AddOrders(temp);
            }
            //ag.IsValid();
            // 2. Check are you valid ( invariants)
            //cust.Validate();

            // 3. Calling Repository

            Irepository<Customer> irepository = new CustomerRepository(); // RDBMS
            irepository.Update(cust); // Locally

            // 4. publish the event Queue , API
            foreach (var e in cust.getEvents())
            {
                var eve = Program.map.Map<UpdateCustomerEvent>(request);
                eventDb.AppendEvent(eve); // Audit trail
                //irepository.Save(e);
                // Send to Queue
                // ...
                // Send the objectevent as its the other MS
                // Debit credit sends it 
            }

            return null;
        }
    }
    public class DeleteCustomer : ICommand
    {

    }
    public interface IQuery<TResult>
    {

    }
    public class GetCustomerByIdCommand : IRequest<CustomerOrderResult>
    {
        public int Id { get; set; }
        public GetCustomerByIdCommand(int id)
        {
            Id = id;
        }
    }
    public class CustomerOrderResult
    {
        public int Id { get; set; } // DB repository local DB
        public Guid guid { get; set; } // cross deployment
        public string Name { get; set; }
        public string Country { get; set; }
        public Money money { get; set; }
    }
    public interface IQueryHandler<in TQuery, TResponse>
       where TQuery : IRequest<TResponse>
    {
        Task<TResponse> HandleAsync(TQuery query);
    }
    public class GetCustomerByIdHandler : IRequestHandler<GetCustomerByIdCommand, CustomerOrderResult>
    {
        public Task<CustomerOrderResult> Handle(GetCustomerByIdCommand request, CancellationToken cancellationToken)
        {
            Irepository<Customer> res = new CustomerRepository();
            var cust = (from temp in res.GetAll()
                        where temp.Id == request.Id
                        select temp).ToList()[0];
            if (cust != null)
            {
                CustomerOrderResult c = new CustomerOrderResult();
                c.Id = cust.Id;
                c.guid = cust.guid;
                c.Name = cust.Name;
                c.Country = cust.Country;
                return Task.FromResult(c);
            }
            return null;
        }
    }
}
