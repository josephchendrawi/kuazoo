using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace com.kuazoo
{
    public static class WebSetting
    {
        public static string FaceBookId;
        public static string FaceBookSecret;
        public static string FacebookAction;
        public static string Url;
        public static string TwitterKey;
        public static string TwitterSecret;
        public static string TwitterToken;
        public static string TwitterTokenSecret;
    }

    [ServiceContract]
    public interface  IGeneralService
    {
    }

    [DataContract]
    public enum TransactionStatus
    {
        [EnumMember]
        Completed = 1,
        [EnumMember]
        Ship = 2,
        [EnumMember]
        Pending= 3,
        [EnumMember]
        WaitingPayment = 4,
    }
    [DataContract]
    public enum ProcessStatus
    {
        [EnumMember]
        Yes = 1,
        [EnumMember]
        No = 2
    }

    [DataContract]
    public enum PaymentMethod
    {
        [EnumMember]
        MOLPay = 1,
        [EnumMember]
        Ipay88 = 2,
        [EnumMember]
        Visa = 3,
        [EnumMember]
        Master = 4,
        [EnumMember]
        Amex = 5
    }

    [DataContract]
    public enum KPointAction
    {
        [EnumMember]
        InviteFriend = 1,
        [EnumMember]
        PostBeforePurchase = 2,
        [EnumMember]
        PostAfterPurchase = 3,
        [EnumMember]
        PurchaseItem = 4,
        [EnumMember]
        Transfer = 5,
        [EnumMember]
        Redeemed = 6,
        [EnumMember]
        Revert = 7,
    }
    [DataContract]
    public class Static
    {
        [DataMember]
        public int StaticId { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }
    }
}
