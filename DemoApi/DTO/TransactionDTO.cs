using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemoApi.DTO
{
    public class TransactionDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string CategoryName { get; set; }
        public bool? Type { get; set; }
        public string Date { get; set; }
        public double? Amount { get; set; }
    }
}