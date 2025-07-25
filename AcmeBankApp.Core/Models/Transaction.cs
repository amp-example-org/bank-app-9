using System;
using System.ComponentModel.DataAnnotations;

namespace AcmeBankApp.Core.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        
        public int FromAccountId { get; set; }
        
        public int? ToAccountId { get; set; } // Nullable for deposits/withdrawals
        
        [Required]
        [StringLength(20)]
        public string TransactionType { get; set; } // Transfer, Deposit, Withdrawal, Payment
        
        public decimal Amount { get; set; }
        
        [StringLength(200)]
        public string Description { get; set; }
        
        public DateTime TransactionDate { get; set; }
        
        [StringLength(50)]
        public string Status { get; set; } // Pending, Completed, Failed
        
        // Legacy - stores sensitive info in description sometimes
        public string InternalNotes { get; set; }
        
        // Navigation properties
        public virtual Account FromAccount { get; set; }
        public virtual Account ToAccount { get; set; }
    }
}
