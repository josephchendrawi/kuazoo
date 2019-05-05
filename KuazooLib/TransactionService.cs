using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Net.Mail;
using System.Threading;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Web.Mvc;
using iTextSharp.text.html.simpleparser;

namespace com.kuazoo
{
    public class TransactionService : ITransactionService
    {


        public Response<int> CreateTransaction(TransactionsHeader item, string path, int gmt = 8)
        {
            Response<int> response = null;
            using (var context = new entity.KuazooEntities())
            {
                if (item.TransactionId != 0)
                {
                    var entityTransaction = from d in context.kzTransactions
                                            where d.id == item.TransactionId
                                            select d;
                    if (entityTransaction.Count() > 0)
                    {
                        string oldstatus = entityTransaction.First().status;
                        entityTransaction.First().transaction_status = item.TransactionStatus;
                        entityTransaction.First().last_action = "3";
                        entityTransaction.First().tranID = item.tranID;
                        entityTransaction.First().orderid = item.orderid;
                        entityTransaction.First().status = item.status;
                        entityTransaction.First().domain = item.domain;
                        entityTransaction.First().amount = item.amount;
                        entityTransaction.First().currency = item.currency;
                        entityTransaction.First().appcode = item.appcode;
                        entityTransaction.First().paydate = item.paydate;
                        entityTransaction.First().skey = item.skey;
                        entityTransaction.First().channel = item.channel;
                        entityTransaction.First().error_code = item.error_code;
                        entityTransaction.First().error_desc = item.error_desc;
                        context.SaveChanges();
                        if (item.status == "00")
                        {
                            if (oldstatus != item.status)
                            {
                                int userid = (int)entityTransaction.First().user_id;
                                foreach (var v in entityTransaction.First().kzTransactionDetails)
                                {
                                    com.kuazoo.PointDetail purchasepoint = new PointDetail();
                                    purchasepoint.Type = KPointAction.PurchaseItem;
                                    purchasepoint.UserId = userid;
                                    purchasepoint.Remarks = "";
                                    decimal amountpoint = (decimal)v.price;
                                    if (entityTransaction.First().kzPromotion != null && entityTransaction.First().kzPromotion.type == 1)
                                    {
                                        amountpoint = amountpoint - (amountpoint * (decimal)entityTransaction.First().kzPromotion.value / 100);
                                    }
                                    else if (entityTransaction.First().kzPromotion != null && entityTransaction.First().kzPromotion.type == 2)
                                    {
                                        amountpoint = amountpoint - (decimal)entityTransaction.First().kzPromotion.value;
                                    }
                                    if ((decimal)entityTransaction.First().kpoint > 0)
                                    {
                                        amountpoint = amountpoint - ((decimal)entityTransaction.First().kpoint / 100);
                                    }
                                    amountpoint = Math.Floor(amountpoint);
                                    amountpoint = amountpoint * PointService.GetPointAction(purchasepoint.Type).Result;
                                    purchasepoint.Amount = amountpoint;
                                    purchasepoint.InventoryItemId = (int)v.inventoryitem_id;
                                    purchasepoint.TransactionId = item.TransactionId;
                                    new PointService().CreatePointByContext(context, purchasepoint);
                                    UpdateSalesVisualMeterV2((int)v.inventoryitem_id, (DateTime)v.kzTransaction.transaction_date, v.variance);
                                    CheckMinimumTargetNotif((int)v.inventoryitem_id);
                                    context.SaveChanges();
                                    if (v.kzInventoryItem.inventoryitem_type_id == 2)
                                    {
                                        int qty = (int)v.qty;
                                        var entityCodeTra = from d in context.kzPreCodes
                                                            where d.transaction_id == item.TransactionId
                                                            select d;
                                        if (entityCodeTra.Count() > 0)
                                        {
                                            qty = qty - entityCodeTra.Count();
                                        }
                                        if (qty > 0)
                                        {
                                            var entityCode = (from d in context.kzPreCodes
                                                              where d.transaction_id.HasValue == false
                                                              orderby d.id ascending
                                                              select d).Take(qty);
                                            foreach (var c in entityCode)
                                            {
                                                c.transaction_id = item.TransactionId;
                                            }
                                            context.SaveChanges();
                                        }
                                    }
                                }
                                TransactionMemberNotif(item.TransactionId, path, gmt);
                                TransactionGiftNotice(item.TransactionId, path, gmt);
                            }

                        }
                        else if (item.status == "11")//failed
                        {
                            decimal point = (decimal)entityTransaction.First().kpoint;
                            if (point > 0)
                            {
                                int userid = (int)entityTransaction.First().user_id;
                                int inventoryitemid = 0;
                                foreach (var v in entityTransaction.First().kzTransactionDetails)
                                {
                                    inventoryitemid = (int)v.inventoryitem_id;
                                }
                                int kpointid = 0;
                                var entityPointHeader = from d in context.kzKPointTrxHs
                                                        where d.user_id == userid
                                                        && d.last_action != "5"
                                                        select d;
                                if (entityPointHeader.Count() > 0)
                                {
                                    kpointid = entityPointHeader.First().id;
                                }
                                bool check = true;
                                if (kpointid > 0)
                                {
                                    int revert = (int)KPointAction.Revert;
                                    var entityPointDetail = from d in context.kzKPointTrxDs
                                                            where d.inventoryitem_id == inventoryitemid
                                                            && d.amount == point
                                                            && d.action_type == revert
                                                            && d.kpointh_id == kpointid
                                                            && d.transaction_id == item.TransactionId
                                                            select d;
                                    if (entityPointDetail.Count() > 0)
                                    {
                                        check = false;
                                    }
                                }
                                if (check)
                                {
                                    com.kuazoo.PointDetail purchasepoint = new PointDetail();
                                    purchasepoint.Type = KPointAction.Revert;
                                    purchasepoint.UserId = userid;
                                    purchasepoint.Remarks = "";
                                    purchasepoint.Amount = point;
                                    purchasepoint.InventoryItemId = inventoryitemid;
                                    purchasepoint.TransactionId = item.TransactionId;
                                    new PointService().CreatePointByContext(context, purchasepoint);
                                }
                            }
                        }

                        response = Response<int>.Create(item.TransactionId);
                    }
                    else
                    {
                        throw new CustomException(CustomErrorType.TransactionNotFound);
                    }
                }
                else
                {
                    entity.kzTransaction mmtran = new entity.kzTransaction();
                    mmtran.user_id = item.MemberId;
                    mmtran.transaction_date = item.TransactionDate;
                    mmtran.transaction_status = item.TransactionStatus;
                    if (item.PromotionId != 0)
                    {
                        mmtran.promotion_id = item.PromotionId;
                    }
                    mmtran.kpoint = item.KPoint;
                    mmtran.last_action = "1";
                    context.AddTokzTransactions(mmtran);
                    context.SaveChanges();
                    int transactionid = mmtran.id;

                    if (item.KPoint > 0)
                    {
                        com.kuazoo.PointDetail redeempoint = new PointDetail();
                        redeempoint.Type = KPointAction.Redeemed;
                        redeempoint.UserId = item.MemberId;
                        redeempoint.Remarks = "";
                        redeempoint.Amount = item.KPoint;
                        redeempoint.InventoryItemId = item.Detail.First().InventoryItemId;
                        redeempoint.TransactionId = transactionid;
                        new PointService().CreatePointByContext(context, redeempoint);
                        context.SaveChanges();
                    }
                    foreach (var v in item.Detail)
                    {
                        entity.kzTransactionDetail mmtrand = new entity.kzTransactionDetail();
                        mmtrand.transaction_id = transactionid;
                        mmtrand.inventoryitem_id = v.InventoryItemId;
                        if (v.FlashDealId != null && v.FlashDealId != 0)
                        {
                            mmtrand.flashdeal_id = v.FlashDealId;
                        }
                        mmtrand.variance = v.Variance;
                        mmtrand.qty = v.Qty;
                        mmtrand.discount = v.Discount;
                        mmtrand.price = v.Price;
                        context.AddTokzTransactionDetails(mmtrand);

                        //com.kuazoo.PointDetail purchasepoint = new PointDetail();
                        //purchasepoint.Type = KPointAction.PurchaseItem;
                        //purchasepoint.UserId = item.MemberId;
                        //purchasepoint.Remarks = "";
                        //decimal amountpoint = v.Price;
                        //if (item.PromotionType == 1)
                        //{
                        //    amountpoint = amountpoint  - (amountpoint* item.PromotionValue / 100);
                        //}
                        //else if(item.PromotionType == 2)
                        //{
                        //    amountpoint = amountpoint - item.PromotionValue;
                        //}
                        //if (item.KPoint > 0)
                        //{
                        //    amountpoint = amountpoint - (item.KPoint / 100);
                        //}
                        //amountpoint = Math.Floor(amountpoint);
                        //amountpoint = amountpoint * PointService.GetPointAction(purchasepoint.Type).Result;
                        //purchasepoint.Amount = amountpoint;
                        //purchasepoint.InventoryItemId = v.InventoryItemId;
                        //purchasepoint.TransactionId = transactionid;
                        //new PointService().CreatePointByContext(context, purchasepoint);
                    }
                    context.SaveChanges();
                    //foreach(var v in item.Detail){
                    //    UpdateSalesVisualMeterV2(v.InventoryItemId, item.TransactionDate, v.Variance);
                    //    CheckMinimumTargetNotif(v.InventoryItemId);
                    //}
                    response = Response<int>.Create(transactionid);
                }
            }
            return response;
        }

