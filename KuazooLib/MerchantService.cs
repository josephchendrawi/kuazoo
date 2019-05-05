using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace com.kuazoo
{
    public class MerchantService : IMerchantServices
    {

        public Response<bool> CreateMerchant(Merchant Merchant)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                if (Merchant.MerchantId != 0)
                {
                    var entityMerchant = from d in context.kzMerchants
                                     where d.id == Merchant.MerchantId
                                     select d;
                    if (entityMerchant.Count() > 0)
                    {
                        var entityMerchant2 = from d in context.kzMerchants
                                          where d.name.ToLower() == Merchant.Name.ToLower()
                                          && d.id != Merchant.MerchantId
                                          select d;
                        if (entityMerchant2.Count() > 0)
                        {
                            throw new CustomException(CustomErrorType.MerchantAlreadyAssign);
                        }
                        else
                        {
                            entityMerchant.First().name = Merchant.Name;
                            entityMerchant.First().address_line1 = Merchant.AddressLine1;
                            entityMerchant.First().address_line2 = Merchant.AddressLine2;
                            entityMerchant.First().city_id = Merchant.City.CityId;
                            entityMerchant.First().postcode = Merchant.PostCode;
                            entityMerchant.First().contact_number = Merchant.ContactNumber;
                            entityMerchant.First().email = Merchant.Email;
                            entityMerchant.First().website = Merchant.Website;
                            entityMerchant.First().facebook = Merchant.Facebook;
                            entityMerchant.First().latitude = Merchant.Latitude;
                            entityMerchant.First().longitude = Merchant.Longitude;
                            entityMerchant.First().status_id = Merchant.Status.StatusId;
                            entityMerchant.First().last_updated = DateTime.UtcNow;
                            entityMerchant.First().last_action = "3";
                            entityMerchant.First().description = Merchant.Description;
                            context.SaveChanges();
                            #region image
                            if (Merchant.SubImageId != null)
                            {
                                try
                                {
                                    var entityImage = from d in context.kzMerchantImages
                                                      where d.merchant_id == Merchant.MerchantId
                                                      select d;
                                    foreach (var v in entityImage)
                                    {
                                        context.DeleteObject(v);
                                    }
                                    context.SaveChanges();
                                    int i = 0;
                                    foreach (var v in Merchant.SubImageId)
                                    {
                                        entity.kzMerchantImage mmimage = new entity.kzMerchantImage();
                                        mmimage.merchant_id = Merchant.MerchantId;
                                        mmimage.image_id = v;
                                        mmimage.image_url = Merchant.SubImageUrlLink[i];
                                        context.AddTokzMerchantImages(mmimage);
                                        context.SaveChanges();
                                        i++;
                                    }
                                }
                                catch
                                {

                                }
                            }
                            else
                            {
                                var entityImage = from d in context.kzMerchantImages
                                                  where d.merchant_id == Merchant.MerchantId
                                                  select d;
                                foreach (var v in entityImage)
                                {
                                    context.DeleteObject(v);
                                }
                                context.SaveChanges();
                            }

                            #endregion
                            response = Response<bool>.Create(true);
                        }
                    }
                    else
                    {
                        throw new CustomException(CustomErrorType.MerchantNotFound);
                    }
                }
                else
                {
                    var entityMerchant = from d in context.kzMerchants
                                     where d.name.ToLower() == Merchant.Name.ToLower()
                                     select d;
                    if (entityMerchant.Count() > 0)
                    {
                        throw new CustomException(CustomErrorType.MerchantAlreadyAssign);
                    }
                    else
                    {
                        entity.kzMerchant mmentity = new entity.kzMerchant();
                        mmentity.name = Merchant.Name;
                        mmentity.address_line1 = Merchant.AddressLine1;
                        mmentity.address_line2 = Merchant.AddressLine2;
                        mmentity.city_id = Merchant.City.CityId;
                        mmentity.postcode = Merchant.PostCode;
                        mmentity.contact_number = Merchant.ContactNumber;
                        mmentity.email = Merchant.Email;
                        mmentity.website = Merchant.Website;
                        mmentity.facebook = Merchant.Facebook;
                        mmentity.latitude = Merchant.Latitude;
                        mmentity.longitude = Merchant.Longitude;
                        mmentity.status_id = Merchant.Status.StatusId;
                        mmentity.last_created = DateTime.UtcNow;
                        mmentity.last_updated = DateTime.UtcNow;
                        mmentity.last_action = "1";
                        mmentity.description = Merchant.Description;
                        context.AddTokzMerchants(mmentity);
                        context.SaveChanges();
                        int MerchantId = mmentity.id;
                        #region image
                        if (Merchant.SubImageId != null)
                        {
                            try
                            {
                                int i = 0;
                                foreach (var v in Merchant.SubImageId)
                                {
                                    entity.kzMerchantImage mmimage = new entity.kzMerchantImage();
                                    mmimage.merchant_id = MerchantId;
                                    mmimage.image_id = v;
                                    mmimage.image_url = Merchant.SubImageUrlLink[i];
                                    context.AddTokzMerchantImages(mmimage);
                                    context.SaveChanges();
                                    i++;
                                }
                            }
                            catch
                            {

                            }
                        }
                        #endregion
                        response = Response<bool>.Create(true);
                    }
                }
            }
            return response;
        }
        public Response<List<Merchant>> GetMerchantList()
        {
            Response<List<Merchant>> response = null;
            List<Merchant> MerchantList = new List<Merchant>();
            using (var context = new entity.KuazooEntities())
            {
                var entityMerchant = from d in context.kzMerchants.Include("kzStatu").Include("kzCity")
                                     orderby d.last_created descending
                                 select d;
                foreach (var v in entityMerchant)
                {
                    Merchant Merchant = new Merchant();
                    Merchant.MerchantId = v.id;
                    Merchant.Name = v.name;
                    Merchant.AddressLine1 = v.address_line1;
                    Merchant.AddressLine2 = v.address_line2;
                    City city = new City()
                    {
                        CityId = (int)v.city_id,
                        Name = v.kzCity.name
                    };
                    Merchant.City = city;

                    Country country = new Country(){
                        CountryId = (int)v.kzCity.country_id,
                        Name = v.kzCity.kzCountry.name
                    };
                    Merchant.Country = country;

                    Merchant.PostCode = v.postcode;
                    Merchant.ContactNumber = v.contact_number;
                    Merchant.Email = v.email;
                    Merchant.Website = v.website;
                    Merchant.Facebook = v.facebook;
                    Merchant.Latitude = (float)v.latitude;
                    Merchant.Longitude = (float)v.longitude;
                    Status stat = new Status()
                    {
                        StatusId = (int)v.status_id,
                        StatusName = v.kzStatu.name
                    };
                    Merchant.Status = stat;
                    Merchant.Create = (DateTime)v.last_created;
                    Merchant.Update = (DateTime)v.last_updated;
                    Merchant.LastAction = v.last_action;

                    Merchant.Description = v.description;

                    List<int> subimgidmerchant = new List<int>();
                    List<string> subimgnamemerchant = new List<string>();
                    List<string> subimgurlmerchant = new List<string>();
                    List<string> subimgurllinkmerchant = new List<string>();
                    var entitySubImgmerchant = from d in context.kzMerchantImages
                                            from e in context.kzImages
                                            where d.image_id == e.id && d.merchant_id == v.id
                                            select new { d.image_id, e.url, d.image_url };
                    foreach (var v2 in entitySubImgmerchant)
                    {
                        subimgidmerchant.Add(v2.image_id);
                        subimgnamemerchant.Add(v2.url);
                        subimgurlmerchant.Add(ConfigurationManager.AppSettings["uploadpath"] + v2.url);
                        subimgurllinkmerchant.Add(v2.image_url);
                    }
                    Merchant.SubImageId = subimgidmerchant;
                    Merchant.SubImageName = subimgnamemerchant;
                    Merchant.SubImageUrl = subimgurlmerchant;
                    Merchant.SubImageUrlLink = subimgurllinkmerchant;
                    MerchantList.Add(Merchant);
                }
                response = Response<List<Merchant>>.Create(MerchantList);
            }

            return response;
        }
        public Response<Merchant> GetMerchantById(int MerchantId)
        {
            Response<Merchant> response = null;
            Merchant Merchant = new Merchant();
            using (var context = new entity.KuazooEntities())
            {
                var entityMerchant = from d in context.kzMerchants.Include("kzStatu").Include("kzCity")
                                     where d.id == MerchantId
                                     select d;
                var v = entityMerchant.First();
                if(v!=null)
                {
                    Merchant.MerchantId = v.id;
                    Merchant.Name = v.name;
                    Merchant.AddressLine1 = v.address_line1;
                    Merchant.AddressLine2 = v.address_line2;
                    City city = new City()
                    {
                        CityId = (int)v.city_id,
                        Name = v.kzCity.name
                    };
                    Merchant.City = city;

                    Country country = new Country()
                    {
                        CountryId = (int)v.kzCity.country_id,
                        Name = v.kzCity.kzCountry.name
                    };
                    Merchant.Country = country;

                    Merchant.PostCode = v.postcode;
                    Merchant.ContactNumber = v.contact_number;
                    Merchant.Email = v.email;
                    Merchant.Website = v.website;
                    Merchant.Facebook = v.facebook;
                    Merchant.Latitude = (float)v.latitude;
                    Merchant.Longitude = (float)v.longitude;
                    Status stat = new Status()
                    {
                        StatusId = (int)v.status_id,
                        StatusName = v.kzStatu.name
                    };
                    Merchant.Status = stat;
                    Merchant.Create = (DateTime)v.last_created;
                    Merchant.Update = (DateTime)v.last_updated;
                    Merchant.LastAction = v.last_action;

                    Merchant.Description = v.description;

                    List<int> subimgidmerchant = new List<int>();
                    List<string> subimgnamemerchant = new List<string>();
                    List<string> subimgurlmerchant = new List<string>();
                    List<string> subimgurllinkmerchant = new List<string>();
                    var entitySubImgmerchant = from d in context.kzMerchantImages
                                               from e in context.kzImages
                                               where d.image_id == e.id && d.merchant_id == v.id
                                               select new { d.image_id, e.url, d.image_url };
                    foreach (var v2 in entitySubImgmerchant)
                    {
                        subimgidmerchant.Add(v2.image_id);
                        subimgnamemerchant.Add(v2.url);
                        subimgurlmerchant.Add(ConfigurationManager.AppSettings["uploadpath"] + v2.url);
                        subimgurllinkmerchant.Add(v2.image_url);
                    }
                    Merchant.SubImageId = subimgidmerchant;
                    Merchant.SubImageName = subimgnamemerchant;
                    Merchant.SubImageUrl = subimgurlmerchant;
                    Merchant.SubImageUrlLink = subimgurllinkmerchant;
                }
                response = Response<Merchant>.Create(Merchant);
            }

            return response;
        }
        public Response<bool> DeleteMerchant(int MerchantId)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                var entityUser = from d in context.kzMerchants
                                 where d.id == MerchantId
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
        public Response<bool> DuplicateMerchant(int MerchantId)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                Merchant item = GetMerchantById(MerchantId).Result;
                item.MerchantId = 0;
                item.Name = item.Name + " - Copy";
                var result = CreateMerchant(item).Result;
                response = Response<bool>.Create(result);
            }

            return response;
        }
    }
}
