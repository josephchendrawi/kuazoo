using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
namespace com.kuazoo.Models
{
    public class BillModel
    {
        public class Bill
        {
            //[Required(ErrorMessage = "*")]
            //[Display(Name = "First Name")]
            //public string FirstName { get; set; }
            //[Required(ErrorMessage = "*")]
            //[Display(Name = "Last Name")]
            //public string LastName { get; set; }
            //[Required(ErrorMessage = "*")]
            //[Display(Name = "Address line")]
            //public string AddressLine1 { get; set; }
            //[Display(Name = "Address line 2")]
            //public string AddressLine2 { get; set; }
            //[Required(ErrorMessage = "*")]
            //[Display(Name = "City")]
            //public string City { get; set; }
            //[Required(ErrorMessage = "*")]
            //[Display(Name = "State")]
            //public string State { get; set; }
            //[Required(ErrorMessage = "*")]
            //[Display(Name = "Country")]
            //public string Country { get; set; }
            //[Required(ErrorMessage = "*")]
            //[Display(Name = "Zip Code")]
            //public string ZipCode { get; set; }
            //[Required(ErrorMessage = "*")]
            //[Display(Name = "Email")]
            //[RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "*")]
            //public string Email { get; set; }
            //[Required(ErrorMessage = "*")]
            //[Display(Name = "Gender")]
            //public int Gender { get; set; }
            //[Required(ErrorMessage = "*")]
            //[Display(Name = "Phone")]
            //public string Phone { get; set; }


            //[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
            //[DataType(DataType.Password)]
            //[Display(Name = "New password")]
            //public string NewPassword { get; set; }

            //[DataType(DataType.Password)]
            //[Display(Name = "Confirm new password")]
            //[Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            //public string ConfirmPassword { get; set; }


            public string Shipping { get; set; }
            public string Billing { get; set; }

            public string Order { get; set; }

            public int PromotionId { get; set; }
            public int PromotionType { get; set; }
            public decimal PromotionValue { get; set; }
            public int KPoint { get; set; }

            public int PaymentType { get; set; }
            public int TransactionId { get; set; }
            public string MOLUrl { get; set; }
        }
        public class BillCB
        {
            public string nbcb { get; set; }
            public string tranID { get; set; }
            public string orderid { get; set; }
            public string status { get; set; }
            public string domain { get; set; }
            public string amount { get; set; }
            public string currency { get; set; }
            public string appcode { get; set; }
            public string paydate { get; set; }
            public string skey { get; set; }
            public string channel { get; set; }
            public string error_code { get; set; }
            public string error_desc { get; set; }
        }
    }
}