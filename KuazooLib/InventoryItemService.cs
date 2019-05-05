using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;
using System.Data.Linq.SqlClient;
namespace com.kuazoo
{
    public class InventoryItemService : IInventoryItemService
    {

        public Response<bool> CreateInventoryItem(InventoryItem item)
        {
            Response<bool> response = null;
            bool sendemail = false;
            string merchant="";
            DateTime expireOld=DateTime.MinValue;
            DateTime expireNew = DateTime.MinValue;
            int inventoryid=0;
            string inventory="";
            bool marginchange = false;
            decimal oldmargin = 0;
            using (var context = new entity.KuazooEntities())
            {
                if (item.InventoryItemId != 0)
                {
                    var entityInventoryItem = from d in context.kzInventoryItems
                                              where d.id == item.InventoryItemId
                                     select d;
                    if (entityInventoryItem.Count() > 0)
                    {
                        var entityInventoryItem2 = from d in context.kzInventoryItems
                                                   where d.name.ToLower() == item.Name.ToLower()
                                          && d.id != item.InventoryItemId
                                          select d;
                        if (entityInventoryItem2.Count() > 0)
                        {
                            throw new CustomException(CustomErrorType.InventoryItemAlreadyAssign);
                        }
                        else
                        {
                            entityInventoryItem.First().name = item.Name;
                            entityInventoryItem.First().short_desc = item.ShortDesc;
                            entityInventoryItem.First().general_description = item.GeneralDescription;
                            entityInventoryItem.First().price = item.Price;
                            if (entityInventoryItem.First().margin != item.Margin)
                            {
                                oldmargin = (decimal)entityInventoryItem.First().margin;
                                marginchange = true;
                            }
                            entityInventoryItem.First().margin = item.Margin;
                            entityInventoryItem.First().description = item.Description;
                            entityInventoryItem.First().terms = item.Terms;
                            entityInventoryItem.First().merchant_id = item.Merchant.MerchantId;
                            entityInventoryItem.First().city_id = item.City.CityId;
                            entityInventoryItem.First().keywords = item.Keyword;
                            entityInventoryItem.First().last_updated = DateTime.UtcNow;
                            entityInventoryItem.First().inventoryitem_type_id = item.InventoryItemType.InventoryItemTypeId;
                            entityInventoryItem.First().discount = item.Discount;
                            if (entityInventoryItem.First().expiry_date != item.ExpireDate)
                            {
                                inventoryid = item.InventoryItemId;
                                inventory = entityInventoryItem.First().name;
                                merchant = entityInventoryItem.First().kzMerchant.name;
                                expireOld = (DateTime)entityInventoryItem.First().expiry_date;
                                expireNew = item.ExpireDate;
                                sendemail = true;
                            }
                            entityInventoryItem.First().expiry_date = item.ExpireDate;
                            entityInventoryItem.First().publish_date = item.PublishDate;
                            entityInventoryItem.First().remainsales = item.RemainSales;
                            entityInventoryItem.First().maximumsales = item.MaximumSales;
                            entityInventoryItem.First().minimumtarget = item.MinimumTarget;
                            entityInventoryItem.First().salesvisualmeter = item.SalesVisualMeter;
                            entityInventoryItem.First().flag = item.Flag;
                            //entityInventoryItem.First().featured = item.Featured;
                            entityInventoryItem.First().last_action = "3";

                            entityInventoryItem.First().prize_id = item.PrizeId;
                            entityInventoryItem.First().draft = item.Draft;
                            entityInventoryItem.First().priority = item.Priority;
                            context.SaveChanges();
                            var merchantEntity = from d in context.kzMerchants
                                                 where d.id == item.Merchant.MerchantId
                                                 select d;
                            string merchantname = "";
                            if (merchantEntity.Count() > 0) { merchantname = merchantEntity.First().name; }
                            var itemtypeEntity = from d in context.kzInventoryItemTypes
                                                 where d.id == item.InventoryItemType.InventoryItemTypeId
                                                 select d;
                            string itemtype = itemtypeEntity.First().name;
                            #region variance
                            if (item.Variance != null)
                            {
                                try
                                {
                                    //List<Dictionary<string, decimal>> oldvariance = new List<Dictionary<string, decimal>>();
                                    var entityVariance = from d in context.kzVariances
                                                         where d.inventoryitem_id == item.InventoryItemId
                                                         select d;
                                    foreach (var v in entityVariance)
                                    {
                                        context.DeleteObject(v);
                                    }
                                    context.SaveChanges();
                                    int countv = 0;

                                    foreach (var v in item.Variance)
                                    {
                                        countv = countv + 1;
                                        entity.kzVariance mmvar = new entity.kzVariance();
                                        mmvar.inventoryitem_id = item.InventoryItemId;
                                        mmvar.name = v.Variance;
                                        mmvar.price = v.Price;
                                        mmvar.discount = v.Discount;
                                        mmvar.margin = v.Margin;
                                        mmvar.available_limit = v.AvailabelLimit;
                                        mmvar.sku = merchantname.Substring(0, 2).ToUpper() + "-" + String.Format("{0:D4}", item.InventoryItemId) + "-" + itemtype.Substring(0, 2).ToUpper() + "-" + string.Format("{0:D3}", countv);
                                        context.AddTokzVariances(mmvar);
                                        context.SaveChanges();
                                    }
                                }
                                catch
                                {

                                }
                            }
                            #endregion
                            #region image
                            if (item.ImageId != null && item.ImageId !=0)
                            {
                                try
                                {
                                    var entityImage = from d in context.kzInventoryItemImages
                                                      where d.inventoryitem_id == item.InventoryItemId
                                                      && d.main == true
                                                      select d;
                                    if (entityImage.Count() > 0)
                                    {
                                        if (entityImage.FirstOrDefault().image_id != item.ImageId)
                                        {
                                            if (item.ImageId != 0)
                                            {
                                                context.DeleteObject(entityImage.FirstOrDefault());
                                                context.SaveChanges();
                                                entity.kzInventoryItemImage mmimage = new entity.kzInventoryItemImage();
                                                mmimage.inventoryitem_id = item.InventoryItemId;
                                                mmimage.image_id = item.ImageId;
                                                mmimage.main = true;
                                                context.AddTokzInventoryItemImages(mmimage);
                                                context.SaveChanges();
                                            }
                                            else
                                            {
                                                context.DeleteObject(entityImage.FirstOrDefault());
                                                context.SaveChanges();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        entity.kzInventoryItemImage mmimage = new entity.kzInventoryItemImage();
                                        mmimage.inventoryitem_id = item.InventoryItemId;
                                        mmimage.image_id = item.ImageId;
                                        mmimage.main = true;
                                        context.AddTokzInventoryItemImages(mmimage);
                                        context.SaveChanges();
                                    }
                                }
                                catch
                                {

                                }
                            }
                            if (item.SubImageId != null)
                            {
                                try
                                {
                                    var entityImage = from d in context.kzInventoryItemImages
                                                      where d.inventoryitem_id == item.InventoryItemId
                                                      && d.main == false
                                                      select d;
                                    foreach (var v in entityImage)
                                    {
                                        context.DeleteObject(v);
                                    }
                                    context.SaveChanges();
                                    foreach (var v in item.SubImageId)
                                    {
                                        entity.kzInventoryItemImage mmimage = new entity.kzInventoryItemImage();
                                        mmimage.inventoryitem_id = item.InventoryItemId;
                                        mmimage.image_id = v;
                                        mmimage.main = false;
                                        context.AddTokzInventoryItemImages(mmimage);
                                        context.SaveChanges();
                                    }
                                }
                                catch
                                {

                                }
                            }
                            else
                            {
                                var entityImage = from d in context.kzInventoryItemImages
                                                  where d.inventoryitem_id == item.InventoryItemId
                                                  && d.main == false
                                                  select d;
                                foreach (var v in entityImage)
                                {
                                    context.DeleteObject(v);
                                }
                                context.SaveChanges();
                            }

                            #endregion
                            #region tag

                            if (item.Tag != null)
                            {
                                string currtag = "";
                                var entityTag = from d in context.kzInventoryItemTags
                                                where d.inventoryitem_id == item.InventoryItemId
                                                orderby d.tag_id ascending
                                                select d;
                                foreach (var v in entityTag)
                                {
                                    currtag += v.tag_id + ",";
                                }
                                if (currtag.Length > 1)
                                {
                                    currtag = currtag.Substring(0, currtag.Length - 1);
                                }
                                if (item.Tag != currtag)
                                {
                                    var entityTag2 = from d in context.kzInventoryItemTags
                                                     where d.inventoryitem_id == item.InventoryItemId
                                                     orderby d.tag_id ascending
                                                     select d;
                                    foreach (var v in entityTag2)
                                    {
                                        context.DeleteObject(v);
                                    }

                                    string[] taglist = item.Tag.Split(',');
                                    foreach (var v in taglist)
                                    {
                                        try
                                        {
                                            int tagid = Convert.ToInt16(v);
                                            entity.kzInventoryItemTag mmtag = new entity.kzInventoryItemTag();
                                            mmtag.inventoryitem_id = item.InventoryItemId;
                                            mmtag.tag_id = tagid;
                                            context.AddTokzInventoryItemTags(mmtag);
                                        }
                                        catch
                                        {

                                        }
                                    }
                                }
                            }
                            else
                            {
                                var entityTag2 = from d in context.kzInventoryItemTags
                                                 where d.inventoryitem_id == item.InventoryItemId
                                                 orderby d.tag_id ascending
                                                 select d;
                                foreach (var v in entityTag2)
                                {
                                    context.DeleteObject(v);
                                }
                            }
                            #endregion
                            context.SaveChanges();
                            //#region flashdeal
                            //if (item.FlashDeal != null)
                            //{
                            //    item.FlashDeal.InventoryItemId = item.InventoryItemId;
                            //    try
                            //    {
                            //        new FlashDealService().CreateFlashDeal(item.FlashDeal);
                            //    }
                            //    catch
                            //    {

                            //    }
                            //}
                            //#endregion
                            if (item.Draft == false)
                            {
                                if (sendemail)
                                {
                                    //MemberNotifEmail(merchant, inventoryid, inventory, expireOld, expireNew);
                                }
                                new TransactionService().UpdateSalesVisualMeterEditInventory(item.InventoryItemId, marginchange, oldmargin);
                            }
                            response = Response<bool>.Create(true);
                        }
                    }
                    else
                    {
                        throw new CustomException(CustomErrorType.InventoryItemNotFound);
                    }
                }
                else
                {
                    var entityInventoryItem = from d in context.kzInventoryItems
                                              where d.name.ToLower() == item.Name.ToLower()
                                     select d;
                    if (entityInventoryItem.Count() > 0)
                    {
                        throw new CustomException(CustomErrorType.InventoryItemAlreadyAssign);
                    }
                    else
                    {
                        var curradmin = new AdminService().GetCurrentAdmin().Result;
                        entity.kzInventoryItem mmentity = new entity.kzInventoryItem();
                        mmentity.name = item.Name;
                        mmentity.short_desc = item.ShortDesc;
                        mmentity.price = (decimal)item.Price;
                        mmentity.margin = (decimal)item.Margin;
                        mmentity.general_description = item.GeneralDescription;
                        mmentity.description = item.Description;
                        mmentity.terms = item.Terms;
                        mmentity.created_by = curradmin.AdminId;
                        mmentity.merchant_id = item.Merchant.MerchantId;
                        mmentity.city_id = item.City.CityId;
                        mmentity.keywords = item.Keyword;
                        mmentity.last_created = DateTime.UtcNow;
                        mmentity.last_updated = DateTime.UtcNow;
                        mmentity.inventoryitem_type_id = item.InventoryItemType.InventoryItemTypeId;
                        mmentity.discount = item.Discount;
                        mmentity.expiry_date = item.ExpireDate;
                        mmentity.publish_date = item.PublishDate;
                        mmentity.maximumsales = item.MaximumSales;
                        mmentity.remainsales = item.RemainSales;
                        mmentity.minimumtarget = item.MinimumTarget;
                        mmentity.salesvisualmeter = item.SalesVisualMeter;
                        if (item.PrizeId != 0)
                        {
                            mmentity.prize_id = item.PrizeId;
                        }
                        mmentity.flag = item.Flag;
                        mmentity.featured = false;// item.Featured;
                        mmentity.last_action = "1";
                        mmentity.draft = item.Draft;
                        mmentity.priority = item.Priority;
                        context.AddTokzInventoryItems(mmentity);
                        context.SaveChanges();
                        var merchantEntity = from d in context.kzMerchants
                                             where d.id == item.Merchant.MerchantId
                                             select d;
                        string merchantname = "";
                        if (merchantEntity.Count() > 0) { merchantname = merchantEntity.First().name; }
                        var itemtypeEntity = from d in context.kzInventoryItemTypes
                                             where d.id == item.InventoryItemType.InventoryItemTypeId
                                             select d;
                        string itemtype = itemtypeEntity.First().name;

                        int InventoryItemid = mmentity.id;
                        if (item.Tag != null)
                        {
                            string[] taglist = item.Tag.Split(',');
                            foreach(var v in taglist)
                            {
                                try
                                {
                                    int tagid = Convert.ToInt16(v);
                                    entity.kzInventoryItemTag mmtag = new entity.kzInventoryItemTag();
                                    mmtag.inventoryitem_id = InventoryItemid;
                                    mmtag.tag_id = tagid;
                                    context.AddTokzInventoryItemTags(mmtag);
                                }
                                catch{
                                    
                                }
                            }
                            context.SaveChanges();
                        }
                        #region variance
                        if (item.Variance != null)
                        {
                            try
                            {

                                int countv = 0;
                                foreach (var v in item.Variance)
                                {
                                    countv = countv + 1;
                                    entity.kzVariance mmvar = new entity.kzVariance();
                                    mmvar.inventoryitem_id = InventoryItemid;
                                    mmvar.name = v.Variance;
                                    mmvar.price = v.Price;
                                    mmvar.discount = v.Discount;
                                    mmvar.margin = v.Margin;
                                    mmvar.available_limit = v.AvailabelLimit;
                                    mmvar.sku = merchantname.Substring(0, 2).ToUpper() + "-" + String.Format("{0:D4}", InventoryItemid) + "-" + itemtype.Substring(0, 2).ToUpper() + "-" + string.Format("{0:D3}", countv);
                                    context.AddTokzVariances(mmvar);
                                    context.SaveChanges();
                                }
                            }
                            catch
                            {

                            }
                        }
                        #endregion
                        #region image
                        if (item.ImageId != null && item.ImageId != 0)
                        {
                            try
                            {
                                entity.kzInventoryItemImage mmimage = new entity.kzInventoryItemImage();
                                mmimage.inventoryitem_id = InventoryItemid;
                                mmimage.image_id = item.ImageId;
                                mmimage.main = true;
                                context.AddTokzInventoryItemImages(mmimage);
                                context.SaveChanges();
                            }
                            catch
                            {

                            }
                        }
                        if (item.SubImageId != null)
                        {
                            try
                            {
                                foreach (var v in item.SubImageId)
                                {
                                    entity.kzInventoryItemImage mmimage = new entity.kzInventoryItemImage();
                                    mmimage.inventoryitem_id = InventoryItemid;
                                    mmimage.image_id = v;
                                    mmimage.main = false;
                                    context.AddTokzInventoryItemImages(mmimage);
                                    context.SaveChanges();
                                }
                            }
                            catch
                            {

                            }
                        }
                        #endregion
                      
                        #region flashdeal
                        if (item.FlashDeal != null)
                        {
                            item.FlashDeal.InventoryItemId = InventoryItemid;
                            try
                            {
                                new FlashDealService().CreateFlashDeal(item.FlashDeal);
                            }
                            catch
                            {

                            }
                        }
                        #endregion
                        if (item.Draft == false)
                        {
                            new TransactionService().UpdateSalesVisualMeterNewInventory(InventoryItemid);
                        }
                        response = Response<bool>.Create(true);
                    }
                }
            }
            return response;
        }
        public Response<List<InventoryItem>> GetInventoryItemList()
        {
            Response<List<InventoryItem>> response = null;
            List<InventoryItem> InventoryItemList = new List<InventoryItem>();
            using (var context = new entity.KuazooEntities())
            {
                var entityInventoryItem = from d in context.kzInventoryItems.Include("kzMerchant").Include("kzInventoryItemType")
                                          where d.last_action!="5"
                                          orderby d.last_created descending
                                 select d;
                foreach (var v in entityInventoryItem)
                {
                    InventoryItem InventoryItem = new InventoryItem();
                    InventoryItem.Draft = (bool)v.draft;
                    InventoryItem.InventoryItemId = v.id;
                    InventoryItem.Name = v.name;
                    InventoryItem.ShortDesc = v.short_desc;
                    InventoryItem.GeneralDescription = "";
                    if (v.general_description != null)
                    {
                        InventoryItem.GeneralDescription = v.general_description;
                    }
                    InventoryItem.Priority = (int)v.priority;
                    InventoryItem.Price = (decimal)v.price;
                    InventoryItem.Margin = (decimal)v.margin;
                    InventoryItem.Description = v.description;
                    InventoryItem.Terms = v.terms;
                    InventoryItem.Keyword = v.keywords;

                    City merchantcity = new City()
                    {
                        CityId = (int)v.kzMerchant.city_id,
                        Name = v.kzMerchant.kzCity.name
                    };
                    Country merchantcountry = new Country()
                    {
                        CountryId = (int)v.kzMerchant.kzCity.country_id,
                        Name = v.kzMerchant.kzCity.kzCountry.name
                    };

                    Merchant merchant = new Merchant()
                    {
                        MerchantId = (int)v.merchant_id,
                        Name = v.kzMerchant.name,
                        City = merchantcity,
                        Country = merchantcountry
                    };
                    InventoryItem.Merchant = merchant;

                    City city = new City()
                    {
                        CityId = (int)v.city_id,
                        Name = v.kzCity.name
                    };
                    InventoryItem.City = city;

                    Country country = new Country()
                    {
                        CountryId = (int)v.kzCity.country_id,
                        Name = v.kzCity.kzCountry.name
                    };
                    InventoryItem.Country = country;

                    InventoryItemType InventoryItemtype = new InventoryItemType()
                    {
                        InventoryItemTypeId = (int)v.inventoryitem_type_id,
                        InventoryItemTypeName = v.kzInventoryItemType.name
                    };
                    InventoryItem.InventoryItemType = InventoryItemtype;
                    InventoryItem.Discount = (decimal)v.discount;
                    InventoryItem.ExpireDate = (DateTime)v.expiry_date;
                    InventoryItem.PublishDate = (DateTime)v.publish_date;
                    InventoryItem.MinimumTarget = (decimal)v.minimumtarget;
                    InventoryItem.SalesVisualMeter = (decimal)v.salesvisualmeter;
                    InventoryItem.MaximumSales = (decimal)v.maximumsales;
                    InventoryItem.RemainSales = (decimal)v.remainsales;
                    InventoryItem.Create = (DateTime)v.last_created;
                    InventoryItem.Update = (DateTime)v.last_updated;
                    InventoryItem.Flag = (bool)v.flag;
                    InventoryItem.Featured = (bool)v.featured;
                    InventoryItem.LastAction = v.last_action;
                   
                    if(v.kzInventoryItemRP!=null)
                    {
                        InventoryItem.Popular = (bool)v.kzInventoryItemRP.popular;
                        InventoryItem.Reviewed = (bool)v.kzInventoryItemRP.reviewed;
                    }
                    else
                    {
                        InventoryItem.Popular = false;
                        InventoryItem.Reviewed = false;
                    }

                    string currtag = "";
                    string currtagname = "";
                    var entityTag = from d in context.kzInventoryItemTags
                                    from e in context.kzTags
                                    where d.tag_id == e.id && d.inventoryitem_id == v.id
                                    orderby d.tag_id ascending
                                    select new { d.tag_id, e.name };
                    foreach (var v2 in entityTag)
                    {
                        currtag += v2.tag_id + ",";
                        currtagname += v2.name + ", ";
                    }
                    if (currtag.Length > 1)
                    {
                        currtag = currtag.Substring(0, currtag.Length - 1);
                        currtagname = currtagname.Substring(0, currtagname.Length - 2);
                    }
                    InventoryItem.Tag = currtag;
                    InventoryItem.TagName = currtagname;

                    List<VarianceDetail> listvariance = new List<VarianceDetail>();
                    var entityVarience = from d in context.kzVariances
                                         where d.inventoryitem_id == v.id
                                         select d;
                    foreach (var varien in entityVarience)
                    {
                        listvariance.Add(new VarianceDetail() { Variance = varien.name, Price = (decimal)varien.price, Discount= (decimal)varien.discount,
                            Margin= (decimal)varien.margin, AvailabelLimit=(int)varien.available_limit });
                    }
                    InventoryItem.Variance = listvariance;

                    var entityImg = from d in context.kzInventoryItemImages
                                    from e in context.kzImages
                                    where d.image_id == e.id && d.inventoryitem_id == v.id
                                    && d.main == true
                                    select new { d.image_id, e.url };

                    if (entityImg.Count() > 0)
                    {
                        InventoryItem.ImageId = entityImg.First().image_id;
                        InventoryItem.ImageName = entityImg.First().url;
                        InventoryItem.ImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImg.First().url;
                    }
                    else
                    {
                        InventoryItem.ImageId = 0;
                        InventoryItem.ImageName = "";
                        InventoryItem.ImageUrl = "";
                    }
                    
                    List<int> subimgid = new List<int>();
                    List<string> subimgname = new List<string>();
                    List<string> subimgurl = new List<string>();
                    var entitySubImg = from d in context.kzInventoryItemImages
                                    from e in context.kzImages
                                    where d.image_id == e.id && d.inventoryitem_id == v.id
                                    && d.main == false
                                    select new { d.image_id, e.url };
                    foreach(var v2 in entitySubImg)
                    {
                        subimgid.Add(v2.image_id);
                        subimgname.Add(v2.url);
                        subimgurl.Add(ConfigurationManager.AppSettings["uploadpath"] + v2.url);
                    }
                    InventoryItem.SubImageId = subimgid;
                    InventoryItem.SubImageName = subimgname;
                    InventoryItem.SubImageUrl = subimgurl;

                    decimal currentvisualmeter = InventoryItem.SalesVisualMeter;

                    var entityPrize = v.kzPrize;
                    if (entityPrize != null)
                    {
                        decimal fakevisual = (decimal)entityPrize.fake_visualmeter;
                        if (currentvisualmeter < fakevisual)
                        {
                            InventoryItem.SalesVisualMeter = fakevisual;
                        }   
                        Prize pr = new Prize();
                        pr.PrizeId = (int)entityPrize.id;
                        pr.Name = entityPrize.name;
                        pr.Price= (decimal)entityPrize.price;
                        pr.Description = entityPrize.description;
                        pr.ExpiryDate = (DateTime)entityPrize.expiry_date;
                        pr.GroupName = entityPrize.group_name == null ? string.Empty : entityPrize.group_name;

                        var entityImgPrize = from d in context.kzPrizeImages
                                        from e in context.kzImages
                                        where d.image_id == e.id && d.prize_id == pr.PrizeId
                                        && d.main == true
                                        select new { d.image_id, e.url };

                        if (entityImgPrize.Count() > 0)
                        {
                            pr.ImageId = entityImgPrize.First().image_id;
                            pr.ImageName = entityImgPrize.First().url;
                            pr.ImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImgPrize.First().url;
                        }
                        else
                        {
                            pr.ImageId = 0;
                            pr.ImageName = "";
                            pr.ImageUrl = "";
                        }

                        List<int> subimgidprize = new List<int>();
                        List<string> subimgnameprize = new List<string>();
                        List<string> subimgurlprize = new List<string>();
                        var entitySubImgprize = from d in context.kzPrizeImages
                                                from e in context.kzImages
                                                where d.image_id == e.id && d.prize_id == pr.PrizeId
                                                && d.main == false
                                                select new { d.image_id, e.url };
                        foreach (var v2 in entitySubImgprize)
                        {
                            subimgidprize.Add(v2.image_id);
                            subimgnameprize.Add(v2.url);
                            subimgurlprize.Add(ConfigurationManager.AppSettings["uploadpath"] + v2.url);
                        }
                        pr.SubImageId = subimgidprize;
                        pr.SubImageName = subimgnameprize;
                        pr.SubImageUrl = subimgurlprize;

                        pr.SponsorName = entityPrize.sponsor_name;
                        pr.Terms = entityPrize.terms;
                        pr.Detail = entityPrize.detail;
                        pr.FreeDeal = false;
                        if (v.kzPrize.freedeal != null)
                        {
                            pr.FreeDeal = (bool)v.kzPrize.freedeal;
                        }
                        InventoryItem.Prize = pr;
                    }
                    InventoryItemList.Add(InventoryItem);
                }
                response = Response<List<InventoryItem>>.Create(InventoryItemList);
            }

            return response;
        }

        public Response<List<InventoryItem>> GetInventoryItemListExpireSoon()
        {
            Response<List<InventoryItem>> response = null;
            List<InventoryItem> InventoryItemList = new List<InventoryItem>();
            using (var context = new entity.KuazooEntities())
            {
                DateTime today = DateTime.UtcNow;
                DateTime daymin5 = today.AddDays(-5);
                var entityInventoryItem = from d in context.kzInventoryItems.Include("kzMerchant").Include("kzInventoryItemType")
                                          where d.kzPrize.last_action != "5" && d.kzPrize.expiry_date >= daymin5 && d.kzPrize.expiry_date <= today
                                          orderby d.last_created descending
                                          select d;
                foreach (var v in entityInventoryItem)
                {
                    InventoryItem InventoryItem = new InventoryItem();
                    InventoryItem.InventoryItemId = v.id;
                    InventoryItem.Name = v.name;
                    InventoryItem.ExpireDate = (DateTime)v.kzPrize.expiry_date;

                    InventoryItemList.Add(InventoryItem);
                }
                response = Response<List<InventoryItem>>.Create(InventoryItemList);
            }

            return response;
        }
        public Response<List<InventoryItem>> GetInventoryItemArchivedList()
        {
            Response<List<InventoryItem>> response = null;
            List<InventoryItem> InventoryItemList = new List<InventoryItem>();
            using (var context = new entity.KuazooEntities())
            {
                var entityInventoryItem = from d in context.kzInventoryItems.Include("kzMerchant").Include("kzInventoryItemType")
                                          where d.last_action == "5"
                                          orderby d.last_created descending
                                          select d;
                foreach (var v in entityInventoryItem)
                {
                    InventoryItem InventoryItem = new InventoryItem();
                    InventoryItem.Draft = (bool)v.draft;
                    InventoryItem.InventoryItemId = v.id;
                    InventoryItem.Name = v.name;
                    InventoryItem.ShortDesc = v.short_desc;
                    InventoryItem.GeneralDescription = "";
                    if (v.general_description != null)
                    {
                        InventoryItem.GeneralDescription = v.general_description;
                    }
                    InventoryItem.Price = (decimal)v.price;
                    InventoryItem.Margin = (decimal)v.margin;
                    InventoryItem.Description = v.description;
                    InventoryItem.Terms = v.terms;
                    InventoryItem.Keyword = v.keywords;


                    City merchantcity = new City()
                    {
                        CityId = (int)v.kzMerchant.city_id,
                        Name = v.kzMerchant.kzCity.name
                    };
                    Country merchantcountry = new Country()
                    {
                        CountryId = (int)v.kzMerchant.kzCity.country_id,
                        Name = v.kzMerchant.kzCity.kzCountry.name
                    };

                    Merchant merchant = new Merchant()
                    {
                        MerchantId = (int)v.merchant_id,
                        Name = v.kzMerchant.name,
                        City = merchantcity,
                        Country = merchantcountry
                    };
                    InventoryItem.Merchant = merchant;

                    City city = new City()
                    {
                        CityId = (int)v.city_id,
                        Name = v.kzCity.name
                    };
                    InventoryItem.City = city;

                    Country country = new Country()
                    {
                        CountryId = (int)v.kzCity.country_id,
                        Name = v.kzCity.kzCountry.name
                    };
                    InventoryItem.Country = country;
                    InventoryItem.Merchant = merchant;
                    InventoryItemType InventoryItemtype = new InventoryItemType()
                    {
                        InventoryItemTypeId = (int)v.inventoryitem_type_id,
                        InventoryItemTypeName = v.kzInventoryItemType.name
                    };
                    InventoryItem.InventoryItemType = InventoryItemtype;
                    InventoryItem.Discount = (decimal)v.discount;
                    InventoryItem.ExpireDate = (DateTime)v.expiry_date;
                    InventoryItem.PublishDate = (DateTime)v.publish_date;
                    InventoryItem.MinimumTarget = (decimal)v.minimumtarget;
                    InventoryItem.SalesVisualMeter = (decimal)v.salesvisualmeter;
                    InventoryItem.MaximumSales = (decimal)v.maximumsales;
                    //InventoryItem.RemainSales = (decimal)v.remainsales;
                    InventoryItem.Create = (DateTime)v.last_created;
                    InventoryItem.Update = (DateTime)v.last_updated;
                    InventoryItem.Flag = (bool)v.flag;
                    InventoryItem.Featured = (bool)v.featured;
                    InventoryItem.LastAction = v.last_action;
                    string currtag = "";
                    string currtagname = "";
                    var entityTag = from d in context.kzInventoryItemTags
                                    from e in context.kzTags
                                    where d.tag_id == e.id && d.inventoryitem_id == v.id
                                    orderby d.tag_id ascending
                                    select new { d.tag_id, e.name };
                    foreach (var v2 in entityTag)
                    {
                        currtag += v2.tag_id + ",";
                        currtagname += v2.name + ", ";
                    }
                    if (currtag.Length > 1)
                    {
                        currtag = currtag.Substring(0, currtag.Length - 1);
                        currtagname = currtagname.Substring(0, currtagname.Length - 2);
                    }
                    InventoryItem.Tag = currtag;
                    InventoryItem.TagName = currtagname;

                    List<VarianceDetail> listvariance = new List<VarianceDetail>();
                    var entityVarience = from d in context.kzVariances
                                         where d.inventoryitem_id == v.id
                                         select d;
                    foreach (var varien in entityVarience)
                    {
                        listvariance.Add(new VarianceDetail() { Variance = varien.name, Price = (decimal)varien.price, Discount = (decimal)varien.discount });
                    }
                    InventoryItem.Variance = listvariance;

                    var entityImg = from d in context.kzInventoryItemImages
                                    from e in context.kzImages
                                    where d.image_id == e.id && d.inventoryitem_id == v.id
                                    && d.main == true
                                    select new { d.image_id, e.url };

                    if (entityImg.Count() > 0)
                    {
                        InventoryItem.ImageId = entityImg.First().image_id;
                        InventoryItem.ImageName = entityImg.First().url;
                        InventoryItem.ImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImg.First().url;
                    }
                    else
                    {
                        InventoryItem.ImageId = 0;
                        InventoryItem.ImageName = "";
                        InventoryItem.ImageUrl = "";
                    }

                    List<int> subimgid = new List<int>();
                    List<string> subimgname = new List<string>();
                    List<string> subimgurl = new List<string>();
                    var entitySubImg = from d in context.kzInventoryItemImages
                                       from e in context.kzImages
                                       where d.image_id == e.id && d.inventoryitem_id == v.id
                                       && d.main == false
                                       select new { d.image_id, e.url };
                    foreach (var v2 in entitySubImg)
                    {
                        subimgid.Add(v2.image_id);
                        subimgname.Add(v2.url);
                        subimgurl.Add(ConfigurationManager.AppSettings["uploadpath"] + v2.url);
                    }
                    InventoryItem.SubImageId = subimgid;
                    InventoryItem.SubImageName = subimgname;
                    InventoryItem.SubImageUrl = subimgurl;

                    decimal currentvisualmeter = InventoryItem.SalesVisualMeter;

                    var entityPrize = v.kzPrize;
                    if (entityPrize != null)
                    {
                        decimal fakevisual = (decimal)entityPrize.fake_visualmeter;
                        if (currentvisualmeter < fakevisual)
                        {
                            InventoryItem.SalesVisualMeter = fakevisual;
                        }   
                        Prize pr = new Prize();
                        pr.PrizeId = (int)entityPrize.id;
                        pr.Name = entityPrize.name;
                        pr.Price = (decimal)entityPrize.price;
                        pr.Description = entityPrize.description;
                        pr.ExpiryDate = (DateTime)entityPrize.expiry_date;
                        pr.GroupName = entityPrize.group_name == null ? string.Empty : entityPrize.group_name;

                        var entityImgPrize = from d in context.kzPrizeImages
                                             from e in context.kzImages
                                             where d.image_id == e.id && d.prize_id == pr.PrizeId
                                             && d.main == true
                                             select new { d.image_id, e.url };

                        if (entityImgPrize.Count() > 0)
                        {
                            pr.ImageId = entityImgPrize.First().image_id;
                            pr.ImageName = entityImgPrize.First().url;
                            pr.ImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImgPrize.First().url;
                        }
                        else
                        {
                            pr.ImageId = 0;
                            pr.ImageName = "";
                            pr.ImageUrl = "";
                        }

                        List<int> subimgidprize = new List<int>();
                        List<string> subimgnameprize = new List<string>();
                        List<string> subimgurlprize = new List<string>();
                        var entitySubImgprize = from d in context.kzPrizeImages
                                                from e in context.kzImages
                                                where d.image_id == e.id && d.prize_id == pr.PrizeId
                                                && d.main == false
                                                select new { d.image_id, e.url };
                        foreach (var v2 in entitySubImgprize)
                        {
                            subimgidprize.Add(v2.image_id);
                            subimgnameprize.Add(v2.url);
                            subimgurlprize.Add(ConfigurationManager.AppSettings["uploadpath"] + v2.url);
                        }
                        pr.SubImageId = subimgidprize;
                        pr.SubImageName = subimgnameprize;
                        pr.SubImageUrl = subimgurlprize;

                        pr.SponsorName = entityPrize.sponsor_name;
                        pr.Terms = entityPrize.terms;
                        pr.Detail = entityPrize.detail;
                        pr.FreeDeal = false;
                        if (v.kzPrize.freedeal != null)
                        {
                            pr.FreeDeal = (bool)v.kzPrize.freedeal;
                        }
                        InventoryItem.Prize = pr;
                    }
                    InventoryItemList.Add(InventoryItem);
                }
                response = Response<List<InventoryItem>>.Create(InventoryItemList);
            }

            return response;
        }
             
        public Response<List<InventoryItem>> GetInventoryItemListStatusTypeCityDate(String InventoryItemName,String PrizeName, int StatusId, int TypeId, int CityId, DateTime From, DateTime To)
        {
            Response<List<InventoryItem>> response = null;
            List<InventoryItem> InventoryItemList = new List<InventoryItem>();
            using (var context = new entity.KuazooEntities())
            {
                var entityInventoryItem = from d in context.kzInventoryItems.Include("kzMerchant").Include("kzInventoryItemType")
                                          where d.publish_date >= From && d.publish_date <= To
                                          select d;
                if (InventoryItemName != null && InventoryItemName != "")
                {
                    entityInventoryItem = entityInventoryItem.Where(x => x.name.Contains(InventoryItemName));
                }
                if (PrizeName != null && PrizeName != "")
                {
                    entityInventoryItem = entityInventoryItem.Where(x => x.kzPrize.name.Contains(PrizeName));
                }
                if (StatusId != 0)
                {
                    bool flag = false;
                    if (StatusId == 1) flag = true;
                    entityInventoryItem = entityInventoryItem.Where(x => x.flag == flag);
                }
                if (TypeId != 0)
                {
                    entityInventoryItem = entityInventoryItem.Where(x => x.inventoryitem_type_id == TypeId);
                }
                if (CityId != 0)
                {
                    entityInventoryItem = entityInventoryItem.Where(x => x.kzMerchant.city_id == CityId);
                }
                
                foreach (var v in entityInventoryItem)
                {
                    InventoryItem InventoryItem = new InventoryItem();
                    InventoryItem.InventoryItemId = v.id;
                    InventoryItem.Name = v.name;
                    InventoryItem.ShortDesc = v.short_desc;
                    InventoryItem.GeneralDescription = "";
                    if (v.general_description != null)
                    {
                        InventoryItem.GeneralDescription = v.general_description;
                    }
                    InventoryItem.Price = (decimal)v.price;
                    InventoryItem.Margin = (decimal)v.margin;
                    InventoryItem.Description = v.description;
                    InventoryItem.Terms = v.terms;
                    InventoryItem.Keyword = v.keywords;


                    City merchantcity = new City()
                    {
                        CityId = (int)v.kzMerchant.city_id,
                        Name = v.kzMerchant.kzCity.name
                    };
                    Country merchantcountry = new Country()
                    {
                        CountryId = (int)v.kzMerchant.kzCity.country_id,
                        Name = v.kzMerchant.kzCity.kzCountry.name
                    };

                    Merchant merchant = new Merchant()
                    {
                        MerchantId = (int)v.merchant_id,
                        Name = v.kzMerchant.name,
                        City = merchantcity,
                        Country = merchantcountry
                    };
                    InventoryItem.Merchant = merchant;

                    City city = new City()
                    {
                        CityId = (int)v.city_id,
                        Name = v.kzCity.name
                    };
                    InventoryItem.City = city;

                    Country country = new Country()
                    {
                        CountryId = (int)v.kzCity.country_id,
                        Name = v.kzCity.kzCountry.name
                    };
                    InventoryItem.Country = country;
                    InventoryItem.Merchant = merchant;
                    InventoryItemType InventoryItemtype = new InventoryItemType()
                    {
                        InventoryItemTypeId = (int)v.inventoryitem_type_id,
                        InventoryItemTypeName = v.kzInventoryItemType.name
                    };
                    InventoryItem.InventoryItemType = InventoryItemtype;
                    InventoryItem.Discount = (decimal)v.discount;
                    InventoryItem.ExpireDate = (DateTime)v.expiry_date;
                    InventoryItem.PublishDate = (DateTime)v.publish_date;
                    InventoryItem.MinimumTarget = (decimal)v.minimumtarget;
                    InventoryItem.SalesVisualMeter = (decimal)v.salesvisualmeter;
                    InventoryItem.MaximumSales = (decimal)v.maximumsales;
                    //InventoryItem.RemainSales = (decimal)v.remainsales;
                    InventoryItem.Create = (DateTime)v.last_created;
                    InventoryItem.Update = (DateTime)v.last_updated;
                    InventoryItem.Flag = (bool)v.flag;
                    InventoryItem.Featured = (bool)v.featured;
                    InventoryItem.LastAction = v.last_action;
                    string currtag = "";
                    string currtagname = "";
                    var entityTag = from d in context.kzInventoryItemTags
                                    from e in context.kzTags
                                    where d.tag_id == e.id && d.inventoryitem_id == v.id
                                    orderby d.tag_id ascending
                                    select new { d.tag_id, e.name };
                    foreach (var v2 in entityTag)
                    {
                        currtag += v2.tag_id + ",";
                        currtagname += v2.name + ", ";
                    }
                    if (currtag.Length > 1)
                    {
                        currtag = currtag.Substring(0, currtag.Length - 1);
                        currtagname = currtagname.Substring(0, currtagname.Length - 2);
                    }
                    InventoryItem.Tag = currtag;
                    InventoryItem.TagName = currtagname;

                    List<VarianceDetail> listvariance = new List<VarianceDetail>();
                    var entityVarience = from d in context.kzVariances
                                         where d.inventoryitem_id == v.id
                                         select d;
                    foreach (var varien in entityVarience)
                    {
                        listvariance.Add(new VarianceDetail() { Variance = varien.name, Price = (decimal)varien.price, Discount = (decimal)varien.discount });
                    }
                    InventoryItem.Variance = listvariance;

                    decimal currentvisualmeter = InventoryItem.SalesVisualMeter;

                    var entityPrize = v.kzPrize;
                    if (entityPrize != null)
                    {
                        decimal fakevisual = (decimal)entityPrize.fake_visualmeter;
                        if (currentvisualmeter < fakevisual)
                        {
                            InventoryItem.SalesVisualMeter = fakevisual;
                        }   
                        Prize pr = new Prize();
                        pr.PrizeId = (int)entityPrize.id;
                        pr.Name = entityPrize.name;
                        pr.Price = (decimal)entityPrize.price;
                        pr.Description = entityPrize.description;
                        pr.ExpiryDate = (DateTime)entityPrize.expiry_date;
                        pr.GroupName = entityPrize.group_name == null ? string.Empty : entityPrize.group_name;
                        pr.SponsorName = entityPrize.sponsor_name;
                        pr.Terms = entityPrize.terms;
                        pr.Detail = entityPrize.detail;
                        pr.FreeDeal = false;
                        if (v.kzPrize.freedeal != null)
                        {
                            pr.FreeDeal = (bool)v.kzPrize.freedeal;
                        }
                        InventoryItem.Prize = pr;
                    }
                    InventoryItemList.Add(InventoryItem);
                }
                response = Response<List<InventoryItem>>.Create(InventoryItemList);
            }

            return response;
        }

        public Response<List<InventoryItem>> GetInventoryItemListByPrize(int PrizeId)
        {
            Response<List<InventoryItem>> response = null;
            List<InventoryItem> InventoryItemList = new List<InventoryItem>();
            using (var context = new entity.KuazooEntities())
            {
                var entityInventoryItem = from d in context.kzInventoryItems.Include("kzMerchant").Include("kzInventoryItemType")
                                          where d.prize_id == PrizeId && d.last_action !="5"
                                          select d;
                foreach (var v in entityInventoryItem)
                {
                    InventoryItem InventoryItem = new InventoryItem();
                    InventoryItem.Draft = (bool)v.draft;
                    InventoryItem.InventoryItemId = v.id;
                    InventoryItem.Name = v.name;
                    InventoryItem.ShortDesc = v.short_desc;
                    InventoryItem.GeneralDescription = "";
                    if (v.general_description != null)
                    {
                        InventoryItem.GeneralDescription = v.general_description;
                    }
                    InventoryItem.Price = (decimal)v.price;
                    InventoryItem.Margin = (decimal)v.margin;
                    InventoryItem.Description = v.description;
                    InventoryItem.Terms = v.terms;
                    InventoryItem.Keyword = v.keywords;


                    City merchantcity = new City()
                    {
                        CityId = (int)v.kzMerchant.city_id,
                        Name = v.kzMerchant.kzCity.name
                    };
                    Country merchantcountry = new Country()
                    {
                        CountryId = (int)v.kzMerchant.kzCity.country_id,
                        Name = v.kzMerchant.kzCity.kzCountry.name
                    };

                    Merchant merchant = new Merchant()
                    {
                        MerchantId = (int)v.merchant_id,
                        Name = v.kzMerchant.name,
                        City = merchantcity,
                        Country = merchantcountry
                    };
                    InventoryItem.Merchant = merchant;

                    City city = new City()
                    {
                        CityId = (int)v.city_id,
                        Name = v.kzCity.name
                    };
                    InventoryItem.City = city;

                    Country country = new Country()
                    {
                        CountryId = (int)v.kzCity.country_id,
                        Name = v.kzCity.kzCountry.name
                    };
                    InventoryItem.Country = country;
                    InventoryItem.Merchant = merchant;
                    InventoryItemType InventoryItemtype = new InventoryItemType()
                    {
                        InventoryItemTypeId = (int)v.inventoryitem_type_id,
                        InventoryItemTypeName = v.kzInventoryItemType.name
                    };
                    InventoryItem.InventoryItemType = InventoryItemtype;
                    InventoryItem.Discount = (decimal)v.discount;
                    InventoryItem.ExpireDate = (DateTime)v.expiry_date;
                    InventoryItem.PublishDate = (DateTime)v.publish_date;
                    InventoryItem.MinimumTarget = (decimal)v.minimumtarget;
                    InventoryItem.SalesVisualMeter = (decimal)v.salesvisualmeter;
                    InventoryItem.MaximumSales = (decimal)v.maximumsales;
                    //InventoryItem.RemainSales = (decimal)v.remainsales;
                    InventoryItem.Create = (DateTime)v.last_created;
                    InventoryItem.Update = (DateTime)v.last_updated;
                    InventoryItem.Flag = (bool)v.flag;
                    InventoryItem.Featured = (bool)v.featured;
                    InventoryItem.LastAction = v.last_action;
                    string currtag = "";
                    string currtagname = "";
                    var entityTag = from d in context.kzInventoryItemTags
                                    from e in context.kzTags
                                    where d.tag_id == e.id && d.inventoryitem_id == v.id
                                    orderby d.tag_id ascending
                                    select new { d.tag_id, e.name };
                    foreach (var v2 in entityTag)
                    {
                        currtag += v2.tag_id + ",";
                        currtagname += v2.name + ", ";
                    }
                    if (currtag.Length > 1)
                    {
                        currtag = currtag.Substring(0, currtag.Length - 1);
                        currtagname = currtagname.Substring(0, currtagname.Length - 2);
                    }
                    InventoryItem.Tag = currtag;
                    InventoryItem.TagName = currtagname;

                    List<VarianceDetail> listvariance = new List<VarianceDetail>();
                    var entityVarience = from d in context.kzVariances
                                         where d.inventoryitem_id == v.id
                                         select d;
                    foreach (var varien in entityVarience)
                    {
                        listvariance.Add(new VarianceDetail() { Variance = varien.name, Price = (decimal)varien.price, Discount = (decimal)varien.discount });
                    }
                    InventoryItem.Variance = listvariance;

                    var entityImg = from d in context.kzInventoryItemImages
                                    from e in context.kzImages
                                    where d.image_id == e.id && d.inventoryitem_id == v.id
                                    && d.main == true
                                    select new { d.image_id, e.url };

                    if (entityImg.Count() > 0)
                    {
                        InventoryItem.ImageId = entityImg.First().image_id;
                        InventoryItem.ImageName = entityImg.First().url;
                        InventoryItem.ImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImg.First().url;
                    }
                    else
                    {
                        InventoryItem.ImageId = 0;
                        InventoryItem.ImageName = "";
                        InventoryItem.ImageUrl = "";
                    }

                    List<int> subimgid = new List<int>();
                    List<string> subimgname = new List<string>();
                    List<string> subimgurl = new List<string>();
                    var entitySubImg = from d in context.kzInventoryItemImages
                                       from e in context.kzImages
                                       where d.image_id == e.id && d.inventoryitem_id == v.id
                                       && d.main == false
                                       select new { d.image_id, e.url };
                    foreach (var v2 in entitySubImg)
                    {
                        subimgid.Add(v2.image_id);
                        subimgname.Add(v2.url);
                        subimgurl.Add(ConfigurationManager.AppSettings["uploadpath"] + v2.url);
                    }
                    InventoryItem.SubImageId = subimgid;
                    InventoryItem.SubImageName = subimgname;
                    InventoryItem.SubImageUrl = subimgurl;

                    decimal currentvisualmeter = InventoryItem.SalesVisualMeter;

                    var entityPrize = v.kzPrize;
                    if (entityPrize != null)
                    {
                        decimal fakevisual = (decimal)entityPrize.fake_visualmeter;
                        if (currentvisualmeter < fakevisual)
                        {
                            InventoryItem.SalesVisualMeter = fakevisual;
                        }   
                        Prize pr = new Prize();
                        pr.PrizeId = (int)entityPrize.id;
                        pr.Name = entityPrize.name;
                        pr.Price = (decimal)entityPrize.price;
                        pr.Description = entityPrize.description;
                        pr.ExpiryDate = (DateTime)entityPrize.expiry_date;
                        pr.GroupName = entityPrize.group_name == null ? string.Empty : entityPrize.group_name;

                        var entityImgPrize = from d in context.kzPrizeImages
                                             from e in context.kzImages
                                             where d.image_id == e.id && d.prize_id == pr.PrizeId
                                             && d.main == true
                                             select new { d.image_id, e.url };

                        if (entityImgPrize.Count() > 0)
                        {
                            pr.ImageId = entityImgPrize.First().image_id;
                            pr.ImageName = entityImgPrize.First().url;
                            pr.ImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImgPrize.First().url;
                        }
                        else
                        {
                            pr.ImageId = 0;
                            pr.ImageName = "";
                            pr.ImageUrl = "";
                        }

                        List<int> subimgidprize = new List<int>();
                        List<string> subimgnameprize = new List<string>();
                        List<string> subimgurlprize = new List<string>();
                        var entitySubImgprize = from d in context.kzPrizeImages
                                                from e in context.kzImages
                                                where d.image_id == e.id && d.prize_id == pr.PrizeId
                                                && d.main == false
                                                select new { d.image_id, e.url };
                        foreach (var v2 in entitySubImgprize)
                        {
                            subimgidprize.Add(v2.image_id);
                            subimgnameprize.Add(v2.url);
                            subimgurlprize.Add(ConfigurationManager.AppSettings["uploadpath"] + v2.url);
                        }
                        pr.SubImageId = subimgidprize;
                        pr.SubImageName = subimgnameprize;
                        pr.SubImageUrl = subimgurlprize;

                        pr.SponsorName = entityPrize.sponsor_name;
                        pr.Terms = entityPrize.terms;
                        pr.Detail = entityPrize.detail;
                        pr.FreeDeal = false;
                        if (v.kzPrize.freedeal != null)
                        {
                            pr.FreeDeal = (bool)v.kzPrize.freedeal;
                        }
                        InventoryItem.Prize = pr;
                    }
                    InventoryItemList.Add(InventoryItem);
                }
                response = Response<List<InventoryItem>>.Create(InventoryItemList);
            }

            return response;
        }

        public Response<List<InventoryItemShow>> GetInventoryItemListByCity(int CityId, DateTime today, ref int totalCount, string Category="", int pagesize=4, int page=1)
        {
            Response<List<InventoryItemShow>> response = null;
            List<InventoryItemShow> InventoryItemList = new List<InventoryItemShow>();
            var user = new UserService().GetCurrentUserWithoutTrace().Result;
            using (var context = new entity.KuazooEntities())
            {
                var entityInventoryItem = from d in context.kzInventoryItems.Include("kzMerchant").Include("kzInventoryItemType")
                                          where d.kzPrize.last_action !="5" && d.kzPrize.expiry_date > today && today >= d.publish_date
                                          && d.flag == true && d.last_action != "5"
                                          && d.draft == false
                                          select d;
                if (CityId != 0)
                {
                    entityInventoryItem = entityInventoryItem.Where(x => x.city_id == CityId);
                }
                if (Category != "")
                {
                    List<int> currtag = new List<int>();
                    String[] listCat = Category.Split(',');
                    var entityTag = from d in context.kzTags
                                    where listCat.Contains(d.name)
                                    select d;
                    foreach (var v2 in entityTag)
                    {
                        currtag.Add(v2.id);
                    }
                    var entityTag2 = from d in context.kzInventoryItemTags
                                     where currtag.Contains(d.tag_id)
                                     select d;
                    List<int> currInv = new List<int>();
                    foreach (var v in entityTag2)
                    {
                        currInv.Add(v.inventoryitem_id);
                    }
                    entityInventoryItem = entityInventoryItem.Where(x => currInv.Contains(x.id));
                }
                totalCount = entityInventoryItem.Count();
                entityInventoryItem = entityInventoryItem.OrderBy(x => x.priority).ThenByDescending(x => x.publish_date);
                foreach (var v in entityInventoryItem.Skip(pagesize * (page)).Take(pagesize))
                {
                    InventoryItemShow InventoryItem = new InventoryItemShow();

                    InventoryItem.Wishlist = false;
                    if (user.UserId != 0)
                    {
                        if (v.kzWishLists.Where(x => x.user_id == user.UserId && x.last_action !="5").Count() > 0)
                        {
                            InventoryItem.Wishlist = true;
                        }
                    }

                    InventoryItem.InventoryItemId = v.id;
                    InventoryItem.Name = v.name;
                    InventoryItem.ShortDesc = v.short_desc;
                    InventoryItem.GeneralDescription = "";
                    if (v.general_description != null)
                    {
                        InventoryItem.GeneralDescription = v.general_description;
                    }
                    InventoryItem.Description = "";
                    if (v.description != null)
                    {
                        InventoryItem.Description = v.description;
                    }
                    InventoryItem.Terms = v.terms;
                    InventoryItem.Price = (decimal)v.price;
                    InventoryItem.TypeId = (int)v.inventoryitem_type_id;
                    InventoryItem.TypeName = v.kzInventoryItemType.name;
                    InventoryItem.Discount = (decimal)v.discount;
                    //var entityTransaction = from d in context.kzTransactionDetails
                    //                        where d.inventoryitem_id == v.id
                    //                        select d;
                    //if (entityTransaction.Count() > 0)
                    //{
                    //    InventoryItem.Sales = entityTransaction.Count();
                    //}
                    //else
                    //{
                    //    InventoryItem.Sales = 0;
                    //}
                    InventoryItem.Sales = (decimal)v.remainsales;
                    InventoryItem.MaximumSales = (decimal)v.maximumsales;
                    InventoryItem.SalesVisualMeter = (decimal)v.salesvisualmeter;
                    InventoryItem.Featured = (bool)v.featured;

                    List<String> currtag = new List<string>();
                    var entityTag = from d in context.kzInventoryItemTags
                                    from e in context.kzTags
                                    where d.tag_id == e.id && d.inventoryitem_id == v.id
                                    orderby d.tag_id ascending
                                    select new { d.tag_id, e.name };
                    foreach (var v2 in entityTag)
                    {
                        currtag.Add(v2.name);
                    }
                    InventoryItem.Tag = currtag;

                    List<VarianceDetail> listvariance = new List<VarianceDetail>();
                    var entityVarience = from d in context.kzVariances
                                         where d.inventoryitem_id == v.id
                                         select d;
                    foreach (var varien in entityVarience)
                    {
                        string _vari = varien.name + "`" + varien.price + "`" + varien.discount;
                        int count = context.kzTransactionDetails.Where(x => x.inventoryitem_id == v.id && x.variance.ToLower() == _vari.ToLower() && x.kzTransaction.last_action != "5").Count();
                        int alimit = (int)varien.available_limit - count;
                        if (alimit < 0) { alimit = 0; }
                        listvariance.Add(new VarianceDetail() { Variance = varien.name, Price = (decimal)varien.price, Discount = (decimal)varien.discount, AvailabelLimit = alimit });
                    }
                    InventoryItem.Variance = listvariance;

                    var entityImg = from d in context.kzInventoryItemImages
                                    from e in context.kzImages
                                    where d.image_id == e.id && d.inventoryitem_id == v.id
                                    && d.main == true
                                    select new { d.image_id, e.url };

                    if (entityImg.Count() > 0)
                    {
                        InventoryItem.ImageId = entityImg.First().image_id;
                        InventoryItem.ImageName = entityImg.First().url;
                        InventoryItem.ImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImg.First().url;
                    }
                    else
                    {
                        InventoryItem.ImageId = 0;
                        InventoryItem.ImageName = "";
                        InventoryItem.ImageUrl = "";
                    }

                    decimal currentvisualmeter = InventoryItem.SalesVisualMeter;

                    var entityPrize = v.kzPrize;
                    if (entityPrize != null)
                    {
                        decimal fakevisual = (decimal)entityPrize.fake_visualmeter;
                        if (currentvisualmeter < fakevisual)
                        {
                            InventoryItem.SalesVisualMeter = fakevisual;
                        }                       

                        Prize pr = new Prize();
                        pr.PrizeId = (int)entityPrize.id;
                        pr.Name = entityPrize.name;
                        pr.Price = (decimal)entityPrize.price;
                        pr.Description = entityPrize.description==null?string.Empty:entityPrize.description;
                        pr.SponsorName = entityPrize.sponsor_name == null ? string.Empty : entityPrize.sponsor_name;
                        pr.Terms = entityPrize.terms == null ? string.Empty : entityPrize.terms;
                        pr.Detail = entityPrize.detail == null ? string.Empty : entityPrize.detail;
                        pr.ExpiryDate = (DateTime)entityPrize.expiry_date;
                        pr.GroupName = entityPrize.group_name == null ? string.Empty : entityPrize.group_name;
                        var entityImgPrize = from d in context.kzPrizeImages
                                             from e in context.kzImages
                                             where d.image_id == e.id && d.prize_id == pr.PrizeId
                                             && d.main == true
                                             select new { d.image_id, e.url };

                        if (entityImgPrize.Count() > 0)
                        {
                            pr.ImageId = entityImgPrize.First().image_id;
                            pr.ImageName = entityImgPrize.First().url;
                            pr.ImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImgPrize.First().url;
                        }
                        else
                        {
                            pr.ImageId = 0;
                            pr.ImageName = "";
                            pr.ImageUrl = "";
                        }
                        pr.GameType = 0;
                        if (v.kzPrize.game_type != null)
                        {
                            pr.GameType = (int)v.kzPrize.game_type;
                        }
                        pr.FreeDeal = false;
                        if (v.kzPrize.freedeal != null)
                        {
                            pr.FreeDeal = (bool)v.kzPrize.freedeal;
                        }
                        InventoryItem.Prize = pr;
                    }
                    var entityFlashDeal = from d in context.kzFlashDeals.Include("kzInventoryItem")
                                          where d.inventoryitem_id == v.id && d.last_action!="5" && d.flag==true
                                          && today>= d.start_time && today <= d.end_time
                                          select d;
                    if (entityFlashDeal.Count() > 0)
                    {
                        FlashDeal FlashDeal = new FlashDeal();
                        FlashDeal.FlashDealId = entityFlashDeal.First().id;
                        FlashDeal.MerchantId = (int)entityFlashDeal.First().kzInventoryItem.merchant_id;
                        FlashDeal.MerchantName = entityFlashDeal.First().kzInventoryItem.kzMerchant.name;
                        FlashDeal.InventoryItemId = (int)entityFlashDeal.First().inventoryitem_id;
                        FlashDeal.InventoryItemName = entityFlashDeal.First().kzInventoryItem.name;
                        FlashDeal.Discount = (decimal)entityFlashDeal.First().discount;
                        FlashDeal.StartTime = (DateTime)entityFlashDeal.First().start_time;
                        FlashDeal.EndTime = (DateTime)entityFlashDeal.First().end_time;
                        FlashDeal.Flag = (Boolean)entityFlashDeal.First().flag;
                        FlashDeal.LastAction = entityFlashDeal.First().last_action;
                        InventoryItem.FlashDeal = FlashDeal;
                    }


                    InventoryItemList.Add(InventoryItem);
                }
                response = Response<List<InventoryItemShow>>.Create(InventoryItemList);
            }

            return response;
        }
        public Response<List<InventoryItemShow>> GetInventoryItemListByCityKuazooDeal(int CityId, DateTime today, string Category, int pagesize = 4, int page = 1)
        {
            Response<List<InventoryItemShow>> response = null;
            List<InventoryItemShow> InventoryItemList = new List<InventoryItemShow>();
            var user = new UserService().GetCurrentUserWithoutTrace().Result;
            using (var context = new entity.KuazooEntities())
            {
                var entityInventoryItem = from d in context.kzInventoryItems.Include("kzMerchant").Include("kzInventoryItemType")
                                          where d.kzPrize.last_action != "5" && d.kzPrize.expiry_date > today && today >= d.publish_date
                                          && d.flag == true && d.last_action != "5"
                                          && d.featured == true
                                          && d.draft == false
                                          select d;
                if (CityId != 0)
                {
                    entityInventoryItem = entityInventoryItem.Where(x => x.city_id == CityId);
                }
                if (Category != "")
                {
                    List<int> currtag = new List<int>();
                    String[] listCat = Category.Split(',');
                    var entityTag = from d in context.kzTags
                                    where listCat.Contains(d.name)
                                    select d;
                    foreach (var v2 in entityTag)
                    {
                        currtag.Add(v2.id);
                    }
                    var entityTag2 = from d in context.kzInventoryItemTags
                                     where currtag.Contains(d.tag_id)
                                     select d;
                    List<int> currInv = new List<int>();
                    foreach (var v in entityTag2)
                    {
                        currInv.Add(v.inventoryitem_id);
                    }
                    entityInventoryItem = entityInventoryItem.Where(x => currInv.Contains(x.id));
                }
                entityInventoryItem = entityInventoryItem.OrderBy(x=>x.featured_seq).ThenByDescending(x=>x.last_created);

                foreach (var v in entityInventoryItem)
                {
                    InventoryItemShow InventoryItem = new InventoryItemShow(); 
                    InventoryItem.Wishlist = false;
                    InventoryItem.FeatureSeq = (int)v.featured_seq;
                    InventoryItem.FeaturedText = "";
                    if ((InventoryItem.FeatureSeq ==1 || InventoryItem.FeatureSeq==4) && InventoryItem.FeaturedText != null)
                    {
                        InventoryItem.FeaturedText = v.featured_text;
                    }
                    if (user.UserId != 0)
                    {
                        if (v.kzWishLists.Where(x => x.user_id == user.UserId && x.last_action !="5").Count() > 0)
                        {
                            InventoryItem.Wishlist = true;
                        }
                    }
                    InventoryItem.InventoryItemId = v.id;
                    InventoryItem.Name = v.name;
                    InventoryItem.ShortDesc = v.short_desc;
                    InventoryItem.GeneralDescription = "";
                    if (v.general_description != null)
                    {
                        InventoryItem.GeneralDescription = v.general_description;
                    }
                    InventoryItem.Description = "";
                    if (v.description != null)
                    {
                        InventoryItem.Description = v.description;
                    }
                    InventoryItem.Terms = v.terms;
                    InventoryItem.Price = (decimal)v.price;
                    InventoryItem.TypeId = (int)v.inventoryitem_type_id;
                    InventoryItem.TypeName = v.kzInventoryItemType.name;
                    InventoryItem.Discount = (decimal)v.discount;
                    //var entityTransaction = from d in context.kzTransactionDetails
                    //                        where d.inventoryitem_id == v.id
                    //                        select d;
                    //if (entityTransaction.Count() > 0)
                    //{
                    //    InventoryItem.Sales = entityTransaction.Count();
                    //}
                    //else
                    //{
                    //    InventoryItem.Sales = 0;
                    //}
                    InventoryItem.Sales = (decimal)v.remainsales;
                    InventoryItem.MaximumSales = (decimal)v.maximumsales;
                    InventoryItem.SalesVisualMeter = (decimal)v.salesvisualmeter;
                    InventoryItem.Featured = (bool)v.featured;

                    List<String> currtag = new List<string>();
                    var entityTag = from d in context.kzInventoryItemTags
                                    from e in context.kzTags
                                    where d.tag_id == e.id && d.inventoryitem_id == v.id
                                    orderby d.tag_id ascending
                                    select new { d.tag_id, e.name };
                    foreach (var v2 in entityTag)
                    {
                        currtag.Add(v2.name);
                    }
                    InventoryItem.Tag = currtag;

                    List<VarianceDetail> listvariance = new List<VarianceDetail>();
                    var entityVarience = from d in context.kzVariances
                                         where d.inventoryitem_id == v.id
                                         select d;
                    foreach (var varien in entityVarience)
                    {
                        string _vari = varien.name + "`" + varien.price + "`" + varien.discount;
                        int count = context.kzTransactionDetails.Where(x => x.inventoryitem_id == v.id && x.variance.ToLower() == _vari.ToLower() && x.kzTransaction.last_action != "5").Count();
                        int alimit = (int)varien.available_limit - count;
                        if (alimit < 0) { alimit = 0; }
                        listvariance.Add(new VarianceDetail() { Variance = varien.name, Price = (decimal)varien.price, Discount = (decimal)varien.discount, AvailabelLimit=alimit  });
                    }
                    InventoryItem.Variance = listvariance;

                    var entityImg = from d in context.kzInventoryItemImages
                                    from e in context.kzImages
                                    where d.image_id == e.id && d.inventoryitem_id == v.id
                                    && d.main == true
                                    select new { d.image_id, e.url };

                    if (entityImg.Count() > 0)
                    {
                        InventoryItem.ImageId = entityImg.First().image_id;
                        InventoryItem.ImageName = entityImg.First().url;
                        InventoryItem.ImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImg.First().url;
                    }
                    else
                    {
                        InventoryItem.ImageId = 0;
                        InventoryItem.ImageName = "";
                        InventoryItem.ImageUrl = "";
                    }

                    decimal currentvisualmeter = InventoryItem.SalesVisualMeter;

                    var entityPrize = v.kzPrize;
                    if (entityPrize != null)
                    {
                        decimal fakevisual = (decimal)entityPrize.fake_visualmeter;
                        if (currentvisualmeter < fakevisual)
                        {
                            InventoryItem.SalesVisualMeter = fakevisual;
                        }   
                        Prize pr = new Prize();
                        pr.PrizeId = (int)entityPrize.id;
                        pr.Name = entityPrize.name;
                        pr.Price = (decimal)entityPrize.price;
                        pr.Description = entityPrize.description == null ? string.Empty : entityPrize.description;
                        pr.SponsorName = entityPrize.sponsor_name == null ? string.Empty : entityPrize.sponsor_name;
                        pr.Terms = entityPrize.terms == null ? string.Empty : entityPrize.terms;
                        pr.Detail = entityPrize.detail == null ? string.Empty : entityPrize.detail;
                        pr.ExpiryDate = (DateTime)entityPrize.expiry_date;
                        pr.GroupName = entityPrize.group_name == null ? string.Empty : entityPrize.group_name;
                        var entityImgPrize = from d in context.kzPrizeImages
                                             from e in context.kzImages
                                             where d.image_id == e.id && d.prize_id == pr.PrizeId
                                             && d.main == true
                                             select new { d.image_id, e.url };

                        if (entityImgPrize.Count() > 0)
                        {
                            pr.ImageId = entityImgPrize.First().image_id;
                            pr.ImageName = entityImgPrize.First().url;
                            pr.ImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImgPrize.First().url;
                        }
                        else
                        {
                            pr.ImageId = 0;
                            pr.ImageName = "";
                            pr.ImageUrl = "";
                        }

                        pr.GameType = 0;
                        if (v.kzPrize.game_type != null)
                        {
                            pr.GameType = (int)v.kzPrize.game_type;
                        }
                        pr.FreeDeal = false;
                        if (v.kzPrize.freedeal != null)
                        {
                            pr.FreeDeal = (bool)v.kzPrize.freedeal;
                        }
                        InventoryItem.Prize = pr;
                    }
                    var entityFlashDeal = from d in context.kzFlashDeals.Include("kzInventoryItem")
                                          where d.inventoryitem_id == v.id && d.last_action != "5" && d.flag == true
                                          && today >= d.start_time && today <= d.end_time
                                          select d;
                    if (entityFlashDeal.Count() > 0)
                    {
                        FlashDeal FlashDeal = new FlashDeal();
                        FlashDeal.FlashDealId = entityFlashDeal.First().id;
                        FlashDeal.MerchantId = (int)entityFlashDeal.First().kzInventoryItem.merchant_id;
                        FlashDeal.MerchantName = entityFlashDeal.First().kzInventoryItem.kzMerchant.name;
                        FlashDeal.InventoryItemId = (int)entityFlashDeal.First().inventoryitem_id;
                        FlashDeal.InventoryItemName = entityFlashDeal.First().kzInventoryItem.name;
                        FlashDeal.Discount = (decimal)entityFlashDeal.First().discount;
                        FlashDeal.StartTime = (DateTime)entityFlashDeal.First().start_time;
                        FlashDeal.EndTime = (DateTime)entityFlashDeal.First().end_time;
                        FlashDeal.Flag = (Boolean)entityFlashDeal.First().flag;
                        FlashDeal.LastAction = entityFlashDeal.First().last_action;
                        InventoryItem.FlashDeal = FlashDeal;
                    }
                    InventoryItemList.Add(InventoryItem);
                }
                response = Response<List<InventoryItemShow>>.Create(InventoryItemList);
            }

            return response;
        }
        public Response<List<InventoryItemShow>> GetInventoryItemListByCityRaffle(int CityId, DateTime today, string Category, int pagesize = 4, int page = 1)
        {
            Response<List<InventoryItemShow>> response = null;
            List<InventoryItemShow> InventoryItemList = new List<InventoryItemShow>();
            var user = new UserService().GetCurrentUserWithoutTrace().Result;
            using (var context = new entity.KuazooEntities())
            {
                var entityInventoryItem = from d in context.kzInventoryItems.Include("kzMerchant").Include("kzInventoryItemType")
                                          where d.kzPrize.last_action != "5" && d.kzPrize.expiry_date > today && today >= d.publish_date
                                          && d.flag == true && d.last_action != "5"
                                          && d.kzInventoryItemType.name.ToLower().Equals("raffle")
                                          && d.draft == false
                                          select d;
                if (CityId != 0)
                {
                    entityInventoryItem = entityInventoryItem.Where(x => x.city_id == CityId);
                }
                if (Category != "")
                {
                    List<int> currtag = new List<int>();
                    String[] listCat = Category.Split(',');
                    var entityTag = from d in context.kzTags
                                    where listCat.Contains(d.name)
                                    select d;
                    foreach (var v2 in entityTag)
                    {
                        currtag.Add(v2.id);
                    }
                    var entityTag2 = from d in context.kzInventoryItemTags
                                     where currtag.Contains(d.tag_id)
                                     select d;
                    List<int> currInv = new List<int>();
                    foreach (var v in entityTag2)
                    {
                        currInv.Add(v.inventoryitem_id);
                    }
                    entityInventoryItem = entityInventoryItem.Where(x => currInv.Contains(x.id));
                }
                entityInventoryItem = entityInventoryItem.OrderByDescending(x => x.publish_date);

                foreach (var v in entityInventoryItem)
                {
                    InventoryItemShow InventoryItem = new InventoryItemShow();
                    InventoryItem.Wishlist = false;
                    if (user.UserId != 0)
                    {
                        if (v.kzWishLists.Where(x => x.user_id == user.UserId && x.last_action !="5").Count() > 0)
                        {
                            InventoryItem.Wishlist = true;
                        }
                    }
                    InventoryItem.InventoryItemId = v.id;
                    InventoryItem.Name = v.name;
                    InventoryItem.ShortDesc = v.short_desc;
                    InventoryItem.GeneralDescription = "";
                    if (v.general_description != null)
                    {
                        InventoryItem.GeneralDescription = v.general_description;
                    }
                    InventoryItem.Description = "";
                    if (v.description != null)
                    {
                        InventoryItem.Description = v.description;
                    }
                    InventoryItem.Terms = v.terms;
                    InventoryItem.Price = (decimal)v.price;
                    InventoryItem.TypeId = (int)v.inventoryitem_type_id;
                    InventoryItem.TypeName = v.kzInventoryItemType.name;
                    InventoryItem.Discount = (decimal)v.discount;
                    //var entityTransaction = from d in context.kzTransactionDetails
                    //                        where d.inventoryitem_id == v.id
                    //                        select d;
                    //if (entityTransaction.Count() > 0)
                    //{
                    //    InventoryItem.Sales = entityTransaction.Count();
                    //}
                    //else
                    //{
                    //    InventoryItem.Sales = 0;
                    //}
                    InventoryItem.Sales = (decimal)v.remainsales;
                    InventoryItem.MaximumSales = (decimal)v.maximumsales;
                    InventoryItem.SalesVisualMeter = (decimal)v.salesvisualmeter;
                    InventoryItem.Featured = (bool)v.featured;

                    List<String> currtag = new List<string>();
                    var entityTag = from d in context.kzInventoryItemTags
                                    from e in context.kzTags
                                    where d.tag_id == e.id && d.inventoryitem_id == v.id
                                    orderby d.tag_id ascending
                                    select new { d.tag_id, e.name };
                    foreach (var v2 in entityTag)
                    {
                        currtag.Add(v2.name);
                    }
                    InventoryItem.Tag = currtag;

                    List<VarianceDetail> listvariance = new List<VarianceDetail>();
                    var entityVarience = from d in context.kzVariances
                                         where d.inventoryitem_id == v.id
                                         select d;
                    foreach (var varien in entityVarience)
                    {
                        listvariance.Add(new VarianceDetail() { Variance = varien.name, Price = (decimal)varien.price, Discount = (decimal)varien.discount });
                    }
                    InventoryItem.Variance = listvariance;

                    var entityImg = from d in context.kzInventoryItemImages
                                    from e in context.kzImages
                                    where d.image_id == e.id && d.inventoryitem_id == v.id
                                    && d.main == true
                                    select new { d.image_id, e.url };

                    if (entityImg.Count() > 0)
                    {
                        InventoryItem.ImageId = entityImg.First().image_id;
                        InventoryItem.ImageName = entityImg.First().url;
                        InventoryItem.ImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImg.First().url;
                    }
                    else
                    {
                        InventoryItem.ImageId = 0;
                        InventoryItem.ImageName = "";
                        InventoryItem.ImageUrl = "";
                    }

                    decimal currentvisualmeter = InventoryItem.SalesVisualMeter;

                    var entityPrize = v.kzPrize;
                    if (entityPrize != null)
                    {
                        decimal fakevisual = (decimal)entityPrize.fake_visualmeter;
                        if (currentvisualmeter < fakevisual)
                        {
                            InventoryItem.SalesVisualMeter = fakevisual;
                        }   
                        Prize pr = new Prize();
                        pr.PrizeId = (int)entityPrize.id;
                        pr.Name = entityPrize.name;
                        pr.Price = (decimal)entityPrize.price;
                        pr.Description = entityPrize.description == null ? string.Empty : entityPrize.description;
                        pr.SponsorName = entityPrize.sponsor_name == null ? string.Empty : entityPrize.sponsor_name;
                        pr.Terms = entityPrize.terms == null ? string.Empty : entityPrize.terms;
                        pr.Detail = entityPrize.detail == null ? string.Empty : entityPrize.detail;
                        pr.ExpiryDate = (DateTime)entityPrize.expiry_date;
                        pr.GroupName = entityPrize.group_name == null ? string.Empty : entityPrize.group_name;
                        var entityImgPrize = from d in context.kzPrizeImages
                                             from e in context.kzImages
                                             where d.image_id == e.id && d.prize_id == pr.PrizeId
                                             && d.main == true
                                             select new { d.image_id, e.url };

                        if (entityImgPrize.Count() > 0)
                        {
                            pr.ImageId = entityImgPrize.First().image_id;
                            pr.ImageName = entityImgPrize.First().url;
                            pr.ImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImgPrize.First().url;
                        }
                        else
                        {
                            pr.ImageId = 0;
                            pr.ImageName = "";
                            pr.ImageUrl = "";
                        }

                        pr.GameType = 0;
                        if (v.kzPrize.game_type != null)
                        {
                            pr.GameType = (int)v.kzPrize.game_type;
                        }
                        pr.FreeDeal = false;
                        if (v.kzPrize.freedeal != null)
                        {
                            pr.FreeDeal = (bool)v.kzPrize.freedeal;
                        }
                        InventoryItem.Prize = pr;
                    }
                    var entityFlashDeal = from d in context.kzFlashDeals.Include("kzInventoryItem")
                                          where d.inventoryitem_id == v.id && d.last_action != "5" && d.flag == true
                                          && today >= d.start_time && today <= d.end_time
                                          select d;
                    if (entityFlashDeal.Count() > 0)
                    {
                        FlashDeal FlashDeal = new FlashDeal();
                        FlashDeal.FlashDealId = entityFlashDeal.First().id;
                        FlashDeal.MerchantId = (int)entityFlashDeal.First().kzInventoryItem.merchant_id;
                        FlashDeal.MerchantName = entityFlashDeal.First().kzInventoryItem.kzMerchant.name;
                        FlashDeal.InventoryItemId = (int)entityFlashDeal.First().inventoryitem_id;
                        FlashDeal.InventoryItemName = entityFlashDeal.First().kzInventoryItem.name;
                        FlashDeal.Discount = (decimal)entityFlashDeal.First().discount;
                        FlashDeal.StartTime = (DateTime)entityFlashDeal.First().start_time;
                        FlashDeal.EndTime = (DateTime)entityFlashDeal.First().end_time;
                        FlashDeal.Flag = (Boolean)entityFlashDeal.First().flag;
                        FlashDeal.LastAction = entityFlashDeal.First().last_action;
                        InventoryItem.FlashDeal = FlashDeal;
                    }
                    InventoryItemList.Add(InventoryItem);
                }
                response = Response<List<InventoryItemShow>>.Create(InventoryItemList);
            }

            return response;
        }
        public Response<List<InventoryItemShow>> GetInventoryItemListByCityFlashDeal(int CityId, DateTime today, string Category, int pagesize = 4, int page = 1)
        {
            Response<List<InventoryItemShow>> response = null;
            List<InventoryItemShow> InventoryItemList = new List<InventoryItemShow>();
            var user = new UserService().GetCurrentUserWithoutTrace().Result;
            using (var context = new entity.KuazooEntities())
            {
                var entityInventoryItem = from d in context.kzInventoryItems.Include("kzMerchant").Include("kzInventoryItemType")
                                          where d.kzPrize.last_action != "5" && d.kzPrize.expiry_date > today && today >= d.publish_date
                                          && d.flag == true && d.last_action != "5"
                                            && d.kzFlashDeals.Where(x => x.last_action != "5" &&
                                                x.flag == true && today >= x.start_time && today <= x.end_time).Count() > 0
                                          && d.draft == false
                                          select d;
                if (CityId != 0)
                {
                    entityInventoryItem = entityInventoryItem.Where(x => x.city_id == CityId);
                }
                if (Category != "")
                {
                    List<int> currtag = new List<int>();
                    String[] listCat = Category.Split(',');
                    var entityTag = from d in context.kzTags
                                    where listCat.Contains(d.name)
                                    select d;
                    foreach (var v2 in entityTag)
                    {
                        currtag.Add(v2.id);
                    }
                    var entityTag2 = from d in context.kzInventoryItemTags
                                     where currtag.Contains(d.tag_id)
                                     select d;
                    List<int> currInv = new List<int>();
                    foreach (var v in entityTag2)
                    {
                        currInv.Add(v.inventoryitem_id);
                    }
                    entityInventoryItem = entityInventoryItem.Where(x => currInv.Contains(x.id));
                }
                entityInventoryItem = entityInventoryItem.OrderBy(x => x.priority).ThenByDescending(x => x.publish_date);

                foreach (var v in entityInventoryItem)
                {
                    InventoryItemShow InventoryItem = new InventoryItemShow();
                    InventoryItem.Wishlist = false;
                    InventoryItem.Featured = (bool)v.featured;
                    InventoryItem.Description = "";
                    if (v.description != null)
                    {
                        InventoryItem.Description = v.description;
                    }
                    if (user.UserId != 0)
                    {
                        if (v.kzWishLists.Where(x => x.user_id == user.UserId && x.last_action !="5").Count() > 0)
                        {
                            InventoryItem.Wishlist = true;
                        }
                    }

                    var entityFlashDeal = from d in context.kzFlashDeals.Include("kzInventoryItem")
                                          where d.inventoryitem_id == v.id && d.last_action != "5" && d.flag == true
                                          && today >= d.start_time && today <= d.end_time
                                          select d;
                    if (entityFlashDeal.Count() > 0)
                    {
                        FlashDeal FlashDeal = new FlashDeal();
                        FlashDeal.FlashDealId = entityFlashDeal.First().id;
                        FlashDeal.MerchantId = (int)entityFlashDeal.First().kzInventoryItem.merchant_id;
                        FlashDeal.MerchantName = entityFlashDeal.First().kzInventoryItem.kzMerchant.name;
                        FlashDeal.InventoryItemId = (int)entityFlashDeal.First().inventoryitem_id;
                        FlashDeal.InventoryItemName = entityFlashDeal.First().kzInventoryItem.name;
                        FlashDeal.Discount = (decimal)entityFlashDeal.First().discount;
                        FlashDeal.StartTime = (DateTime)entityFlashDeal.First().start_time;
                        FlashDeal.EndTime = (DateTime)entityFlashDeal.First().end_time;
                        FlashDeal.Flag = (Boolean)entityFlashDeal.First().flag;
                        FlashDeal.LastAction = entityFlashDeal.First().last_action;
                        InventoryItem.FlashDeal = FlashDeal;

                        InventoryItem.InventoryItemId = v.id;
                        InventoryItem.Name = v.name;
                        InventoryItem.ShortDesc = v.short_desc;
                        InventoryItem.GeneralDescription = "";
                        if (v.general_description != null)
                        {
                            InventoryItem.GeneralDescription = v.general_description;
                        }
                        InventoryItem.Terms = v.terms;
                        InventoryItem.Price = (decimal)v.price;
                        InventoryItem.TypeId = (int)v.inventoryitem_type_id;
                        InventoryItem.TypeName = v.kzInventoryItemType.name;
                        InventoryItem.Discount = (decimal)v.discount;
                        //var entityTransaction = from d in context.kzTransactionDetails
                        //                        where d.inventoryitem_id == v.id
                        //                        select d;
                        //if (entityTransaction.Count() > 0)
                        //{
                        //    InventoryItem.Sales = entityTransaction.Count();
                        //}
                        //else
                        //{
                        //    InventoryItem.Sales = 0;
                        //}
                        InventoryItem.Sales = (decimal)v.remainsales;
                        InventoryItem.MaximumSales = (decimal)v.maximumsales;
                        InventoryItem.SalesVisualMeter = (decimal)v.salesvisualmeter;

                        List<String> currtag = new List<string>();
                        var entityTag = from d in context.kzInventoryItemTags
                                        from e in context.kzTags
                                        where d.tag_id == e.id && d.inventoryitem_id == v.id
                                        orderby d.tag_id ascending
                                        select new { d.tag_id, e.name };
                        foreach (var v2 in entityTag)
                        {
                            currtag.Add(v2.name);
                        }
                        InventoryItem.Tag = currtag;

                        List<VarianceDetail> listvariance = new List<VarianceDetail>();
                        var entityVarience = from d in context.kzVariances
                                             where d.inventoryitem_id == v.id
                                             select d;
                        foreach (var varien in entityVarience)
                        {
                            listvariance.Add(new VarianceDetail() { Variance = varien.name, Price = (decimal)varien.price, Discount = (decimal)varien.discount });
                        }
                        InventoryItem.Variance = listvariance;

                        var entityImg = from d in context.kzInventoryItemImages
                                        from e in context.kzImages
                                        where d.image_id == e.id && d.inventoryitem_id == v.id
                                        && d.main == true
                                        select new { d.image_id, e.url };

                        if (entityImg.Count() > 0)
                        {
                            InventoryItem.ImageId = entityImg.First().image_id;
                            InventoryItem.ImageName = entityImg.First().url;
                            InventoryItem.ImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImg.First().url;
                        }
                        else
                        {
                            InventoryItem.ImageId = 0;
                            InventoryItem.ImageName = "";
                            InventoryItem.ImageUrl = "";
                        }
                        decimal currentvisualmeter = InventoryItem.SalesVisualMeter;

                        var entityPrize = v.kzPrize;
                        if (entityPrize != null)
                        {
                            decimal fakevisual = (decimal)entityPrize.fake_visualmeter;
                            if (currentvisualmeter < fakevisual)
                            {
                                InventoryItem.SalesVisualMeter = fakevisual;
                            }   
                            Prize pr = new Prize();
                            pr.PrizeId = (int)entityPrize.id;
                            pr.Name = entityPrize.name;
                            pr.Price = (decimal)entityPrize.price;
                            pr.Description = entityPrize.description == null ? string.Empty : entityPrize.description;
                            pr.SponsorName = entityPrize.sponsor_name == null ? string.Empty : entityPrize.sponsor_name;
                            pr.Terms = entityPrize.terms == null ? string.Empty : entityPrize.terms;
                            pr.Detail = entityPrize.detail == null ? string.Empty : entityPrize.detail;
                            pr.ExpiryDate = (DateTime)entityPrize.expiry_date;
                            pr.GroupName = entityPrize.group_name == null ? string.Empty : entityPrize.group_name;
                            var entityImgPrize = from d in context.kzPrizeImages
                                                 from e in context.kzImages
                                                 where d.image_id == e.id && d.prize_id == pr.PrizeId
                                                 && d.main == true
                                                 select new { d.image_id, e.url };

                            if (entityImgPrize.Count() > 0)
                            {
                                pr.ImageId = entityImgPrize.First().image_id;
                                pr.ImageName = entityImgPrize.First().url;
                                pr.ImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImgPrize.First().url;
                            }
                            else
                            {
                                pr.ImageId = 0;
                                pr.ImageName = "";
                                pr.ImageUrl = "";
                            }

                            pr.GameType = 0;
                            if (v.kzPrize.game_type != null)
                            {
                                pr.GameType = (int)v.kzPrize.game_type;
                            }
                            pr.FreeDeal = false;
                            if (v.kzPrize.freedeal != null)
                            {
                                pr.FreeDeal = (bool)v.kzPrize.freedeal;
                            }
                            InventoryItem.Prize = pr;
                        }
                        InventoryItemList.Add(InventoryItem);
                    }
                }
                response = Response<List<InventoryItemShow>>.Create(InventoryItemList);
            }

            return response;
        }


        public Response<List<InventoryItemShow>> GetInventoryItemListByFlashDealTake(DateTime today, int take)
        {
            Response<List<InventoryItemShow>> response = null;
            List<InventoryItemShow> InventoryItemList = new List<InventoryItemShow>();
            using (var context = new entity.KuazooEntities())
            {
                var entityInventoryItem = from d in context.kzInventoryItems.Include("kzMerchant").Include("kzInventoryItemType")
                                          where d.kzPrize.last_action != "5" && d.kzPrize.expiry_date > today && today >= d.publish_date
                                          && d.flag == true && d.last_action != "5"
                                          select d;
                entityInventoryItem = entityInventoryItem.OrderByDescending(x => x.publish_date);
                int i = 0;
                foreach (var v in entityInventoryItem)
                {
                    InventoryItemShow InventoryItem = new InventoryItemShow();
                    InventoryItem.Featured = (bool)v.featured;
                    if (i >= take) break;
                    var entityFlashDeal = from d in context.kzFlashDeals.Include("kzInventoryItem")
                                          where d.inventoryitem_id == v.id && d.last_action != "5" && d.flag == true
                                          && today >= d.start_time && today <= d.end_time
                                          && d.kzInventoryItem.draft == false
                                          select d;
                    if (entityFlashDeal.Count() > 0)
                    {
                        FlashDeal FlashDeal = new FlashDeal();
                        FlashDeal.FlashDealId = entityFlashDeal.First().id;
                        FlashDeal.MerchantId = (int)entityFlashDeal.First().kzInventoryItem.merchant_id;
                        FlashDeal.MerchantName = entityFlashDeal.First().kzInventoryItem.kzMerchant.name;
                        FlashDeal.InventoryItemId = (int)entityFlashDeal.First().inventoryitem_id;
                        FlashDeal.InventoryItemName = entityFlashDeal.First().kzInventoryItem.name;
                        FlashDeal.Discount = (decimal)entityFlashDeal.First().discount;
                        FlashDeal.StartTime = (DateTime)entityFlashDeal.First().start_time;
                        FlashDeal.EndTime = (DateTime)entityFlashDeal.First().end_time;
                        FlashDeal.Flag = (Boolean)entityFlashDeal.First().flag;
                        FlashDeal.LastAction = entityFlashDeal.First().last_action;
                        InventoryItem.FlashDeal = FlashDeal;

                        InventoryItem.InventoryItemId = v.id;
                        InventoryItem.Name = v.name;
                        InventoryItem.ShortDesc = v.short_desc;
                        InventoryItem.GeneralDescription = "";
                        if (v.general_description != null)
                        {
                            InventoryItem.GeneralDescription = v.general_description;
                        }
                        InventoryItem.Terms = v.terms;
                        InventoryItem.Price = (decimal)v.price;
                        InventoryItem.TypeId = (int)v.inventoryitem_type_id;
                        InventoryItem.TypeName = v.kzInventoryItemType.name;
                        InventoryItem.Discount = (decimal)v.discount;
                        //var entityTransaction = from d in context.kzTransactionDetails
                        //                        where d.inventoryitem_id == v.id
                        //                        select d;
                        //if (entityTransaction.Count() > 0)
                        //{
                        //    InventoryItem.Sales = entityTransaction.Count();
                        //}
                        //else
                        //{
                        //    InventoryItem.Sales = 0;
                        //}
                        InventoryItem.Sales = (decimal)v.remainsales;
                        InventoryItem.MaximumSales = (decimal)v.maximumsales;
                        InventoryItem.SalesVisualMeter = (decimal)v.salesvisualmeter;

                        List<VarianceDetail> listvariance = new List<VarianceDetail>();
                        var entityVarience = from d in context.kzVariances
                                             where d.inventoryitem_id == v.id
                                             select d;
                        foreach (var varien in entityVarience)
                        {
                            listvariance.Add(new VarianceDetail() { Variance = varien.name, Price = (decimal)varien.price, Discount = (decimal)varien.discount });
                        }
                        InventoryItem.Variance = listvariance;

                        var entityImg = from d in context.kzInventoryItemImages
                                        from e in context.kzImages
                                        where d.image_id == e.id && d.inventoryitem_id == v.id
                                        && d.main == true
                                        select new { d.image_id, e.url };

                        if (entityImg.Count() > 0)
                        {
                            InventoryItem.ImageId = entityImg.First().image_id;
                            InventoryItem.ImageName = entityImg.First().url;
                            InventoryItem.ImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImg.First().url;
                        }
                        else
                        {
                            InventoryItem.ImageId = 0;
                            InventoryItem.ImageName = "";
                            InventoryItem.ImageUrl = "";
                        }
                        decimal currentvisualmeter = InventoryItem.SalesVisualMeter;

                        var entityPrize = v.kzPrize;
                        if (entityPrize != null)
                        {
                            decimal fakevisual = (decimal)entityPrize.fake_visualmeter;
                            if (currentvisualmeter < fakevisual)
                            {
                                InventoryItem.SalesVisualMeter = fakevisual;
                            }   
                            Prize pr = new Prize();
                            pr.PrizeId = (int)entityPrize.id;
                            pr.Name = entityPrize.name;
                            pr.Price = (decimal)entityPrize.price;
                            pr.Description = entityPrize.description == null ? string.Empty : entityPrize.description;
                            pr.SponsorName = entityPrize.sponsor_name == null ? string.Empty : entityPrize.sponsor_name;
                            pr.Terms = entityPrize.terms == null ? string.Empty : entityPrize.terms;
                            pr.Detail = entityPrize.detail == null ? string.Empty : entityPrize.detail;
                            pr.ExpiryDate = (DateTime)entityPrize.expiry_date;
                            pr.GroupName = entityPrize.group_name == null ? string.Empty : entityPrize.group_name;
                            var entityImgPrize = from d in context.kzPrizeImages
                                                 from e in context.kzImages
                                                 where d.image_id == e.id && d.prize_id == pr.PrizeId
                                                 && d.main == true
                                                 select new { d.image_id, e.url };

                            if (entityImgPrize.Count() > 0)
                            {
                                pr.ImageId = entityImgPrize.First().image_id;
                                pr.ImageName = entityImgPrize.First().url;
                                pr.ImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImgPrize.First().url;
                            }
                            else
                            {
                                pr.ImageId = 0;
                                pr.ImageName = "";
                                pr.ImageUrl = "";
                            }

                            pr.GameType = 0;
                            if (v.kzPrize.game_type != null)
                            {
                                pr.GameType = (int)v.kzPrize.game_type;
                            }
                            pr.FreeDeal = false;
                            if (v.kzPrize.freedeal != null)
                            {
                                pr.FreeDeal = (bool)v.kzPrize.freedeal;
                            }
                            InventoryItem.Prize = pr;
                        }
                        InventoryItemList.Add(InventoryItem);
                        i++;
                    }
                }
                response = Response<List<InventoryItemShow>>.Create(InventoryItemList);
            }

            return response;
        }
        public Response<List<InventoryItemShow>> GetInventoryItemListTake(DateTime today,int take)
        {
            Response<List<InventoryItemShow>> response = null;
            List<InventoryItemShow> InventoryItemList = new List<InventoryItemShow>();
            using (var context = new entity.KuazooEntities())
            {
                var entityInventoryItem = from d in context.kzInventoryItems.Include("kzMerchant").Include("kzInventoryItemType")
                                          where d.kzPrize.last_action != "5" && d.kzPrize.expiry_date > today && today >= d.publish_date
                                          && d.flag == true && d.last_action != "5" && d.salesvisualmeter < 100
                                          && d.draft == false
                                          select d;
                entityInventoryItem = entityInventoryItem.OrderByDescending(x => x.publish_date);
                foreach (var v in entityInventoryItem.Take(take))
                {
                    InventoryItemShow InventoryItem = new InventoryItemShow();
                    InventoryItem.InventoryItemId = v.id;
                    InventoryItem.Name = v.name;
                    InventoryItem.ShortDesc = v.short_desc;
                    InventoryItem.GeneralDescription = "";
                    if (v.general_description != null)
                    {
                        InventoryItem.GeneralDescription = v.general_description;
                    }
                    InventoryItem.Terms = v.terms;
                    InventoryItem.Price = (decimal)v.price;
                    InventoryItem.TypeId = (int)v.inventoryitem_type_id;
                    InventoryItem.TypeName = v.kzInventoryItemType.name;
                    InventoryItem.Discount = (decimal)v.discount;
                    //var entityTransaction = from d in context.kzTransactionDetails
                    //                        where d.inventoryitem_id == v.id
                    //                        select d;
                    //if (entityTransaction.Count() > 0)
                    //{
                    //    InventoryItem.Sales = entityTransaction.Count();
                    //}
                    //else
                    //{
                    //    InventoryItem.Sales = 0;
                    //}
                    InventoryItem.Sales = (decimal)v.remainsales;
                    InventoryItem.MaximumSales = (decimal)v.maximumsales;
                    InventoryItem.SalesVisualMeter = (decimal)v.salesvisualmeter;
                    InventoryItem.Featured = (bool)v.featured;


                    List<VarianceDetail> listvariance = new List<VarianceDetail>();
                    var entityVarience = from d in context.kzVariances
                                         where d.inventoryitem_id == v.id
                                         select d;
                    foreach (var varien in entityVarience)
                    {
                        listvariance.Add(new VarianceDetail() { Variance = varien.name, Price = (decimal)varien.price, Discount = (decimal)varien.discount });
                    }
                    InventoryItem.Variance = listvariance;

                    var entityImg = from d in context.kzInventoryItemImages
                                    from e in context.kzImages
                                    where d.image_id == e.id && d.inventoryitem_id == v.id
                                    && d.main == true
                                    select new { d.image_id, e.url };

                    if (entityImg.Count() > 0)
                    {
                        InventoryItem.ImageId = entityImg.First().image_id;
                        InventoryItem.ImageName = entityImg.First().url;
                        InventoryItem.ImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImg.First().url;
                    }
                    else
                    {
                        InventoryItem.ImageId = 0;
                        InventoryItem.ImageName = "";
                        InventoryItem.ImageUrl = "";
                    }
                    decimal currentvisualmeter = InventoryItem.SalesVisualMeter;

                    var entityPrize = v.kzPrize;
                    if (entityPrize != null)
                    {
                        decimal fakevisual = (decimal)entityPrize.fake_visualmeter;
                        if (currentvisualmeter < fakevisual)
                        {
                            InventoryItem.SalesVisualMeter = fakevisual;
                        }   
                        Prize pr = new Prize();
                        pr.PrizeId = (int)entityPrize.id;
                        pr.Name = entityPrize.name;
                        pr.Price = (decimal)entityPrize.price;
                        pr.Description = entityPrize.description == null ? string.Empty : entityPrize.description;
                        pr.SponsorName = entityPrize.sponsor_name == null ? string.Empty : entityPrize.sponsor_name;
                        pr.Terms = entityPrize.terms == null ? string.Empty : entityPrize.terms;
                        pr.Detail = entityPrize.detail == null ? string.Empty : entityPrize.detail;
                        pr.ExpiryDate = (DateTime)entityPrize.expiry_date;
                        pr.GroupName = entityPrize.group_name == null ? string.Empty : entityPrize.group_name;
                        var entityImgPrize = from d in context.kzPrizeImages
                                             from e in context.kzImages
                                             where d.image_id == e.id && d.prize_id == pr.PrizeId
                                             && d.main == true
                                             select new { d.image_id, e.url };

                        if (entityImgPrize.Count() > 0)
                        {
                            pr.ImageId = entityImgPrize.First().image_id;
                            pr.ImageName = entityImgPrize.First().url;
                            pr.ImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImgPrize.First().url;
                        }
                        else
                        {
                            pr.ImageId = 0;
                            pr.ImageName = "";
                            pr.ImageUrl = "";
                        }

                        pr.GameType = 0;
                        if (v.kzPrize.game_type != null)
                        {
                            pr.GameType = (int)v.kzPrize.game_type;
                        }
                        pr.FreeDeal = false;
                        if (v.kzPrize.freedeal != null)
                        {
                            pr.FreeDeal = (bool)v.kzPrize.freedeal;
                        }
                        InventoryItem.Prize = pr;
                    }
                    var entityFlashDeal = from d in context.kzFlashDeals.Include("kzInventoryItem")
                                          where d.inventoryitem_id == v.id && d.last_action != "5" && d.flag == true
                                          && today >= d.start_time && today <= d.end_time
                                          select d;
                    if (entityFlashDeal.Count() > 0)
                    {
                        FlashDeal FlashDeal = new FlashDeal();
                        FlashDeal.FlashDealId = entityFlashDeal.First().id;
                        FlashDeal.MerchantId = (int)entityFlashDeal.First().kzInventoryItem.merchant_id;
                        FlashDeal.MerchantName = entityFlashDeal.First().kzInventoryItem.kzMerchant.name;
                        FlashDeal.InventoryItemId = (int)entityFlashDeal.First().inventoryitem_id;
                        FlashDeal.InventoryItemName = entityFlashDeal.First().kzInventoryItem.name;
                        FlashDeal.Discount = (decimal)entityFlashDeal.First().discount;
                        FlashDeal.StartTime = (DateTime)entityFlashDeal.First().start_time;
                        FlashDeal.EndTime = (DateTime)entityFlashDeal.First().end_time;
                        FlashDeal.Flag = (Boolean)entityFlashDeal.First().flag;
                        FlashDeal.LastAction = entityFlashDeal.First().last_action;
                        InventoryItem.FlashDeal = FlashDeal;
                    }
                    InventoryItemList.Add(InventoryItem);
                }
                response = Response<List<InventoryItemShow>>.Create(InventoryItemList);
            }

            return response;
        }
        public Response<List<InventoryItemShow>> GetInventoryItemListTakeV2(DateTime today, int take)
        {
            Response<List<InventoryItemShow>> response = null;
            List<InventoryItemShow> InventoryItemList = new List<InventoryItemShow>();
            using (var context = new entity.KuazooEntities())
            {
                var entityInventoryItem = from d in context.kzInventoryItems.Include("kzMerchant").Include("kzInventoryItemType")
                                          where d.kzPrize.last_action != "5" && d.kzPrize.expiry_date > today && today >= d.publish_date
                                          && d.flag == true && d.last_action != "5" && d.salesvisualmeter < 100
                                          && d.kzFlashDeals.Count() == 0
                                          && d.draft == false
                                          select d;
                entityInventoryItem = entityInventoryItem.OrderByDescending(x => x.publish_date);
                foreach (var v in entityInventoryItem.Take(take))
                {
                    InventoryItemShow InventoryItem = new InventoryItemShow();
                    InventoryItem.InventoryItemId = v.id;
                    InventoryItem.Name = v.name;
                    InventoryItem.ShortDesc = v.short_desc;
                    InventoryItem.GeneralDescription = "";
                    if (v.general_description != null)
                    {
                        InventoryItem.GeneralDescription = v.general_description;
                    }
                    InventoryItem.Terms = v.terms;
                    InventoryItem.Price = (decimal)v.price;
                    InventoryItem.TypeId = (int)v.inventoryitem_type_id;
                    InventoryItem.TypeName = v.kzInventoryItemType.name;
                    InventoryItem.Discount = (decimal)v.discount;
                    //var entityTransaction = from d in context.kzTransactionDetails
                    //                        where d.inventoryitem_id == v.id
                    //                        select d;
                    //if (entityTransaction.Count() > 0)
                    //{
                    //    InventoryItem.Sales = entityTransaction.Count();
                    //}
                    //else
                    //{
                    //    InventoryItem.Sales = 0;
                    //}
                    InventoryItem.Sales = (decimal)v.remainsales;
                    InventoryItem.MaximumSales = (decimal)v.maximumsales;
                    InventoryItem.SalesVisualMeter = (decimal)v.salesvisualmeter;
                    InventoryItem.Featured = (bool)v.featured;


                    List<VarianceDetail> listvariance = new List<VarianceDetail>();
                    var entityVarience = from d in context.kzVariances
                                         where d.inventoryitem_id == v.id
                                         select d;
                    foreach (var varien in entityVarience)
                    {
                        listvariance.Add(new VarianceDetail() { Variance = varien.name, Price = (decimal)varien.price, Discount = (decimal)varien.discount });
                    }
                    InventoryItem.Variance = listvariance;

                    var entityImg = from d in context.kzInventoryItemImages
                                    from e in context.kzImages
                                    where d.image_id == e.id && d.inventoryitem_id == v.id
                                    && d.main == true
                                    select new { d.image_id, e.url };

                    if (entityImg.Count() > 0)
                    {
                        InventoryItem.ImageId = entityImg.First().image_id;
                        InventoryItem.ImageName = entityImg.First().url;
                        InventoryItem.ImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImg.First().url;
                    }
                    else
                    {
                        InventoryItem.ImageId = 0;
                        InventoryItem.ImageName = "";
                        InventoryItem.ImageUrl = "";
                    }
                    decimal currentvisualmeter = InventoryItem.SalesVisualMeter;

                    var entityPrize = v.kzPrize;
                    if (entityPrize != null)
                    {
                        decimal fakevisual = (decimal)entityPrize.fake_visualmeter;
                        if (currentvisualmeter < fakevisual)
                        {
                            InventoryItem.SalesVisualMeter = fakevisual;
                        }
                        Prize pr = new Prize();
                        pr.PrizeId = (int)entityPrize.id;
                        pr.Name = entityPrize.name;
                        pr.Price = (decimal)entityPrize.price;
                        pr.Description = entityPrize.description == null ? string.Empty : entityPrize.description;
                        pr.SponsorName = entityPrize.sponsor_name == null ? string.Empty : entityPrize.sponsor_name;
                        pr.Terms = entityPrize.terms == null ? string.Empty : entityPrize.terms;
                        pr.Detail = entityPrize.detail == null ? string.Empty : entityPrize.detail;
                        pr.ExpiryDate = (DateTime)entityPrize.expiry_date;
                        pr.GroupName = entityPrize.group_name == null ? string.Empty : entityPrize.group_name;
                        var entityImgPrize = from d in context.kzPrizeImages
                                             from e in context.kzImages
                                             where d.image_id == e.id && d.prize_id == pr.PrizeId
                                             && d.main == true
                                             select new { d.image_id, e.url };

                        if (entityImgPrize.Count() > 0)
                        {
                            pr.ImageId = entityImgPrize.First().image_id;
                            pr.ImageName = entityImgPrize.First().url;
                            pr.ImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImgPrize.First().url;
                        }
                        else
                        {
                            pr.ImageId = 0;
                            pr.ImageName = "";
                            pr.ImageUrl = "";
                        }

                        pr.GameType = 0;
                        if (v.kzPrize.game_type != null)
                        {
                            pr.GameType = (int)v.kzPrize.game_type;
                        }
                        pr.FreeDeal = false;
                        if (v.kzPrize.freedeal != null)
                        {
                            pr.FreeDeal = (bool)v.kzPrize.freedeal;
                        }
                        InventoryItem.Prize = pr;
                    }
                    var entityFlashDeal = from d in context.kzFlashDeals.Include("kzInventoryItem")
                                          where d.inventoryitem_id == v.id && d.last_action != "5" && d.flag == true
                                          && today >= d.start_time && today <= d.end_time
                                          select d;
                    if (entityFlashDeal.Count() > 0)
                    {
                        FlashDeal FlashDeal = new FlashDeal();
                        FlashDeal.FlashDealId = entityFlashDeal.First().id;
                        FlashDeal.MerchantId = (int)entityFlashDeal.First().kzInventoryItem.merchant_id;
                        FlashDeal.MerchantName = entityFlashDeal.First().kzInventoryItem.kzMerchant.name;
                        FlashDeal.InventoryItemId = (int)entityFlashDeal.First().inventoryitem_id;
                        FlashDeal.InventoryItemName = entityFlashDeal.First().kzInventoryItem.name;
                        FlashDeal.Discount = (decimal)entityFlashDeal.First().discount;
                        FlashDeal.StartTime = (DateTime)entityFlashDeal.First().start_time;
                        FlashDeal.EndTime = (DateTime)entityFlashDeal.First().end_time;
                        FlashDeal.Flag = (Boolean)entityFlashDeal.First().flag;
                        FlashDeal.LastAction = entityFlashDeal.First().last_action;
                        InventoryItem.FlashDeal = FlashDeal;
                    }
                    InventoryItemList.Add(InventoryItem);
                }
                response = Response<List<InventoryItemShow>>.Create(InventoryItemList);
            }

            return response;
        }


        public Response<List<InventoryItemShow>> GetInventoryItemListBySearch(string Search, DateTime today)
        {
            Response<List<InventoryItemShow>> response = null;
            List<InventoryItemShow> InventoryItemList = new List<InventoryItemShow>();
            using (var context = new entity.KuazooEntities())
            {
                string pattern = " " + Search + " ";
                string pattern1 = Search + " ";
                string pattern2 = " " + Search;
                string pattern3 = Search;
                var entityInvTag = from d in context.kzInventoryItemTags
                                   from e in context.kzTags
                                   where d.tag_id == e.id && e.name == Search
                                   select d.inventoryitem_id;
                List<int> inventoryList = new List<int>();
                foreach (var v in entityInvTag)
                {
                    inventoryList.Add(v);
                }

                var entityInventoryItem = from d in context.kzInventoryItems.Include("kzMerchant").Include("kzInventoryItemType")
                                          where d.kzPrize.last_action != "5" && d.kzPrize.expiry_date > today && today >= d.publish_date
                                          && d.flag == true && d.last_action != "5"
                                          && d.draft == false
                                          && ((" "+d.name+" ").Contains(pattern) ||
                                            d.description.StartsWith(pattern1) || d.description.EndsWith(pattern2) || d.description.Contains(pattern3) ||
                                            d.general_description.StartsWith(pattern1) || d.general_description.EndsWith(pattern2) || d.general_description.Contains(pattern3) ||
                                            (" " + d.short_desc + " ").Contains(pattern) ||
                                            inventoryList.Contains(d.id)
                                          )
                                          orderby d.last_created descending
                                          select d;
                foreach (var v in entityInventoryItem)
                {
                    InventoryItemShow InventoryItem = new InventoryItemShow();
                    InventoryItem.InventoryItemId = v.id;
                    InventoryItem.Name = v.name;
                    InventoryItem.ShortDesc = v.short_desc;
                    InventoryItem.GeneralDescription = "";
                    if (v.general_description != null)
                    {
                        InventoryItem.GeneralDescription = v.general_description;
                    }
                    InventoryItem.Terms = v.terms;
                    InventoryItem.Price = (decimal)v.price;
                    InventoryItem.TypeId = (int)v.inventoryitem_type_id;
                    InventoryItem.TypeName = v.kzInventoryItemType.name;
                    InventoryItem.Discount = (decimal)v.discount;
                    //var entityTransaction = from d in context.kzTransactionDetails
                    //                        where d.inventoryitem_id == v.id
                    //                        select d;
                    //if (entityTransaction.Count() > 0)
                    //{
                    //    InventoryItem.Sales = entityTransaction.Count();
                    //}
                    //else
                    //{
                    //    InventoryItem.Sales = 0;
                    //}
                    InventoryItem.Sales = (decimal)v.remainsales;
                    InventoryItem.MaximumSales = (decimal)v.maximumsales;
                    InventoryItem.SalesVisualMeter = (decimal)v.salesvisualmeter;
                    InventoryItem.Featured = (bool)v.featured;



                    List<VarianceDetail> listvariance = new List<VarianceDetail>();
                    var entityVarience = from d in context.kzVariances
                                         where d.inventoryitem_id == v.id
                                         select d;
                    foreach (var varien in entityVarience)
                    {
                        listvariance.Add(new VarianceDetail() { Variance = varien.name, Price = (decimal)varien.price, Discount = (decimal)varien.discount });
                    }
                    InventoryItem.Variance = listvariance;

                    var entityImg = from d in context.kzInventoryItemImages
                                    from e in context.kzImages
                                    where d.image_id == e.id && d.inventoryitem_id == v.id
                                    && d.main == true
                                    select new { d.image_id, e.url };

                    if (entityImg.Count() > 0)
                    {
                        InventoryItem.ImageId = entityImg.First().image_id;
                        InventoryItem.ImageName = entityImg.First().url;
                        InventoryItem.ImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImg.First().url;
                    }
                    else
                    {
                        InventoryItem.ImageId = 0;
                        InventoryItem.ImageName = "";
                        InventoryItem.ImageUrl = "";
                    }
                    decimal currentvisualmeter = InventoryItem.SalesVisualMeter;

                    var entityPrize = v.kzPrize;
                    if (entityPrize != null)
                    {
                        decimal fakevisual = (decimal)entityPrize.fake_visualmeter;
                        if (currentvisualmeter < fakevisual)
                        {
                            InventoryItem.SalesVisualMeter = fakevisual;
                        }   
                        Prize pr = new Prize();
                        pr.PrizeId = (int)entityPrize.id;
                        pr.Name = entityPrize.name;
                        pr.Price = (decimal)entityPrize.price;
                        pr.Description = entityPrize.description == null ? string.Empty : entityPrize.description;
                        pr.SponsorName = entityPrize.sponsor_name == null ? string.Empty : entityPrize.sponsor_name;
                        pr.Terms = entityPrize.terms == null ? string.Empty : entityPrize.terms;
                        pr.Detail = entityPrize.detail == null ? string.Empty : entityPrize.detail;
                        pr.ExpiryDate = (DateTime)entityPrize.expiry_date;
                        pr.GroupName = entityPrize.group_name == null ? string.Empty : entityPrize.group_name;
                        var entityImgPrize = from d in context.kzPrizeImages
                                             from e in context.kzImages
                                             where d.image_id == e.id && d.prize_id == pr.PrizeId
                                             && d.main == true
                                             select new { d.image_id, e.url };

                        if (entityImgPrize.Count() > 0)
                        {
                            pr.ImageId = entityImgPrize.First().image_id;
                            pr.ImageName = entityImgPrize.First().url;
                            pr.ImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImgPrize.First().url;
                        }
                        else
                        {
                            pr.ImageId = 0;
                            pr.ImageName = "";
                            pr.ImageUrl = "";
                        }

                        pr.GameType = 0;
                        if (v.kzPrize.game_type != null)
                        {
                            pr.GameType = (int)v.kzPrize.game_type;
                        }
                        pr.FreeDeal = false;
                        if (v.kzPrize.freedeal != null)
                        {
                            pr.FreeDeal = (bool)v.kzPrize.freedeal;
                        }
                        InventoryItem.Prize = pr;
                    }
                    var entityFlashDeal = from d in context.kzFlashDeals.Include("kzInventoryItem")
                                          where d.inventoryitem_id == v.id && d.last_action != "5" && d.flag == true
                                          && today >= d.start_time && today <= d.end_time
                                          select d;
                    if (entityFlashDeal.Count() > 0)
                    {
                        FlashDeal FlashDeal = new FlashDeal();
                        FlashDeal.FlashDealId = entityFlashDeal.First().id;
                        FlashDeal.MerchantId = (int)entityFlashDeal.First().kzInventoryItem.merchant_id;
                        FlashDeal.MerchantName = entityFlashDeal.First().kzInventoryItem.kzMerchant.name;
                        FlashDeal.InventoryItemId = (int)entityFlashDeal.First().inventoryitem_id;
                        FlashDeal.InventoryItemName = entityFlashDeal.First().kzInventoryItem.name;
                        FlashDeal.Discount = (decimal)entityFlashDeal.First().discount;
                        FlashDeal.StartTime = (DateTime)entityFlashDeal.First().start_time;
                        FlashDeal.EndTime = (DateTime)entityFlashDeal.First().end_time;
                        FlashDeal.Flag = (Boolean)entityFlashDeal.First().flag;
                        FlashDeal.LastAction = entityFlashDeal.First().last_action;
                        InventoryItem.FlashDeal = FlashDeal;
                    }
                    InventoryItemList.Add(InventoryItem);
                }
                response = Response<List<InventoryItemShow>>.Create(InventoryItemList);
            }

            return response;
        }

        public Response<List<InventoryItemShow>> GetInventoryItemListExpireSoon4(DateTime today, int InventoryItemJustPurchase)
        {
            Response<List<InventoryItemShow>> response = null;
            List<InventoryItemShow> InventoryItemList = new List<InventoryItemShow>();
            using (var context = new entity.KuazooEntities())
            {
                //var userPurchse = (from d in context.kzTransactionDetails
                                  //where d.kzTransaction.user_id == UserId
                                  //select d.inventoryitem_id).Distinct().ToList();
                var entityInventoryItem = (from d in context.kzInventoryItems.Include("kzMerchant").Include("kzInventoryItemType")
                                           where d.kzPrize.last_action != "5" && d.kzPrize.expiry_date > today && today >= d.publish_date
                                          && d.flag == true && d.last_action != "5"
                                          && d.draft == false
                                          //&& userPurchse.Contains(d.id)==false
                                          && d.id != InventoryItemJustPurchase
                                          orderby d.kzPrize.expiry_date ascending
                                          select d).Take(4);
                foreach (var v in entityInventoryItem)
                {
                    InventoryItemShow InventoryItem = new InventoryItemShow();
                    InventoryItem.InventoryItemId = v.id;
                    InventoryItem.Name = v.name;
                    InventoryItem.ShortDesc = v.short_desc;
                    InventoryItem.GeneralDescription = "";
                    if (v.general_description != null)
                    {
                        InventoryItem.GeneralDescription = v.general_description;
                    }
                    InventoryItem.Terms = v.terms;
                    InventoryItem.Price = (decimal)v.price;
                    InventoryItem.TypeId = (int)v.inventoryitem_type_id;
                    InventoryItem.TypeName = v.kzInventoryItemType.name;
                    InventoryItem.Discount = (decimal)v.discount;
                    //var entityTransaction = from d in context.kzTransactionDetails
                    //                        where d.inventoryitem_id == v.id
                    //                        select d;
                    //if (entityTransaction.Count() > 0)
                    //{
                    //    InventoryItem.Sales = entityTransaction.Count();
                    //}
                    //else
                    //{
                    //    InventoryItem.Sales = 0;
                    //}
                    InventoryItem.Sales = (decimal)v.remainsales;
                    InventoryItem.MaximumSales = (decimal)v.maximumsales;
                    InventoryItem.SalesVisualMeter = (decimal)v.salesvisualmeter;
                    InventoryItem.Featured = (bool)v.featured;

                    List<VarianceDetail> listvariance = new List<VarianceDetail>();
                    var entityVarience = from d in context.kzVariances
                                         where d.inventoryitem_id == v.id
                                         select d;
                    foreach (var varien in entityVarience)
                    {
                        listvariance.Add(new VarianceDetail() { Variance = varien.name, Price = (decimal)varien.price, Discount = (decimal)varien.discount });
                    }
                    InventoryItem.Variance = listvariance;

                    var entityImg = from d in context.kzInventoryItemImages
                                    from e in context.kzImages
                                    where d.image_id == e.id && d.inventoryitem_id == v.id
                                    && d.main == true
                                    select new { d.image_id, e.url };

                    if (entityImg.Count() > 0)
                    {
                        InventoryItem.ImageId = entityImg.First().image_id;
                        InventoryItem.ImageName = entityImg.First().url;
                        InventoryItem.ImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImg.First().url;
                    }
                    else
                    {
                        InventoryItem.ImageId = 0;
                        InventoryItem.ImageName = "";
                        InventoryItem.ImageUrl = "";
                    }
                    decimal currentvisualmeter = InventoryItem.SalesVisualMeter;

                    var entityPrize = v.kzPrize;
                    if (entityPrize != null)
                    {
                        decimal fakevisual = (decimal)entityPrize.fake_visualmeter;
                        if (currentvisualmeter < fakevisual)
                        {
                            InventoryItem.SalesVisualMeter = fakevisual;
                        }   
                        Prize pr = new Prize();
                        pr.PrizeId = (int)entityPrize.id;
                        pr.Name = entityPrize.name;
                        pr.Price = (decimal)entityPrize.price;
                        pr.Description = entityPrize.description == null ? string.Empty : entityPrize.description;
                        pr.SponsorName = entityPrize.sponsor_name == null ? string.Empty : entityPrize.sponsor_name;
                        pr.Terms = entityPrize.terms == null ? string.Empty : entityPrize.terms;
                        pr.Detail = entityPrize.detail == null ? string.Empty : entityPrize.detail;
                        pr.ExpiryDate = (DateTime)entityPrize.expiry_date;
                        pr.GroupName = entityPrize.group_name == null ? string.Empty : entityPrize.group_name;
                        var entityImgPrize = from d in context.kzPrizeImages
                                             from e in context.kzImages
                                             where d.image_id == e.id && d.prize_id == pr.PrizeId
                                             && d.main == true
                                             select new { d.image_id, e.url };

                        if (entityImgPrize.Count() > 0)
                        {
                            pr.ImageId = entityImgPrize.First().image_id;
                            pr.ImageName = entityImgPrize.First().url;
                            pr.ImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImgPrize.First().url;
                        }
                        else
                        {
                            pr.ImageId = 0;
                            pr.ImageName = "";
                            pr.ImageUrl = "";
                        }

                        pr.GameType = 0;
                        if (v.kzPrize.game_type != null)
                        {
                            pr.GameType = (int)v.kzPrize.game_type;
                        }
                        pr.FreeDeal = false;
                        if (v.kzPrize.freedeal != null)
                        {
                            pr.FreeDeal = (bool)v.kzPrize.freedeal;
                        }
                        InventoryItem.Prize = pr;
                    }
                    var entityFlashDeal = from d in context.kzFlashDeals.Include("kzInventoryItem")
                                          where d.inventoryitem_id == v.id && d.last_action != "5" && d.flag == true
                                          && today >= d.start_time && today <= d.end_time
                                          select d;
                    if (entityFlashDeal.Count() > 0)
                    {
                        FlashDeal FlashDeal = new FlashDeal();
                        FlashDeal.FlashDealId = entityFlashDeal.First().id;
                        FlashDeal.MerchantId = (int)entityFlashDeal.First().kzInventoryItem.merchant_id;
                        FlashDeal.MerchantName = entityFlashDeal.First().kzInventoryItem.kzMerchant.name;
                        FlashDeal.InventoryItemId = (int)entityFlashDeal.First().inventoryitem_id;
                        FlashDeal.InventoryItemName = entityFlashDeal.First().kzInventoryItem.name;
                        FlashDeal.Discount = (decimal)entityFlashDeal.First().discount;
                        FlashDeal.StartTime = (DateTime)entityFlashDeal.First().start_time;
                        FlashDeal.EndTime = (DateTime)entityFlashDeal.First().end_time;
                        FlashDeal.Flag = (Boolean)entityFlashDeal.First().flag;
                        FlashDeal.LastAction = entityFlashDeal.First().last_action;
                        InventoryItem.FlashDeal = FlashDeal;
                    }
                    InventoryItemList.Add(InventoryItem);
                }
                response = Response<List<InventoryItemShow>>.Create(InventoryItemList);
            }

            return response;
        }
        public int GetInventoryItemFlashDealCount(DateTime today)
        {
            int response = 0;
            using (var context = new entity.KuazooEntities())
            {
                var entityFlashDeal = from d in context.kzFlashDeals.Include("kzInventoryItem")
                                      where d.last_action != "5" && d.flag == true
                                      && today >= d.start_time && today <= d.end_time
                                      && d.kzInventoryItem.kzPrize.expiry_date> today && today>=d.kzInventoryItem.publish_date
                                      && d.kzInventoryItem.flag==true && d.kzInventoryItem.last_action !="5"
                                      select d;
                if (entityFlashDeal != null)
                {
                    response = entityFlashDeal.Count();
                }
            }

            return response;
        }

        public Response<InventoryItemShow> GetInventoryItemDetail(int InventoryItemId, DateTime today)
        {
            Response<InventoryItemShow> response = null;
            InventoryItemShow InventoryItem = new InventoryItemShow();
            using (var context = new entity.KuazooEntities())
            {
                var entityInventoryItem = from d in context.kzInventoryItems.Include("kzMerchant").Include("kzInventoryItemType")
                                          where d.id == InventoryItemId && d.kzPrize.last_action != "5" && d.kzPrize.expiry_date > today && today >= d.publish_date
                                          && d.flag == true && d.last_action != "5"
                                          && d.draft == false
                                          select d;
                if (entityInventoryItem.Count() > 0)
                {
                    var v = entityInventoryItem.First();
                    InventoryItem.InventoryItemId = v.id;
                    InventoryItem.Name = v.name;
                    InventoryItem.ShortDesc = v.short_desc;
                    InventoryItem.GeneralDescription = "";
                    if (v.general_description != null)
                    {
                        InventoryItem.GeneralDescription = v.general_description;
                    }
                    InventoryItem.Description = v.description;
                    InventoryItem.Terms = v.terms;
                    InventoryItem.Price = (decimal)v.price;
                    InventoryItem.TypeId = (int)v.inventoryitem_type_id;
                    InventoryItem.TypeName = v.kzInventoryItemType.name;
                    InventoryItem.Discount = (decimal)v.discount;
                    //var entityTransaction = from d in context.kzTransactionDetails
                    //                        where d.inventoryitem_id == v.id
                    //                        select d;
                    //if (entityTransaction.Count() > 0)
                    //{
                    //    InventoryItem.Sales = entityTransaction.Count();
                    //}
                    //else
                    //{
                    //    InventoryItem.Sales = 0;
                    //}
                    InventoryItem.Sales = (decimal)v.remainsales;
                    InventoryItem.MaximumSales = (decimal)v.maximumsales;
                    InventoryItem.SalesVisualMeter = (decimal)v.salesvisualmeter;
                    InventoryItem.ExpireDate = (DateTime)v.expiry_date;
                    InventoryItem.Featured = (bool)v.featured;

                    InventoryItem.Description = v.description;
                    List<String> currtag = new List<string>();
                    var entityTag = from d in context.kzInventoryItemTags
                                    from e in context.kzTags
                                    where d.tag_id == e.id && d.inventoryitem_id == v.id
                                    orderby d.tag_id ascending
                                    select new { d.tag_id, e.name };
                    foreach (var v2 in entityTag)
                    {
                        currtag.Add(v2.name);
                    }
                    InventoryItem.Tag = currtag;

                    InventoryItem.MerchantName = v.kzMerchant.name;
                    InventoryItem.AddressLine1 = v.kzMerchant.address_line1;
                    InventoryItem.AddressLine2 = v.kzMerchant.address_line2;
                    InventoryItem.MerchantCityName = v.kzMerchant.kzCity.name;
                    InventoryItem.MerchantCountryName = v.kzMerchant.kzCity.kzCountry.name;
                    InventoryItem.Latitude = (double)v.kzMerchant.latitude;
                    InventoryItem.Longitude = (double)v.kzMerchant.longitude;
                    InventoryItem.PostCode = v.kzMerchant.postcode;

                    List<VarianceDetail> listvariance = new List<VarianceDetail>();
                    var entityVarience = from d in context.kzVariances
                                         where d.inventoryitem_id == v.id
                                         select d;
                    foreach (var varien in entityVarience)
                    {
                        string _vari = varien.name + "`" + varien.price + "`" + varien.discount;
                        int count = context.kzTransactionDetails.Where(x => x.inventoryitem_id == v.id && x.variance.ToLower() == _vari.ToLower() && x.kzTransaction.last_action != "5").Count();
                        int alimit = (int)varien.available_limit - count;
                        if (alimit < 0) { alimit = 0; }
                        listvariance.Add(new VarianceDetail() { Variance = varien.name, Price = (decimal)varien.price, Discount = (decimal)varien.discount, AvailabelLimit = alimit });
                    }
                    InventoryItem.Variance = listvariance;

                    var entityImg = from d in context.kzInventoryItemImages
                                    from e in context.kzImages
                                    where d.image_id == e.id && d.inventoryitem_id == v.id
                                    && d.main == true
                                    select new { d.image_id, e.url };

                    if (entityImg.Count() > 0)
                    {
                        InventoryItem.ImageId = entityImg.First().image_id;
                        InventoryItem.ImageName = entityImg.First().url;
                        InventoryItem.ImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImg.First().url;
                    }
                    else
                    {
                        InventoryItem.ImageId = 0;
                        InventoryItem.ImageName = "";
                        InventoryItem.ImageUrl = "";
                    }
                    List<int> subimgid = new List<int>();
                    List<string> subimgname = new List<string>();
                    List<string> subimgurl = new List<string>();
                    var entitySubImg = from d in context.kzInventoryItemImages
                                       from e in context.kzImages
                                       where d.image_id == e.id && d.inventoryitem_id == v.id
                                       && d.main == false
                                       select new { d.image_id, e.url };
                    foreach (var v2 in entitySubImg)
                    {
                        subimgid.Add(v2.image_id);
                        subimgname.Add(v2.url);
                        subimgurl.Add(ConfigurationManager.AppSettings["uploadpath"] + v2.url);
                    }
                    InventoryItem.SubImageId = subimgid;
                    InventoryItem.SubImageName = subimgname;
                    InventoryItem.SubImageUrl = subimgurl;


                    decimal currentvisualmeter = InventoryItem.SalesVisualMeter;

                    var entityPrize = v.kzPrize;
                    if (entityPrize != null)
                    {
                        decimal fakevisual = (decimal)entityPrize.fake_visualmeter;
                        if (currentvisualmeter < fakevisual)
                        {
                            InventoryItem.SalesVisualMeter = fakevisual;
                        }   
                        Prize pr = new Prize();
                        pr.PrizeId = (int)entityPrize.id;
                        pr.Name = entityPrize.name;
                        pr.Price = (decimal)entityPrize.price;
                        pr.Description = entityPrize.description == null ? string.Empty : entityPrize.description;
                        pr.SponsorName = entityPrize.sponsor_name == null ? string.Empty : entityPrize.sponsor_name;
                        pr.Terms = entityPrize.terms == null ? string.Empty : entityPrize.terms;
                        pr.Detail = entityPrize.detail == null ? string.Empty : entityPrize.detail;
                        pr.ExpiryDate = (DateTime)entityPrize.expiry_date;
                        pr.GroupName = entityPrize.group_name == null ? string.Empty : entityPrize.group_name;
                        var entityImgPrize = from d in context.kzPrizeImages
                                             from e in context.kzImages
                                             where d.image_id == e.id && d.prize_id == pr.PrizeId
                                             && d.main == true
                                             select new { d.image_id, e.url };

                        if (entityImgPrize.Count() > 0)
                        {
                            pr.ImageId = entityImgPrize.First().image_id;
                            pr.ImageName = entityImgPrize.First().url;
                            pr.ImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImgPrize.First().url;
                        }
                        else
                        {
                            pr.ImageId = 0;
                            pr.ImageName = "";
                            pr.ImageUrl = "";
                        }
                        List<int> subimgidprize = new List<int>();
                        List<string> subimgnameprize = new List<string>();
                        List<string> subimgurlprize = new List<string>();
                        var entitySubImgprize = from d in context.kzPrizeImages
                                                from e in context.kzImages
                                                where d.image_id == e.id && d.prize_id == pr.PrizeId
                                                && d.main == false
                                                select new { d.image_id, e.url };
                        foreach (var v2 in entitySubImgprize)
                        {
                            subimgidprize.Add(v2.image_id);
                            subimgnameprize.Add(v2.url);
                            subimgurlprize.Add(ConfigurationManager.AppSettings["uploadpath"] + v2.url);
                        }
                        pr.SubImageId = subimgidprize;
                        pr.SubImageName = subimgnameprize;
                        pr.SubImageUrl = subimgurlprize;

                        pr.GameType = 0;
                        if (v.kzPrize.game_type != null)
                        {
                            pr.GameType = (int)v.kzPrize.game_type;
                        }
                        pr.FreeDeal = false;
                        if (v.kzPrize.freedeal != null)
                        {
                            pr.FreeDeal = (bool)v.kzPrize.freedeal;
                        }
                        InventoryItem.Prize = pr;
                    }
                    var entityFlashDeal = from d in context.kzFlashDeals.Include("kzInventoryItem")
                                          where d.inventoryitem_id == v.id && d.last_action != "5" && d.flag == true
                                          && today >= d.start_time && today <= d.end_time
                                          select d;
                    if (entityFlashDeal.Count() > 0)
                    {
                        FlashDeal FlashDeal = new FlashDeal();
                        FlashDeal.FlashDealId = entityFlashDeal.First().id;
                        FlashDeal.MerchantId = (int)entityFlashDeal.First().kzInventoryItem.merchant_id;
                        FlashDeal.MerchantName = entityFlashDeal.First().kzInventoryItem.kzMerchant.name;
                        FlashDeal.InventoryItemId = (int)entityFlashDeal.First().inventoryitem_id;
                        FlashDeal.InventoryItemName = entityFlashDeal.First().kzInventoryItem.name;
                        FlashDeal.Discount = (decimal)entityFlashDeal.First().discount;
                        FlashDeal.StartTime = (DateTime)entityFlashDeal.First().start_time;
                        FlashDeal.EndTime = (DateTime)entityFlashDeal.First().end_time;
                        FlashDeal.Flag = (Boolean)entityFlashDeal.First().flag;
                        FlashDeal.LastAction = entityFlashDeal.First().last_action;
                        InventoryItem.FlashDeal = FlashDeal;
                    }
                }
                response = Response<InventoryItemShow>.Create(InventoryItem);
            }

            return response;
        }
        public Response<InventoryItemShow> GetInventoryItemDetailByName(String name, DateTime today)
        {
            Response<InventoryItemShow> response = null;
            InventoryItemShow InventoryItem = new InventoryItemShow();
            using (var context = new entity.KuazooEntities())
            {
                var entityInventoryItem = from d in context.kzInventoryItems.Include("kzMerchant").Include("kzInventoryItemType")
                                          where d.name.Replace(" ", "-").Replace("%", "percent").Replace("&", "and") == name && d.kzPrize.last_action != "5" && d.kzPrize.expiry_date > today && today >= d.publish_date
                                          && d.flag == true && d.last_action != "5"
                                          && d.draft == false
                                          select d;

                foreach (var v in entityInventoryItem)
                {
                    InventoryItem.InventoryItemId = v.id;
                    InventoryItem.Name = v.name;
                    InventoryItem.ShortDesc = v.short_desc;
                    InventoryItem.GeneralDescription = "";
                    if (v.general_description != null)
                    {
                        InventoryItem.GeneralDescription = v.general_description;
                    }
                    InventoryItem.Description = v.description;
                    InventoryItem.Terms = v.terms;
                    InventoryItem.Price = (decimal)v.price;
                    InventoryItem.TypeId = (int)v.inventoryitem_type_id;
                    InventoryItem.TypeName = v.kzInventoryItemType.name;
                    InventoryItem.Discount = (decimal)v.discount;
                    //var entityTransaction = from d in context.kzTransactionDetails
                    //                        where d.inventoryitem_id == v.id
                    //                        select d;
                    //if (entityTransaction.Count() > 0)
                    //{
                    //    InventoryItem.Sales = entityTransaction.Count();
                    //}
                    //else
                    //{
                    //    InventoryItem.Sales = 0;
                    //}
                    InventoryItem.Sales = (decimal)v.remainsales;
                    InventoryItem.MaximumSales = (decimal)v.maximumsales;
                    InventoryItem.SalesVisualMeter = (decimal)v.salesvisualmeter;
                    InventoryItem.ExpireDate = (DateTime)v.expiry_date;
                    InventoryItem.Featured = (bool)v.featured;

                    List<String> currtag = new List<string>();
                    var entityTag = from d in context.kzInventoryItemTags
                                    from e in context.kzTags
                                    where d.tag_id == e.id && d.inventoryitem_id == v.id
                                    orderby d.tag_id ascending
                                    select new { d.tag_id, e.name };
                    foreach (var v2 in entityTag)
                    {
                        currtag.Add(v2.name);
                    }
                    InventoryItem.Tag = currtag;

                    InventoryItem.MerchantName = v.kzMerchant.name;
                    InventoryItem.MerchantContactNumber = v.kzMerchant.contact_number;
                    InventoryItem.MerchantEmail = v.kzMerchant.email;
                    InventoryItem.MerchantWebsite = v.kzMerchant.website;
                    InventoryItem.MerchantFacebook = v.kzMerchant.facebook;
                    InventoryItem.AddressLine1 = v.kzMerchant.address_line1;
                    InventoryItem.AddressLine2 = v.kzMerchant.address_line2;
                    InventoryItem.MerchantCityName = v.kzMerchant.kzCity.name;
                    InventoryItem.MerchantCountryName = v.kzMerchant.kzCity.kzCountry.name;
                    InventoryItem.Latitude = (double)v.kzMerchant.latitude;
                    InventoryItem.Longitude = (double)v.kzMerchant.longitude;
                    InventoryItem.PostCode = v.kzMerchant.postcode;
                    InventoryItem.MerchantDescription = v.kzMerchant.description;
                    List<int> mersubimgid = new List<int>();
                    List<string> mersubimgname = new List<string>();
                    List<string> mersubimgurl = new List<string>();
                    List<string> mersubimgurllink = new List<string>();
                    var entityMerSubImg = from d in context.kzMerchantImages
                                       from e in context.kzImages
                                       where d.image_id == e.id && d.merchant_id == v.kzMerchant.id
                                       select new { d.image_id, e.url , d.image_url};
                    foreach (var v2 in entityMerSubImg)
                    {
                        mersubimgid.Add(v2.image_id);
                        mersubimgname.Add(v2.url);
                        mersubimgurl.Add(ConfigurationManager.AppSettings["uploadpath"] + v2.url);
                        mersubimgurllink.Add(v2.image_url);
                    }
                    InventoryItem.MerchantSubImageId = mersubimgid;
                    InventoryItem.MerchantSubImageName = mersubimgname;
                    InventoryItem.MerchantSubImageUrl = mersubimgurl;
                    InventoryItem.MerchantSubImageUrlLink = mersubimgurllink;


                    List<VarianceDetail> listvariance = new List<VarianceDetail>();
                    var entityVarience = from d in context.kzVariances
                                         where d.inventoryitem_id == v.id
                                         select d;
                    foreach (var varien in entityVarience)
                    {
                        string _vari = varien.name + "`" + varien.price + "`" + varien.discount;
                        int count = context.kzTransactionDetails.Where(x => x.inventoryitem_id == v.id && x.variance.ToLower() == _vari.ToLower() && x.kzTransaction.last_action != "5").Count();
                        int alimit = (int)varien.available_limit - count;
                        if (alimit < 0) { alimit = 0; }
                        listvariance.Add(new VarianceDetail() { Variance = varien.name, Price = (decimal)varien.price, Discount = (decimal)varien.discount, AvailabelLimit = alimit });
                    }
                    InventoryItem.Variance = listvariance;

                    var entityImg = from d in context.kzInventoryItemImages
                                    from e in context.kzImages
                                    where d.image_id == e.id && d.inventoryitem_id == v.id
                                    && d.main == true
                                    select new { d.image_id, e.url };

                    if (entityImg.Count() > 0)
                    {
                        InventoryItem.ImageId = entityImg.First().image_id;
                        InventoryItem.ImageName = entityImg.First().url;
                        InventoryItem.ImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImg.First().url;
                    }
                    else
                    {
                        InventoryItem.ImageId = 0;
                        InventoryItem.ImageName = "";
                        InventoryItem.ImageUrl = "";
                    }
                    List<int> subimgid = new List<int>();
                    List<string> subimgname = new List<string>();
                    List<string> subimgurl = new List<string>();
                    var entitySubImg = from d in context.kzInventoryItemImages
                                       from e in context.kzImages
                                       where d.image_id == e.id && d.inventoryitem_id == v.id
                                       && d.main == false
                                       select new { d.image_id, e.url };
                    foreach (var v2 in entitySubImg)
                    {
                        subimgid.Add(v2.image_id);
                        subimgname.Add(v2.url);
                        subimgurl.Add(ConfigurationManager.AppSettings["uploadpath"] + v2.url);
                    }
                    InventoryItem.SubImageId = subimgid;
                    InventoryItem.SubImageName = subimgname;
                    InventoryItem.SubImageUrl = subimgurl;


                    decimal currentvisualmeter = InventoryItem.SalesVisualMeter;

                    var entityPrize = v.kzPrize;
                    if (entityPrize != null)
                    {
                        decimal fakevisual = (decimal)entityPrize.fake_visualmeter;
                        if (currentvisualmeter < fakevisual)
                        {
                            InventoryItem.SalesVisualMeter = fakevisual;
                        }   
                        Prize pr = new Prize();
                        pr.PrizeId = (int)entityPrize.id;
                        pr.Name = entityPrize.name;
                        pr.Price = (decimal)entityPrize.price;
                        pr.Description = entityPrize.description == null ? string.Empty : entityPrize.description;
                        pr.SponsorName = entityPrize.sponsor_name == null ? string.Empty : entityPrize.sponsor_name;
                        pr.Terms = entityPrize.terms == null ? string.Empty : entityPrize.terms;
                        pr.Detail = entityPrize.detail == null ? string.Empty : entityPrize.detail;
                        pr.ExpiryDate = (DateTime)entityPrize.expiry_date;
                        pr.GroupName = entityPrize.group_name == null ? string.Empty : entityPrize.group_name;
                        var entityImgPrize = from d in context.kzPrizeImages
                                             from e in context.kzImages
                                             where d.image_id == e.id && d.prize_id == pr.PrizeId
                                             && d.main == true
                                             select new { d.image_id, e.url };

                        if (entityImgPrize.Count() > 0)
                        {
                            pr.ImageId = entityImgPrize.First().image_id;
                            pr.ImageName = entityImgPrize.First().url;
                            pr.ImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImgPrize.First().url;
                        }
                        else
                        {
                            pr.ImageId = 0;
                            pr.ImageName = "";
                            pr.ImageUrl = "";
                        }
                        List<int> subimgidprize = new List<int>();
                        List<string> subimgnameprize = new List<string>();
                        List<string> subimgurlprize = new List<string>();
                        var entitySubImgprize = from d in context.kzPrizeImages
                                                from e in context.kzImages
                                                where d.image_id == e.id && d.prize_id == pr.PrizeId
                                                && d.main == false
                                                select new { d.image_id, e.url };
                        foreach (var v2 in entitySubImgprize)
                        {
                            subimgidprize.Add(v2.image_id);
                            subimgnameprize.Add(v2.url);
                            subimgurlprize.Add(ConfigurationManager.AppSettings["uploadpath"] + v2.url);
                        }
                        pr.SubImageId = subimgidprize;
                        pr.SubImageName = subimgnameprize;
                        pr.SubImageUrl = subimgurlprize;

                        pr.GameType = 0;
                        if (v.kzPrize.game_type != null)
                        {
                            pr.GameType = (int)v.kzPrize.game_type;
                        }
                        pr.FreeDeal = false;
                        if (v.kzPrize.freedeal != null)
                        {
                            pr.FreeDeal = (bool)v.kzPrize.freedeal;
                        }
                        InventoryItem.Prize = pr;
                    }
                    var entityFlashDeal = from d in context.kzFlashDeals.Include("kzInventoryItem")
                                          where d.inventoryitem_id == v.id && d.last_action != "5" && d.flag == true
                                          && today >= d.start_time && today <= d.end_time
                                          select d;
                    if (entityFlashDeal.Count() > 0)
                    {
                        FlashDeal FlashDeal = new FlashDeal();
                        FlashDeal.FlashDealId = entityFlashDeal.First().id;
                        FlashDeal.MerchantId = (int)entityFlashDeal.First().kzInventoryItem.merchant_id;
                        FlashDeal.MerchantName = entityFlashDeal.First().kzInventoryItem.kzMerchant.name;
                        FlashDeal.InventoryItemId = (int)entityFlashDeal.First().inventoryitem_id;
                        FlashDeal.InventoryItemName = entityFlashDeal.First().kzInventoryItem.name;
                        FlashDeal.Discount = (decimal)entityFlashDeal.First().discount;
                        FlashDeal.StartTime = (DateTime)entityFlashDeal.First().start_time;
                        FlashDeal.EndTime = (DateTime)entityFlashDeal.First().end_time;
                        FlashDeal.Flag = (Boolean)entityFlashDeal.First().flag;
                        FlashDeal.LastAction = entityFlashDeal.First().last_action;
                        InventoryItem.FlashDeal = FlashDeal;
                    }
                }
                response = Response<InventoryItemShow>.Create(InventoryItem);
            }

            return response;
        }
        public Response<InventoryItem> GetInventoryItemById(int InventoryItemId)
        {
            Response<InventoryItem> response = null;
            InventoryItem InventoryItem = new InventoryItem();
            using (var context = new entity.KuazooEntities())
            {
                var entityInventoryItem = from d in context.kzInventoryItems.Include("kzMerchant").Include("kzInventoryItemType")
                                    where d.id == InventoryItemId
                                    select d;
                var v = entityInventoryItem.First();
                if (v != null)
                {
                    InventoryItem.Draft = (bool)v.draft;
                    InventoryItem.InventoryItemId = v.id;
                    InventoryItem.Name = v.name;
                    InventoryItem.ShortDesc = v.short_desc;
                    InventoryItem.Price = (decimal)v.price;
                    InventoryItem.Margin = (decimal)v.margin;
                    InventoryItem.Description = v.description;
                    InventoryItem.GeneralDescription = v.general_description;
                    InventoryItem.Terms = v.terms;
                    InventoryItem.Keyword = v.keywords;
                    InventoryItem.Priority = (int)v.priority;

                    City merchantcity = new City()
                    {
                        CityId = (int)v.kzMerchant.city_id,
                        Name = v.kzMerchant.kzCity.name
                    };
                    Country merchantcountry = new Country()
                    {
                        CountryId = (int)v.kzMerchant.kzCity.country_id,
                        Name = v.kzMerchant.kzCity.kzCountry.name
                    };

                    Merchant merchant = new Merchant()
                    {
                        MerchantId = (int)v.merchant_id,
                        Name = v.kzMerchant.name,
                        City = merchantcity,
                        Country = merchantcountry
                    };
                    InventoryItem.Merchant = merchant;

                    City city = new City()
                    {
                        CityId = (int)v.city_id,
                        Name = v.kzCity.name
                    };
                    InventoryItem.City = city;

                    Country country = new Country()
                    {
                        CountryId = (int)v.kzCity.country_id,
                        Name = v.kzCity.kzCountry.name
                    };
                    InventoryItem.Country = country;
                    InventoryItem.Merchant = merchant;
                    InventoryItemType InventoryItemtype = new InventoryItemType()
                    {
                        InventoryItemTypeId = (int)v.inventoryitem_type_id,
                        InventoryItemTypeName = v.kzInventoryItemType.name
                    };
                    InventoryItem.InventoryItemType = InventoryItemtype;
                    InventoryItem.Discount = (decimal)v.discount;
                    InventoryItem.ExpireDate = (DateTime)v.expiry_date;
                    InventoryItem.PublishDate = (DateTime)v.publish_date;
                    InventoryItem.MinimumTarget = (decimal)v.minimumtarget;
                    InventoryItem.SalesVisualMeter = (decimal)v.salesvisualmeter;
                    InventoryItem.MaximumSales = (decimal)v.maximumsales;
                    InventoryItem.RemainSales = (decimal)v.remainsales;
                    InventoryItem.Create = (DateTime)v.last_created;
                    InventoryItem.Update = (DateTime)v.last_updated;
                    InventoryItem.Flag = (bool)v.flag;
                    InventoryItem.Featured = (bool)v.featured;
                    InventoryItem.LastAction = v.last_action;
                    string currtag = "";
                    string currtagname = "";
                    var entityTag = from d in context.kzInventoryItemTags
                                    from e in context.kzTags
                                    where d.tag_id == e.id && d.inventoryitem_id == v.id
                                    orderby d.tag_id ascending
                                    select new { d.tag_id, e.name };
                    foreach (var v2 in entityTag)
                    {
                        currtag += v2.tag_id + ",";
                        currtagname += v2.name + ", ";
                    }
                    if (currtag.Length > 1)
                    {
                        currtag = currtag.Substring(0, currtag.Length - 1);
                        currtagname = currtagname.Substring(0, currtagname.Length - 2);
                    }
                    InventoryItem.Tag = currtag;
                    InventoryItem.TagName = currtagname;

                    List<VarianceDetail> listvariance = new List<VarianceDetail>();
                    var entityVarience = from d in context.kzVariances
                                         where d.inventoryitem_id == v.id
                                         select d;
                    foreach (var varien in entityVarience)
                    {
                        string _variane = varien.name;
                        var entityTrans = from d in context.kzTransactionDetails
                                          where d.inventoryitem_id == v.id && d.variance.Contains(_variane + "`")
                                          select d;
                        int used = entityTrans.Count();
                        listvariance.Add(new VarianceDetail() { Variance = varien.name, Price = (decimal)varien.price, Discount = (decimal)varien.discount,
                                    Margin = (decimal)varien.margin, AvailabelLimit = (int)varien.available_limit, Used = used});
                    }
                    InventoryItem.Variance = listvariance;

                    var entityImg = from d in context.kzInventoryItemImages
                                    from e in context.kzImages
                                    where d.image_id == e.id && d.inventoryitem_id == v.id
                                    && d.main == true
                                    select new { d.image_id, e.url };

                    if (entityImg.Count() > 0)
                    {
                        InventoryItem.ImageId = entityImg.First().image_id;
                        InventoryItem.ImageName = entityImg.First().url;
                        InventoryItem.ImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImg.First().url;
                    }
                    else
                    {
                        InventoryItem.ImageId = 0;
                        InventoryItem.ImageName = "";
                        InventoryItem.ImageUrl = "";
                    }

                    List<int> subimgid = new List<int>();
                    List<string> subimgname = new List<string>();
                    List<string> subimgurl = new List<string>();
                    var entitySubImg = from d in context.kzInventoryItemImages
                                    from e in context.kzImages
                                    where d.image_id == e.id && d.inventoryitem_id == v.id
                                    && d.main == false
                                    select new { d.image_id, e.url };
                    foreach(var v2 in entitySubImg)
                    {
                        subimgid.Add(v2.image_id);
                        subimgname.Add(v2.url);
                        subimgurl.Add(ConfigurationManager.AppSettings["uploadpath"] + v2.url);
                    }
                    InventoryItem.SubImageId = subimgid;
                    InventoryItem.SubImageName = subimgname;
                    InventoryItem.SubImageUrl = subimgurl;


                    decimal currentvisualmeter = InventoryItem.SalesVisualMeter;

                    var entityPrize = v.kzPrize;
                    if (entityPrize != null)
                    {
                        decimal fakevisual = (decimal)entityPrize.fake_visualmeter;
                        if (currentvisualmeter < fakevisual)
                        {
                            InventoryItem.SalesVisualMeter = fakevisual;
                        }   
                        InventoryItem.PrizeId = (int)entityPrize.id;
                        Prize pr = new Prize();
                        pr.PrizeId = (int)entityPrize.id;
                        pr.Name = entityPrize.name;
                        pr.Price = (decimal)entityPrize.price;
                        pr.Description = entityPrize.description == null ? string.Empty : entityPrize.description;
                        pr.SponsorName = entityPrize.sponsor_name == null ? string.Empty : entityPrize.sponsor_name;
                        pr.Terms = entityPrize.terms == null ? string.Empty : entityPrize.terms;
                        pr.Detail = entityPrize.detail == null ? string.Empty : entityPrize.detail;
                        pr.ExpiryDate = (DateTime)entityPrize.expiry_date;
                        pr.GroupName = entityPrize.group_name == null ? string.Empty : entityPrize.group_name;
                        var entityImgPrize = from d in context.kzPrizeImages
                                             from e in context.kzImages
                                             where d.image_id == e.id && d.prize_id == pr.PrizeId
                                             && d.main == true
                                             select new { d.image_id, e.url };
                        if (entityImgPrize.Count() > 0)
                        {
                            pr.ImageId = entityImgPrize.First().image_id;
                            pr.ImageName = entityImgPrize.First().url;
                            pr.ImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImgPrize.First().url;
                        }
                        else
                        {
                            pr.ImageId = 0;
                            pr.ImageName = "";
                            pr.ImageUrl = "";
                        }

                        List<int> subimgidprize = new List<int>();
                        List<string> subimgnameprize = new List<string>();
                        List<string> subimgurlprize = new List<string>();
                        var entitySubImgprize = from d in context.kzPrizeImages
                                                from e in context.kzImages
                                                where d.image_id == e.id && d.prize_id == pr.PrizeId
                                                && d.main == false
                                                select new { d.image_id, e.url };
                        foreach (var v2 in entitySubImgprize)
                        {
                            subimgidprize.Add(v2.image_id);
                            subimgnameprize.Add(v2.url);
                            subimgurlprize.Add(ConfigurationManager.AppSettings["uploadpath"] + v2.url);
                        }
                        pr.SubImageId = subimgidprize;
                        pr.SubImageName = subimgnameprize;
                        pr.SubImageUrl = subimgurlprize;
                        pr.SponsorName = entityPrize.sponsor_name;
                        pr.Terms = entityPrize.terms;
                        pr.Detail = entityPrize.detail;
                        pr.FreeDeal = false;
                        if (v.kzPrize.freedeal != null)
                        {
                            pr.FreeDeal = (bool)v.kzPrize.freedeal;
                        }
                        InventoryItem.Prize = pr;
                    }
                }
                response = Response<InventoryItem>.Create(InventoryItem);
            }

            return response;
        }
        public Response<bool> DeleteInventoryItem(int InventoryItemId)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                var entityUser = from d in context.kzInventoryItems
                                 where d.id == InventoryItemId
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
        public Response<bool> UpdateInventoryItem(int InventoryItemId, int InventoryPriority, bool InventoryPopular, bool InventoryReviewed)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                var entityUser = from d in context.kzInventoryItems
                                 where d.id == InventoryItemId
                                 select d;
                if (entityUser.Count() > 0)
                {
                    entityUser.First().priority = InventoryPriority;
                    if (entityUser.First().kzInventoryItemRP != null)
                    {
                        entityUser.First().kzInventoryItemRP.popular = InventoryPopular;
                        entityUser.First().kzInventoryItemRP.reviewed = InventoryReviewed;
                    }
                    else
                    {
                        entity.kzInventoryItemRP mmentity = new entity.kzInventoryItemRP();
                        mmentity.inventory_id = InventoryItemId;
                        mmentity.popular = InventoryPopular;
                        mmentity.reviewed = InventoryReviewed;
                        context.AddTokzInventoryItemRPs(mmentity);
                    }
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
        public Response<bool> DuplicateInventoryItem(int InventoryItemId)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                InventoryItem item = GetInventoryItemById(InventoryItemId).Result;
                item.InventoryItemId = 0;
                item.Prize = null;
                item.PrizeId = 0;
                item.Name = item.Name + " - Copy";
                var result = CreateInventoryItem(item).Result;
                response = Response<bool>.Create(result);
            }

            return response;
        }



        public Response<bool> CreateWishlist(int InventoryItemid)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                var user = new UserService().GetCurrentUser().Result;
                if (user.UserId != 0)
                {
                    var entitywish = from d in context.kzWishLists
                                     where d.user_id == user.UserId && d.inventoryitem_id == InventoryItemid
                                     && d.last_action != "5"
                                     select d;
                    if (entitywish.Count() > 0)
                    {
                        throw new CustomException(CustomErrorType.WishlistAlreadyAssign);
                    }
                    else
                    {
                        entity.kzWishList wi = new entity.kzWishList();
                        wi.user_id = user.UserId;
                        wi.inventoryitem_id = InventoryItemid;
                        wi.last_action = "1";
                        wi.last_created = DateTime.UtcNow;
                        wi.last_updated = DateTime.UtcNow;
                        context.AddTokzWishLists(wi);
                        context.SaveChanges();
                        response = Response<bool>.Create(true);
                    }
                }
                else
                {
                    throw new CustomException(CustomErrorType.Unauthorized);
                }
            }
            return response;
        }
        public Response<bool> DeleteWishlist(int WishId)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                var entityUser = from d in context.kzWishLists
                                 where d.id == WishId
                                 select d;
                if (entityUser.Count() > 0)
                {
                    entityUser.First().last_action = "5";
                    entityUser.First().last_updated = DateTime.UtcNow;
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
        public Response<List<Wishlist>> GetWishList(DateTime today)
        {
            Response<List<Wishlist>> response = null;
            List<Wishlist> Wishlistlist = new List<Wishlist>();
            using (var context = new entity.KuazooEntities())
            {
                var user = new UserService().GetCurrentUser().Result;
                var entityWish = from d in context.kzWishLists
                                 where d.user_id == user.UserId
                                 && d.last_action !="5"
                                 select d;
                foreach (var v in entityWish)
                {
                    int inventoryid = (int)v.inventoryitem_id;
                    var res = GetInventoryItemDetail(inventoryid, today);
                    if (res != null && res.Result != null && res.Result.InventoryItemId != 0)
                    {
                        Wishlist wi = new Wishlist();
                        wi.WishlistId = v.id;
                        wi.InventoryItem = res.Result;
                        Wishlistlist.Add(wi);
                    }
                }
                response = Response<List<Wishlist>>.Create(Wishlistlist);
            }

            return response;
        }
        //public Response<bool> CreateInventoryItemTag(InventoryItemTag InventoryItem, int oldTagId=0)
        //{
        //    Response<bool> response = null;
        //    using (var context = new entity.KuazooEntities())
        //    {
        //        if (oldTagId != 0)
        //        {
        //            var entityInventoryItem = from d in context.kzInventoryItemTags
        //                             where d.inventoryitem_id == InventoryItem.InventoryItemId && d.tag_id == oldTagId
        //                             select d;
        //            if (entityInventoryItem.Count() > 0)
        //            {
        //                var entityInventoryItem2 = from d in context.kzInventoryItemTags
        //                                           from e in context.kzTags
        //                                           where d.tag_id == e.id
        //                                           && d.inventoryitem_id == InventoryItem.InventoryItemId
        //                                           && d.tag_id != oldTagId
        //                                           && e.name.ToLower() == InventoryItem.TagName.ToLower()
        //                                           select d;
        //                if (entityInventoryItem2.Count() > 0)
        //                {
        //                    throw new CustomException(CustomErrorType.InventoryItemTagAlreadyAssign);
        //                }
        //                else
        //                {
        //                    entityInventoryItem.First().inventoryitem_id = InventoryItem.InventoryItemId;

        //                    //search tagname
        //                    int tagid = 0;
        //                    var entityTag = from d in context.kzTags
        //                                    where d.name.ToLower() == InventoryItem.TagName.ToLower()
        //                                    select d;
        //                    if (entityTag.Count() > 0)
        //                    {
        //                        tagid = entityTag.First().id;
        //                    }
        //                    else
        //                    {
        //                        entity.kzTag tags = new entity.kzTag()
        //                        {
        //                            name = InventoryItem.TagName,
        //                            last_created = DateTime.UtcNow,
        //                            last_updated = DateTime.UtcNow,
        //                            last_action = "1"
        //                        };
        //                        context.AddTokzTags(tags);
        //                        context.SaveChanges();
        //                        tagid = tags.id;
        //                    }

        //                    entityInventoryItem.First().tag_id = tagid;//InventoryItem.TagId;
        //                    context.SaveChanges();
        //                    response = Response<bool>.Create(true);
        //                }
        //            }
        //            else
        //            {
        //                throw new CustomException(CustomErrorType.InventoryItemTagNotFound);
        //            }
        //        }
        //        else
        //        {
        //            var entityInventoryItem = from d in context.kzInventoryItemTags
        //                                       from e in context.kzTags
        //                                       where d.tag_id == e.id
        //                                       && d.inventoryitem_id == InventoryItem.InventoryItemId
        //                                       && e.name.ToLower() == InventoryItem.TagName.ToLower()
        //                                       select d;
        //            if (entityInventoryItem.Count() > 0)
        //            {
        //                throw new CustomException(CustomErrorType.InventoryItemTagAlreadyAssign);
        //            }
        //            else
        //            {
        //                entity.kzInventoryItemTag mmentity = new entity.kzInventoryItemTag();
        //                mmentity.inventoryitem_id = InventoryItem.InventoryItemId;

        //                //search tagname
        //                int tagid = 0;
        //                var entityTag = from d in context.kzTags
        //                                where d.name.ToLower() == InventoryItem.TagName.ToLower()
        //                                select d;
        //                if (entityTag.Count() > 0)
        //                {
        //                    tagid = entityTag.First().id;
        //                }
        //                else
        //                {
        //                    entity.kzTag tags = new entity.kzTag()
        //                    {
        //                        name = InventoryItem.TagName,
        //                        last_created = DateTime.UtcNow,
        //                        last_updated = DateTime.UtcNow,
        //                        last_action = "1"
        //                    };
        //                    context.AddTokzTags(tags);
        //                    context.SaveChanges();
        //                    tagid = tags.id;
        //                }

        //                mmentity.tag_id = tagid;//InventoryItem.TagId;
        //                context.AddTokzInventoryItemTags(mmentity);
        //                context.SaveChanges();
        //                response = Response<bool>.Create(true);
        //            }
        //        }
        //    }
        //    return response;
        //}
        //public Response<List<InventoryItemTag>> GetInventoryItemTagList(int InventoryItemId)
        //{
        //    Response<List<InventoryItemTag>> response = null;
        //    List<InventoryItemTag> InventoryItemList = new List<InventoryItemTag>();
        //    using (var context = new entity.KuazooEntities())
        //    {
        //        var entityInventoryItem = from d in context.kzInventoryItemTags
        //                                  from e in context.kzTags
        //                                  from f in context.kzInventoryItems
        //                                  where d.tag_id == e.id && d.inventoryitem_id == f.id
        //                                  && d.inventoryitem_id == InventoryItemId
        //                                  select new { d.inventoryitem_id, inventoryitem_name = f.name, d.tag_id, tag_name = e.name };
        //        foreach (var v in entityInventoryItem)
        //        {
        //            InventoryItemTag InventoryItem = new InventoryItemTag();
        //            InventoryItem.InventoryItemId = (int)v.inventoryitem_id;
        //            InventoryItem.InventoryItemName = v.inventoryitem_name;
        //            InventoryItem.TagId = (int)v.tag_id;
        //            InventoryItem.TagName = v.tag_name;
        //            InventoryItemList.Add(InventoryItem);
        //        }
        //        response = Response<List<InventoryItemTag>>.Create(InventoryItemList);
        //    }

        //    return response;
        //}
        //public Response<bool> DeleteInventoryItemTag(int InventoryItemId, int TagId)
        //{
        //    Response<bool> response = null;
        //    using (var context = new entity.KuazooEntities())
        //    {
        //        var entityUser = from d in context.kzInventoryItemTags
        //                         where d.inventoryitem_id == InventoryItemId
        //                         && d.tag_id == TagId
        //                         select d;
        //        if (entityUser.Count() > 0)
        //        {
        //            context.DeleteObject(entityUser.First());
        //            //entityUser.First().last_action = "5";
        //            context.SaveChanges();
        //            response = Response<bool>.Create(true);
        //        }
        //        else
        //        {
        //            response = Response<bool>.Create(false);
        //        }
        //    }

        //    return response;
        //}

        public Response<List<Tag>> GetTagList()
        {
            Response<List<Tag>> response = null;
            List<Tag> tagList = new List<Tag>();
            using (var context = new entity.KuazooEntities())
            {
                var entitytag = from d in context.kzTags
                                where d.last_action != "5"
                                select d;
                foreach (var v in entitytag)
                {
                    Tag tag = new Tag();
                    tag.Name = v.name;
                    tag.TagId = v.id;
                    tagList.Add(tag);
                }
                response = Response<List<Tag>>.Create(tagList);
            }

            return response;
        }

        public Response<List<InventoryItemType>> GetInventoryItemTypeList()
        {
            Response<List<InventoryItemType>> response = null;
            List<InventoryItemType> typeList = new List<InventoryItemType>();
            using (var context = new entity.KuazooEntities())
            {
                var entitytag = from d in context.kzInventoryItemTypes
                                where d.last_action != "5"
                                select d;
                foreach (var v in entitytag)
                {
                    InventoryItemType type = new InventoryItemType();
                    type.InventoryItemTypeName = v.name;
                    type.InventoryItemTypeId = v.id;
                    typeList.Add(type);
                }
                response = Response<List<InventoryItemType>>.Create(typeList);
            }

            return response;
        }
        public Response<List<Prize>> GetPrizeList()
        {
            Response<List<Prize>> response = null;
            List<Prize> typeList = new List<Prize>();
            using (var context = new entity.KuazooEntities())
            {
                var entitytag = from d in context.kzPrizes
                                where d.last_action != "5"
                                select d;
                foreach (var v in entitytag)
                {
                    Prize type = new Prize();
                    type.PrizeId = v.id;
                    type.Name = v.name;
                    type.SponsorName = v.sponsor_name;
                    type.Price = (decimal)v.price;
                    type.Description = v.description;
                    type.Terms = v.terms;
                    type.Detail = v.detail;
                    typeList.Add(type);
                }
                response = Response<List<Prize>>.Create(typeList);
            }

            return response;
        }


        public Response<List<InventoryFeatured>> GetInventoryFeatured(DateTime today)
        {
            Response<List<InventoryFeatured>> response = null;
            List<InventoryFeatured> InventoryItemList = new List<InventoryFeatured>();
            using (var context = new entity.KuazooEntities())
            {
                var entityInventoryItem = from d in context.kzInventoryItems
                                          where d.kzPrize.last_action != "5" && d.kzPrize.expiry_date > today && today >= d.publish_date
                                          && d.flag == true && d.last_action != "5"
                                          &&d.featured == true
                                          orderby d.featured_seq ascending, d.last_created descending
                                          select d;
                foreach (var v in entityInventoryItem)
                {
                    InventoryFeatured InventoryItem = new InventoryFeatured();
                    InventoryItem.InventoryItemId = v.id;
                    InventoryItem.Name = v.name;
                    InventoryItem.Featured = (bool)v.featured;
                    InventoryItem.FeatureSeq = (int)v.featured_seq;
                    InventoryItem.FeaturedText = "";
                    if (v.featured_seq == 1 || v.featured_seq == 4)
                    {
                        if (v.featured_text != null)
                        {
                            InventoryItem.FeaturedText = v.featured_text;
                        }
                    }
                    InventoryItemList.Add(InventoryItem);
                }
                response = Response<List<InventoryFeatured>>.Create(InventoryItemList);
            }

            return response;
        }
        public Response<List<InventoryItem>> GetInventoryNonFeatured(DateTime today)
        {
            Response<List<InventoryItem>> response = null;
            List<InventoryItem> InventoryItemList = new List<InventoryItem>();
            using (var context = new entity.KuazooEntities())
            {
                var entityInventoryItem = from d in context.kzInventoryItems
                                          where d.kzPrize.last_action != "5" && d.kzPrize.expiry_date > today && today >= d.publish_date
                                          && d.flag == true && d.last_action != "5"
                                          && d.featured == false
                                          select d;
                foreach (var v in entityInventoryItem)
                {
                    InventoryItem inv = new InventoryItem();
                    inv.InventoryItemId = v.id;
                    inv.Name = v.name;
                    InventoryItemList.Add(inv);
                }
                response = Response<List<InventoryItem>>.Create(InventoryItemList);
            }

            return response;
        }
        public Response<bool> CreateFeaturedDeal(DateTime today,InventoryFeatured deal)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {

                var entityInventoryItem = from d in context.kzInventoryItems.Include("kzMerchant").Include("kzInventoryItemType")
                                          where d.kzPrize.last_action != "5" && d.kzPrize.expiry_date > today && today >= d.publish_date
                                            && d.flag == true && d.last_action != "5"
                                            && d.id== deal.InventoryItemId
                                            select d;
                if (entityInventoryItem.Count() > 0)
                {
                    if (deal.FeatureSeq >= 5)
                    {
                        entityInventoryItem.First().featured = deal.Featured;
                        entityInventoryItem.First().featured_seq = deal.FeatureSeq;
                    }
                    else
                    {
                        //update
                        if (entityInventoryItem.First().featured == true)
                        {
                            int seqchange = (int)entityInventoryItem.First().featured_seq;
                            var entity2 = from d in context.kzInventoryItems.Include("kzMerchant").Include("kzInventoryItemType")
                                          where d.kzPrize.last_action != "5" && d.kzPrize.expiry_date > today && today >= d.publish_date
                                            && d.flag == true && d.last_action != "5"
                                            && d.id != deal.InventoryItemId
                                            && d.featured== true && d.featured_seq == deal.FeatureSeq
                                            select d;
                            if (entity2.Count() > 0)
                            {
                                entity2.First().featured_seq = seqchange;
                                entityInventoryItem.First().featured_seq = deal.FeatureSeq;
                            }
                            else
                            {
                                entityInventoryItem.First().featured_seq = deal.FeatureSeq;
                            }
                        }
                        //new
                        else
                        {
                            var entity2 = from d in context.kzInventoryItems.Include("kzMerchant").Include("kzInventoryItemType")
                                          where d.kzPrize.last_action != "5" && d.kzPrize.expiry_date > today && today >= d.publish_date
                                            && d.flag == true && d.last_action != "5"
                                            && d.id != deal.InventoryItemId
                                            && d.featured == true && d.featured_seq == deal.FeatureSeq
                                            select d;
                            if (entity2.Count() > 0)
                            {
                                entity2.First().featured_seq = 5;
                            }
                            entityInventoryItem.First().featured = deal.Featured;
                            entityInventoryItem.First().featured_seq = deal.FeatureSeq;

                        }
                        //add text for big
                        if (deal.FeatureSeq == 1 || deal.FeatureSeq == 4)
                        {
                            entityInventoryItem.First().featured_text = deal.FeaturedText;
                        }
                    }
                    context.SaveChanges();
                    response = Response<bool>.Create(true);
                }
            }
            return response;
        }
        public Response<bool> DeleteInventoryItemFeature(int InventoryItemId)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                var entityUser = from d in context.kzInventoryItems
                                 where d.id == InventoryItemId
                                 select d;
                if (entityUser.Count() > 0)
                {
                    entityUser.First().featured = false;
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

        public void MemberNotifEmail(String MerchantName, int InventoryItem, String InventoryName, DateTime ExpireDateOld, DateTime ExpireDateNew)
        {
            try
            {

                using (var context = new entity.KuazooEntities())
                {
                    string to = "";
                    string body = "";
                    string subject = "Changed of expiry date of your purchases";
                    var entityMember = (from d in context.kzTransactionDetails
                                        where d.inventoryitem_id == InventoryItem
                                        select d.kzTransaction.kzUser.email).Distinct();
                    foreach (var v in entityMember)
                    {
                        if (to == "") to = v;
                        else to = to + "," + v;
                    }
                    body = "Item " + InventoryName + " from Merchant " + MerchantName + "<br/>" + "Expiry Date " + string.Format("{0:dd MMM yyyy}", ExpireDateOld) + " has changed to " + string.Format("{0:dd MMM yyyy}", ExpireDateNew);
                    new GeneralService().SendEmail(subject, to, body);
                }                     
            }
            catch (Exception ex)
            {

            }
        }
        public bool ShareEmail(int InventoryItemId, String EmailFrom, String EmailTo, String Firstname, String Lastname)
        {
            try
            {
                using (var context = new entity.KuazooEntities())
                {
                    string body = "";
                    string subject = "Great deal recommended by your friend : " + Firstname +" "+Lastname;
                    string feature = "";
                    string flashdeal = "";
                    string deal = "";
                    string urlflashdeal = WebSetting.Url + "/Home/FlashDeals";
                    string urldeal = WebSetting.Url;

                    #region feature
                    var entityfeature = GetInventoryItemById(InventoryItemId).Result;
                    if (entityfeature != null)
                    {
                        string urlfeature = WebSetting.Url + "/Home/Deals/" + kuazoo.helper.ReplaceSymbol.Replace(entityfeature.Name) + "?d=1";
                        string discountprice = @"";
                        if (entityfeature.FlashDeal == null)
                        {
                            if (entityfeature.Discount > 0)
                            {
                                decimal pricedis = entityfeature.Price - (entityfeature.Price * entityfeature.Discount / 100);
                                discountprice = @"<div class=""boxoffsmall"" style=""font-size: 15px;font-family: OpenSans,Verdana;color: #999999;text-decoration: line-through;"">RM " + entityfeature.Price.ToString("N2") + @"</div>
                                                    <div class=""boxoffbig"" style=""font-size: 34px;font-family: OpenSansSemiBold,Verdana;color: #0093c9;line-height:normal;margin-top:-6px"">RM " + pricedis.ToString("N0") + @"</div>";
                            }
                            else
                            {
                                discountprice = @"<div class=""boxoffbig"" style=""font-size: 34px;font-family: OpenSansSemiBold,Verdana;color: #0093c9;line-height:normal;margin-top:-6px"">RM " + entityfeature.Price.ToString("N2") + @"</div>";
                            }
                        }
                        else
                        {
                            if (entityfeature.FlashDeal.Discount > 0)
                            {
                                decimal pricedis = entityfeature.Price - (entityfeature.Price * entityfeature.FlashDeal.Discount / 100);
                                discountprice = @"<div class=""boxoffsmall"" style=""font-size: 15px;font-family: OpenSans,Verdana;color: #999999;text-decoration: line-through;"">RM " + entityfeature.Price.ToString("N2") + @"</div>
                                                    <div class=""boxoffbig"" style=""font-size: 34px;font-family: OpenSansSemiBold,Verdana;color: #0093c9;line-height:normal;margin-top:-6px"">RM " + pricedis.ToString("N0") + @"</div>";
                            }
                            else
                            {
                                discountprice = @"<div class=""boxoffbig"" style=""font-size: 34px;font-family: OpenSansSemiBold,Verdana;color: #0093c9;line-height:normal;margin-top:-6px"">RM " + entityfeature.Price.ToString("N2") + @"</div>";
                            }
                        }
                        if (entityfeature.InventoryItemType.InventoryItemTypeName.ToLower().Equals("raffle"))
                        {
                            feature = @" <a href=""" + urlfeature + @""">
                                        <div class=""dealbox"" style=""width:718px;border: 1px solid #cccccc;background: #fafafa;margin: 10px;float: left;-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;"">
                                            <div class=""boxraffle"" style=""height: 355px;width: 100%;display: table;background: url('http://hka.siggie.net:8123/kuazoo/content/img/rafflebg-big.png')no-repeat;background-size: 100% 100%;"">
                                                <div class=""boxrafflecontent"" style=""width:718px;display: table-cell;vertical-align: middle;padding-left: 5px;"">
                                                    <div style=""display:inline-table;width:271px"">
                                                    <div class=""raflleticket"" style=""background: url('http://hka.siggie.net:8123/kuazoo/content/img/raffleticket.png')no-repeat;background-size: 100% 100%;width: 263px;height: 133px;display: table-cell;vertical-align: middle;text-align: center;padding-left: 40px;font-family: OpenSansBold,Verdana;color: #432b11;font-size: 50px;"">RM " + entityfeature.Price + @"</div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class=""boxfooter"" style=""height: 98px;display: table;"">
                                                <div class=""boxoff"" style=""display: table-cell;text-align: center;min-width: 124px;vertical-align: middle;"">
                                                    <div class=""boxoffbig2""><span class=""boxoffmedium"">RM</span>" + entityfeature.Price + @"</div>
                                                </div>
                                                <div class=""boxprice"" style=""display: table-cell;vertical-align: middle;width: 100%;"">
                                                    <div>
                                                        <span class=""pricedisc"" style=""font-size: 20px;font-family: OpenSansSemiBold,Verdana;color: #0093c9;"">RAFFLE TICKET</span>
                                                    </div>
                                                    <div>
                                                        <div class=""boxbarbg"" style=""width: 96%;height: 10px;background: #cccccc;border-radius: 5px;""><div class=""boxbaractive"" style=""width:" + entityfeature.SalesVisualMeter + @"%;height: 10px;background: url('http://hka.siggie.net:8123/kuazoo/Content/img/boxbar559.jpg')no-repeat;background-size: 312px 100%;border-radius: 5px;""></div></div>
                                                    </div>
                                                    <div class=""boxtext"" style=""font-size: 16px;font-family: OpenSans,Verdana;color: #333333;"">" + entityfeature.RemainSales + @" raffle tickets remaining!</div>
                                                </div>
                                            </div>
                                        </div>
                                        </a>";
                        }
                        else if (entityfeature.SalesVisualMeter >= 100 || entityfeature.RemainSales < 1)
                        {
                            feature = @"<a href=""" + urlfeature + @""">
                                        <div class=""dealbox"" style=""width:718px;border: 1px solid #cccccc;background: #fafafa;margin: 10px;float: left;-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;"">
                                            <div class=""boxheader"" style=""height: 60px;display: table;padding-left: 5px;"">
                                                <div class=""boxheadertext"" style=""width:541px;font-size: 16px;font-family: OpenSansBold,Verdana;color: #333333;display: table-cell;vertical-align: middle;"">" + entityfeature.Name + @"</div>
                                            </div>
                                            <div class=""boxcontent"" style=""position: relative;height: 365px;overflow: hidden;"">
                                                <img src=""" + entityfeature.ImageUrl + @""" style=""width: 100%;position: absolute;top: 0;left: 0;"" />
                                            </div>
                                            <div class=""boxfooter"" style=""height: 98px;display: table;"">
                                                <div class=""boxoff"" style=""display: table-cell;text-align: center;min-width: 180px;vertical-align: middle;"">
                                                        " + discountprice + @"
                                                </div>
                                                <div class=""boxprice"" style=""display: table-cell;vertical-align: middle;width: 100%;"">
                                                    <div>
                                                        <div class=""boxdisc"" style=""font-size: 16px;font-family: OpenSans,Verdana;color: #333333;"">" + entityfeature.Discount.ToString("N0") + @"% OFF</div>
                                                    </div>
                                                    <div>
                                                        <div class=""boxbarbg"" style=""width: 96%;height: 10px;background: #cccccc;border-radius: 5px;""><div class=""boxbaractive"" style=""width:100%;height: 10px;background: url('http://hka.siggie.net:8123/kuazoo/Content/img/boxbar559.jpg')no-repeat;background-size: 312px 100%;border-radius: 5px;""></div></div>
                                                    </div>
                                                    <div class=""boxtext"" style=""font-size: 16px;font-family: OpenSans,Verdana;color: #333333;"">All deal sold!</div>
                                                </div>
                                            </div>
                                        </div>
                                        </a>";
                        }
                        else if (entityfeature.FlashDeal != null)
                        {
                            feature = @"<a href=""" + urlfeature + @""">
                                        <div class=""dealbox"" style=""width:718px;border: 1px solid #cccccc;background: #fafafa;margin: 10px;float: left;-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;"">
                                            <div class=""boxheader"" style=""height: 60px;display: table;padding-left: 5px;"">
                                                <div class=""boxheadertext"" style=""width:541px;font-size: 16px;font-family: OpenSansBold,Verdana;color: #333333;display: table-cell;vertical-align: middle;"">" + entityfeature.Name + @"</div>
                                            </div>
                                            <div class=""boxcontent"" style=""position: relative;height: 365px;overflow: hidden;"">
                                                <img src=""" + entityfeature.ImageUrl + @""" style=""width: 100%;position: absolute;top: 0;left: 0;"" />
                                            </div>
                                            <div class=""boxfooter"" style=""height: 98px;display: table;"">
                                                <div class=""boxoff"" style=""display: table-cell;text-align: center;min-width: 180px;vertical-align: middle;"">
                                                        " + discountprice + @"
                                                </div>
                                                <div class=""boxprice"" style=""display: table-cell;vertical-align: middle;width: 100%;"">
                                                    <div>
                                                        <div class=""boxdisc"" style=""font-size: 16px;font-family: OpenSans,Verdana;color: #333333;"">" + entityfeature.FlashDeal.Discount.ToString("N0") + @"% OFF</div>
                                                    </div>
                                                    <div>
                                                        <div class=""boxbarbg"" style=""width: 96%;height: 10px;background: #cccccc;border-radius: 5px;""><div class=""boxbaractive"" style=""width:" + entityfeature.SalesVisualMeter + @"%;height: 10px;background: url('http://hka.siggie.net:8123/kuazoo/Content/img/boxbar559.jpg')no-repeat;background-size: 312px 100%;border-radius: 5px;""></div></div>
                                                    </div>
                                                    <div class=""boxtext"" style=""font-size: 16px;font-family: OpenSans,Verdana;color: #333333;"">" + entityfeature.RemainSales + @" deals remaining!</div>
                                                </div>
                                            </div>
                                        </div>
                                        </a>";
                        }
                        else
                        {
                            feature = @"<a href=""" + urlfeature + @""">
                                        <div class=""dealbox"" style=""width:718px;border: 1px solid #cccccc;background: #fafafa;margin: 10px;float: left;-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;"">
                                            <div class=""boxheader"" style=""height: 60px;display: table;padding-left: 5px;"">
                                                <div class=""boxheadertext"" style=""width:541px;font-size: 16px;font-family: OpenSansBold,Verdana;color: #333333;display: table-cell;vertical-align: middle;"">" + entityfeature.Name + @"</div>
                                            </div>
                                            <div class=""boxcontent"" style=""position: relative;height: 365px;overflow: hidden;"">
                                                <img src=""" + entityfeature.ImageUrl + @""" style=""width: 100%;position: absolute;top: 0;left: 0;"" />
                                            </div>
                                            <div class=""boxfooter"" style=""height: 98px;display: table;"">
                                                <div class=""boxoff"" style=""display: table-cell;text-align: center;min-width: 180px;vertical-align: middle;"">
                                                        " + discountprice + @"
                                                </div>
                                                <div class=""boxprice"" style=""display: table-cell;vertical-align: middle;width: 100%;"">
                                                    <div>
                                                        <div class=""boxdisc"" style=""font-size: 16px;font-family: OpenSans,Verdana;color: #333333;"">" + entityfeature.Discount.ToString("N0") + @"% OFF</div>
                                                    </div>
                                                    <div>
                                                        <div class=""boxbarbg"" style=""width: 96%;height: 10px;background: #cccccc;border-radius: 5px;""><div class=""boxbaractive"" style=""width:" + entityfeature.SalesVisualMeter + @"%;height: 10px;background: url('http://hka.siggie.net:8123/kuazoo/Content/img/boxbar559.jpg')no-repeat;background-size: 312px 100%;border-radius: 5px;""></div></div>
                                                    </div>
                                                    <div class=""boxtext"" style=""font-size: 16px;font-family: OpenSans,Verdana;color: #333333;"">" + entityfeature.RemainSales + @" deals remaining!</div>
                                                </div>
                                            </div>
                                        </div>
                                        </a>";
                        }

                    }
                    #endregion
                    #region flashdeal
//                    var entityflashdeals = GetInventoryItemListByFlashDealTake(DateTime.UtcNow, 4).Result;
//                    string _urlflashdeal = "";
//                    foreach (var entityflashdeal in entityflashdeals)
//                    {
//                        _urlflashdeal = WebSetting.Url + "/Home/Deals/" + kuazoo.helper.ReplaceSymbol.Replace(entityflashdeal.Name) + "?d=2";
//                        string discountprice = @"";
//                        if (entityflashdeal.FlashDeal != null)
//                        {
//                            if (entityflashdeal.FlashDeal.Discount > 0)
//                            {
//                                decimal pricedis = entityflashdeal.Price - (entityflashdeal.Price * entityflashdeal.FlashDeal.Discount / 100);
//                                discountprice = @"<span class=""priceori"" style=""font-size: 16px;font-family: OpenSans,Verdana;color: #999999;text-decoration: line-through;"">RM " + entityflashdeal.Price + @"</span>
//                            <span class=""pricedisc"" style=""font-size: 20px;font-family: OpenSansSemiBold,Verdana;color: #0093c9;"">RM " + pricedis.ToString("N0") + @"</span>";
//                            }
//                            else
//                            {
//                                discountprice = @"<span class=""pricedisc"" style=""font-size: 20px;font-family: OpenSansSemiBold,Verdana;color: #0093c9;"">RM " + entityflashdeal.Prize + @"</span>";
//                            }
//                            flashdeal += @"<a href=""" + _urlflashdeal + @""">
//                                        <div class=""dealbox"" style=""border: 1px solid #cccccc;background: #fafafa;width: 348px;margin: 10px;float: left;-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;"">
//                                            <div class=""boxheader"" style=""height: 60px;display: table;padding-left: 5px;"">
//                                                <div class=""boxheadertext"" style=""font-size: 16px;font-family: OpenSansBold,Verdana;color: #333333;display: table-cell;vertical-align: middle;width: 191px;"">" + entityflashdeal.Name + @"</div>
//                                            </div>
//                                            <div class=""boxcontent"" style=""position: relative;height: 259px;max-height:259px;overflow: hidden;"">
//                                                <img src=""" + entityflashdeal.ImageUrl + @""" style=""width: 100%;position: absolute;top: 0;left: 0;"" />
//                                            </div>
//                                            <div class=""boxfooter"" style=""height: 98px;display: table;"">
//                                                <div class=""boxoff"" style=""display: table-cell;text-align: center;min-width: 124px;vertical-align: middle;"">
//                                                    <div class=""boxoffbig"" style=""font-size: 50px;font-family: OpenSansSemiBold,Verdana;color: #333333;line-height: normal;"">" + entityflashdeal.FlashDeal.Discount.ToString("N0") + @"%</div>
//                                                    <div  class=""boxoffsmall"" style=""font-size: 12px;font-family: OpenSans,Verdana;color: #333333;letter-spacing: 5px;padding-bottom: 10px;"">OFF</div>
//                                                </div>
//                                                <div class=""boxprice"" style=""display: table-cell;vertical-align: middle;width: 100%;"">
//                                                    <div>
//                                                        " + discountprice + @"
//                                                    </div>
//                                                    <div>
//                                                        <div class=""boxbarbg"" style=""width: 96%;height: 10px;background: #cccccc;border-radius: 5px;""><div class=""boxbaractive"" style=""width:" + entityflashdeal.SalesVisualMeter + @"%;height: 10px;background: url('http://hka.siggie.net:8123/kuazoo/Content/img/boxbar.jpg')no-repeat;background-size: 312px 100%;border-radius: 5px;""></div></div>
//                                                    </div>
//                                                    <div class=""boxtext"" style=""font-size: 16px;font-family: OpenSans,Verdana;color: #333333;"">" + entityflashdeal.Sales + @" deals remaining!</div>
//                                                </div>
//                                            </div>
//                                        </div>
//                                        </a>";
//                        }

//                    }
                    #endregion

                    #region deal
                    var entitydeals = GetInventoryItemListTake(DateTime.UtcNow, 4).Result;
                    string _urldeal = "";
                    foreach (var entitydeal in entitydeals)
                    {
                        _urldeal = WebSetting.Url + "/Home/Deals/" + kuazoo.helper.ReplaceSymbol.Replace(entitydeal.Name) + "?d=1";
                        string discountprice = @"";
                        if (entitydeal.FlashDeal == null)
                        {
                            if (entitydeal.Discount > 0)
                            {
                                decimal pricedis = entitydeal.Price - (entitydeal.Price * entitydeal.Discount / 100);
                                discountprice = @"<div class=""boxoffsmall"" style=""font-size: 15px;font-family: OpenSans,Verdana;color: #999999;text-decoration: line-through;"">RM " + entitydeal.Price.ToString("N2") + @"</div>
                                                    <div class=""boxoffbig"" style=""font-size: 34px;font-family: OpenSansSemiBold,Verdana;color: #0093c9;line-height:normal;margin-top:-6px"">RM " + pricedis.ToString("N0") + @"</div>";
                            }
                            else
                            {
                                discountprice = @"<div class=""boxoffbig"" style=""font-size: 34px;font-family: OpenSansSemiBold,Verdana;color: #0093c9;line-height:normal;margin-top:-6px"">RM " + entitydeal.Price.ToString("N2") + @"</div>";
                            }
                        }
                        else
                        {
                            if (entitydeal.FlashDeal.Discount > 0)
                            {
                                decimal pricedis = entitydeal.Price - (entitydeal.Price * entitydeal.FlashDeal.Discount / 100);
                                discountprice = @"<div class=""boxoffsmall"" style=""font-size: 15px;font-family: OpenSans,Verdana;color: #999999;text-decoration: line-through;"">RM " + entitydeal.Price.ToString("N2") + @"</div>
                                                    <div class=""boxoffbig"" style=""font-size: 34px;font-family: OpenSansSemiBold,Verdana;color: #0093c9;line-height:normal;margin-top:-6px"">RM " + pricedis.ToString("N0") + @"</div>";
                            }
                            else
                            {
                                discountprice = @"<div class=""boxoffbig"" style=""font-size: 34px;font-family: OpenSansSemiBold,Verdana;color: #0093c9;line-height:normal;margin-top:-6px"">RM " + entitydeal.Price.ToString("N2") + @"</div>";
                            }
                        }
                        if (entitydeal.TypeName.ToLower().Equals("raffle"))
                        {
                            deal += @" <a href=""" + _urldeal + @""">
                                        <div class=""dealbox"" style=""border: 1px solid #cccccc;background: #fafafa;width: 348px;margin: 10px;float: left;-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;"">
                                            <div class=""boxraffle"" style=""height: 355px;width: 100%;display: table;background: url('http://hka.siggie.net:8123/kuazoo/content/img/rafflebg.png')no-repeat;background-size: 100% 100%;"">
                                                <div class=""boxrafflecontent"" style=""width:191px;display: table-cell;vertical-align: middle;padding-left: 5px;"">
                                                    <div style=""display:inline-table;width:271px"">
                                                        <div class=""raflleticket"" style=""background: url('http://hka.siggie.net:8123/kuazoo/content/img/raffleticket.png')no-repeat;background-size: 100% 100%;width: 263px;height: 133px;display: table-cell;vertical-align: middle;text-align: center;padding-left: 40px;font-family: OpenSansBold,Verdana;color: #432b11;font-size: 50px;"">RM " + entitydeal.Price + @"</div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class=""boxfooter"" style=""height: 98px;display: table;"">
                                                <div class=""boxoff"" style=""display: table-cell;text-align: center;min-width: 124px;vertical-align: middle;"">
                                                    <div class=""boxoffbig2""><span class=""boxoffmedium"">RM</span>" + entitydeal.Price + @"</div>
                                                </div>
                                                <div class=""boxprice"" style=""display: table-cell;vertical-align: middle;width: 100%;"">
                                                    <div>
                                                        <span class=""pricedisc"" style=""font-size: 20px;font-family: OpenSansSemiBold,Verdana;color: #0093c9;"">RAFFLE TICKET</span>
                                                    </div>
                                                    <div>
                                                        <div class=""boxbarbg"" style=""width: 96%;height: 10px;background: #cccccc;border-radius: 5px;""><div class=""boxbaractive"" style=""width:" + entitydeal.SalesVisualMeter + @"%;height: 10px;background: url('http://hka.siggie.net:8123/kuazoo/Content/img/boxbar.jpg')no-repeat;background-size: 312px 100%;border-radius: 5px;""></div></div>
                                                    </div>
                                                    <div class=""boxtext"" style=""font-size: 16px;font-family: OpenSans,Verdana;color: #333333;"">" + entitydeal.Sales + @" raffle tickets remaining!</div>
                                                </div>
                                            </div>
                                        </div>
                                        </a>";
                        }
                        else if (entitydeal.SalesVisualMeter >= 100 || entitydeal.Sales < 1)
                        {
                            deal += @"<a href=""" + _urldeal + @""">
                                        <div class=""dealbox"" style=""border: 1px solid #cccccc;background: #fafafa;width: 348px;margin: 10px;float: left;-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;"">
                                            <div class=""boxheader"" style=""height: 60px;display: table;padding-left: 5px;"">
                                                <div class=""boxheadertext"" style=""font-size: 16px;font-family: OpenSansBold,Verdana;color: #333333;display: table-cell;vertical-align: middle;width: 191px;"">" + entitydeal.Name + @"</div>
                                            </div>
                                            <div class=""boxcontent"" style=""position: relative;height: 259px;max-height:259px;overflow: hidden;"">
                                                <img src=""" + entitydeal.ImageUrl + @"""style=""width: 100%;position: absolute;top: 0;left: 0;"" />
                                            </div>
                                            <div class=""boxfooter"" style=""height: 98px;display: table;"">
                                                <div class=""boxoff"" style=""display: table-cell;text-align: center;min-width: 180px;vertical-align: middle;"">
                                                        " + discountprice + @"
                                                </div>
                                                <div class=""boxprice"" style=""display: table-cell;vertical-align: middle;width: 100%;"">
                                                    <div>
                                                        <div class=""boxdisc"" style=""font-size: 16px;font-family: OpenSans,Verdana;color: #333333;"">" + entitydeal.Discount.ToString("N0") + @"% OFF</div>
                                                    </div>
                                                    <div>
                                                        <div class=""boxbarbg"" style=""width: 96%;height: 10px;background: #cccccc;border-radius: 5px;""><div class=""boxbaractive"" style=""width:100%;height: 10px;background: url('http://hka.siggie.net:8123/kuazoo/Content/img/boxbar.jpg')no-repeat;background-size: 312px 100%;border-radius: 5px;""></div></div>
                                                    </div>
                                                    <div class=""boxtext"" style=""font-size: 16px;font-family: OpenSans,Verdana;color: #333333;"">All deal sold!</div>
                                                </div>
                                            </div>
                                        </div>
                                        </a>";
                        }
                        else if (entitydeal.FlashDeal != null)
                        {
                            deal += @"<a href=""" + _urldeal + @""">
                                        <div class=""dealbox"" style=""border: 1px solid #cccccc;background: #fafafa;width: 348px;margin: 10px;float: left;-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;"">
                                            <div class=""boxheader"" style=""height: 60px;display: table;padding-left: 5px;"">
                                                <div class=""boxheadertext"" style=""font-size: 16px;font-family: OpenSansBold,Verdana;color: #333333;display: table-cell;vertical-align: middle;width: 191px;"">" + entitydeal.Name + @"</div>
                                            </div>
                                            <div class=""boxcontent"" style=""position: relative;height: 259px;max-height:259px;overflow: hidden;"">
                                                <img src=""" + entitydeal.ImageUrl + @""" style=""width: 100%;position: absolute;top: 0;left: 0;""/>
                                            </div>
                                            <div class=""boxfooter"" style=""height: 98px;display: table;"">
                                                <div class=""boxoff"" style=""display: table-cell;text-align: center;min-width: 180px;vertical-align: middle;"">
                                                        " + discountprice + @"
                                                </div>
                                                <div class=""boxprice"" style=""display: table-cell;vertical-align: middle;width: 100%;"">
                                                    <div>
                                                        <div class=""boxdisc"" style=""font-size: 16px;font-family: OpenSans,Verdana;color: #333333;"">" + entitydeal.FlashDeal.Discount.ToString("N0") + @"% OFF</div>
                                                    </div>
                                                    <div>
                                                        <div class=""boxbarbg"" style=""width: 96%;height: 10px;background: #cccccc;border-radius: 5px;""><div class=""boxbaractive"" style=""width:" + entitydeal.SalesVisualMeter + @"%;height: 10px;background: url('http://hka.siggie.net:8123/kuazoo/Content/img/boxbar.jpg')no-repeat;background-size: 312px 100%;border-radius: 5px;""></div></div>
                                                    </div>
                                                    <div class=""boxtext"" style=""font-size: 16px;font-family: OpenSans,Verdana;color: #333333;"">" + entitydeal.Sales + @" deals remaining!</div>
                                                </div>
                                            </div>
                                        </div>
                                        </a>";
                        }
                        else
                        {
                            deal += @"<a href=""" + _urldeal + @""">
                                        <div class=""dealbox"" style=""border: 1px solid #cccccc;background: #fafafa;width: 348px;margin: 10px;float: left;-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;"">
                                            <div class=""boxheader"" style=""height: 60px;display: table;padding-left: 5px;"">
                                                <div class=""boxheadertext"" style=""font-size: 16px;font-family: OpenSansBold,Verdana;color: #333333;display: table-cell;vertical-align: middle;width: 191px;"">" + entitydeal.Name + @"</div>
                                            </div>
                                            <div class=""boxcontent"" style=""position: relative;height: 259px;max-height:259px;overflow: hidden;"">
                                                <img src=""" + entitydeal.ImageUrl + @""" style=""width: 100%;position: absolute;top: 0;left: 0;"" />
                                            </div>
                                            <div class=""boxfooter"" style=""height: 98px;display: table;"">
                                                <div class=""boxoff"" style=""display: table-cell;text-align: center;min-width: 180px;vertical-align: middle;"">
                                                        " + discountprice + @"
                                                </div>
                                                <div class=""boxprice"" style=""display: table-cell;vertical-align: middle;width: 100%;"">
                                                    <div>
                                                        <div class=""boxdisc"" style=""font-size: 16px;font-family: OpenSans,Verdana;color: #333333;"">" + entitydeal.Discount.ToString("N0") + @"% OFF</div>
                                                    </div>
                                                    <div>
                                                        <div class=""boxbarbg"" style=""width: 96%;height: 10px;background: #cccccc;border-radius: 5px;""><div class=""boxbaractive"" style=""width:" + entitydeal.SalesVisualMeter + @"%;height: 10px;background: url('http://hka.siggie.net:8123/kuazoo/Content/img/boxbar.jpg')no-repeat;background-size: 312px 100%;border-radius: 5px;""></div></div>
                                                    </div>
                                                    <div class=""boxtext"" style=""font-size: 16px;font-family: OpenSans,Verdana;color: #333333;"">" + entitydeal.Sales + @" deals remaining!</div>
                                                </div>
                                            </div>
                                        </div>
                                        </a>";
                        }
                    }
                    #endregion
                    body = helper.Email.CreateShareEmail(feature, flashdeal, deal, urlflashdeal, urldeal);

                    new GeneralService().SendEmail(subject, EmailTo, body, "");
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public Response<int> CheckFreeDeal(int id, bool login)
        {
            Response<int> response = null;
            using (var context = new entity.KuazooEntities())
            {
                var entityDeal = from d in context.kzInventoryItems
                                 where d.kzPrize.freedeal == true
                                 && d.id == id
                                 select d;
                if (entityDeal.Count() > 0)// is freedeal
                {
                    if (login)
                    {
                        var user = UserService.GetCurrentUserStatic().Result;
                        if (user.UserId != 0)
                        {

                            var entityTransaction = from d in context.kzTransactionFreeDeals
                                                    where d.user_id == user.UserId
                                                    && d.inventoryitem_id == id
                                                    select d;
                            if (entityTransaction.Count() == 0)
                            {
                                response = Response<int>.Create(1);//free deal 
                            }
                            else
                            {
                                response = Response<int>.Create(2);//free deal ald used
                            }
                        }
                        else//not login
                        {
                            response = Response<int>.Create(3);
                        }
                    }
                    else//not login
                    {
                        response = Response<int>.Create(3);
                    }

                }
                else//not freedeal
                {
                    response = Response<int>.Create(0);
                }
            }
            return response;
        }
//        public bool ShareEmail(int InventoryItemId, String EmailFrom, String EmailTo)
//        {
//            try
//            {
//                using (var context = new entity.KuazooEntities())
//                {
//                    string body = "";
//                    string subject = "Great deal recommended by your friend : "+EmailFrom;
//                    string feature = "";
//                    string flashdeal = "";
//                    string deal = "";
//                    string urlflashdeal = WebSetting.Url+"/Home/FlashDeals";
//                    string urldeal =WebSetting.Url;

//                    #region feature
//                    var entityfeature = GetInventoryItemById(InventoryItemId).Result;
//                    if (entityfeature != null)
//                    {
//                        string urlfeature = WebSetting.Url + "/Home/Deals/" + entityfeature.Name.Replace(" ", "-") + "?d=1";
//                        string discountprice=@"";
//                        if (entityfeature.FlashDeal == null)
//                        {
//                            if (entityfeature.Discount > 0)
//                            {
//                                decimal pricedis = entityfeature.Price - (entityfeature.Price * entityfeature.Discount / 100);
//                                discountprice = @"<span class=""priceori"" style=""font-size: 16px;font-family: OpenSans,Verdana;color: #999999;text-decoration: line-through;"">RM " + entityfeature.Prize + @"</span>
//                            <span class=""pricedisc"" style=""font-size: 20px;font-family: OpenSansSemiBold,Verdana;color: #0093c9;"">RM " + pricedis + @"</span>";
//                            }
//                            else
//                            {
//                                discountprice = @"<span class=""pricedisc"" style=""font-size: 20px;font-family: OpenSansSemiBold,Verdana;color: #0093c9;"">RM " + entityfeature.Prize + @"</span>";
//                            }
//                        }
//                        else
//                        {
//                            if (entityfeature.FlashDeal.Discount > 0)
//                            {
//                                decimal pricedis = entityfeature.Price - (entityfeature.Price * entityfeature.FlashDeal.Discount / 100);
//                                discountprice = @"<span class=""priceori"" style=""font-size: 16px;font-family: OpenSans,Verdana;color: #999999;text-decoration: line-through;"">RM " + entityfeature.Prize + @"</span>
//                            <span class=""pricedisc"" style=""font-size: 20px;font-family: OpenSansSemiBold,Verdana;color: #0093c9;"">RM " + pricedis + @"</span>";
//                            }
//                            else
//                            {
//                                discountprice = @"<span class=""pricedisc"" style=""font-size: 20px;font-family: OpenSansSemiBold,Verdana;color: #0093c9;"">RM " + entityfeature.Prize + @"</span>";
//                            }
//                        }
//                        if (entityfeature.InventoryItemType.InventoryItemTypeName.ToLower().Equals("raffle"))
//                        {
//                            feature = @" <a href="""+urlfeature+ @""">
//                                        <div class=""dealbox"" style=""width:718px;border: 1px solid #cccccc;background: #fafafa;margin: 10px;float: left;-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;"">
//                                            <div class=""boxraffle"" style=""height: 355px;width: 100%;display: table;background: url('http://hka.siggie.net:8123/kuazoo/content/img/rafflebg.png')no-repeat;background-size: 100% 100%;"">
//                                                <div class=""boxrafflecontent"" style=""width:541px;display: table-cell;vertical-align: middle;padding-left: 5px;"">
//                                                    <div class=""raflleticket"" style=""background: url('http://hka.siggie.net:8123/kuazoo/content/img/raffleticket.png')no-repeat;background-size: 100% 100%;width: 263px;height: 133px;display: table-cell;vertical-align: middle;text-align: center;padding-left: 40px;font-family: OpenSansBold,Verdana;color: #432b11;font-size: 50px;"">RM " + entityfeature.Price + @"</div>
//                                                </div>
//                                                <div class=""boxprize"" style=""width: 150px;position: relative;-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;"">
//                                                    <div class=""boxprizeribbon"" style=""-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;width: 130px;height: 244px;position: absolute;top: -2px;left: 10px;background: white;-webkit-filter: drop-shadow(0 3px 5px rgba(0,0,0,.2));-moz-filter: drop-shadow(0 3px 5px rgba(0,0,0,.2));-ms-filter: drop-shadow(0 3px 5px rgba(0,0,0,.2));-o-filter: drop-shadow(0 3px 5px rgba(0,0,0,.2));filter: drop-shadow(0 3px 5px rgba(0,0,0,.2));z-index: 10;text-align: center;padding: 10px;"">
//                                                        <div class=""boxprizetitle"" style=""font-size: 14px;font-family: OpenSansBold,Verdana;color: #d60c7d;padding-top: 10px;padding-bottom: 20px;"">Sur'PRIZE'</div>
//                                                        <div class=""boxprizeimg"" style=""height: 110px;position: relative;overflow: hidden;"">
//                                                            <img src=""" +entityfeature.Prize.ImageUrl+@""" />
//                                                        </div>
//                                                        <div class=""boxprizedetail"" style=""font-size: 14px;font-family: OpenSans,Verdana;color: #333333;text-align: center;"">"+entityfeature.Prize.Name+@"</div> <div style=""border-left: 65px solid transparent;border-top: 50px solid #ffffff;border-right: 65px solid transparent;position: absolute;top: 244px;left: 0px;z-index: -1;""></div>
//                                                    </div>
//                                                </div>
//                                            </div>
//                                            <div class=""boxfooter"" style=""height: 98px;display: table;"">
//                                                <div class=""boxoff"" style=""display: table-cell;text-align: center;min-width: 124px;vertical-align: middle;"">
//                                                    <div class=""boxoffbig2""><span class=""boxoffmedium"">RM</span>"+entityfeature.Price+@"</div>
//                                                </div>
//                                                <div class=""boxprice"" style=""display: table-cell;vertical-align: middle;width: 100%;"">
//                                                    <div>
//                                                        <span class=""pricedisc"" style=""font-size: 20px;font-family: OpenSansSemiBold,Verdana;color: #0093c9;"">RAFFLE TICKET</span>
//                                                    </div>
//                                                    <div>
//                                                        <div class=""boxbarbg"" style=""width: 96%;height: 10px;background: #cccccc;border-radius: 5px;""><div class=""boxbaractive"" style=""width:"+entityfeature.SalesVisualMeter+@"%;height: 10px;background: url('http://hka.siggie.net:8123/kuazoo/Content/img/boxbar559.jpg')no-repeat;background-size: 312px 100%;border-radius: 5px;""></div></div>
//                                                    </div>
//                                                    <div class=""boxtext"" style=""font-size: 16px;font-family: OpenSans,Verdana;color: #333333;"">"+entityfeature.RemainSales+@" raffle tickets remaining!</div>
//                                                </div>
//                                            </div>
//                                        </div>
//                                        </a>";
//                        }
//                        else if (entityfeature.SalesVisualMeter >= 100 || entityfeature.RemainSales < 1)
//                        {
//                            feature = @"<a href=""" + urlfeature + @""">
//                                        <div class=""dealbox"" style=""width:718px;border: 1px solid #cccccc;background: #fafafa;margin: 10px;float: left;-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;"">
//                                            <div class=""boxheader"" style=""height: 60px;display: table;padding-left: 5px;"">
//                                                <div class=""boxheadertext"" style=""width:541px;font-size: 16px;font-family: OpenSansBold,Verdana;color: #333333;display: table-cell;vertical-align: middle;"">" + entityfeature.Name + @"</div>
//                                                <div class=""boxprize"" style=""width: 150px;position: relative;-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;"">
//                                                    <div class=""boxprizeribbon"" style=""-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;width: 130px;height: 244px;position: absolute;top: -2px;left: 10px;background: white;-webkit-filter: drop-shadow(0 3px 5px rgba(0,0,0,.2));-moz-filter: drop-shadow(0 3px 5px rgba(0,0,0,.2));-ms-filter: drop-shadow(0 3px 5px rgba(0,0,0,.2));-o-filter: drop-shadow(0 3px 5px rgba(0,0,0,.2));filter: drop-shadow(0 3px 5px rgba(0,0,0,.2));z-index: 10;text-align: center;padding: 10px;"">
//                                                        <div class=""boxprizetitle"" style=""font-size: 14px;font-family: OpenSansBold,Verdana;color: #d60c7d;padding-top: 10px;padding-bottom: 20px;"">Sur'PRIZE'</div>
//                                                        <div class=""boxprizeimg"" style=""height: 110px;position: relative;overflow: hidden;"">
//                                                            <img src=""" +entityfeature.Prize.ImageUrl+@""" style=""width: 100%;position: absolute;top: 0;left: 0;bottom: 0;margin: auto;""/>
//                                                        </div>
//                                                        <div class=""boxprizedetail"" style=""font-size: 14px;font-family: OpenSans,Verdana;color: #333333;text-align: center;"">"+entityfeature.Prize.Name+@"</div> <div style=""border-left: 65px solid transparent;border-top: 50px solid #ffffff;border-right: 65px solid transparent;position: absolute;top: 244px;left: 0px;z-index: -1;""></div>
//                                                    </div>
//                                                </div>
//                                            </div>
//                                            <div class=""boxcontent"" style=""position: relative;height: 259px;max-height:259px;overflow: hidden;"">
//                                                <div class=""boxkuazoo"" style=""position: absolute;z-index: 1;height: 100%;width: 100%;display: table"">
//                                                    <div class=""boxkuazoobg"" style=""position: absolute;z-index: 1;background: #0093c9;height: 100%;width: 100%;opacity: 0.85;""></div>
//                                                    <div class=""boxcontentwrap"" style=""display: table-cell;vertical-align: middle;text-align: center;"">
//                                                        <div class=""boxTitle"" style=""z-index: 2;position: relative;font-size: 44px;color: white;font-family: OpenSansBold,Verdana;"">KUAZOO!</div>
//                                                        <div class=""boxSubtitle"" style=""z-index: 2;position: relative;font-size: 12px;color: white;font-family: OpenSansSemiBold,Verdana;letter-spacing: 4px;"">DRAW IS ON!</div>
//                                                    </div>
//                                                    <div class=""boxcontentprizetemp"" style=""width: 170px;""></div>
//                                                </div>
//                                                <img src="""+entityfeature.ImageUrl+@""" style=""width: 100%;position: absolute;top: 0;left: 0;"" />
//                                            </div>
//                                            <div class=""boxfooter"" style=""height: 98px;display: table;"">
//                                                <div class=""boxoff"" style=""display: table-cell;text-align: center;min-width: 124px;vertical-align: middle;"">
//                                                    <div class=""boxoffbig"" style=""font-size: 50px;font-family: OpenSansSemiBold,Verdana;color: #333333;line-height: normal;"">"+entityfeature.Discount+@"%</div>
//                                                    <div  class=""boxoffsmall"" style=""font-size: 12px;font-family: OpenSans,Verdana;color: #333333;letter-spacing: 5px;padding-bottom: 10px;"">OFF</div>
//                                                </div>
//                                                <div class=""boxprice"" style=""display: table-cell;vertical-align: middle;width: 100%;"">
//                                                    <div>
//                                                        "+discountprice+ @"
//                                                    </div>
//                                                    <div>
//                                                        <div class=""boxbarbg"" style=""width: 96%;height: 10px;background: #cccccc;border-radius: 5px;""><div class=""boxbaractive"" style=""width:100%;height: 10px;background: url('http://hka.siggie.net:8123/kuazoo/Content/img/boxbar559.jpg')no-repeat;background-size: 312px 100%;border-radius: 5px;""></div></div>
//                                                    </div>
//                                                    <div class=""boxtext"" style=""font-size: 16px;font-family: OpenSans,Verdana;color: #333333;"">All deal sold!</div>
//                                                </div>
//                                            </div>
//                                        </div>
//                                        </a>";
//                        }
//                        else if (entityfeature.FlashDeal != null)
//                        {
//                            feature = @"<a href=""" + urlfeature + @""">
//                                        <div class=""dealbox"" style=""width:718px;border: 1px solid #cccccc;background: #fafafa;margin: 10px;float: left;-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;"">
//                                            <div class=""boxheader"" style=""height: 60px;display: table;padding-left: 5px;"">
//                                                <div class=""boxheadertext"" style=""width:541px;font-size: 16px;font-family: OpenSansBold,Verdana;color: #333333;display: table-cell;vertical-align: middle;"">" + entityfeature.Name + @"</div>
//                                                <div class=""boxprize"" style=""width: 150px;position: relative;-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;"">
//                                                    <div class=""boxprizeribbon"" style=""-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;width: 130px;height: 244px;position: absolute;top: -2px;left: 10px;background: white;-webkit-filter: drop-shadow(0 3px 5px rgba(0,0,0,.2));-moz-filter: drop-shadow(0 3px 5px rgba(0,0,0,.2));-ms-filter: drop-shadow(0 3px 5px rgba(0,0,0,.2));-o-filter: drop-shadow(0 3px 5px rgba(0,0,0,.2));filter: drop-shadow(0 3px 5px rgba(0,0,0,.2));z-index: 10;text-align: center;padding: 10px;"">
//                                                        <div class=""boxprizetitle"" style=""font-size: 14px;font-family: OpenSansBold,Verdana;color: #d60c7d;padding-top: 10px;padding-bottom: 20px;"">Sur'PRIZE'</div>
//                                                        <div class=""boxprizeimg"" style=""height: 110px;position: relative;overflow: hidden;"">
//                                                            <img src=""" +entityfeature.Prize.ImageUrl+@""" style=""width: 100%;position: absolute;top: 0;left: 0;bottom: 0;margin: auto;""/>
//                                                        </div>
//                                                        <div class=""boxprizedetail"" style=""font-size: 14px;font-family: OpenSans,Verdana;color: #333333;text-align: center;"">"+entityfeature.Prize.Name+@"</div> <div style=""border-left: 65px solid transparent;border-top: 50px solid #ffffff;border-right: 65px solid transparent;position: absolute;top: 244px;left: 0px;z-index: -1;""></div>
//                                                    </div>
//                                                </div>
//                                            </div>
//                                            <div class=""boxcontent"" style=""position: relative;height: 259px;max-height:259px;overflow: hidden;"">
//                                                <img src="""+entityfeature.ImageUrl+@""" style=""width: 100%;position: absolute;top: 0;left: 0;"" />
//                                            </div>
//                                            <div class=""boxfooter"" style=""height: 98px;display: table;"">
//                                                <div class=""boxoff"" style=""display: table-cell;text-align: center;min-width: 124px;vertical-align: middle;"">
//                                                    <div class=""boxoffbig"" style=""font-size: 50px;font-family: OpenSansSemiBold,Verdana;color: #333333;line-height: normal;"">"+entityfeature.FlashDeal.Discount+@"%</div>
//                                                    <div  class=""boxoffsmall"" style=""font-size: 12px;font-family: OpenSans,Verdana;color: #333333;letter-spacing: 5px;padding-bottom: 10px;"">OFF</div>
//                                                </div>
//                                                <div class=""boxprice"" style=""display: table-cell;vertical-align: middle;width: 100%;"">
//                                                    <div>
//                                                        " + discountprice + @"
//                                                    </div>
//                                                    <div>
//                                                        <div class=""boxbarbg"" style=""width: 96%;height: 10px;background: #cccccc;border-radius: 5px;""><div class=""boxbaractive"" style=""width:" + entityfeature.SalesVisualMeter + @"%;height: 10px;background: url('http://hka.siggie.net:8123/kuazoo/Content/img/boxbar559.jpg')no-repeat;background-size: 312px 100%;border-radius: 5px;""></div></div>
//                                                    </div>
//                                                    <div class=""boxtext"" style=""font-size: 16px;font-family: OpenSans,Verdana;color: #333333;"">" + entityfeature.RemainSales + @" deals remaining!</div>
//                                                </div>
//                                            </div>
//                                        </div>
//                                        </a>";
//                        }
//                        else
//                        {
//                            feature = @"<a href=""" + urlfeature + @""">
//                                        <div class=""dealbox"" style=""width:718px;border: 1px solid #cccccc;background: #fafafa;margin: 10px;float: left;-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;"">
//                                            <div class=""boxheader"" style=""height: 60px;display: table;padding-left: 5px;"">
//                                                <div class=""boxheadertext"" style=""width:541px;font-size: 16px;font-family: OpenSansBold,Verdana;color: #333333;display: table-cell;vertical-align: middle;"">" + entityfeature.Name + @"</div>
//                                                <div class=""boxprize"" style=""width: 150px;position: relative;-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;"">
//                                                    <div class=""boxprizeribbon"" style=""-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;width: 130px;height: 244px;position: absolute;top: -2px;left: 10px;background: white;-webkit-filter: drop-shadow(0 3px 5px rgba(0,0,0,.2));-moz-filter: drop-shadow(0 3px 5px rgba(0,0,0,.2));-ms-filter: drop-shadow(0 3px 5px rgba(0,0,0,.2));-o-filter: drop-shadow(0 3px 5px rgba(0,0,0,.2));filter: drop-shadow(0 3px 5px rgba(0,0,0,.2));z-index: 10;text-align: center;padding: 10px;"">
//                                                        <div class=""boxprizetitle"" style=""font-size: 14px;font-family: OpenSansBold,Verdana;color: #d60c7d;padding-top: 10px;padding-bottom: 20px;"">Sur'PRIZE'</div>
//                                                        <div class=""boxprizeimg"" style=""height: 110px;position: relative;overflow: hidden;"">
//                                                            <img src=""" +entityfeature.Prize.ImageUrl+@""" style=""width: 100%;position: absolute;top: 0;left: 0;bottom: 0;margin: auto;""/>
//                                                        </div>
//                                                        <div class=""boxprizedetail"" style=""font-size: 14px;font-family: OpenSans,Verdana;color: #333333;text-align: center;"">"+entityfeature.Prize.Name+@"</div> <div style=""border-left: 65px solid transparent;border-top: 50px solid #ffffff;border-right: 65px solid transparent;position: absolute;top: 244px;left: 0px;z-index: -1;""></div>
//                                                    </div>
//                                                </div>
//                                            </div>
//                                            <div class=""boxcontent"" style=""position: relative;height: 259px;max-height:259px;overflow: hidden;"">
//                                                <img src="""+entityfeature.ImageUrl+@""" style=""width: 100%;position: absolute;top: 0;left: 0;"" />
//                                            </div>
//                                            <div class=""boxfooter"" style=""height: 98px;display: table;"">
//                                                <div class=""boxoff"" style=""display: table-cell;text-align: center;min-width: 124px;vertical-align: middle;"">
//                                                    <div class=""boxoffbig"" style=""font-size: 50px;font-family: OpenSansSemiBold,Verdana;color: #333333;line-height: normal;"">"+entityfeature.Discount+@"%</div>
//                                                    <div  class=""boxoffsmall"" style=""font-size: 12px;font-family: OpenSans,Verdana;color: #333333;letter-spacing: 5px;padding-bottom: 10px;"">OFF</div>
//                                                </div>
//                                                <div class=""boxprice"" style=""display: table-cell;vertical-align: middle;width: 100%;"">
//                                                    <div>
//                                                        
//                                                        " + discountprice + @"
//                                                    </div>
//                                                    <div>
//                                                        <div class=""boxbarbg"" style=""width: 96%;height: 10px;background: #cccccc;border-radius: 5px;""><div class=""boxbaractive"" style=""width:" + entityfeature.SalesVisualMeter + @"%;height: 10px;background: url('http://hka.siggie.net:8123/kuazoo/Content/img/boxbar559.jpg')no-repeat;background-size: 312px 100%;border-radius: 5px;""></div></div>
//                                                    </div>
//                                                    <div class=""boxtext"" style=""font-size: 16px;font-family: OpenSans,Verdana;color: #333333;"">" +entityfeature.RemainSales+@" deals remaining!</div>
//                                                </div>
//                                            </div>
//                                        </div>
//                                        </a>";
//                        }

//                    }
//                    #endregion
//                    #region flashdeal
//                    var entityflashdeals = GetInventoryItemListByFlashDealTake(DateTime.UtcNow,4).Result;
//                    string _urlflashdeal = "";
//                    foreach (var entityflashdeal in entityflashdeals) 
//                    {
//                        _urlflashdeal = WebSetting.Url + "/Home/Deals/" + entityflashdeal.Name.Replace(" ", "-") + "?d=2";
//                        string discountprice = @"";
//                        if (entityflashdeal.FlashDeal != null)
//                        {
//                            if (entityflashdeal.FlashDeal.Discount > 0)
//                            {
//                                decimal pricedis = entityflashdeal.Price - (entityflashdeal.Price * entityflashdeal.FlashDeal.Discount / 100);
//                                discountprice = @"<span class=""priceori"" style=""font-size: 16px;font-family: OpenSans,Verdana;color: #999999;text-decoration: line-through;"">RM " + entityflashdeal.Prize + @"</span>
//                            <span class=""pricedisc"" style=""font-size: 20px;font-family: OpenSansSemiBold,Verdana;color: #0093c9;"">RM " + pricedis + @"</span>";
//                            }
//                            else
//                            {
//                                discountprice = @"<span class=""pricedisc"" style=""font-size: 20px;font-family: OpenSansSemiBold,Verdana;color: #0093c9;"">RM " + entityflashdeal.Prize + @"</span>";
//                            }
//                            flashdeal += @"<a href=""" + _urlflashdeal + @""">
//                                        <div class=""dealbox"" style=""border: 1px solid #cccccc;background: #fafafa;width: 348px;margin: 10px;float: left;-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;"">
//                                            <div class=""boxheader"" style=""height: 60px;display: table;padding-left: 5px;"">
//                                                <div class=""boxheadertext"" style=""font-size: 16px;font-family: OpenSansBold,Verdana;color: #333333;display: table-cell;vertical-align: middle;width: 191px;"">" + entityflashdeal.Name + @"</div>
//                                                <div class=""boxprize"" style=""width: 150px;position: relative;-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;"">
//                                                    <div class=""boxprizeribbon"" style=""-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;width: 130px;height: 244px;position: absolute;top: -2px;left: 10px;background: white;-webkit-filter: drop-shadow(0 3px 5px rgba(0,0,0,.2));-moz-filter: drop-shadow(0 3px 5px rgba(0,0,0,.2));-ms-filter: drop-shadow(0 3px 5px rgba(0,0,0,.2));-o-filter: drop-shadow(0 3px 5px rgba(0,0,0,.2));filter: drop-shadow(0 3px 5px rgba(0,0,0,.2));z-index: 10;text-align: center;padding: 10px;"">
//                                                        <div class=""boxprizetitle"" style=""font-size: 14px;font-family: OpenSansBold,Verdana;color: #d60c7d;padding-top: 10px;padding-bottom: 20px;"">Sur'PRIZE'</div>
//                                                        <div class=""boxprizeimg"" style=""height: 110px;position: relative;overflow: hidden;"">
//                                                            <img src=""" + entityflashdeal.Prize.ImageUrl + @""" style=""width: 100%;position: absolute;top: 0;left: 0;bottom: 0;margin: auto;"" />
//                                                        </div>
//                                                        <div class=""boxprizedetail"" style=""font-size: 14px;font-family: OpenSans,Verdana;color: #333333;text-align: center;"">" + entityflashdeal.Prize.Name + @"</div><div style=""border-left: 65px solid transparent;border-top: 50px solid #ffffff;border-right: 65px solid transparent;position: absolute;top: 244px;left: 0px;z-index: -1;""></div>
//                                                    </div>
//                                                </div>
//                                            </div>
//                                            <div class=""boxcontent"" style=""position: relative;height: 259px;max-height:259px;overflow: hidden;"">
//                                                <img src=""" + entityflashdeal.ImageUrl + @""" style=""width: 100%;position: absolute;top: 0;left: 0;"" />
//                                            </div>
//                                            <div class=""boxfooter"" style=""height: 98px;display: table;"">
//                                                <div class=""boxoff"" style=""display: table-cell;text-align: center;min-width: 124px;vertical-align: middle;"">
//                                                    <div class=""boxoffbig"" style=""font-size: 50px;font-family: OpenSansSemiBold,Verdana;color: #333333;line-height: normal;"">" + entityflashdeal.FlashDeal.Discount + @"%</div>
//                                                    <div  class=""boxoffsmall"" style=""font-size: 12px;font-family: OpenSans,Verdana;color: #333333;letter-spacing: 5px;padding-bottom: 10px;"">OFF</div>
//                                                </div>
//                                                <div class=""boxprice"" style=""display: table-cell;vertical-align: middle;width: 100%;"">
//                                                    <div>
//                                                        " + discountprice + @"
//                                                    </div>
//                                                    <div>
//                                                        <div class=""boxbarbg"" style=""width: 96%;height: 10px;background: #cccccc;border-radius: 5px;""><div class=""boxbaractive"" style=""width:" + entityflashdeal.SalesVisualMeter + @"%;height: 10px;background: url('http://hka.siggie.net:8123/kuazoo/Content/img/boxbar.jpg')no-repeat;background-size: 312px 100%;border-radius: 5px;""></div></div>
//                                                    </div>
//                                                    <div class=""boxtext"" style=""font-size: 16px;font-family: OpenSans,Verdana;color: #333333;"">" + entityflashdeal.Sales + @" deals remaining!</div>
//                                                </div>
//                                            </div>
//                                        </div>
//                                        </a>";
//                        }

//                    }
//                    #endregion

//                    #region deal
//                    var entitydeals = GetInventoryItemListTake(DateTime.UtcNow, 4).Result;
//                    string _urldeal = "";
//                    foreach (var entitydeal in entitydeals)
//                    {
//                        _urldeal = WebSetting.Url + "/Home/Deals/" + entitydeal.Name.Replace(" ", "-") + "?d=1";
//                        string discountprice = @"";
//                        if (entitydeal.FlashDeal == null)
//                        {
//                            if (entitydeal.Discount > 0)
//                            {
//                                decimal pricedis = entitydeal.Price - (entitydeal.Price * entitydeal.Discount / 100);
//                                discountprice = @"<span class=""priceori"" style=""font-size: 16px;font-family: OpenSans,Verdana;color: #999999;text-decoration: line-through;"">RM " + entitydeal.Prize + @"</span>
//                            <span class=""pricedisc"" style=""font-size: 20px;font-family: OpenSansSemiBold,Verdana;color: #0093c9;"">RM " + pricedis + @"</span>";
//                            }
//                            else
//                            {
//                                discountprice = @"<span class=""pricedisc"" style=""font-size: 20px;font-family: OpenSansSemiBold,Verdana;color: #0093c9;"">RM " + entitydeal.Prize + @"</span>";
//                            }
//                        }
//                        else
//                        {
//                            if (entitydeal.FlashDeal.Discount > 0)
//                            {
//                                decimal pricedis = entitydeal.Price - (entitydeal.Price * entitydeal.FlashDeal.Discount / 100);
//                                discountprice = @"<span class=""priceori"" style=""font-size: 16px;font-family: OpenSans,Verdana;color: #999999;text-decoration: line-through;"">RM " + entitydeal.Prize + @"</span>
//                            <span class=""pricedisc"" style=""font-size: 20px;font-family: OpenSansSemiBold,Verdana;color: #0093c9;"">RM " + pricedis + @"</span>";
//                            }
//                            else
//                            {
//                                discountprice = @"<span class=""pricedisc"" style=""font-size: 20px;font-family: OpenSansSemiBold,Verdana;color: #0093c9;"">RM " + entitydeal.Prize + @"</span>";
//                            }
//                        }
//                        if (entitydeal.TypeName.ToLower().Equals("raffle"))
//                        {
//                            deal += @" <a href=""" + _urldeal + @""">
//                                        <div class=""dealbox"" style=""border: 1px solid #cccccc;background: #fafafa;width: 348px;margin: 10px;float: left;-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;"">
//                                            <div class=""boxraffle"" style=""height: 355px;width: 100%;display: table;background: url('http://hka.siggie.net:8123/kuazoo/content/img/rafflebg.png')no-repeat;background-size: 100% 100%;"">
//                                                <div class=""boxrafflecontent"" style=""width:191px;display: table-cell;vertical-align: middle;padding-left: 5px;"">
//                                                    <div class=""raflleticket"" style=""background: url('http://hka.siggie.net:8123/kuazoo/content/img/raffleticket.png')no-repeat;background-size: 100% 100%;width: 263px;height: 133px;display: table-cell;vertical-align: middle;text-align: center;padding-left: 40px;font-family: OpenSansBold,Verdana;color: #432b11;font-size: 50px;"">RM " + entitydeal.Price + @"</div>
//                                                </div>
//                                                <div class=""boxprize"" style=""width: 150px;position: relative;-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;"">
//                                                    <div class=""boxprizeribbon"" style=""-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;width: 130px;height: 244px;position: absolute;top: -2px;left: 10px;background: white;-webkit-filter: drop-shadow(0 3px 5px rgba(0,0,0,.2));-moz-filter: drop-shadow(0 3px 5px rgba(0,0,0,.2));-ms-filter: drop-shadow(0 3px 5px rgba(0,0,0,.2));-o-filter: drop-shadow(0 3px 5px rgba(0,0,0,.2));filter: drop-shadow(0 3px 5px rgba(0,0,0,.2));z-index: 10;text-align: center;padding: 10px;"">
//                                                        <div class=""boxprizetitle"" style=""font-size: 14px;font-family: OpenSansBold,Verdana;color: #d60c7d;padding-top: 10px;padding-bottom: 20px;"">Sur'PRIZE'</div>
//                                                        <div class=""boxprizeimg"" style=""height: 110px;position: relative;overflow: hidden;"">
//                                                            <img src=""" + entitydeal.Prize.ImageUrl + @""" style=""width: 100%;position: absolute;top: 0;left: 0;bottom: 0;margin: auto;"" />
//                                                        </div>
//                                                        <div class=""boxprizedetail"" style=""font-size: 14px;font-family: OpenSans,Verdana;color: #333333;text-align: center;"">" + entitydeal.Prize.Name + @"</div><div style=""border-left: 65px solid transparent;border-top: 50px solid #ffffff;border-right: 65px solid transparent;position: absolute;top: 244px;left: 0px;z-index: -1;""></div>
//                                                    </div>
//                                                </div>
//                                            </div>
//                                            <div class=""boxfooter"" style=""height: 98px;display: table;"">
//                                                <div class=""boxoff"" style=""display: table-cell;text-align: center;min-width: 124px;vertical-align: middle;"">
//                                                    <div class=""boxoffbig2""><span class=""boxoffmedium"">RM</span>" + entitydeal.Price + @"</div>
//                                                </div>
//                                                <div class=""boxprice"" style=""display: table-cell;vertical-align: middle;width: 100%;"">
//                                                    <div>
//                                                        <span class=""pricedisc"" style=""font-size: 20px;font-family: OpenSansSemiBold,Verdana;color: #0093c9;"">RAFFLE TICKET</span>
//                                                    </div>
//                                                    <div>
//                                                        <div class=""boxbarbg"" style=""width: 96%;height: 10px;background: #cccccc;border-radius: 5px;""><div class=""boxbaractive"" style=""width:" + entitydeal.SalesVisualMeter + @"%;height: 10px;background: url('http://hka.siggie.net:8123/kuazoo/Content/img/boxbar.jpg')no-repeat;background-size: 312px 100%;border-radius: 5px;""></div></div>
//                                                    </div>
//                                                    <div class=""boxtext"" style=""font-size: 16px;font-family: OpenSans,Verdana;color: #333333;"">" + entitydeal.Sales + @" raffle tickets remaining!</div>
//                                                </div>
//                                            </div>
//                                        </div>
//                                        </a>";
//                        }
//                        else if (entitydeal.SalesVisualMeter >= 100 || entitydeal.Sales < 1)
//                        {
//                            deal += @"<a href=""" + _urldeal + @""">
//                                        <div class=""dealbox"" style=""border: 1px solid #cccccc;background: #fafafa;width: 348px;margin: 10px;float: left;-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;"">
//                                            <div class=""boxheader"" style=""height: 60px;display: table;padding-left: 5px;"">
//                                                <div class=""boxheadertext"" style=""font-size: 16px;font-family: OpenSansBold,Verdana;color: #333333;display: table-cell;vertical-align: middle;width: 191px;"">" + entitydeal.Name + @"</div>
//                                                <div class=""boxprize"" style=""width: 150px;position: relative;-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;"">
//                                                    <div class=""boxprizeribbon"" style=""-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;width: 130px;height: 244px;position: absolute;top: -2px;left: 10px;background: white;-webkit-filter: drop-shadow(0 3px 5px rgba(0,0,0,.2));-moz-filter: drop-shadow(0 3px 5px rgba(0,0,0,.2));-ms-filter: drop-shadow(0 3px 5px rgba(0,0,0,.2));-o-filter: drop-shadow(0 3px 5px rgba(0,0,0,.2));filter: drop-shadow(0 3px 5px rgba(0,0,0,.2));z-index: 10;text-align: center;padding: 10px;"">
//                                                        <div class=""boxprizetitle"" style=""font-size: 14px;font-family: OpenSansBold,Verdana;color: #d60c7d;padding-top: 10px;padding-bottom: 20px;"">Sur'PRIZE'</div>
//                                                        <div class=""boxprizeimg"" style=""height: 110px;position: relative;overflow: hidden;"">
//                                                            <img src=""" + entitydeal.Prize.ImageUrl + @""" style=""width: 100%;position: absolute;top: 0;left: 0;bottom: 0;margin: auto;""/>
//                                                        </div>
//                                                        <div class=""boxprizedetail"" style=""font-size: 14px;font-family: OpenSans,Verdana;color: #333333;text-align: center;"">" + entitydeal.Prize.Name + @"</div><div style=""border-left: 65px solid transparent;border-top: 50px solid #ffffff;border-right: 65px solid transparent;position: absolute;top: 244px;left: 0px;z-index: -1;""></div>
//                                                    </div>
//                                                </div>
//                                            </div>
//                                            <div class=""boxcontent"" style=""position: relative;height: 259px;max-height:259px;overflow: hidden;"">
//                                                <div class=""boxkuazoo"" style=""position: absolute;z-index: 1;height: 100%;width: 100%;display: table"">
//                                                    <div class=""boxkuazoobg"" style=""position: absolute;z-index: 1;background: #0093c9;height: 100%;width: 100%;opacity: 0.85;""></div>
//                                                    <div class=""boxcontentwrap"" style=""display: table-cell;vertical-align: middle;text-align: center;"">
//                                                        <div class=""boxTitle"" style=""z-index: 2;position: relative;font-size: 44px;color: white;font-family: OpenSansBold,Verdana;"">KUAZOO!</div>
//                                                        <div class=""boxSubtitle"" style=""z-index: 2;position: relative;font-size: 12px;color: white;font-family: OpenSansSemiBold,Verdana;letter-spacing: 4px;"">DRAW IS ON!</div>
//                                                    </div>
//                                                    <div class=""boxcontentprizetemp"" style=""width: 170px;""></div>
//                                                </div>
//                                                <img src=""" + entitydeal.ImageUrl + @"""style=""width: 100%;position: absolute;top: 0;left: 0;"" />
//                                            </div>
//                                            <div class=""boxfooter"" style=""height: 98px;display: table;"">
//                                                <div class=""boxoff"" style=""display: table-cell;text-align: center;min-width: 124px;vertical-align: middle;"">
//                                                    <div class=""boxoffbig"" style=""font-size: 50px;font-family: OpenSansSemiBold,Verdana;color: #333333;line-height: normal;"">" + entitydeal.Discount + @"%</div>
//                                                    <div  class=""boxoffsmall"" style=""font-size: 12px;font-family: OpenSans,Verdana;color: #333333;letter-spacing: 5px;padding-bottom: 10px;"">OFF</div>
//                                                </div>
//                                                <div class=""boxprice"" style=""display: table-cell;vertical-align: middle;width: 100%;"">
//                                                    <div>
//                                                        " + discountprice + @"
//                                                    </div>
//                                                    <div>
//                                                        <div class=""boxbarbg"" style=""width: 96%;height: 10px;background: #cccccc;border-radius: 5px;""><div class=""boxbaractive"" style=""width:100%;height: 10px;background: url('http://hka.siggie.net:8123/kuazoo/Content/img/boxbar.jpg')no-repeat;background-size: 312px 100%;border-radius: 5px;""></div></div>
//                                                    </div>
//                                                    <div class=""boxtext"" style=""font-size: 16px;font-family: OpenSans,Verdana;color: #333333;"">All deal sold!</div>
//                                                </div>
//                                            </div>
//                                        </div>
//                                        </a>";
//                        }
//                        else if (entitydeal.FlashDeal != null)
//                        {
//                            deal += @"<a href=""" + _urldeal + @""">
//                                        <div class=""dealbox"" style=""border: 1px solid #cccccc;background: #fafafa;width: 348px;margin: 10px;float: left;-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;"">
//                                            <div class=""boxheader"" style=""height: 60px;display: table;padding-left: 5px;"">
//                                                <div class=""boxheadertext"" style=""font-size: 16px;font-family: OpenSansBold,Verdana;color: #333333;display: table-cell;vertical-align: middle;width: 191px;"">" + entitydeal.Name + @"</div>
//                                                <div class=""boxprize"" style=""width: 150px;position: relative;-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;"">
//                                                    <div class=""boxprizeribbon"" style=""-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;width: 130px;height: 244px;position: absolute;top: -2px;left: 10px;background: white;-webkit-filter: drop-shadow(0 3px 5px rgba(0,0,0,.2));-moz-filter: drop-shadow(0 3px 5px rgba(0,0,0,.2));-ms-filter: drop-shadow(0 3px 5px rgba(0,0,0,.2));-o-filter: drop-shadow(0 3px 5px rgba(0,0,0,.2));filter: drop-shadow(0 3px 5px rgba(0,0,0,.2));z-index: 10;text-align: center;padding: 10px;"">
//                                                        <div class=""boxprizetitle"" style=""font-size: 14px;font-family: OpenSansBold,Verdana;color: #d60c7d;padding-top: 10px;padding-bottom: 20px;"">Sur'PRIZE'</div>
//                                                        <div class=""boxprizeimg"" style=""height: 110px;position: relative;overflow: hidden;"">
//                                                            <img src=""" + entitydeal.Prize.ImageUrl + @""" style=""width: 100%;position: absolute;top: 0;left: 0;bottom: 0;margin: auto;""/>
//                                                        </div>
//                                                        <div class=""boxprizedetail"" style=""font-size: 14px;font-family: OpenSans,Verdana;color: #333333;text-align: center;"">" + entitydeal.Prize.Name + @"</div><div style=""border-left: 65px solid transparent;border-top: 50px solid #ffffff;border-right: 65px solid transparent;position: absolute;top: 244px;left: 0px;z-index: -1;""></div>
//                                                    </div>
//                                                </div>
//                                            </div>
//                                            <div class=""boxcontent"" style=""position: relative;height: 259px;max-height:259px;overflow: hidden;"">
//                                                <img src=""" + entitydeal.ImageUrl + @""" style=""width: 100%;position: absolute;top: 0;left: 0;""/>
//                                            </div>
//                                            <div class=""boxfooter"" style=""height: 98px;display: table;"">
//                                                <div class=""boxoff"" style=""display: table-cell;text-align: center;min-width: 124px;vertical-align: middle;"">
//                                                    <div class=""boxoffbig"" style=""font-size: 50px;font-family: OpenSansSemiBold,Verdana;color: #333333;line-height: normal;"">" + entitydeal.FlashDeal.Discount + @"%</div>
//                                                    <div  class=""boxoffsmall"" style=""font-size: 12px;font-family: OpenSans,Verdana;color: #333333;letter-spacing: 5px;padding-bottom: 10px;"">OFF</div>
//                                                </div>
//                                                <div class=""boxprice"" style=""display: table-cell;vertical-align: middle;width: 100%;"">
//                                                    <div>
//                                                        " + discountprice + @"
//                                                    </div>
//                                                    <div>
//                                                        <div class=""boxbarbg"" style=""width: 96%;height: 10px;background: #cccccc;border-radius: 5px;""><div class=""boxbaractive"" style=""width:" + entitydeal.SalesVisualMeter + @"%;height: 10px;background: url('http://hka.siggie.net:8123/kuazoo/Content/img/boxbar.jpg')no-repeat;background-size: 312px 100%;border-radius: 5px;""></div></div>
//                                                    </div>
//                                                    <div class=""boxtext"" style=""font-size: 16px;font-family: OpenSans,Verdana;color: #333333;"">" + entitydeal.Sales + @" deals remaining!</div>
//                                                </div>
//                                            </div>
//                                        </div>
//                                        </a>";
//                        }
//                        else
//                        {
//                            deal += @"<a href=""" + _urldeal + @""">
//                                        <div class=""dealbox"" style=""border: 1px solid #cccccc;background: #fafafa;width: 348px;margin: 10px;float: left;-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;"">
//                                            <div class=""boxheader"" style=""height: 60px;display: table;padding-left: 5px;"">
//                                                <div class=""boxheadertext"" style=""font-size: 16px;font-family: OpenSansBold,Verdana;color: #333333;display: table-cell;vertical-align: middle;width: 191px;"">" + entitydeal.Name + @"</div>
//                                                <div class=""boxprize"" style=""width: 150px;position: relative;-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;"">
//                                                    <div class=""boxprizeribbon"" style=""-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;width: 130px;height: 244px;position: absolute;top: -2px;left: 10px;background: white;-webkit-filter: drop-shadow(0 3px 5px rgba(0,0,0,.2));-moz-filter: drop-shadow(0 3px 5px rgba(0,0,0,.2));-ms-filter: drop-shadow(0 3px 5px rgba(0,0,0,.2));-o-filter: drop-shadow(0 3px 5px rgba(0,0,0,.2));filter: drop-shadow(0 3px 5px rgba(0,0,0,.2));z-index: 10;text-align: center;padding: 10px;"">
//                                                        <div class=""boxprizetitle"" style=""font-size: 14px;font-family: OpenSansBold,Verdana;color: #d60c7d;padding-top: 10px;padding-bottom: 20px;"">Sur'PRIZE'</div>
//                                                        <div class=""boxprizeimg"" style=""height: 110px;position: relative;overflow: hidden;"">
//                                                            <img src=""" + entitydeal.Prize.ImageUrl + @""" style=""width: 100%;position: absolute;top: 0;left: 0;bottom: 0;margin: auto;""/>
//                                                        </div>
//                                                        <div class=""boxprizedetail"" style=""font-size: 14px;font-family: OpenSans,Verdana;color: #333333;text-align: center;"">" + entitydeal.Prize.Name + @"</div><div style=""border-left: 65px solid transparent;border-top: 50px solid #ffffff;border-right: 65px solid transparent;position: absolute;top: 244px;left: 0px;z-index: -1;""></div>
//                                                    </div>
//                                                </div>
//                                            </div>
//                                            <div class=""boxcontent"" style=""position: relative;height: 259px;max-height:259px;overflow: hidden;"">
//                                                <img src=""" + entitydeal.ImageUrl + @""" style=""width: 100%;position: absolute;top: 0;left: 0;"" />
//                                            </div>
//                                            <div class=""boxfooter"" style=""height: 98px;display: table;"">
//                                                <div class=""boxoff"" style=""display: table-cell;text-align: center;min-width: 124px;vertical-align: middle;"">
//                                                    <div class=""boxoffbig"" style=""font-size: 50px;font-family: OpenSansSemiBold,Verdana;color: #333333;line-height: normal;"">" + entitydeal.Discount + @"%</div>
//                                                    <div  class=""boxoffsmall"" style=""font-size: 12px;font-family: OpenSans,Verdana;color: #333333;letter-spacing: 5px;padding-bottom: 10px;"">OFF</div>
//                                                </div>
//                                                <div class=""boxprice"" style=""display: table-cell;vertical-align: middle;width: 100%;"">
//                                                    <div>
//                                                        
//                                                        " + discountprice + @"
//                                                    </div>
//                                                    <div>
//                                                        <div class=""boxbarbg"" style=""width: 96%;height: 10px;background: #cccccc;border-radius: 5px;""><div class=""boxbaractive"" style=""width:" + entitydeal.SalesVisualMeter + @"%;height: 10px;background: url('http://hka.siggie.net:8123/kuazoo/Content/img/boxbar.jpg')no-repeat;background-size: 312px 100%;border-radius: 5px;""></div></div>
//                                                    </div>
//                                                    <div class=""boxtext"" style=""font-size: 16px;font-family: OpenSans,Verdana;color: #333333;"">" + entitydeal.Sales + @" deals remaining!</div>
//                                                </div>
//                                            </div>
//                                        </div>
//                                        </a>";
//                        }
//                    }
//                    #endregion
//                    body = helper.Email.CreateShareEmail(feature,flashdeal,deal,urlflashdeal,urldeal);
//                    new GeneralService().SendEmail(subject, EmailTo, body, EmailFrom);
//                }
//                return true;
//            }
//            catch (Exception e)
//            {
//                return false;
//            }
//        }
    }
}
