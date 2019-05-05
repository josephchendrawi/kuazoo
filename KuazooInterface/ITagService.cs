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
    public interface ITagService
    {
        [OperationContract]
        Response<bool> CreateTag(Tag Tag);
        [OperationContract]
        Response<List<Tag>> GetTagList();
        [OperationContract]
        Response<bool> DeleteTag(int TagId);
    }
    [DataContract]
    public class Tag
    {
        [DataMember]
        public int TagId { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public bool ShowAsCategory { get; set; }
        [DataMember]
        public Tag Parent { get; set; }
        [DataMember]
        public DateTime Create { get; set; }
        [DataMember]
        public DateTime Update { get; set; }
        public string LastAction { get; set; }
    }
}
