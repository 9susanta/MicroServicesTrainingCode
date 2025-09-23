using AutoMapper;
using CustomerManagementMicroService.Domain;
using CustomerManagementMicroService.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerManagementMicroService.Application
{
    public class CustomerDtotoCustomer : Profile
    {
        public CustomerDtotoCustomer()
        {
            CreateMap<CustomerDTO, CreateCustomerCommand>()
             .ForMember(dest => dest.money, opt => opt.Ignore())
             .ForMember(dest => dest.Orders, opt => opt.MapFrom(src => src.Orders));
             
            CreateMap<Order, Order>();

        }
    }
    public class CustomertoCustomer : Profile
    {
        public CustomertoCustomer()
        {
            CreateMap<Customer, Customer>()
             .ForMember(dest => dest.money, opt => opt.Ignore())
             .ForMember(dest => dest.Orders, opt => opt.MapFrom(src => src.Orders))
             .ConstructUsing(src => new Customer(src.Id, src.guid,
                                                     src.Name,
                                                    src.Country
                                                    ));
            CreateMap<Order, Order>();

        }
    }
    public class UpdateCommandtoCustomer : Profile
    {
        public UpdateCommandtoCustomer()
        {
            CreateMap<UpdateCustomerCommand, Customer>()

                    .ConstructUsing(src => new Customer(src.Id, src.guid,
                                                src.Name,
                                            src.Country
                                            ));

        }
    }
    public class CustomerCommandtoCustomer : Profile
    {
        public CustomerCommandtoCustomer()
        {
            CreateMap<CreateCustomerCommand, Customer>()
             .ConstructUsing(src => new Customer(src.Id, src.guid,
                                                     src.Name,
                                                    src.Country
                                                    ));
        }
    }

    public class CustomerCommandtoCustomerEvent : Profile
    {
        public CustomerCommandtoCustomerEvent()
        {
            CreateMap<CreateCustomerCommand, CreateCustomerEvent>();
        }
    }

    public class UpdateCommandtoCustomerEvent : Profile
    {
        public UpdateCommandtoCustomerEvent()
        {
            CreateMap<UpdateCustomerCommand, UpdateCustomerEvent>();
        }
    }
}
