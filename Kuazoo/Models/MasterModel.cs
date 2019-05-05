using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace com.kuazoo.Models
{
    public abstract class MasterModel
    {
        public class Country
        {
            public int CountryId { get; set; }
            public string CountryName { get; set; }
        }
        public class City
        {
            public int CityId { get; set; }
            public string CityName { get; set; }
        }
        public class Currency
        {
            public int CurrencyId { get; set; }
            public string CurrencyName { get; set; }
            public string CurrencyCode { get; set; }
        }
    }
}