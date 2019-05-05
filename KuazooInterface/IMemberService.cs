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
    public interface IMemberService
    {
        [OperationContract]
        Response<bool> CreateMember(Member Member);
        [OperationContract]
        Response<List<Member>> GetMemberList();
        [OperationContract]
        Response<bool> ChangeMemberStatus(int MemberId, int MemberStatus);
    }

    [DataContract]
    public class MemberStatus
    {
        [DataMember]
        public int MemberStatusId { get; set; }
        [DataMember]
        public string MemberStatusName { get; set; }
    }
    [DataContract]
    public class Facebook
    {
        [DataMember]
        public int FacebookId { get; set; }
    }
    [DataContract]
    public class Member
    {
        [DataMember]
        public int MemberId { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public int Gender { get; set; }
        [DataMember]
        public DateTime DateOfBirth { get; set; }
        [DataMember]
        public bool LastLockout { get; set; }
        [DataMember]
        public DateTime? LastLockoutDate { get; set; }
        [DataMember]
        public MemberStatus MemberStatus { get; set; }
        [DataMember]
        public String Password { get; set; }
        [DataMember]
        public Facebook Facebook { get; set; }
        public DateTime Create { get; set; }
    }
    [DataContract]
    public class Subscribe
    {
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public DateTime SubscribeDate { get; set; }
    }
}
