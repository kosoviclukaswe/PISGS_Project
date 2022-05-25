using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcesss.Data
{
    [Table("SignUpRequest")]
    public class SignUpRequest
    {
        public string SignUpRequestId { get; set; }
        public string UserId { get; set; }
        public string RoleId { get; set; }
        public DateTime Date { get; set; }
    }
}
