using CustomerManagementMicroService.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerManagementMicroService.DTO
{
    public class CustomerDTO
    {
        public int Id { get; set; }
        public Guid guid { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string Amount { get; set; }
        public List<Order> Orders { get; set; }
        public CustomerDTO()
        {
            this.Orders = new List<Order>();
        }
    }
}
