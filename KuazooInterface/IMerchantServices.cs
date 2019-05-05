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
    public interface IMerchantServices
    {
        [OperationContract]
        Response<bool> CreateMerchant(Merchant Merchant);
        [OperationContract]
        Response<List<Merchant>> GetMerchantList();
        [OperationContract]
        Response<bool> DeleteMerchant(int MerchantId);
    }
    [DataContract]
    public class Merchant
    {
        [DataMember]
        public int MerchantId { get; set; }
        [DataMember]
        public string Name {get;set;}
        [DataMember]
        public string AddressLine1 { get; set; }
        [DataMember]
        public string AddressLine2 { get; set; }
        [DataMember]
        public Country Country { get; set; }
        [DataMember]
        public City City { get; set; }
        [DataMember]
        public string PostCode { get; set; }
        [DataMember]
        public string ContactNumber { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string Website { get; set; }
        [DataMember]
        public string Facebook { get; set; }
        [DataMember]
        public double Latitude { get; set; }
        [DataMember]
        public double Longitude { get; set; }
        [DataMember]
        public Status Status { get; set; }

        [DataMember]
        public string Description { get; set; }

        public List<int> SubImageId { get; set; }
        public List<string> SubImageName { get; set; }
        public List<string> SubImageUrl { get; set; }
        public List<string> SubImageUrlLink { get; set; }
        [DataMember]
        public DateTime Create { get; set; }
        [DataMember]
        public DateTime Update { get; set; }
        public string LastAction { get; set; }
    }
    [DataContract]
    public class Status
    {
        [DataMember]
        public int StatusId { get; set; }
        [DataMember]
        public string StatusName { get; set; }
    }
    [DataContract]
    public enum MerchantStatus
    {
        [EnumMember]
        Premium = 1,
        [EnumMember]
        Pro = 2,
        [EnumMember]
        Inactive = 3
    }
}
