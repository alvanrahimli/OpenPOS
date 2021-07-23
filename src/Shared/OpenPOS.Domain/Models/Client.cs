using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpenPOS.Domain.Models
{
    public class Client
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime? FirstSaleDate { get; set; }
        public DateTime? LastSaleDate { get; set; }
        public decimal Debt { get; set; }
        public string Notes { get; set; }

        public Guid StoreId { get; set; }
        public Store Store { get; set; }
        public List<Transaction> Transactions { get; set; }

        [NotMapped] public string FirstSale =>  FirstSaleDate?.ToString("dd MMM yyyy") ?? "Yoxdur";
        [NotMapped] public string LastSale => LastSaleDate?.ToString("dd MMM yyyy") ?? "Yoxdur";
    }
}