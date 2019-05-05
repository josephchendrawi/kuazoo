using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
namespace com.kuazoo.Models
{
    public class MerchantModel
    {
        [ScaffoldColumn(false)]
        public int MerchantId { get; set; }
        
        [Required(ErrorMessage = "*")]
        [Display(Name = "Merchant Name")]
        public string Name { get; set; }

        [Display(Name = "Address line")]
        public string AddressLine1 { get; set; }
        [Display(Name = "Address line 2")]
        public string AddressLine2 { get; set; }

        [Display(Name = "Country")]
        public int CountryId { get; set; }
        [UIHint("ClientCountry")]
        public MasterModel.Country Country { get; set; }

        [Required(ErrorMessage="*")]
        [Display(Name = "City")]
        public int CityId { get; set; }
        [UIHint("ClientCity")]
        public MasterModel.City City { get; set; }

        [Display(Name = "Postal Code")]
        public string PostCode { get; set; }
        [Display(Name = "Contact Number")]
        public string ContactNumber { get; set; }

        [Display(Name = "Email")]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*",ErrorMessage="*")]
        public string Email { get; set; }

        [Display(Name = "Website")]
        //[RegularExpression(@"(http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&:/~\+#]*[\w\-\@?^=%&/~\+#])?",ErrorMessage="*")]
        public string Website { get; set; }

        [Display(Name = "Facebook")]
        //[RegularExpression(@"(http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&:/~\+#]*[\w\-\@?^=%&/~\+#])?", ErrorMessage = "*")]
        public string Facebook { get; set; }

        public string TempLatLong { get; set; }
        [Display(Name = "Latitude")]
        public double Latitude { get; set; }
        [Display(Name = "Longitude")]
        public double Longitude { get; set; }
        [Display(Name = "Status")]
        public int StatusId { get; set; }

        public string LastAction { get; set; }
        public DateTime Create { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        public string SubImageId { get; set; }
        public string SubImageName { get; set; }
        public string SubImageUrl { get; set; }
        public string SubImageUrlLink { get; set; }

    }
}