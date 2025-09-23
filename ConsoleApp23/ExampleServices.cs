using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomerManagementMicroService.Domain;
using CustomerManagementMicroService.Infrastructure;
using MediatR;
namespace ConsoleApp23
{

    // domain service
    public class ValidationService // view model
    {
        // does not have identity
        public bool Validate(Customer obj)
        {
            if (obj.Country.Length ==0) return false;
            return true;
        }
    }
    // application service ( binding logic , mediator )
    public class ControllerService
    {
        public bool Add()
        {
            Irepository<Customer> repository = null; // IS
            //repository.Save(new Customer() { Name = "shiv" }); // Entiy
            return true;
        }
    }
    // insfrastructuree
   
    
    
    
    public static class ValidateCustomer
    {
        public  static bool Validate(Customer obj)
        {
            /// validations
            return true;
        }
    }
}
