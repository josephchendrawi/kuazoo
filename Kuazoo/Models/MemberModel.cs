using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
namespace com.kuazoo.Models
{
    public class MemberModel
    {
        public class Member
        {
            [ScaffoldColumn(false)]
            public int MemberId { get; set; }
            [Required(ErrorMessage = "*")]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }
            [Required(ErrorMessage = "*")]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }
            [Required(ErrorMessage = "*")]
            [Display(Name = "Email")]
            [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "*")]
            public string Email { get; set; }
            [Required(ErrorMessage = "*")]
            [Display(Name = "Gender")]
            public int Gender { get; set; }
            [Display(Name = "Gender")]
            public string GenderStr { get; set; }
            [Display(Name = "Date Of Birth")]
            public DateTime DateOfBirth { get; set; }
            public bool LastLockout { get; set; }
            public DateTime? LastLockoutDate { get; set; }
            [Required(ErrorMessage = "*")]
            [Display(Name = "Status")]
            public MemberStatus MemberStatus { get; set; }
            public Facebook Facebook { get; set; }
            public DateTime Create { get; set; }
        }
        public sealed class MemberStatus
        {
            public int MemberStatusId { get; set; }
            public string MemberStatusName { get; set; }
        }

        public class SubscribeList
        {
            public string Email { get; set; }
            public DateTime SubscribeDate { get; set; }
        }
        public class Facebook
        {
            public int FacebookId { get; set; }
        }

        public class MemberExportData
        {
            public int MemberId { get; set; }
            [Display(Name = "First Name")]
            public string FirstName { get; set; }
            [Display(Name = "Last Name")]
            public string LastName { get; set; }
            [Display(Name = "Email")]
            public string Email { get; set; }
            [Display(Name = "Gender")]
            public string Gender { get; set; }
            [Display(Name = "Date Of Birth")]
            public DateTime DateOfBirth { get; set; }
            [Display(Name = "Status")]
            public string MemberStatus { get; set; }
        }

        public class MemberVM
        {
            public int MemberId { get; set; }
            public string Email { get; set; }

            [Required(ErrorMessage = "*")]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }
            [Required(ErrorMessage = "*")]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }
            [Required(ErrorMessage = "*")]
            [Display(Name = "Gender")]
            public int Gender { get; set; }
            [Display(Name = "Date Of Birth")]
            public DateTime? DateOfBirth { get; set; }
            public int ImageId { get; set; }
            public string ImageName { get; set; }
            public string ImageUrl { get; set; }
            public Boolean Notif { get; set; }

            public Boolean Facebook { get; set; }
            public Boolean Twitter { get; set; }

            public MemberShipping Shipping { get; set; }
            public MemberBilling Billing { get; set; }
        }
        public class MemberShipping
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string AddressLine1 { get; set; }
            public string AddressLine2 { get; set; }
            [Required(ErrorMessage = "*")]
            public string City { get; set; }
            [Required(ErrorMessage = "*")]
            public string State { get; set; }
            [Required(ErrorMessage = "*")]
            public string Country { get; set; }
            [Required(ErrorMessage = "*")]
            public string ZipCode { get; set; }
            public int Gender { get; set; }
            public string Phone { get; set; }
            public bool Gift { get; set; }
            public string Note { get; set; }
        }
        public class MemberBilling
        {
            public int PaymentMethod { get; set; }
            public String PaymentMethodStr { get; set; }
            public String PaymentCC { get; set; }
            public String PaymentCCV { get; set; }
            public int PaymentExpireMonth { get; set; }
            public int PaymentExpireYear { get; set; }

            [Required(ErrorMessage = "*")]
            public string FirstName { get; set; }
            [Required(ErrorMessage = "*")]
            public string LastName { get; set; }
            [Required(ErrorMessage = "*")]
            public string AddressLine1 { get; set; }
            public string AddressLine2 { get; set; }
            [Required(ErrorMessage = "*")]
            public string City { get; set; }
            [Required(ErrorMessage = "*")]
            public string State { get; set; }
            [Required(ErrorMessage = "*")]
            public string Country { get; set; }
            [Required(ErrorMessage = "*")]
            public string ZipCode { get; set; }
            public int Gender { get; set; }
            public string Phone { get; set; }
        }

        public class MemberVM2
        {
            public int MemberId { get; set; }
            public string Email { get; set; }

            [Required(ErrorMessage = "*")]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }
            [Required(ErrorMessage = "*")]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }
            [Required(ErrorMessage = "*")]
            [Display(Name = "Gender")]
            public int Gender { get; set; }
            //[Display(Name = "Date Of Birth")]
            //public DateTime? DateOfBirth { get; set; }
            public int ImageId { get; set; }
            public string ImageName { get; set; }
            public string ImageUrl { get; set; }
            public Boolean Notif { get; set; }

            public Boolean Facebook { get; set; }
            public Boolean Twitter { get; set; }

            public MemberShipping2 Shipping { get; set; }
            public MemberBilling2 Billing { get; set; }

            public int Day { get; set; }
            public int Month { get; set; }
            public int Year { get; set; }
        }
        public class MemberShipping2
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string AddressLine1 { get; set; }
            public string AddressLine2 { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string Country { get; set; }
            public string ZipCode { get; set; }
            public int Gender { get; set; }
            public string Phone { get; set; }
            public bool Gift { get; set; }
            public string Note { get; set; }
        }
        public class MemberBilling2
        {
            public int PaymentMethod { get; set; }
            public String PaymentMethodStr { get; set; }
            public String PaymentCC { get; set; }
            public String PaymentCCV { get; set; }
            public int PaymentExpireMonth { get; set; }
            public int PaymentExpireYear { get; set; }

            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string AddressLine1 { get; set; }
            public string AddressLine2 { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string Country { get; set; }
            public string ZipCode { get; set; }
            public int Gender { get; set; }
            public string Phone { get; set; }
        }
    }
}