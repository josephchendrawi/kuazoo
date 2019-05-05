using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
namespace com.kuazoo.Models
{
    public class InventoryItemModel
    {    
        public class InventoryItem
        {
            //[ScaffoldColumn(false)]
            public int InventoryItemId { get; set; }

            //[Required(ErrorMessage = "*")]
            [Display(Name = "Inventory Item Name")]
            public string Name { get; set; }
            [Display(Name = "Short Description")]
            public string ShortDesc { get; set; }

            //[Required(ErrorMessage = "*")]
            [Display(Name = "Original Price")]
            public decimal Price { get; set; }

            //[Required(ErrorMessage = "*")]
            [Display(Name = "Margin")]
            public decimal Margin { get; set; }

            public int CreateBy { get; set; }

            [Display(Name = "General Description")]
            public string GeneralDescription { get; set; }
            [Display(Name = "Product Description")]
            [AllowHtml]
            public string Description { get; set; }
            [Display(Name = "Terms & Conditions")]
            public string Terms { get; set; }

            //[Required(ErrorMessage = "*")]
            [Display(Name = "Merchant")]
            public int MerchantId { get; set; }
            public string MerchantName { get; set; }
            public string MerchantCityName { get; set; }
            public string MerchantCountryName { get; set; }

            [Display(Name = "Country")]
            public int CountryId { get; set; }
            [UIHint("ClientCountry")]
            public MasterModel.Country Country { get; set; }

            //[Required(ErrorMessage = "*")]
            [Display(Name = "City")]
            public int CityId { get; set; }
            [UIHint("ClientCity")]
            public MasterModel.City City { get; set; }

            [Display(Name = "Keywords")]
            public string Keyword { get; set; }

            //[Required(ErrorMessage = "*")]
            [Display(Name = "Inventory Item Type")]
            public int InventoryItemTypeId { get; set; }
            public string InventoryItemTypeName { get; set; }

            //[Required(ErrorMessage = "*")]
            [Display(Name ="Discount")]
            public decimal Discount { get; set; }

            //[Required(ErrorMessage = "*")]
            [Display(Name="Expiry Date")]
            public DateTime ExpireDate { get; set; }

            //[Required(ErrorMessage = "*")]
            [Display(Name="Maximum Sales")]
            public Decimal MaximumSales { get; set; }
            //[Required(ErrorMessage = "*")]
            [Display(Name = "Remain Sales")]
            public Decimal RemainSales { get; set; }
            //[Required(ErrorMessage = "*")]
            [Display(Name = "Revenue")]
            public Decimal Revenue{ get; set; }

            //[Required(ErrorMessage = "*")]
            [Display(Name="Publish Date")]
            public DateTime PublishDate { get; set; }

            //[Required(ErrorMessage = "*")]
            [Display(Name = "Minimum Sales")]
            public Decimal MinimumTarget { get; set; }

            //[Required(ErrorMessage = "*")]
            [Display(Name = "Sales Visual Meter")]
            public Decimal SalesVisualMeter { get; set; }

            [Display(Name = "Tag")]
            public string Tag { get; set; }
            public string TagName { get; set; }

            //[Required(ErrorMessage = "*")]
            public int ImageId { get; set; }
            public string ImageName { get; set; }
            public string ImageUrl { get; set; }
            public string SubImageId { get; set; }
            public string SubImageName { get; set; }
            public string SubImageUrl { get; set; }

            [Display(Name = "Active Status")]
            public Boolean Flag { get; set; }
            [Display(Name = "Set as Featured Deal?")]
            public Boolean Featured { get; set; }

            public string LastAction { get; set; }


            //[Required(ErrorMessage = "*")]
            [Display(Name = "Prize")]
            public int PrizeId { get; set; }
            public string PrizeName{ get; set; }

            [Display(Name = "Variance")]
            public string Variance { get; set; }
            public string TempVariance { get; set; }

            //public Prize Prize { get; set; }
            public FlashDeal FlashDeal { get; set; }
            //public FlashDealDraft FlashDealDraft { get; set; }
            public DateTime Create { get; set; }
            public int Limit { get; set; }
            public String VarianceName { get; set; }
            public bool Kuazooed { get; set; }
            public Boolean Draft { get; set; }
            public int? Priority { get; set; }
            public Boolean Popular { get; set; }
            public Boolean Reviewed { get; set; }
        }

        public class InventoryFeatured
        {
            public int InventoryItemId { get; set; }
            public string Name { get; set; }
            public Boolean Featured { get; set; }
            public int FeatureSeq { get; set; }
            public string FeaturedText { get; set; }
        }
        public class InventoryItemShow
        {
            public int InventoryItemId { get; set; }
            public int TypeId { get; set; }
            public string TypeName { get; set; }
            public string Name { get; set; }
            public string ShortDesc { get; set; }
            public decimal Price { get; set; }
            public decimal Discount { get; set; }
            public Decimal Sales { get; set; }
            public Decimal MaximumSales { get; set; }
            public Decimal SalesVisualMeter { get; set; }
            //public DateTime ExpireDate { get; set; }
            //public String ExpireDateStr { get; set; }


