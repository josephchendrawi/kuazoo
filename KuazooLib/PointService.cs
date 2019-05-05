using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Net.Mail;
using System.Threading;

namespace com.kuazoo
{
    public class PointService : IPointService
    {

        public Response<bool> UpdatePointAction(PointAction item)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                if (item.PointActionId == 0)
                {
                    throw new CustomException(CustomErrorType.PointServiceUnknown);
                }
                else
                {
                    var entityType = from d in context.kzKPointActions
                                     where d.code == item.Code
                                     && d.amount != item.Amount
                                     && d.last_action !="5"
                                     select d;
                    if (entityType.Count() > 0)
                    {
                        entityType.First().last_updated = DateTime.UtcNow;
                        entityType.First().last_action = "5";
                        entity.kzKPointAction pa = new entity.kzKPointAction();
                        pa.code = item.Code;
                        pa.description = item.Desc;
                        pa.amount = item.Amount;
                        pa.last_action = "1";
                        context.AddTokzKPointActions(pa);
                        context.SaveChanges();
                    }
                }
                response = Response<bool>.Create(true);
            }
            return response;
        }
        public Response<List<PointAction>> GetListPointAction()
        {
            Response<List<PointAction>> response = null;
            List<PointAction> listAction = new List<PointAction>();
            using (var context = new entity.KuazooEntities())
            {
                var entityAction = from d in context.kzKPointActions
                                   where d.last_action != "5"
                                   select d;
                foreach (var v in entityAction)
                {
                    PointAction pa = new PointAction();
                    pa.PointActionId = v.id;
                    pa.Code = (int)v.code;
                    pa.Amount = (decimal)v.amount;
                    pa.Desc = v.description;
                    listAction.Add(pa);
                }
                response = Response<List<PointAction>>.Create(listAction);
            }
            return response;
        }
        public Response<PointAction> GetPointActionById(int id)
        {
            Response<PointAction> response = null;
            PointAction pa = new PointAction();
            using (var context = new entity.KuazooEntities())
            {
                var entityAction = from d in context.kzKPointActions
                                   where d.last_action != "5" && d.id == id
                                   select d;
                if(entityAction.Count()>0)
                {
                    pa.PointActionId = entityAction.First().id;
                    pa.Code = (int)entityAction.First().code;
                    pa.Amount = (decimal)entityAction.First().amount;
                    pa.Desc = entityAction.First().description;
                }
                response = Response<PointAction>.Create(pa);
            }
            return response;
        }
        public static Response<decimal> GetPointAction(KPointAction action)
        {
            Response<decimal> response = null;
            decimal point = 0;
            using (var context = new entity.KuazooEntities())
            {
                int code = (int)action;
                var entitypointAction = from d in context.kzKPointActions
                                        where d.last_action != "5"
                                        && d.code == code
                                        select d;
                if (entitypointAction.Count() > 0)
                {
                    point = (decimal)entitypointAction.First().amount;
                }
            }
            response = Response<decimal>.Create(point);
            return response;
        }

        public Response<bool> CheckLimit(PointDetail item,int gmt)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                if (item.Type == KPointAction.PostAfterPurchase || item.Type == KPointAction.PostBeforePurchase)
                {
                    var entityPointHeader = from d in context.kzKPointTrxHs
                                            where d.user_id == item.UserId
                                            && d.last_action != "5"
                                            select d;
                    if (entityPointHeader.Count() > 0)
                    {
                        DateTime today = DateTime.UtcNow.AddHours(gmt).Date;
                        DateTime nextday = today.AddDays(1);
                        int actiontype1 = (int)KPointAction.PostAfterPurchase;
                        int actiontype2 = (int)KPointAction.PostBeforePurchase;
                        int pointheaderid = (int)entityPointHeader.First().id;
                        if (item.Remarks.Contains("Email"))
                        {
                            var entityPointDetail = from d in context.kzKPointTrxDs
                                                    where d.kpointh_id == pointheaderid
                                                    && (d.action_type == actiontype1 || d.action_type == actiontype2)
                                                    && d.remarks.Contains("Email")
                                                    && System.Data.Objects.EntityFunctions.AddHours(d.last_created, gmt) >= today && System.Data.Objects.EntityFunctions.AddHours(d.last_created, gmt) < nextday
                                                    select d;
                            if(entityPointDetail.Count()>=3)
                            {
                                response = Response<bool>.Create(false);
                            }
                            else
                            {
                                response = Response<bool>.Create(true);
                            }
                        }
                        else
                        {
                            var entityPointDetail = from d in context.kzKPointTrxDs
                                                    where d.kpointh_id == pointheaderid
                                                    && (d.action_type == actiontype1 || d.action_type == actiontype2)
                                                    && d.remarks.Contains(item.Remarks)
                                                    && System.Data.Objects.EntityFunctions.AddHours(d.last_created, gmt) >= today && System.Data.Objects.EntityFunctions.AddHours(d.last_created, gmt) < nextday
                                                    select d;
                            if (entityPointDetail.Count() >= 3)
                            {
                                response = Response<bool>.Create(false);
                            }
                            else
                            {
                                var entityPointDetail2 = from d in context.kzKPointTrxDs
                                                         where d.kpointh_id == pointheaderid
                                                         && d.inventoryitem_id == item.InventoryItemId
                                                         && (d.action_type == actiontype1 || d.action_type == actiontype2)
                                                         && System.Data.Objects.EntityFunctions.AddHours(d.last_created, gmt) >= today && System.Data.Objects.EntityFunctions.AddHours(d.last_created, gmt) < nextday
                                                         && d.remarks.Contains(item.Remarks)
                                                         select d;
                                if (entityPointDetail2.Count() >= 1)
                                {
                                    response = Response<bool>.Create(false);
                                }
                                else
                                {
                                    response = Response<bool>.Create(true);
                                }
                            }
                        }
                    }
                    else
                    {
                        response = Response<bool>.Create(true);
                    }
                }
                else
                {
                    response = Response<bool>.Create(true);
                }
            }
            return response;
        }
        public Response<bool> CreatePoint(PointDetail item)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                var entityPointHeader = from d in context.kzKPointTrxHs
                                        where d.user_id == item.UserId
                                        && d.last_action != "5"
                                        select d;
                if(entityPointHeader.Count() >0)
                {
                    entity.kzKPointTrxD detail = new entity.kzKPointTrxD();
                    detail.kpointh_id = entityPointHeader.First().id;
                    detail.action_type = (int)item.Type;
                    if (item.Type == KPointAction.Redeemed)
                    {
                        detail.amount = item.Amount * -1;
                        entityPointHeader.First().balance = entityPointHeader.First().balance - item.Amount;
                    }
                    else if (item.Type == KPointAction.Transfer)
                    {
                        if (item.UserId == item.FromUser)
                        {
                            detail.amount = item.Amount * -1;
                            entityPointHeader.First().balance = entityPointHeader.First().balance - item.Amount;
                        }
                        else
                        {
                            detail.amount = item.Amount;
                            entityPointHeader.First().balance = entityPointHeader.First().balance + item.Amount;
                        }
                        detail.from_user = item.FromUser;
                        detail.to_user = item.ToUser;
                    }
                    else if (item.Type == KPointAction.PostAfterPurchase)
                    {
                        detail.inventoryitem_id = item.InventoryItemId;
                        detail.amount = item.Amount;
                        entityPointHeader.First().balance = entityPointHeader.First().balance + item.Amount;
                    }
                    else if (item.Type == KPointAction.PostBeforePurchase)
                    {
                        detail.inventoryitem_id = item.InventoryItemId;
                        detail.amount = item.Amount;
                        entityPointHeader.First().balance = entityPointHeader.First().balance + item.Amount;
                    }
                    else if (item.Type == KPointAction.PurchaseItem)
                    {
                        detail.inventoryitem_id = item.InventoryItemId;
                        detail.transaction_id = item.TransactionId;
                        detail.amount = item.Amount;
                        entityPointHeader.First().balance = entityPointHeader.First().balance + item.Amount;
                    }
                    else
                    {
                        detail.amount = item.Amount;
                        entityPointHeader.First().balance = entityPointHeader.First().balance + item.Amount;
                    }
                    detail.remarks = item.Remarks;
                    detail.last_created = DateTime.UtcNow;
                    entityPointHeader.First().last_action = "3";
                    entityPointHeader.First().last_updated = DateTime.UtcNow;
                    context.AddTokzKPointTrxDs(detail);
                    context.SaveChanges();
                    if (item.Type == KPointAction.Transfer)
                    {
                        item.UserId = item.ToUser;
                        CreatePoint(item);
                    }
                }
                else
                {
                    entity.kzKPointTrxH header = new entity.kzKPointTrxH();
                    header.user_id = item.UserId;
                    header.balance = 0;
                    header.last_action = "1";
                    header.last_updated = DateTime.UtcNow;
                    context.AddTokzKPointTrxHs(header);
                    context.SaveChanges();
                    CreatePoint(item);
                }
                response = Response<bool>.Create(true);
            }
            return response;
        }

        public void CreatePointByContext(entity.KuazooEntities context, PointDetail item)
        {
            var entityPointHeader = from d in context.kzKPointTrxHs
                                    where d.user_id == item.UserId
                                    && d.last_action != "5"
                                    select d;
            if (entityPointHeader.Count() > 0)
            {
                entity.kzKPointTrxD detail = new entity.kzKPointTrxD();
                detail.kpointh_id = entityPointHeader.First().id;
                detail.action_type = (int)item.Type;
                if (item.Type == KPointAction.Redeemed)
                {
                    detail.inventoryitem_id = item.InventoryItemId;
                    detail.amount = item.Amount * -1;
                    detail.transaction_id = item.TransactionId;
                    entityPointHeader.First().balance = entityPointHeader.First().balance - item.Amount;
                }
                else if (item.Type == KPointAction.Transfer)
                {
                    if (item.UserId == item.FromUser)
                    {
                        detail.amount = item.Amount * -1;
                        entityPointHeader.First().balance = entityPointHeader.First().balance - item.Amount;
                    }
                    else
                    {
                        detail.amount = item.Amount;
                        entityPointHeader.First().balance = entityPointHeader.First().balance + item.Amount;
                    }
                    detail.from_user = item.FromUser;
                    detail.to_user = item.ToUser;
                }
                else if (item.Type == KPointAction.PostAfterPurchase)
                {
                    detail.inventoryitem_id = item.InventoryItemId;
                    detail.amount = item.Amount;
                    entityPointHeader.First().balance = entityPointHeader.First().balance + item.Amount;
                }
                else if (item.Type == KPointAction.PostBeforePurchase)
                {
                    detail.inventoryitem_id = item.InventoryItemId;
                    detail.amount = item.Amount;
                    entityPointHeader.First().balance = entityPointHeader.First().balance + item.Amount;
                }
                else if (item.Type == KPointAction.PurchaseItem)
                {
                    detail.inventoryitem_id = item.InventoryItemId;
                    detail.transaction_id = item.TransactionId;
                    detail.amount = item.Amount;
                    entityPointHeader.First().balance = entityPointHeader.First().balance + item.Amount;
                }
                else if(item.Type == KPointAction.Revert)
                {
                    detail.inventoryitem_id = item.InventoryItemId;
                    detail.amount = item.Amount;
                    detail.transaction_id = item.TransactionId;
                    entityPointHeader.First().balance = entityPointHeader.First().balance + item.Amount;
                }
                else
                {
                    detail.amount = item.Amount;
                    entityPointHeader.First().balance = entityPointHeader.First().balance + item.Amount;
                }
                detail.remarks = item.Remarks;
                detail.last_created = DateTime.UtcNow;
                entityPointHeader.First().last_action = "3";
                entityPointHeader.First().last_updated = DateTime.UtcNow;
                context.AddTokzKPointTrxDs(detail);
                context.SaveChanges();
                if (item.Type == KPointAction.Transfer)
                {
                    item.UserId = item.ToUser;
                    CreatePoint(item);
                }
            }
            else
            {
                entity.kzKPointTrxH header = new entity.kzKPointTrxH();
                header.user_id = item.UserId;
                header.balance = 0;
                header.last_action = "1";
                header.last_updated = DateTime.UtcNow;
                context.AddTokzKPointTrxHs(header);
                context.SaveChanges();
                CreatePoint(item);
            }
        }

        public Response<PointHeader> GetListPoint()
        {
            Response<PointHeader> response = null;
            PointHeader header = new PointHeader();
            List<PointDetail> listDetail = new List<PointDetail>();
            using (var context = new entity.KuazooEntities())
            {
                var user = new UserService().GetCurrentUser().Result;
                if (user.UserId != 0)
                {
                    var entitypointHeader = from d in context.kzKPointTrxHs
                                            where d.user_id == user.UserId
                                            && d.last_action != "5"
                                            select d;
                    if (entitypointHeader.Count() > 0 && entitypointHeader.First().kzKPointTrxDs.Count()>0)
                    {
                        header.PointHeaderId = (int)entitypointHeader.First().id;
                        header.UserId = (int)entitypointHeader.First().user_id;
                        header.Balance = (decimal)entitypointHeader.First().balance;
                        foreach (var v in entitypointHeader.First().kzKPointTrxDs.OrderByDescending(x=>x.last_created))
                        {
                            PointDetail detail = new PointDetail();
                            detail.UserId = header.UserId;
                            detail.PointDetailId = v.id;
                            detail.Amount = (decimal)v.amount;
                            detail.Type = (KPointAction)v.action_type;
                            detail.FromUser =0;
                            detail.FromUserName = "";
                            if (v.from_user != null)
                            {
                                detail.FromUser = (int)v.from_user;
                                detail.FromUserName = UserService.GetUserNameById((int)v.from_user);
                            }
                            detail.ToUser = 0;
                            detail.ToUserName = "";
                            if (v.to_user != null)
                            {
                                detail.ToUser = (int)v.to_user;
                                detail.ToUserName = UserService.GetUserNameById((int)v.to_user);
                            }
                            detail.Remarks = v.remarks;
                            detail.Create = (DateTime)v.last_created;
                            detail.InventoryItemId = 0;
                            detail.InventoryItemName = "";
                            if (v.inventoryitem_id != null)
                            {
                                detail.InventoryItemId = (int)v.inventoryitem_id;
                                detail.InventoryItemName = v.kzInventoryItem.name;
                            }
                            detail.TransactionId = 0;
                            if (v.transaction_id != null)
                            {
                                detail.TransactionId = (int)v.transaction_id;
                            }

                            listDetail.Add(detail);
                        }
                        header.ListDetail = listDetail;
                    }
                    response = Response<PointHeader>.Create(header);
                }
            }
            return response;
        }
        public static Response<decimal> GetPointBalance(int UserId)
        {
            Response<decimal> response = null;
            decimal point = 0;
            using (var context = new entity.KuazooEntities())
            {
                if (UserId != 0)
                {
                    var entitypointHeader = from d in context.kzKPointTrxHs
                                            where d.user_id == UserId
                                            && d.last_action != "5"
                                            select d;
                    if (entitypointHeader.Count() > 0)
                    {
                        point = (decimal)entitypointHeader.First().balance;
                    }
                }
            }
            response = Response<decimal>.Create(point);
            return response;
        }

    }
}
