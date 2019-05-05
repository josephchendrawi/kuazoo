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
    public interface ITransactionService
    {
        //[OperationContract]
        //Response<bool> CreateTransaction(Transaction Transaction);
        [OperationContract]
        Response<List<Transaction>> GetTransactionList();
        [OperationContract]
        Response<List<TransactionDetail>> GetTransactionDetailByInventoryItemId(int InventoryItemId);
        //[OperationContract]
        //Response<bool> DeleteTransaction(int TransactionId);
    }

    [DataContract]
    public class Transaction
    {
        [DataMember]
        public int MerchantId { get; set; }
        [DataMember]
        public string MerchantName { get; set; }
        [DataMember]
        public int InventoryItemId { get; set; }
        [DataMember]
        public string InventoryItemName { get; set; }
        [DataMember]
        public decimal Sales { get; set; }
        [DataMember]
        public decimal NewOrder { get; set; }
        [DataMember]
        public decimal MaximumSales { get; set; }
        [DataMember]
        public decimal SalesVisualMeter { get; set; }
        [DataMember]
        public DateTime ExpireDate { get; set; }
    }
    [DataContract]
    public class TransactionDetail
    {
        [DataMember]
        public int No { get; set; }
        [DataMember]
        public int TransactionId { get; set; }
        [DataMember]
        public int MerchantId { get; set; }
        [DataMember]
        public string MerchantName { get; set; }
        [DataMember]
        public int InventoryItemId { get; set; }
        [DataMember]
        public string InventoryItemName { get; set; }
        [DataMember]
        public int MemberId { get; set; }
        [DataMember]
        public string MemberEmail { get; set; }
        [DataMember]
        public bool MemberPlay { get; set; }
        [DataMember]
        public DateTime TransactionDate { get; set; }
        [DataMember]
        public int? FlashDealId { get; set; }
        public string LastAction { get; set; }
        [DataMember]
        public Member Member { get; set; }
        [DataMember]
        public InventoryItem InventoryItem { get; set; }
        [DataMember]
        public FlashDeal FlashDeal { get; set; }
        [DataMember]
        public int TransactionTypeId { get; set; }
        [DataMember]
        public string TransactionType { get; set; }
        [DataMember]
        public DateTime? ProcessDate { get; set; }
        [DataMember]
        public string PaymentStatus { get; set; }
    }

    [DataContract]
    public class TransactionDetail2
    {
        [DataMember]
        public int TransactionId { get; set; }
        [DataMember]
        public string OrderNo { get; set; }
        [DataMember]
        public DateTime TransactionDate { get; set; }
        [DataMember]
        public int MerchantId { get; set; }
        [DataMember]
        public string MerchantName { get; set; }
        [DataMember]
        public int InventoryItemId { get; set; }
        [DataMember]
        public string InventoryItemName { get; set; }
        [DataMember]
        public string SKU { get; set; }
        [DataMember]
        public int Qty { get; set; }
        [DataMember]
        public int MemberId { get; set; }
        [DataMember]
        public string MemberFirstName { get; set; }
        [DataMember]
        public string MemberLastName { get; set; }
        [DataMember]
        public string MemberEmail { get; set; }
        [DataMember]
        public string Phone { get; set; }
        [DataMember]
        public string AddressLine1 { get; set; }
        [DataMember]
        public string AddressLine2 { get; set; }
        [DataMember]
        public string City { get; set; }
        [DataMember]
        public string State { get; set; }
        [DataMember]
        public string Country { get; set; }
        [DataMember]
        public string ZipCode { get; set; }
    }
    [DataContract]
    public class TransactionDetail3
    {
        [DataMember]
        public int TransactionId { get; set; }
        [DataMember]
        public DateTime TransactionDate { get; set; }
        [DataMember]
        public int MerchantId { get; set; }
        [DataMember]
        public string MerchantName { get; set; }
        [DataMember]
        public int InventoryItemId { get; set; }
        [DataMember]
        public string InventoryItemName { get; set; }
        [DataMember]
        public string Variance { get; set; }
        [DataMember]
        public string SKU { get; set; }
        [DataMember]
        public int Qty { get; set; }
        [DataMember]
        public string VoucherCode { get; set; }
        [DataMember]
        public int MemberId { get; set; }
        [DataMember]
        public string MemberFirstName { get; set; }
        [DataMember]
        public string MemberLastName { get; set; }
        [DataMember]
        public string MemberEmail { get; set; }
        [DataMember]
        public string MemberPhone { get; set; }
        [DataMember]
        public string MemberAddress1 { get; set; }
        [DataMember]
        public string MemberAddress2 { get; set; }
        [DataMember]
        public string MemberCity { get; set; }
        [DataMember]
        public string MemberState { get; set; }
        [DataMember]
        public string MemberPostCode { get; set; }
        [DataMember]
        public string MemberCountry { get; set; }
        [DataMember]
        public string TransactionStatus { get; set; }
        [DataMember]
        public string ProcessStatus { get; set; }
        [DataMember]
        public DateTime? ProcessDate { get; set; }
        [DataMember]
        public string PaymentStatusId { get; set; }
        [DataMember]
        public string PaymentStatus { get; set; }
        [DataMember]
        public DateTime? PaymentDate { get; set; }
        [DataMember]
        public decimal PaymentAmount { get; set; }
        [DataMember]
        public string PaymentMethod { get; set; }
        [DataMember]
        public bool Gift { get; set; }
        [DataMember]
        public string GiftNote { get; set; }
    }


    [DataContract]
    public class TransactionsHeader
    {
        [DataMember]
        public int TransactionId { get; set; }
        [DataMember]
        public int MemberId { get; set; }
        [DataMember]
        public DateTime TransactionDate { get; set; }
        [DataMember]
        public int TransactionStatus { get; set; }
        [DataMember]
        public string PaymentStatus { get; set; }
        [DataMember]
        public string VoucherCode { get; set; }
        [DataMember]
        public string LastAction { get; set; }
        [DataMember]
        public List<TransactionsDetail> Detail { get; set; }
        [DataMember]
        public Bill shipping { get; set; }
        [DataMember]
        public Bill billing { get; set; }
        [DataMember]
        public int PromotionId { get; set; }
        [DataMember]
        public int PromotionType { get; set; }
        [DataMember]
        public decimal PromotionValue { get; set; }
        [DataMember]
        public decimal KPoint { get; set; }

        public string tranID { get; set; }
        public string orderid { get; set; }
        public string status { get; set; }
        public string domain { get; set; }
        public decimal amount { get; set; }
        public string currency { get; set; }
        public string appcode { get; set; }
        public DateTime paydate { get; set; }
        public string skey { get; set; }
        public string channel { get; set; }
        public string error_code { get; set; }
        public string error_desc { get; set; }
        public bool reviewed { get; set; }
        [DataMember]
        public List<InventoryItemShow> SimilarDetail { get; set; }
    }
    [DataContract]
    public class CallbackPayment
    {
        public string nbcb { get; set; }
        public string tranID { get; set; }
        public string orderid { get; set; }
        public string status { get; set; }
        public string domain { get; set; }
        public decimal amount { get; set; }
        public string currency { get; set; }
        public string appcode { get; set; }
        public DateTime paydate { get; set; }
        public string skey { get; set; }
        public string channel { get; set; }
        public string error_code { get; set; }
        public string error_desc { get; set; }
    }
    [DataContract]
    public class TransactionsDetail
    {
        [DataMember]
        public int TransactionDetailId { get; set; }
        [DataMember]
        public int TransactionId { get; set; }
        [DataMember]
        public int InventoryItemId { get; set; }
        [DataMember]
        public string InventoryItemName { get; set; }
        [DataMember]
        public string InventoryItemImageUrl { get; set; }
        [DataMember]
        public decimal InventoryItemRemainSales { get; set; }
        [DataMember]
        public decimal InventoryItemSalesVisualMeter { get; set; }
        [DataMember]
        public int? FlashDealId { get; set; }
        [DataMember]
        public string Variance { get; set; }
        [DataMember]
        public int Qty { get; set; }
        [DataMember]
        public decimal Discount{ get; set; }
        [DataMember]
        public decimal Price { get; set; }
        [DataMember]
        public int PrizeId { get; set; }
        [DataMember]
        public string PrizeName { get; set; }
        [DataMember]
        public string PrizeNameImageUrl { get; set; }
        [DataMember]
        public DateTime PrizeExpire { get; set; }
    }


    [DataContract]
    public class MinimumTarget
    {
        [DataMember]
        public int MerchantId { get; set; }
        [DataMember]
        public string MerchantName { get; set; }
        [DataMember]
        public int InventoryItemId { get; set; }
        [DataMember]
        public string InventoryItemName { get; set; }
        [DataMember]
        public decimal Minimum { get; set; }
        [DataMember]
        public decimal CurrentSales { get; set; }
    }
    [DataContract]
    public class Bill
    {
        [DataMember]
        public int PaymentMethod { get; set; }
        [DataMember]
        public String PaymentCC { get; set; }
        [DataMember]
        public String PaymentCCV { get; set; }
        [DataMember]
        public int PaymentExpireMonth { get; set; }
        [DataMember]
        public int PaymentExpireYear { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        [DataMember]
        public string City { get; set; }
        [DataMember]
        public string State { get; set; }
        [DataMember]
        public string Country { get; set; }
        [DataMember]
        public string ZipCode { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public int Gender { get; set; }
        [DataMember]
        public string Phone { get; set; }
        [DataMember]
        public bool Gift { get; set; }
        [DataMember]
        public string Note { get; set; }

        public string Password { get; set; }
    }
    [DataContract]
    public class OrderItem
    {
        [DataMember]
        public int id { get; set; }
        [DataMember]
        public string variance { get; set; }
        [DataMember]
        public int qty { get; set; }
        [DataMember]
        public int flashid { get; set; }
        [DataMember]
        public decimal discount { get; set; }
        [DataMember]
        public decimal price { get; set; }
    }
    [DataContract]
    public class BillingC
    {
        [DataMember]
        public string payment { get; set; }
        [DataMember]
        public string cc { get; set; }
        [DataMember]
        public string ccv { get; set; }
        [DataMember]
        public string month{ get; set; }
        [DataMember]
        public string year { get; set; }
        [DataMember]
        public int gender { get; set; }
        [DataMember]
        public string fn { get; set; }
        [DataMember]
        public string ln { get; set; }
        [DataMember]
        public string ad1 { get; set; }
        [DataMember]
        public string ad2 { get; set; }
        [DataMember]
        public string city { get; set; }
        [DataMember]
        public string state { get; set; }
        [DataMember]
        public string zip { get; set; }
        [DataMember]
        public string country { get; set; }
        [DataMember]
        public string email { get; set; }
        [DataMember]
        public string ph { get; set; }
        [DataMember]
        public string pass { get; set; }
    }
    [DataContract]
    public class ShippingC
    {
        [DataMember]
        public int gender { get; set; }
        [DataMember]
        public string fn { get; set; }
        [DataMember]
        public string ln { get; set; }
        [DataMember]
        public string ad1 { get; set; }
        [DataMember]
        public string ad2 { get; set; }
        [DataMember]
        public string ph { get; set; }
        [DataMember]
        public string city { get; set; }
        [DataMember]
        public string state { get; set; }
        [DataMember]
        public string zip { get; set; }
        [DataMember]
        public string country { get; set; }
        [DataMember]
        public int gift { get; set; }
        [DataMember]
        public string note { get; set; }
        [DataMember]
        public string rcemail { get; set; }
    }
    [DataContract]
    public class PromotionCode
    {
        [DataMember]
        public int PromotionId { get; set; }
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public int Type { get; set; }
        [DataMember]
        public string TypeName { get; set; }
        [DataMember]
        public decimal Value { get; set; }
    }

    [DataContract]
    public class TransactionHtml
    {
        [DataMember]
        public string customer {get;set;}
        [DataMember]
        public string merchant {get;set;}
        [DataMember]
        public string orderno {get;set;}
        [DataMember]
        public DateTime transactiondate {get;set;}
        [DataMember]
        public decimal amount {get;set;}
        [DataMember]
        public string paymenttype {get;set;}
        [DataMember]
        public string url {get;set;}
        [DataMember]
        public string dealname {get;set;}
        [DataMember]
        public string customeraddress {get;set;}
        [DataMember]
        public string sku {get;set;}
        [DataMember]
        public decimal price {get;set;}
        [DataMember]
        public decimal promo {get;set;}
        [DataMember]
        public string promocode {get;set;}
        [DataMember]
        public decimal kpoint {get;set;}
        [DataMember]
        public string dealinterest {get;set;}
        [DataMember]
        public string mailto {get;set;}
        [DataMember]
        public string imageurl {get;set;}
        [DataMember]
        public string fineprint {get;set;}
        [DataMember]
        public string vouchercode { get; set; }
    }
    [DataContract]
    public class TempCalculation
    {
        [DataMember]
        public decimal RemainSales { get; set; }
        [DataMember]
        public decimal VisualMeter{ get; set; }
    }
}