            public string GeneralDescription { get; set; }
            public string Description { get; set; }
            public string Terms { get; set; }
            public List<string> Tag { get; set; }

            public string MerchantName { get; set; }
            public string MerchantContactNumber { get; set; }
            public string MerchantEmail { get; set; }
            public string MerchantWebsite { get; set; }
            public string MerchantFacebook { get; set; }
            public string AddressLine1 { get; set; }
            public string AddressLine2 { get; set; }
            public string MerchantCityName { get; set; }
            public string MerchantCountryName { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public string PostCode { get; set; }
            public string MerchantDescription { get; set; }
            public List<int> MerchantSubImageId { get; set; }
            public List<string> MerchantSubImageName { get; set; }
            public List<string> MerchantSubImageUrl { get; set; }
            public List<string> MerchantSubImageUrlLink { get; set; }

            public string Variance { get; set; }

            public int ImageId { get; set; }
            public string ImageName { get; set; }
            public string ImageUrl { get; set; }

            public List<int> SubImageId { get; set; }
            public List<string> SubImageName { get; set; }
            public List<string> SubImageUrl { get; set; }

            public PrizeShow Prize { get; set; }
            public FlashDeal FlashDeal { get; set; }
            public ReviewVMCount ReviewCount { get; set; }

            public int WishlistId { get; set; }
            public bool Wishlist { get; set; }
            public int FeatureSeq { get; set; }
            public string FeaturedText { get; set; }
        }
        public class InventoryItemVM
        {
            public List<InventoryItemShow> item { get; set; }
            public int TotalCount { get; set; }
            public bool HasNext { get; set; }
            public int NextPage { get; set; }
        }
        public sealed class InventoryItemType
        {
            public int InventoryItemTypeId { get; set; }
            public string InventoryItemTypeName { get; set; }
        }

        public sealed class InventoryItemTag
        {
            public InventoryItem InventoryItemvm { get; set; }
            public int TagId { get; set; }
            public string TagName { get; set; }
            public Tag Tag { get; set; }
        }
        public sealed class Tag
        {
            public int TagId { get; set; }

            [Display(Name = "Tag", Description = "Tag name.")]
            [DataType(DataType.Text)]
            [Required(ErrorMessage = "*")]
            public string TagName { get; set; }

            [Display(Name = "Show As Category")]
            public bool ShowAsCategory { get; set; }

            [Display(Name = "Tag Parent")]
            public Nullable<int> ParentId { get; set; }
            public string ParentName { get; set; }
            public string LastAction { get; set; }
            public DateTime Create { get; set; }
        }
        public sealed class FlashDeal
        {
            [ScaffoldColumn(false)]
            public int FlashDealId { get; set; }

            [Display(Name = "Merchant")]
            public int MerchantId { get; set; }
            public string MerchantName { get; set; }

            //[Required(ErrorMessage = "*")]
            [Display(Name = "Inventory Item")]
            public int InventoryItemId { get; set; }
            public string InventoryItemName { get; set; }

            //[Required(ErrorMessage = "*")]
            [Display(Name = "Additional Discount")]
            public decimal Discount { get; set; }
            //[Required(ErrorMessage = "*")]
            [Display(Name = "Start Time")]
            public DateTime StartTime { get; set; }
            public String StartTimeStr { get; set; }
            //[Required(ErrorMessage = "*")]
            [Display(Name = "End Time")]
            public DateTime EndTime { get; set; }
            public String EndTimeStr { get; set; }
            [Display(Name = "Active")]
            public bool Flag { get; set; }
            public string LastAction { get; set; }
        }
        public sealed class FlashDealDraft
        {
            public string FlashDealId { get; set; }
            public string Discount { get; set; }
            public string StartTime { get; set; }
            public string EndTime { get; set; }
            public string Flag { get; set; }
        }
        public sealed class Prize
        {
            [ScaffoldColumn(false)]
            public int PrizeId { get; set; }
            [Required(ErrorMessage = "*")]
            [Display(Name = "Name")]
            public string Name { get; set; }
            [Required(ErrorMessage = "*")]
            [Display(Name = "Price")]
            public Decimal Price { get; set; }
            [Display(Name = "Description")]
            public string Description { get; set; }
            [Display(Name = "Expiry Date")]
            public DateTime ExpiryDate { get; set; }
            public string ExpiryDateStr { get; set; }
            [Display(Name = "Publish Date")]
            public DateTime PublishDate { get; set; }
            [Display(Name = "Group Name")]
            public string GroupName { get; set; }
            [Display(Name = "Total Revenue")]
            public Decimal TotalRevenue { get; set; }
            [Display(Name = "Sponsor Name")]
            public string SponsorName { get; set; }
            [Display(Name = "Terms")]
            public string Terms { get; set; }
            [Display(Name = "How to Redeem")]
            public string Detail { get; set; }
            [Required(ErrorMessage = "*")]
            public int ImageId { get; set; }
            public string ImageName { get; set; }
            public string ImageUrl { get; set; }
            public string SubImageId { get; set; }
            public string SubImageName { get; set; }
            public string SubImageUrl { get; set; }
            public string LastAction { get; set; }
            public string WinnerEmail { get; set; }
            public string ClosestWinnerEmail { get; set; }
            public DateTime Create { get; set; }
            public int GameType { get; set; }
            [Required(ErrorMessage = "*")]
            [Display(Name = "Sales Visual Meter")]
            public Decimal FakeVisualMeter { get; set; }
            public bool FreeDeal { get; set; }

        }
        public sealed class PrizeShow
        {
            public int PrizeId { get; set; }
            public string Name { get; set; }
            public Decimal Price { get; set; }
            public string Description { get; set; }
            private DateTime _expirydate;
            public DateTime ExpiryDate { get; set; }
            public String ExpiryDateStr { get; set; }
            public string GroupName { get; set; }
            public string SponsorName { get; set; }
            public string Terms { get; set; }
            public string Detail { get; set; }
            public int ImageId { get; set; }
            public string ImageName { get; set; }
            public string ImageUrl { get; set; }
            public List<int> SubImageId { get; set; }
            public List<string> SubImageName { get; set; }
            public List<string> SubImageUrl { get; set; }
            public int GameType { get; set; }
            public bool FreeDeal { get; set; }
        }
        public sealed class Image
        {
            public int ImageId { get; set; }

