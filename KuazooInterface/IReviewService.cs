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
    public interface IReviewService
    {
        [OperationContract]
        Response<Review> CreateReview(Review Review);
        [OperationContract]
        Response<List<Review>> GetReviewByInventoryItemId(int InventoryItemId);
        [OperationContract]
        Response<bool> DeleteReview(long ReviewId);
    }
    [DataContract]
    public class Review
    {
        [DataMember]
        public long ReviewId { get; set; }
        [DataMember]
        public int InventoryItemId { get; set; }
        [DataMember]
        public string InventoryItemName { get; set; }
        public string InventoryItemImageUrl { get; set; }
        public string PrizeImageUrl { get; set; }
        [DataMember]
        public int MemberId { get; set; }
        [DataMember]
        public string MemberEmail { get; set; }
        [DataMember]
        public string MemberFullName { get; set; }
        [DataMember]
        public string MemberImage { get; set; }
        [DataMember]
        public int Rating { get; set; }
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public DateTime ReviewDate { get; set; }
        public String ReviewDateStr { get; set; }
        public string LastAction { get; set; }
    }
    [DataContract]
    public class ReviewData
    {
        [DataMember]
        public int Rating { get; set; }
        [DataMember]
        public int ReviewCount { get; set; }
    }

}
