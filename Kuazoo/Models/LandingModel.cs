using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
namespace com.kuazoo.Models
{
    public class LandingModel
    {
        public class LandingVM
        {
            public List<BannerModel> ListBanner { get; set; }
            public List<BVDModel> ListBVD { get; set; }
        }

        public class SliderPreview
        {
            public string Link { get; set; }
            public string SubImageUrl { get; set; }
        }
    }
}