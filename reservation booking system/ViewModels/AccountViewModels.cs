using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CompareAttribute = System.ComponentModel.DataAnnotations.CompareAttribute;

namespace reservation_booking_system.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }

    public class RegisterAdViewModel
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        [Remote(action: "IsEmailUsed", controller: "Account", ErrorMessage = "Email already exists in Database")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)\S{6,20}$", ErrorMessage = "Minimum 6 Max 20 characters atleast 1 Alphabet, 1 Number and 1 Special Character and avoid space")]
        public string Password { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The passwords do not match.")]

        public string ConfirmPassword { get; set; }
        [Required]
        [Display(Name = "Contact")]
        [RegularExpression(@"^[0-9]{7,12}$", ErrorMessage = "Minimum 8 Max 12 digit and avoid space")]
        public string Contact { get; set; }

    }
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "UserName")]
        [RegularExpression(@"^(?=.*[a-z])\S{6,15}$", ErrorMessage = "Minimum 6 Max 15 characters, 1 small letter and avoid space")]
        [Remote(action: "IsUserNameUsed", controller: "Account", ErrorMessage = "User Name already exists in Database")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        [Remote(action: "IsEmailUsed", controller: "Account", ErrorMessage = "Email already exists in Database")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)\S{6,20}$", ErrorMessage = "Minimum 6 Max 20 characters atleast 1 Alphabet, 1 Number and avoid space")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The passwords do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [RegularExpression(@"^[0-9]{7,12}$", ErrorMessage = "Minimum 8 Max 12 digit and avoid space")]
        [Display(Name = "Contact")]
        public string Contact { get; set; }
    }

    public class UserData
    {
        public string Name { get; set; }
        public string Role { get; set; }
        public int AccessLevel { get; set; }
    }
}