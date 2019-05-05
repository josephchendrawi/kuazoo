using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
namespace com.kuazoo.Models
{
    public sealed class PointActionModel
    {
        public int PointActionId { get; set; }
        public int Code { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Amount")]
        [Required(ErrorMessage = "*")]
        public decimal Amount { get; set; }
    }
    public sealed class StaticModel
    {
        public int StaticId { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        [Required(ErrorMessage = "*")]
        public string Description { get; set; }
    }
}