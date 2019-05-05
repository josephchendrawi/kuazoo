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
    public interface ILandingServices
    {
        [OperationContract]
        Response<List<Banner>> GetBannerList();
        [OperationContract]
        Response<bool> CreateBanner(Banner Banner);
        [OperationContract]
        Response<Banner> GetBannerById(int BannerId);
    }

    [DataContract]
    public class Banner
    {
        [DataMember]
        public int BannerId { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Link { get; set; }

        public int SubImageId { get; set; }
        public string SubImageName { get; set; }
        public string SubImageUrl { get; set; }
        public string SubImageUrlLink { get; set; }

        public string LastAction { get; set; }
        public int Seq { get; set; }
    }

    [DataContract]
    public class BVD
    {
        [DataMember]
        public int BVDId { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Link { get; set; }
        [DataMember]
        public int Type { get; set; }

        public int SubImageId { get; set; }
        public string SubImageName { get; set; }
        public string SubImageUrl { get; set; }
        public string SubImageUrlLink { get; set; }

        public string LastAction { get; set; }
        public int? Seq { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}
