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
    public interface  ICountryService
    {
        [OperationContract]
        Response<bool> CreateCountry(Country Country);
        [OperationContract]
        Response<bool> DeleteCountry(int CountryId);
        [OperationContract]
        Response<bool> CreateState(State State);
        [OperationContract]
        Response<bool> DeleteState(int StateId);
        [OperationContract]
        Response<bool> CreateCity(City City);
        [OperationContract]
        Response<bool> DeleteCity(int CityId);

        [OperationContract]
        Response<List<Country>> GetCountryList();
        [OperationContract]
        Response<List<City>> GetCityList();
        [OperationContract]
        Response<List<State>> GetStateList();
        [OperationContract]
        Response<List<State>> GetStateListByName(String CountryName);
    }
    [DataContract]
    public class Country
    {
        [DataMember]
        public int CountryId { get; set; }
        [DataMember]
        public string Name { get; set; }
        public string LastAction { get; set; }
    }
    [DataContract]
    public class State
    {
        [DataMember]
        public int StateId { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int CountryId { get; set; }
        [DataMember]
        public string CountryName { get; set; }
        public string LastAction { get; set; }
    }
    [DataContract]
    public class City
    {
        [DataMember]
        public int CityId { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int CountryId { get; set; }
        [DataMember]
        public string CountryName { get; set; }
        public string LastAction { get; set; }
    }
    [DataContract]
    public class Currency
    {
        [DataMember]
        public int CurrencyId { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Code { get; set; }
    }
}
