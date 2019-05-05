using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace com.kuazoo
{
    [ServiceContract]
    public interface IInventoryItemService
    {
        [OperationContract]
        Response<bool> CreateInventoryItem(InventoryItem InventoryItem);
        [OperationContract]
        Response<List<InventoryItem>> GetInventoryItemList();
        [OperationContract]
        Response<bool> DeleteInventoryItem(int InventoryItemId);
    }

    [DataContract]
    public class InventoryItemType
    {
        [DataMember]
        public int InventoryItemTypeId { get; set; }
        [DataMember]
        public string InventoryItemTypeName { get; set; }
    }


    [DataContract]
    public class InventoryItem
    {
        [DataMember]
        public int InventoryItemId { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string ShortDesc { get; set; }
        [DataMember]
        public decimal Price { get; set; }
        [DataMember]
        public decimal Margin { get; set; }
        [DataMember]
        public string GeneralDescription { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string Terms { get; set; }
        [DataMember]
        public Merchant Merchant { get; set; }
        [DataMember]
        public Country Country { get; set; }
        [DataMember]
        public City City { get; set; }
        [DataMember]
        public string Keyword { get; set; }
        [DataMember]
        public InventoryItemType InventoryItemType { get; set; }
        [DataMember]
        public decimal Discount { get; set; }
        [DataMember]
        public DateTime ExpireDate { get; set; }
        [DataMember]
        public Decimal MaximumSales { get; set; }
        [DataMember]
        public Decimal RemainSales { get; set; }
        [DataMember]
        public DateTime PublishDate { get; set; }
        [DataMember]
        public Decimal MinimumTarget { get; set; }
        [DataMember]
        public Decimal SalesVisualMeter { get; set; }
        public string Tag { get; set; }
        public string TagName { get; set; }

        public int ImageId { get; set; }
        public string ImageName { get; set; }
        public string ImageUrl { get; set; }

        public List<int> SubImageId { get; set; }
        public List<string> SubImageName { get; set; }
        public List<string> SubImageUrl { get; set; }

        [DataMember]
        public Admin CreateBy { get; set; }
        [DataMember]
        public DateTime Create { get; set; }
        [DataMember]
        public DateTime Update { get; set; }
        [DataMember]
        public Boolean Flag { get; set; }
        [DataMember]
        public Boolean Featured { get; set; }
        public string LastAction { get; set; }

        public int PrizeId { get; set; }
        public string PrizeName { get; set; }
        public Prize Prize { get; set; }
        public FlashDeal FlashDeal { get; set; }
        public List<VarianceDetail> Variance { get; set; }
        public Boolean Draft { get; set; }

        public int Priority { get; set; }
        [DataMember]
        public Boolean FreeDeal { get; set; }
        [DataMember]
        public Boolean Popular { get; set; }
        [DataMember]
        public Boolean Reviewed { get; set; }
    }

    [DataContract]
    public class InventoryItemShow
    {
        [DataMember]
        public int InventoryItemId { get; set; }
        [DataMember]
        public int TypeId { get; set; }
        [DataMember]
        public string TypeName { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string ShortDesc { get; set; }
        [DataMember]
        public decimal Price { get; set; }
        [DataMember]
        public string GeneralDescription { get; set; }
        [DataMember]
        public decimal Discount { get; set; }
        [DataMember]
        public Decimal Sales { get; set; }
        [DataMember]
        public Decimal MaximumSales { get; set; }
        [DataMember]
        public Decimal SalesVisualMeter { get; set; }
        [DataMember]
        public DateTime ExpireDate { get; set; }


        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string Terms { get; set; }
        [DataMember]
        public List<string> Tag { get; set; }

        [DataMember]
        public string MerchantName { get; set; }
        [DataMember]
        public string MerchantContactNumber { get; set; }
        [DataMember]
        public string MerchantEmail { get; set; }
        [DataMember]
        public string MerchantWebsite { get; set; }
        [DataMember]
        public string MerchantFacebook { get; set; }
        [DataMember]
        public string AddressLine1 { get; set; }
        [DataMember]
        public string AddressLine2 { get; set; }
        [DataMember]
        public string MerchantCityName { get; set; }
        [DataMember]
        public string MerchantCountryName { get; set; }
        [DataMember]
        public double Latitude { get; set; }
        [DataMember]
        public double Longitude { get; set; }
        [DataMember]
        public string PostCode { get; set; }
        [DataMember]
        public string MerchantDescription { get; set; }
        public List<int> MerchantSubImageId { get; set; }
        public List<string> MerchantSubImageName { get; set; }
        public List<string> MerchantSubImageUrl { get; set; }
        public List<string> MerchantSubImageUrlLink { get; set; }


        public int ImageId { get; set; }
        public string ImageName { get; set; }
        public string ImageUrl { get; set; }

        public List<int> SubImageId { get; set; }
        public List<string> SubImageName { get; set; }
        public List<string> SubImageUrl { get; set; }

        public Prize Prize { get; set; }
        public FlashDeal FlashDeal { get; set; }
        public List<VarianceDetail> Variance { get; set; }

        public bool Wishlist { get; set; }
        public bool Featured { get; set; }
        public int FeatureSeq { get; set; }
        public string FeaturedText { get; set; }
        [DataMember]
        public Boolean FreeDeal { get; set; }
    }
    [DataContract]
    public class InventoryItemTag
    {
        [DataMember]
        public int InventoryItemId { get; set; }
        [DataMember]
        public string InventoryItemName { get; set; }
        [DataMember]
        public int TagId { get; set; }
        [DataMember]
        public string TagName { get; set; }
    }

    [DataContract]
    public class Wishlist
    {
        [DataMember]
        public int WishlistId { get; set; }
        [DataMember]
        public InventoryItemShow InventoryItem { get; set; }

    }

    [DataContract]
    public class InventoryFeatured
    {
        [DataMember]
        public int InventoryItemId { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public Boolean Featured { get; set; }
        [DataMember]
        public int FeatureSeq { get; set; }
        [DataMember]
        public string FeaturedText { get; set; }
    }

    [DataContract]
    public class VarianceDetail
    {
        [DataMember]
        public string Variance { get; set; }
        [DataMember]
        public decimal Price { get; set; }
        [DataMember]
        public decimal Discount { get; set; }
        [DataMember]
        public decimal Margin { get; set; }
        [DataMember]
        public int AvailabelLimit { get; set; }
        [DataMember]
        public int Used { get; set; }
    }
}
