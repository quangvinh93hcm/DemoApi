using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq.Expressions;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using DemoApi.Models;
using DemoApi.DTO;
using System.Web.Http.Cors;

namespace DemoApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TransactionController : ApiController
    {
        private Entities db = new Entities();

        private static readonly Expression<Func<Transaction, TransactionDTO>> AsTransactionDto =
            x => new TransactionDTO
            {
                Id = x.Id,
                UserName = x.User.UserName,
                CategoryName = x.Category.Name,
                Type = x.Type,
                Date = x.Date,
                Amount = x.Amount
                
            };

        // GET api/Transaction
        public IQueryable<TransactionDTO> GetTransactions()
        {
            return db.Transactions.Select(AsTransactionDto);
        }

        // GET api/Transaction/5
        [ResponseType(typeof(Transaction))]
        public IHttpActionResult GetTransaction(int id)
        {
            TransactionDTO transaction = db.Transactions.Where(t => t.Id == id).Select(AsTransactionDto).FirstOrDefault();
            if (transaction == null)
            {
                return NotFound();
            }

            return Ok(transaction);
        }

        // PUT api/Transaction/5
        public IHttpActionResult PutTransaction(int id, Transaction transaction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != transaction.Id)
            {
                return BadRequest();
            }

            db.Entry(transaction).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST api/Transaction
        [ResponseType(typeof(Transaction))]
        public IHttpActionResult PostTransaction(Transaction transaction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Transactions.Add(transaction);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (TransactionExists(transaction.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = transaction.Id }, transaction);
        }

        // DELETE api/Transaction/5
        [ResponseType(typeof(Transaction))]
        public IHttpActionResult DeleteTransaction(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return NotFound();
            }

            db.Transactions.Remove(transaction);
            db.SaveChanges();

            return Ok(transaction);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TransactionExists(int id)
        {
            return db.Transactions.Count(e => e.Id == id) > 0;
        }
    }
}