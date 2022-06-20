using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PUSGS_Project.Models
{
    public class SignUpViewModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage = "Password confirmation is required")]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        public string Fullname { get; set; }
        [Required]
        public DateTime Birthdate { get; set; }
        [Required]
        public string Address { get; set; }
        public string ImagePath { get; set; } = @"../images/user.jpg";
        [Required]
        public string Role { get; set; }
        public string ReturnUrl { get; set; }
        public IList<AuthenticationScheme> ExternalSignUp { get; set; }
    }
}
