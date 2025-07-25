using System.ComponentModel.DataAnnotations;

namespace AcmeBankApp.Core.Models
{
    public class TransferModel
    {
        [Required]
        [Display(Name = "From Account")]
        public int FromAccountId { get; set; }
        
        [Required]
        [Display(Name = "To Account")]
        public int ToAccountId { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }
        
        [StringLength(200)]
        [Display(Name = "Description")]
        public string Description { get; set; }
    }
}
