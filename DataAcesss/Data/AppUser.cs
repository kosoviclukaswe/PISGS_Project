using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace DataAcesss.Data
{
    public enum Role
    {
        ADMINISTRATOR,
        DELIVERER,
        CONSUMER
    }
    
    [Table("AppUser")]
    public partial class AppUser : IdentityUser
    {
        [Column("Password")]
        [Required]
        public string Password { get; set; }
        [Column("Fullname")]
        public string Fullname { get; set; }
        [Required]
        public DateTime Birthdate { get; set; }
        public string Address { get; set; }
        [Column("ImagePath")]
        public string ImagePath { get; set; }

        [Index(nameof(UserName), IsUnique = true)]
        public override string UserName { get; set; }

        [Index(nameof(Email), IsUnique = true)]
        public override string Email { get; set; }
    }
}
