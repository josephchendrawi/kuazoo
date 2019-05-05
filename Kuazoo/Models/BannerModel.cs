using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
namespace com.kuazoo.Models
{
    public class BannerModel
    {
        [ScaffoldColumn(false)]
        public int BannerId { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Banner Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "*")]
        public string Link { get; set; }

        [Required(ErrorMessage = "Image is required")]
        public int SubImageId { get; set; }
        public string SubImageName { get; set; }
        public string SubImageUrl { get; set; }
        public string SubImageUrlLink { get; set; }

        public string LastAction { get; set; }
        public int? Seq { get; set; }
    }
}