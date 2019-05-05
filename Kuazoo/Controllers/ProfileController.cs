using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using com.kuazoo;
using com.kuazoo.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Kendo.Mvc;
using System.Configuration;
using System.Web.Security;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.IO;
using Rotativa;

namespace Kuazoo.Controllers
{
    public class ProfileController : BaseController
    {
        //
        // GET: /Profile/

        private InventoryItemService inventoryitemservice = new InventoryItemService();
        private TransactionService transactionservice = new TransactionService();
        private UserService userservice = new UserService();
        private ReviewService reviewservice = new ReviewService();
        private ImageService imageservice = new ImageService();
        private PointService pointservice = new PointService();
        public ActionResult Index()
        {
            return RedirectToAction("AccountSetting", "Profile");
            //if (Request.IsAuthenticated && UserService.NotGuest().Result)
            //{

            //    return View();
            //}
            //else
            //{
            //    return RedirectToAction("Index","Home");
            //}
        }

        [HttpPost]
        public JsonResult GetDays(string syear = "1991", string smonth = "2")
        {
            int year = int.Parse(syear);
            int month = int.Parse(smonth);

            int end = -1;
            if (year % 4 == 0 && month == 2)
            {
                end = 29;
            }
            else if(month == 2)
            {
                end = 28;
            }
            else if (month == 1 || month == 3 || month == 5 || month == 7 || month == 8 || month == 10 || month == 12)
            {
                end = 31;
            }
            else
            {
                end = 30;
            }

            List<string> days = new List<string>();

            for (int i = 1; i <= end; i++)
            {
                days.Add(i.ToString());
            }

            return Json(days);
        }

