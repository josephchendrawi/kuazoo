using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Configuration;

namespace com.kuazoo
{
    public class AdminService : IAdminService
    {
        public Response<bool> CreateAdmin(Admin Admin)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                if (Admin.AdminId != 0)
                {
                    var entityAdmin = from d in context.kzAdmins
                                     where d.id == Admin.AdminId
                                     select d;
                    if (entityAdmin.Count() > 0)
                    {
                        var entityAdmin2 = from d in context.kzAdmins
                                          where d.email.ToLower() == Admin.Email.ToLower()
                                          && d.id != Admin.AdminId
                                          select d;
                        if (entityAdmin2.Count() > 0)
                        {
                            throw new CustomException(CustomErrorType.AdminAlreadyAssign);
                        }
                        else
                        {
                            entityAdmin.First().first_name = Admin.FirstName;
                            entityAdmin.First().last_name =  Admin.LastName;
                            entityAdmin.First().last_updated = DateTime.UtcNow;
                            entityAdmin.First().last_action = "3";
                            context.SaveChanges();
                            response = Response<bool>.Create(true);
                        }
                    }
                    else
                    {
                        throw new CustomException(CustomErrorType.AdminNotFound);
                    }
                }
                else
                {
                    var entityAdmin = from d in context.kzAdmins
                                     where d.email.ToLower() == Admin.Email.ToLower()
                                     select d;
                    if (entityAdmin.Count() > 0)
                    {
                        throw new CustomException(CustomErrorType.AdminAlreadyAssign);
                    }
                    else
                    {
                        entity.kzAdmin mmentity = new entity.kzAdmin();
                        mmentity.first_name = Admin.FirstName;
                        mmentity.last_name =  Admin.LastName;
                        mmentity.email = Admin.Email;
                        mmentity.last_updated = DateTime.UtcNow;
                        mmentity.last_created = DateTime.UtcNow;
                        //string key = Security.RandomString(60);
                        string key1 = "ku4zo0Admin";
                        string key = Security.checkHMAC(key1, Admin.Email.ToLower());
                        string pass = Security.checkHMAC(key, Admin.Password);
                        mmentity.password = pass;
                        mmentity.password_salt = key;
                        mmentity.last_action = "1";
                        context.AddTokzAdmins(mmentity);
                        context.SaveChanges();
                        response = Response<bool>.Create(true);
                    }
                }
            }
            return response;
        }
        public Response<bool> CheckLogin(Admin Admin)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
               
                var entityAdmin = from d in context.kzAdmins
                                 where d.email.ToLower() == Admin.Email.ToLower()
                                 && d.last_action !="5"
                                 select d;
                if (entityAdmin.Count() > 0)
                {
                    string key = entityAdmin.First().password_salt;
                    string pass = entityAdmin.First().password;
                    string passcek = Security.checkHMAC(key, Admin.Password);
                    if (pass == passcek)
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
                    throw new CustomException(CustomErrorType.AdminEmailPasswordWrong);
                }
            }
            return response;
        }

        public Response<bool> ChangePass(int Adminid, string oldpass, string newpass)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {

                var entityAdmin = from d in context.kzAdmins
                                 where d.id == Adminid
                                 && d.last_action !="5"
                                 select d;
                if (entityAdmin.Count() > 0)
                {
                    string key = entityAdmin.First().password_salt;
                    string pass = entityAdmin.First().password;
                    string passcek = Security.checkHMAC(key, oldpass);
                    if (pass == passcek)
                    {
                        string newpasssalt = Security.checkHMAC(key, newpass);
                        entityAdmin.First().password = newpasssalt;
                        context.SaveChanges();
                        response = Response<bool>.Create(true);
                    }
                    else
                    {
                        throw new CustomException(CustomErrorType.AdminPasswordWrong);
                    }
                }
                else
                {
                    throw new CustomException(CustomErrorType.AdminNotFound);
                }
            }
            return response;
        }
        public Response<Admin> GetCurrentAdmin()
        {
            try
            {
                Response<Admin> response = null;
                using (var context = new entity.KuazooEntities())
                {
                    string currentAdminName = Thread.CurrentPrincipal.Identity.Name;
                    var entityAdmin = from d in context.kzAdmins
                                        where d.email.ToLower() == currentAdminName.ToLower()
                                        && d.last_action !="5"
                                        select d;
                    Admin us = new Admin();
                    us.AdminId = entityAdmin.First().id;
                    us.FirstName = entityAdmin.First().first_name;
                    us.LastName =  entityAdmin.First().last_name;
                    us.Email = entityAdmin.First().email;
                    response = Response<Admin>.Create(us); 
                }
                return response;
            }
            catch (Exception ex)
            {
                throw new CustomException(CustomErrorType.Unauthenticated);
            }
        }

        public static Response<bool> Admin()
        {
            Response<bool> response = null;
            try
            {
                using (var context = new entity.KuazooEntities())
                {
                    string currentAdminName = Thread.CurrentPrincipal.Identity.Name;
                    var entityAdmin = from d in context.kzAdmins
                                      where d.email.ToLower() == currentAdminName.ToLower()
                                      && d.last_action != "5"
                                      select d;
                    if(entityAdmin.Count()>0)
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
        public Response<List<Admin>> GetAdminList()
        {
            try
            {
                Response<List<Admin>> response = null;
                List<Admin> listadmin = new List<Admin>();
                using (var context = new entity.KuazooEntities())
                {
                    var entityAdmin = from d in context.kzAdmins
                                      where d.last_action != "5" && d.email !="superadmin"
                                      select d;
                    foreach (var v in entityAdmin)
                    {
                        Admin us = new Admin();
                        us.AdminId =v.id;
                        us.Email = v.email;
                        us.FirstName =v.first_name;
                        us.LastName = v.last_name;
                        us.Create = (DateTime)v.last_created;
                        us.Update = (DateTime)v.last_updated;
                        us.LastAction = v.last_action;
                        listadmin.Add(us);
                    }
                    response = Response<List<Admin>>.Create(listadmin);
                }
                return response;
            }
            catch (Exception ex)
            {
                throw new CustomException(CustomErrorType.Unauthenticated);
            }
        }
        public Response<Admin> GetAdminById(int AdminId)
        {
            Response<Admin> response = null;
            Admin us = new Admin();
            using (var context = new entity.KuazooEntities())
            {
                var entityAdmin = from d in context.kzAdmins
                                  where d.id == AdminId && d.email != "superadmin"
                                    select d;
                var v = entityAdmin.First();
                if (v != null)
                {
                    us.AdminId = v.id;
                    us.Email = v.email;
                    us.FirstName = v.first_name;
                    us.LastName = v.last_name;
                    us.Create = (DateTime)v.last_created;
                    us.Update = (DateTime)v.last_updated;
                    us.LastAction = v.last_action;
                }
                response = Response<Admin>.Create(us);
            }

            return response;
        }
        public Response<bool> DeleteAdmin(int AdminId)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                var entityUser = from d in context.kzAdmins
                                 where d.id == AdminId && d.email != "superadmin"
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
