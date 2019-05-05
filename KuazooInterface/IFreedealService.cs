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
    public interface IFreeDealService
    {
    }

    [DataContract]
    public class FreeDealTransaction
    {
        [DataMember]
        public int FreeDealId { get; set; }
        [DataMember]
        public int MemberId { get; set; }
        [DataMember]
        public int InventoryItemId { get; set; }
        [DataMember]
        public int? FlashDealId { get; set; }
        [DataMember]
        public string Variance { get; set; }
        [DataMember]
        public int Qty { get; set; }
        [DataMember]
        public decimal Discount { get; set; }
        [DataMember]
        public decimal Price { get; set; }
        [DataMember]
        public bool ProcessStatus { get; set; }
    }
    [DataContract]
    public class FreeDealTransactionReport
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
        public string ProcessStatus { get; set; }
        [DataMember]
        public DateTime? ProcessDate { get; set; }
    }
}
