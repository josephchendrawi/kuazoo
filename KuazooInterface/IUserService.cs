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
    public interface IUserService
    {
        [OperationContract]
        Response<bool> CreateUser(User User);
        [OperationContract]
        Response<bool> CheckLogin(User user);
    }
    [DataContract]
    public class User
    {
        [DataMember]
        public int UserId { get; set; }
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
        public int UserStatus { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public string FacebookId { get; set; }
        [DataMember]
        public bool Verify { get; set; }
        [DataMember]
        public decimal KPoint { get; set; }
        [DataMember]
        public bool Notif { get; set; }
        [DataMember]
        public Boolean Facebook { get; set; }
        [DataMember]
        public Boolean Twitter { get; set; }

        public int ImageId { get; set; }
        public string ImageName { get; set; }
        public string ImageUrl { get; set; }
    }
    [DataContract]
    public class FbUser
    {
        [DataMember]
        public string AccessToken { get; set; }
        [DataMember]
        public string FacebookId { get; set; }
        [DataMember]
        public FbAccType AccountType { get; set; }
        [DataMember]
        public string FacebookName { get; set; }
        [DataMember]
        public string FacebookEmail { get; set; }
        [DataMember]
        public string FacebookGender { get; set; }
        [DataMember]
        public string FaceBookDob { get; set; }
    }

    [DataContract]
    public enum FbAccType
    {
        [EnumMember]
        Normal = 0,
        [EnumMember]
        ForceLink = 1,
        [EnumMember]
        ForceNew = 2,
        [EnumMember]
        AutoLinked = 3,
    }
}