        public void CheckPoint(int TransactionId, decimal point)
        {
            using (var context = new entity.KuazooEntities())
            {
                if (TransactionId != 0)
                {
                    var entityTransaction = from d in context.kzTransactions
                                            where d.id == TransactionId
                                            select d;
                    if (entityTransaction.Count() > 0)
                    {
                        if (point > 0)
                        {
                            int userid = (int)entityTransaction.First().user_id;
                            int kpointid = 0;
                            var entityPointHeader = from d in context.kzKPointTrxHs
                                                    where d.user_id == userid
                                                    && d.last_action != "5"
                                                    select d;
                            if (entityPointHeader.Count() > 0)
                            {
                                kpointid = entityPointHeader.First().id;
                            }
                            int inventoryitemid = 0;
                            foreach (var v in entityTransaction.First().kzTransactionDetails)
                            {
                                inventoryitemid = (int)v.inventoryitem_id;
                            }
                            if (kpointid > 0)
                            {
                                int revert = (int)KPointAction.Revert;

                                var entityPointDetail = from d in context.kzKPointTrxDs
                                                        where d.inventoryitem_id == inventoryitemid
                                                        && d.amount == point
                                                        && d.kpointh_id == kpointid
                                                        && d.transaction_id == TransactionId
                                                        orderby d.last_created descending 
                                                        select d;
                                if (entityPointDetail.Count() > 0)
                                {
                                    if (entityPointDetail.First().action_type == revert)//if the last is revert, adding back
                                    {
                                        com.kuazoo.PointDetail redeempoint = new PointDetail();
                                        redeempoint.Type = KPointAction.Redeemed;
                                        redeempoint.UserId = userid;
                                        redeempoint.Remarks = "";
                                        redeempoint.Amount = point;
                                        redeempoint.InventoryItemId = inventoryitemid;
                                        redeempoint.TransactionId = TransactionId;
                                        new PointService().CreatePointByContext(context, redeempoint);
                                        context.SaveChanges();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public Response<bool> CreateCallbakLog(CallbackPayment item)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                entity.kzCallbackMOLLog cal = new entity.kzCallbackMOLLog();
                cal.creationdate = DateTime.UtcNow;
                cal.nbcb = item.nbcb;
                cal.tranID = item.tranID;
                cal.orderid = item.orderid;
                cal.status = item.status;
                cal.domain = item.domain;
                cal.amount = item.amount;
                cal.currency = item.currency;
                cal.appcode = item.appcode;
                cal.paydate = item.paydate;
                cal.skey = item.skey;
                cal.channel = item.channel;
                cal.error_code = item.error_code;
                cal.error_desc = item.error_desc;
                context.AddTokzCallbackMOLLogs(cal);
                context.SaveChanges();
                response = Response<bool>.Create(true);
            }
            return response;
        }
        public Response<bool> CheckTransactionPayment(int TransactionId)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                if (TransactionId != 0)
                {
                    var entityTransaction = from d in context.kzTransactions
                                            where d.id == TransactionId
                                            select d;
                    if (entityTransaction.Count() > 0 && entityTransaction.First().tranID != null && entityTransaction.First().tranID != "" && entityTransaction.First().status != "11")
                    {
                        response = Response<bool>.Create(true);
                    }
                    else
                    {
                        response = Response<bool>.Create(false);
                    }
                }
                else
                {
                    throw new CustomException(CustomErrorType.TransactionNotFound);
                }
            }
            return response;
        }
        public Response<bool> UpdateTransactionPrizeParticipation(int TransactionId)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                if (TransactionId != 0)
                {
                    var entityTransaction = from d in context.kzTransactions
                                            where d.id == TransactionId
                                            select d;
                    if (entityTransaction.Count() > 0)
                    {
                        entityTransaction.First().participate_game = true;
                        context.SaveChanges();
                        response = Response<bool>.Create(true);
                    }
                    else
                    {
                        throw new CustomException(CustomErrorType.TransactionNotFound);
                    }
                }
                else
                {
                    throw new CustomException(CustomErrorType.TransactionNotFound);
                }
            }
            return response;
        }
        public Response<bool> UpdateTransactionStatus(int TransactionId, int TransactionStatus)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                if (TransactionId != 0)
                {
                    var entityTransaction = from d in context.kzTransactions
                                            where d.id == TransactionId
                                            select d;
                    if (entityTransaction.Count() > 0)
                    {
                        entityTransaction.First().transaction_status = TransactionStatus;
                        entityTransaction.First().process_date = DateTime.UtcNow;
                        context.SaveChanges();
                        response = Response<bool>.Create(true);
                    }
                    else
                    {
                        throw new CustomException(CustomErrorType.TransactionNotFound);
                    }
                }
                else
                {
                    throw new CustomException(CustomErrorType.TransactionNotFound);
                }
            }
            return response;
        }
        public Response<decimal> GetTransactionStatusCount()
        {
            Response<decimal> response = null;
            using (var context = new entity.KuazooEntities())
            {
                var entityTransaction = from d in context.kzTransactions
                                        where d.last_action != "5"
                                        && d.status == "00" //success
                                        && (d.process_status == null || d.process_status != 1)
                                        select d;
                decimal result = entityTransaction.Count();

                response = Response<decimal>.Create(result);
            }
            return response;
        }
        public Response<bool> CreateShipping(ShippingC shipm, int TransactionId)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                var entityShip = from d in context.kzShippings
                                 where d.transaction_id == TransactionId
                                 select d;
                if (entityShip.Count() > 0)
                {
                    entityShip.First().first_name = shipm.fn;
                    entityShip.First().last_name = shipm.ln;
                    entityShip.First().phone = shipm.ph;
                    entityShip.First().gender = shipm.gender;
                    entityShip.First().address_line1 = shipm.ad1;
                    entityShip.First().address_line2 = shipm.ad2;
                    entityShip.First().city = shipm.city;
                    entityShip.First().state = shipm.state;
                    entityShip.First().country = shipm.country;
                    entityShip.First().zipcode = shipm.zip;
                    bool gift = false;
                    if (shipm.gift == 1) gift = true;
                    entityShip.First().gift = gift;
                    entityShip.First().note = shipm.note;
                    entityShip.First().last_action = "3";
                    entityShip.First().rcemail = shipm.rcemail;
                    context.SaveChanges();
                }
                else
                {
                    entity.kzShipping ship = new entity.kzShipping();
                    ship.transaction_id = TransactionId;
                    ship.first_name = shipm.fn;
                    ship.last_name = shipm.ln;
                    ship.phone = shipm.ph;
                    ship.gender = shipm.gender;
                    ship.address_line1 = shipm.ad1;
                    ship.address_line2 = shipm.ad2;
                    ship.city = shipm.city;
                    ship.state = shipm.state;
                    ship.country = shipm.country;
                    ship.zipcode = shipm.zip;
                    bool gift = false;
                    if (shipm.gift == 1) gift = true;
                    ship.gift = gift;
                    ship.note = shipm.note;
                    ship.last_action = "1";
                    ship.rcemail = shipm.rcemail;
                    context.AddTokzShippings(ship);
                    context.SaveChanges();
                }
                response = Response<bool>.Create(true);
            }
            return response;
        }

        public Response<bool> CreateShippingUser(ShippingC shipm, int UserId)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                var entityShip = from d in context.kzShippingUsers
                                 where d.user_id == UserId
                                 select d;
                if (entityShip.Count() > 0)
                {
                    entityShip.First().first_name = shipm.fn;
                    entityShip.First().last_name = shipm.ln;
                    entityShip.First().phone = shipm.ph;
                    entityShip.First().gender = shipm.gender;
                    entityShip.First().address_line1 = shipm.ad1;
                    entityShip.First().address_line2 = shipm.ad2;
                    entityShip.First().city = shipm.city;
                    entityShip.First().state = shipm.state;
                    entityShip.First().country = shipm.country;
                    entityShip.First().zipcode = shipm.zip;
                    bool gift = false;
                    if (shipm.gift == 1) gift = true;
                    entityShip.First().gift = gift;
                    entityShip.First().note = shipm.note;
                    entityShip.First().last_action = "3";
                    context.SaveChanges();
                }
                else
                {
                    entity.kzShippingUser ship = new entity.kzShippingUser();
                    ship.user_id = UserId;
                    ship.first_name = shipm.fn;
                    ship.last_name = shipm.ln;
                    ship.phone = shipm.ph;
                    ship.gender = shipm.gender;
                    ship.address_line1 = shipm.ad1;
                    ship.address_line2 = shipm.ad2;
                    ship.city = shipm.city;
                    ship.state = shipm.state;
                    ship.country = shipm.country;
                    ship.zipcode = shipm.zip;
                    bool gift = false;
                    if (shipm.gift == 1) gift = true;
                    ship.gift = gift;
                    ship.note = shipm.note;
                    ship.last_action = "1";
                    context.AddTokzShippingUsers(ship);
                    context.SaveChanges();
                }
                response = Response<bool>.Create(true);
            }
            return response;
        }
        public Response<bool> CreateBilling(BillingC shipm, int TransactionId)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                var entityShip = from d in context.kzBillings
                                 where d.transaction_id == TransactionId
                                 select d;
                if (entityShip.Count() > 0)
                {
                    int payment = 0;
                    if (shipm.payment.ToLower() == "molpay") payment = (int)PaymentMethod.MOLPay;
                    else if (shipm.payment.ToLower() == "ipay88") payment = (int)PaymentMethod.Ipay88;
                    else if (shipm.payment.ToLower() == "visa") payment = (int)PaymentMethod.Visa;
                    else if (shipm.payment.ToLower() == "master") payment = (int)PaymentMethod.Master;
                    else if (shipm.payment.ToLower() == "amex") payment = (int)PaymentMethod.Amex;
                    entityShip.First().payment_method = payment;
                    entityShip.First().payment_cc = shipm.cc;
                    entityShip.First().payment_ccv = shipm.ccv;
                    int month = 0;
                    int.TryParse(shipm.month, out month);
                    int year = 0;
                    int.TryParse(shipm.year, out year);
                    entityShip.First().payment_expm = month;
                    entityShip.First().payment_expy = year;

                    entityShip.First().first_name = shipm.fn;
                    entityShip.First().last_name = shipm.ln;
                    entityShip.First().phone = shipm.ph;
                    entityShip.First().gender = shipm.gender;
                    entityShip.First().address_line1 = shipm.ad1;
                    entityShip.First().address_line2 = shipm.ad2;
                    entityShip.First().city = shipm.city;
                    entityShip.First().state = shipm.state;
                    entityShip.First().country = shipm.country;
                    entityShip.First().zipcode = shipm.zip;
                    entityShip.First().last_action = "3";
                    context.SaveChanges();
                }
                else
                {
                    entity.kzBilling ship = new entity.kzBilling();

                    int payment = 0;
                    if (shipm.payment.ToLower() == "molpay") payment = (int)PaymentMethod.MOLPay;
                    else if (shipm.payment.ToLower() == "ipay88") payment = (int)PaymentMethod.Ipay88;
                    else if (shipm.payment.ToLower() == "visa") payment = (int)PaymentMethod.Visa;
                    else if (shipm.payment.ToLower() == "master") payment = (int)PaymentMethod.Master;
                    else if (shipm.payment.ToLower() == "amex") payment = (int)PaymentMethod.Amex;
                    ship.payment_method = payment;
                    ship.payment_cc = shipm.cc;
                    ship.payment_ccv = shipm.ccv;
                    int month = 0;
                    int.TryParse(shipm.month, out month);
                    int year = 0;
                    int.TryParse(shipm.year, out year);
                    ship.payment_expm = month;
                    ship.payment_expy = year;

                    ship.transaction_id = TransactionId;
                    ship.first_name = shipm.fn;
                    ship.last_name = shipm.ln;
                    ship.phone = shipm.ph;
                    ship.gender = shipm.gender;
                    ship.address_line1 = shipm.ad1;
                    ship.address_line2 = shipm.ad2;
                    ship.city = shipm.city;
                    ship.state = shipm.state;
                    ship.country = shipm.country;
                    ship.zipcode = shipm.zip;
                    ship.last_action = "1";
                    context.AddTokzBillings(ship);
                    context.SaveChanges();
                }
                response = Response<bool>.Create(true);
            }
            return response;
        }
        public Response<bool> CreateBillingUser(BillingC shipm, int UserId)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                var entityShip = from d in context.kzBillingUsers
                                 where d.user_id == UserId
                                 select d;
                if (entityShip.Count() > 0)
                {
                    int payment = 0;
                    if (shipm.payment.ToLower() == "molpay") payment = (int)PaymentMethod.MOLPay;
                    else if (shipm.payment.ToLower() == "ipay88") payment = (int)PaymentMethod.Ipay88;
                    else if (shipm.payment.ToLower() == "visa") payment = (int)PaymentMethod.Visa;
                    else if (shipm.payment.ToLower() == "master") payment = (int)PaymentMethod.Master;
                    else if (shipm.payment.ToLower() == "amex") payment = (int)PaymentMethod.Amex;
                    entityShip.First().payment_method = payment;
                    entityShip.First().payment_cc = shipm.cc;
                    //entityShip.First().payment_ccv = shipm.ccv;
                    int month = 0;
                    int.TryParse(shipm.month, out month);
                    int year = 0;
                    int.TryParse(shipm.year, out year);
                    entityShip.First().payment_expm = month;
                    entityShip.First().payment_expy = year;

                    entityShip.First().first_name = shipm.fn;
                    entityShip.First().last_name = shipm.ln;
                    entityShip.First().phone = shipm.ph;
                    entityShip.First().gender = shipm.gender;
                    entityShip.First().address_line1 = shipm.ad1;
                    entityShip.First().address_line2 = shipm.ad2;
                    entityShip.First().city = shipm.city;
                    entityShip.First().state = shipm.state;
                    entityShip.First().country = shipm.country;
                    entityShip.First().zipcode = shipm.zip;
                    entityShip.First().last_action = "3";
                    context.SaveChanges();
                }
                else
                {
                    entity.kzBillingUser ship = new entity.kzBillingUser();

                    int payment = 0;
                    if (shipm.payment.ToLower() == "molpay") payment = (int)PaymentMethod.MOLPay;
                    else if (shipm.payment.ToLower() == "ipay88") payment = (int)PaymentMethod.Ipay88;
                    else if (shipm.payment.ToLower() == "visa") payment = (int)PaymentMethod.Visa;
                    else if (shipm.payment.ToLower() == "master") payment = (int)PaymentMethod.Master;
                    else if (shipm.payment.ToLower() == "amex") payment = (int)PaymentMethod.Amex;
                    ship.payment_method = payment;
                    ship.payment_cc = shipm.cc;
                    ship.payment_ccv = "";//shipm.ccv;
                    int month = 0;
                    int.TryParse(shipm.month, out month);
                    int year = 0;
                    int.TryParse(shipm.year, out year);
                    ship.payment_expm = month;
                    ship.payment_expy = year;

                    ship.user_id = UserId;
                    ship.first_name = shipm.fn;
                    ship.last_name = shipm.ln;
                    ship.phone = shipm.ph;
                    ship.gender = shipm.gender;
                    ship.address_line1 = shipm.ad1;
                    ship.address_line2 = shipm.ad2;
                    ship.city = shipm.city;
                    ship.state = shipm.state;
                    ship.country = shipm.country;
                    ship.zipcode = shipm.zip;
                    ship.last_action = "1";
                    context.AddTokzBillingUsers(ship);
                    context.SaveChanges();
                }
                response = Response<bool>.Create(true);
            }
            return response;
        }
        public Response<bool> CreateBillAccount(BillingC item)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                string currentUserName = Thread.CurrentPrincipal.Identity.Name;
                if (currentUserName == "Guest")
                {
                    User us = new User();
                    us.FirstName = item.fn;
                    us.LastName = item.ln;
                    us.Email = item.email;
                    us.Gender = item.gender;
                    us.UserStatus = 5;
                    us.Password = Security.RandomString2(5);
                    if (item.pass != null || item.pass != "")
                    {
                        us.Password = item.pass;
                    }
                    us.Verify = true;
                    bool result = new UserService().CreateUser(us).Result;
                }

                response = Response<bool>.Create(true);
            }
            return response;
        }


        public Response<Bill> GetCurrentUserShipping()
        {
            Response<Bill> response = null;
            Bill bill = new Bill();
            using (var context = new entity.KuazooEntities())
            {
                var user = new UserService().GetCurrentUser().Result;
                if (user.UserId != 0)
                {
                    //var entityShip = from d in context.kzShippings
                    //                 where d.kzTransaction.user_id == user.UserId
                    //                 orderby d.kzTransaction.transaction_date descending
                    //                 select d;
                    var entityShip = from d in context.kzShippingUsers
                                     where d.user_id == user.UserId
                                     select d;
                    if (entityShip.Count() > 0)
                    {
                        bill.FirstName = entityShip.First().first_name;
                        bill.LastName = entityShip.First().last_name;
                        bill.Phone = entityShip.First().phone;
                        bill.Gender = (int)entityShip.First().gender;
                        bill.AddressLine1 = entityShip.First().address_line1;
                        bill.AddressLine2 = entityShip.First().address_line2;
                        bill.City = entityShip.First().city;
                        bill.State = entityShip.First().state;
                        bill.Country = entityShip.First().country;
                        bill.ZipCode = entityShip.First().zipcode;
                        bill.Gift = (bool)entityShip.First().gift;
                        bill.Note = entityShip.First().note;
                        response = Response<Bill>.Create(bill);
                    }
                }
            }
            return response;
        }

        public Response<Bill> GetCurrentUserBilling()
        {
            Response<Bill> response = null;
            Bill bill = new Bill();
            using (var context = new entity.KuazooEntities())
            {
                var user = new UserService().GetCurrentUser().Result;
                if (user.UserId != 0)
                {
                    //var entityShip = from d in context.kzBillings
                    //                 where d.kzTransaction.user_id == user.UserId
                    //                 orderby d.kzTransaction.transaction_date descending
                    //                 select d;
                    var entityShip = from d in context.kzBillingUsers
                                     where d.user_id == user.UserId
                                     select d;
                    if (entityShip.Count() > 0)
                    {
                        bill.PaymentMethod = (int)entityShip.First().payment_method;
                        bill.PaymentCC = entityShip.First().payment_cc;
                        bill.PaymentCCV = "";//entityShip.First().payment_ccv;
                        bill.PaymentExpireMonth = 1;
                        bill.PaymentExpireYear = DateTime.Now.Year;
                        if (entityShip.First().payment_expm != null && entityShip.First().payment_expm != 0)
                        {
                            bill.PaymentExpireMonth = (int)entityShip.First().payment_expm;
                        }
                        if (entityShip.First().payment_expy != null && entityShip.First().payment_expy != 0)
                        {
                            bill.PaymentExpireYear = (int)entityShip.First().payment_expy;
                        }
                        bill.FirstName = entityShip.First().first_name;
                        bill.LastName = entityShip.First().last_name;
                        bill.Phone = entityShip.First().phone;
                        bill.Gender = (int)entityShip.First().gender;
                        bill.AddressLine1 = entityShip.First().address_line1;
                        bill.AddressLine2 = entityShip.First().address_line2;
                        bill.City = entityShip.First().city;
                        bill.State = entityShip.First().state;
                        bill.Country = entityShip.First().country;
                        bill.ZipCode = entityShip.First().zipcode;
                        bill.Email = user.Email;
                        response = Response<Bill>.Create(bill);
                    }
                    else
                    {
                        bill.Email = user.Email;
                        response = Response<Bill>.Create(bill);
                    }
                }
            }
            return response;
        }


        public Response<PromotionCode> CheckPromotionCode(string code)
        {
            Response<PromotionCode> response = null;
            PromotionCode promo = new PromotionCode();
            using (var context = new entity.KuazooEntities())
            {
                //var user = new UserService().GetCurrentUser().Result;
                //if (user.UserId != 0)
                //{
                var entityShip = from d in context.kzPromotions
                                 where d.code == code
                                 && d.valid_date >= DateTime.UtcNow
                                 && d.flag == true
                                 && d.last_action != "5"
                                 select d;
                if (entityShip.Count() > 0)
                {
                    promo.PromotionId = entityShip.First().id;
                    promo.Code = entityShip.First().code;
                    promo.Type = (int)entityShip.First().type;
                    promo.TypeName = "Discount";
                    if (promo.Type == 2)
                    {
                        promo.TypeName = "Amount";
                    }
                    promo.Value = (decimal)entityShip.First().value;
                    response = Response<PromotionCode>.Create(promo);
                }
                //}
            }
            return response;
        }

        public Response<bool> CheckTransactionPromotionCodeUsed(int promoid, int userid, int tId = 0)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                if (promoid != 0)
                {
                    var entityTransaction = from d in context.kzTransactions
                                            where d.user_id == userid && d.promotion_id == promoid
                                            && d.last_action != "5"
                                            && (d.status != "11" || d.status == null)
                                            && d.id != tId
                                            select d;
                    if (entityTransaction.Count() > 0)
                    {
                        response = Response<bool>.Create(true);
                    }
                    else
                    {
                        response = Response<bool>.Create(false);
                    }
                }
                else
                {
                    response = Response<bool>.Create(false);
                }
            }
            return response;
        }

        public Response<TransactionsHeader> GetTransactionsListAnHour(DateTime today)
        {
            Response<TransactionsHeader> response = null;
            TransactionsHeader transaction = new TransactionsHeader();
            using (var context = new entity.KuazooEntities())
            {
                DateTime now = DateTime.UtcNow.AddHours(-1);
                var user = new UserService().GetCurrentUser().Result;
                if (user.UserId != 0)
                {
                    var entityTransaction = from d in context.kzTransactions
                                            where d.transaction_date >= now
                                            && d.last_action != "5"
                                            && d.user_id == user.UserId
                                            orderby d.transaction_date descending
                                            select d;
                    if (entityTransaction.Count() > 0)
                    {
                        transaction.TransactionId = entityTransaction.First().id;
                        transaction.TransactionStatus = (int)entityTransaction.First().transaction_status;
                        transaction.TransactionDate = (DateTime)entityTransaction.First().transaction_date;
                        transaction.MemberId = (int)entityTransaction.First().user_id;


                        Bill shipping = new Bill();
                        var entityShip = from d in context.kzShippings
                                         where d.transaction_id == transaction.TransactionId
                                         orderby d.kzTransaction.transaction_date descending
                                         select d;
                        if (entityShip.Count() > 0)
                        {
                            shipping.FirstName = entityShip.First().first_name;
                            shipping.LastName = entityShip.First().last_name;
                            shipping.Phone = entityShip.First().phone;
                            shipping.Gender = (int)entityShip.First().gender;
                            shipping.AddressLine1 = entityShip.First().address_line1;
                            shipping.AddressLine2 = entityShip.First().address_line2;
                            shipping.City = entityShip.First().city;
                            shipping.State = entityShip.First().state;
                            shipping.Country = entityShip.First().country;
                            shipping.ZipCode = entityShip.First().zipcode;
                            shipping.Gift = (bool)entityShip.First().gift;
                            shipping.Note = entityShip.First().note;
                            shipping.Email = entityShip.First().rcemail;
                        }
                        transaction.shipping = shipping;

                        Bill bill = new Bill();
                        var entityBill = from d in context.kzBillings
                                         where d.transaction_id == transaction.TransactionId
                                         orderby d.kzTransaction.transaction_date descending
                                         select d;
                        if (entityBill.Count() > 0)
                        {
                            bill.PaymentMethod = (int)entityBill.First().payment_method;
                            bill.PaymentCC = entityBill.First().payment_cc;
                            bill.PaymentCCV = "";//entityBill.First().payment_ccv;
                            bill.PaymentExpireMonth = (int)entityBill.First().payment_expm;
                            bill.PaymentExpireYear = (int)entityBill.First().payment_expy;
                            bill.FirstName = entityBill.First().first_name;
                            bill.LastName = entityBill.First().last_name;
                            bill.Phone = entityBill.First().phone;
                            bill.Gender = (int)entityBill.First().gender;
                            bill.AddressLine1 = entityBill.First().address_line1;
                            bill.AddressLine2 = entityBill.First().address_line2;
                            bill.City = entityBill.First().city;
                            bill.State = entityBill.First().state;
                            bill.Country = entityBill.First().country;
                            bill.ZipCode = entityBill.First().zipcode;
                        }
                        transaction.billing = bill;

                        int inventoryitemidjustpurchase = 0;
                        //List<int> tag = new List<int>();
                        List<TransactionsDetail> detail = new List<TransactionsDetail>();
                        var entityTransactiondetail = from d in context.kzTransactionDetails
                                                      where d.transaction_id == transaction.TransactionId
                                                      select d;
                        foreach (var v in entityTransactiondetail)
                        {
                            inventoryitemidjustpurchase = (int)v.inventoryitem_id;
                            TransactionsDetail de = new TransactionsDetail();
                            de.TransactionDetailId = (int)v.id;
                            de.TransactionId = (int)v.transaction_id;
                            de.InventoryItemId = (int)v.inventoryitem_id;
                            de.InventoryItemName = v.kzInventoryItem.name;

                            var entityImg = from d in context.kzInventoryItemImages
                                            from e in context.kzImages
                                            where d.image_id == e.id && d.inventoryitem_id == de.InventoryItemId
                                            && d.main == true
                                            select new { d.image_id, e.url };

                            if (entityImg.Count() > 0)
                            {
                                de.InventoryItemImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImg.First().url;
                            }
                            else
                            {
                                de.InventoryItemImageUrl = "";
                            }

                            if (v.flashdeal_id != null)
                            {
                                de.FlashDealId = v.flashdeal_id;
                            }
                            de.Variance = v.variance;
                            de.Qty = (int)v.qty;
                            de.Discount = (decimal)v.discount;
                            de.Price = (decimal)v.price;
                            if (v.kzInventoryItem.prize_id != null)
                            {
                                de.PrizeId = (int)v.kzInventoryItem.prize_id;
                                de.PrizeName = v.kzInventoryItem.kzPrize.name;
                                var entityImgPrize = from d in context.kzPrizeImages
                                                     from e in context.kzImages
                                                     where d.image_id == e.id && d.prize_id == de.PrizeId
                                                     && d.main == true
                                                     select new { d.image_id, e.url };

                                if (entityImgPrize.Count() > 0)
                                {
                                    de.PrizeNameImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImgPrize.First().url;
                                }
                                else
                                {
                                    de.PrizeNameImageUrl = "";
                                }
                            }
                            detail.Add(de);

                            //var entityTag = from d in context.kzInventoryItemTags
                            //                from e in context.kzTags
                            //                where d.tag_id == e.id && d.inventoryitem_id == v.inventoryitem_id
                            //                orderby d.tag_id ascending
                            //                select new { d.tag_id, e.name };
                            //foreach (var v2 in entityTag)
                            //{
                            //    if (tag.IndexOf(v2.tag_id)==-1)
                            //    {
                            //        tag.Add(v2.tag_id);
                            //    }
                            //}
                        }
                        transaction.Detail = detail;

                        #region similar tag
                        //List<InventoryItemShow> similar = new List<InventoryItemShow>();
                        //if (tag.Count() > 0)
                        //{
                        //    int invid = detail.First().InventoryItemId;
                        //    var entitySimilar = from d in context.kzInventoryItemTags
                        //                        where tag.Contains(d.tag_id) && d.inventoryitem_id != invid
                        //                        select d;
                        //    List<int> inventorySimilary = new List<int>();
                        //    foreach(var v3 in entitySimilar){
                        //        if (inventorySimilary.IndexOf(v3.inventoryitem_id)==-1)
                        //        {
                        //            inventorySimilary.Add(v3.inventoryitem_id);
                        //        }
                        //    }
                        //    var entityInvSimilar = (from d in context.kzInventoryItems
                        //                           where inventorySimilary.Contains(d.id)
                        //                           && d.expiry_date > today && today >= d.publish_date
                        //                            && d.flag == true && d.last_action != "5"
                        //                           orderby d.last_created descending
                        //                           select d).Take(4);
                        //    foreach (var v in entityInvSimilar)
                        //    {
                        //        InventoryItemShow InventoryItem = new InventoryItemShow();
                        //        InventoryItem.InventoryItemId = v.id;
                        //        InventoryItem.Name = v.name;
                        //        InventoryItem.ShortDesc = v.short_desc;
                        //        InventoryItem.Price = (decimal)v.price;
                        //        InventoryItem.TypeId = (int)v.inventoryitem_type_id;
                        //        InventoryItem.TypeName = v.kzInventoryItemType.name;
                        //        InventoryItem.Discount = (decimal)v.discount;
                        //        var entityTransaction2 = from d in context.kzTransactionDetails
                        //                                where d.inventoryitem_id == v.id
                        //                                select d;
                        //        if (entityTransaction2.Count() > 0)
                        //        {
                        //            InventoryItem.Sales = entityTransaction2.Count();
                        //        }
                        //        else
                        //        {
                        //            InventoryItem.Sales = 0;
                        //        }
                        //        InventoryItem.MaximumSales = (decimal)v.maximumsales;
                        //        InventoryItem.SalesVisualMeter = (decimal)v.salesvisualmeter;

                        //        var entityImg = from d in context.kzInventoryItemImages
                        //                        from e in context.kzImages
                        //                        where d.image_id == e.id && d.inventoryitem_id == v.id
                        //                        && d.main == true
                        //                        select new { d.image_id, e.url };

                        //        if (entityImg.Count() > 0)
                        //        {
                        //            InventoryItem.ImageId = entityImg.First().image_id;
                        //            InventoryItem.ImageName = entityImg.First().url;
                        //            InventoryItem.ImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImg.First().url;
                        //        }
                        //        else
                        //        {
                        //            InventoryItem.ImageId = 0;
                        //            InventoryItem.ImageName = "";
                        //            InventoryItem.ImageUrl = "";
                        //        }
                        //        var entityPrize = v.kzPrize;
                        //        if (entityPrize != null)
                        //        {
                        //            Prize pr = new Prize();
                        //            pr.PrizeId = (int)entityPrize.id;
                        //            pr.Name = entityPrize.name;
                        //            pr.Price = (decimal)entityPrize.price;
                        //            pr.Description = entityPrize.description == null ? string.Empty : entityPrize.description;
                        //            pr.SponsorName = entityPrize.sponsor_name == null ? string.Empty : entityPrize.sponsor_name;
                        //            pr.Terms = entityPrize.terms == null ? string.Empty : entityPrize.terms;
                        //            pr.Detail = entityPrize.detail == null ? string.Empty : entityPrize.detail;
                        //            var entityImgPrize = from d in context.kzPrizeImages
                        //                                 from e in context.kzImages
                        //                                 where d.image_id == e.id && d.prize_id == pr.PrizeId
                        //                                 && d.main == true
                        //                                 select new { d.image_id, e.url };

                        //            if (entityImgPrize.Count() > 0)
                        //            {
                        //                pr.ImageId = entityImgPrize.First().image_id;
                        //                pr.ImageName = entityImgPrize.First().url;
                        //                pr.ImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImgPrize.First().url;
                        //            }
                        //            else
                        //            {
                        //                pr.ImageId = 0;
                        //                pr.ImageName = "";
                        //                pr.ImageUrl = "";
                        //            }

                        //            InventoryItem.Prize = pr;
                        //        }
                        //        var entityFlashDeal = from d in context.kzFlashDeals.Include("kzInventoryItem")
                        //                              where d.inventoryitem_id == v.id && d.last_action != "5" && d.flag == true
                        //                              && today >= d.start_time && today <= d.end_time
                        //                              select d;
                        //        if (entityFlashDeal.Count() > 0)
                        //        {
                        //            FlashDeal FlashDeal = new FlashDeal();
                        //            FlashDeal.FlashDealId = entityFlashDeal.First().id;
                        //            FlashDeal.MerchantId = (int)entityFlashDeal.First().kzInventoryItem.merchant_id;
                        //            FlashDeal.MerchantName = entityFlashDeal.First().kzInventoryItem.kzMerchant.name;
                        //            FlashDeal.InventoryItemId = (int)entityFlashDeal.First().inventoryitem_id;
                        //            FlashDeal.InventoryItemName = entityFlashDeal.First().kzInventoryItem.name;
                        //            FlashDeal.Discount = (decimal)entityFlashDeal.First().discount;
                        //            FlashDeal.StartTime = (DateTime)entityFlashDeal.First().start_time;
                        //            FlashDeal.EndTime = (DateTime)entityFlashDeal.First().end_time;
                        //            FlashDeal.Flag = (Boolean)entityFlashDeal.First().flag;
                        //            FlashDeal.LastAction = entityFlashDeal.First().last_action;
                        //            InventoryItem.FlashDeal = FlashDeal;
                        //        }
                        //        similar.Add(InventoryItem);
                        //    }
                        //}
                        #endregion
                        transaction.SimilarDetail = new InventoryItemService().GetInventoryItemListExpireSoon4(today, inventoryitemidjustpurchase).Result;

                    }
                }
                response = Response<TransactionsHeader>.Create(transaction);
            }

            return response;
        }

        public Response<List<TransactionsHeader>> GetTransactionsListByUser()
        {
            Response<List<TransactionsHeader>> response = null;
            List<TransactionsHeader> transactionlist = new List<TransactionsHeader>();
            using (var context = new entity.KuazooEntities())
            {
                var user = new UserService().GetCurrentUser().Result;
                var entityTransaction = from d in context.kzTransactions
                                        where d.user_id == user.UserId && d.last_action != "5"
                                        && d.tranID != null && (d.status == "22" || d.status == "00")
                                        orderby d.transaction_date descending
                                        select d;
                foreach (var trans in entityTransaction)
                {
                    TransactionsHeader transaction = new TransactionsHeader();
                    transaction.TransactionId = trans.id;
                    transaction.TransactionStatus = (int)trans.transaction_status;
                    transaction.MemberId = (int)trans.user_id;
                    transaction.TransactionDate = (DateTime)trans.transaction_date;
                    transaction.KPoint = (decimal)trans.kpoint;
                    transaction.PromotionId = 0;
                    if (trans.promotion_id != null && trans.kzPromotion != null)
                    {
                        transaction.PromotionId = (int)trans.promotion_id;
                        transaction.PromotionType = (int)trans.kzPromotion.type;
                        transaction.PromotionValue = (decimal)trans.kzPromotion.value;
                    }

                    transaction.PaymentStatus = "";
                    if (trans.status == "00")
                    {
                        transaction.PaymentStatus = "Successful";
                    }
                    else if (trans.status == "22")
                    {
                        transaction.PaymentStatus = "Pending";
                    }
                    transaction.VoucherCode = "";
                    var voucherEntities = from d in context.kzPreCodes
                                          where d.transaction_id == transaction.TransactionId
                                          select d;
                    if (voucherEntities.Count() > 0)
                    {
                        List<string> _vo = new List<string>();
                        foreach (var vo in voucherEntities)
                        {
                            _vo.Add(vo.code);
                        }
                        transaction.VoucherCode = string.Join(", ", _vo);
                    }

                    Bill shipping = new Bill();
                    var entityShip = from d in context.kzShippings
                                     where d.transaction_id == transaction.TransactionId
                                     orderby d.kzTransaction.transaction_date descending
                                     select d;
                    if (entityShip.Count() > 0)
                    {
                        shipping.FirstName = entityShip.First().first_name;
                        shipping.LastName = entityShip.First().last_name;
                        shipping.Phone = entityShip.First().phone;
                        shipping.Gender = (int)entityShip.First().gender;
                        shipping.AddressLine1 = entityShip.First().address_line1;
                        shipping.AddressLine2 = entityShip.First().address_line2;
                        shipping.City = entityShip.First().city;
                        shipping.State = entityShip.First().state;
                        shipping.Country = entityShip.First().country;
                        shipping.ZipCode = entityShip.First().zipcode;
                        shipping.Gift = (bool)entityShip.First().gift;
                        shipping.Note = entityShip.First().note;
                        shipping.Email = entityShip.First().rcemail;
                    }
                    transaction.shipping = shipping;

                    Bill bill = new Bill();
                    var entityBill = from d in context.kzBillings
                                     where d.transaction_id == transaction.TransactionId
                                     orderby d.kzTransaction.transaction_date descending
                                     select d;
                    if (entityBill.Count() > 0)
                    {
                        bill.PaymentMethod = (int)entityBill.First().payment_method;
                        bill.PaymentCC = entityBill.First().payment_cc;
                        bill.PaymentCCV = "";//entityBill.First().payment_ccv;
                        bill.PaymentExpireMonth = (int)entityBill.First().payment_expm;
                        bill.PaymentExpireYear = (int)entityBill.First().payment_expy;
                        bill.FirstName = entityBill.First().first_name;
                        bill.LastName = entityBill.First().last_name;
                        bill.Phone = entityBill.First().phone;
                        bill.Gender = (int)entityBill.First().gender;
                        bill.AddressLine1 = entityBill.First().address_line1;
                        bill.AddressLine2 = entityBill.First().address_line2;
                        bill.City = entityBill.First().city;
                        bill.State = entityBill.First().state;
                        bill.Country = entityBill.First().country;
                        bill.ZipCode = entityBill.First().zipcode;
                    }
                    transaction.billing = bill;

                    List<TransactionsDetail> detail = new List<TransactionsDetail>();
                    var entityTransactiondetail = from d in context.kzTransactionDetails
                                                  where d.transaction_id == transaction.TransactionId
                                                  select d;
                    foreach (var v in entityTransactiondetail)
                    {
                        TransactionsDetail de = new TransactionsDetail();
                        de.TransactionDetailId = (int)v.id;
                        de.TransactionId = (int)v.transaction_id;
                        de.InventoryItemId = (int)v.inventoryitem_id;
                        de.InventoryItemName = v.kzInventoryItem.name;
                        de.InventoryItemRemainSales = (decimal)v.kzInventoryItem.remainsales;
                        de.InventoryItemSalesVisualMeter = (decimal)v.kzInventoryItem.salesvisualmeter;

                        var entityImg = from d in context.kzInventoryItemImages
                                        from e in context.kzImages
                                        where d.image_id == e.id && d.inventoryitem_id == de.InventoryItemId
                                        && d.main == true
                                        select new { d.image_id, e.url };

                        if (entityImg.Count() > 0)
                        {
                            de.InventoryItemImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImg.First().url;
                        }
                        else
                        {
                            de.InventoryItemImageUrl = "";
                        }

                        if (v.flashdeal_id != null)
                        {
                            de.FlashDealId = v.flashdeal_id;
                        }
                        de.Variance = v.variance;
                        de.Qty = (int)v.qty;
                        de.Discount = (decimal)v.discount;
                        de.Price = (decimal)v.price;
                        if (v.kzInventoryItem.prize_id != null)
                        {
                            de.PrizeId = (int)v.kzInventoryItem.prize_id;
                            de.PrizeName = v.kzInventoryItem.kzPrize.name;
                            de.PrizeExpire = (DateTime)v.kzInventoryItem.kzPrize.expiry_date;
                            var entityImgPrize = from d in context.kzPrizeImages
                                                 from e in context.kzImages
                                                 where d.image_id == e.id && d.prize_id == de.PrizeId
                                                 && d.main == true
                                                 select new { d.image_id, e.url };

                            if (entityImgPrize.Count() > 0)
                            {
                                de.PrizeNameImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImgPrize.First().url;
                            }
                            else
                            {
                                de.PrizeNameImageUrl = "";
                            }
                        }
                        detail.Add(de);
                    }
                    transaction.Detail = detail;

                    transaction.reviewed = ReviewService.GetReviewByInventoryItemByUser(context, user.UserId, detail[0].InventoryItemId);

                    transactionlist.Add(transaction);
                }
                response = Response<List<TransactionsHeader>>.Create(transactionlist);
            }

            return response;
        }

        public Response<List<Transaction>> GetTransactionList()
        {
            Response<List<Transaction>> response = null;
            List<Transaction> TransactionList = new List<Transaction>();
            using (var context = new entity.KuazooEntities())
            {
                var entityTransaction = from d in context.kzTransactionDetails.Include("kzInventoryItem")
                                        group d by d.inventoryitem_id into g
                                        select new
                                        {
                                            InventoryItemId = g.Key,
                                            InventoryItemName = g.FirstOrDefault().kzInventoryItem.name,
                                            Sales = g.Count(),
                                            MaximumSales = g.FirstOrDefault().kzInventoryItem.maximumsales,
                                            MerchantId = g.FirstOrDefault().kzInventoryItem.merchant_id,
                                            MerchantName = g.FirstOrDefault().kzInventoryItem.kzMerchant.name,
                                            ExpireDate = g.FirstOrDefault().kzInventoryItem.kzPrize.expiry_date,
                                            SalesVisualMeter = g.FirstOrDefault().kzInventoryItem.salesvisualmeter
                                        };
                foreach (var v in entityTransaction)
                {
                    Transaction Transaction = new Transaction();
                    Transaction.MerchantId = (int)v.MerchantId;
                    Transaction.MerchantName = v.MerchantName;
                    Transaction.InventoryItemId = (int)v.InventoryItemId;
                    Transaction.InventoryItemName = v.InventoryItemName;
                    Transaction.Sales = v.Sales;
                    Transaction.NewOrder = 0;
                    var entitTran = from d in context.kzTransactionDetails
                                    where d.inventoryitem_id == Transaction.InventoryItemId && d.kzTransaction.last_action != "5"
                                    && d.kzTransaction.transaction_status != 1 && d.kzTransaction.transaction_status != 2
                                    select d;
                    Transaction.NewOrder = entitTran.Count();
                    Transaction.MaximumSales = (decimal)v.MaximumSales;
                    Transaction.SalesVisualMeter = (decimal)v.SalesVisualMeter;
                    Transaction.ExpireDate = (DateTime)v.ExpireDate;
                    TransactionList.Add(Transaction);
                }
                response = Response<List<Transaction>>.Create(TransactionList);
            }

            return response;
        }
        public Response<List<Transaction>> GetTransactionListHot()
        {
            Response<List<Transaction>> response = null;
            List<Transaction> TransactionList = new List<Transaction>();
            using (var context = new entity.KuazooEntities())
            {
                var entityTransaction = from d in context.kzTransactionDetails.Include("kzInventoryItem")
                                        group d by d.inventoryitem_id into g
                                        select new
                                        {
                                            InventoryItemId = g.Key,
                                            InventoryItemName = g.FirstOrDefault().kzInventoryItem.name,
                                            Sales = g.Count()
                                        };
                foreach (var v in entityTransaction.OrderByDescending(x => x.Sales).Take(3))
                {
                    Transaction Transaction = new Transaction();
                    Transaction.InventoryItemId = (int)v.InventoryItemId;
                    Transaction.InventoryItemName = v.InventoryItemName;
                    Transaction.Sales = v.Sales;
                    TransactionList.Add(Transaction);
                }
                response = Response<List<Transaction>>.Create(TransactionList);
            }

            return response;
        }
        public Response<List<TransactionDetail>> GetTransactionDetailByInventoryItemId(int InventoryItemId)
        {
            Response<List<TransactionDetail>> response = null;
            List<TransactionDetail> TransactionList = new List<TransactionDetail>();
            using (var context = new entity.KuazooEntities())
            {
                var entityTransaction = from d in context.kzTransactionDetails.Include("kzInventoryItem")
                                        where d.inventoryitem_id == InventoryItemId
                                        orderby d.kzTransaction.transaction_date descending
                                        select d;
                foreach (var v in entityTransaction)
                {
                    TransactionDetail Transaction = new TransactionDetail();
                    Transaction.TransactionId = v.id;
                    Transaction.MerchantId = (int)v.kzInventoryItem.merchant_id;
                    Transaction.MerchantName = v.kzInventoryItem.kzMerchant.name;
                    Transaction.InventoryItemId = (int)v.inventoryitem_id;
                    Transaction.InventoryItemName = v.kzInventoryItem.name;
                    Transaction.MemberId = (int)v.kzTransaction.user_id;
                    Transaction.MemberEmail = v.kzTransaction.kzUser.email;
                    Transaction.TransactionDate = (DateTime)v.kzTransaction.transaction_date;
                    if (v.flashdeal_id != null)
                    {
                        Transaction.FlashDealId = v.flashdeal_id;
                    }
                    Transaction.LastAction = v.kzTransaction.last_action;
                    Transaction.TransactionTypeId = (int)v.kzTransaction.transaction_status;
                    TransactionStatus tyid = (TransactionStatus)v.kzTransaction.transaction_status;
                    Transaction.TransactionType = tyid.ToString();
                    if (v.kzTransaction.process_date != null)
                    {
                        Transaction.ProcessDate = (DateTime)v.kzTransaction.process_date;
                    }
                    TransactionList.Add(Transaction);
                }
                response = Response<List<TransactionDetail>>.Create(TransactionList);
            }

            return response;
        }

        public Response<List<TransactionDetail3>> GetAllTransactionList()
        {
            Response<List<TransactionDetail3>> response = null;
            List<TransactionDetail3> TransactionList = new List<TransactionDetail3>();
            using (var context = new entity.KuazooEntities())
            {
                var entityTransaction = from d in context.kzTransactionDetails.Include("kzInventoryItem")
                                        where d.kzTransaction.last_action != "5"
                                        && (d.kzTransaction.status != "11" || d.kzTransaction.status == null)
                                        orderby d.kzTransaction.transaction_date descending
                                        select d;
                foreach (var v in entityTransaction)
                {
                    TransactionDetail3 Transaction = new TransactionDetail3();
                    Transaction.TransactionId = (int)v.transaction_id;
                    Transaction.TransactionDate = (DateTime)v.kzTransaction.transaction_date;
                    Transaction.MerchantId = (int)v.kzInventoryItem.merchant_id;
                    Transaction.MerchantName = v.kzInventoryItem.kzMerchant.name;
                    Transaction.InventoryItemId = (int)v.inventoryitem_id;
                    Transaction.InventoryItemName = v.kzInventoryItem.name;
                    string variance = v.variance.Split('`')[0];
                    Transaction.Variance = variance;
                    var varianceentity = from d in context.kzVariances
                                         where d.inventoryitem_id == Transaction.InventoryItemId
                                         && d.name.ToLower() == variance.ToLower()
                                         select d;
                    Transaction.SKU = "";
                    if (varianceentity.Count() > 0)
                    {
                        Transaction.SKU = varianceentity.First().sku.ToString();
                    }
                    Transaction.Qty = (int)v.qty;
                    var voucherEntities = from d in context.kzPreCodes
                                          where d.transaction_id == Transaction.TransactionId
                                          select d;
                    List<string> _vo = new List<string>();
                    foreach (var vo in voucherEntities)
                    {
                        _vo.Add(vo.code);
                    }
                    Transaction.VoucherCode = string.Join(", ", _vo);

                    Transaction.MemberId = (int)v.kzTransaction.user_id;
                    Transaction.MemberEmail = v.kzTransaction.kzUser.email;
                    if (v.kzTransaction.kzShippings.Count() > 0)
                    {
                        Transaction.MemberFirstName = v.kzTransaction.kzShippings.First().first_name;
                        Transaction.MemberLastName = v.kzTransaction.kzShippings.First().last_name;
                        Transaction.MemberPhone = v.kzTransaction.kzShippings.First().phone;
                        Transaction.MemberAddress1 = v.kzTransaction.kzShippings.First().address_line1;
                        Transaction.MemberAddress2 = v.kzTransaction.kzShippings.First().address_line2;
                        Transaction.MemberCity = v.kzTransaction.kzShippings.First().city;
                        Transaction.MemberState = v.kzTransaction.kzShippings.First().state;
                        Transaction.MemberPostCode = v.kzTransaction.kzShippings.First().zipcode;
                        Transaction.MemberCountry = v.kzTransaction.kzShippings.First().country;
                        Transaction.Gift = (bool)v.kzTransaction.kzShippings.First().gift;
                        Transaction.GiftNote = v.kzTransaction.kzShippings.First().note;
                    }

                    //Transaction.ProcessStatusId = (int)v.kzTransaction.transaction_status;
                    TransactionStatus tyid = (TransactionStatus)v.kzTransaction.transaction_status;
                    Transaction.TransactionStatus = tyid.ToString();
                    if (v.kzTransaction.process_status != null)
                    {
                        ProcessStatus pcid = (ProcessStatus)v.kzTransaction.process_status;
                        Transaction.ProcessStatus = pcid.ToString();
                    }
                    else
                    {
                        Transaction.ProcessStatus = ProcessStatus.No.ToString();
                    }
                    if (v.kzTransaction.process_date != null)
                    {
                        Transaction.ProcessDate = (DateTime)v.kzTransaction.process_date;
                    }

                    if (v.kzTransaction.tranID != null && v.kzTransaction.status != null && v.kzTransaction.paydate != null)
                    {
                        Transaction.PaymentStatusId = v.kzTransaction.status;
                        if (v.kzTransaction.status == "00")
                        {
                            Transaction.PaymentStatus = "Success";
                        }
                        else if (v.kzTransaction.status == "22")
                        {
                            Transaction.PaymentStatus = "Pending";
                        }
                        else
                        {
                            Transaction.PaymentStatus = "Failed";
                        }
                        Transaction.PaymentDate = (DateTime)v.kzTransaction.paydate;
                        Transaction.PaymentAmount = v.kzTransaction.amount ?? 0M;
                    }
                    else
                    {
                        Transaction.PaymentStatus = "Pending Verification";
                    }

                    Transaction.PaymentMethod = v.kzTransaction.channel;

                    TransactionList.Add(Transaction);
                }
                response = Response<List<TransactionDetail3>>.Create(TransactionList);
            }

            return response;
        }
        public Response<List<TransactionDetail3>> GetAllTransactionFailedList()
        {
            Response<List<TransactionDetail3>> response = null;
            List<TransactionDetail3> TransactionList = new List<TransactionDetail3>();
            using (var context = new entity.KuazooEntities())
            {
                var entityTransaction = from d in context.kzTransactionDetails.Include("kzInventoryItem")
                                        where d.kzTransaction.last_action != "5"
                                        && d.kzTransaction.tranID != null && d.kzTransaction.status == "11"
                                        orderby d.kzTransaction.transaction_date descending
                                        select d;
                foreach (var v in entityTransaction)
                {
                    TransactionDetail3 Transaction = new TransactionDetail3();
                    Transaction.TransactionId = (int)v.transaction_id;
                    Transaction.TransactionDate = (DateTime)v.kzTransaction.transaction_date;
                    Transaction.MerchantId = (int)v.kzInventoryItem.merchant_id;
                    Transaction.MerchantName = v.kzInventoryItem.kzMerchant.name;
                    Transaction.InventoryItemId = (int)v.inventoryitem_id;
                    Transaction.InventoryItemName = v.kzInventoryItem.name;
                    string variance = v.variance.Split('`')[0];
                    Transaction.Variance = variance;
                    var varianceentity = from d in context.kzVariances
                                         where d.inventoryitem_id == Transaction.InventoryItemId
                                         && d.name.ToLower() == variance.ToLower()
                                         select d;
                    Transaction.SKU = "";
                    if (varianceentity.Count() > 0)
                    {
                        Transaction.SKU = varianceentity.First().sku.ToString();
                    }
                    Transaction.Qty = (int)v.qty;
                    Transaction.MemberId = (int)v.kzTransaction.user_id;
                    Transaction.MemberEmail = v.kzTransaction.kzUser.email;
                    Transaction.MemberFirstName = v.kzTransaction.kzShippings.First().first_name;
                    Transaction.MemberLastName = v.kzTransaction.kzShippings.First().last_name;
                    Transaction.MemberPhone = v.kzTransaction.kzShippings.First().phone;
                    Transaction.MemberAddress1 = v.kzTransaction.kzShippings.First().address_line1;
                    Transaction.MemberAddress2 = v.kzTransaction.kzShippings.First().address_line2;
                    Transaction.MemberCity = v.kzTransaction.kzShippings.First().city;
                    Transaction.MemberState = v.kzTransaction.kzShippings.First().state;
                    Transaction.MemberPostCode = v.kzTransaction.kzShippings.First().zipcode;
                    Transaction.MemberCountry = v.kzTransaction.kzShippings.First().country;

                    //Transaction.ProcessStatusId = (int)v.kzTransaction.transaction_status;
                    TransactionStatus tyid = (TransactionStatus)v.kzTransaction.transaction_status;
                    Transaction.TransactionStatus = tyid.ToString();
                    if (v.kzTransaction.process_status != null)
                    {
                        ProcessStatus pcid = (ProcessStatus)v.kzTransaction.process_status;
                        Transaction.ProcessStatus = pcid.ToString();
                    }
                    else
                    {
                        Transaction.ProcessStatus = ProcessStatus.No.ToString();
                    }
                    if (v.kzTransaction.process_date != null)
                    {
                        Transaction.ProcessDate = (DateTime)v.kzTransaction.process_date;
                    }

                    if (v.kzTransaction.tranID != null && v.kzTransaction.status != null && v.kzTransaction.paydate != null)
                    {
                        Transaction.PaymentStatusId = v.kzTransaction.status;
                        if (v.kzTransaction.status == "00")
                        {
                            Transaction.PaymentStatus = "Success";
                        }
                        else if (v.kzTransaction.status == "22")
                        {
                            Transaction.PaymentStatus = "Pending";
                        }
                        else
                        {
                            Transaction.PaymentStatus = "Failed";
                        }
                        Transaction.PaymentDate = (DateTime)v.kzTransaction.paydate;
                        Transaction.PaymentAmount = v.kzTransaction.amount ?? 0M;
                    }

                    TransactionList.Add(Transaction);
                }
                response = Response<List<TransactionDetail3>>.Create(TransactionList);
            }

            return response;
        }
        public Response<List<TransactionDetail3>> GetSelectedTransactionList(string transactionlist)
        {
            Response<List<TransactionDetail3>> response = null;
            List<TransactionDetail3> TransactionList = new List<TransactionDetail3>();
            using (var context = new entity.KuazooEntities())
            {
                List<int> listtrans = new List<int>();
                string[] _tranlsit = transactionlist.Split(',');
                foreach (var v in _tranlsit)
                {
                    try
                    {
                        if (v != "")
                        {
                            listtrans.Add(int.Parse(v));
                        }
                    }
                    catch { }
                }
                var entityTransaction = from d in context.kzTransactionDetails.Include("kzInventoryItem")
                                        where d.kzTransaction.last_action != "5" && listtrans.Contains(d.transaction_id.Value)
                                        orderby d.kzTransaction.transaction_date descending
                                        select d;
                foreach (var v in entityTransaction)
                {
                    TransactionDetail3 Transaction = new TransactionDetail3();
                    Transaction.TransactionId = (int)v.transaction_id;
                    Transaction.TransactionDate = (DateTime)v.kzTransaction.transaction_date;
                    Transaction.MerchantId = (int)v.kzInventoryItem.merchant_id;
                    Transaction.MerchantName = v.kzInventoryItem.kzMerchant.name;
                    Transaction.InventoryItemId = (int)v.inventoryitem_id;
                    Transaction.InventoryItemName = v.kzInventoryItem.name;
                    string variance = v.variance.Split('`')[0];
                    Transaction.Variance = variance;
                    var varianceentity = from d in context.kzVariances
                                         where d.inventoryitem_id == Transaction.InventoryItemId
                                         && d.name.ToLower() == variance.ToLower()
                                         select d;
                    Transaction.SKU = "";
                    if (varianceentity.Count() > 0)
                    {
                        Transaction.SKU = varianceentity.First().sku.ToString();
                    }
                    Transaction.Qty = (int)v.qty;
                    var voucherEntities = from d in context.kzPreCodes
                                          where d.transaction_id == Transaction.TransactionId
                                          select d;
                    List<string> _vo = new List<string>();
                    foreach (var vo in voucherEntities)
                    {
                        _vo.Add(vo.code);
                    }
                    Transaction.VoucherCode = string.Join(", ", _vo);
                    Transaction.MemberId = (int)v.kzTransaction.user_id;
                    Transaction.MemberEmail = v.kzTransaction.kzUser.email;
                    if (v.kzTransaction.kzShippings.Count() > 0)
                    {
                        Transaction.MemberFirstName = v.kzTransaction.kzShippings.First().first_name;
                        Transaction.MemberLastName = v.kzTransaction.kzShippings.First().last_name;
                        Transaction.MemberPhone = v.kzTransaction.kzShippings.First().phone;
                        Transaction.MemberAddress1 = v.kzTransaction.kzShippings.First().address_line1;
                        Transaction.MemberAddress2 = v.kzTransaction.kzShippings.First().address_line2;
                        Transaction.MemberCity = v.kzTransaction.kzShippings.First().city;
                        Transaction.MemberState = v.kzTransaction.kzShippings.First().state;
                        Transaction.MemberPostCode = v.kzTransaction.kzShippings.First().zipcode;
                        Transaction.MemberCountry = v.kzTransaction.kzShippings.First().country;
                        Transaction.Gift = (bool)v.kzTransaction.kzShippings.First().gift;
                        Transaction.GiftNote = v.kzTransaction.kzShippings.First().note;
                    }

                    //Transaction.ProcessStatusId = (int)v.kzTransaction.transaction_status;
                    TransactionStatus tyid = (TransactionStatus)v.kzTransaction.transaction_status;
                    Transaction.TransactionStatus = tyid.ToString();
                    if (v.kzTransaction.process_status != null)
                    {
                        ProcessStatus pcid = (ProcessStatus)v.kzTransaction.process_status;
                        Transaction.ProcessStatus = pcid.ToString();
                    }
                    else
                    {
                        Transaction.ProcessStatus = ProcessStatus.No.ToString();
                    }
                    if (v.kzTransaction.process_date != null)
                    {
                        Transaction.ProcessDate = (DateTime)v.kzTransaction.process_date;
                    }

                    if (v.kzTransaction.tranID != null && v.kzTransaction.status != null && v.kzTransaction.paydate != null)
                    {
                        Transaction.PaymentStatusId = v.kzTransaction.status;
                        if (v.kzTransaction.status == "00")
                        {
                            Transaction.PaymentStatus = "Success";
                        }
                        else if (v.kzTransaction.status == "22")
                        {
                            Transaction.PaymentStatus = "Pending";
                        }
                        else
                        {
                            Transaction.PaymentStatus = "Failed";
                        }
                        Transaction.PaymentDate = (DateTime)v.kzTransaction.paydate;
                        Transaction.PaymentAmount = v.kzTransaction.amount ?? 0M;
                    }

                    Transaction.PaymentMethod = v.kzTransaction.channel;

                    TransactionList.Add(Transaction);
                }
                response = Response<List<TransactionDetail3>>.Create(TransactionList);
            }

            return response;
        }

        public Response<Boolean> UpdateSelectedTransactionList(string transactionlist)
        {
            Response<Boolean> response = null;
            using (var context = new entity.KuazooEntities())
            {
                List<int> listtrans = new List<int>();
                string[] _tranlsit = transactionlist.Split(',');
                foreach (var v in _tranlsit)
                {
                    try
                    {
                        if (v != "")
                        {
                            listtrans.Add(int.Parse(v));
                        }
                    }
                    catch { }
                }
                DateTime now = DateTime.UtcNow;
                var entityTransaction = from d in context.kzTransactions
                                        where d.last_action != "5" && listtrans.Contains(d.id)
                                        select d;
                foreach (var v in entityTransaction)
                {
                    if (v.process_status == null || v.process_status != 1)
                    {
                        v.process_status = 1;
                        v.process_date = now;
                    }
                }
                context.SaveChanges();
                response = Response<Boolean>.Create(true);
            }

            return response;
        }

        public Response<TransactionDetail> GetTransactionDetailById(int TransactionId)
        {
            Response<TransactionDetail> response = null;
            TransactionDetail Transaction = new TransactionDetail();
            using (var context = new entity.KuazooEntities())
            {
                var entityTransaction = from d in context.kzTransactionDetails.Include("kzInventoryItem")
                                        where d.id == TransactionId
                                        select d;
                var v = entityTransaction.First();
                if (v != null)
                {
                    Transaction.TransactionId = v.id;
                    Transaction.MerchantId = (int)v.kzInventoryItem.merchant_id;
                    Transaction.MerchantName = v.kzInventoryItem.kzMerchant.name;
                    Transaction.InventoryItemId = (int)v.inventoryitem_id;
                    Transaction.InventoryItemName = v.kzInventoryItem.name;
                    Transaction.MemberId = (int)v.kzTransaction.user_id;
                    Transaction.MemberEmail = v.kzTransaction.kzUser.email;
                    Transaction.TransactionDate = (DateTime)v.kzTransaction.transaction_date;
                    Transaction.InventoryItem = new InventoryItemService().GetInventoryItemById((int)v.inventoryitem_id).Result;
                    Transaction.Member = new MemberService().GetMemberById((int)v.kzTransaction.user_id).Result;
                    if (v.flashdeal_id != null)
                    {
                        Transaction.FlashDealId = v.flashdeal_id;
                        //Transaction.FlashDeal = new FlashDealService().GetFlashDealById((int)v.flashdeal_id).Result;
                    }
                    Transaction.LastAction = v.kzTransaction.last_action;
                }
                response = Response<TransactionDetail>.Create(Transaction);
            }

            return response;
        }

        public Response<List<TransactionDetail>> GetTransactionDetailIdDate(int InventoryItemId, DateTime From, DateTime To)
        {
            Response<List<TransactionDetail>> response = null;
            List<TransactionDetail> TransactionList = new List<TransactionDetail>();
            using (var context = new entity.KuazooEntities())
            {
                var entityTransaction = from d in context.kzTransactionDetails.Include("kzInventoryItem")
                                        where d.kzTransaction.transaction_date >= From && d.kzTransaction.transaction_date <= To
                                        select d;
                if (InventoryItemId != 0)
                {
                    entityTransaction = entityTransaction.Where(x => x.inventoryitem_id == InventoryItemId);
                }
                foreach (var v in entityTransaction)
                {
                    TransactionDetail Transaction = new TransactionDetail();
                    Transaction.TransactionId = v.id;
                    Transaction.MerchantId = (int)v.kzInventoryItem.merchant_id;
                    Transaction.MerchantName = v.kzInventoryItem.kzMerchant.name;
                    Transaction.InventoryItemId = (int)v.inventoryitem_id;
                    Transaction.InventoryItemName = v.kzInventoryItem.name;
                    Transaction.MemberId = (int)v.kzTransaction.user_id;
                    Transaction.MemberEmail = v.kzTransaction.kzUser.email;
                    Transaction.TransactionDate = (DateTime)v.kzTransaction.transaction_date;
                    if (v.flashdeal_id != null)
                    {
                        Transaction.FlashDealId = v.flashdeal_id;
                        Transaction.FlashDeal = new FlashDealService().GetFlashDealById((int)v.flashdeal_id).Result;
                    }
                    Transaction.LastAction = v.kzTransaction.last_action;
                    TransactionList.Add(Transaction);
                }
                response = Response<List<TransactionDetail>>.Create(TransactionList);
            }

            return response;
        }

        public Response<List<TransactionDetail2>> GetTransactionDetailIdDate2(int InventoryItemId, DateTime From, DateTime To)
        {
            Response<List<TransactionDetail2>> response = null;
            List<TransactionDetail2> TransactionList = new List<TransactionDetail2>();
            using (var context = new entity.KuazooEntities())
            {
                var entityTransaction = from d in context.kzTransactionDetails.Include("kzInventoryItem")
                                        where d.kzTransaction.transaction_date >= From && d.kzTransaction.transaction_date <= To
                                        select d;
                if (InventoryItemId != 0)
                {
                    entityTransaction = entityTransaction.Where(x => x.inventoryitem_id == InventoryItemId);
                }
                Random rnd = new Random();
                foreach (var v in entityTransaction)
                {
                    TransactionDetail2 Transaction = new TransactionDetail2();
                    Transaction.TransactionId = v.id;
                    Transaction.OrderNo = GeneralService.TransactionCode(v.id, (DateTime)v.kzTransaction.transaction_date);
                    Transaction.TransactionDate = (DateTime)v.kzTransaction.transaction_date;
                    Transaction.MerchantId = (int)v.kzInventoryItem.merchant_id;
                    Transaction.MerchantName = v.kzInventoryItem.kzMerchant.name;
                    Transaction.InventoryItemId = (int)v.inventoryitem_id;
                    Transaction.InventoryItemName = v.kzInventoryItem.name;
                    Transaction.SKU = "";
                    var variance = v.variance.Split('`');
                    if (variance.Length > 0)
                    {
                        string variancename = variance[0].ToLower();
                        var entityVarieance = from d in context.kzVariances
                                              where d.inventoryitem_id == Transaction.InventoryItemId
                                              && d.name.ToLower() == variancename
                                              select d;
                        if (entityVarieance.Count() > 0)
                        {
                            Transaction.SKU = entityVarieance.First().sku;
                        }
                    }
                    Transaction.Qty = (int)v.qty;
                    Transaction.MemberId = (int)v.kzTransaction.user_id;
                    Transaction.MemberEmail = v.kzTransaction.kzUser.email;
                    Transaction.MemberFirstName = "";
                    Transaction.MemberLastName = "";
                    Transaction.Phone = "";
                    Transaction.AddressLine1 = "";
                    Transaction.AddressLine2 = "";
                    Transaction.City = "";
                    Transaction.Country = "";
                    Transaction.State = "";
                    if (v.kzTransaction.kzShippings.Count() > 0)
                    {
                        Transaction.MemberFirstName = v.kzTransaction.kzShippings.First().first_name;
                        Transaction.MemberLastName = v.kzTransaction.kzShippings.First().last_name;
                        Transaction.Phone = v.kzTransaction.kzShippings.First().phone;
                        Transaction.AddressLine1 = v.kzTransaction.kzShippings.First().address_line1;
                        Transaction.AddressLine2 = v.kzTransaction.kzShippings.First().address_line2;
                        Transaction.City = v.kzTransaction.kzShippings.First().city;
                        Transaction.Country = v.kzTransaction.kzShippings.First().country;
                        Transaction.State = v.kzTransaction.kzShippings.First().state;
                        Transaction.ZipCode = v.kzTransaction.kzShippings.First().zipcode;
                    }
                    TransactionList.Add(Transaction);
                }
                response = Response<List<TransactionDetail2>>.Create(TransactionList);
            }

            return response;
        }
        public Response<List<TransactionDetail>> GetTransactionDetailByPrize(int PrizeId)
        {
            Response<List<TransactionDetail>> response = null;
            List<TransactionDetail> TransactionList = new List<TransactionDetail>();
            using (var context = new entity.KuazooEntities())
            {
                var entityTransaction = from d in context.kzTransactionDetails.Include("kzInventoryItem")
                                        where d.kzInventoryItem.prize_id == PrizeId
                                        orderby d.kzTransaction.transaction_date descending
                                        select d;
                foreach (var v in entityTransaction)
                {
                    TransactionDetail Transaction = new TransactionDetail();
                    Transaction.TransactionId = v.id;
                    Transaction.MerchantId = (int)v.kzInventoryItem.merchant_id;
                    Transaction.MerchantName = v.kzInventoryItem.kzMerchant.name;
                    Transaction.InventoryItemId = (int)v.inventoryitem_id;
                    Transaction.InventoryItemName = v.kzInventoryItem.name;
                    Transaction.MemberId = (int)v.kzTransaction.user_id;
                    Transaction.MemberEmail = v.kzTransaction.kzUser.email;
                    //var entityGame = from d in context.kzGameTransactions
                    //                 where d.kzGame.prize_id == v.kzInventoryItem.prize_id
                    //                 && d.user_id == Transaction.MemberId
                    //                 select d;
                    //Transaction.MemberPlay = false;
                    //if (entityGame.Count() > 0)
                    //{
                    //    Transaction.MemberPlay = true;
                    //}
                    Transaction.MemberPlay = false;
                    if (v.kzTransaction.kzGameTransactions.Count() > 0)
                    {
                        Transaction.MemberPlay = true;
                    }
                    Transaction.TransactionDate = (DateTime)v.kzTransaction.transaction_date;
                    if (v.flashdeal_id != null)
                    {
                        Transaction.FlashDealId = v.flashdeal_id;
                        Transaction.FlashDeal = new FlashDealService().GetFlashDealById((int)v.flashdeal_id).Result;
                    }

                    Transaction.PaymentStatus = "";
                    if (v.kzTransaction.status == "00")
                    {
                        Transaction.PaymentStatus = "Successful";
                    }
                    else if (v.kzTransaction.status == "22")
                    {
                        Transaction.PaymentStatus = "Pending";
                    }

                    Transaction.LastAction = v.kzTransaction.last_action;
                    TransactionList.Add(Transaction);
                }
                response = Response<List<TransactionDetail>>.Create(TransactionList);
            }

            return response;
        }
        public Response<List<TransactionDetail>> GetTransactionDetailByPrizeGame51Win(int PrizeId)
        {
            Response<List<TransactionDetail>> response = null;
            List<TransactionDetail> TransactionList = new List<TransactionDetail>();
            int nostart = 0;
            int asdes = 0;
            using (var context = new entity.KuazooEntities())
            {
                var entityTransaction = from d in context.kzTransactionDetails.Include("kzInventoryItem")
                                        where d.kzInventoryItem.prize_id == PrizeId
                                        select d;
                if (entityTransaction.Count() >= 51)
                {
                    asdes = 1;
                    nostart = 47;
                    entityTransaction = entityTransaction.OrderBy(x => x.kzTransaction.transaction_date).Skip(46).Take(9);
                }
                else
                {
                    asdes = 0;
                    entityTransaction = entityTransaction.OrderByDescending(x => x.kzTransaction.transaction_date).Take(9);
                    nostart = entityTransaction.Count();
                }
                foreach (var v in entityTransaction)
                {
                    TransactionDetail Transaction = new TransactionDetail();
                    Transaction.No = nostart;
                    if (asdes == 0)
                    {
                        nostart = nostart - 1;
                    }
                    else
                    {
                        nostart = nostart + 1;
                    }
                    Transaction.TransactionId = v.id;
                    Transaction.MerchantId = (int)v.kzInventoryItem.merchant_id;
                    Transaction.MerchantName = v.kzInventoryItem.kzMerchant.name;
                    Transaction.InventoryItemId = (int)v.inventoryitem_id;
                    Transaction.InventoryItemName = v.kzInventoryItem.name;
                    Transaction.MemberId = (int)v.kzTransaction.user_id;
                    Transaction.MemberEmail = v.kzTransaction.kzUser.email;
                    Transaction.MemberPlay = false;
                    if (v.kzTransaction.participate_game != null && v.kzTransaction.participate_game == true)
                    {
                        Transaction.MemberPlay = true;
                    }
                    Transaction.TransactionDate = (DateTime)v.kzTransaction.transaction_date;
                    if (v.flashdeal_id != null)
                    {
                        Transaction.FlashDealId = v.flashdeal_id;
                        Transaction.FlashDeal = new FlashDealService().GetFlashDealById((int)v.flashdeal_id).Result;
                    }
                    Transaction.LastAction = v.kzTransaction.last_action;
                    TransactionList.Add(Transaction);
                }
                response = Response<List<TransactionDetail>>.Create(TransactionList);
            }

            return response;
        }
        public Response<List<MinimumTarget>> GetMinimumTargetList()
        {
            Response<List<MinimumTarget>> response = null;
            List<MinimumTarget> TransactionList = new List<MinimumTarget>();
            using (var context = new entity.KuazooEntities())
            {
                var entityTransaction = from d in context.kzTransactionDetails.Include("kzInventoryItem")
                                        group d by d.inventoryitem_id into g
                                        where g.Count() > g.FirstOrDefault().kzInventoryItem.minimumtarget
                                        && g.FirstOrDefault().kzInventoryItem.minimumtarget > 0
                                        select new
                                        {
                                            InventoryItemId = g.Key,
                                            InventoryItemName = g.FirstOrDefault().kzInventoryItem.name,
                                            MinimumTarget = g.FirstOrDefault().kzInventoryItem.minimumtarget,
                                            CurrentSales = g.Count(),
                                            MerchantId = g.FirstOrDefault().kzInventoryItem.merchant_id,
                                            MerchantName = g.FirstOrDefault().kzInventoryItem.kzMerchant.name
                                        };
                foreach (var v in entityTransaction)
                {
                    MinimumTarget mintarget = new MinimumTarget();
                    mintarget.MerchantId = (int)v.MerchantId;
                    mintarget.MerchantName = v.MerchantName;
                    mintarget.InventoryItemId = (int)v.InventoryItemId;
                    mintarget.InventoryItemName = v.InventoryItemName;
                    mintarget.Minimum = (decimal)v.MinimumTarget;
                    mintarget.CurrentSales = v.CurrentSales;
                    TransactionList.Add(mintarget);
                }
                response = Response<List<MinimumTarget>>.Create(TransactionList);
            }

            return response;
        }
        public Response<Decimal> GetMinimumTargetCount()
        {
            Response<Decimal> response = null;
            Decimal count = 0;
            using (var context = new entity.KuazooEntities())
            {
                var entityTransaction = from d in context.kzTransactionDetails.Include("kzInventoryItem")
                                        group d by d.inventoryitem_id into g
                                        where g.Count() > g.FirstOrDefault().kzInventoryItem.minimumtarget
                                        && g.FirstOrDefault().kzInventoryItem.minimumtarget > 0
                                        select new
                                        {
                                            InventoryItemId = g.Key,
                                        };
                count = entityTransaction.Count();
                response = Response<Decimal>.Create(count);
            }

            return response;
        }

        public Response<bool> CheckDealPurchase(int InventoryItemId, int UserId)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                if (UserId != 0)
                {
                    var entityTransaction = from d in context.kzTransactionDetails
                                            where d.inventoryitem_id == InventoryItemId
                                            && d.kzTransaction.user_id == UserId
                                            && d.kzTransaction.last_action != "5"
                                            && d.kzTransaction.tranID != null
                                            && d.kzTransaction.status == "00"
                                            select d;
                    if (entityTransaction.Count() > 0)
                    {
                        response = Response<bool>.Create(true);
                    }
                    else
                    {
                        response = Response<bool>.Create(false);
                    }
                }
                else
                {
                    response = Response<bool>.Create(false);
                }
            }
            return response;
        }
        public Response<bool> CheckVarianceLimit(int InventoryItemId, string Variance)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                string[] vari = Variance.Split('`');
                string _vari = vari[0];
                var entityVariance = from d in context.kzVariances
                                     where d.inventoryitem_id == InventoryItemId
                                     && d.name.ToLower() == _vari.ToLower()
                                     select d;
                if (entityVariance.Count() > 0)
                {
                    int limit = (int)entityVariance.First().available_limit;
                    var entityTransaction = from d in context.kzTransactionDetails
                                            where d.inventoryitem_id == InventoryItemId
                                            && d.kzTransaction.last_action != "5"
                                            && d.variance.ToLower() == Variance.ToLower()
                                            && d.kzTransaction.tranID != null && d.kzTransaction.status == "00"
                                            select d;
                    if (limit > entityTransaction.Count())
                    {
                        response = Response<bool>.Create(true);
                    }
                    else
                    {
                        response = Response<bool>.Create(false);
                    }
                }
                else
                {
                    response = Response<bool>.Create(false);
                }
            }
            return response;
        }

        public void UpdateSalesVisualMeter(int InventoryItemId, DateTime today)
        {
            try
            {
                using (var context = new entity.KuazooEntities())
                {
                    var entityInventory = from d in context.kzInventoryItems
                                          where d.id == InventoryItemId && d.draft == false
                                                  && d.last_action != "5"
                                          select d;
                    if (entityInventory != null && entityInventory.Count() > 0)
                    {
                        //decimal maxsales = (decimal)entityInventory.First().maximumsales;
                        //decimal sales = 0;

                        //var entityTransaciton = from d in context.kzTransactionDetails
                        //                        where d.inventoryitem_id == InventoryItemId
                        //                        select d;
                        //if (entityTransaciton != null && entityTransaciton.Count() > 0)
                        //{
                        //    sales = entityTransaciton.Count();
                        //}
                        //decimal visualmeter = 0;
                        //if (maxsales > 0)
                        //{
                        //    visualmeter = Math.Ceiling((sales / maxsales) * 100);
                        //}
                        //else visualmeter = 100;
                        //if (entityInventory.First().salesvisualmeter < visualmeter)
                        //{
                        //    entityInventory.First().salesvisualmeter = visualmeter;
                        //    context.SaveChanges();
                        //}
                        int groupid = (int)entityInventory.First().prize_id;
                        decimal caltotalrevenue = 0;
                        decimal totalrevenue = (decimal)entityInventory.First().kzPrize.total_revenue;
                        decimal calrevenue = 0;
                        decimal sales = 1;
                        if (entityInventory.First().kzPrize.cal_total_revenue != null)
                        {
                            caltotalrevenue = (decimal)entityInventory.First().kzPrize.cal_total_revenue;
                        }
                        //var entityTransaciton = from d in context.kzTransactionDetails
                        //                        where d.inventoryitem_id == InventoryItemId
                        //                        select d;
                        //if (entityTransaciton != null && entityTransaciton.Count() > 0)
                        //{
                        //    sales = entityTransaciton.Count();
                        //}
                        //decimal disprice = (decimal)entityInventory.First().price - ((decimal)entityInventory.First().price * (decimal)entityInventory.First().discount / 100);
                        //calrevenue = disprice * sales;
                        calrevenue = (decimal)entityInventory.First().margin * sales;
                        caltotalrevenue += calrevenue;
                        decimal percentagebar = caltotalrevenue / totalrevenue * 100;
                        if (percentagebar > 100) percentagebar = 100;
                        else if (percentagebar < 0) percentagebar = 0;
                        percentagebar = Math.Round(percentagebar, 2);
                        decimal remainsales = 0;
                        var entitySameInventory = from d in context.kzInventoryItems
                                                  where d.prize_id == groupid && d.draft == false
                                                  && d.flag == true && d.last_action != "5"
                                                  select d;
                        foreach (var v2 in entitySameInventory)
                        {
                            try
                            {
                                //disprice = (decimal)v2.price - ((decimal)v2.price * (decimal)v2.discount / 100);
                                //remainsales = (totalrevenue - caltotalrevenue) / disprice;
                                remainsales = (totalrevenue - caltotalrevenue) / (decimal)v2.margin;
                                remainsales = Math.Ceiling(remainsales);
                            }
                            catch
                            {
                                remainsales = 0;
                            }
                            if (remainsales < 0) remainsales = 0;
                            v2.remainsales = remainsales;
                            v2.salesvisualmeter = percentagebar;
                        }
                        entityInventory.First().kzPrize.cal_total_revenue = caltotalrevenue;
                        context.SaveChanges();

                        #region old
                        //int groupid = (int)entityInventory.First().prize_id;
                        //var entitySameInventory = from d in context.kzInventoryItems
                        //                          where d.prize_id == groupid
                        //                          && d.flag == true && d.last_action != "5"
                        //                          select d;
                        //decimal totalrevenue = (decimal)entitySameInventory.First().kzPrize.total_revenue;
                        //decimal sales = 0;
                        //decimal calrevenue = 0;
                        //decimal caltotalrevenue = 0;
                        //foreach (var v2 in entitySameInventory)
                        //{
                        //    sales = 0;
                        //    calrevenue = 0;
                        //    var entityTransaciton = from d in context.kzTransactionDetails
                        //                            where d.inventoryitem_id == v2.id
                        //                            select d;
                        //    if (entityTransaciton != null && entityTransaciton.Count() > 0)
                        //    {
                        //        sales = entityTransaciton.Count();
                        //    }
                        //    decimal disprice = (decimal)v2.price - ((decimal)v2.price * (decimal)v2.discount / 100);
                        //    calrevenue = disprice * sales;
                        //    caltotalrevenue += calrevenue;
                        //}
                        //decimal percentagebar = caltotalrevenue / totalrevenue * 100;
                        //if (percentagebar > 100) percentagebar = 100;
                        //else if (percentagebar < 0) percentagebar = 0;
                        //percentagebar = Math.Round(percentagebar, 2);
                        //decimal remainsales=0;
                        //foreach (var v2 in entitySameInventory)
                        //{
                        //    try
                        //    {
                        //        decimal disprice = (decimal)v2.price - ((decimal)v2.price * (decimal)v2.discount / 100);
                        //        remainsales = (totalrevenue - caltotalrevenue) / disprice;
                        //        remainsales = Math.Ceiling(remainsales);
                        //    }
                        //    catch
                        //    {
                        //        remainsales = 0;
                        //    }
                        //    if (remainsales < 0) remainsales = 0;
                        //    v2.remainsales = remainsales;
                        //    v2.salesvisualmeter = percentagebar;
                        //}
                        #endregion
                    }
                }

            }
            catch
            {
            }
        }
        public void UpdateSalesVisualMeterV2(int InventoryItemId, DateTime today, String variance)
        {
            try
            {
                using (var context = new entity.KuazooEntities())
                {
                    var entityInventory = from d in context.kzInventoryItems
                                          where d.id == InventoryItemId && d.draft == false
                                                  && d.last_action != "5"
                                          select d;
                    if (entityInventory != null && entityInventory.Count() > 0)
                    {
                        int groupid = (int)entityInventory.First().prize_id;
                        decimal caltotalrevenue = 0;
                        decimal totalrevenue = (decimal)entityInventory.First().kzPrize.total_revenue;
                        decimal calrevenue = 0;
                        decimal sales = 1;
                        if (entityInventory.First().kzPrize.cal_total_revenue != null)
                        {
                            caltotalrevenue = (decimal)entityInventory.First().kzPrize.cal_total_revenue;
                        }
                        if (variance != "")
                        {
                            try
                            {
                                string[] varilist = variance.Split('`');
                                string _vari = varilist[0];
                                var varianceEntity = from d in context.kzVariances
                                                     where d.inventoryitem_id == InventoryItemId
                                                     && d.name.ToLower() == _vari.ToLower()
                                                     select d;
                                if (varianceEntity.Count() > 0)
                                {
                                    calrevenue = (decimal)varianceEntity.First().margin * sales;
                                }
                            }
                            catch
                            {
                                calrevenue = (decimal)entityInventory.First().margin * sales;
                            }
                        }
                        else
                        {
                            calrevenue = (decimal)entityInventory.First().margin * sales;
                        }
                        caltotalrevenue += calrevenue;
                        decimal percentagebar = caltotalrevenue / totalrevenue * 100;
                        if (percentagebar > 100) percentagebar = 100;
                        else if (percentagebar < 0) percentagebar = 0;
                        percentagebar = Math.Round(percentagebar, 2);
                        decimal remainsales = 0;
                        decimal marginlowest = 0;
                        var varianceLowestEntity = from d in context.kzVariances
                                                   where d.inventoryitem_id == InventoryItemId
                                                   orderby d.margin ascending
                                                   select d;
                        if (varianceLowestEntity.Count() > 0)
                        {
                            marginlowest = (decimal)varianceLowestEntity.First().margin;
                        }
                        var entitySameInventory = from d in context.kzInventoryItems
                                                  where d.prize_id == groupid && d.draft == false
                                                  && d.flag == true && d.last_action != "5"
                                                  select d;
                        foreach (var v2 in entitySameInventory)
                        {
                            marginlowest = 0;
                            var variance2LowestEntity = from d in context.kzVariances
                                                        where d.inventoryitem_id == v2.id
                                                        orderby d.margin ascending
                                                        select d;
                            if (variance2LowestEntity.Count() > 0)
                            {
                                marginlowest = (decimal)variance2LowestEntity.First().margin;
                            }
                            try
                            {
                                if (marginlowest != 0)
                                {
                                    if (marginlowest > (decimal)v2.margin)
                                    {
                                        remainsales = (totalrevenue - caltotalrevenue) / (decimal)v2.margin;
                                    }
                                    else
                                    {
                                        remainsales = (totalrevenue - caltotalrevenue) / marginlowest;
                                    }
                                }
                                else
                                {
                                    remainsales = (totalrevenue - caltotalrevenue) / (decimal)v2.margin;
                                }
                                remainsales = Math.Ceiling(remainsales);
                            }
                            catch
                            {
                                remainsales = 0;
                            }
                            if (remainsales < 0) remainsales = 0;
                            v2.remainsales = remainsales;
                            v2.salesvisualmeter = percentagebar;
                        }
                        entityInventory.First().kzPrize.cal_total_revenue = caltotalrevenue;
                        context.SaveChanges();

                    }
                }

            }
            catch
            {
            }
        }
        public void UpdateSalesVisualMeterNewInventory(int InventoryItemId)
        {
            try
            {
                using (var context = new entity.KuazooEntities())
                {
                    var entityInventory = from d in context.kzInventoryItems
                                          where d.id == InventoryItemId && d.draft == false
                                                  && d.last_action != "5"
                                          select d;
                    if (entityInventory != null && entityInventory.Count() > 0)
                    {
                        int groupid = (int)entityInventory.First().prize_id;
                        decimal caltotalrevenue = 0;
                        decimal totalrevenue = (decimal)entityInventory.First().kzPrize.total_revenue;

                        if (entityInventory.First().kzPrize.cal_total_revenue != null)
                        {
                            caltotalrevenue = (decimal)entityInventory.First().kzPrize.cal_total_revenue;
                        }
                        decimal percentagebar = caltotalrevenue / totalrevenue * 100;
                        if (percentagebar > 100) percentagebar = 100;
                        else if (percentagebar < 0) percentagebar = 0;
                        percentagebar = Math.Round(percentagebar, 2);
                        if (caltotalrevenue == 0)
                        {
                            decimal remainsales = 0;
                            decimal marginlowest = 0;
                            var varianceLowestEntity = from d in context.kzVariances
                                                       where d.inventoryitem_id == InventoryItemId
                                                       orderby d.margin ascending
                                                       select d;
                            if (varianceLowestEntity.Count() > 0)
                            {
                                marginlowest = (decimal)varianceLowestEntity.First().margin;
                            }
                            //decimal disprice = 0;
                            //try
                            //{
                            //    //disprice = (decimal)entityInventory.First().price - ((decimal)entityInventory.First().price * (decimal)entityInventory.First().discount / 100);
                            //    //remainsales = (totalrevenue - caltotalrevenue) / disprice;
                            //    remainsales = (totalrevenue - caltotalrevenue) / (decimal)entityInventory.First().margin;
                            //    remainsales = Math.Ceiling(remainsales);
                            //}
                            //catch
                            //{
                            //    remainsales = 0;
                            //}
                            try
                            {
                                if (marginlowest != 0)
                                {
                                    if (marginlowest > (decimal)entityInventory.First().margin)
                                    {
                                        remainsales = (totalrevenue - caltotalrevenue) / (decimal)entityInventory.First().margin;
                                    }
                                    else
                                    {
                                        remainsales = (totalrevenue - caltotalrevenue) / marginlowest;
                                    }
                                }
                                else
                                {
                                    remainsales = (totalrevenue - caltotalrevenue) / (decimal)entityInventory.First().margin;
                                }
                                remainsales = Math.Ceiling(remainsales);
                            }
                            catch
                            {
                                remainsales = 0;
                            }
                            if (remainsales < 0) remainsales = 0;
                            entityInventory.First().remainsales = remainsales;
                            entityInventory.First().salesvisualmeter = percentagebar;
                        }
                        else
                        {
                            decimal remainsales = 0;
                            decimal marginlowest = 0;
                            var varianceLowestEntity = from d in context.kzVariances
                                                       where d.inventoryitem_id == InventoryItemId
                                                       orderby d.margin ascending
                                                       select d;
                            if (varianceLowestEntity.Count() > 0)
                            {
                                marginlowest = (decimal)varianceLowestEntity.First().margin;
                            }
                            var entitySameInventory = from d in context.kzInventoryItems
                                                      where d.prize_id == groupid && d.draft == false
                                                      && d.flag == true && d.last_action != "5"
                                                      select d;
                            foreach (var v2 in entitySameInventory)
                            {
                                //try
                                //{
                                //    //disprice = (decimal)v2.price - ((decimal)v2.price * (decimal)v2.discount / 100);
                                //    //remainsales = (totalrevenue - caltotalrevenue) / disprice;
                                //    remainsales = (totalrevenue - caltotalrevenue) / (decimal)v2.margin;
                                //    remainsales = Math.Ceiling(remainsales);
                                //}
                                //catch
                                //{
                                //    remainsales = 0;
                                //}
                                marginlowest = 0;
                                var variance2LowestEntity = from d in context.kzVariances
                                                            where d.inventoryitem_id == v2.id
                                                            orderby d.margin ascending
                                                            select d;
                                if (variance2LowestEntity.Count() > 0)
                                {
                                    marginlowest = (decimal)variance2LowestEntity.First().margin;
                                }
                                try
                                {
                                    if (marginlowest != 0)
                                    {
                                        if (marginlowest > (decimal)v2.margin)
                                        {
                                            remainsales = (totalrevenue - caltotalrevenue) / (decimal)v2.margin;
                                        }
                                        else
                                        {
                                            remainsales = (totalrevenue - caltotalrevenue) / marginlowest;
                                        }
                                    }
                                    else
                                    {
                                        remainsales = (totalrevenue - caltotalrevenue) / (decimal)v2.margin;
                                    }
                                    remainsales = Math.Ceiling(remainsales);
                                }
                                catch
                                {
                                    remainsales = 0;
                                }
                                if (remainsales < 0) remainsales = 0;
                                v2.remainsales = remainsales;
                                v2.salesvisualmeter = percentagebar;
                            }
                        }

                        entityInventory.First().kzPrize.cal_total_revenue = caltotalrevenue;
                        context.SaveChanges();
                    }
                }

            }
            catch
            {
            }
        }

        public void UpdateSalesVisualMeterEditInventory(int InventoryItemId, bool MarginChange, decimal OldMargin)
        {
            try
            {
                using (var context = new entity.KuazooEntities())
                {
                    var entityInventory = from d in context.kzInventoryItems
                                          where d.id == InventoryItemId && d.draft == false
                                                  && d.last_action != "5"
                                          select d;
                    if (entityInventory != null && entityInventory.Count() > 0)
                    {
                        int groupid = (int)entityInventory.First().prize_id;
                        decimal caltotalrevenue = 0;
                        decimal totalrevenue = (decimal)entityInventory.First().kzPrize.total_revenue;

                        if (entityInventory.First().kzPrize.cal_total_revenue != null)
                        {
                            caltotalrevenue = (decimal)entityInventory.First().kzPrize.cal_total_revenue;
                        }
                        //when margin change
                        //if (MarginChange)
                        //{
                        //    int sales = 0;
                        //    var entityTransaciton = from d in context.kzTransactionDetails
                        //                            where d.inventoryitem_id == InventoryItemId
                        //                            select d;
                        //    if (entityTransaciton != null && entityTransaciton.Count() > 0)
                        //    {
                        //        sales = entityTransaciton.Count();
                        //    }
                        //    decimal reducerevenue = sales * OldMargin;
                        //    caltotalrevenue = caltotalrevenue - reducerevenue;
                        //    decimal newrevenue = sales * (decimal)entityInventory.First().margin;
                        //    caltotalrevenue = caltotalrevenue + newrevenue;
                        //}


                        decimal percentagebar = caltotalrevenue / totalrevenue * 100;
                        if (percentagebar > 100) percentagebar = 100;
                        else if (percentagebar < 0) percentagebar = 0;
                        percentagebar = Math.Round(percentagebar, 2);
                        decimal marginlowest = 0;
                        var varianceLowestEntity = from d in context.kzVariances
                                                   where d.inventoryitem_id == InventoryItemId
                                                   orderby d.margin ascending
                                                   select d;
                        if (varianceLowestEntity.Count() > 0)
                        {
                            marginlowest = (decimal)varianceLowestEntity.First().margin;
                        }
                        if (caltotalrevenue == 0)
                        {
                            decimal remainsales = 0;
                            //decimal disprice = 0;

                            try
                            {
                                if (marginlowest != 0)
                                {
                                    if (marginlowest > (decimal)entityInventory.First().margin)
                                    {
                                        remainsales = (totalrevenue - caltotalrevenue) / (decimal)entityInventory.First().margin;
                                    }
                                    else
                                    {
                                        remainsales = (totalrevenue - caltotalrevenue) / marginlowest;
                                    }
                                }
                                else
                                {
                                    remainsales = (totalrevenue - caltotalrevenue) / (decimal)entityInventory.First().margin;
                                }
                                remainsales = Math.Ceiling(remainsales);
                            }
                            catch
                            {
                                remainsales = 0;
                            }
                            if (remainsales < 0) remainsales = 0;
                            entityInventory.First().remainsales = remainsales;
                            entityInventory.First().salesvisualmeter = percentagebar;
                        }
                        else
                        {
                            decimal remainsales = 0;
                            var entitySameInventory = from d in context.kzInventoryItems
                                                      where d.prize_id == groupid
                                                      && d.flag == true && d.last_action != "5"
                                                      select d;
                            foreach (var v2 in entitySameInventory)
                            {
                                marginlowest = 0;
                                var variance2LowestEntity = from d in context.kzVariances
                                                            where d.inventoryitem_id == v2.id
                                                            orderby d.margin ascending
                                                            select d;
                                if (variance2LowestEntity.Count() > 0)
                                {
                                    marginlowest = (decimal)variance2LowestEntity.First().margin;
                                }
                                try
                                {
                                    if (marginlowest != 0)
                                    {
                                        if (marginlowest > (decimal)v2.margin)
                                        {
                                            remainsales = (totalrevenue - caltotalrevenue) / (decimal)v2.margin;
                                        }
                                        else
                                        {
                                            remainsales = (totalrevenue - caltotalrevenue) / marginlowest;
                                        }
                                    }
                                    else
                                    {
                                        remainsales = (totalrevenue - caltotalrevenue) / (decimal)v2.margin;
                                    }
                                    //disprice = (decimal)v2.price - ((decimal)v2.price * (decimal)v2.discount / 100);
                                    //remainsales = (totalrevenue - caltotalrevenue) / disprice;
                                    //remainsales = (totalrevenue - caltotalrevenue) / (decimal)v2.margin;
                                    remainsales = Math.Ceiling(remainsales);
                                }
                                catch
                                {
                                    remainsales = 0;
                                }
                                if (remainsales < 0) remainsales = 0;
                                v2.remainsales = remainsales;
                                v2.salesvisualmeter = percentagebar;
                            }
                        }

                        entityInventory.First().kzPrize.cal_total_revenue = caltotalrevenue;
                        context.SaveChanges();
                    }
                }

            }
            catch
            {
            }
        }


        public TempCalculation GetTempSalesVisualMeterNewInventory(int prizeid, decimal margin)
        {
            TempCalculation result = new TempCalculation();
            try
            {
                using (var context = new entity.KuazooEntities())
                {
                    var entityPrize = from d in context.kzPrizes
                                      where d.id == prizeid && d.last_action != "5"
                                      select d;
                    if (entityPrize != null && entityPrize.Count() > 0)
                    {
                        int groupid = prizeid;
                        decimal caltotalrevenue = 0;
                        decimal totalrevenue = (decimal)entityPrize.First().total_revenue;

                        if (entityPrize.First().cal_total_revenue != null)
                        {
                            caltotalrevenue = (decimal)entityPrize.First().cal_total_revenue;
                        }
                        decimal percentagebar = caltotalrevenue / totalrevenue * 100;
                        if (percentagebar > 100) percentagebar = 100;
                        else if (percentagebar < 0) percentagebar = 0;
                        percentagebar = Math.Round(percentagebar, 2);
                        decimal remainsales = 0;
                        try
                        {
                            remainsales = (totalrevenue - caltotalrevenue) / margin;
                            remainsales = Math.Ceiling(remainsales);
                        }
                        catch
                        {
                            remainsales = 0;
                        }
                        if (remainsales < 0) remainsales = 0;
                        result.RemainSales = remainsales;
                        result.VisualMeter = percentagebar;
                    }
                    else
                    {
                        result.RemainSales = 0;
                        result.VisualMeter = 0;
                    }
                }

            }
            catch
            {
                result.RemainSales = 0;
                result.VisualMeter = 0;
            }
            return result;
        }

        public void CheckMinimumTargetNotif(int InventoryItemId)
        {
            try
            {
                using (var context = new entity.KuazooEntities())
                {
                    var entityTransaction = from d in context.kzTransactionDetails.Include("kzInventoryItem")
                                            where d.inventoryitem_id == InventoryItemId
                                            group d by d.inventoryitem_id into g
                                            where g.Count() == g.FirstOrDefault().kzInventoryItem.minimumtarget
                                            && g.FirstOrDefault().kzInventoryItem.minimumtarget > 0
                                            select new
                                            {
                                                InventoryItemId = g.Key,
                                                InventoryItemName = g.FirstOrDefault().kzInventoryItem.name,
                                                MinimumTarget = g.FirstOrDefault().kzInventoryItem.minimumtarget,
                                                CurrentSales = g.Count(),
                                                MerchantId = g.FirstOrDefault().kzInventoryItem.merchant_id,
                                                MerchantName = g.FirstOrDefault().kzInventoryItem.kzMerchant.name
                                            };
                    if (entityTransaction != null && entityTransaction.Count() > 0)
                    {


                        string to = "noreply@kuazoo.com.my";
                        string body = "";
                        string subject = "Deal Hit Minimum Target";

                        //body = "Item " + entityTransaction.First().InventoryItemName + " from Merchant " + entityTransaction.First().MerchantName + " hit the minimum target";
                        body = helper.Email.CreateMinimumTarget(entityTransaction.First().InventoryItemName, entityTransaction.First().MerchantName);
                        new GeneralService().SendEmail(subject, to, body);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public TransactionHtml TransactionHtml(int Transactionid, int gmt)
        {
            TransactionHtml response = new kuazoo.TransactionHtml();
            try
            {
                using (var context = new entity.KuazooEntities())
                {
                    var entityTransaction = from d in context.kzTransactions
                                            where d.id == Transactionid
                                            select d;
                    if (entityTransaction != null && entityTransaction.Count() > 0)
                    {
                        int userid = (int)entityTransaction.First().user_id;
                        var userEntity = from d in context.kzUsers
                                         where d.id == userid
                                         select d;
                        string to = userEntity.First().email;
                        string body = "";
                        string orderno = GeneralService.TransactionCode(entityTransaction.First().id, (DateTime)entityTransaction.First().transaction_date);
                        decimal point = 0;
                        decimal promo = 0;
                        string promocode = "";
                        int promotype = 1;
                        if (entityTransaction.First().kpoint != null)
                        {
                            point = (decimal)entityTransaction.First().kpoint;
                            point = point / 100;
                        }
                        if (entityTransaction.First().promotion_id != null && entityTransaction.First().kzPromotion != null)
                        {
                            promotype = (int)entityTransaction.First().kzPromotion.type;
                            promo = (decimal)entityTransaction.First().kzPromotion.value;
                            promocode = entityTransaction.First().kzPromotion.code;
                        }

                        string customer = entityTransaction.First().kzUser.first_name + " " + entityTransaction.First().kzUser.last_name;
                        var merchanten = entityTransaction.First().kzTransactionDetails.First().kzInventoryItem.kzMerchant;
                        string merchant = merchanten.name;
                        var customeren = entityTransaction.First().kzBillings.First();
                        string customeraddress = "";
                        if (customeren.address_line1 != null && customeren.address_line1 != "")
                        {
                            customeraddress += customeren.address_line1 + ", ";
                        }
                        if (customeren.address_line2 != null && customeren.address_line2 != "")
                        {
                            customeraddress += customeren.address_line2 + ", ";
                        }
                        if (customeren.zipcode != null && customeren.zipcode != "")
                        {
                            customeraddress += customeren.zipcode + ", ";
                        }
                        if (customeren.city != null && customeren.city != "")
                        {
                            customeraddress += customeren.city + ", ";
                        }
                        if (customeren.state != null && customeren.state != "")
                        {
                            customeraddress += customeren.state + ", ";
                        }
                        if (customeren.country != null && customeren.country != "")
                        {
                            customeraddress += customeren.country + ", ";
                        }
                        var entityTD = from d in context.kzTransactionDetails
                                       where d.transaction_id == Transactionid
                                       select d;
                        decimal total = 0;
                        decimal price = 0;
                        foreach (var v in entityTD)
                        {
                            price = (decimal)v.price;
                            total = total + price;
                        }
                        if (promotype == 1)
                        {
                            promo = total * promo / 100;
                        }
                        total = total - promo - point;

                        DateTime transactiondate = (DateTime)entityTransaction.First().transaction_date;
                        transactiondate = transactiondate.AddHours(gmt);
                        //PaymentMethod paymentmethod = (PaymentMethod)entityTransaction.First().kzBillings.First().payment_method;
                        //string paymenttype = paymentmethod.ToString();
                        string paymenttype = "MOL - " + entityTransaction.First().channel;
                        string dealname = entityTransaction.First().kzTransactionDetails.First().kzInventoryItem.name;
                        int dealid = (int)entityTransaction.First().kzTransactionDetails.First().inventoryitem_id;
                        string url = WebSetting.Url + "/Home/Deals/" + kuazoo.helper.ReplaceSymbol.Replace(dealname) + "?d=1";
                        string fineprint = entityTransaction.First().kzTransactionDetails.First().kzInventoryItem.terms;

                        string imageurl = "";
                        var entityImg = from d in context.kzInventoryItemImages
                                        from e in context.kzImages
                                        where d.image_id == e.id && d.inventoryitem_id == dealid
                                        && d.main == true
                                        select new { d.image_id, e.url };

                        if (entityImg.Count() > 0)
                        {
                            imageurl = ConfigurationManager.AppSettings["uploadpath"] + entityImg.First().url;
                        }
                        string variance = entityTransaction.First().kzTransactionDetails.First().variance;
                        string[] _vari = variance.Split('`');
                        string variname = _vari[0];
                        var entityVariance = from d in context.kzVariances
                                             where d.name == variname && d.inventoryitem_id == dealid
                                             select d;
                        string sku = "";
                        if (entityVariance.Count() > 0)
                        {
                            sku = entityVariance.First().sku;
                        }
                        string interestdealimg = "<tr>";
                        string interestdeal = "<tr>";

                        var entitydeals = new InventoryItemService().GetInventoryItemListTakeV2(DateTime.UtcNow, 3).Result;
                        string _urldeal;
                        foreach (var v in entitydeals)
                        {
                            _urldeal = WebSetting.Url + "/Home/Deals/" + kuazoo.helper.ReplaceSymbol.Replace(v.Name) + "?d=1";
                            interestdealimg += @"<td style=""width:30%"">
												<a href=""" + _urldeal + @""">
													<img src=""" + v.ImageUrl + @""" alt=""" + v.Name + @""" style=""width:100%"">
												</a>
											</td>";
                            decimal pricedis = v.Price - (v.Price * v.Discount / 100);
                            interestdeal += @"<td style=""vertical-align: top;"">
												<table border=""0"" cellpadding=""0"" cellspacing=""0"" style=""width:100%"">
													<tr>
														<td style=""text-align:left;vertical-align:top"">
															" + v.Name + @"  
														</td>
														<td style=""text-align:right;color:#0084cf; width:100px;vertical-align:top"">
															RM " + pricedis.ToString("N0") + @"<br/>
															<span style=""font-size:13px;color:#666666"">" + v.Discount.ToString("N0") + @"% OFF</span>
														</td>
													</tr>	
												</table>
											</td>";
                        }
                        interestdealimg += @"</tr>";
                        interestdeal += @"</tr>";


                        if (entityTransaction.First().kzTransactionDetails.First().kzInventoryItem.inventoryitem_type_id == 1)
                        {
                            response.customer = customer;
                            response.merchant = merchant;
                            response.orderno = orderno;
                            response.transactiondate = transactiondate;
                            response.amount = total;
                            response.paymenttype = paymenttype;
                            response.url = url;
                            response.dealname = dealname;
                            response.customeraddress = customer;
                            response.sku = sku;
                            response.price = price;
                            response.promo = promo;
                            response.promocode = promocode;
                            response.kpoint = point;
                            response.dealinterest = interestdealimg + interestdeal;
                            response.mailto = to;
                            response.imageurl = imageurl;
                        }
                        else
                        {
                            List<string> voucherlist = new List<string>();
                            string vouchercode = "";
                            var entityCode = from d in context.kzPreCodes
                                             where d.transaction_id == Transactionid
                                             select d;
                            int i = 0;
                            foreach (var vo in entityCode)
                            {
                                i++;
                                if (i == 1)
                                {
                                    vouchercode += "<tr>";
                                }
                                voucherlist.Add(vo.code);
                                vouchercode += "<td><strong>Voucher Code: </strong> " + vo.code + "</td>";
                                if (i == 3)
                                {
                                    vouchercode += "</tr>";
                                    i = 0;
                                }
                            }
                            if (i != 0)
                            {
                                vouchercode += "</tr>";
                            }
                            response.customer = customer;
                            response.merchant = merchant;
                            response.orderno = orderno;
                            response.transactiondate = transactiondate;
                            response.amount = total;
                            response.paymenttype = paymenttype;
                            response.url = url;
                            response.dealname = dealname;
                            response.fineprint = fineprint;
                            response.sku = sku;
                            response.price = price;
                            response.promo = promo;
                            response.promocode = promocode;
                            response.kpoint = point;
                            response.vouchercode = vouchercode;
                            response.dealinterest = interestdealimg + interestdeal;
                            response.mailto = to;
                            response.imageurl = imageurl;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return response;
        }

        public void TransactionMemberNotif(int Transactionid, string path, int gmt)
        {
            try
            {
                using (var context = new entity.KuazooEntities())
                {
                    var entityTransaction = from d in context.kzTransactions
                                            where d.id == Transactionid
                                            select d;
                    if (entityTransaction != null && entityTransaction.Count() > 0)
                    {
                        int userid = (int)entityTransaction.First().user_id;
                        var userEntity = from d in context.kzUsers
                                         where d.id == userid
                                         select d;
                        string to = userEntity.First().email;
                        string body = "";
                        string orderno = GeneralService.TransactionCode(entityTransaction.First().id, (DateTime)entityTransaction.First().transaction_date);
                        decimal point = 0;
                        decimal promo = 0;
                        string promocode = "";
                        int promotype = 1;
                        if (entityTransaction.First().kpoint != null)
                        {
                            point = (decimal)entityTransaction.First().kpoint;
                            point = point / 100;
                        }
                        if (entityTransaction.First().promotion_id != null && entityTransaction.First().kzPromotion != null)
                        {
                            promotype = (int)entityTransaction.First().kzPromotion.type;
                            promo = (decimal)entityTransaction.First().kzPromotion.value;
                            promocode = entityTransaction.First().kzPromotion.code;
                        }
                        string subject = "Thanks for Shopping at Kuazoo! Order No:" + orderno;

                        string customer = entityTransaction.First().kzUser.first_name + " " + entityTransaction.First().kzUser.last_name;
                        var merchanten = entityTransaction.First().kzTransactionDetails.First().kzInventoryItem.kzMerchant;
                        string merchant = merchanten.name;
                        var customeren = entityTransaction.First().kzBillings.First();
                        string customeraddress = "";
                        if (customeren.address_line1 != null && customeren.address_line1 != "")
                        {
                            customeraddress += customeren.address_line1 + ", ";
                        }
                        if (customeren.address_line2 != null && customeren.address_line2 != "")
                        {
                            customeraddress += customeren.address_line2 + ", ";
                        }
                        if (customeren.zipcode != null && customeren.zipcode != "")
                        {
                            customeraddress += customeren.zipcode + ", ";
                        }
                        if (customeren.city != null && customeren.city != "")
                        {
                            customeraddress += customeren.city + ", ";
                        }
                        if (customeren.state != null && customeren.state != "")
                        {
                            customeraddress += customeren.state + ", ";
                        }
                        if (customeren.country != null && customeren.country != "")
                        {
                            customeraddress += customeren.country + ", ";
                        }
                        var entityTD = from d in context.kzTransactionDetails
                                       where d.transaction_id == Transactionid
                                       select d;
                        decimal total = 0;
                        decimal price = 0;
                        foreach (var v in entityTD)
                        {
                            price = (decimal)v.price;
                            total = total + price;
                        }
                        if (promotype == 1)
                        {
                            promo = total * promo / 100;
                        }
                        total = total - promo - point;

                        DateTime transactiondate = (DateTime)entityTransaction.First().transaction_date;
                        transactiondate = transactiondate.AddHours(gmt);
                        //PaymentMethod paymentmethod = (PaymentMethod)entityTransaction.First().kzBillings.First().payment_method;
                        //string paymenttype = paymentmethod.ToString();
                        string paymenttype = "MOL - " + entityTransaction.First().channel;
                        string dealname = entityTransaction.First().kzTransactionDetails.First().kzInventoryItem.name;
                        int dealid = (int)entityTransaction.First().kzTransactionDetails.First().inventoryitem_id;
                        string url = WebSetting.Url + "/Home/Deals/" + kuazoo.helper.ReplaceSymbol.Replace(dealname) + "?d=1";
                        string fineprint = entityTransaction.First().kzTransactionDetails.First().kzInventoryItem.terms;

                        string imageurl = "";
                        var entityImg = from d in context.kzInventoryItemImages
                                        from e in context.kzImages
                                        where d.image_id == e.id && d.inventoryitem_id == dealid
                                        && d.main == true
                                        select new { d.image_id, e.url };

                        if (entityImg.Count() > 0)
                        {
                            imageurl = ConfigurationManager.AppSettings["uploadpath"] + entityImg.First().url;
                        }
                        string variance = entityTransaction.First().kzTransactionDetails.First().variance;
                        string[] _vari = variance.Split('`');
                        string variname = _vari[0];
                        var entityVariance = from d in context.kzVariances
                                             where d.name == variname && d.inventoryitem_id == dealid
                                             select d;
                        string sku = "";
                        if (entityVariance.Count() > 0)
                        {
                            sku = entityVariance.First().sku;
                        }
                        string interestdealimg = "<tr>";
                        string interestdeal = "<tr>";

                        var entitydeals = new InventoryItemService().GetInventoryItemListTakeV2(DateTime.UtcNow, 3).Result;
                        string _urldeal;
                        foreach (var v in entitydeals)
                        {
                            _urldeal = WebSetting.Url + "/Home/Deals/" + kuazoo.helper.ReplaceSymbol.Replace(v.Name) + "?d=1";
                            interestdealimg += @"<td style=""width:30%"">
												<a href=""" + _urldeal + @""">
													<img src=""" + v.ImageUrl + @""" alt=""" + v.Name + @""" style=""width:100%"">
												</a>
											</td>";
                            decimal pricedis = v.Price - (v.Price * v.Discount / 100);
                            interestdeal += @"<td style=""vertical-align: top;"">
												<table border=""0"" cellpadding=""0"" cellspacing=""0"" style=""width:100%"">
													<tr>
														<td style=""text-align:left;vertical-align:top"">
															" + v.Name + @"  
														</td>
														<td style=""text-align:right;color:#0084cf; width:100px;vertical-align:top"">
															RM " + pricedis.ToString("N0") + @"<br/>
															<span style=""font-size:13px;color:#666666"">" + v.Discount.ToString("N0") + @"% OFF</span>
														</td>
													</tr>	
												</table>
											</td>";
                        }
                        interestdealimg += @"</tr>";
                        interestdeal += @"</tr>";


                        if (entityTransaction.First().kzTransactionDetails.First().kzInventoryItem.inventoryitem_type_id == 1)
                        {
                            body = helper.Email.CreatePurchaseEmailV2(customer, merchant, orderno, transactiondate, total, paymenttype, url, dealname, customeraddress, sku, price, promo, promocode, point, interestdealimg + interestdeal, to, imageurl, variname);

                            new GeneralService().SendEmail(subject, to, body);
                        }
                        else
                        {
                            List<string> voucherlist = new List<string>();
                            string vouchercode = "";
                            var entityCode = from d in context.kzPreCodes
                                             where d.transaction_id == Transactionid
                                             select d;
                            int i = 0;
                            foreach (var vo in entityCode)
                            {
                                i++;
                                if (i == 1)
                                {
                                    vouchercode += "<tr>";
                                }
                                voucherlist.Add(vo.code);
                                vouchercode += "<td><strong>Voucher Code: </strong> " + vo.code + "</td>";
                                if (i == 3)
                                {
                                    vouchercode += "</tr>";
                                    i = 0;
                                }
                            }
                            if (i != 0)
                            {
                                vouchercode += "</tr>";
                            }
                            //MemoryStream pdf = Getpdf(subject, voucherlist);
                            // Attachment att1 = new Attachment(pdf, "voucher.pdf");
                            body = helper.Email.CreatePurchaseServicesEmailV2(customer, merchant, orderno, transactiondate, total, paymenttype, url, dealname, fineprint, sku, price, promo, promocode, point, vouchercode, interestdealimg + interestdeal, to, imageurl, variname);
                            //MemoryStream pdf = GetpdfHtml2(body, path);
                            //Attachment att1 = new Attachment(pdf, "voucher.pdf");
                            //new GeneralService().SendEmail(subject, to, body, "", att1);
                            new GeneralService().SendEmail(subject, to, body);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void TransactionGiftNotice(int Transactionid, string path, int gmt)
        {
            try
            {
                using (var context = new entity.KuazooEntities())
                {
                    var entityTransaction = from d in context.kzTransactions
                                            where d.id == Transactionid
                                            select d;
                    if (entityTransaction != null && entityTransaction.Count() > 0)
                    {
                        int userid = (int)entityTransaction.First().user_id;
                        var userEntity = from d in context.kzUsers
                                         where d.id == userid
                                         select d;

                        var shippingEntity = from d in context.kzShippings
                                             where d.transaction_id == Transactionid
                                             select d;

                        if (shippingEntity.First().gift == false)
                        {
                            return;
                        }

                        string to = shippingEntity.First().rcemail;
                        string subject = "A gift for You";
                        string body = "";

                        string namefrom = userEntity.First().first_name + " " + userEntity.First().last_name;
                        string nameto = shippingEntity.First().first_name + " " + shippingEntity.First().last_name;

                        string orderno = GeneralService.TransactionCode(entityTransaction.First().id, (DateTime)entityTransaction.First().transaction_date);

                        int dealid = (int)entityTransaction.First().kzTransactionDetails.First().inventoryitem_id;

                        string imageurl = "";
                        var entityImg = from d in context.kzInventoryItemImages
                                        from e in context.kzImages
                                        where d.image_id == e.id && d.inventoryitem_id == dealid
                                        && d.main == true
                                        select new { d.image_id, e.url };

                        if (entityImg.Count() > 0)
                        {
                            imageurl = ConfigurationManager.AppSettings["uploadpath"] + entityImg.First().url;
                        }

                        string dealname = entityTransaction.First().kzTransactionDetails.First().kzInventoryItem.name;

                        string note = shippingEntity.First().note;

                        string variance = entityTransaction.First().kzTransactionDetails.First().variance;
                        string[] _vari = variance.Split('`');
                        string variname = _vari[0];

                        DateTime expirationdate;
                        expirationdate = DateTime.UtcNow; /////for testing purpose

                        ///

                        //string url = WebSetting.Url + "/Home/Deals/" + kuazoo.helper.ReplaceSymbol.Replace(dealname) + "?d=1";
                        string fineprint = entityTransaction.First().kzTransactionDetails.First().kzInventoryItem.terms;


                        //var entityVariance = from d in context.kzVariances
                        //                     where d.name == variname && d.inventoryitem_id == dealid
                        //                     select d;
                        //string sku = "";
                        //if (entityVariance.Count() > 0)
                        //{
                        //    sku = entityVariance.First().sku;
                        //}


                        if (entityTransaction.First().kzTransactionDetails.First().kzInventoryItem.inventoryitem_type_id == 1)
                        {
                            body = helper.Email.CreateGiftEmail(dealname, imageurl, nameto, namefrom, note, variname, fineprint, orderno, expirationdate);

                            new GeneralService().SendEmail(subject, to, body);
                        }
                        else
                        {
                            List<string> voucherlist = new List<string>();
                            string vouchercode = "";
                            var entityCode = from d in context.kzPreCodes
                                             where d.transaction_id == Transactionid
                                             select d;
                            //int i = 0;
                            //foreach (var vo in entityCode)
                            //{
                            //    i++;
                            //    if (i == 1)
                            //    {
                            //        vouchercode += "<tr>";
                            //    }
                            //    voucherlist.Add(vo.code);
                            //    vouchercode += "<td><strong>Voucher Code: </strong> " + vo.code + "</td>";
                            //    if (i == 3)
                            //    {
                            //        vouchercode += "</tr>";
                            //        i = 0;
                            //    }
                            //}
                            //if (i != 0)
                            //{
                            //    vouchercode += "</tr>";
                            //}
                            foreach (var vo in entityCode)
                            {
                                vouchercode += "<div>" + vo.code + "</div>";
                            }

                            body = helper.Email.CreateGiftServicesEmail(dealname, imageurl, nameto, namefrom, note, variname, fineprint, orderno, expirationdate, vouchercode);

                            new GeneralService().SendEmail(subject, to, body);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public MemoryStream Getpdf(string subject, List<String> voucher)
        {
            MemoryStream workStream = new MemoryStream();
            Document document = new Document(iTextSharp.text.PageSize.LETTER, 10, 10, 42, 35);
            PdfWriter.GetInstance(document, workStream).CloseStream = false;

            document.Open();
            document.Add(new Paragraph(subject));
            document.Add(new Paragraph("Your Voucher Code as below:"));
            foreach (var v in voucher)
            {
                document.Add(new Paragraph(v));
            }
            document.Close();
            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;

            return workStream;
        }

        public MemoryStream GetpdfHtml(string html)
        {
            MemoryStream workStream = new MemoryStream();
            Document document = new Document(iTextSharp.text.PageSize.LETTER, 10, 10, 42, 35);
            PdfWriter.GetInstance(document, workStream).CloseStream = false;

            HTMLWorker hw = new HTMLWorker(document);
            document.Open();
            hw.Parse(new StringReader(html.ToString()));
            document.Close();
            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;

            return workStream;
        }
        public MemoryStream GetpdfHtml2(string html, string path)
        {

            byte[] pdfByteArray = Rotativa.WkhtmltopdfDriver.ConvertHtml(path, "-q", html);
            var input = new MemoryStream(pdfByteArray);
            return input;
        }

    }
}
