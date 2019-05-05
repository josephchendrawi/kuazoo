using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
namespace com.kuazoo.Models
{

    public sealed class CountryModel
    {
        public int CountryId { get; set; }

        [Display(Name = "Country", Description = "Country name.")]
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "*")]
        public string CountryName { get; set; }
        public string LastAction { get; set; }
        public DateTime Create { get; set; }
    }
    public sealed class StateModel
    {
        public int StateId { get; set; }

        [Display(Name = "State", Description = "State name.")]
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "*")]
        public string StateName { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Country")]
        public int CountryId { get; set; }
        public string CountryName { get; set; }

        public string LastAction { get; set; }
        public DateTime Create { get; set; }
    }
    public sealed class CityModel
    {
        public int CityId { get; set; }

        [Display(Name = "City", Description = "City name.")]
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "*")]
        public string CityName { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Country")]
        public int CountryId { get; set; }
        public string CountryName { get; set; }

        public string LastAction { get; set; }
        public DateTime Create { get; set; }
    }
}