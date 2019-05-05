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
    public interface IImageService
    {
        [OperationContract]
        Response<bool> CreateImage(Image Image);
        [OperationContract]
        Response<List<Image>> GetImageList();
        [OperationContract]
        Response<bool> DeleteImage(int ImageId);
    }
    [DataContract]
    public class Image
    {
        [DataMember]
        public int ImageId { get; set; }
        [DataMember]
        public string Url { get; set; }
        [DataMember]
        public string Name { get; set; }
        public string LastAction { get; set; }
    }
}