        public ActionResult AccountSetting()
        {
            if (Request.IsAuthenticated && UserService.NotGuest().Result)
            {
                MemberModel.MemberVM2 model = new MemberModel.MemberVM2();
                var member = userservice.GetCurrentUser().Result;
                var shipping = transactionservice.GetCurrentUserShipping();
                var billing = transactionservice.GetCurrentUserBilling();

                model.MemberId = member.UserId;
                model.Email = member.Email;
                model.FirstName = member.FirstName;
                model.LastName = member.LastName;
                model.Gender = member.Gender;
                //model.DateOfBirth = member.DateOfBirth;
                model.Day = member.DateOfBirth.Day;
                model.Month = member.DateOfBirth.Month;
                model.Year = member.DateOfBirth.Year;
                model.ImageId = member.ImageId;
                model.ImageName = member.ImageName;
                model.ImageUrl = member.ImageUrl;
                model.Notif = member.Notif;
                model.Facebook = member.Facebook;
                model.Twitter = member.Twitter;
                if (shipping != null)
                {
                    MemberModel.MemberShipping2 ship = new MemberModel.MemberShipping2();
                    ship.FirstName = shipping.Result.FirstName;
                    ship.LastName = shipping.Result.LastName;
                    ship.AddressLine1 = shipping.Result.AddressLine1;
                    ship.AddressLine2 = shipping.Result.AddressLine2;
                    ship.City = shipping.Result.City;
                    ship.State = shipping.Result.State;
                    ship.Country = shipping.Result.Country;
                    ship.ZipCode = shipping.Result.ZipCode;
                    ship.Gender = shipping.Result.Gender;
                    ship.Phone = shipping.Result.Phone;
                    ship.Gift = shipping.Result.Gift;
                    ship.Note = shipping.Result.Note;
                    model.Shipping = ship;
                }
                else
                {
                    MemberModel.MemberShipping2 ship = new MemberModel.MemberShipping2();
                    ship.FirstName = model.FirstName;
                    ship.LastName = model.LastName;
                    ship.AddressLine1 = "";
                    ship.AddressLine2 = "";
                    ship.City = "";
                    ship.State = "";
                    ship.Country = "";
                    ship.ZipCode = "";
                    ship.Gender = model.Gender;
                    ship.Phone = "";
                    ship.Gift =  false;
                    ship.Note = "";
                    model.Shipping = ship;
                }
                if (billing != null)
                {
                    MemberModel.MemberBilling2 ship = new MemberModel.MemberBilling2();
                    ship.PaymentMethod = billing.Result.PaymentMethod;
                    PaymentMethod py = (PaymentMethod)billing.Result.PaymentMethod;
                    ship.PaymentMethodStr = py.ToString().ToLower();
                    ship.PaymentCC = billing.Result.PaymentCC;
                    ship.PaymentExpireMonth = billing.Result.PaymentExpireMonth;
                    ship.PaymentExpireYear = billing.Result.PaymentExpireYear;
                    ship.FirstName = billing.Result.FirstName;
                    ship.LastName = billing.Result.LastName;
                    ship.AddressLine1 = billing.Result.AddressLine1;
                    ship.AddressLine2 = billing.Result.AddressLine2;
                    ship.City = billing.Result.City;
                    ship.State = billing.Result.State;
                    ship.Country = billing.Result.Country;
                    ship.ZipCode = billing.Result.ZipCode;
                    ship.Gender = billing.Result.Gender;
                    ship.Phone = billing.Result.Phone;
                    model.Billing = ship;
                }
                else
                {
                    MemberModel.MemberBilling2 ship = new MemberModel.MemberBilling2();
                    ship.PaymentMethod = 0;
                    ship.PaymentMethodStr = "";
                    ship.PaymentCC = "";
                    ship.PaymentExpireMonth = 1;
                    ship.PaymentExpireYear = DateTime.Now.Year;
                    ship.FirstName = model.FirstName;
                    ship.LastName = model.LastName;
                    ship.AddressLine1 = "";
                    ship.AddressLine2 = "";
                    ship.City = "";
                    ship.State = "";
                    ship.Country = "";
                    ship.ZipCode = "";
                    ship.Gender = model.Gender;
                    ship.Phone = "";
                    model.Billing = ship;
                }


                if (TempData["msg"] != null)
                {
                    ViewBag.msg = TempData["msg"];
                }
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AccountSetting(FormCollection collection, MemberModel.MemberVM2 model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    User user = new User();
                    var useren = userservice.GetCurrentUser().Result;
                    user.UserId = useren.UserId;
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Gender = model.Gender;
                    user.ImageId = model.ImageId;
                    //user.DateOfBirth = (DateTime)model.DateOfBirth;
                    user.DateOfBirth = new DateTime(model.Year, model.Month, model.Day);
                    user.UserStatus = useren.UserStatus;
                    user.Notif = model.Notif;
                    var resultuser = userservice.UpdateUser(user).Result;

                    ShippingC ship = new ShippingC();
                    ship.fn = model.FirstName;
                    ship.ln = model.LastName;
                    ship.gender = model.Gender; //
                    ship.ph = model.Shipping.Phone;
                    ship.ad1 = model.Shipping.AddressLine1;
                    ship.ad2 = model.Shipping.AddressLine2;
                    ship.city = model.Shipping.City;
                    ship.state = model.Shipping.State;
                    ship.zip = model.Shipping.ZipCode;
                    ship.country = model.Shipping.Country;
                    int gift = 0;
                    if (model.Shipping.Gift) gift = 1;
                    ship.gift = gift;
                    ship.note = model.Shipping.Note;

                    var resultship = transactionservice.CreateShippingUser(ship, useren.UserId).Result;

                    BillingC bill = new BillingC();
                    bill.payment = model.Billing.PaymentMethodStr;
                    bill.cc = model.Billing.PaymentCC;
                    bill.ccv = model.Billing.PaymentCCV;
                    bill.month = model.Billing.PaymentExpireMonth.ToString();
                    bill.year = model.Billing.PaymentExpireYear.ToString();
                    bill.fn = model.Billing.FirstName;
                    bill.ln = model.Billing.LastName;
                    bill.gender = model.Gender; //
                    bill.ph = model.Billing.Phone;
                    bill.ad1 = model.Billing.AddressLine1;
                    bill.ad2 = model.Billing.AddressLine2;
                    bill.city = model.Billing.City;
                    bill.state = model.Billing.State;
                    bill.zip = model.Billing.ZipCode;
                    bill.country = model.Billing.Country;

                    var resultbill = transactionservice.CreateBillingUser(bill, useren.UserId).Result;


                    TempData["msg"] = "1";
                    return RedirectToAction("AccountSetting", "Profile");
                }
                catch (Exception ex)
                {
                    CustomException(ex);
                    ViewBag.err = ex.Message;
                }
            }
            else
            {
                if (String.IsNullOrWhiteSpace(model.FirstName))
                {
                    ViewBag.err2 = "Please fill up First Name!";
                }
                else if (String.IsNullOrWhiteSpace(model.LastName))
                {
                    ViewBag.err2 = "Please fill up Last Name!";
                }
                else if (model.Gender == 0)
                {
                    ViewBag.err2 = "Please fill up Gender!";
                }
            }
            return View(model);
        }
        public ActionResult ImageList()
        {
            return View();
        }
        [HttpPost]
        public virtual ActionResult ImageListUploadImage()//name and strsubimage come from post jquery
        {
            HttpPostedFileBase imageUpload = Request.Files["imagefile"];
            bool isUpload = false;
            string message = "";
            string imageurl = "";
            string imagename = "";
            int result = 0;
            if (imageUpload != null)
            {
                try
                {
                    if (!Helper.IsValidImage(imageUpload.FileName))
                    {
                        message = "File is not an image file";
                    }
                    string name = DateTime.UtcNow.ToString("yyyyMMddhhmmss");
                    imagename = Helper.uploadProfileImage(imageUpload, name);
                    imageurl = ConfigurationManager.AppSettings["uploadpath"] + imagename;

                    com.kuazoo.Image pro = new com.kuazoo.Image();
                    pro.Name = imagename;

                    result = imageservice.CreateImageId(pro).Result;
                    isUpload = true;
                    message = "Image uploaded successfully!";
                }
                catch (Exception ex)
                {
                    CustomException(ex);
                    message = string.Format("File upload failed: {0}", ex.Message);
                }
            }
            return Json(new { isUpload = isUpload, message = message, imageurl = imageurl, imagename = imagename, imageid = result }, "text/html");
        }


