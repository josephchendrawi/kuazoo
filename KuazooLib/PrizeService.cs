using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Net.Mail;
using System.Net;
using com.kuazoo.helper;

namespace com.kuazoo
{
    public class PrizeService : IPrizeService
    {

        private DistanceApi distanceApi = new HarversineFormula();

        public Response<bool> CreatePrize(Prize item)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                if (item.PrizeId != 0)
                {
                    var entityPrize = from d in context.kzPrizes
                                              where d.id == item.PrizeId
                                     select d;
                    if (entityPrize.Count() > 0)
                    {
                        var entityPrize2 = from d in context.kzPrizes
                                                   where d.name.ToLower() == item.Name.ToLower()
                                          && d.id != item.PrizeId
                                          select d;
                        if (entityPrize2.Count() > 0)
                        {
                            throw new CustomException(CustomErrorType.PrizeAlreadyAssign);
                        }
                        else
                        {
                            entityPrize.First().name = item.Name;
                            entityPrize.First().price = item.Price;
                            entityPrize.First().description = item.Description;
                            entityPrize.First().sponsor_name = item.SponsorName;
                            entityPrize.First().terms = item.Terms;
                            entityPrize.First().detail = item.Detail;
                            entityPrize.First().expiry_date = item.ExpiryDate;
                            entityPrize.First().publish_date = item.PublishDate;
                            entityPrize.First().group_name = item.GroupName;
                            entityPrize.First().total_revenue = item.TotalRevenue;
                            entityPrize.First().last_updated = DateTime.UtcNow;
                            entityPrize.First().last_action = "3";
                            entityPrize.First().fake_visualmeter = item.FakeVisualMeter;
                            entityPrize.First().freedeal = item.FreeDeal;
                            context.SaveChanges();
                            #region image
                            if (item.ImageId != null)
                            {
                                try
                                {
                                    var entityImage = from d in context.kzPrizeImages
                                                      where d.prize_id == item.PrizeId
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
                                                entity.kzPrizeImage mmimage = new entity.kzPrizeImage();
                                                mmimage.prize_id = item.PrizeId;
                                                mmimage.image_id = item.ImageId;
                                                mmimage.main = true;
                                                context.AddTokzPrizeImages(mmimage);
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
                                        entity.kzPrizeImage mmimage = new entity.kzPrizeImage();
                                        mmimage.prize_id = item.PrizeId;
                                        mmimage.image_id = item.ImageId;
                                        mmimage.main = true;
                                        context.AddTokzPrizeImages(mmimage);
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
                                    var entityImage = from d in context.kzPrizeImages
                                                      where d.prize_id == item.PrizeId
                                                      && d.main == false
                                                      select d;
                                    foreach (var v in entityImage)
                                    {
                                        context.DeleteObject(v);
                                    }
                                    context.SaveChanges();
                                    foreach (var v in item.SubImageId)
                                    {
                                        entity.kzPrizeImage mmimage = new entity.kzPrizeImage();
                                        mmimage.prize_id = item.PrizeId;
                                        mmimage.image_id = v;
                                        mmimage.main = false;
                                        context.AddTokzPrizeImages(mmimage);
                                        context.SaveChanges();
                                    }
                                }
                                catch
                                {

                                }
                            }
                            else
                            {
                                var entityImage = from d in context.kzPrizeImages
                                                  where d.prize_id == item.PrizeId
                                                  && d.main == false
                                                  select d;
                                foreach (var v in entityImage)
                                {
                                    context.DeleteObject(v);
                                }
                                context.SaveChanges();
                            }

                            #endregion
                            context.SaveChanges();
                            #region game
                             var entitygame = from d in context.kzGames
                                where d.last_action != "5" && d.prize_id == item.PrizeId
                                select d;
                             if (entitygame.Count() > 0)
                             {
                                 entitygame.First().expiry_date = item.ExpiryDate;
                                 context.SaveChanges();
                             }
                            #endregion
                            response = Response<bool>.Create(true);
                        }
                    }
                    else
                    {
                        throw new CustomException(CustomErrorType.PrizeNotFound);
                    }
                }
                else
                {
                    var entityPrize = from d in context.kzPrizes
                                              where d.name.ToLower() == item.Name.ToLower()
                                     select d;
                    if (entityPrize.Count() > 0)
                    {
                        throw new CustomException(CustomErrorType.PrizeAlreadyAssign);
                    }
                    else
                    {
                        entity.kzPrize mmentity = new entity.kzPrize();
                        mmentity.name = item.Name;
                        mmentity.price = item.Price;
                        mmentity.description = item.Description;
                        mmentity.sponsor_name = item.SponsorName;
                        mmentity.terms = item.Terms;
                        mmentity.detail = item.Detail;
                        mmentity.expiry_date = item.ExpiryDate;
                        mmentity.publish_date = item.PublishDate;
                        mmentity.group_name = item.GroupName;
                        mmentity.total_revenue = item.TotalRevenue;
                        mmentity.last_created = DateTime.UtcNow;
                        mmentity.last_updated = DateTime.UtcNow;
                        mmentity.last_action = "1";
                        mmentity.fake_visualmeter = item.FakeVisualMeter;
                        mmentity.freedeal = item.FreeDeal;
                        context.AddTokzPrizes(mmentity);
                        context.SaveChanges();
                        int Prizeid = mmentity.id;
                        #region image
                        if (item.ImageId != null || item.ImageId != 0)
                        {
                            try
                            {
                                entity.kzPrizeImage mmimage = new entity.kzPrizeImage();
                                mmimage.prize_id = Prizeid;
                                mmimage.image_id = item.ImageId;
                                mmimage.main = true;
                                context.AddTokzPrizeImages(mmimage);
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
                                    entity.kzPrizeImage mmimage = new entity.kzPrizeImage();
                                    mmimage.prize_id = Prizeid;
                                    mmimage.image_id = v;
                                    mmimage.main = false;
                                    context.AddTokzPrizeImages(mmimage);
                                    context.SaveChanges();
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
        public Response<bool> UpdatePrizeGameType(Prize item)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                if (item.PrizeId != 0)
                {
                    var entityPrize = from d in context.kzPrizes
                                      where d.id == item.PrizeId
                                      select d;
                    if (entityPrize.Count() > 0)
                    {
                        entityPrize.First().game_type = item.GameType;
                        context.SaveChanges();
                        response = Response<bool>.Create(true);
                    }
                    else
                    {
                        throw new CustomException(CustomErrorType.PrizeNotFound);
                    }
                }
                else
                {
                    throw new CustomException(CustomErrorType.PrizeNotFound);
                }
            }
            return response;
        }
        public Response<bool> DeletePrize(int PrizeId)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                var entityUser = from d in context.kzPrizes
                                 where d.id == PrizeId
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
        public Response<List<Prize>> GetPrizeList()
        {
            Response<List<Prize>> response = null;
            List<Prize> typeList = new List<Prize>();
            using (var context = new entity.KuazooEntities())
            {
                var entitytag = from d in context.kzPrizes
                                where d.last_action != "5"
                                orderby d.last_created descending
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
                    type.ExpiryDate = (DateTime)v.expiry_date;
                    type.PublishDate = (DateTime)v.publish_date;
                    type.GroupName = v.group_name;
                    type.TotalRevenue = (decimal)v.total_revenue;
                    type.LastAction = v.last_action;
                    type.Create = (DateTime)v.last_created;
                    type.GameType = 0;
                    type.FakeVisualMeter = (decimal)v.fake_visualmeter;
                    if (v.game_type != null)
                    {
                        type.GameType = (int)v.game_type;
                    }
                    var entityImgPrize = from d in context.kzPrizeImages
                                         from e in context.kzImages
                                         where d.image_id == e.id && d.prize_id == v.id
                                         && d.main == true
                                         select new { d.image_id, e.url };

                    if (entityImgPrize.Count() > 0)
                    {
                        type.ImageId = entityImgPrize.First().image_id;
                        type.ImageName = entityImgPrize.First().url;
                        type.ImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImgPrize.First().url;
                    }
                    else
                    {
                        type.ImageId = 0;
                        type.ImageName = "";
                        type.ImageUrl = "";
                    }

                    List<int> subimgidprize = new List<int>();
                    List<string> subimgnameprize = new List<string>();
                    List<string> subimgurlprize = new List<string>();
                    var entitySubImgprize = from d in context.kzPrizeImages
                                            from e in context.kzImages
                                            where d.image_id == e.id && d.prize_id == v.id
                                            && d.main == false
                                            select new { d.image_id, e.url };
                    foreach (var v2 in entitySubImgprize)
                    {
                        subimgidprize.Add(v2.image_id);
                        subimgnameprize.Add(v2.url);
                        subimgurlprize.Add(ConfigurationManager.AppSettings["uploadpath"] + v2.url);
                    }
                    type.SubImageId = subimgidprize;
                    type.SubImageName = subimgnameprize;
                    type.SubImageUrl = subimgurlprize;
                    type.WinnerEmail = "";
                    var entityWinner = from d in context.kzWinners
                                       where d.prize_id == type.PrizeId
                                       select d;
                    if (entityWinner.Count() > 0)
                    {
                        type.WinnerEmail = entityWinner.First().kzUser.email;
                    }
                    typeList.Add(type);
                }
                response = Response<List<Prize>>.Create(typeList);
            }

            return response;
        }
        public Response<List<Prize>> GetArchivedPrizeList()
        {
            Response<List<Prize>> response = null;
            List<Prize> typeList = new List<Prize>();
            using (var context = new entity.KuazooEntities())
            {
                var entitytag = from d in context.kzPrizes
                                where d.last_action == "5"
                                orderby d.last_created descending
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
                    type.ExpiryDate = (DateTime)v.expiry_date;
                    type.PublishDate = (DateTime)v.publish_date;
                    type.GroupName = v.group_name;
                    type.TotalRevenue = (decimal)v.total_revenue;
                    type.LastAction = v.last_action;
                    type.Create = (DateTime)v.last_created;
                    type.GameType = 0;
                    type.FakeVisualMeter = (decimal)v.fake_visualmeter;
                    if (v.game_type != null)
                    {
                        type.GameType = (int)v.game_type;
                    }
                    var entityImgPrize = from d in context.kzPrizeImages
                                         from e in context.kzImages
                                         where d.image_id == e.id && d.prize_id == v.id
                                         && d.main == true
                                         select new { d.image_id, e.url };

                    if (entityImgPrize.Count() > 0)
                    {
                        type.ImageId = entityImgPrize.First().image_id;
                        type.ImageName = entityImgPrize.First().url;
                        type.ImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImgPrize.First().url;
                    }
                    else
                    {
                        type.ImageId = 0;
                        type.ImageName = "";
                        type.ImageUrl = "";
                    }

                    List<int> subimgidprize = new List<int>();
                    List<string> subimgnameprize = new List<string>();
                    List<string> subimgurlprize = new List<string>();
                    var entitySubImgprize = from d in context.kzPrizeImages
                                            from e in context.kzImages
                                            where d.image_id == e.id && d.prize_id == v.id
                                            && d.main == false
                                            select new { d.image_id, e.url };
                    foreach (var v2 in entitySubImgprize)
                    {
                        subimgidprize.Add(v2.image_id);
                        subimgnameprize.Add(v2.url);
                        subimgurlprize.Add(ConfigurationManager.AppSettings["uploadpath"] + v2.url);
                    }
                    type.SubImageId = subimgidprize;
                    type.SubImageName = subimgnameprize;
                    type.SubImageUrl = subimgurlprize;
                    type.WinnerEmail = "";
                    var entityWinner = from d in context.kzWinners
                                       where d.prize_id == type.PrizeId
                                       select d;
                    if (entityWinner.Count() > 0)
                    {
                        type.WinnerEmail = entityWinner.First().kzUser.email;
                    }
                    typeList.Add(type);
                }
                response = Response<List<Prize>>.Create(typeList);
            }

            return response;
        }
        public Response<List<Prize>> GetPrizeListExpireSoon()
        {
            Response<List<Prize>> response = null;
            List<Prize> typeList = new List<Prize>();
            using (var context = new entity.KuazooEntities())
            {
                DateTime today = DateTime.UtcNow;
                DateTime daymin5 = today.AddDays(-5);
                var entitytag = from d in context.kzPrizes
                                where d.last_action != "5" && d.expiry_date>=daymin5 && d.expiry_date<=today
                                orderby d.last_created descending
                                select d;
                foreach (var v in entitytag)
                {
                    Prize type = new Prize();
                    type.PrizeId = v.id;
                    type.Name = v.name;
                    type.ExpiryDate = (DateTime)v.expiry_date;
                    typeList.Add(type);
                }
                response = Response<List<Prize>>.Create(typeList);
            }

            return response;
        }
        public Response<Prize> GetPrizeById(int PrizeId)
        {
            Response<Prize> response = null;
            Prize type = new Prize();
            using (var context = new entity.KuazooEntities())
            {
                var entitytag = from d in context.kzPrizes
                                where d.last_action != "5" && d.id ==PrizeId
                                select d;
                if(entitytag.Count()>0)
                {
                    var v = entitytag.FirstOrDefault();
                    type.PrizeId = v.id;
                    type.Name = v.name;
                    type.SponsorName = v.sponsor_name;
                    type.Price = (decimal)v.price;
                    type.Description = v.description;
                    type.Terms = v.terms;
                    type.Detail = v.detail;
                    type.ExpiryDate = (DateTime)v.expiry_date;
                    type.PublishDate = (DateTime)v.publish_date;
                    type.GroupName = v.group_name;
                    type.TotalRevenue = (decimal)v.total_revenue;
                    type.LastAction = v.last_action;
                    type.GameType = 0;
                    type.FakeVisualMeter = (decimal)v.fake_visualmeter;
                    type.FreeDeal = (bool)v.freedeal;
                    if (v.game_type != null)
                    {
                        type.GameType = (int)v.game_type;
                    }
                    var entityImgPrize = from d in context.kzPrizeImages
                                         from e in context.kzImages
                                         where d.image_id == e.id && d.prize_id == v.id
                                         && d.main == true
                                         select new { d.image_id, e.url };

                    if (entityImgPrize.Count() > 0)
                    {
                        type.ImageId = entityImgPrize.First().image_id;
                        type.ImageName = entityImgPrize.First().url;
                        type.ImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImgPrize.First().url;
                    }
                    else
                    {
                        type.ImageId = 0;
                        type.ImageName = "";
                        type.ImageUrl = "";
                    }

                    List<int> subimgidprize = new List<int>();
                    List<string> subimgnameprize = new List<string>();
                    List<string> subimgurlprize = new List<string>();
                    var entitySubImgprize = from d in context.kzPrizeImages
                                            from e in context.kzImages
                                            where d.image_id == e.id && d.prize_id == v.id
                                            && d.main == false
                                            select new { d.image_id, e.url };
                    foreach (var v2 in entitySubImgprize)
                    {
                        subimgidprize.Add(v2.image_id);
                        subimgnameprize.Add(v2.url);
                        subimgurlprize.Add(ConfigurationManager.AppSettings["uploadpath"] + v2.url);
                    }
                    type.SubImageId = subimgidprize;
                    type.SubImageName = subimgnameprize;
                    type.SubImageUrl = subimgurlprize;

                    type.WinnerEmail = "";
                    var entityWinner = from d in context.kzWinners
                                        where d.prize_id == PrizeId
                                        select d;
                    if (entityWinner.Count() > 0)
                    {
                        type.WinnerEmail = entityWinner.First().kzUser.email;
                    }
                    type.ClosestWinnerEmail = "";
                    var entityWinnerTransaction = from d in context.kzGameTransactions
                                                  where d.kzGame.prize_id == PrizeId
                                                  select d;
                    string tempClosest = "";
                    double tempClosestValue = 0;
                    int xfirst = 0;
                    double tempValue = 0;
                    //double tempX, tempY = 0;
                    foreach (var vt in entityWinnerTransaction)
                    {
                        //tempX = (double)(vt.kzGame.hidden_latitude - vt.hidden_latitude);
                        //tempY = (double)(vt.kzGame.hidden_longitude - vt.hidden_longitude);
                        //if (tempX < 0) tempX = tempX * -1;
                        //if (tempY < 0) tempY = tempY * -1;
                        tempValue = distanceApi.GetDistance((double)vt.kzGame.hidden_latitude, (double)vt.kzGame.hidden_longitude, (double)vt.hidden_latitude, (double)vt.hidden_longitude, DistanceApi.DistanceType.Kilometers);
                        if (xfirst == 0)
                        {
                            //tempClosestValue = (tempX + tempY);
                            tempClosestValue = tempValue;
                            tempClosest = vt.kzUser.email;
                            xfirst = 1;
                        }
                        else
                        {
                            if (tempValue < tempClosestValue)
                            {
                                tempClosestValue = tempValue;
                                tempClosest = vt.kzUser.email;
                            }
                            //if ((tempX + tempY) < tempClosestValue)
                            //{
                            //    tempClosestValue = (tempX + tempY);
                            //    tempClosest = vt.kzUser.email;
                            //}
                        }
                    }
                    if (tempClosest != "")
                    {
                        type.ClosestWinnerEmail = tempClosest;
                    }
                }
                response = Response<Prize>.Create(type);
            }

            return response;
        }
        public Response<Prize> GetArchivedPrizeById(int PrizeId)
        {
            Response<Prize> response = null;
            Prize type = new Prize();
            using (var context = new entity.KuazooEntities())
            {
                var entitytag = from d in context.kzPrizes
                                where d.last_action == "5" && d.id == PrizeId
                                select d;
                if (entitytag.Count() > 0)
                {
                    var v = entitytag.FirstOrDefault();
                    type.PrizeId = v.id;
                    type.Name = v.name;
                    type.SponsorName = v.sponsor_name;
                    type.Price = (decimal)v.price;
                    type.Description = v.description;
                    type.Terms = v.terms;
                    type.Detail = v.detail;
                    type.ExpiryDate = (DateTime)v.expiry_date;
                    type.PublishDate = (DateTime)v.publish_date;
                    type.GroupName = v.group_name;
                    type.TotalRevenue = (decimal)v.total_revenue;
                    type.LastAction = v.last_action;
                    type.GameType = 0;
                    type.FakeVisualMeter = (decimal)v.fake_visualmeter;
                    if (v.game_type != null)
                    {
                        type.GameType = (int)v.game_type;
                    }
                    var entityImgPrize = from d in context.kzPrizeImages
                                         from e in context.kzImages
                                         where d.image_id == e.id && d.prize_id == v.id
                                         && d.main == true
                                         select new { d.image_id, e.url };

                    if (entityImgPrize.Count() > 0)
                    {
                        type.ImageId = entityImgPrize.First().image_id;
                        type.ImageName = entityImgPrize.First().url;
                        type.ImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImgPrize.First().url;
                    }
                    else
                    {
                        type.ImageId = 0;
                        type.ImageName = "";
                        type.ImageUrl = "";
                    }

                    List<int> subimgidprize = new List<int>();
                    List<string> subimgnameprize = new List<string>();
                    List<string> subimgurlprize = new List<string>();
                    var entitySubImgprize = from d in context.kzPrizeImages
                                            from e in context.kzImages
                                            where d.image_id == e.id && d.prize_id == v.id
                                            && d.main == false
                                            select new { d.image_id, e.url };
                    foreach (var v2 in entitySubImgprize)
                    {
                        subimgidprize.Add(v2.image_id);
                        subimgnameprize.Add(v2.url);
                        subimgurlprize.Add(ConfigurationManager.AppSettings["uploadpath"] + v2.url);
                    }
                    type.SubImageId = subimgidprize;
                    type.SubImageName = subimgnameprize;
                    type.SubImageUrl = subimgurlprize;

                    type.WinnerEmail = "";
                    var entityWinner = from d in context.kzWinners
                                       where d.prize_id == PrizeId
                                       select d;
                    if (entityWinner.Count() > 0)
                    {
                        type.WinnerEmail = entityWinner.First().kzUser.email;
                    }
                    type.ClosestWinnerEmail = "";
                    var entityWinnerTransaction = from d in context.kzGameTransactions
                                                  where d.kzGame.prize_id == PrizeId
                                                  select d;
                    string tempClosest = "";
                    double tempClosestValue = 0;
                    int xfirst = 0;
                    double tempValue = 0;
                    //double tempX, tempY = 0;
                    foreach (var vt in entityWinnerTransaction)
                    {
                        //tempX = (double)(vt.kzGame.hidden_latitude - vt.hidden_latitude);
                        //tempY = (double)(vt.kzGame.hidden_longitude - vt.hidden_longitude);
                        //if (tempX < 0) tempX = tempX * -1;
                        //if (tempY < 0) tempY = tempY * -1;
                        tempValue = distanceApi.GetDistance((double)vt.kzGame.hidden_latitude, (double)vt.kzGame.hidden_longitude, (double)vt.hidden_latitude, (double)vt.hidden_longitude, DistanceApi.DistanceType.Kilometers);
                        if (xfirst == 0)
                        {
                            //tempClosestValue = (tempX + tempY);
                            tempClosestValue = tempValue;
                            tempClosest = vt.kzUser.email;
                            xfirst = 1;
                        }
                        else
                        {
                            if (tempValue < tempClosestValue)
                            {
                                tempClosestValue = tempValue;
                                tempClosest = vt.kzUser.email;
                            }
                            //if ((tempX + tempY) < tempClosestValue)
                            //{
                            //    tempClosestValue = (tempX + tempY);
                            //    tempClosest = vt.kzUser.email;
                            //}
                        }
                    }
                    if (tempClosest != "")
                    {
                        type.ClosestWinnerEmail = tempClosest;
                    }
                }
                response = Response<Prize>.Create(type);
            }

            return response;
        }
        public Response<bool> DuplicatePrize(int PrizeId)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                Prize item = GetPrizeById(PrizeId).Result;
                item.PrizeId = 0;
                item.WinnerEmail = "";
                item.ClosestWinnerEmail = "";
                item.GameType = 0;
                item.Name = item.Name + " - Copy";
                var result = CreatePrize(item).Result;
                response = Response<bool>.Create(result);
            }

