using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcesss.Data
{
    public enum SignUpRequestStatus
    {
        APPROVED,
        DISAPPROVED,
        PENDING
    }
    [Table("SignUpRequest")]
    public class SignUpRequest
    {
        [Required]
        public int SignUpRequestId { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public SignUpRequestStatus Status { get; set; }
        [Required]
        public AppUser AppUser { get; set; }
        [Required]
        [ForeignKey("AppUserId")]
        public string AppUserId { get; set; }
    }
}
