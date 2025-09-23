using AutoMapper;
using Azure.Core;
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
        IMapper _mapper;
        CustomerDbContext _db;
        public CustomerRepository(IMapper mapper , CustomerDbContext db)
        {
            _mapper = mapper;
            _db = db;
        }
        public bool Delete(Customer entity)
        {
            
            var custupdate = (from temp in _db.Customers.Include(e => e.Orders)
                              where temp.Id == entity.Id
                              select temp).ToList()[0];
            _db.Remove(custupdate);
            _db.SaveChanges();
            return true;
        }

        public List<Customer> GetAll()
        {
            return _db.Customers.ToList();
        }

        public bool Save(Customer entity)
        {
            _db.Add(entity);
            _db.SaveChanges();
            return true;
        }

        public bool Update(Customer entity)
        {

            var custupdate = (from temp in _db.Customers
                              where temp.Id == entity.Id
                              select temp).ToList()[0];
            _mapper.Map(entity, custupdate);

            _db.SaveChanges();
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
        EventDbContext _db;
        public SqlServerEventDb(EventDbContext db)
        {
           _db = db;
        }
        public bool AppendEvent(IEvent e)
        {
            //var lastEvent = _db.EventRecords
            //                        .Where(e1 => e1.guid == e.guid)
            //                        .MaxBy(r1 => r1.Version);
            var er = new EventRecord()
            {
                guid = e.guid,
                eventType = e.eventType,
                //Version = (lastEvent?.Version ?? 0) + 1,
                eventData = JsonConvert.SerializeObject(e)
            };

           
            _db.EventRecords.Add(er);
            _db.SaveChanges();
            return true;
        }

        public List<IEventRecord> GetEvents(Guid aggregateId)
        {
            return (List<IEventRecord>)
                _db.EventRecords.Where(e => e.guid == aggregateId);
        }

        public List<IEventRecord> GetEvents()
        {
            return _db.EventRecords.ToList<IEventRecord>();
            
        }
    }
    public class InmemoryEventStore : IEventStore<IEvent>
    {
        public bool AppendEvent(IEvent e)
        {
            //var lastEvent = Program.EventDB
            //                        .Where(e1 => e1.guid == e.guid)
            //                        .MaxBy(r1 => r1.Version);
            var er = new EventRecord()
            {
                guid = e.guid,
                eventType = e.eventType,
                //Version = (lastEvent?.Version ?? 0) + 1,
                eventData = JsonConvert.SerializeObject(e)
            };
            // write to sql server
            //Program.EventDB.Add(er);
            return true;
        }

        public List<IEventRecord> GetEvents(Guid aggregateId)
        {
            //return (List<IEventRecord>)
            //    Program.EventDB.ToList<IEventRecord>().Where(e => e.guid == aggregateId);
            return null;
        }

        public List<IEventRecord> GetEvents()
        {
            //return Program.EventDB.ToList<IEventRecord>();
            return null;
        }
    }
}
