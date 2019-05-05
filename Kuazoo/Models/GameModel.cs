using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
namespace com.kuazoo.Models
{
    public class GameModel
    {
        [ScaffoldColumn(false)]
        public int GameId { get; set; }
        [Required(ErrorMessage = "*")]
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "*")]
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Required(ErrorMessage = "*")]
        [Display(Name = "Instruction")]
        public string Instruction { get; set; }
        [Required(ErrorMessage = "*")]
        [Display(Name = "Expiry Date")]
        public DateTime ExpiryDate { get; set; }
        public string ExpiryDateStr { get; set; }
        [Required(ErrorMessage = "*")]
        [Display(Name = "Hidden Latitude")]
        public double Latitude { get; set; }
        [Required(ErrorMessage = "*")]
        [Display(Name = "Hidden Longitude")]
        public double Longitude { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Prize")]
        public int PrizeId { get; set; }
        public string PrizeName { get; set; }

        [Required(ErrorMessage = "*")]
        public int ImageId { get; set; }
        public string ImageName { get; set; }
        public string ImageUrl { get; set; }
        public string LastAction { get; set; }
        public DateTime Create { get; set; }
    }
    public class GameModelShow
    {
        [ScaffoldColumn(false)]
        public int GameId { get; set; }
        public string GameName { get; set; }
        public string GameDescritpion { get; set; }
        public string GameInstruction { get; set; }

        [Required(ErrorMessage = "*")]
        public double UserLatitude { get; set; }
        [Required(ErrorMessage = "*")]
        public double UserLongitude { get; set; }

        public int ImageId { get; set; }
        public string ImageName { get; set; }
        public string ImageUrl { get; set; }

        public string OrderNo { get; set; }
        public int TransactionId { get; set; }
        public int GameAttempt { get; set; }
    }
    public class GameModelFinish
    {
        public int InventoryItemId { get; set; }
        public string PrizeName { get; set; }
        public string PrizeImageUrl { get; set; }
        public string OrderNo { get; set; }
        public int AttemptLeft { get; set; }
    }
}