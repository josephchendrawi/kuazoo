using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Net.Mail;

namespace com.kuazoo
{
    public class ReviewService : IReviewService
    {

        public Response<Review> CreateReview(Review item)
        {
            Response<Review> response = null;
            using (var context = new entity.KuazooEntities())
            {
                //if (item.ReviewId != 0)
                //{
                //    var entityReview = from d in context.kzReviews
                //                            where d.id == item.ReviewId
                //                            select d;
                //    if (entityReview.Count() > 0)
                //    {
                //        entityReview.First().inventoryitem_id = item.InventoryItemId;
                //        entityReview.First().user_id = item.MemberId;
                //        entityReview.First().rating = item.Rating;
                //        entityReview.First().message = item.Message;
                //        entityReview.First().review_date = item.ReviewDate;
                //        entityReview.First().last_action = "3";
                //        context.SaveChanges();
                //        response = Response<bool>.Create(true);
                //    }
                //    else
                //    {
                //        throw new CustomException(CustomErrorType.ReviewNotFound);
                //    }
                //}
                //else
                //{
                    entity.kzReview mmflash = new entity.kzReview();
                    mmflash.inventoryitem_id = item.InventoryItemId;
                    mmflash.user_id = item.MemberId;
                    mmflash.rating = item.Rating;
                    mmflash.message = item.Message;
                    mmflash.review_date = DateTime.UtcNow;
                    mmflash.last_action = "1";
                    context.AddTokzReviews(mmflash);
                    context.SaveChanges();
                    response = Response<Review>.Create(GetReviewById(mmflash.id).Result);
                //}
            }
            return response;
        }

