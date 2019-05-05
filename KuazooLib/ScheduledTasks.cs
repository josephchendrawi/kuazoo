using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.kuazoo
{
    public class ScheduledTasks
    {
        private static GeneralService generalservice = new GeneralService();
        public static void CheckFlashDealNotifEmail()
        {
            DateTime today = DateTime.UtcNow;

            using (var context = new entity.KuazooEntities())
            {
                var entityFlashDeal = from d in context.kzFlashDeals.Include("kzInventoryItem")
                                      where d.email_flag==false && d.last_action != "5" && d.flag == true
                                      && today >= d.start_time && today <= d.end_time
                                      select d;
                foreach(var v in entityFlashDeal)
                {
                    v.email_flag = true;
                    try
                    {
                        string body = "";
                        string subject = "Deal " + v.kzInventoryItem.name + " Hit Flash Deal";
                        //body = "Deal <a href='" + WebSetting.Url + "/Home/Deals/" + v.kzInventoryItem.name.Replace(" ", "-") + "?d=2'>" + v.kzInventoryItem.name + "</a> Hit Flash Deal";
                        //body += "<br/>Discount from " + v.kzInventoryItem.discount + "% to " + (v.kzInventoryItem.discount+v.discount) + "%";
                        body = helper.Email.CreateFlashDeal(v.kzInventoryItem.name, WebSetting.Url + "/Home/Deals/" + v.kzInventoryItem.name.Replace(" ", "-") + "?d=2", (decimal)v.kzInventoryItem.discount, (decimal)(v.kzInventoryItem.discount + v.discount));

                        var entityUser = from d in context.kzUsers
                                         where d.notif == true
                                         select d;
                        foreach (var v2 in entityUser)
                        {
                            //generalservice.SendEmail(subject, v2.email, body); //for now dont send flash deal email first.
                        }
                    }
                    catch
                    {

                    }
                }
                context.SaveChanges();
                var entityKuzooDeal = from d in context.kzInventoryItems.Include("kzMerchant").Include("kzInventoryItemType")
                                      where d.email_flag == false && d.kzPrize.expiry_date > today && today >= d.publish_date
                                      && d.flag == true && d.last_action != "5"
                                      && d.salesvisualmeter >= 100
                                      select d;
                foreach (var v in entityKuzooDeal)
                {
                    v.email_flag = true;
                    try
                    {
                        string to = "noreply@kuazoo.com.my";
                        string body = "";
                        string subject = "Deal "+ v.name +" Hit Kuazoo Deal";

                        //body = "Item " + v.name + " from Merchant " + v.kzMerchant.name + " hit the Kuazoo deal";
                        body = helper.Email.CreateKuazooDeal(v.name, WebSetting.Url + "/Home/Deals/" + v.name.Replace(" ", "-") + "?d=1");

                        generalservice.SendEmail(subject, to, body);
                    }
                    catch
                    {

                    }
                }
                context.SaveChanges();
            }
        }
    }
}
