using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
namespace com.kuazoo.Models
{

    public sealed class AdminModel
    {
        public int AdminId { get; set; }

        [Display(Name = "Admin", Description = "Admin name.")]
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "*")]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Display(Name = "Username", Description = "Username")]
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "*")]
        public string Email { get; set; }

        [Required(ErrorMessage = "*")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string LastAction { get; set; }
        public DateTime Create { get; set; }
    }
    public sealed class AdminModel2
    {
        public int AdminId { get; set; }

        [Display(Name = "Admin", Description = "Admin name.")]
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "*")]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Display(Name = "Username", Description = "Username")]
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "*")]
        public string Email { get; set; }

        public string LastAction { get; set; }
        public DateTime Create { get; set; }
    }
}