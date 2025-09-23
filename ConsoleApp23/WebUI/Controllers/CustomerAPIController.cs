using AutoMapper;
using CustomerManagementMicroService.Application;
using CustomerManagementMicroService.Domain;
using CustomerManagementMicroService.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using UI.Infrastructure;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerAPIController : ControllerBase
    {
        private IMediator _mediator;
        private IMapper _mapper;
        private IMessageService<Customer> _messageService;
        public CustomerAPIController(IMediator mediator , 
                                    IMessageService<Customer> messageService)
        {
            _mediator = mediator;
            _messageService = messageService;
        }
        // GET: api/<CustomerAPIController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<CustomerAPIController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<CustomerAPIController>
        [HttpPost]
        public void Post([FromBody] CustomerDTO obj)
        {
            CreateCustomerCommand c = new CreateCustomerCommand();
            c.guid = Guid.NewGuid();
            c.Country = obj.Country;
            c.Name = obj.Name;
            Money m1 = new Money(Convert.ToDecimal(obj.Amount));
            c.money = m1;
            foreach (var item in obj.Orders)
            {
                c.Orders.Add(item);
            }
            _mediator.Send(c);
        }

        // PUT api/<CustomerAPIController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<CustomerAPIController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
