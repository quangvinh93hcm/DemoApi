using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using DemoApiVer2.Models;
using System.Linq.Expressions;
using DemoApiVer2.DTO;
using System.Web.Http.Cors;

namespace DemoApiVer2.Controllers
{
    [RoutePrefix("api/Transaction")]
    public class TransactionController : ApiController
    {
        private Entities db = new Entities();

        private static readonly Expression<Func<Transaction, TransactionDTO>> AsTransactionDto =
            x => new TransactionDTO
            {
                Id = x.Id,
                UserName = x.User.UserName,
                CategoryName = x.Category.Name,
                TypeName = x.Category.Type.Name,
                Date = x.Date,
                Amount = x.Amount,
                Note = x.Note
            };

        // GET api/Transaction
        [HttpGet]
        public IQueryable<TransactionDTO> GetTransactions()
        {
            return db.Transactions.Select(AsTransactionDto);
        }

        

        // GET api/Transaction/5
        [HttpGet]
        public IHttpActionResult GetTransaction(int id)
        {
            TransactionDTO transaction = db.Transactions.Where(t => t.Id == id).Select(AsTransactionDto).FirstOrDefault();
            if (transaction == null)
            {
                return NotFound();
            }

            return Ok(transaction);
        }

        [Route("User/{Id:int}")]
        [HttpGet]
        public IHttpActionResult TransactionByUserId(int id)
        {
            IQueryable<TransactionDTO> transaction = db.Transactions.Where(t => t.UserId == id).Select(AsTransactionDto);
            if (transaction == null)
            {
                return NotFound();
            }
            return Ok(transaction);
        }

        // PUT api/TransactionDTO/5
        [HttpPut]
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
        [HttpPost]
        public IHttpActionResult PostTransaction(Transaction transaction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            db.Transactions.Add(transaction);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = transaction.Id }, transaction);
        }

        // DELETE api/TransactionDTO/5
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