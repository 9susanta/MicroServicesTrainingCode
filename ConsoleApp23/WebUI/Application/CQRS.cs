using AutoMapper;
using CustomerManagementMicroService.Domain;
using CustomerManagementMicroService.Infrastructure;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Infrastructure;

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
        IMapper _mapper;
        Irepository<Customer> _irepository;
        IEventStore<IEvent> _eventDb;
        IMessageService<Customer> _messageService;
       
        public CreateCustomerHandler(IMapper mapper , 
                                Irepository<Customer> irepository,
                                IEventStore<IEvent> eventDb,
                                IMessageService<Customer> messageService)
        {
            _irepository = irepository; 
            _mapper = mapper;
            _eventDb = eventDb;
            _messageService = messageService;
        }
        public async Task<Customer> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            
            try
            {
                //  1. command translate in to enrity // aggregate
                var cust = _mapper.Map<Customer>(request);


                //ag.IsValid();
                // 2. Check are you valid ( invariants)
                //cust.Validate();

                // 3. Calling Repository
                foreach (var temp in request.Orders)
                {
                    cust.AddOrders(temp);
                }



                _irepository.Save(cust); // Locally


                foreach (var e in cust.getEvents())
                {
                    if (e is CreateCustomerEvent)
                    {
                        var eve = _mapper.Map<CreateCustomerEvent>(request);
                        _eventDb.AppendEvent(eve); // Audit trail
                    }
                    if (e is CreateOrderEvent)
                    {
                     //   var eve = _mapper.Map<CreateOrderEvent>(request);
                      //  _eventDb.AppendEvent(eve); // Audit trail
                                                   // Microservice
                    }
                    // Queues..
                }
                // 4. publish the event Queue , API
                _messageService.SendMessage(cust).Wait();
                return cust;
            }
            catch (Exception)
            {

                throw;
            }
            
            
        }
    }
    public class UpdateCustomerHandler : IRequestHandler<UpdateCustomerCommand, Customer>
    {
        IMapper _mapper;
        Irepository<Customer> _repository;
        IEventStore<IEvent> _eventDb;
        public UpdateCustomerHandler(IMapper mapper , 
                            Irepository<Customer> repository , IEventStore<IEvent> eventDb)
        {
            _mapper = mapper;
            _repository = repository;
            _eventDb = eventDb;
        }
        public Task<Customer> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {

            //  1. command translate in to enrity // aggregate
            var cust = _mapper.Map<Customer>(request); // Shallow cloining
            
            foreach(var temp in request.Orders)
            {
                cust.AddOrders(temp);
            }
            //ag.IsValid();
            // 2. Check are you valid ( invariants)
            //cust.Validate();

            // 3. Calling Repository

           
            _repository.Update(cust); // Locally

            // 4. publish the event Queue , API
            foreach (var e in cust.getEvents())
            {
                var eve = _mapper.Map<UpdateCustomerEvent>(request);
                _eventDb.AppendEvent(eve); // Audit trail
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
        Irepository<Customer> _rep;
        public GetCustomerByIdHandler(Irepository<Customer> rep)
        {
            _rep = rep;
        }
        public Task<CustomerOrderResult> Handle(GetCustomerByIdCommand request, CancellationToken cancellationToken)
        {
           
            var cust = (from temp in _rep.GetAll()
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
