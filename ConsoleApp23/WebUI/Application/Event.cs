using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CustomerManagementMicroService.Domain;

namespace CustomerManagementMicroService.Application
{
    public interface IEvent
    {
        public int Id { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        Guid guid { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        int Version { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        string eventType { get; set; }

    }
    public class UpdateCustomerEvent : IEvent
    {
        [NotMapped]
        public int Id { get; set; }
        [NotMapped]
        public Guid guid { get; set; }

        public int Version { get; set; }
        public string eventType { get; set; }

        public string Name { get; set; }
        public string Country { get; set; }

        public UpdateCustomerEvent()
        {
            eventType = "UpdateCustomer";
        }
    }
    public class CreateCustomerEvent : IEvent
    {
        [NotMapped]
        public int Id { get; set; }
        public Guid guid { get; set; }
        public int Version { get; set; }
        public string eventType { get; set; }

        public string Name { get; set; }
        public string Country { get; set; }
        public Money money { get; set; }
        public DateTime CreateDateTime { get; set; }
        public int ClientId { get; set; }
        public CreateCustomerEvent()
        {
            eventType = "CreateCustomer";
        }
    }
    public class DeleteCustomerEvent : IEvent
    {
        [NotMapped]
        public int Id { get; set; }
        public Guid guid { get; set; }
        public int Version { get; set; }
        public string eventType { get; set; }

        public string Name { get; set; }
        public string Country { get; set; }
        public Money money { get; set; }
        public DateTime CreateDateTime { get; set; }
        public int ClientId { get; set; }
        public DeleteCustomerEvent()
        {
            eventType = "DeleteCustomer";
        }
    }
    public class CreateOrderEvent : IEvent
    {
        [NotMapped]
        public int Id { get; set; }
        public Guid guid { get; set; }
        public string eventType { get; set; }
        public int Version { get; set; }
        public string Item { get; set; }
        public decimal Amount { get; set; }

        public CreateOrderEvent()
        {
            eventType = "CreateOrderEvent";
        }
    }
    public class DeleteOrderEvent : IEvent
    {
        [NotMapped]
        public int Id { get; set; }
        public Guid guid { get; set; }
        public string eventType { get; set; }
        public int Version { get; set; }
        public string Item { get; set; }
        public decimal Amount { get; set; }
        public DeleteOrderEvent()
        {
            eventType = "DeleteOrderEvent";
        }
    }
}
