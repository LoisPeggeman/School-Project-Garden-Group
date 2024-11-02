﻿using Model.Enums;
using Model.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class TicketDao : DAO
    {
        public TicketDao() : base()
        {

        }

        public List<Ticket> GetTickets()
        {
            List<Ticket> tickets = Db.GetCollection<Ticket>("tickets")
                .Aggregate()
                .ToList();

            return tickets;
        }
        public List<PartialUser> Getemployees()
        {
            List<PartialUser> employees = Db.GetCollection<PartialUser>("employees")
                .Aggregate()
                .Project(e => new PartialUser
                {
                    Id = e.Id,
                    name = e.name,
                    email = e.email,
                    phone_number = e.phone_number
                })
                .ToList();

            return employees;
        }

        public void SaveTicket(Ticket ticket)
        {
            IMongoCollection<Ticket> ticketCollection = Db.GetCollection<Ticket>("tickets");
            ticketCollection.InsertOne(ticket);
        }

        public List<Ticket> GetTicketsEmployees()
        {
            List<Ticket> tickets = Db.GetCollection<Ticket>("tickets")
                .Aggregate()
                .Match(e => e.status == "open")
                .SortByDescending(t => t.created_at)
                .Limit(25)
                .ToList();

            return tickets;
        }

        public List<Ticket> GetTicketsByStatus(Status_Enum status)
        {
            List<Ticket> tickets = Db.GetCollection<Ticket>("tickets")
                .Aggregate()
                .Match(Builders<Ticket>.Filter.Eq(ticket => ticket.status, status.ToString()))
                .SortByDescending(t => t.created_at)
                .Limit(25)
                .ToList();

            return tickets;
        }

        public List<Ticket> SearchTickets(string searchQuery)
        {
            var tickets = Db.GetCollection<Ticket>("tickets")
                .Aggregate()
                .Match(Builders<Ticket>.Filter.Text(searchQuery))
                .Sort(Builders<Ticket>.Sort.Descending(ticket => ticket.created_at))
                .ToList();

            return tickets;
        }

        public void ChangeTicketStatus(ObjectId ticketId, Status_Enum status)
        {
            var tickets = Db.GetCollection<Ticket>("tickets")
                .UpdateOne(
                Builders<Ticket>.Filter.Eq(ticket => ticket._id, ticketId),
                Builders<Ticket>.Update.Set(ticket => ticket.status, status.ToString())
                );
        }

        public List<TicketsCount> GetTicketsCountByStatus(string status)
        {
            try
            {
                List<TicketsCount> tickets = Db.GetCollection<Ticket>("tickets")
                    .Aggregate()
                    .Match(e => e.status == status)
                    .Group(
                        g => $"{status} tickets",
                        g => new TicketsCount
                        {
                            _id = g.Key,
                            count = g.Count()
                        }).ToList();

                return tickets;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public List<TicketsCount> GetTicketsPastDeadlineCount()
        {
            List<TicketsCount> tickets = Db.GetCollection<Ticket>("tickets")
                .Aggregate()
                .Match(e => e.status == "open"
                && e.created_at < DateTime.UtcNow)
                .Group(g => "Tickets Past Deadline",
                g => new TicketsCount
                {
                    _id = g.Key,
                    count = g.Count(),
                }).ToList();

            return tickets;

        }

        public void UpdateEmployeeNameInTickets(string employeeId, string newName)
        {
            // Update the employee's name in all tickets where they are referenced
            var filter = Builders<Ticket>.Filter.Or(
                Builders<Ticket>.Filter.Eq("reported_by.Id", employeeId),
                Builders<Ticket>.Filter.Eq("assigned_to.Id", employeeId),
                Builders<Ticket>.Filter.Eq("resolved_by.Id", employeeId)
            );

            var update = Builders<Ticket>.Update
                .Set("reported_by.name", newName)
                .Set("assigned_to.name", newName)
                .Set("resolved_by.name", newName);

            Db.GetCollection<Ticket>("tickets").UpdateMany(filter, update);
        }

        public Ticket GetTicketById(ObjectId ticketId)
        {
            var filter = Builders<Ticket>.Filter.Eq("_id", ticketId);
            return Db.GetCollection<Ticket>("tickets").Find(filter).FirstOrDefault();
        }

        public void AddCommentIdToTicket(ObjectId ticketId, ObjectId commentId)
        {
            var filter = Builders<Ticket>.Filter.Eq("_id", ticketId);
            var update = Builders<Ticket>.Update.Push("commentIds", commentId);
            Db.GetCollection<Ticket>("tickets").UpdateOne(filter, update);
        }
    }
}