            [Display(Name = "Image")]
            [DataType(DataType.Text)]
            [Required(ErrorMessage = "*")]
            public string Name { get; set; }
            public string Url { get; set; }
            public string LastAction { get; set; }
        }
        public sealed class ImageList
        {
            public List<Image> List { get; set; }
        }
        public class InventoryItemExportData
        {
            public int InventoryItemId { get; set; }
            [Display(Name = "Inventory Item Name")]
            public string InventoryItemName { get; set; }
            [Display(Name = "Short Description")]
            public string ShortDesc { get; set; }
            [Display(Name = "Original Price")]
            public decimal Price { get; set; }
            [Display(Name = "General Description")]
            public string GeneralDescription { get; set; }
            [Display(Name = "Product Description")]
            public string Description { get; set; }
            [Display(Name = "Merchant")]
            public int MerchantId { get; set; }
            public string MerchantName { get; set; }
            public string CityName { get; set; }
            public string CountryName { get; set; }
            [Display(Name = "Keywords")]
            public string Keyword { get; set; }
            public string InventoryItemType { get; set; }
            [Display(Name = "Discount")]
            public decimal Discount { get; set; }
            [Display(Name = "Expiry Date")]
            public DateTime ExpireDate { get; set; }
            [Display(Name = "Maximum Sales")]
            public Decimal MaximumSales { get; set; }
            [Display(Name = "Publish Date")]
            public DateTime PublishDate { get; set; }
            [Display(Name = "Minimum Target")]
            public Decimal MinimumTarget { get; set; }
            public string Tag { get; set; }
            public string Status { get; set; }

            public int PrizeId { get; set; }
            public string PrizeName { get; set; }
            [Display(Name = "Price")]
            public Decimal PrizePrice { get; set; }
            [Display(Name = "Description")]
            public string PrizeDescription { get; set; }
            [Display(Name = "Sponsor Name")]
            public string SponsorName { get; set; }
            [Display(Name = "Terms")]
            public string Terms { get; set; }
            [Display(Name = "How to Redeem")]
            public string HowToRedeem { get; set; }
        }
        public sealed class Promotion
        {
            public int PromotionId { get; set; }

            [Display(Name = "Promotion Code")]
            [DataType(DataType.Text)]
            [Required(ErrorMessage = "*")]
            public string PromoCode { get; set; }

            [Display(Name = "Type")]
            public int PromoType { get; set; }
            public string PromoTypeStr { get; set; }

            [Display(Name = "Value")]
            public decimal PromoValue { get; set; }

            [Display(Name = "Valid Date")]
            private DateTime _validdate;
            public DateTime ValidDate { get { return this._validdate; } set { this._validdate = new DateTime(value.Ticks, DateTimeKind.Utc); } }
           
            [Display(Name = "Active Status")]
            public bool Flag { get; set; }

            public string LastAction { get; set; }
            public DateTime Create { get; set; }
        }

        public class WinnerShowModel
        {
            public List<WinnerShowVM> Item { get; set; }
            public int Total { get; set; }
        }
        public class WinnerShowVM
        {
            public string Sort { get; set; }
            public List<WinnerShow> WinnerList { get; set; }
        }
        public class WinnerShow
        {
            public int WinnerId { get; set; }
            public string PrizeName { get; set; }
            public string PrizeImageUrl { get; set; }
            public DateTime WinnerDate { get; set; }

            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string MemberImageUrl { get; set; }
            public string Email { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string Country { get; set; }
        }
    }
}