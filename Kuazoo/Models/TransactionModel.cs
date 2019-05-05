using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
namespace com.kuazoo.Models
{
    public class TransactionModel
    {
        public class Transaction
        {
            public int MerchantId { get; set; }
            public string MerchantName { get; set; }
            public int InventoryItemId { get; set; }
            public string InventoryItemName { get; set; }
            public decimal Sales { get; set; }
            public decimal MaximumSales { get; set; }
            public decimal NewOrder { get; set; }
            private DateTime _expiredate;
            public DateTime ExpireDate { get; set; }
            public String ExpireDateStr { get; set; }
            public bool Kuazooed { get; set; }
        }
        public class TransactionDetail
        {
            public int TransactionId { get; set; }
            public string OrderId { get; set; }
            public int MerchantId { get; set; }
            public string MerchantName { get; set; }
            public int InventoryItemId { get; set; }
            public string InventoryItemName { get; set; }
            public int MemberId { get; set; }
            public string MemberEmail { get; set; }
            private DateTime _transactiondate;
            public DateTime TransactionDate { get; set; }

            public String TransactionDateStr { get; set; }
            public int? FlashDealId { get; set; }
            public string LastAction { get; set; }
            public MemberModel.Member Member { get; set; }
            public InventoryItemModel.InventoryItem InventoryItem { get; set; }
            //public InventoryItemModel.FlashDeal FlashDeal { get; set; }

            public TransactionStatusDrop ProcessStatus { get; set; }
            public DateTime? ProcessDate { get; set; }
        }
        public class TransactionDetail3
        {
            public int TransactionId { get; set; }
            public string OrderId { get; set; }
            public DateTime TransactionDate { get; set; }
            public int MerchantId { get; set; }
            public string MerchantName { get; set; }
            public int InventoryItemId { get; set; }
            public string InventoryItemName { get; set; }
            public string Variance { get; set; }
            public string SKU { get; set; }
            public int Qty { get; set; }
            public int CustomerId { get; set; }
            public string CustomerFirstName { get; set; }
            public string CustomerLastName { get; set; }
            public string CustomerEmail { get; set; }
            public string CustomerPhone { get; set; }
            public string CustomerAddress1 { get; set; }
            public string CustomerAddress2 { get; set; }
            public string CustomerCity { get; set; }
            public string CustomerState { get; set; }
            public string CustomerPostCode { get; set; }
            public string CustomerCountry { get; set; }
            public string OrderCompletion { get; set; }
            public string ProcessStatus { get; set; }
            public DateTime? ProcessDate { get; set; }
            public string PaymentStatusId { get; set; }
            public string PaymentStatus { get; set; }
            public DateTime? PaymentDate { get; set; }
            public string PaymentMethod { get; set; }
            public string Gift { get; set; }

            public int DropdownType { get; set; }
            public string TransactionSelected { get; set; }
        }
        public class TransactionDetail3Extract
        {
            public int TransactionId { get; set; }
            public string OrderId { get; set; }
            public DateTime TransactionDate { get; set; }
            public int MerchantId { get; set; }
            public string MerchantName { get; set; }
            public int InventoryItemId { get; set; }
            public string InventoryItemName { get; set; }
            public string Variance { get; set; }
            public string SKU { get; set; }
            public int Qty { get; set; }
            public string VoucherCode { get; set; }
            public int CustomerId { get; set; }
            public string CustomerFirstName { get; set; }
            public string CustomerLastName { get; set; }
            public string CustomerEmail { get; set; }
            public string CustomerPhone { get; set; }
            public string CustomerAddress1 { get; set; }
            public string CustomerAddress2 { get; set; }
            public string CustomerCity { get; set; }
            public string CustomerState { get; set; }
            public string CustomerPostCode { get; set; }
            public string CustomerCountry { get; set; }
            public string OrderCompletion { get; set; }
            public string ProcessStatus { get; set; }
            public DateTime? ProcessDate { get; set; }
            public string PaymentStatusId { get; set; }
            public string PaymentStatus { get; set; }
            public DateTime? PaymentDate { get; set; }
            public decimal? PaymentAmount { get; set; }
            public string PaymentMethod { get; set; }
            public string Gift { get; set; }
            public string GiftNote { get; set; }
        }
        public class TransactionUserPlay
        {
            public int TransactionId { get; set; }
            public int InventoryItemId { get; set; }
            public string InventoryItemName { get; set; }
            public int MemberId { get; set; }
            public string MemberEmail { get; set; }
            public DateTime TransactionDate { get; set; }
            public bool MemberPlay { get; set; }
            public int No { get; set; }
            public string PaymentStatus { get; set; }
        }
        public class MinimumTarget
        {
            public int MerchantId { get; set; }
            public string MerchantName { get; set; }
            public int InventoryItemId { get; set; }
            public string InventoryItemName { get; set; }
            public decimal Minimum { get; set; }
            public decimal CurrentSales { get; set; }
        }


