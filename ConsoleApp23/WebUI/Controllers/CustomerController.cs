using AutoMapper;
using CustomerManagementMicroService.Application;
using CustomerManagementMicroService.Domain;
using CustomerManagementMicroService.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace UI.Controllers
{
    public class CustomerController : Controller
    {
        private IMediator _mediator;
        private  IMapper _mapper;

        public CustomerController(IMediator mediator)
        {
            _mediator = mediator;
        }
        public IActionResult AddCustomer()
        {
            return View("CustomerOrder");
        }
        public IActionResult SubmitCustomer(CustomerDTO obj)
        {
           // Service.Add(new Customer());/// Onion
            CreateCustomerCommand c = new CreateCustomerCommand();
            c.guid = Guid.NewGuid();
            c.Country = obj.Country;
            c.Name = obj.Name;
            Money m1 = new Money(Convert.ToDecimal(obj.Amount));

            c.money = m1;
            _mediator.Send(c);
            return View("CustomerOrder");
        }
    }
}
