using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.kuazoo
{
    public class TagService : ITagService
    {

        public Response<bool> CreateTag(Tag Tag)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                if (Tag.TagId != 0)
                {
                    var entityTag = from d in context.kzTags
                                     where d.id == Tag.TagId
                                     select d;
                    if (entityTag.Count() > 0)
                    {
                        var entityTag2 = from d in context.kzTags
                                          where d.name.ToLower() == Tag.Name.ToLower()
                                          && d.id != Tag.TagId
                                          select d;
                        if (entityTag2.Count() > 0)
                        {
                            throw new CustomException(CustomErrorType.TagAlreadyAssign);
                        }
                        else
                        {
                            entityTag.First().name = Tag.Name;
                            entityTag.First().showAsCategory = Tag.ShowAsCategory;
                            if (Tag.Parent != null)
                            {
                                entityTag.First().parent_id = Tag.Parent.TagId;
                            }
                            else
                            {
                                entityTag.First().parent_id = null;
                            }
                            entityTag.First().last_updated = DateTime.UtcNow;
                            entityTag.First().last_action = "3";
                            context.SaveChanges();
                            response = Response<bool>.Create(true);
                        }
                    }
                    else
                    {
                        throw new CustomException(CustomErrorType.TagNotFound);
                    }
                }
                else
                {
                    var entityTag = from d in context.kzTags
                                     where d.name.ToLower() == Tag.Name.ToLower()
                                     select d;
                    if (entityTag.Count() > 0)
                    {
                        throw new CustomException(CustomErrorType.TagAlreadyAssign);
                    }
                    else
                    {
                        entity.kzTag mmentity = new entity.kzTag();
                        mmentity.name = Tag.Name;
                        mmentity.showAsCategory = Tag.ShowAsCategory;
                        if (Tag.Parent != null)
                        {
                            mmentity.parent_id = Tag.Parent.TagId;
                        }
                        mmentity.last_created = DateTime.UtcNow;
                        mmentity.last_updated = DateTime.UtcNow;
                        mmentity.last_action = "1";
                        context.AddTokzTags(mmentity);
                        context.SaveChanges();
                        response = Response<bool>.Create(true);
                    }
                }
            }
            return response;
        }
        public Response<List<Tag>> GetTagList()
        {
            Response<List<Tag>> response = null;
            List<Tag> TagList = new List<Tag>();
            using (var context = new entity.KuazooEntities())
            {
                var entityTag = from d in context.kzTags
                                orderby d.last_created descending
                                 select d;
                foreach (var v in entityTag)
                {
                    Tag Tag = new Tag();
                    Tag.TagId = v.id;
                    Tag.Name = v.name;
                    if (v.showAsCategory == null)
                    {
                        Tag.ShowAsCategory = false;
                    }
                    else
                    {
                        Tag.ShowAsCategory = (bool)v.showAsCategory;
                    }
                    if (v.parent_id != null)
                    {
                        Tag parent = new Tag();
                        parent.TagId = (int)v.parent_id;
                        parent.Name = v.kzTag1.name;
                        Tag.Parent = parent;
                    }
                    Tag.LastAction = v.last_action;
                    Tag.Create = (DateTime)v.last_created;
                    TagList.Add(Tag);
                }
                response = Response<List<Tag>>.Create(TagList);
            }

            return response;
        }
        public Response<List<Tag>> GetTagListActive()
        {
            Response<List<Tag>> response = null;
            List<Tag> TagList = new List<Tag>();
            using (var context = new entity.KuazooEntities())
            {
                var entityTag = from d in context.kzTags
                                where d.last_action !="5" && d.showAsCategory.HasValue && d.showAsCategory==true
                                orderby d.name
                                select d;
                foreach (var v in entityTag)
                {
                    Tag Tag = new Tag();
                    Tag.TagId = v.id;
                    Tag.Name = v.name;
                    if (v.showAsCategory == null)
                    {
                        Tag.ShowAsCategory = false;
                    }
                    else
                    {
                        Tag.ShowAsCategory = (bool)v.showAsCategory;
                    }
                    if (v.parent_id != null)
                    {
                        Tag parent = new Tag();
                        parent.TagId = (int)v.parent_id;
                        parent.Name = v.kzTag1.name;
                        Tag.Parent = parent;
                    }
                    TagList.Add(Tag);
                }
                response = Response<List<Tag>>.Create(TagList);
            }

            return response;
        }
        public Response<Tag> GetTagById(int TagId)
        {
            Response<Tag> response = null;
            Tag Tag = new Tag();
            using (var context = new entity.KuazooEntities())
            {
                var entityTag = from d in context.kzTags
                                where d.id == TagId
                                select d;
                var v = entityTag.First();
                if (v != null)
                {
                    Tag.TagId = v.id;
                    Tag.Name = v.name;
                    if (v.showAsCategory == null)
                    {
                        Tag.ShowAsCategory = false;
                    }
                    else
                    {
                        Tag.ShowAsCategory = (bool)v.showAsCategory;
                    }
                    if (v.parent_id != null)
                    {
                        Tag parent = new Tag();
                        parent.TagId = (int)v.parent_id;
                        Tag.Parent = parent;
                    }
                    Tag.LastAction = v.last_action;
                }
                response = Response<Tag>.Create(Tag);
            }

            return response;
        }
        public Response<bool> DeleteTag(int TagId)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                var entityUser = from d in context.kzTags
                                 where d.id == TagId
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
