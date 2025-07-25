using System;
using System.ComponentModel.DataAnnotations;

namespace AcmeBankApp.Core.Models
{
    public class User
    {
        public int UserId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string UserName { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Email { get; set; }
        
        [Required]
        public string PasswordHash { get; set; }
        
        [StringLength(100)]
        public string FirstName { get; set; }
        
        [StringLength(100)]
        public string LastName { get; set; }
        
        public DateTime CreatedDate { get; set; }
        
        public DateTime? LastLogin { get; set; }
        
        public bool IsActive { get; set; }
        
        // Legacy property - stored as plain text (security issue)
        public string SecurityQuestion { get; set; }
        public string SecurityAnswer { get; set; }
    }
}