        public Response<bool> UpdateReview(Review item)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                if (item.ReviewId != 0)
                {
                    var entityReview = from d in context.kzReviews
                                       where d.id == item.ReviewId
                                       select d;
                    if (entityReview.Count() > 0)
                    {
                        entityReview.First().rating = item.Rating;
                        entityReview.First().message = item.Message;
                        entityReview.First().last_action = "3";
                        context.SaveChanges();
                        response = Response<bool>.Create(true);
                    }
                    else
                    {
                        throw new CustomException(CustomErrorType.ReviewNotFound);
                    }
                }
                else
                {
                    throw new CustomException(CustomErrorType.ReviewNotFound);
                }
            }
            return response;
        }
        public Response<List<Review>> GetReviewByInventoryItemId(int InventoryItemId)
        {
            Response<List<Review>> response = null;
            List<Review> ReviewList = new List<Review>();
            using (var context = new entity.KuazooEntities())
            {
                var entityReview = from d in context.kzReviews.Include("kzInventoryItem").Include("kzUser")
                                   where d.inventoryitem_id == InventoryItemId
                                   && d.last_action != "5"
                                   orderby d.review_date descending
                                   select d;
                foreach (var v in entityReview)
                {
                    Review Review = new Review();
                    Review.ReviewId = (int)v.id;
                    Review.InventoryItemId = (int)v.inventoryitem_id;
                    Review.InventoryItemName = v.kzInventoryItem.name;
                    Review.MemberId = (int)v.user_id;
                    Review.MemberEmail = v.kzUser.email;
                    Review.MemberFullName = v.kzUser.first_name + " " + v.kzUser.last_name;
                    if (v.kzUser.kzFacebooks != null && v.kzUser.kzFacebooks.Count() > 0)
                    {
                        Review.MemberImage = "http://graph.facebook.com/" + v.kzUser.kzFacebooks.First().facebookid + "/picture";
                    }
                    else if (v.kzUser.image_id != null)
                    {
                        Review.MemberImage = ConfigurationManager.AppSettings["uploadpath"] + v.kzUser.kzImage.url;
                    }
                    else
                    {
                        Review.MemberImage = ConfigurationManager.AppSettings["urlpath"] + "Content/img/userimg.png";
                    }
                    Review.Rating = (int)v.rating;
                    Review.Message = v.message;
                    Review.ReviewDate = (DateTime)v.review_date;
                    Review.LastAction = v.last_action;
                    ReviewList.Add(Review);
                }
                response = Response<List<Review>>.Create(ReviewList);
            }

            return response;
        }
        public Response<List<Review>> GetReviewByInventoryItemByUser()
        {
            Response<List<Review>> response = null;
            List<Review> ReviewList = new List<Review>();
            using (var context = new entity.KuazooEntities())
            {
                var user = new UserService().GetCurrentUser().Result;
                var entityReview = from d in context.kzReviews.Include("kzInventoryItem").Include("kzUser")
                                   where d.user_id == user.UserId
                                   && d.last_action != "5"
                                   orderby d.review_date descending
                                   select d;
                foreach (var v in entityReview)
                {
                    Review Review = new Review();
                    Review.ReviewId = (int)v.id;
                    Review.InventoryItemId = (int)v.inventoryitem_id;
                    Review.InventoryItemName = v.kzInventoryItem.name;

                    var entityImg = from d in context.kzInventoryItemImages
                                    from e in context.kzImages
                                    where d.image_id == e.id && d.inventoryitem_id == Review.InventoryItemId
                                    && d.main == true
                                    select new { d.image_id, e.url };

                    if (entityImg.Count() > 0)
                    {
                        Review.InventoryItemImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImg.First().url;
                    }
                    else
                    {
                        Review.InventoryItemImageUrl = "";
                    }
                    Review.PrizeImageUrl = "";
                    if (v.kzInventoryItem.prize_id != null)
                    {
                        int PrizeId = (int)v.kzInventoryItem.prize_id;
                        var entityImgPrize = from d in context.kzPrizeImages
                                             from e in context.kzImages
                                             where d.image_id == e.id && d.prize_id == PrizeId
                                             && d.main == true
                                             select new { d.image_id, e.url };

                        if (entityImgPrize.Count() > 0)
                        {
                            Review.PrizeImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImgPrize.First().url;
                        }
                        else
                        {
                            Review.PrizeImageUrl = "";
                        }
                    }

                    Review.MemberId = (int)v.user_id;
                    Review.MemberEmail = v.kzUser.email;
                    Review.MemberFullName = v.kzUser.first_name + " " + v.kzUser.last_name;

                    if (v.kzUser.kzFacebooks != null && v.kzUser.kzFacebooks.Count() > 0)
                    {
                        Review.MemberImage = "http://graph.facebook.com/" + v.kzUser.kzFacebooks.First().facebookid + "/picture";
                    }
                    else
                    {
                        Review.MemberImage = ConfigurationManager.AppSettings["urlpath"] + "Content/img/userimg.png";
                    }
                    Review.Rating = (int)v.rating;
                    Review.Message = v.message;
                    Review.ReviewDate = (DateTime)v.review_date;
                    Review.LastAction = v.last_action;
                    ReviewList.Add(Review);
                }
                response = Response<List<Review>>.Create(ReviewList);
            }

            return response;
        }

        public static Boolean GetReviewByInventoryItemByUser(entity.KuazooEntities context, int userid, int InventoryItemId)
        {
            try{
                var entityReview = from d in context.kzReviews.Include("kzInventoryItem").Include("kzUser")
                                   where d.user_id == userid
                                   && d.last_action != "5"
                                   && d.inventoryitem_id == InventoryItemId
                                   select d;
                if(entityReview.Count()>0){
                    return true;
                }
                else{
                    return false;
                }
            }
            catch(Exception ex){
                return false;
            }
        }
        public Response<ReviewData> GetReviewCount(int InventoryItemId)
        {
            Response<ReviewData> response = null;
            ReviewData reviewshow = new ReviewData();
            using (var context = new entity.KuazooEntities())
            {
                var entityReview = from d in context.kzReviews.Include("kzInventoryItem").Include("kzUser")
                                   where d.inventoryitem_id == InventoryItemId
                                   && d.last_action != "5"
                                   select d;
                reviewshow.Rating=0;
                reviewshow.ReviewCount=0;
                if (entityReview != null && entityReview.Count() > 0)
                {
                    reviewshow.Rating = (int)entityReview.Sum(x=>x.rating);
                    reviewshow.ReviewCount = entityReview.Count();
                }
                response = Response<ReviewData>.Create(reviewshow);
            }

            return response;
        }
        public Response<Review> GetReviewById(long ReviewId)
        {
            Response<Review> response = null;
            Review Review = new Review();
            using (var context = new entity.KuazooEntities())
            {
                var entityReview = from d in context.kzReviews.Include("kzInventoryItem").Include("kzUser")
                                   where d.id == ReviewId
                                   && d.last_action != "5"
                                   select d;
                var v = entityReview.First();
                if (v != null)
                {
                    Review.ReviewId = (int)v.id;
                    Review.InventoryItemId = (int)v.inventoryitem_id;
                    Review.InventoryItemName = v.kzInventoryItem.name;
                    Review.MemberId = (int)v.user_id;
                    Review.MemberEmail = v.kzUser.email;
                    Review.MemberFullName = v.kzUser.first_name + " " + v.kzUser.last_name;
                    if (v.kzUser.kzFacebooks != null && v.kzUser.kzFacebooks.Count() > 0)
                    {
                        Review.MemberImage = "http://graph.facebook.com/" + v.kzUser.kzFacebooks.First().facebookid + "/picture";
                    }
                    else
                    {
                        Review.MemberImage = ConfigurationManager.AppSettings["urlpath"] + "Content/img/userimg.png";
                    }
                    Review.Rating = (int)v.rating;
                    Review.Message = v.message;
                    Review.ReviewDate = (DateTime)v.review_date;
                    Review.LastAction = v.last_action;
                }
                response = Response<Review>.Create(Review);
            }

            return response;
        }
        public Response<bool> DeleteReview(long ReviewId)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                var entityUser = from d in context.kzReviews
                                 where d.id == ReviewId
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
