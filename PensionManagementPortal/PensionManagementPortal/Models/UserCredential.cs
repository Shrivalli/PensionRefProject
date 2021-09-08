using System;
using System.ComponentModel.DataAnnotations;

namespace PensionManagementPortal.Models
{
    public class UserCredential
    {
        [Required]
        [MinLength(3, ErrorMessage = "Username too short.")]
        public string UserName { get; set; }
        [Required]
        [MinLength(6, ErrorMessage = "Length of password cannot be less than 6.")]
        public string Password { get; set; }
    }
}
