using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Net.Mail;
using System.Net;

namespace com.kuazoo
{
    public class GameService : IGameService
    {

        public Response<bool> CreateGame(Game item)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                if (item.GameId != 0)
                {
                    var entityGame = from d in context.kzGames
                                              where d.id == item.GameId
                                     select d;
                    if (entityGame.Count() > 0)
                    {
                        //var entityGame2 = from d in context.kzGames
                        //                  where d.name.ToLower() == item.Name.ToLower()
                        //                  && d.id != item.GameId
                        //                  select d;
                        //if (entityGame2.Count() > 0)
                        //{
                        //    throw new CustomException(CustomErrorType.GameAlreadyAssign);
                        //}
                        //else
                        //{
                            entityGame.First().name = item.Name;
                            entityGame.First().prize_id = item.PrizeId;
                            entityGame.First().description = item.Description;
                            entityGame.First().instruction = item.Instruction;
                            entityGame.First().expiry_date = item.ExpiryDate;
                            entityGame.First().hidden_latitude = item.Latitude;
                            entityGame.First().hidden_longitude = item.Longitude;
                            if (item.ImageId != null && item.ImageId != 0)
                            {
                                entityGame.First().image_id = item.ImageId;
                            }
                            entityGame.First().last_updated = DateTime.UtcNow;
                            entityGame.First().last_action = "3";
                            context.SaveChanges();
                            response = Response<bool>.Create(true);
                        //}
                    }
                    else
                    {
                        throw new CustomException(CustomErrorType.GameNotFound);
                    }
                }
                else
                {
                    //var entityGame = from d in context.kzGames
                    //                          where d.name.ToLower() == item.Name.ToLower()
                    //                 select d;
                    //if (entityGame.Count() > 0)
                    //{
                    //    throw new CustomException(CustomErrorType.GameAlreadyAssign);
                    //}
                    //else
                    //{
                        entity.kzGame mmentity = new entity.kzGame();
                        mmentity.name = item.Name;
                        mmentity.prize_id = item.PrizeId;
                        mmentity.description = item.Description;
                        mmentity.instruction = item.Instruction;
                        mmentity.expiry_date = item.ExpiryDate;
                        mmentity.hidden_latitude = item.Latitude;
                        mmentity.hidden_longitude = item.Longitude;
                        mmentity.image_id = item.ImageId;
                        mmentity.last_created = DateTime.UtcNow;
                        mmentity.last_updated = DateTime.UtcNow;
                        mmentity.last_action = "1";
                        context.AddTokzGames(mmentity);
                        context.SaveChanges();
                        response = Response<bool>.Create(true);
                    //}
                }
            }
            return response;
        }
        public Response<bool> DeleteGame(int GameId)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                var entityUser = from d in context.kzGames
                                 where d.id == GameId
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
        public Response<List<Game>> GetGameList(int PrizeId)
        {
            Response<List<Game>> response = null;
            List<Game> typeList = new List<Game>();
            using (var context = new entity.KuazooEntities())
            {
                var entitytag = from d in context.kzGames
                                where d.last_action != "5" && d.prize_id == PrizeId
                                orderby d.last_created descending
                                select d;
                foreach (var v in entitytag)
                {
                    Game type = new Game();
                    type.GameId = v.id;
                    type.PrizeId = (int)v.prize_id;
                    type.PrizeName = v.kzPrize.name;
                    type.Name = v.name;
                    type.Description = v.description;
                    type.Instruction = v.instruction;
                    type.ExpiryDate = (DateTime)v.expiry_date;
                    type.Latitude = (double)v.hidden_latitude;
                    type.Longitude = (double)v.hidden_longitude;
                    type.LastAction = v.last_action;
                    type.Create = (DateTime)v.last_created;

                    var entityImgGame = from d in context.kzImages
                                        where d.id == v.image_id
                                         select d;

                    if (entityImgGame.Count() > 0)
                    {
                        type.ImageId = entityImgGame.First().id;
                        type.ImageName = entityImgGame.First().url;
                        type.ImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImgGame.First().url;
                    }
                    else
                    {
                        type.ImageId = 0;
                        type.ImageName = "";
                        type.ImageUrl = "";
                    }
                    typeList.Add(type);
                }
                response = Response<List<Game>>.Create(typeList);
            }

            return response;
        }
        public Response<Game> GetGameById(int GameId)
        {
            Response<Game> response = null;
            Game type = new Game();
            using (var context = new entity.KuazooEntities())
            {
                var entitytag = from d in context.kzGames
                                where d.last_action != "5" && d.id ==GameId
                                select d;
                if(entitytag.Count()>0)
                {
                    var v = entitytag.FirstOrDefault();
                    type.GameId = v.id;
                    type.PrizeId = (int)v.prize_id;
                    type.PrizeName = v.kzPrize.name;
                    type.Name = v.name;
                    type.Description = v.description;
                    type.Instruction = v.instruction;
                    type.ExpiryDate = (DateTime)v.expiry_date;
                    type.Latitude = (double)v.hidden_latitude;
                    type.Longitude = (double)v.hidden_longitude;
                    type.LastAction = v.last_action;
                    type.Create = (DateTime)v.last_created;

                    var entityImgGame = from d in context.kzImages
                                        where d.id == v.image_id
                                        select d;

                    if (entityImgGame.Count() > 0)
                    {
                        type.ImageId = entityImgGame.First().id;
                        type.ImageName = entityImgGame.First().url;
                        type.ImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImgGame.First().url;
                    }
                    else
                    {
                        type.ImageId = 0;
                        type.ImageName = "";
                        type.ImageUrl = "";
                    }
                }
                response = Response<Game>.Create(type);
            }

            return response;
        }

        public Response<bool> CreateGameTransaction(GameTransaction item)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                var user = new UserService().GetCurrentUser().Result;

                entity.kzGameTransaction mmentity = new entity.kzGameTransaction();
                mmentity.user_id = user.UserId;
                mmentity.transaction_id = item.TransactionId;
                mmentity.game_id = item.GameId;
                mmentity.hidden_latitude = item.UserLatitude;
                mmentity.hidden_longitude = item.UserLongitude;
                mmentity.transaction_date = DateTime.UtcNow;
                mmentity.timeused = item.TimeUsed;
                mmentity.last_action = "1";
                context.AddTokzGameTransactions(mmentity);
                context.SaveChanges();
                response = Response<bool>.Create(true);
            }
            return response;
        }
        public Response<GamePrize> GetTransactionPrizeById(int TransactionId)
        {
            Response<GamePrize> response = null;
            using (var context = new entity.KuazooEntities())
            {
                var user = new UserService().GetCurrentUser().Result;
                GamePrize pr = new GamePrize();
                var entityTransaction = from d in context.kzTransactionDetails
                                        where d.transaction_id == TransactionId && d.kzTransaction.user_id == user.UserId
                                        select d;
                int prizeid = 0;
                int qty = 0;
                pr.GameType = 0;
                if (entityTransaction.Count() > 0)
                {
                    prizeid = (int)entityTransaction.First().kzInventoryItem.prize_id;
                    qty = (int)entityTransaction.First().qty;
                    if (entityTransaction.First().kzInventoryItem.kzPrize.game_type != null)
                    {
                        pr.GameType = (int)entityTransaction.First().kzInventoryItem.kzPrize.game_type;
                    }
                }
                pr.PrizeId = prizeid;
                pr.Qty = qty;
                response = Response<GamePrize>.Create(pr);
            }

            return response;
        }
        public Response<GameFinish> GetPrizeByTransactionId(int TransactionId)
        {
            Response<GameFinish> response = null;
            using (var context = new entity.KuazooEntities())
            {
                var user = new UserService().GetCurrentUser().Result;
                GameFinish result = new GameFinish();
                var entityTransaction = from d in context.kzTransactionDetails
                                        where d.transaction_id == TransactionId && d.kzTransaction.user_id == user.UserId
                                        select d;
                if (entityTransaction.Count() > 0)
                {
                    result.InventoryItemId = (int)entityTransaction.First().inventoryitem_id;
                    result.PrizeName = entityTransaction.First().kzInventoryItem.kzPrize.name;
                    int prizeid = (int)entityTransaction.First().kzInventoryItem.prize_id;
                    var entityImgPrize = from d in context.kzPrizeImages
                                         from e in context.kzImages
                                         where d.image_id == e.id && d.prize_id == prizeid
                                         && d.main == true
                                         select new { d.image_id, e.url };
                    if (entityImgPrize.Count() > 0)
                    {
                        result.PrizeImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImgPrize.First().url;
                    }
                    else
                    {
                        result.PrizeImageUrl = "";
                    }
                    string orderno = GeneralService.TransactionCode(TransactionId, (DateTime)entityTransaction.First().kzTransaction.transaction_date);
                    result.OrderNo = orderno;

                }
                response = Response<GameFinish>.Create(result);
            }

            return response;
        }
        public Response<Game> GetGameByPrizeId(int PrizeId, DateTime today)
        {
            Response<Game> response = null;
            Game type = new Game();
            using (var context = new entity.KuazooEntities())
            {
                var entitytag = from d in context.kzGames
                                where d.last_action != "5" && d.prize_id == PrizeId
                                && d.expiry_date>=today
                                select d;
                if (entitytag.Count() > 0)
                {
                    var v = entitytag.FirstOrDefault();
                    type.GameId = v.id;
                    type.PrizeId = (int)v.prize_id;
                    type.PrizeName = v.kzPrize.name;
                    type.Name = v.name;
                    type.Description = v.description;
                    type.Instruction = v.instruction;
                    type.ExpiryDate = (DateTime)v.expiry_date;
                    type.Latitude = (double)v.hidden_latitude;
                    type.Longitude = (double)v.hidden_longitude;
                    type.LastAction = v.last_action;
                    type.Create = (DateTime)v.last_created;
                    var entityImgGame = from d in context.kzImages
                                        where d.id == v.image_id
                                        select d;

                    if (entityImgGame.Count() > 0)
                    {
                        type.ImageId = entityImgGame.First().id;
                        type.ImageName = entityImgGame.First().url;
                        type.ImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImgGame.First().url;
                    }
                    else
                    {
                        type.ImageId = 0;
                        type.ImageName = "";
                        type.ImageUrl = "";
                    }
                }
                response = Response<Game>.Create(type);
            }

            return response;
        }

        public Response<int> GetGameTransactionCount(int TransactionId, int GameId)
        {
            Response<int> response = null;
            int result = 0;
            using (var context = new entity.KuazooEntities())
            {
                var user = new UserService().GetCurrentUser().Result;
                var entityGame = from d in context.kzGameTransactions
                                where d.transaction_id == TransactionId && d.game_id == GameId && d.user_id==user.UserId
                                select d;
                if (entityGame.Count() > 0)
                {
                    result = entityGame.Count();
                }
                response = Response<int>.Create(result);
            }

            return response;
        }
    }
}
