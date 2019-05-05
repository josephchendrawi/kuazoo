using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace com.kuazoo
{
    public class ImageService : IImageService
    {

        public Response<bool> CreateImage(Image Image)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                if (Image.ImageId != 0)
                {
                    var entityImage = from d in context.kzImages
                                     where d.id == Image.ImageId
                                     select d;
                    if (entityImage.Count() > 0)
                    {
                        var entityImage2 = from d in context.kzImages
                                           where d.url.ToLower() == Image.Name.ToLower()
                                           && d.id != Image.ImageId
                                           select d;
                        if (entityImage2.Count() > 0)
                        {
                            throw new CustomException(CustomErrorType.ImageAlreadyAssign);
                        }
                        else
                        {
                            entityImage.First().url = Image.Name;
                            entityImage.First().last_action = "3";
                            context.SaveChanges();
                            response = Response<bool>.Create(true);
                        }
                    }
                    else
                    {
                        throw new CustomException(CustomErrorType.ImageNotFound);
                    }
                }
                else
                {
                    var entityImage = from d in context.kzImages
                                      where d.url.ToLower() == Image.Name.ToLower()
                                      select d;
                    if (entityImage.Count() > 0)
                    {
                        throw new CustomException(CustomErrorType.ImageAlreadyAssign);
                    }
                    else
                    {
                        entity.kzImage mmentity = new entity.kzImage();
                        mmentity.url = Image.Name;
                        mmentity.last_action = "1";
                        context.AddTokzImages(mmentity);
                        context.SaveChanges();
                        response = Response<bool>.Create(true);
                    }
                }
            }
            return response;
        }

        public Response<int> CreateImageId(Image Image)
        {
            Response<int> response = null;
            using (var context = new entity.KuazooEntities())
            {
                entity.kzImage mmentity = new entity.kzImage();
                mmentity.url = Image.Name;
                mmentity.last_action = "1";
                context.AddTokzImages(mmentity);
                context.SaveChanges();
                response = Response<int>.Create(mmentity.id);
            }
            return response;
        }
        public Response<List<Image>> GetImageList()
        {
            Response<List<Image>> response = null;
            List<Image> ImageList = new List<Image>();
            using (var context = new entity.KuazooEntities())
            {
                var entityImage = from d in context.kzImages
                                 select d;
                foreach (var v in entityImage)
                {
                    Image Image = new Image();
                    Image.ImageId = v.id;
                    Image.Url = ConfigurationManager.AppSettings["uploadpath"] + v.url;
                    Image.Name = v.url;
                    Image.LastAction = v.last_action;
                    ImageList.Add(Image);
                }
                response = Response<List<Image>>.Create(ImageList);
            }

            return response;
        }

        public Response<List<Image>> GetImageListDesc()
        {
            Response<List<Image>> response = null;
            List<Image> ImageList = new List<Image>();
            using (var context = new entity.KuazooEntities())
            {
                var entityImage = from d in context.kzImages
                                  orderby d.id descending
                                  select d;
                foreach (var v in entityImage)
                {
                    Image Image = new Image();
                    Image.ImageId = v.id;
                    Image.Url = ConfigurationManager.AppSettings["uploadpath"] + v.url;
                    Image.Name = v.url;
                    Image.LastAction = v.last_action;
                    ImageList.Add(Image);
                }
                response = Response<List<Image>>.Create(ImageList);
            }

            return response;
        }

        public Response<List<Image>> GetImageListDescBySkipTake(int skip, int take)
        {
            Response<List<Image>> response = null;
            List<Image> ImageList = new List<Image>();
            using (var context = new entity.KuazooEntities())
            {
                var entityImage = (from d in context.kzImages
                                  orderby d.id descending
                                  select d).Skip(skip).Take(take);
                foreach (var v in entityImage)
                {
                    Image Image = new Image();
                    Image.ImageId = v.id;
                    Image.Url = ConfigurationManager.AppSettings["uploadpath"] + v.url;
                    Image.Name = v.url;
                    Image.LastAction = v.last_action;
                    ImageList.Add(Image);
                }
                response = Response<List<Image>>.Create(ImageList);
            }

            return response;
        }
        public Response<List<Image>> GetImageListDescByImageId(string imageid)
        {
            Response<List<Image>> response = null;
            List<Image> ImageList = new List<Image>();
            using (var context = new entity.KuazooEntities())
            {
                string[] _listimage = imageid.Split(',');
                List<int> ListImageId = new List<int>();
                foreach (var v in _listimage)
                {
                    if (v != "") ListImageId.Add(int.Parse(v));
                }
                var entityImage = (from d in context.kzImages
                                   where ListImageId.Any(id => d.id.Equals(id))
                                   orderby d.id descending
                                   select d);
                foreach (var v in entityImage)
                {
                    Image Image = new Image();
                    Image.ImageId = v.id;
                    Image.Url = ConfigurationManager.AppSettings["uploadpath"] + v.url;
                    Image.Name = v.url;
                    Image.LastAction = v.last_action;
                    ImageList.Add(Image);
                }
                response = Response<List<Image>>.Create(ImageList);
            }

            return response;
        }
        public Response<Image> GetImageById(int ImageId)
        {
            Response<Image> response = null;
            Image Image = new Image();
            using (var context = new entity.KuazooEntities())
            {
                var entityImage = from d in context.kzImages
                                where d.id == ImageId
                                select d;
                var v = entityImage.First();
                if (v != null)
                {
                    Image.ImageId = v.id;
                    Image.Url = ConfigurationManager.AppSettings["uploadpath"] + v.url;
                    Image.Name = v.url;
                    Image.LastAction = v.last_action;
                }
                response = Response<Image>.Create(Image);
            }

            return response;
        }
        public Response<bool> DeleteImage(int ImageId)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                var entityUser = from d in context.kzImages
                                 where d.id == ImageId
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
