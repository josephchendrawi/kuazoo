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
    public interface IPrizeService
    {
        [OperationContract]
        Response<bool> CreatePrize(Prize Prize);
        [OperationContract]
        Response<List<Prize>> GetPrizeList();
        [OperationContract]
        Response<bool> DeletePrize(int PrizeId);
    }

    [DataContract]
    public class Prize
    {
        [DataMember]
        public int PrizeId { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public Decimal Price { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string SponsorName { get; set; }
        [DataMember]
        public string Terms { get; set; }
        [DataMember]
        public string Detail { get; set; }
        [DataMember]
        public DateTime ExpiryDate { get; set; }
        [DataMember]
        public DateTime PublishDate { get; set; }
        [DataMember]
        public string GroupName { get; set; }
        [DataMember]
        public Decimal TotalRevenue { get; set; }

        [DataMember]
        public decimal FakeVisualMeter { get; set; }

        public int ImageId { get; set; }
        public string ImageName { get; set; }
        public string ImageUrl { get; set; }

        public List<int> SubImageId { get; set; }
        public List<string> SubImageName { get; set; }
        public List<string> SubImageUrl { get; set; }
        public string LastAction { get; set; }

        public string WinnerEmail { get; set; }
        public string ClosestWinnerEmail { get; set; }
        public DateTime Create { get; set; }
        public int GameType { get; set; }
        public bool FreeDeal { get; set; }
    }
    [DataContract]
    public class WinnerModel
    {
        [DataMember]
        public List<WinnerVM> Item { get; set; }
        [DataMember]
        public int Total { get; set; }
    }
    [DataContract]
    public class WinnerVM
    {
        [DataMember]
        public string Sort { get; set; }
        [DataMember]
        public List<Winner> WinnerList { get; set; }
    }
    [DataContract]
    public class Winner
    {
        [DataMember]
        public int WinnerId { get; set; }
        [DataMember]
        public string PrizeName { get; set; }
        [DataMember]
        public string PrizeImageUrl { get; set; }
        [DataMember]
        public DateTime WinnerDate { get; set; }

        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public string MemberImageUrl { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string City { get; set; }
        [DataMember]
        public string State { get; set; }
        [DataMember]
        public string Country { get; set; }
    }
}
