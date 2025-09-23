using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using MediatR;
using System.Windows.Input;
using CustomerManagementMicroService.Application;
using Microsoft.EntityFrameworkCore;
using System;
using CustomerManagementMicroService.Infrastructure;
using CustomerManagementMicroService.Domain;
using Microsoft.EntityFrameworkCore.Metadata;
using NuGet.Protocol.Plugins;
using UI.Infrastructure;
namespace UI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddTransient<IMessageService<Customer>, RabbitMQService>();
            builder.Services.AddScoped<CreateCustomerCommand>();
            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });
            builder.Services.AddAutoMapper(typeof(Program));
            builder.Services.AddDbContext<CustomerDbContext>(options =>
                            options.UseSqlServer(builder.Configuration.GetConnectionString("MainDb")));
            builder.Services.AddDbContext<EventDbContext>(options =>
                           options.UseSqlServer(builder.Configuration.GetConnectionString("Audit")));
            builder.Services.AddScoped<Irepository<Customer>, CustomerRepository>();
            builder.Services.AddScoped<IEventStore<IEvent>, SqlServerEventDb>();

            var app = builder.Build();
           
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
