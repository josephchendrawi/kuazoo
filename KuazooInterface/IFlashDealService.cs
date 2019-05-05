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
    public interface IFlashDealService
    {
        [OperationContract]
        Response<bool> CreateFlashDeal(FlashDeal FlashDeal);
        [OperationContract]
        Response<List<FlashDeal>> GetFlashDealList();
        [OperationContract]
        Response<bool> DeleteFlashDeal(int FlashDealId);
    }

    [DataContract]
    public class FlashDeal
    {
        [DataMember]
        public int FlashDealId { get; set; }
        [DataMember]
        public int MerchantId { get; set; }
        [DataMember]
        public string MerchantName { get; set; }
        [DataMember]
        public int InventoryItemId { get; set; }
        [DataMember]
        public string InventoryItemName { get; set; }
        [DataMember]
        public decimal Discount { get; set; }
        [DataMember]
        public DateTime StartTime { get; set; }
        [DataMember]
        public DateTime EndTime { get; set; }
        [DataMember]
        public bool Flag { get; set; }
        public string LastAction { get; set; }
    }
}
