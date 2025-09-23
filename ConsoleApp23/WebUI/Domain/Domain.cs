using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CustomerManagementMicroService.Application;

namespace CustomerManagementMicroService.Domain
{
    public static class FactoryCustomer
    {
        public static Customer Create(int id,
                                    string name,
                                    decimal amount
                                    , List<Order> orders)
        {
            var c1 = new Customer();
            c1.Id = id;
            c1.Name = name;

            foreach (var item in orders)
            {
                c1.AddOrders((Order)item.Clone());
            }
            return c1;
        }
    }
    public class ExcelUploadCustomer
    {
        // Customer...
    }
    public class Customer
    {
        public int Id { get; set; } // DB repository local DB
        public Guid guid { get; set; } // cross deployment
        public string Name { get; set; }
        public string Country { get; set; }
        public Money money { get; set; }
        //private readonly List<Order> _orders = new();                     // mutable inside

        //public IReadOnlyList<Order> _Orders { get; set; }
        [JsonInclude]
        public List<Order> Orders = new List<Order>(); // read-only view

        private List<IEvent> _events = new List<IEvent>();
        
        public bool IsDelete { get; private set; }
        public Customer()
        {

        }
        
        public void Remove()
        {
            IsDelete = true;
            _events.Add(new DeleteCustomerEvent()
            {
                guid = guid,
                Name = Name,
                Country = Country
            });

            foreach (var item in Orders)
            {
                item.Remove();
                _events.Add(new DeleteOrderEvent()
                {
                    Id = Id,
                    guid = guid,
                    Amount = item.Amount,
                    Item = item.Item
                });
            }
        }
        public Customer(int _id,
                        Guid _guid,
                        string _name,
                        string _country)
        {
            Orders = new List<Order>();
            _events.Add(new CreateCustomerEvent() { guid = _guid, Name = _name, Country = _country });

            //if (_guid == Guid.Empty)
            //{
            //    this._events.Add(new CreateCustomerEvent() { guid = _guid, Name = _name, Country = _country });
            //}
            //else
            //{
            //    this._events.Add(new UpdateCustomerEvent() { guid = _guid, Name = _name, Country = _country });

            //}
        }

        public List<IEvent> getEvents()
        {
            return _events.ToList();
        }
        public IList<Order> GetOrders()
        {
            return Orders.ToList(); // clone
        }
        public bool AddOrders(Order o)
        {
            decimal total = o.Amount;
            foreach (Order order in Orders)
            {
                total += order.Amount;
            }

            if (total > 2000)
            {
                throw new Exception("can not be above 200");
            }
            Orders.Add(o);
            _events.Add(new CreateOrderEvent() { Item = o.Item });
            return true;
        }
        public override bool Equals(object? obj)
        {
            return Id == ((Customer)obj).Id;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine("customer".GetHashCode(), Id.GetHashCode());
        }

    }
    public class Order : ICloneable
    {
        public int Id { get; set; }
        public string Item { get; set; }
        public decimal Amount { get; set; }
        public bool IsDeleted { get; private set; }
        public void Remove()
        {

            IsDeleted = true;
        }
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
    record MyMoney
    {
        public int Id { get; init; }
        public decimal Value { get; init; }

    }
    public class Money
    {
        public int Id { get; set; } // DB 
        public decimal Value { get; init; } // immutable
        public Money(decimal value)
        {
            Value = value;
        }
        public override bool Equals(object? obj)
        {
            // complexity
            return Value == ((Money)obj).Value; // code for comparison
        }
        public override int GetHashCode()
        {
            return HashCode.Combine("money".GetHashCode(), Value.GetHashCode());
        }
    }
}
