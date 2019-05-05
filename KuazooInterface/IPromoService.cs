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
    public interface IPromoService
    {
        [OperationContract]
        Response<bool> CreatePromotion(Promotion Promo);
        [OperationContract]
        Response<List<Promotion>> GetPromotionList();
        [OperationContract]
        Response<bool> DeletePromotion(int PromotionId);
    }
    [DataContract]
    public class Promotion
    {
        [DataMember]
        public int PromotionId { get; set; }
        [DataMember]
        public string PromoCode { get; set; }
        [DataMember]
        public int PromoType{ get; set; }
        [DataMember]
        public decimal PromoValue { get; set; }
        [DataMember]
        public bool Flag { get; set; }
        [DataMember]
        public DateTime ValidDate { get; set; }
        public string LastAction { get; set; }
        public DateTime Create { get; set; }
    }
}
