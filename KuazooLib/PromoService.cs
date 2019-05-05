using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.kuazoo
{
    public class PromoService : IPromoService
    {

        public Response<bool> CreatePromotion(Promotion Promo)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                if (Promo.PromotionId != 0)
                {
                    var entityPromo = from d in context.kzPromotions
                                     where d.id == Promo.PromotionId
                                     select d;
                    if (entityPromo.Count() > 0)
                    {
                        var entityPromo2 = from d in context.kzPromotions
                                          where d.code.ToLower() == Promo.PromoCode.ToLower()
                                          && d.id != Promo.PromotionId
                                          select d;
                        if (entityPromo2.Count() > 0)
                        {
                            throw new CustomException(CustomErrorType.PromotionAlreadyAssign);
                        }
                        else
                        {
                            entityPromo.First().code = Promo.PromoCode;
                            entityPromo.First().type = Promo.PromoType;
                            entityPromo.First().value = Promo.PromoValue;
                            entityPromo.First().flag = Promo.Flag;
                            entityPromo.First().valid_date = Promo.ValidDate;
                            entityPromo.First().last_updated = DateTime.UtcNow;
                            entityPromo.First().last_action = "3";
                            context.SaveChanges();
                            response = Response<bool>.Create(true);
                        }
                    }
                    else
                    {
                        throw new CustomException(CustomErrorType.PromotionNotFound);
                    }
                }
                else
                {
                    var entityPromo = from d in context.kzPromotions
                                      where d.code.ToLower() == Promo.PromoCode.ToLower()
                                     select d;
                    if (entityPromo.Count() > 0)
                    {
                        throw new CustomException(CustomErrorType.PromotionAlreadyAssign);
                    }
                    else
                    {
                        entity.kzPromotion mmentity = new entity.kzPromotion();
                        mmentity.code = Promo.PromoCode;
                        mmentity.type = Promo.PromoType;
                        mmentity.value = Promo.PromoValue;
                        mmentity.flag = Promo.Flag;
                        mmentity.valid_date = Promo.ValidDate;
                        mmentity.last_created = DateTime.UtcNow;
                        mmentity.last_updated = DateTime.UtcNow;
                        mmentity.last_action = "1";
                        context.AddTokzPromotions(mmentity);
                        context.SaveChanges();
                        response = Response<bool>.Create(true);
                    }
                }
            }
            return response;
        }
        public Response<List<Promotion>> GetPromotionList()
        {
            Response<List<Promotion>> response = null;
            List<Promotion> PromoList = new List<Promotion>();
            using (var context = new entity.KuazooEntities())
            {
                var entityPromo = from d in context.kzPromotions
                                  orderby d.last_created descending
                                 select d;
                foreach (var v in entityPromo)
                {
                    Promotion Promo = new Promotion();
                    Promo.PromotionId = v.id;
                    Promo.PromoCode = v.code;
                    Promo.PromoType = (int)v.type;
                    Promo.PromoValue = (decimal)v.value;
                    Promo.Flag = (bool)v.flag;
                    Promo.ValidDate = (DateTime)v.valid_date;
                    Promo.LastAction = v.last_action;
                    Promo.Create = (DateTime)v.last_created;
                    PromoList.Add(Promo);
                }
                response = Response<List<Promotion>>.Create(PromoList);
            }

            return response;
        }
        public Response<Promotion> GetPromotionById(int PromotionId)
        {
            Response<Promotion> response = null;
            Promotion Promo = new Promotion();
            using (var context = new entity.KuazooEntities())
            {
                var entityPromo = from d in context.kzPromotions
                                where d.id == PromotionId
                                select d;
                var v = entityPromo.First();
                if (v != null)
                {
                    Promo.PromotionId = v.id;
                    Promo.PromoCode = v.code;
                    Promo.PromoType = (int)v.type;
                    Promo.PromoValue = (decimal)v.value;
                    Promo.Flag = (bool)v.flag;
                    Promo.ValidDate = (DateTime)v.valid_date;
                    Promo.LastAction = v.last_action;
                }
                response = Response<Promotion>.Create(Promo);
            }

            return response;
        }
        public Response<bool> DeletePromotion(int PromotionId)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                var entityUser = from d in context.kzPromotions
                                 where d.id == PromotionId
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

    }
}
