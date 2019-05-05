using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace com.kuazoo
{
    public class LandingService : ILandingServices
    {
        public Response<List<Banner>> GetBannerList()
        {
            Response<List<Banner>> response = null;
            List<Banner> BannerList = new List<Banner>();
            using (var context = new entity.KuazooEntities())
            {
                var entityBanner = from d in context.kzBanners
                                   orderby d.seq ascending
                                   select d;
                foreach (var v in entityBanner)
                {
                    Banner Banner = new Banner();
                    Banner.BannerId = v.id;
                    Banner.Name = v.name;
                    Banner.Link = v.link;
                    Banner.LastAction = v.last_action;
                    Banner.Seq = (int)v.seq;

                    int subimgidbanner = new int();
                    string subimgnamebanner = "";
                    string subimgurlbanner = "";
                    string subimgurllinkbanner = "";
                    var entitySubImgbanner = from d in context.kzBannerImages
                                               from e in context.kzImages
                                               where d.image_id == e.id && d.banner_id == v.id
                                               select new { d.image_id, e.url, d.image_url };
                    if (entitySubImgbanner.Count() > 0)
                    {
                        subimgidbanner = entitySubImgbanner.First().image_id;
                        subimgnamebanner = entitySubImgbanner.First().url;
                        subimgurlbanner = ConfigurationManager.AppSettings["uploadpath"] + entitySubImgbanner.First().url;
                        subimgurllinkbanner = entitySubImgbanner.First().image_url;
                    }

                    Banner.SubImageId = subimgidbanner;
                    Banner.SubImageName = subimgnamebanner;
                    Banner.SubImageUrl = subimgurlbanner;
                    Banner.SubImageUrlLink = subimgurllinkbanner;
                    BannerList.Add(Banner);
                }
                response = Response<List<Banner>>.Create(BannerList);
            }

            return response;
        }

        public Response<bool> CreateBanner(Banner Banner)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                if (Banner.BannerId != 0)
                {
                    var entityBanner = from d in context.kzBanners
                                         where d.id == Banner.BannerId
                                         select d;
                    if (entityBanner.Count() > 0)
                    {

                        entityBanner.First().name = Banner.Name;
                        entityBanner.First().link = Banner.Link;
                        entityBanner.First().last_action = "3";
                        entityBanner.First().seq = Banner.Seq;
                        context.SaveChanges();

                        if (Banner.SubImageId != null)
                        {
                            try
                            {
                                var entityImage = from d in context.kzBannerImages
                                                  where d.banner_id == Banner.BannerId
                                                  select d;
                                foreach (var v in entityImage)
                                {
                                    context.DeleteObject(v);
                                }
                                context.SaveChanges();

                                entity.kzBannerImage bnimage = new entity.kzBannerImage();
                                bnimage.banner_id = Banner.BannerId;
                                bnimage.image_id = Banner.SubImageId;
                                bnimage.image_url = Banner.SubImageUrlLink;
                                context.AddTokzBannerImages(bnimage);
                                context.SaveChanges();
                            }
                            catch
                            {

                            }
                        }
                        else
                        {
                            throw new CustomException(CustomErrorType.ImageNotFound);
                        }

                        response = Response<bool>.Create(true);
                        
                    }
                    else
                    {
                        throw new CustomException(CustomErrorType.BannerNotFound);
                    }
                }
                else
                {
                    entity.kzBanner bnentity = new entity.kzBanner();
                    bnentity.name = Banner.Name;
                    bnentity.last_action = "1";
                    bnentity.seq = Banner.Seq;
                    bnentity.link = Banner.Link;
                    context.AddTokzBanners(bnentity);
                    context.SaveChanges();
                    int BannerId = bnentity.id;
                    #region image
                    if (Banner.SubImageId != null)
                    {
                        try
                        {
                            entity.kzBannerImage bnimage = new entity.kzBannerImage();
                            bnimage.banner_id = BannerId;
                            bnimage.image_id = Banner.SubImageId;
                            bnimage.image_url = Banner.SubImageUrlLink;
                            context.AddTokzBannerImages(bnimage);
                            context.SaveChanges();                            
                        }
                        catch
                        {

                        }
                    }
                    #endregion
                    response = Response<bool>.Create(true);
                }
                
            }
            return response;
        }

        public Response<Banner> GetBannerById(int BannerId)
        {
            Response<Banner> response = null;
            Banner Banner = new Banner();
            using (var context = new entity.KuazooEntities())
            {
                var entityBanner = from d in context.kzBanners
                                   where d.id == BannerId
                                   select d;
                var v = entityBanner.First();
                if (v != null)
                {
                    Banner.BannerId = v.id;
                    Banner.Name = v.name;
                    Banner.Link = v.link;
                    Banner.LastAction = v.last_action;
                    Banner.Seq = (int)v.seq;

                    var entityImgBanner = from d in context.kzBannerImages
                                               from e in context.kzImages
                                               where d.image_id == e.id && d.banner_id == v.id
                                               select new { d.image_id, e.url, d.image_url };

                    Banner.SubImageId = entityImgBanner.First().image_id;
                    Banner.SubImageName = entityImgBanner.First().url;
                    Banner.SubImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImgBanner.First().url;
                    Banner.SubImageUrlLink = entityImgBanner.First().image_url;
                }
                response = Response<Banner>.Create(Banner);
            }

            return response;
        }

        public Response<bool> DeleteBanner(int BannerId)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                var entityUser = from d in context.kzBanners
                                 where d.id == BannerId
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

        public Response<List<BVD>> GetBVDList()
        {
            Response<List<BVD>> response = null;
            List<BVD> BVDList = new List<BVD>();
            using (var context = new entity.KuazooEntities())
            {
                var entityBVD = from d in context.kzBVDs
                                orderby d.seq ascending
                                select d;
                foreach (var v in entityBVD)
                {
                    BVD BVD = new BVD();
                    BVD.BVDId = v.id;
                    BVD.Title = v.title;
                    BVD.Link = v.link;
                    BVD.Type = (int)v.typee;
                    BVD.LastAction = v.last_action;
                    BVD.Seq = (int)v.seq;
                    BVD.UpdatedDate = v.updated_date;

                    int subimgidbvd = new int();
                    string subimgnamebvd = "";
                    string subimgurlbvd = "";
                    string subimgurllinkbvd = "";
                    var entitySubImgbvd = from d in context.kzBVDImages
                                          from e in context.kzImages
                                          where d.image_id == e.id && d.bvd_id == v.id
                                          select new { d.image_id, e.url, d.image_url };
                    if (entitySubImgbvd.Count() > 0)
                    {
                        subimgidbvd = entitySubImgbvd.First().image_id;
                        subimgnamebvd = entitySubImgbvd.First().url;
                        subimgurlbvd = ConfigurationManager.AppSettings["uploadpath"] + entitySubImgbvd.First().url;
                        subimgurllinkbvd = entitySubImgbvd.First().image_url;
                    }

                    BVD.SubImageId = subimgidbvd;
                    BVD.SubImageName = subimgnamebvd;
                    BVD.SubImageUrl = subimgurlbvd;
                    BVD.SubImageUrlLink = subimgurllinkbvd;
                    BVDList.Add(BVD);
                }
                response = Response<List<BVD>>.Create(BVDList);
            }

            return response;
        }


        public Response<bool> CreateBVD(BVD BVD)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                if (BVD.BVDId != 0)
                {
                    var entityBVD = from d in context.kzBVDs
                                    where d.id == BVD.BVDId
                                    select d;
                    if (entityBVD.Count() > 0)
                    {

                        entityBVD.First().title = BVD.Title;
                        entityBVD.First().typee = BVD.Type;
                        entityBVD.First().link = BVD.Link;
                        entityBVD.First().last_action = "3";
                        if (BVD.UpdatedDate == null)
                            entityBVD.First().updated_date = DateTime.UtcNow;
                        else
                            entityBVD.First().updated_date = BVD.UpdatedDate;
                        entityBVD.First().seq = BVD.Seq;
                        context.SaveChanges();

                        if (BVD.SubImageId != null)
                        {
                            try
                            {
                                var entityImage = from d in context.kzBVDImages
                                                  where d.bvd_id == BVD.BVDId
                                                  select d;
                                foreach (var v in entityImage)
                                {
                                    context.DeleteObject(v);
                                }
                                context.SaveChanges();

                                entity.kzBVDImage bvdimage = new entity.kzBVDImage();
                                bvdimage.bvd_id = BVD.BVDId;
                                bvdimage.image_id = BVD.SubImageId;
                                bvdimage.image_url = BVD.SubImageUrlLink;
                                context.AddTokzBVDImages(bvdimage);
                                context.SaveChanges();
                            }
                            catch
                            {

                            }
                        }
                        else
                        {
                            throw new CustomException(CustomErrorType.ImageNotFound);
                        }

                        response = Response<bool>.Create(true);

                    }
                    else
                    {
                        throw new CustomException(CustomErrorType.BVDNotFound);
                    }
                }
                else
                {
                    entity.kzBVD bvdentity = new entity.kzBVD();
                    bvdentity.title = BVD.Title;
                    bvdentity.typee = BVD.Type;
                    bvdentity.last_action = "1";
                    bvdentity.created_date = DateTime.UtcNow;
                    if (BVD.UpdatedDate == null)
                        bvdentity.updated_date = DateTime.UtcNow;
                    else
                        bvdentity.updated_date = BVD.UpdatedDate;
                    bvdentity.seq = BVD.Seq;
                    bvdentity.link = BVD.Link;
                    context.AddTokzBVDs(bvdentity);
                    context.SaveChanges();
                    int BVDId = bvdentity.id;
                    #region image
                    if (BVD.SubImageId != null)
                    {
                        try
                        {
                            entity.kzBVDImage bvdimage = new entity.kzBVDImage();
                            bvdimage.bvd_id = BVDId;
                            bvdimage.image_id = BVD.SubImageId;
                            bvdimage.image_url = BVD.SubImageUrlLink;
                            context.AddTokzBVDImages(bvdimage);
                            context.SaveChanges();
                        }
                        catch
                        {

                        }
                    }
                    #endregion
                    response = Response<bool>.Create(true);
                }

            }
            return response;
        }


        public Response<bool> DeleteBVD(int BVDId)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                var entityUser = from d in context.kzBVDs
                                 where d.id == BVDId
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


        public Response<BVD> GetBVDById(int BVDId)
        {
            Response<BVD> response = null;
            BVD BVD = new BVD();
            using (var context = new entity.KuazooEntities())
            {
                var entityBVD = from d in context.kzBVDs
                                where d.id == BVDId
                                select d;
                var v = entityBVD.First();
                if (v != null)
                {
                    BVD.BVDId = v.id;
                    BVD.Title = v.title;
                    BVD.Type = (int)v.typee;
                    BVD.Link = v.link;
                    BVD.LastAction = v.last_action;
                    BVD.Seq = (int)v.seq;
                    BVD.UpdatedDate = v.updated_date;

                    var entityImgBVD = from d in context.kzBVDImages
                                       from e in context.kzImages
                                       where d.image_id == e.id && d.bvd_id == v.id
                                       select new { d.image_id, e.url, d.image_url };

                    BVD.SubImageId = entityImgBVD.First().image_id;
                    BVD.SubImageName = entityImgBVD.First().url;
                    BVD.SubImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImgBVD.First().url;
                    BVD.SubImageUrlLink = entityImgBVD.First().image_url;
                }
                response = Response<BVD>.Create(BVD);
            }

            return response;
        }


    }
}
