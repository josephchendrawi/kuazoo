using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace com.kuazoo
{
    public class FlashDealService : IFlashDealService
    {

        public Response<bool> CreateFlashDeal(FlashDeal item)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                if (item.FlashDealId != 0)
                {
                    var entityFlashDeal = from d in context.kzFlashDeals
                                              where d.id == item.FlashDealId
                                     select d;
                    if (entityFlashDeal.Count() > 0)
                    {
                        var entityFlashDeal2 = from d in context.kzFlashDeals
                                               where d.inventoryitem_id == item.InventoryItemId 
                                                && d.id != item.FlashDealId
                                               select d;
                        entityFlashDeal2 = entityFlashDeal2.Where(d => (item.StartTime >= d.start_time && item.StartTime <= d.end_time)
                                                || (item.EndTime >= d.start_time && item.EndTime <= d.end_time)
                                                || (d.start_time >= item.StartTime && d.start_time <= item.EndTime)
                                                || (d.end_time >= item.StartTime && d.end_time <= item.EndTime));
                        if (entityFlashDeal2.Count() > 0)
                        {
                            throw new CustomException(CustomErrorType.FlashDealAlreadyAssign);
                        }
                        else
                        {
                            entityFlashDeal.First().inventoryitem_id = item.InventoryItemId;
                            entityFlashDeal.First().discount = item.Discount;
                            entityFlashDeal.First().start_time = item.StartTime;
                            entityFlashDeal.First().end_time = item.EndTime;
                            entityFlashDeal.First().flag = item.Flag;
                            entityFlashDeal.First().last_action = "3";
                            entityFlashDeal.First().email_flag = false;
                            context.SaveChanges();
                            response = Response<bool>.Create(true);
                            //NotifMemberFlashDeal(item.FlashDealId);
                        }
                    }
                    else
                    {
                        throw new CustomException(CustomErrorType.FlashDealNotFound);
                    }
                }
                else
                {
                   var entityFlashDeal2 = from d in context.kzFlashDeals
                                          where d.inventoryitem_id == item.InventoryItemId
                                          select d;
                   entityFlashDeal2 = entityFlashDeal2.Where(d => (item.StartTime >= d.start_time && item.StartTime <= d.end_time)
                                          || (item.EndTime >= d.start_time && item.EndTime <= d.end_time)
                                          || (d.start_time >= item.StartTime && d.start_time <= item.EndTime)
                                          || (d.end_time >= item.StartTime && d.end_time <= item.EndTime));
                   if (entityFlashDeal2.Count() > 0)
                   {
                       throw new CustomException(CustomErrorType.FlashDealAlreadyAssign);
                   }
                   else
                   {
                       entity.kzFlashDeal mmflash = new entity.kzFlashDeal();
                       mmflash.inventoryitem_id = item.InventoryItemId;
                       mmflash.discount = item.Discount;
                       mmflash.start_time = item.StartTime;
                       mmflash.end_time = item.EndTime;
                       mmflash.flag = item.Flag;
                       mmflash.last_action = "1";
                       mmflash.email_flag = false;
                       context.AddTokzFlashDeals(mmflash);
                       context.SaveChanges();
                       response = Response<bool>.Create(true);
                       //NotifMemberFlashDeal(mmflash.id);
                   }
                }
            }
            return response;
        }
        public Response<List<FlashDeal>> GetFlashDealList()
        {
            Response<List<FlashDeal>> response = null;
            List<FlashDeal> FlashDealList = new List<FlashDeal>();
            using (var context = new entity.KuazooEntities())
            {
                var entityFlashDeal = from d in context.kzFlashDeals.Include("kzInventoryItem")
                                      orderby d.end_time descending
                                 select d;
                foreach (var v in entityFlashDeal)
                {
                    FlashDeal FlashDeal = new FlashDeal();
                    FlashDeal.FlashDealId = v.id;
                    FlashDeal.MerchantId = (int)v.kzInventoryItem.merchant_id;
                    FlashDeal.MerchantName = v.kzInventoryItem.kzMerchant.name;
                    FlashDeal.InventoryItemId = (int)v.inventoryitem_id;
                    FlashDeal.InventoryItemName = v.kzInventoryItem.name;
                    FlashDeal.Discount = (decimal)v.discount;
                    FlashDeal.StartTime = (DateTime)v.start_time;
                    FlashDeal.EndTime = (DateTime)v.end_time;
                    FlashDeal.Flag = (Boolean)v.flag;
                    FlashDeal.LastAction = v.last_action;
                    FlashDealList.Add(FlashDeal);
                }
                response = Response<List<FlashDeal>>.Create(FlashDealList);
            }

            return response;
        }
        public Response<FlashDeal> GetFlashDealById(int FlashDealId)
        {
            Response<FlashDeal> response = null;
            FlashDeal FlashDeal = new FlashDeal();
            using (var context = new entity.KuazooEntities())
            {
                var entityFlashDeal = from d in context.kzFlashDeals.Include("kzInventoryItem")
                                    where d.id == FlashDealId
                                    select d;
                var v = entityFlashDeal.First();
                if (v != null)
                {
                    FlashDeal.FlashDealId = v.id;
                    FlashDeal.MerchantId = (int)v.kzInventoryItem.merchant_id;
                    FlashDeal.MerchantName = v.kzInventoryItem.kzMerchant.name;
                    FlashDeal.InventoryItemId = (int)v.inventoryitem_id;
                    FlashDeal.InventoryItemName = v.kzInventoryItem.name;
                    FlashDeal.Discount = (decimal)v.discount;
                    FlashDeal.StartTime = (DateTime)v.start_time;
                    FlashDeal.EndTime = (DateTime)v.end_time;
                    FlashDeal.Flag = (Boolean)v.flag;
                    FlashDeal.LastAction = v.last_action;
                }
                response = Response<FlashDeal>.Create(FlashDeal);
            }

            return response;
        }
        public Response<List<FlashDeal>> GetFlashDealListByInventoryItem(int InventoryItemId)
        {
            Response<List<FlashDeal>> response = null;
            List<FlashDeal> FlashDealList = new List<FlashDeal>();
            using (var context = new entity.KuazooEntities())
            {
                var entityFlashDeal = from d in context.kzFlashDeals.Include("kzInventoryItem")
                                      where d.inventoryitem_id == InventoryItemId
                                      select d;
                foreach (var v in entityFlashDeal)
                {
                    FlashDeal FlashDeal = new FlashDeal();
                    FlashDeal.FlashDealId = v.id;
                    FlashDeal.MerchantId = (int)v.kzInventoryItem.merchant_id;
                    FlashDeal.MerchantName = v.kzInventoryItem.kzMerchant.name;
                    FlashDeal.InventoryItemId = (int)v.inventoryitem_id;
                    FlashDeal.InventoryItemName = v.kzInventoryItem.name;
                    FlashDeal.Discount = (decimal)v.discount;
                    FlashDeal.StartTime = (DateTime)v.start_time;
                    FlashDeal.EndTime = (DateTime)v.end_time;
                    FlashDeal.Flag = (Boolean)v.flag;
                    FlashDeal.LastAction = v.last_action;
                    FlashDealList.Add(FlashDeal);
                }
                response = Response<List<FlashDeal>>.Create(FlashDealList);
            }

            return response;
        }
        public Response<List<InventoryItem>> GetInventoryItemList()
        {
            Response<List<InventoryItem>> response = null;
            List<InventoryItem> InventoryItemList = new List<InventoryItem>();
            using (var context = new entity.KuazooEntities())
            {
                var entityInventoryItem = from d in context.kzInventoryItems
                                      where d.last_action != "5"
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
        public Response<List<InventoryItem>> GetInventoryItemListByMerchant(int MerchantId)
        {
            Response<List<InventoryItem>> response = null;
            List<InventoryItem> InventoryItemList = new List<InventoryItem>();
            using (var context = new entity.KuazooEntities())
            {
                var entityInventoryItem = from d in context.kzInventoryItems
                                          where d.merchant_id == MerchantId && d.last_action != "5"
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
        public Response<bool> DeleteFlashDeal(int FlashDealId)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                var entityUser = from d in context.kzFlashDeals
                                 where d.id == FlashDealId
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


        //public void NotifMemberFlashDeal(int FlashDealId)
        //{
        //    try
        //    {
        //        using (var context = new entity.KuazooEntities())
        //        {
        //            //DateTime today = DateTime.Now;
        //            //var entityFlashDeal = from d in context.kzFlashDeals.Include("kzInventoryItem")
        //            //                      where d.id == FlashDealId && d.last_action != "5" && d.flag == true
        //            //                      && today >= d.start_time && today <= d.end_time
        //            //                      select d;
        //            //if (entityFlashDeal != null && entityFlashDeal.Count() > 0)
        //            //{
        //            //    string body = "";
        //            //    string subject = "Deal " + entityFlashDeal.First().kzInventoryItem.name + " Hit Flash Deal";
        //            //    body = "Deal <a href='" + WebSetting.Url + "/Home/Deals/" + entityFlashDeal.First().kzInventoryItem.name.Replace(" ", "-") + "?d=2'>" + entityFlashDeal.First().kzInventoryItem.name + "</a> Hit Flash Deal";
        //            //    body += "<br/>Discount from " + entityFlashDeal.First().kzInventoryItem.discount + "% to " + entityFlashDeal.First().discount + "%";

        //            //    var entityUser = from d in context.kzUsers
        //            //                     where d.notif == true
        //            //                     select d;
        //            //    foreach (var v in entityUser)
        //            //    {
        //            //        new GeneralService().SendEmail(subject, v.email, body);
        //            //    }
        //            //}
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

    }
}
