using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.kuazoo
{
    public class CountryService : ICountryService
    {

        public Response<bool> CreateCountry(Country Country)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                if (Country.CountryId != 0)
                {
                    var entityCountry = from d in context.kzCountries
                                    where d.id == Country.CountryId
                                    select d;
                    if (entityCountry.Count() > 0)
                    {
                        var entityCountry2 = from d in context.kzCountries
                                         where d.name.ToLower() == Country.Name.ToLower()
                                         && d.id != Country.CountryId
                                         select d;
                        if (entityCountry2.Count() > 0)
                        {
                            throw new CustomException(CustomErrorType.CountryAlreadyAssign);
                        }
                        else
                        {
                            entityCountry.First().name = Country.Name;
                            entityCountry.First().last_action = "3";
                            context.SaveChanges();
                            response = Response<bool>.Create(true);
                        }
                    }
                    else
                    {
                        throw new CustomException(CustomErrorType.CountryNotFound);
                    }
                }
                else
                {
                    var entityCountry = from d in context.kzCountries
                                    where d.name.ToLower() == Country.Name.ToLower()
                                    select d;
                    if (entityCountry.Count() > 0)
                    {
                        throw new CustomException(CustomErrorType.CountryAlreadyAssign);
                    }
                    else
                    {
                        entity.kzCountry mmentity = new entity.kzCountry();
                        mmentity.name = Country.Name;
                        mmentity.last_action = "1";
                        context.AddTokzCountries(mmentity);
                        context.SaveChanges();
                        response = Response<bool>.Create(true);
                    }
                }
            }
            return response;
        }
        public Response<bool> DeleteCountry(int CountryId)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                var entityUser = from d in context.kzCountries
                                 where d.id == CountryId
                                 select d;
                if (entityUser.Count() > 0)
                {
                    entityUser.First().last_action = "5";
                    context.SaveChanges();
                    response = Response<bool>.Create(true);
                }
                else
                {
                    response = Response<bool>.Create(false);
                }
            }

            return response;
        }
        public Response<bool> CreateState(State State)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                if (State.StateId != 0)
                {
                    var entityState = from d in context.kzStates
                                        where d.id == State.StateId
                                        select d;
                    if (entityState.Count() > 0)
                    {
                        var entityState2 = from d in context.kzStates
                                             where d.name.ToLower() == State.Name.ToLower()
                                             && d.id != State.StateId
                                             select d;
                        if (entityState2.Count() > 0)
                        {
                            throw new CustomException(CustomErrorType.StateAlreadyAssign);
                        }
                        else
                        {
                            entityState.First().name = State.Name;
                            entityState.First().country_id = State.CountryId;
                            entityState.First().last_action = "3";
                            context.SaveChanges();
                            response = Response<bool>.Create(true);
                        }
                    }
                    else
                    {
                        throw new CustomException(CustomErrorType.StateNotFound);
                    }
                }
                else
                {
                    var entityState = from d in context.kzStates
                                        where d.name.ToLower() == State.Name.ToLower()
                                        select d;
                    if (entityState.Count() > 0)
                    {
                        throw new CustomException(CustomErrorType.StateAlreadyAssign);
                    }
                    else
                    {
                        entity.kzState mmentity = new entity.kzState();
                        mmentity.name = State.Name;
                        mmentity.country_id = State.CountryId;
                        mmentity.last_action = "1";
                        context.AddTokzStates(mmentity);
                        context.SaveChanges();
                        response = Response<bool>.Create(true);
                    }
                }
            }
            return response;
        }
        public Response<bool> DeleteState(int StateId)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                var entityUser = from d in context.kzStates
                                 where d.id == StateId
                                 select d;
                if (entityUser.Count() > 0)
                {
                    entityUser.First().last_action = "5";
                    context.SaveChanges();
                    response = Response<bool>.Create(true);
                }
                else
                {
                    response = Response<bool>.Create(false);
                }
            }

            return response;
        }
        public Response<bool> CreateCity(City City)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                if (City.CityId != 0)
                {
                    var entityCity = from d in context.kzCities
                                      where d.id == City.CityId
                                      select d;
                    if (entityCity.Count() > 0)
                    {
                        var entityCity2 = from d in context.kzCities
                                           where d.name.ToLower() == City.Name.ToLower()
                                           && d.id != City.CityId
                                           select d;
                        if (entityCity2.Count() > 0)
                        {
                            throw new CustomException(CustomErrorType.CityAlreadyAssign);
                        }
                        else
                        {
                            entityCity.First().name = City.Name;
                            entityCity.First().country_id = City.CountryId;
                            entityCity.First().last_action = "3";
                            context.SaveChanges();
                            response = Response<bool>.Create(true);
                        }
                    }
                    else
                    {
                        throw new CustomException(CustomErrorType.CityNotFound);
                    }
                }
                else
                {
                    var entityCity = from d in context.kzCities
                                      where d.name.ToLower() == City.Name.ToLower()
                                      select d;
                    if (entityCity.Count() > 0)
                    {
                        throw new CustomException(CustomErrorType.CityAlreadyAssign);
                    }
                    else
                    {
                        entity.kzCity mmentity = new entity.kzCity();
                        mmentity.name = City.Name;
                        mmentity.country_id = City.CountryId;
                        mmentity.last_action = "1";
                        context.AddTokzCities(mmentity);
                        context.SaveChanges();
                        response = Response<bool>.Create(true);
                    }
                }
            }
            return response;
        }
        public Response<bool> DeleteCity(int CityId)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                var entityUser = from d in context.kzCities
                                 where d.id == CityId
                                 select d;
                if (entityUser.Count() > 0)
                {
                    entityUser.First().last_action = "5";
                    context.SaveChanges();
                    response = Response<bool>.Create(true);
                }
                else
                {
                    response = Response<bool>.Create(false);
                }
            }

            return response;
        }

        public Response<List<Country>> GetCountryList()
        {
            Response<List<Country>> response = null;
            List<Country> CountryList = new List<Country>();
            using (var context = new entity.KuazooEntities())
            {
                var entityCountry = from d in context.kzCountries
                                    where d.last_action != "5"
                                    select d;
                foreach (var v in entityCountry)
                {
                    Country Country = new Country();
                    Country.CountryId = v.id;
                    Country.Name = v.name;
                    Country.LastAction = v.last_action;
                    CountryList.Add(Country);
                }
                response = Response<List<Country>>.Create(CountryList);
            }

            return response;
        }
        public Response<Country> GetCountryById(int CountryId)
        {
            Response<Country> response = null;
            Country Country = new Country();
            using (var context = new entity.KuazooEntities())
            {
                var entityCountry = from d in context.kzCountries
                                where d.id == CountryId
                                select d;
                var v = entityCountry.First();
                if (v != null)
                {
                    Country.CountryId = v.id;
                    Country.Name = v.name;
                    Country.LastAction = v.last_action;
                }
                response = Response<Country>.Create(Country);
            }

            return response;
        }
        public Response<List<Country>> GetCountryListActive()
        {
            Response<List<Country>> response = null;
            List<Country> CountryList = new List<Country>();
            using (var context = new entity.KuazooEntities())
            {
                var entityCountry = from d in context.kzCountries
                                    where d.last_action != "5"
                                    select d;
                foreach (var v in entityCountry)
                {
                    Country Country = new Country();
                    Country.CountryId = v.id;
                    Country.Name = v.name;
                    CountryList.Add(Country);
                }
                response = Response<List<Country>>.Create(CountryList);
            }

            return response;
        }
        public Response<List<State>> GetStateList()
        {
            Response<List<State>> response = null;
            List<State> StateList = new List<State>();
            using (var context = new entity.KuazooEntities())
            {
                var entityState = from d in context.kzStates
                                  where d.last_action != "5"
                                  select d;
                foreach (var v in entityState)
                {
                    State State = new State();
                    State.StateId = v.id;
                    State.Name = v.name;
                    State.CountryId = (int)v.country_id;
                    State.CountryName = v.kzCountry.name;
                    State.LastAction = v.last_action;
                    StateList.Add(State);
                }
                response = Response<List<State>>.Create(StateList);
            }

            return response;
        }
        public Response<State> GetStateById(int StateId)
        {
            Response<State> response = null;
            State State = new State();
            using (var context = new entity.KuazooEntities())
            {
                var entityState = from d in context.kzStates
                                    where d.id == StateId
                                    select d;
                var v = entityState.First();
                if (v != null)
                {
                    State.StateId = v.id;
                    State.Name = v.name;
                    State.CountryId = (int)v.country_id;
                    State.CountryName = v.kzCountry.name;
                    State.LastAction = v.last_action;
                }
                response = Response<State>.Create(State);
            }

            return response;
        }
        public Response<List<State>> GetStateListActive(int CountryId)
        {
            Response<List<State>> response = null;
            List<State> StateList = new List<State>();
            using (var context = new entity.KuazooEntities())
            {
                var entityState = from d in context.kzStates
                                    where d.country_id == CountryId
                                    && d.last_action != "5"
                                    select d;
                foreach (var v in entityState)
                {
                    State State = new State();
                    State.StateId = v.id;
                    State.Name = v.name;
                    StateList.Add(State);
                }
                response = Response<List<State>>.Create(StateList);
            }

            return response;
        }
        public Response<List<State>> GetStateListByName(String CountryName)
        {
            Response<List<State>> response = null;
            List<State> StateList = new List<State>();
            using (var context = new entity.KuazooEntities())
            {
                var entityState = from d in context.kzStates
                                    where d.kzCountry.name == CountryName
                                    && d.last_action != "5"
                                    select d;
                foreach (var v in entityState)
                {
                    State State = new State();
                    State.StateId = v.id;
                    State.Name = v.name;
                    StateList.Add(State);
                }
                response = Response<List<State>>.Create(StateList);
            }

            return response;
        }
        public Response<List<City>> GetCityList()
        {
            Response<List<City>> response = null;
            List<City> CityList = new List<City>();
            using (var context = new entity.KuazooEntities())
            {
                var entityCountry = from d in context.kzCities.Include("kzCurrency")
                                    where d.last_action!="5"
                                    select d;
                foreach (var v in entityCountry)
                {
                    City City = new City();
                    City.CityId = v.id;
                    City.Name = v.name;
                    City.CountryId = (int)v.country_id;
                    City.CountryName = v.kzCountry.name;
                    City.LastAction = v.last_action;
                    //Currency currency = new Currency()
                    //{
                    //    CurrencyId = (int)v.currency_id,
                    //    Name = v.kzCurrency.name,
                    //    Code = v.kzCurrency.code
                    //};
                    //City.Currency = currency;
                    CityList.Add(City);
                }
                response = Response<List<City>>.Create(CityList);
            }

            return response;
        }
        public Response<City> GetCityById(int CityId)
        {
            Response<City> response = null;
            City City = new City();
            using (var context = new entity.KuazooEntities())
            {
                var entityCity = from d in context.kzCities
                                  where d.id == CityId
                                  select d;
                var v = entityCity.First();
                if (v != null)
                {
                    City.CityId = v.id;
                    City.Name = v.name;
                    City.CountryId = (int)v.country_id;
                    City.CountryName = v.kzCountry.name;
                    City.LastAction = v.last_action;
                }
                response = Response<City>.Create(City);
            }

            return response;
        }
        public Response<List<City>> GetCityListActive(int CountryId)
        {
            Response<List<City>> response = null;
            List<City> CityList = new List<City>();
            using (var context = new entity.KuazooEntities())
            {
                var entityCountry = from d in context.kzCities.Include("kzCurrency")
                                    where d.country_id == CountryId
                                    && d.last_action != "5"
                                    select d;
                foreach (var v in entityCountry)
                {
                    City City = new City();
                    City.CityId = v.id;
                    City.Name = v.name;
                    //Currency currency = new Currency()
                    //{
                    //    CurrencyId = (int)v.currency_id,
                    //    Name = v.kzCurrency.name,
                    //    Code = v.kzCurrency.code
                    //};
                    //City.Currency = currency;
                    CityList.Add(City);
                }
                response = Response<List<City>>.Create(CityList);
            }

            return response;
        }
    }
}