        public class TransactionDetailExportData
        {
            public int TransactionId { get; set; }
            public string MerchantName { get; set; }
            public string InventoryItemName { get; set; }
            public string MemberEmail { get; set; }
            public DateTime TransactionDate { get; set; }
            public int? FlashDealId { get; set; }
            public decimal? FlashDealDiscount { get; set; }
            public DateTime? FlashDealStartTime { get; set; }
            public DateTime? FlashDealEndTime { get; set; }
            public string FlashDealStatus { get; set; }
        }
        public class TransactionDetailExportData2
        {
            public int TransactionId { get; set; }
            public string OrderNo { get; set; }
            public DateTime TransactionDate { get; set; }
            public string MerchantName { get; set; }
            public string InventoryItemName { get; set; }
            public string KuazooSKU { get; set; }
            public int Qty { get; set; }
            public string MemberFirstName { get; set; }
            public string MemberLastName { get; set; }
            public string MemberEmail { get; set; }
            public string Phone { get; set; }
            public string AddressLine1 { get; set; }
            public string AddressLine2 { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string Country { get; set; }
            public string ZipCode { get; set; }
        }

        public class ExtractData
        {
            public int ReportType { get; set; }
            public ExtractDataMember Member { get; set; }
            public ExtractDataInventoryItem InventoryItem { get; set; }
            public ExtractDataTransaction Transaction {get;set;}
        }
        public class ExtractDataMember
        {
            public int Status { get; set; }
            public int AgeStart { get; set; }
            public int AgeEnd { get; set; }
        }
        public class ExtractDataInventoryItem
        {
            public string InventoryItemName { get; set; }
            public string PrizeName { get; set; }
            public int Prize { get; set; }
            public int Status { get; set; }
            public int InventoryItemTypeId { get; set; }
            public int CountryId { get; set; }
            public int CityId { get; set; }
            public DateTime PublishedStart { get; set; }
            public DateTime PublishedEnd { get; set; }
        }
        public class ExtractDataTransaction
        {
            public int MerchantId { get; set; }
            public int InventoryItem { get; set; }
            public DateTime TransactionStart { get; set; }
            public DateTime TransactionEnd { get; set; }
        }

        public class TempTransaction
        {
            [Required(ErrorMessage = "*")]
            public int InventoryItemId { get; set; }
            [Required(ErrorMessage = "*")]
            public int MemberId { get; set; }
        }
        public class PromotionCodeVM
        {
            public int PromotionId { get; set; }
            public String Code { get; set; }
            public int Type { get; set; }
            public String TypeName { get; set; }
            public decimal Value { get; set; }
        }


        public class TransactionsHeaderVM
        {
            public TransactionsHeader header { get; set; }
            public List<InventoryItemModel.InventoryItemShow> similar { get; set; }
            public Boolean Game { get; set; }
            public int GameAttempt { get; set; }
            public int GameType { get; set; }
        }
        public class TransactionStatusDrop
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }


        public class FreeDealTransactionReport
        {
            public int TransactionId { get; set; }
            public string OrderId { get; set; }
            public DateTime TransactionDate { get; set; }
            public int MerchantId { get; set; }
            public string MerchantName { get; set; }
            public int InventoryItemId { get; set; }
            public string InventoryItemName { get; set; }
            public string Variance { get; set; }
            public string SKU { get; set; }
            public int Qty { get; set; }
            public int CustomerId { get; set; }
            public string CustomerFirstName { get; set; }
            public string CustomerLastName { get; set; }
            public string CustomerEmail { get; set; }
            public string ProcessStatus { get; set; }
            public DateTime? ProcessDate { get; set; }

            public int DropdownType { get; set; }
            public string TransactionSelected { get; set; }
        }
        public class FreeDealTransactionReportExtract
        {
            public int TransactionId { get; set; }
            public string OrderId { get; set; }
            public DateTime TransactionDate { get; set; }
            public int MerchantId { get; set; }
            public string MerchantName { get; set; }
            public int InventoryItemId { get; set; }
            public string InventoryItemName { get; set; }
            public string Variance { get; set; }
            public string SKU { get; set; }
            public int Qty { get; set; }
            public string VoucherCode { get; set; }
            public int CustomerId { get; set; }
            public string CustomerFirstName { get; set; }
            public string CustomerLastName { get; set; }
            public string CustomerEmail { get; set; }
            public string OrderCompletion { get; set; }
            public string ProcessStatus { get; set; }
            public DateTime? ProcessDate { get; set; }
        }
    }
}