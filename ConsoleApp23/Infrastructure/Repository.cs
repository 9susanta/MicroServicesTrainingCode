using AutoMapper;
using Azure.Core;
using ConsoleApp23;
using CustomerManagementMicroService.Application;
using CustomerManagementMicroService.Domain;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerManagementMicroService.Infrastructure
{
    public interface Irepository<T>
    {
        bool Update(T entity);
        bool Save(T entity);
        List<T> GetAll();
        bool Delete(T entity);
    }
    public class CustomerRepository : Irepository<Customer>
    {
        public bool Delete(Customer entity)
        {
            var db = new CustomerDbContext();

            var custupdate = (from temp in db.Customers.Include(e => e.Orders)
                              where temp.Id == entity.Id
                              select temp).ToList()[0];
            db.Remove(custupdate);
            db.SaveChanges();
            return true;
        }

        public List<Customer> GetAll()
        {
            var db = new CustomerDbContext();
            return db.Customers.ToList();
        }

        public bool Save(Customer entity)
        {
            var db = new CustomerDbContext();
            db.Add(entity);
            db.SaveChanges();
            return true;
        }

        public bool Update(Customer entity)
        {
            var db = new CustomerDbContext();

            var custupdate = (from temp in db.Customers
                              where temp.Id == entity.Id
                              select temp).ToList()[0];
            Program.map.Map(entity, custupdate);

            db.SaveChanges();
            return true;
        }
    }

    public interface IEventStore<T>
    {
        List<IEventRecord> GetEvents(Guid aggregateId);
        List<IEventRecord> GetEvents();

        bool AppendEvent(T e);
    }
    public interface IEventRecord : IEvent
    {

        string eventData { get; set; }

    }
    public class EventRecord : IEventRecord
    {
        [Key]
        public int Id { get; set; }
        public Guid guid { get; set; }
        public int Version { get; set; }
        public string eventType { get; set; }
        public string eventData { get; set; }
    }
    public class SqlServerEventDb : IEventStore<IEvent>
    {
        public bool AppendEvent(IEvent e)
        {
            var lastEvent = Program.EventDB
                                    .Where(e1 => e1.guid == e.guid)
                                    .MaxBy(r1 => r1.Version);
            var er = new EventRecord()
            {
                guid = e.guid,
                eventType = e.eventType,
                Version = (lastEvent?.Version ?? 0) + 1,
                eventData = JsonConvert.SerializeObject(e)
            };

            var db = new EventDbContext();
            db.EventRecords.Add(er);
            db.SaveChanges();
            return true;
        }

        public List<IEventRecord> GetEvents(Guid aggregateId)
        {
            return (List<IEventRecord>)
                Program.EventDB.ToList<IEventRecord>().Where(e => e.guid == aggregateId);
        }

        public List<IEventRecord> GetEvents()
        {
            return Program.EventDB.ToList<IEventRecord>();
        }
    }
    public class InmemoryEventStore : IEventStore<IEvent>
    {
        public bool AppendEvent(IEvent e)
        {
            var lastEvent = Program.EventDB
                                    .Where(e1 => e1.guid == e.guid)
                                    .MaxBy(r1 => r1.Version);
            var er = new EventRecord()
            {
                guid = e.guid,
                eventType = e.eventType,
                Version = (lastEvent?.Version ?? 0) + 1,
                eventData = JsonConvert.SerializeObject(e)
            };
            // write to sql server
            Program.EventDB.Add(er);
            return true;
        }

        public List<IEventRecord> GetEvents(Guid aggregateId)
        {
            return (List<IEventRecord>)
                Program.EventDB.ToList<IEventRecord>().Where(e => e.guid == aggregateId);
        }

        public List<IEventRecord> GetEvents()
        {
            return Program.EventDB.ToList<IEventRecord>();
        }
    }
}
