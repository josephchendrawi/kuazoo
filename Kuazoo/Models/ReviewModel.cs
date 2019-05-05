using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
namespace com.kuazoo.Models
{
    public class ReviewModel
    {
        public List<ReviewVM> ReviewList { get; set; }
        public int InventoryItemId { get; set; }
        public string InventoryItemName { get; set; }
        public string MemberFullName { get; set; }
    }
    public class ReviewVM
    {
        public long ReviewId { get; set; }
        public int InventoryItemId { get; set; }
        public string InventoryItemName { get; set; }
        public string InventoryItemImageUrl { get; set; }
        public string PrizeImageUrl { get; set; }
        public int MemberId { get; set; }
        public string MemberEmail { get; set; }
        public string MemberFullName { get; set; }
        public string MemberImage { get; set; }
        public int Rating { get; set; }
        public string Message { get; set; }
        private DateTime _reviewdate;
        public DateTime ReviewDate { get { return this._reviewdate; } set { this._reviewdate = new DateTime(value.Ticks, DateTimeKind.Utc); } }
        public string ReviewDateStr { get; set; }
        public string LastAction { get; set; }
    }
    public class ReviewVMCount
    {
        public float Rating { get; set; }
        public int ReviewCount { get; set; }
    }
}