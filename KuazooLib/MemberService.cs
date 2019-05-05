using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace com.kuazoo
{
    public class MemberService : IMemberService
    {
        public Response<bool> CreateMember(Member Member)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                if (Member.MemberId != 0)
                {
                    var entityMember = from d in context.kzUsers
                                       where d.id == Member.MemberId
                                       select d;
                    if (entityMember.Count() > 0)
                    {
                        var entityMember2 = from d in context.kzUsers
                                            where d.email.ToLower() == Member.Email.ToLower()
                                              && d.id != Member.MemberId
                                            select d;
                        if (entityMember2.Count() > 0)
                        {
                            throw new CustomException(CustomErrorType.MemberAlreadyAssign);
                        }
                        else
                        {
                            entityMember.First().first_name = Member.FirstName;
                            entityMember.First().last_name = Member.LastName;
                            entityMember.First().email = Member.Email;
                            entityMember.First().gender = Member.Gender;

                            //update also shipping and billing users.
                            var entityMember3 = from d in context.kzShippingUsers
                                                where d.user_id == Member.MemberId
                                                select d;
                            entityMember3.First().gender = Member.Gender;

                            var entityMember4 = from d in context.kzBillingUsers
                                                where d.user_id == Member.MemberId
                                                select d;
                            entityMember4.First().gender = Member.Gender;

                            if (Member.DateOfBirth != DateTime.MinValue)
                            {
                                entityMember.First().dateofbirth = Member.DateOfBirth;
                            }
                            entityMember.First().last_updated = DateTime.UtcNow;
                            entityMember.First().userstatus_id = Member.MemberStatus.MemberStatusId;
                            context.SaveChanges();
                            response = Response<bool>.Create(true);
                        }
                    }
                    else
                    {
                        throw new CustomException(CustomErrorType.MemberNotFound);
                    }
                }
                else
                {
                    var entityMember = from d in context.kzUsers
                                       where d.email.ToLower() == Member.Email.ToLower()
                                       select d;
                    if (entityMember.Count() > 0)
                    {
                        throw new CustomException(CustomErrorType.MemberAlreadyAssign);
                    }
                    else
                    {
                        entity.kzUser mmentity = new entity.kzUser();
                        mmentity.first_name = Member.FirstName;
                        mmentity.last_name = Member.LastName;
                        mmentity.email = Member.Email;
                        mmentity.gender = Member.Gender;
                        mmentity.dateofbirth = Member.DateOfBirth;
                        mmentity.last_created = DateTime.UtcNow;
                        mmentity.last_updated = DateTime.UtcNow;
                        mmentity.last_login = DateTime.UtcNow;
                        mmentity.locked_status = false;
                        mmentity.userstatus_id = Member.MemberStatus.MemberStatusId;
                        string salt = Security.getSalt();
                        string passwordhash = Security.getHash(salt, Member.Password);
                        mmentity.password = passwordhash;
                        mmentity.password_salt = salt;
                        context.AddTokzUsers(mmentity);
                        context.SaveChanges();
                        response = Response<bool>.Create(true);
                    }
                }
            }
            return response;
        }
        public Response<List<Member>> GetMemberList()
        {
            Response<List<Member>> response = null;
            List<Member> MemberList = new List<Member>();
            using (var context = new entity.KuazooEntities())
            {
                var entityMember = (from u in context.kzUsers.Include("kzUserStatu")
                                    join f in context.kzFacebooks on u.id equals f.user_id
                                    into a
                                    from b in a.DefaultIfEmpty()
                                    orderby u.last_created descending
                                    select new
                                    {
                                        u.id,
                                        u.first_name,
                                        u.last_name,
                                        u.email,
                                        u.gender,
                                        u.dateofbirth,
                                        u.locked_status,
                                        u.last_lockout,
                                        u.userstatus_id,
                                        u.kzUserStatu.name,
                                        b.user_id,
                                        u.last_created
                                    }).Distinct().ToList();
                foreach (var v in entityMember)
                {
                    Member Member = new Member();
                    Member.MemberId = v.id;
                    Member.FirstName = v.first_name;
                    Member.LastName = v.last_name;
                    Member.Email = v.email;
                    Member.Gender = (int)v.gender;
                    if (v.dateofbirth != null)
                    {
                        Member.DateOfBirth = (DateTime)v.dateofbirth;
                    }
                    Member.LastLockout = (bool)v.locked_status;
                    if (v.last_lockout != null)
                    {
                        Member.LastLockoutDate = (DateTime)v.last_lockout;
                    }
                    MemberStatus st = new MemberStatus();
                    st.MemberStatusId = (int)v.userstatus_id;
                    st.MemberStatusName = v.name;
                    Member.MemberStatus = st;
                    Facebook fb = new Facebook();
                    fb.FacebookId = Convert.ToInt32(v.user_id);
                    Member.Facebook = fb;
                    Member.Create = (DateTime)v.last_created;
                    MemberList.Add(Member);
                }
                response = Response<List<Member>>.Create(MemberList);
            }

            return response;
        }
        public Response<Member> GetMemberById(int MemberId)
        {
            Response<Member> response = null;
            Member Member = new Member();
            using (var context = new entity.KuazooEntities())
            {
                var entityMember = from d in context.kzUsers.Include("kzUserStatu")
                                    where d.id == MemberId
                                    select d;
                var v = entityMember.First();
                if (v != null)
                {
                    Member.MemberId = v.id;
                    Member.FirstName = v.first_name;
                    Member.LastName = v.last_name;
                    Member.Email = v.email;
                    Member.Gender = (int)v.gender;
                    if (v.dateofbirth != null)
                    {
                        Member.DateOfBirth = (DateTime)v.dateofbirth;
                    }
                    Member.LastLockout = (bool)v.locked_status;
                    if (v.last_lockout != null)
                    {
                        Member.LastLockoutDate = (DateTime)v.last_lockout;
                    }
                    MemberStatus st = new MemberStatus();
                    st.MemberStatusId = (int)v.userstatus_id;
                    st.MemberStatusName = v.kzUserStatu.name;
                    Member.MemberStatus = st;
                }
                response = Response<Member>.Create(Member);
            }

            return response;
        }

        public Response<List<Member>> GetMemberListIdAge(int MemberStatusId, int StartAge, int EndAge)
        {
            Response<List<Member>> response = null;
            List<Member> MemberList = new List<Member>();
            using (var context = new entity.KuazooEntities())
            {
                DateTime now = DateTime.Now;

                var entityMember = from d in context.kzUsers.Include("kzUserStatu")
                                   where System.Data.Objects.EntityFunctions.DiffYears(d.dateofbirth, now) >= StartAge
                                   && System.Data.Objects.EntityFunctions.DiffYears(d.dateofbirth, now) <= EndAge
                                   select d;
                if (MemberStatusId != 0)
                {
                    entityMember = entityMember.Where(x => x.userstatus_id == MemberStatusId);
                }
                foreach (var v in entityMember)
                {
                    Member Member = new Member();
                    Member.MemberId = v.id;
                    Member.FirstName = v.first_name;
                    Member.LastName = v.last_name;
                    Member.Email = v.email;
                    Member.Gender = (int)v.gender;
                    if (v.dateofbirth != null)
                    {
                        Member.DateOfBirth = (DateTime)v.dateofbirth;
                    }
                    Member.LastLockout = (bool)v.locked_status;
                    if (v.last_lockout != null)
                    {
                        Member.LastLockoutDate = (DateTime)v.last_lockout;
                    }
                    MemberStatus st = new MemberStatus();
                    st.MemberStatusId = (int)v.userstatus_id;
                    st.MemberStatusName = v.kzUserStatu.name;
                    Member.MemberStatus = st;
                    MemberList.Add(Member);
                }
                response = Response<List<Member>>.Create(MemberList);
            }

            return response;
        }

        public Response<bool> ChangeMemberStatus(int MemberId, int MemberStatus)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                var entityUser = from d in context.kzUsers
                                 where d.id == MemberId
                                 select d;
                if (entityUser.Count() > 0)
                {
                    entityUser.First().userstatus_id = MemberStatus;
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

        public Response<List<MemberStatus>> GetMemberStatusList()
        {
            Response<List<MemberStatus>> response = null;
            List<MemberStatus> MemberList = new List<MemberStatus>();
            using (var context = new entity.KuazooEntities())
            {
                var entityMember = from d in context.kzUserStatus
                                   where d.last_action != "5"
                                   select d;
                foreach (var v in entityMember)
                {
                    MemberStatus Member = new MemberStatus();
                    Member.MemberStatusId = v.id;
                    Member.MemberStatusName = v.name;
                    MemberList.Add(Member);
                }
                response = Response<List<MemberStatus>>.Create(MemberList);
            }

            return response;
        }


        public Response<List<Subscribe>> GetSubscribeList()
        {
            Response<List<Subscribe>> response = null;
            List<Subscribe> MemberList = new List<Subscribe>();
            using (var context = new entity.KuazooEntities())
            {
                var entityRegisteruser = (from d in context.kzUsers
                                          select new { d.email, d.last_created }).Distinct();
                var registeruserlist = entityRegisteruser.Select(x=>x.email).ToList();
                var entityMember = (from d in context.kzSubscribeEmails
                                    where !registeruserlist.Contains(d.email)
                                    orderby d.created_date descending
                                    select new { d.email, d.created_date }).Distinct();
                foreach (var v in entityMember)
                {
                    if (MemberList.Where(x => x.Email == v.email).Count() == 0)
                    {
                        MemberList.Add(new Subscribe() { Email = v.email, SubscribeDate = (DateTime)v.created_date });
                    }
                }
                foreach (var v in entityRegisteruser)
                {
                    if (MemberList.Where(x => x.Email == v.email).Count() == 0)
                    {
                        MemberList.Add(new Subscribe() { Email = v.email, SubscribeDate = (DateTime)v.last_created });
                    }
                }
                response = Response<List<Subscribe>>.Create(MemberList);
            }

            return response;
        }
    }
}