            return response;
        }


        public Response<bool> CreateWinner(int PrizeId, int TransactionId, int InventoryItemId)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                if (PrizeId != 0 && TransactionId != 0)
                {
                    var entityTransaction = from d in context.kzTransactions
                                            where d.id == TransactionId
                                            select d;
                    if (entityTransaction.Count() > 0)
                    {
                        int userid = (int)entityTransaction.First().user_id;
                        var entityWinner = from d in context.kzWinners
                                           where d.prize_id == PrizeId
                                           select d;
                        if (entityWinner.Count() > 0)
                        {
                            throw new CustomException(CustomErrorType.PrizeWinnerAlreadyAssign);
                        }
                        else
                        {
                            entity.kzWinner win = new entity.kzWinner();
                            win.prize_id = PrizeId;
                            win.user_id = userid;
                            win.transaction_id = TransactionId;
                            win.inventoryitem_id = InventoryItemId;
                            win.create_date = DateTime.UtcNow;
                            context.AddTokzWinners(win);

                            var entityInventory = from d in context.kzInventoryItems
                                                  where d.prize_id == PrizeId && d.last_action!="5"
                                                  select d;
                            foreach(var v in entityInventory){
                                v.salesvisualmeter = 100;
                            }
                            var entityPrize = from d in context.kzPrizes
                                              where d.id == PrizeId
                                              select d;
                            entityPrize.First().last_action = "5";
                            context.SaveChanges();
                            response = Response<bool>.Create(true);
                        }
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


        public Response<WinnerModel> GetWinnerList(string sort, int page, int take, int gmt)
        {
            Response<WinnerModel> response = null;
            WinnerModel model = new WinnerModel();
            List<WinnerVM> winnerlist = new List<WinnerVM>();
            using (var context = new entity.KuazooEntities())
            {
                int year = DateTime.UtcNow.Year;
                DateTime nextyear = new DateTime(year+1,1,1);
                var entityWinner = from d in context.kzWinners
                                   where System.Data.Objects.EntityFunctions.AddHours(d.create_date,gmt) < nextyear
                                   orderby d.create_date descending
                                   select d;
                int total = entityWinner.Count();
                WinnerVM vm = null;
                List<Winner> wnlist = null;
                string ss = "";
                foreach (var v in entityWinner.Skip(page*take).Take(take))
                {
                    if (sort == "day") ss = string.Format("{0:dd MMM yyyy}", v.create_date.Value.AddHours(gmt));
                    else if (sort == "week") ss = "Weeks " + GeneralService.WeeksInYear((DateTime)v.create_date.Value.AddHours(gmt)).ToString();
                    else ss = string.Format("{0:MMMM}", v.create_date.Value.AddHours(gmt));

                    if (vm == null)
                    {
                        vm = new WinnerVM();
                        vm.Sort = ss;
                        wnlist = new List<Winner>();
                    }
                    if (vm.Sort != ss)
                    {
                        vm.WinnerList = wnlist;
                        winnerlist.Add(vm);
                        vm = new WinnerVM();
                        vm.Sort = ss;
                        wnlist = new List<Winner>();
                    }
                    if (vm.Sort == ss)
                    {
                        Winner win = new Winner();
                        win.WinnerId = v.id;
                        win.PrizeName = v.kzPrize.name;
                        var entityImgPrize = from d in context.kzPrizeImages
                                             from e in context.kzImages
                                             where d.image_id == e.id && d.prize_id == v.prize_id
                                             && d.main == true
                                             select new { d.image_id, e.url };

                        if (entityImgPrize.Count() > 0)
                        {
                            win.PrizeImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImgPrize.First().url;
                        }
                        else
                        {
                            win.PrizeImageUrl = "";
                        }

                        win.WinnerDate = (DateTime)v.create_date.Value.AddHours(gmt);
                        win.FirstName = v.kzUser.first_name;
                        win.LastName = v.kzUser.last_name;
                        win.MemberImageUrl = "";
                        if (v.kzUser.kzImage != null)
                        {
                            win.MemberImageUrl = ConfigurationManager.AppSettings["uploadpath"] + v.kzUser.kzImage.url;
                        }
                        win.Email = v.kzUser.email;
                        win.City = "";
                        win.State = "";
                        win.Country = "";
                        var entityShip = from d in context.kzShippingUsers
                                         where d.user_id == v.user_id
                                         select d;
                        if (entityShip.Count() > 0)
                        {

                            win.City = entityShip.First().city;
                            win.State = entityShip.First().state;
                            win.Country = entityShip.First().country;
                        }
                        wnlist.Add(win);
                    }

                }
                if (vm != null && wnlist != null)
                {
                    vm.WinnerList = wnlist;
                    winnerlist.Add(vm);
                }
                model.Item = winnerlist;
                model.Total = total;
                response = Response<WinnerModel>.Create(model);
            }

            return response;
        }
    }
}