        public ActionResult ChangePassword()
        {
            return View();
        }
        public ActionResult MyPurchases()
        {
            if (Request.IsAuthenticated && UserService.NotGuest().Result)
            {
                List<TransactionsHeader> result = transactionservice.GetTransactionsListByUser().Result;
                
                return View(result);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult _Voucher(int id)
        {
            var response = transactionservice.TransactionHtml(id, Helper.defaultGMT);
            return View(response);
        }
        public ActionResult _VoucherServices(int id)
        {
            var response = transactionservice.TransactionHtml(id, Helper.defaultGMT);
            return View(response);
        }
        public ActionResult DownloadVoucher(int id)
        {
            return new ActionAsPdf("_VoucherServices", new { id = id }) { FileName = "Order.pdf", PageSize = Rotativa.Options.Size.A4, PageOrientation = Rotativa.Options.Orientation.Portrait, PageMargins = { Left = 0, Right = 0 } };
        
        }
        //public FileStreamResult DownloadVoucher(int id)
        //{
        //    if (Request.IsAuthenticated && UserService.NotGuest().Result)
        //    {
        //        string html = transactionservice.TransactionHtml(id, Helper.defaultGMT);
        //        if (html !="")
        //        {
        //            MemoryStream m = transactionservice.GetpdfHtml(html);
        //            return new FileStreamResult(m, "application/pdf");
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        public ActionResult MyReviews()
        {
            if (Request.IsAuthenticated && UserService.NotGuest().Result)
            {
                List<com.kuazoo.Review> reviewlist = reviewservice.GetReviewByInventoryItemByUser().Result;
                List<ReviewVM> list = new List<ReviewVM>();
                foreach (var v in reviewlist)
                {
                    list.Add(new ReviewVM() { ReviewId = v.ReviewId, InventoryItemId = v.InventoryItemId, InventoryItemName = v.InventoryItemName, InventoryItemImageUrl =v.InventoryItemImageUrl,PrizeImageUrl =v.PrizeImageUrl , MemberEmail = v.MemberEmail, MemberId = v.MemberId, MemberFullName = v.MemberFullName, MemberImage = v.MemberImage, Message = v.Message, ReviewDate = v.ReviewDate.AddHours(Helper.defaultGMT), ReviewDateStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.ReviewDate.AddHours(Helper.defaultGMT)), Rating = v.Rating });
                }
                return View(list);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public JsonResult ReviewEdit(int id, int rating, string message)
        {
            try
            {
                com.kuazoo.Review rev = new com.kuazoo.Review();
                rev.ReviewId = id;
                rev.Rating = rating;
                rev.Message = message;
                var result = reviewservice.UpdateReview(rev).Result;
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
        [HttpPost]
        public JsonResult ReviewDelete(int id)
        {
            try
            {
                var result = reviewservice.DeleteReview(id).Result;
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

        public ActionResult Wishlist()
        {
            if (Request.IsAuthenticated && UserService.NotGuest().Result)
            {
                DateTime today = DateTime.UtcNow;
                var result = inventoryitemservice.GetWishList(today).Result;
                List<InventoryItemModel.InventoryItemShow> newlistmer = new List<InventoryItemModel.InventoryItemShow>();
                foreach (var v in result)
                {
                    List<string> variance = new List<string>();

                    //bool checkvar = true;
                    //if (v.InventoryItem.Variance.Count == 1)
                    //{
                    //    if (v.InventoryItem.Variance[0].Price == v.InventoryItem.Price && v.InventoryItem.Variance[0].Discount == v.InventoryItem.Discount)
                    //    {
                    //        checkvar = false;
                    //    }
                    //}
                    //if (checkvar)
                    //{
                    if (v.InventoryItem.FlashDeal == null)
                    {
                        foreach (var vari in v.InventoryItem.Variance)
                        {
                            variance.Add(vari.Variance + "`" + vari.Price + "`" + vari.Discount + "`" + vari.AvailabelLimit);
                        }
                    }
                    else
                    {
                        foreach (var vari in v.InventoryItem.Variance)
                        {
                            variance.Add(vari.Variance + "`" + vari.Price + "`" + (vari.Discount + v.InventoryItem.FlashDeal.Discount) + "`" + vari.AvailabelLimit);
                        }
                    }
                    //}
                    if (v.InventoryItem.Prize != null && v.InventoryItem.FlashDeal != null)
                    {
                        newlistmer.Add(new InventoryItemModel.InventoryItemShow() { WishlistId = v.WishlistId, InventoryItemId = v.InventoryItem.InventoryItemId, Name = v.InventoryItem.Name, ShortDesc = v.InventoryItem.ShortDesc, Price = v.InventoryItem.Price, TypeId = v.InventoryItem.TypeId, TypeName = v.InventoryItem.TypeName, Discount = v.InventoryItem.Discount, MaximumSales = v.InventoryItem.MaximumSales, Sales = v.InventoryItem.Sales, SalesVisualMeter = v.InventoryItem.SalesVisualMeter, ImageId = v.InventoryItem.ImageId, ImageName = v.InventoryItem.ImageName, ImageUrl = v.InventoryItem.ImageUrl, SubImageId = v.InventoryItem.SubImageId, SubImageName = v.InventoryItem.SubImageName, SubImageUrl = v.InventoryItem.SubImageUrl, Variance = string.Join("|", variance), Prize = new InventoryItemModel.PrizeShow() { PrizeId = v.InventoryItem.Prize.PrizeId, Name = v.InventoryItem.Prize.Name, Description = v.InventoryItem.Prize.Description, ImageId = v.InventoryItem.Prize.ImageId, ImageName = v.InventoryItem.Prize.ImageName, ImageUrl = v.InventoryItem.Prize.ImageUrl, SubImageId = v.InventoryItem.Prize.SubImageId, SubImageName = v.InventoryItem.Prize.SubImageName, SubImageUrl = v.InventoryItem.Prize.SubImageUrl, Price = v.InventoryItem.Prize.Price, SponsorName = v.InventoryItem.Prize.SponsorName, Detail = v.InventoryItem.Prize.Detail, Terms = v.InventoryItem.Prize.Terms, ExpiryDate = v.InventoryItem.Prize.ExpiryDate.AddHours(Kuazoo.Helper.defaultGMT), ExpiryDateStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.InventoryItem.Prize.ExpiryDate.AddHours(Kuazoo.Helper.defaultGMT)), GroupName = v.InventoryItem.Prize.GroupName, FreeDeal = v.InventoryItem.Prize.FreeDeal }, FlashDeal = new InventoryItemModel.FlashDeal() { FlashDealId = v.InventoryItem.FlashDeal.FlashDealId, Discount = v.InventoryItem.FlashDeal.Discount, MerchantId = v.InventoryItem.FlashDeal.MerchantId, MerchantName = v.InventoryItem.FlashDeal.MerchantName, InventoryItemId = v.InventoryItem.FlashDeal.InventoryItemId, InventoryItemName = v.InventoryItem.FlashDeal.InventoryItemName, StartTime = v.InventoryItem.FlashDeal.StartTime, StartTimeStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.InventoryItem.FlashDeal.StartTime), EndTime = v.InventoryItem.FlashDeal.EndTime.AddHours(Kuazoo.Helper.defaultGMT), EndTimeStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.InventoryItem.FlashDeal.EndTime.AddHours(Kuazoo.Helper.defaultGMT)) } });
                    }
                    else if (v.InventoryItem.Prize != null && v.InventoryItem.FlashDeal == null)
                    {
                        newlistmer.Add(new InventoryItemModel.InventoryItemShow() { WishlistId = v.WishlistId, InventoryItemId = v.InventoryItem.InventoryItemId, Name = v.InventoryItem.Name, ShortDesc = v.InventoryItem.ShortDesc, Price = v.InventoryItem.Price, TypeId = v.InventoryItem.TypeId, TypeName = v.InventoryItem.TypeName, Discount = v.InventoryItem.Discount, MaximumSales = v.InventoryItem.MaximumSales, Sales = v.InventoryItem.Sales, SalesVisualMeter = v.InventoryItem.SalesVisualMeter, ImageId = v.InventoryItem.ImageId, ImageName = v.InventoryItem.ImageName, ImageUrl = v.InventoryItem.ImageUrl, SubImageId = v.InventoryItem.SubImageId, SubImageName = v.InventoryItem.SubImageName, SubImageUrl = v.InventoryItem.SubImageUrl, Variance = string.Join("|", variance), Prize = new InventoryItemModel.PrizeShow() { PrizeId = v.InventoryItem.Prize.PrizeId, Name = v.InventoryItem.Prize.Name, Description = v.InventoryItem.Prize.Description, ImageId = v.InventoryItem.Prize.ImageId, ImageName = v.InventoryItem.Prize.ImageName, ImageUrl = v.InventoryItem.Prize.ImageUrl, SubImageId = v.InventoryItem.Prize.SubImageId, SubImageName = v.InventoryItem.Prize.SubImageName, SubImageUrl = v.InventoryItem.Prize.SubImageUrl, Price = v.InventoryItem.Prize.Price, SponsorName = v.InventoryItem.Prize.SponsorName, Detail = v.InventoryItem.Prize.Detail, Terms = v.InventoryItem.Prize.Terms, ExpiryDate = v.InventoryItem.Prize.ExpiryDate.AddHours(Kuazoo.Helper.defaultGMT), ExpiryDateStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.InventoryItem.Prize.ExpiryDate.AddHours(Kuazoo.Helper.defaultGMT)), GroupName = v.InventoryItem.Prize.GroupName,FreeDeal = v.InventoryItem.Prize.FreeDeal } });
                    }
                    else if (v.InventoryItem.Prize == null && v.InventoryItem.FlashDeal != null)
                    {
                        newlistmer.Add(new InventoryItemModel.InventoryItemShow() { WishlistId = v.WishlistId, InventoryItemId = v.InventoryItem.InventoryItemId, Name = v.InventoryItem.Name, ShortDesc = v.InventoryItem.ShortDesc, Price = v.InventoryItem.Price, TypeId = v.InventoryItem.TypeId, TypeName = v.InventoryItem.TypeName, Discount = v.InventoryItem.Discount, MaximumSales = v.InventoryItem.MaximumSales, Sales = v.InventoryItem.Sales, SalesVisualMeter = v.InventoryItem.SalesVisualMeter, ImageId = v.InventoryItem.ImageId, ImageName = v.InventoryItem.ImageName, ImageUrl = v.InventoryItem.ImageUrl, SubImageId = v.InventoryItem.SubImageId, SubImageName = v.InventoryItem.SubImageName, SubImageUrl = v.InventoryItem.SubImageUrl, Variance = string.Join("|", variance), FlashDeal = new InventoryItemModel.FlashDeal() { FlashDealId = v.InventoryItem.FlashDeal.FlashDealId, Discount = v.InventoryItem.FlashDeal.Discount, MerchantId = v.InventoryItem.FlashDeal.MerchantId, MerchantName = v.InventoryItem.FlashDeal.MerchantName, InventoryItemId = v.InventoryItem.FlashDeal.InventoryItemId, InventoryItemName = v.InventoryItem.FlashDeal.InventoryItemName, StartTime = v.InventoryItem.FlashDeal.StartTime, StartTimeStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.InventoryItem.FlashDeal.StartTime), EndTime = v.InventoryItem.FlashDeal.EndTime.AddHours(Kuazoo.Helper.defaultGMT), EndTimeStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.InventoryItem.FlashDeal.EndTime.AddHours(Kuazoo.Helper.defaultGMT)) } });
                    }
                    else
                    {
                        newlistmer.Add(new InventoryItemModel.InventoryItemShow() { WishlistId = v.WishlistId, InventoryItemId = v.InventoryItem.InventoryItemId, Name = v.InventoryItem.Name, ShortDesc = v.InventoryItem.ShortDesc, Price = v.InventoryItem.Price, TypeId = v.InventoryItem.TypeId, TypeName = v.InventoryItem.TypeName, Discount = v.InventoryItem.Discount, MaximumSales = v.InventoryItem.MaximumSales, Sales = v.InventoryItem.Sales, SalesVisualMeter = v.InventoryItem.SalesVisualMeter, ImageId = v.InventoryItem.ImageId, ImageName = v.InventoryItem.ImageName, ImageUrl = v.InventoryItem.ImageUrl, SubImageId = v.InventoryItem.SubImageId, SubImageName = v.InventoryItem.SubImageName, SubImageUrl = v.InventoryItem.SubImageUrl, Variance = string.Join("|", variance) });
                    }
                }
                InventoryItemModel.InventoryItemVM model = new InventoryItemModel.InventoryItemVM();
                model.item = newlistmer;
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public JsonResult WishlistDelete(int id)
        {
            try
            {
                var result = inventoryitemservice.DeleteWishlist(id).Result;
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

        public ActionResult KPointBalance()
        {
            PointHeader result = null;
            if (Request.IsAuthenticated && UserService.NotGuest().Result)
            {
                result = pointservice.GetListPoint().Result;
            }
            return View(result);
        }
    }
}
