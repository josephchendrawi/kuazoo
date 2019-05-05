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
    public interface IAdminService
    {
    }
    [DataContract]
    public class Admin
    {
        [DataMember]
        public int AdminId { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public DateTime Create { get; set; }
        [DataMember]
        public DateTime Update { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public string PasswordSalt { get; set; }
        public string LastAction { get; set; }
    }
}
