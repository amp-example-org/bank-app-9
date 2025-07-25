using System;
using System.ComponentModel.DataAnnotations;

namespace AcmeBankApp.Core.Models
{
    public class Account
    {
        public int AccountId { get; set; }
        
        [Required]
        [StringLength(20)]
        public string AccountNumber { get; set; }
        
        public int UserId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string AccountType { get; set; } // Checking, Savings, Credit
        
        public decimal Balance { get; set; }
        
        [StringLength(100)]
        public string AccountName { get; set; }
        
        public DateTime CreatedDate { get; set; }
        
        public bool IsActive { get; set; }
        
        // Navigation property
        public virtual User User { get; set; }
    }
}
