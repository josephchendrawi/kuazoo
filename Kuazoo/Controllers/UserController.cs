using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using com.kuazoo.Models;
using com.kuazoo;
using System.Web.Security;

namespace Kuazoo.Controllers
{
    public class UserController : BaseController
    {
        //
        // GET: /User/
        private UserService userservice = new UserService();
        public ActionResult Index()
        {
            return RedirectToAction("SignIn");
        }
        public ActionResult SignIn()
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Index","Home");
            }
            else
            {
                if (TempData["msg"] != null)
                {
                    ViewBag.msg = TempData["msg"];
                    TempData["msg"] = null;
                }
                if (TempData["result"] != null)
                {
                    ViewBag.result = TempData["result"];
                    TempData["result"] = null;
                }
                return View();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn(LogOnModel model, String returnUrl = "")
        {
            if (ModelState.IsValid)
            {
                try
                {
                    com.kuazoo.User pro = new com.kuazoo.User();
                    pro.Email = model.UserName;
                    pro.Password = model.Password;

                    bool result = userservice.CheckLogin(pro).Result;
                    if (result)
                    {
                        FormsAuthentication.SetAuthCookie(pro.Email, model.RememberMe);
                        ViewBag.result = "1";
                        TempData["msg"] = "11";
                        if (returnUrl == "")
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            String url = Request.Url.GetLeftPart(UriPartial.Authority) + System.Web.HttpUtility.UrlDecode(returnUrl);
                            return Redirect(url);
                        }
                    }
                    else
                    {
                        ViewBag.result = "3";
                        TempData["msg"] = "33";
                    }
                    return RedirectToAction("SignIn", "User", routeValues: new { returnUrl = System.Web.HttpUtility.UrlDecode(returnUrl) });
                }
                catch (Exception ex)
                {
                    CustomException(ex);
                    ViewBag.result = ex.Message;
                }
            }
            return View(model);

        }
        [HttpGet]
        public ActionResult Twitterlogin()
        {
            

            return View();
        }
        [HttpGet]
        public ActionResult FacebookLogin(string token, String returnUrl = "")
        {
            try
            {
                Session["token"] = token;

                //externally get location
                Facebook.FacebookAPI api = new Facebook.FacebookAPI(token);
                Facebook.JSONObject userObject = api.Get("/me");

                Session["facebookID"] = userObject.Dictionary.ContainsKey("id") ? userObject.Dictionary["id"].String : string.Empty;
                Session["facebookToken"] = token;

                FbUser fbuser = new FbUser();
                fbuser.AccessToken = token;
                fbuser.FacebookId = userObject.Dictionary.ContainsKey("id") ? userObject.Dictionary["id"].String : string.Empty;
                fbuser.FacebookEmail = userObject.Dictionary.ContainsKey("email") ? userObject.Dictionary["email"].String : string.Empty;
                fbuser.FacebookName = userObject.Dictionary.ContainsKey("name") ? userObject.Dictionary["name"].String : string.Empty;
                fbuser.FacebookGender = userObject.Dictionary.ContainsKey("gender") ? userObject.Dictionary["gender"].String : string.Empty;
                fbuser.FaceBookDob = userObject.Dictionary.ContainsKey("birthday") ? userObject.Dictionary["birthday"].String : string.Empty;

                if (fbuser.FacebookEmail == "")
                {
                    ViewBag.result = "4";
                    TempData["msg"] = "44";
                    if (returnUrl == "")
                    {
                        return RedirectToAction("SignIn", "User");
                    }
                    else
                    {
                        return RedirectToAction("SignIn", "User", new { returnUrl = returnUrl });
                    }
                }
                else
                {

                    int result = userservice.CreateUserUsingFacebook(fbuser).Result;
                    if (result != null && result > 0)
                    {
                        var customer = userservice.GetCurrentUserById(result).Result;
                        FormsAuthentication.SetAuthCookie(customer.Email, true);
                        ViewBag.result = "1";
                        TempData["msg"] = "11";
                        if (returnUrl == "")
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            return Redirect(returnUrl);
                        }
                    }
                    else
                    {
                        TempData["msg"] = "Link to Facebook unsuccessful";
                    }
                }
            }
            catch (Exception ex)
            {
                CustomException(ex);
                TempData["msg"] = ex.Message;
            }
            return RedirectToAction("SignIn", "User");
        }

        public ActionResult FacebookRedirect(String returnUrl = "")
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        public ActionResult SignUp()
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Index","Home");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp(RegisterModel model, String returnUrl = "")
        {
            if (ModelState.IsValid)
            {
                try
                {
                    com.kuazoo.User pro = new com.kuazoo.User();
                    pro.UserId = 0;
                    pro.FirstName = model.FirstName;
                    pro.LastName = model.LastName;
                    pro.Email = model.Email;
                    pro.Password = model.Password;
                    pro.Gender = 0;
                    //if (model.VerifyEmail)
                    //{
                    //    pro.UserStatus = 4;
                    //}
                    //else
                    //{
                    //    pro.UserStatus = 1;
                    //}
                    pro.UserStatus = 1;
                    //pro.Verify = model.VerifyEmail;
                    bool result = userservice.CreateUser(pro).Result;
                    if (result == true)
                    {
                        //if (model.VerifyEmail)
                        //{
                        //    ViewBag.result = "1";
                        //    TempData["msg"] = ViewBag.result;
                        //}
                        FormsAuthentication.SetAuthCookie(pro.Email, true);
                        if (returnUrl == "")
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            return Redirect(returnUrl);
                            //return RedirectToAction("SignIn", "User", new{ returnUrl = returnUrl });
                        }
                    }
                    else
                    {
                        ViewBag.result = "3";
                    }
                    if (returnUrl == "")
                    {
                        return RedirectToAction("SignUp", "User");
                    }
                    else
                    {
                        return RedirectToAction("SignUp", "User", new { returnUrl = returnUrl });
                    }
                }
                catch (Exception ex)
                {
                    CustomException(ex);
                    ViewBag.result = ex.Message;
                }
            }
            return View(model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp2(RegisterModel model, String returnUrl = "")
        {
            if (ModelState.IsValid)
            {
                try
                {
                    com.kuazoo.User pro = new com.kuazoo.User();
                    pro.UserId = 0;
                    pro.FirstName = model.FirstName;
                    pro.LastName = model.LastName;
                    pro.Email = model.Email;
                    pro.Password = model.Password;
                    pro.Gender = 0;
                    //if (model.VerifyEmail)
                    //{
                    //    pro.UserStatus = 4;
                    //}
                    //else
                    //{
                    //    pro.UserStatus = 1;
                    //}
                    pro.UserStatus = 1;
                    //pro.Verify = model.VerifyEmail;
                    bool result = userservice.CreateUser(pro).Result;
                    if (result == true)
                    {
                        //if (model.VerifyEmail)
                        //{
                        //    ViewBag.result = "1";
                        //    TempData["msg"] = ViewBag.result;
                        //}
                        FormsAuthentication.SetAuthCookie(pro.Email, true);
                        if (returnUrl == "")
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            return Redirect(returnUrl);
                            //return RedirectToAction("SignIn", "User", new{ returnUrl = returnUrl });
                        }
                    }
                    else
                    {
                        TempData["result"] = "93";
                    }
                    if (returnUrl == "")
                    {
                        return RedirectToAction("SignIn", "User");
                    }
                    else
                    {
                        return RedirectToAction("SignIn", "User", new { returnUrl = returnUrl });
                    }
                }
                catch (Exception ex)
                {
                    CustomException(ex);
                    TempData["result"] = ex.Message;
                }
            }
            return RedirectToAction("SignIn", "User");

        }


        public ActionResult ForgotPassword()
        {
            if (TempData["msg"] != null)
            {
                ViewBag.msg = TempData["msg"];
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotModel model)
        {
            if (ModelState.IsValid)
            {
                string email = model.Email;
                try
                {
                    int result = userservice.ForgotPassword(email).Result;
                    if (result == 1)
                    {
                        TempData["msg"] = "1";
                    }
                    else if (result == 2)
                    {
                        TempData["msg"] = "2";
                    }
                    else
                    {
                        TempData["msg"] = "3";
                    }
                    return RedirectToAction("ForgotPassword", "User");
                }
                catch (Exception ex)
                {
                    CustomException(ex);
                    TempData["msg"] = ex.Message;
                    return RedirectToAction("ForgotPassword", "User");
                }
            }
            return View(model);
        }
        public ActionResult ResetPassword(String id, String usert)
        {
            ResetModel model = new ResetModel();
            try
            {
                ViewBag.reset = false;
                if (TempData["msg"] != null)
                {
                    ViewBag.msg = TempData["msg"];
                }
                else
                {
                    if (id != null && usert != null)
                    {
                        bool result = userservice.CheckResetPassword(id, usert).Result;
                        if (result)
                        {
                            ViewBag.reset = true;
                            model.User = id;
                            model.Token = usert;
                        }
                    }
                }
            }
            catch{
                ViewBag.reset=false;
                ViewBag.msg="";
            }
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetModel model)
        {
            if (ModelState.IsValid)
            {
                string password = model.NewPassword;
                string user = model.User;
                string token = model.Token;
                try
                {
                    bool result = userservice.ResetPassword(user, token, password).Result;
                    if (result)
                    {
                        TempData["msg"] = "1";
                    }
                    else
                    {
                        TempData["msg"] = "3";
                    }
                    return RedirectToAction("ResetPassword", "User");
                }
                catch (Exception ex)
                {
                    CustomException(ex);
                    TempData["msg"] = ex.Message;
                    return RedirectToAction("ResetPassword", "User");
                }
            }
            ViewBag.reset = true;
            return View(model);
        }

        [HttpPost]
        public JsonResult ChangePass(string oldpass, string newpass)
        {
            try
            {
                int userid = userservice.GetCurrentUser().Result.UserId;
                var result = userservice.ChangePass(userid, oldpass, newpass).Result;
                if (result)
                {
                   
                    return Json(new { success = true, error = ""}, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, error = "Failed" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                CustomException(ex);
                return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Verify(string id)
        {
            try
            {
                bool result = userservice.UserVerifying(id).Result;
                if (result)
                {
                    TempData["msg"] = "22";
                    return RedirectToAction("SignIn", "User");
                }
                else
                {
                    ViewBag.result = "3";
                }
            }
            catch (Exception ex)
            {
                CustomException(ex);
                ViewBag.result = ex.Message;
            }
            return View();
        }

        [HttpGet]
        public JsonResult ResendEmail()
        {
            try
            {
                int userid = userservice.GetCurrentUser().Result.UserId;
                var result = userservice.ResendEmail(userid).Result;
                if (result)
                {
                    return Json(new { success = true, error = "" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, error = "Failed" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                CustomException(ex);
                return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}
