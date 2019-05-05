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
    public interface IPointService
    {
        
    }

    [DataContract]
    public class PointAction
    {
        [DataMember]
        public int PointActionId { get; set; }
        [DataMember]
        public int Code { get; set; }
        [DataMember]
        public string Desc { get; set; }
        [DataMember]
        public decimal Amount { get; set; }
    }

    [DataContract]
    public class PointHeader
    {
        [DataMember]
        public int PointHeaderId { get; set; }
        [DataMember]
        public int UserId { get; set; }
        [DataMember]
        public decimal Balance { get; set; }
        [DataMember]
        public List<PointDetail> ListDetail { get; set; }
    }
    [DataContract]
    public class PointDetail
    {
        [DataMember]
        public int PointDetailId { get; set; }
        [DataMember]
        public int UserId { get; set; }
        [DataMember]
        public decimal Amount { get; set; }
        [DataMember]
        public KPointAction Type { get; set; }
        [DataMember]
        public int FromUser { get; set; }
        [DataMember]
        public string FromUserName { get; set; }
        [DataMember]
        public int ToUser { get; set; }
        [DataMember]
        public string ToUserName { get; set; }
        [DataMember]
        public string Remarks { get; set; }
        [DataMember]
        public DateTime Create { get; set; }
        [DataMember]
        public int InventoryItemId { get; set; }
        [DataMember]
        public string InventoryItemName { get; set; }
        [DataMember]
        public int TransactionId { get; set; }
    }
}
