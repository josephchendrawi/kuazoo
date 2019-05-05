using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Configuration;

namespace com.kuazoo
{
    public class UserService : IUserService
    {
        public Response<bool> CreateUser(User User)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                if (User.UserId != 0)
                {
                    var entityUser = from d in context.kzUsers
                                     where d.id == User.UserId
                                     select d;
                    if (entityUser.Count() > 0)
                    {
                        var entityUser2 = from d in context.kzUsers
                                          where d.email.ToLower() == User.Email.ToLower()
                                          && d.id != User.UserId
                                          select d;
                        if (entityUser2.Count() > 0)
                        {
                            throw new CustomException(CustomErrorType.UserServiceAlreadyAssign);
                        }
                        else
                        {
                            entityUser.First().first_name = User.FirstName;
                            entityUser.First().last_name =  User.LastName;
                            entityUser.First().gender = User.Gender;
                            entityUser.First().last_updated = DateTime.UtcNow;
                            entityUser.First().userstatus_id = User.UserStatus;
                            context.SaveChanges();
                            response = Response<bool>.Create(true);
                        }
                    }
                    else
                    {
                        throw new CustomException(CustomErrorType.UserServiceNotFound);
                    }
                }
                else
                {
                    var entityUser = from d in context.kzUsers
                                     where d.email.ToLower() == User.Email.ToLower()
                                     select d;
                    if (entityUser.Count() > 0)
                    {
                        throw new CustomException(CustomErrorType.UserServiceAlreadyAssign);
                    }
                    else
                    {



                        entity.kzUser mmentity = new entity.kzUser();
                        mmentity.first_name = User.FirstName;
                        mmentity.last_name =  User.LastName;
                        mmentity.email = User.Email;
                        mmentity.gender = User.Gender;
                        mmentity.last_updated = DateTime.UtcNow;
                        mmentity.last_created = DateTime.UtcNow;
                        mmentity.locked_status = false;
                        mmentity.userstatus_id = User.UserStatus;
                        //string key = Security.RandomString(60);
                        string key1 = "ku4zo0";
                        string key = Security.checkHMAC(key1, User.Email.ToLower());
                        string pass = Security.checkHMAC(key, User.Password);
                        mmentity.password = pass;
                        mmentity.password_salt = key;
                        mmentity.notif = true;
                        context.AddTokzUsers(mmentity);
                        context.SaveChanges();

                        //if (User.Verify)
                        //{
                        //    string token1 = Security.Encrypt("verify", mmentity.id.ToString());
                        //    string token2 = Security.Encrypt("verifyagain", token1);

                        //    VerifyEmail(User.Email, User.FirstName + " " + User.LastName, token2);
                        //}
                        response = Response<bool>.Create(true);
                    }
                }
            }
            return response;
        }
        public Response<bool> UpdateUser(User User)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                if (User.UserId != 0)
                {
                    var entityUser = from d in context.kzUsers
                                     where d.id == User.UserId
                                     select d;
                    if (entityUser.Count() > 0)
                    {
                        entityUser.First().first_name = User.FirstName;
                        entityUser.First().last_name =  User.LastName;
                        entityUser.First().gender = User.Gender;
                        if (User.DateOfBirth != null && User.DateOfBirth != DateTime.MinValue)
                        {
                            entityUser.First().dateofbirth = User.DateOfBirth;
                        }
                        if (User.ImageId != null && User.ImageId != 0)
                        {
                            entityUser.First().image_id = User.ImageId;
                        }
                        entityUser.First().last_updated = DateTime.UtcNow;
                        entityUser.First().userstatus_id = User.UserStatus;
                        entityUser.First().notif = User.Notif;
                        context.SaveChanges();
                        response = Response<bool>.Create(true);
                    }
                    else
                    {
                        throw new CustomException(CustomErrorType.UserServiceNotFound);
                    }
                }
                else
                {
                    throw new CustomException(CustomErrorType.UserServiceNotFound);
                }
            }
            return response;
        }
        public Response<bool> CheckUserEmail(string email)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                var user = GetCurrentUser().Result;
                if (email != user.Email)
                {
                    var entityUser = from d in context.kzUsers
                                     where d.email.ToLower() == email.ToLower()
                                     select d;
                    if (entityUser.Count() > 0)
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
        public Response<bool> UserVerifying(string token)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                try
                {
                    string decode1 = Security.Decrypt("verifyagain", token);
                    string decode2 = Security.Decrypt("verify", decode1);

                    int id = int.Parse(decode2);
                    var entityUser = from d in context.kzUsers
                                     where d.id == id && (d.userstatus_id==4 || d.userstatus_id==5)
                                     select d;
                    if (entityUser.Count() > 0)
                    {
                        entityUser.First().userstatus_id = 1;
                        context.SaveChanges();
                        response = Response<bool>.Create(true);
                    }
                    else
                    {
                        throw new CustomException(CustomErrorType.UserServiceNotFound);
                    }
                }
                catch (Exception ex)
                {
                    throw new CustomException(CustomErrorType.UserServiceNotFound);
                }
            }
            return response;
        }

        public Response<int> CreateUserUsingFacebook(FbUser fbUser)
        {
            Response<int> response;
            using (var context = new entity.KuazooEntities())
            {
                int userid= CreateUserWithFbAcc(context,fbUser);
                // check facebook ID exists
                if (context.kzFacebooks.Where(e => e.facebookid.Equals(fbUser.FacebookId) && e.user_id==userid).Count() > 0)
                {
                    UpdateUserFBToken(context, fbUser.AccessToken, userid);
                }
                else
                {
                    CreateFbAccount(context, userid, fbUser);
                }
                response = Response<int>.Create(userid);
            }
            return response;
        }

        private int CreateUserWithFbAcc(entity.KuazooEntities context, FbUser fbUser)
        {
            int userid = 0;
            var entityUser = from d in context.kzUsers
                             where d.email.ToLower() == fbUser.FacebookEmail.ToLower()
                             select d;
            if (entityUser.Count() > 0)
            {
                userid = entityUser.First().id;
            }
            else
            {
                entity.kzUser mmentity = new entity.kzUser();
                mmentity.first_name = fbUser.FacebookName;
                mmentity.last_name = "";
                mmentity.email = fbUser.FacebookEmail;
                int gender = 0;
                if (fbUser.FacebookGender.ToLower() == "male") gender = 1;
                else gender = 2;
                mmentity.gender = gender;
                try
                {
                    string[] dob = fbUser.FaceBookDob.ToString().Split('/');
                    DateTime birth = new DateTime(Convert.ToInt16(dob[2]), Convert.ToInt16(dob[0]), Convert.ToInt16(dob[1]));
                    mmentity.dateofbirth = birth;
                }
                catch{
                }
                mmentity.last_updated = DateTime.UtcNow;
                mmentity.last_created = DateTime.UtcNow;
                mmentity.locked_status = false;
                mmentity.userstatus_id = 1;
                string key = Security.RandomString(60);
                string pass = Security.checkHMAC(key, fbUser.FacebookId);
                mmentity.password = pass;
                mmentity.password_salt = key;
                mmentity.notif = true;
                context.AddTokzUsers(mmentity);
                context.SaveChanges();
                userid = mmentity.id;
            }
            return userid;
        }
        private void CreateFbAccount(entity.KuazooEntities context, int userid, FbUser fbuser)
        {
            // create the facebook account
            entity.kzFacebook facebookAccount = new entity.kzFacebook();
            facebookAccount.facebookid = fbuser.FacebookId;
            facebookAccount.accesstoken = fbuser.AccessToken;
            facebookAccount.user_id = userid;
            facebookAccount.link_date = DateTime.UtcNow;
            context.AddTokzFacebooks(facebookAccount);
            context.SaveChanges();
        }
        public void UpdateUserFBToken(entity.KuazooEntities context, string newToken, int userid)
        {
            var userEntities = from e in context.kzUsers
                                where e.id == userid
                                select e;
            var userEntity = userEntities.FirstOrDefault();

            // facebook id
            var entityFacebookAccount = userEntity.kzFacebooks.FirstOrDefault();
            if (entityFacebookAccount != null)
            {
                entityFacebookAccount.accesstoken = newToken;
                context.SaveChanges();
            }
        }
        public Response<bool> CheckLogin(User user)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
               
                var entityUser = from d in context.kzUsers
                                 where d.email.ToLower() == user.Email.ToLower()
                                 && d.locked_status == false
                                 && (d.userstatus_id == 1 || d.userstatus_id == 4 || d.userstatus_id==5)
                                 select d;
                if (entityUser.Count() > 0)
                {
                    string key = entityUser.First().password_salt;
                    string pass = entityUser.First().password;
                    string passcek = Security.checkHMAC(key, user.Password);
                    if (pass == passcek)
                    {
                        entityUser.First().last_login = DateTime.UtcNow;
                        context.SaveChanges();
                        response = Response<bool>.Create(true);
                    }
                    else
                    {
                        response = Response<bool>.Create(false);
                    }
                }
                else
                {
                    throw new CustomException(CustomErrorType.UserServiceEmailPasswordWrong);
                }
            }
            return response;
        }

        public Response<bool> ChangePass(int userid, string oldpass, string newpass)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {

                var entityUser = from d in context.kzUsers
                                 where d.id == userid
                                 && d.locked_status == false
                                 && (d.userstatus_id == 1 || d.userstatus_id == 4 || d.userstatus_id == 5)
                                 select d;
                if (entityUser.Count() > 0)
                {
                    string key = entityUser.First().password_salt;
                    string pass = entityUser.First().password;
                    string passcek = Security.checkHMAC(key, oldpass);
                    if (pass == passcek)
                    {
                        string newpasssalt = Security.checkHMAC(key, newpass);
                        entityUser.First().password = newpasssalt;
                        context.SaveChanges();
                        response = Response<bool>.Create(true);
                    }
                    else
                    {
                        throw new CustomException(CustomErrorType.UserServicePasswordWrong);
                    }
                }
                else
                {
                    throw new CustomException(CustomErrorType.UserServiceNotFound);
                }
            }
            return response;
        }
        public Response<bool> ResendEmail(int userid)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {

                var entityUser = from d in context.kzUsers
                                 where d.id == userid
                                 && d.locked_status == false
                                 && (d.userstatus_id == 4 || d.userstatus_id == 5)
                                 select d;
                if (entityUser.Count() > 0)
                {

                    string token1 = Security.Encrypt("verify", userid.ToString());
                    string token2 = Security.Encrypt("verifyagain", token1);

                    VerifyEmail(entityUser.First().email, entityUser.First().first_name, token2);
                    response = Response<bool>.Create(true);
                }
                else
                {
                    response = Response<bool>.Create(false);
                }
            }
            return response;
        }
        public Response<User> GetCurrentUser()
        {
            try
            {
                Response<User> response = null;
                using (var context = new entity.KuazooEntities())
                {
                    string currentUserName = Thread.CurrentPrincipal.Identity.Name;
                    if (currentUserName != "Guest")
                    {
                        var entityUser = from d in context.kzUsers
                                         where d.email.ToLower() == currentUserName.ToLower()
                                         && d.locked_status == false
                                         && (d.userstatus_id == 1 || d.userstatus_id == 4 || d.userstatus_id == 5)
                                         select d;
                        User us = new User();
                        us.UserId = entityUser.First().id;
                        us.FirstName = entityUser.First().first_name;
                        us.LastName =  entityUser.First().last_name;
                        us.Gender = (int)entityUser.First().gender;
                        us.Email = entityUser.First().email;
                        if (entityUser.First().dateofbirth != null)
                        {
                            us.DateOfBirth = (DateTime)entityUser.First().dateofbirth;
                        }
                        else
                        {
                            us.DateOfBirth = new DateTime(1901, 1, 1);
                        }
                        us.FacebookId = "";
                        us.Facebook = false;
                        us.Twitter = false;
                        if (entityUser.First().kzFacebooks != null && entityUser.First().kzFacebooks.Count() > 0)
                        {
                            us.FacebookId = entityUser.First().kzFacebooks.First().facebookid;
                            us.Facebook = true;
                        }
                        us.ImageId =0;
                        us.ImageName = "";
                        us.ImageUrl = "";
                        if (entityUser.First().image_id != null)
                        {
                            us.ImageId = (int)entityUser.First().image_id;
                            us.ImageName = entityUser.First().kzImage.url;
                            us.ImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityUser.First().kzImage.url;
                        }
                        us.KPoint = PointService.GetPointBalance(us.UserId).Result;
                        us.Notif = (bool)entityUser.First().notif;
                        us.UserStatus = (int)entityUser.First().userstatus_id;
                        response = Response<User>.Create(us); ;
                    }
                    else
                    {
                        User us = new User();
                        us.UserId = 0;
                        us.FirstName = "Guest";
                        us.LastName = "";
                        us.Gender = 1;
                        us.Email = "";
                        us.FacebookId = "";
                        us.ImageId = 0;
                        us.ImageName = "";
                        us.ImageUrl = "";
                        us.KPoint = 0;
                        us.Notif = false;
                        us.Facebook = false;
                        us.Twitter=false;
                        response = Response<User>.Create(us); ;
                    }
                }
                return response;
            }
            catch (Exception ex)
            {
                throw new CustomException(CustomErrorType.Unauthenticated);
            }
        }
        public static Response<User> GetCurrentUserStatic()
        {
            try
            {
                Response<User> response = null;
                using (var context = new entity.KuazooEntities())
                {
                    string currentUserName = Thread.CurrentPrincipal.Identity.Name;
                    if (currentUserName != "Guest")
                    {
                        var entityUser = from d in context.kzUsers
                                         where d.email.ToLower() == currentUserName.ToLower()
                                         && d.locked_status == false
                                         && (d.userstatus_id == 1 || d.userstatus_id == 4 || d.userstatus_id == 5)
                                         select d;
                        User us = new User();
                        us.UserId = entityUser.First().id;
                        us.FirstName = entityUser.First().first_name;
                        us.LastName = entityUser.First().last_name;
                        us.Gender = (int)entityUser.First().gender;
                        us.Email = entityUser.First().email;
                        if (entityUser.First().dateofbirth != null)
                        {
                            us.DateOfBirth = (DateTime)entityUser.First().dateofbirth;
                        }
                        else
                        {
                            us.DateOfBirth = new DateTime(1901, 1, 1);
                        }
                        us.FacebookId = "";
                        us.Facebook = false;
                        us.Twitter = false;
                        if (entityUser.First().kzFacebooks != null && entityUser.First().kzFacebooks.Count() > 0)
                        {
                            us.FacebookId = entityUser.First().kzFacebooks.First().facebookid;
                            us.Facebook = true;
                        }
                        us.ImageId = 0;
                        us.ImageName = "";
                        us.ImageUrl = "";
                        if (entityUser.First().image_id != null)
                        {
                            us.ImageId = (int)entityUser.First().image_id;
                            us.ImageName = entityUser.First().kzImage.url;
                            us.ImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityUser.First().kzImage.url;
                        }
                        us.KPoint = PointService.GetPointBalance(us.UserId).Result;
                        us.Notif = (bool)entityUser.First().notif;
                        us.UserStatus = (int)entityUser.First().userstatus_id;
                        response = Response<User>.Create(us); ;
                    }
                    else
                    {
                        User us = new User();
                        us.UserId = 0;
                        us.FirstName = "Guest";
                        us.LastName = "";
                        us.Gender = 1;
                        us.Email = "";
                        us.FacebookId = "";
                        us.ImageId = 0;
                        us.ImageName = "";
                        us.ImageUrl = "";
                        us.KPoint = 0;
                        us.Notif = false;
                        us.Facebook = false;
                        us.Twitter = false;
                        response = Response<User>.Create(us); ;
                    }
                }
                return response;
            }
            catch
            {
                User us = new User();
                us.UserId = 0;
                return Response<User>.Create(us); ;
            }
        }
        public Response<User> GetCurrentUserWithoutTrace()
        {
            try
            {
                Response<User> response = null;
                using (var context = new entity.KuazooEntities())
                {
                    string currentUserName = Thread.CurrentPrincipal.Identity.Name;
                    if (currentUserName != "Guest")
                    {
                        var entityUser = from d in context.kzUsers
                                         where d.email.ToLower() == currentUserName.ToLower()
                                         && d.locked_status == false
                                         && (d.userstatus_id == 1 || d.userstatus_id == 4 || d.userstatus_id == 5)
                                         select d;
                        User us = new User();
                        us.UserId = entityUser.First().id;
                        us.FirstName = entityUser.First().first_name;
                        us.LastName = entityUser.First().last_name;
                        us.Gender = (int)entityUser.First().gender;
                        us.Email = entityUser.First().email;
                        if (entityUser.First().dateofbirth != null)
                        {
                            us.DateOfBirth = (DateTime)entityUser.First().dateofbirth;
                        }
                        us.FacebookId = "";
                        us.Facebook = false;
                        us.Twitter = false;
                        if (entityUser.First().kzFacebooks != null && entityUser.First().kzFacebooks.Count() > 0)
                        {
                            us.FacebookId = entityUser.First().kzFacebooks.First().facebookid;
                            us.Facebook = true;
                        }
                        us.ImageId = 0;
                        us.ImageName = "";
                        us.ImageUrl = "";
                        if (entityUser.First().image_id != null)
                        {
                            us.ImageId = (int)entityUser.First().image_id;
                            us.ImageName = entityUser.First().kzImage.url;
                            us.ImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityUser.First().kzImage.url;
                        }
                        us.KPoint = PointService.GetPointBalance(us.UserId).Result;
                        us.Notif = (bool)entityUser.First().notif;
                        us.UserStatus = (int)entityUser.First().userstatus_id;
                        response = Response<User>.Create(us); ;
                    }
                    else
                    {
                        User us = new User();
                        us.UserId = 0;
                        us.FirstName = "Guest";
                        us.LastName = "";
                        us.Gender = 1;
                        us.Email = "";
                        us.FacebookId = "";
                        us.ImageId = 0;
                        us.ImageName = "";
                        us.ImageUrl = "";
                        us.KPoint = 0;
                        us.Notif = false;
                        us.Facebook = false;
                        us.Twitter = false;
                        response = Response<User>.Create(us); ;
                    }
                }
                return response;
            }
            catch (Exception ex)
            {
                User us = new User();
                us.UserId = 0;
                us.FirstName = "Guest";
                us.LastName = "";
                us.Gender = 1;
                us.Email = "";
                us.FacebookId = "";
                us.ImageId = 0;
                us.ImageName = "";
                us.ImageUrl = "";
                us.KPoint = 0;
                us.Notif = false;
                us.Facebook = false;
                us.Twitter = false;
                return Response<User>.Create(us); ;
            }
        }
        public Response<int> GetCurrentUserPending()
        {
            try
            {
                Response<int> response = null;
                using (var context = new entity.KuazooEntities())
                {
                    string currentUserName = Thread.CurrentPrincipal.Identity.Name;
                    if (currentUserName != "Guest")
                    {
                        var entityUser = from d in context.kzUsers
                                         where d.email.ToLower() == currentUserName.ToLower()
                                         && d.locked_status == false
                                         && (d.userstatus_id == 1 || d.userstatus_id == 4 || d.userstatus_id == 5)
                                         select d;
                        int userstatus = (int)entityUser.First().userstatus_id;
                        response = Response<int>.Create(userstatus);
                    }
                    else
                    {
                        response = Response<int>.Create(5);
                    }
                }
                return response;
            }
            catch (Exception ex)
            {
                return Response<int>.Create(0);
            }
        }
        public static Response<bool> NotGuest()
        {
            Response<bool> response = null;
            try
            {
                using (var context = new entity.KuazooEntities())
                {
                    string currentUserName = Thread.CurrentPrincipal.Identity.Name;
                    if (currentUserName != "Guest")
                    {
                        response = Response<bool>.Create(true);
                    }
                    else
                    {
                        response = Response<bool>.Create(false);
                    }
                }
            }
            catch (Exception ex)
            {
                response = Response<bool>.Create(false);
            }
            return response;
        }
        public static Response<bool> ReviewCheckDealPurchase(int InventoryItemId)
        {
            Response<bool> response = null;
            try
            {
                using (var context = new entity.KuazooEntities())
                {
                    int UserId = 0;

                    string currentUserName = Thread.CurrentPrincipal.Identity.Name;
                    if (currentUserName != "Guest")
                    {
                        var entityUser = from d in context.kzUsers
                                         where d.email.ToLower() == currentUserName.ToLower()
                                         && d.locked_status == false
                                         && (d.userstatus_id == 1 || d.userstatus_id == 4 || d.userstatus_id == 5)
                                         select d;
                        UserId = entityUser.First().id;
                    }
                    if (UserId != 0)
                    {
                        Boolean reviewed = ReviewService.GetReviewByInventoryItemByUser(context,UserId,InventoryItemId);

                        if (reviewed == false)
                        {
                            var entityTransaction = from d in context.kzTransactionDetails
                                                    where d.inventoryitem_id == InventoryItemId
                                                    && d.kzTransaction.user_id == UserId
                                                    && d.kzTransaction.last_action != "5"
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
                    else
                    {
                        response = Response<bool>.Create(false);
                    }
                }
            }
            catch
            {
                response = Response<bool>.Create(false);
            }
            return response;
        }
        public Response<User> GetCurrentUserByEmail(string email)
        {
            try
            {
                Response<User> response = null;
                using (var context = new entity.KuazooEntities())
                {
                    var entityUser = from d in context.kzUsers
                                        where d.email.ToLower() == email.ToLower()
                                        && d.locked_status == false
                                        && (d.userstatus_id == 1 || d.userstatus_id == 4 || d.userstatus_id == 5)
                                        select d;
                    User us = new User();
                    us.UserId = entityUser.First().id;
                    us.FirstName = entityUser.First().first_name;
                    us.LastName =  entityUser.First().last_name;
                    us.Gender = (int)entityUser.First().gender;
                    us.Email = entityUser.First().email;
                    if (entityUser.First().dateofbirth != null)
                    {
                        us.DateOfBirth = (DateTime)entityUser.First().dateofbirth;
                    }
                    us.FacebookId = "";
                    us.Facebook = false;
                    us.Twitter = false;
                    if (entityUser.First().kzFacebooks != null && entityUser.First().kzFacebooks.Count() > 0)
                    {
                        us.FacebookId = entityUser.First().kzFacebooks.First().facebookid;
                        us.Facebook = true;
                    }

                    us.ImageId = 0;
                    us.ImageName = "";
                    us.ImageUrl = "";
                    if (entityUser.First().image_id != null)
                    {
                        us.ImageId = (int)entityUser.First().image_id;
                        us.ImageName = entityUser.First().kzImage.url;
                        us.ImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityUser.First().kzImage.url;
                    }
                    us.KPoint = PointService.GetPointBalance(us.UserId).Result;
                    us.Notif = (bool)entityUser.First().notif;
                    response = Response<User>.Create(us);
                }
                return response;
            }
            catch (Exception ex)
            {
                throw new CustomException(CustomErrorType.Unauthenticated);
            }
        }
        public Response<User> GetCurrentUserById(int userid)
        {
            try
            {
                Response<User> response = null;
                using (var context = new entity.KuazooEntities())
                {
                    var entityUser = from d in context.kzUsers
                                     where d.id == userid
                                     select d;
                    User us = new User();
                    us.UserId = entityUser.First().id;
                    us.FirstName = entityUser.First().first_name;
                    us.LastName =  entityUser.First().last_name;
                    us.Gender = (int)entityUser.First().gender;
                    us.Email = entityUser.First().email;
                    if (entityUser.First().dateofbirth != null)
                    {
                        us.DateOfBirth = (DateTime)entityUser.First().dateofbirth;
                    }

                    us.ImageId = 0;
                    us.ImageName = "";
                    us.ImageUrl = "";
                    if (entityUser.First().image_id != null)
                    {
                        us.ImageId = (int)entityUser.First().image_id;
                        us.ImageName = entityUser.First().kzImage.url;
                        us.ImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityUser.First().kzImage.url;
                    }
                    us.KPoint = PointService.GetPointBalance(us.UserId).Result;
                    us.Notif = (bool)entityUser.First().notif;
                    us.Facebook = false;
                    us.Twitter = false;
                    if (entityUser.First().kzFacebooks != null && entityUser.First().kzFacebooks.Count() > 0)
                    {
                        us.Facebook = true;
                    }
                    response = Response<User>.Create(us); ;
                }
                return response;
            }
            catch (Exception ex)
            {
                throw new CustomException(CustomErrorType.Unauthenticated);
            }
        }
        public static String GetUserNameById(int userid)
        {
            string result = "";   
            using (var context = new entity.KuazooEntities())
            {
                var entityUser = from d in context.kzUsers
                                    where d.id == userid
                                    select d;
                if (entityUser.Count() > 0)
                {
                    result = entityUser.First().first_name +" " + entityUser.First().last_name;
                }
            }
            return result;
        }

        public Response<int> ForgotPassword(String Email)
        {
            Response<int> response = null;
            using (var context = new entity.KuazooEntities())
            {

                var entityUser = from d in context.kzUsers
                                 where d.email.ToLower() == Email.ToLower()
                                 && d.userstatus_id == 1
                                 select d;
                if (entityUser.Count() > 0)
                {
                    var entityFb = from f in context.kzFacebooks
                                   where f.user_id == entityUser.FirstOrDefault().id
                                   select f;
                    if (entityFb.Count() > 0)
                    {
                        response = Response<int>.Create(2);
                    }
                    else
                    {
                        string token = Security.RandomString2(255);
                        string usersecret = Security.Encrypt(token, entityUser.First().id.ToString());
                        //string usersecret = Security.checkHMAC2(token, entityUser.First().id.ToString());

                        entityUser.First().resetpass = token;
                        entityUser.First().resetpass_expire = DateTime.UtcNow.AddHours(1);
                        context.SaveChanges();
                        ResetEmail(Email, entityUser.First().first_name, usersecret, token);
                        response = Response<int>.Create(1);
                    }
                }
                else
                {
                    throw new CustomException(CustomErrorType.UserServiceInvalidEmail);
                }
            }
            return response;
        }
        public Response<bool> ResetPassword(String usersecret, String token, String password)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                string user = Security.Decrypt(token, usersecret);
                int userid = int.Parse(user);

                var entityUser = from d in context.kzUsers
                                 where d.id == userid && d.resetpass == token
                                 select d;
                if (entityUser.Count() > 0)
                {
                    DateTime now = DateTime.UtcNow;
                    if ((DateTime)entityUser.First().resetpass_expire < now)
                    {
                        response = Response<bool>.Create(false);
                    }
                    else
                    {
                        entityUser.First().locked_status = false;
                        string key1 = "ku4zo0";
                        string key = Security.checkHMAC(key1, entityUser.First().email.ToLower());
                        string pass = Security.checkHMAC(key, password);
                        entityUser.First().password = pass;
                        entityUser.First().password_salt = key;
                        entityUser.First().resetpass_expire = now;
                        context.SaveChanges();
                        response = Response<bool>.Create(true);
                    }
                }
                else
                {
                    throw new CustomException(CustomErrorType.UserServiceInvalidEmail);
                }
            }
            return response;
        }
        public Response<bool> CheckResetPassword(String usersecret, String token)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                string user = Security.Decrypt(token, usersecret);
                int userid = int.Parse(user);

                var entityUser = from d in context.kzUsers
                                 where d.id == userid && d.resetpass == token
                                 select d;
                if (entityUser.Count() > 0)
                {
                    DateTime now = DateTime.UtcNow;
                    if ((DateTime)entityUser.First().resetpass_expire < now)
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
                    throw new CustomException(CustomErrorType.UserServiceInvalidEmail);
                }
            }
            return response;
        }


        public Response<bool> CreateSubscribe(String email)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                entity.kzSubscribeEmail s = new entity.kzSubscribeEmail();
                s.email = email;
                s.created_date = DateTime.UtcNow;
                context.AddTokzSubscribeEmails(s);
                context.SaveChanges();
                response = Response<bool>.Create(true);
            }
            return response;
        }

        public void VerifyEmail(string email, string name, string token)
        {
            try
            {

                using (var context = new entity.KuazooEntities())
                {
                    string to = email;
                    //string body = "Dear "+name+",<br/><br/> Please <a href='"+ WebSetting.Url +"/User/Verify/"+token+"'>click here</a> to activate your account";
                    string body = helper.Email.CreateActivateAccountEmail(WebSetting.Url + "/User/Verify/" + token);
                    string subject = "One Step to Activate Account";

                    new GeneralService().SendEmail(subject, to, body);
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void ResetEmail(string email, string name, string usersecret, string token)
        {
            try
            {

                using (var context = new entity.KuazooEntities())
                {
                    string to = email;
                    //string body = "Dear "+name+",<br/><br/> Please <a href='"+ WebSetting.Url +"/User/Verify/"+token+"'>click here</a> to activate your account";
                    string body = helper.Email.CreateResetPasswordEmail(name, WebSetting.Url + "/User/ResetPassword/" + usersecret + "?usert=" + token);
                    string subject = "Password Reset Request";

                    new GeneralService().SendEmail(subject, to, body);
                }
            }
            catch (Exception ex)
            {

            }
        }
        public Response<bool> CommingSoonEmail(String email)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                var emailEntity = from d in context.kzEmails
                                  where d.email.ToLower() == email.ToLower()
                                  select d;
                if (emailEntity.Count() > 0)
                {
                }
                else
                {
                    entity.kzEmail ea = new entity.kzEmail();
                    ea.email = email;
                    context.AddTokzEmails(ea);
                    context.SaveChanges();
                }
                response = Response<bool>.Create(true);
            }
            return response;
        }
    }
}
