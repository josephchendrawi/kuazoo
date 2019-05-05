using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
namespace com.kuazoo.Models
{
    public class BVDModel
    {
        [ScaffoldColumn(false)]
        public int BVDId { get; set; }

        [Required(ErrorMessage = "*")]
        public string Title { get; set; }

        [Required(ErrorMessage = "*")]
        public string Link { get; set; }

        [Required(ErrorMessage = "*")]
        public int Type { get; set; }

        [Required(ErrorMessage = "Image is required")]
        public int SubImageId { get; set; }
        public string SubImageName { get; set; }
        public string SubImageUrl { get; set; }
        public string SubImageUrlLink { get; set; }

        public string LastAction { get; set; }
        public int? Seq { get; set; }

        [Display(Name = "Date")]
        public DateTime? UpdatedDate { get; set; }
    }
}