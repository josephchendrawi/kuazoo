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
using System.Text.RegularExpressions;

namespace Kuazoo.Controllers
{
    public class HomeController : BaseController
    {
        private InventoryItemService inventoryitemservice = new InventoryItemService();
        private TransactionService transactionservice = new TransactionService();
        private UserService userservice = new UserService();
        private ReviewService reviewservice = new ReviewService();
        private PrizeService prizeservice = new PrizeService();
        private PointService pointservice = new PointService();
        private CountryService countryservice = new CountryService();
        private GameService gameservice = new GameService();
        private LandingService landingservice = new LandingService();
        private FreeDealService freedealservice = new FreeDealService();
        //public ActionResult Index()
        //{
        //    ViewBag.Message = "Welcome to ASP.NET MVC!";
        //    //return RedirectToAction("Index", "Admin");
        //    return View();
        //}
        //public ActionResult Main()
        //{
        //    return View();
        //}
        [HttpPost]
        public JsonResult CommingSoon(string email)
        {
            try
            {
                var result = userservice.CommingSoonEmail(email).Result;
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

        public ActionResult Index()
        {
            LandingModel.LandingVM model = new LandingModel.LandingVM();

            List<com.kuazoo.Banner> listmer = landingservice.GetBannerList().Result;
            List<BannerModel> newlistmer = new List<BannerModel>();
            foreach (var v in listmer)
            {
                if (v.LastAction != "5")
                {
                    if (v.Link.IndexOf("http") == -1)
                        v.Link = "http://" + v.Link;
                    newlistmer.Add(new BannerModel() { Link = v.Link, SubImageUrl = v.SubImageUrl });
                }
            }
            model.ListBanner = newlistmer;

            List<com.kuazoo.BVD> listmer2 = landingservice.GetBVDList().Result;
            List<BVDModel> newlistmer2 = new List<BVDModel>();
            foreach (var v in listmer2)
            {
                if (v.LastAction != "5")
                {
                    if (v.Link.IndexOf("http") == -1)
                        v.Link = "http://" + v.Link;
                    if(v.Type == 1)
                        v.SubImageUrl = "https://img.youtube.com/vi/" + GetYouTubeId(v.Link) + "/default.jpg";
                    newlistmer2.Add(new BVDModel() { Title = v.Title, Type = v.Type, Link = v.Link, SubImageUrl = v.SubImageUrl, UpdatedDate = v.UpdatedDate });
                }
            }
            model.ListBVD = newlistmer2;


            return View(model);
        }

        public string GetYouTubeId(string videoUrl)
        {
            int mInd = videoUrl.IndexOf("/v/");
            if (mInd != -1)
            {
                string strVideoCode = videoUrl.Substring(videoUrl.IndexOf("/v/") + 3);
                int ind = strVideoCode.IndexOf("?");
                strVideoCode = strVideoCode.Substring(0, ind == -1 ? strVideoCode.Length : ind);
                return strVideoCode;
                //return "https://img.youtube.com/vi/" + strVideoCode + "/default.jpg";
            }
            else
            {
                int mInd2 = videoUrl.IndexOf("v=");
                if (mInd2 != -1)
                {
                    string strVideoCode = videoUrl.Substring(videoUrl.IndexOf("v=") + 2);
                    int ind = strVideoCode.IndexOf("&");
                    strVideoCode = strVideoCode.Substring(0, ind == -1 ? strVideoCode.Length : ind);
                    return strVideoCode;
                    //return "https://img.youtube.com/vi/" + strVideoCode + "/default.jpg";
                }
                else
                    return "";
            }
        }

        public ActionResult TheKuazooStore(int? page, int id = 0, string c = "")
        {
            //transactionservice.TransactionMemberNotif(148, Server.MapPath("/Rotativa"), Helper.defaultGMT); /// for testing purpose
            //transactionservice.TransactionGiftNotice(148, Server.MapPath("/Rotativa"), Helper.defaultGMT); /// for testing purpose
            
            TempData["msg"] = null;
            int pageSize = Helper.pageSize;
            int pageNumber = (page ?? 0);
            int totalCount = 0;
            ViewBag.cityid = id;
            ViewBag.category = c;
            DateTime today = DateTime.UtcNow;
            List<com.kuazoo.InventoryItemShow> listmer = inventoryitemservice.GetInventoryItemListByCity(id, today,ref totalCount, c, pageSize, pageNumber).Result;
            List<InventoryItemModel.InventoryItemShow> newlistmer = new List<InventoryItemModel.InventoryItemShow>();
            foreach (var v in listmer)
            {
                if (v.Prize != null && v.FlashDeal != null)
                {
                    newlistmer.Add(new InventoryItemModel.InventoryItemShow() { InventoryItemId = v.InventoryItemId, Wishlist = v.Wishlist, Name = v.Name, ShortDesc = v.ShortDesc, Description = v.Description, Price = v.Price, TypeId = v.TypeId, TypeName = v.TypeName, Discount = v.Discount, MaximumSales = v.MaximumSales, Sales = v.Sales, SalesVisualMeter = v.SalesVisualMeter, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = v.SubImageId, SubImageName = v.SubImageName, SubImageUrl = v.SubImageUrl, Tag = v.Tag, Prize = new InventoryItemModel.PrizeShow() { PrizeId = v.Prize.PrizeId, GameType = v.Prize.GameType, Name = v.Prize.Name, Description = v.Prize.Description, ImageId = v.Prize.ImageId, ImageName = v.Prize.ImageName, ImageUrl = v.Prize.ImageUrl, SubImageId = v.Prize.SubImageId, SubImageName = v.Prize.SubImageName, SubImageUrl = v.Prize.SubImageUrl, Price = v.Prize.Price, SponsorName = v.Prize.SponsorName, Detail = v.Prize.Detail, Terms = v.Prize.Terms, ExpiryDate = v.Prize.ExpiryDate, ExpiryDateStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.Prize.ExpiryDate), GroupName = v.Prize.GroupName, FreeDeal = v.Prize.FreeDeal }, FlashDeal = new InventoryItemModel.FlashDeal() { FlashDealId = v.FlashDeal.FlashDealId, Discount = v.FlashDeal.Discount, MerchantId = v.FlashDeal.MerchantId, MerchantName = v.FlashDeal.MerchantName, InventoryItemId = v.FlashDeal.InventoryItemId, InventoryItemName = v.FlashDeal.InventoryItemName, StartTime = v.FlashDeal.StartTime, StartTimeStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.FlashDeal.StartTime), EndTime = v.FlashDeal.EndTime, EndTimeStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.FlashDeal.EndTime) } });
                }
                else if (v.Prize != null && v.FlashDeal == null)
                {
                    newlistmer.Add(new InventoryItemModel.InventoryItemShow() { InventoryItemId = v.InventoryItemId, Wishlist = v.Wishlist, Name = v.Name, ShortDesc = v.ShortDesc, Description = v.Description, Price = v.Price, TypeId = v.TypeId, TypeName = v.TypeName, Discount = v.Discount, MaximumSales = v.MaximumSales, Sales = v.Sales, SalesVisualMeter = v.SalesVisualMeter, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = v.SubImageId, SubImageName = v.SubImageName, SubImageUrl = v.SubImageUrl, Tag = v.Tag, Prize = new InventoryItemModel.PrizeShow() { PrizeId = v.Prize.PrizeId, GameType = v.Prize.GameType, Name = v.Prize.Name, Description = v.Prize.Description, ImageId = v.Prize.ImageId, ImageName = v.Prize.ImageName, ImageUrl = v.Prize.ImageUrl, SubImageId = v.Prize.SubImageId, SubImageName = v.Prize.SubImageName, SubImageUrl = v.Prize.SubImageUrl, Price = v.Prize.Price, SponsorName = v.Prize.SponsorName, Detail = v.Prize.Detail, Terms = v.Prize.Terms, ExpiryDate = v.Prize.ExpiryDate, ExpiryDateStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.Prize.ExpiryDate), GroupName = v.Prize.GroupName, FreeDeal = v.Prize.FreeDeal } });
                }
                else if (v.Prize == null && v.FlashDeal != null)
                {
                    newlistmer.Add(new InventoryItemModel.InventoryItemShow() { InventoryItemId = v.InventoryItemId, Wishlist = v.Wishlist, Name = v.Name, ShortDesc = v.ShortDesc, Description = v.Description, Price = v.Price, TypeId = v.TypeId, TypeName = v.TypeName, Discount = v.Discount, MaximumSales = v.MaximumSales, Sales = v.Sales, SalesVisualMeter = v.SalesVisualMeter, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = v.SubImageId, SubImageName = v.SubImageName, SubImageUrl = v.SubImageUrl, Tag = v.Tag, FlashDeal = new InventoryItemModel.FlashDeal() { FlashDealId = v.FlashDeal.FlashDealId, Discount = v.FlashDeal.Discount, MerchantId = v.FlashDeal.MerchantId, MerchantName = v.FlashDeal.MerchantName, InventoryItemId = v.FlashDeal.InventoryItemId, InventoryItemName = v.FlashDeal.InventoryItemName, StartTime = v.FlashDeal.StartTime, StartTimeStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.FlashDeal.StartTime), EndTime = v.FlashDeal.EndTime, EndTimeStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.FlashDeal.EndTime) } });
                }
                else
                {
                    newlistmer.Add(new InventoryItemModel.InventoryItemShow() { InventoryItemId = v.InventoryItemId, Wishlist = v.Wishlist, Name = v.Name, ShortDesc = v.ShortDesc, Description = v.Description, Price = v.Price, TypeId = v.TypeId, TypeName = v.TypeName, Discount = v.Discount, MaximumSales = v.MaximumSales, Sales = v.Sales, SalesVisualMeter = v.SalesVisualMeter, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = v.SubImageId, SubImageName = v.SubImageName, SubImageUrl = v.SubImageUrl, Tag = v.Tag });
                }
            }
            InventoryItemModel.InventoryItemVM model = new InventoryItemModel.InventoryItemVM();
            model.item = newlistmer;
            model.TotalCount = totalCount;
            model.NextPage = pageNumber + 1;
            bool hasnext = false;
            if (((pageNumber + 1) * pageSize) < totalCount)
            {
                hasnext = true;
            }
            model.HasNext = hasnext;
            return View(model);
        }
        public ActionResult FeaturedDeal(int id = 0, string c = "")
        {
            if (TempData["msg"] != null)
            {
                ViewBag.msg = TempData["msg"];
                TempData["msg"] = null;
            }
            ViewBag.cityid = id;
            ViewBag.category = c;
            DateTime today = DateTime.UtcNow;
            List<com.kuazoo.InventoryItemShow> listmer = inventoryitemservice.GetInventoryItemListByCityKuazooDeal(id, today, c).Result;
            List<InventoryItemModel.InventoryItemShow> newlistmer = new List<InventoryItemModel.InventoryItemShow>();
            foreach (var v in listmer)
            {
                List<string> variance = new List<string>();
                //bool checkvar = true;
                //if (v.Variance.Count == 1)
                //{
                //    if (v.Variance[0].Price == v.Price && v.Variance[0].Discount == v.Discount)
                //    {
                //        checkvar = false;
                //    }
                //}
                //if (checkvar)
                //{
                if (v.FlashDeal == null)
                {
                    foreach (var vari in v.Variance)
                    {
                        variance.Add(vari.Variance + "`" + vari.Price + "`" + vari.Discount + "`" + vari.AvailabelLimit);
                    }
                }
                else
                {
                    foreach (var vari in v.Variance)
                    {
                        variance.Add(vari.Variance + "`" + vari.Price + "`" + (vari.Discount + v.FlashDeal.Discount) + "`" + vari.AvailabelLimit);
                    }
                }
                //}
                if (v.Prize != null && v.FlashDeal != null)
                {
                    newlistmer.Add(new InventoryItemModel.InventoryItemShow() { InventoryItemId = v.InventoryItemId, Wishlist = v.Wishlist, Name = v.Name, ShortDesc = v.ShortDesc, Description = v.Description, Price = v.Price, TypeId = v.TypeId, TypeName = v.TypeName, Discount = v.Discount, MaximumSales = v.MaximumSales, Sales = v.Sales, SalesVisualMeter = v.SalesVisualMeter, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = v.SubImageId, SubImageName = v.SubImageName, SubImageUrl = v.SubImageUrl, Tag = v.Tag, Variance = string.Join("|", variance), FeatureSeq = v.FeatureSeq, FeaturedText = v.FeaturedText, Prize = new InventoryItemModel.PrizeShow() { PrizeId = v.Prize.PrizeId, GameType = v.Prize.GameType, Name = v.Prize.Name, Description = v.Prize.Description, ImageId = v.Prize.ImageId, ImageName = v.Prize.ImageName, ImageUrl = v.Prize.ImageUrl, SubImageId = v.Prize.SubImageId, SubImageName = v.Prize.SubImageName, SubImageUrl = v.Prize.SubImageUrl, Price = v.Prize.Price, SponsorName = v.Prize.SponsorName, Detail = v.Prize.Detail, Terms = v.Prize.Terms, ExpiryDate = v.Prize.ExpiryDate, ExpiryDateStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.Prize.ExpiryDate), GroupName = v.Prize.GroupName, FreeDeal = v.Prize.FreeDeal }, FlashDeal = new InventoryItemModel.FlashDeal() { FlashDealId = v.FlashDeal.FlashDealId, Discount = v.FlashDeal.Discount, MerchantId = v.FlashDeal.MerchantId, MerchantName = v.FlashDeal.MerchantName, InventoryItemId = v.FlashDeal.InventoryItemId, InventoryItemName = v.FlashDeal.InventoryItemName, StartTime = v.FlashDeal.StartTime, StartTimeStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.FlashDeal.StartTime), EndTime = v.FlashDeal.EndTime, EndTimeStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.FlashDeal.EndTime) } });
                }
                else if (v.Prize != null && v.FlashDeal == null)
                {
                    newlistmer.Add(new InventoryItemModel.InventoryItemShow() { InventoryItemId = v.InventoryItemId, Wishlist = v.Wishlist, Name = v.Name, ShortDesc = v.ShortDesc, Description = v.Description, Price = v.Price, TypeId = v.TypeId, TypeName = v.TypeName, Discount = v.Discount, MaximumSales = v.MaximumSales, Sales = v.Sales, SalesVisualMeter = v.SalesVisualMeter, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = v.SubImageId, SubImageName = v.SubImageName, SubImageUrl = v.SubImageUrl, Tag = v.Tag, Variance = string.Join("|", variance), FeatureSeq = v.FeatureSeq, FeaturedText = v.FeaturedText, Prize = new InventoryItemModel.PrizeShow() { PrizeId = v.Prize.PrizeId, GameType = v.Prize.GameType, Name = v.Prize.Name, Description = v.Prize.Description, ImageId = v.Prize.ImageId, ImageName = v.Prize.ImageName, ImageUrl = v.Prize.ImageUrl, SubImageId = v.Prize.SubImageId, SubImageName = v.Prize.SubImageName, SubImageUrl = v.Prize.SubImageUrl, Price = v.Prize.Price, SponsorName = v.Prize.SponsorName, Detail = v.Prize.Detail, Terms = v.Prize.Terms, ExpiryDate = v.Prize.ExpiryDate, ExpiryDateStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.Prize.ExpiryDate), GroupName = v.Prize.GroupName, FreeDeal = v.Prize.FreeDeal } });
                }
                else if (v.Prize == null && v.FlashDeal != null)
                {
                    newlistmer.Add(new InventoryItemModel.InventoryItemShow() { InventoryItemId = v.InventoryItemId, Wishlist = v.Wishlist, Name = v.Name, ShortDesc = v.ShortDesc, Description = v.Description, Price = v.Price, TypeId = v.TypeId, TypeName = v.TypeName, Discount = v.Discount, MaximumSales = v.MaximumSales, Sales = v.Sales, SalesVisualMeter = v.SalesVisualMeter, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = v.SubImageId, SubImageName = v.SubImageName, SubImageUrl = v.SubImageUrl, Tag = v.Tag, Variance = string.Join("|", variance), FeatureSeq = v.FeatureSeq, FeaturedText = v.FeaturedText, FlashDeal = new InventoryItemModel.FlashDeal() { FlashDealId = v.FlashDeal.FlashDealId, Discount = v.FlashDeal.Discount, MerchantId = v.FlashDeal.MerchantId, MerchantName = v.FlashDeal.MerchantName, InventoryItemId = v.FlashDeal.InventoryItemId, InventoryItemName = v.FlashDeal.InventoryItemName, StartTime = v.FlashDeal.StartTime, StartTimeStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.FlashDeal.StartTime), EndTime = v.FlashDeal.EndTime, EndTimeStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.FlashDeal.EndTime) } });
                }
                else
                {
                    newlistmer.Add(new InventoryItemModel.InventoryItemShow() { InventoryItemId = v.InventoryItemId, Wishlist = v.Wishlist, Name = v.Name, ShortDesc = v.ShortDesc, Description = v.Description, Price = v.Price, TypeId = v.TypeId, TypeName = v.TypeName, Discount = v.Discount, MaximumSales = v.MaximumSales, Sales = v.Sales, SalesVisualMeter = v.SalesVisualMeter, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = v.SubImageId, SubImageName = v.SubImageName, SubImageUrl = v.SubImageUrl, Tag = v.Tag, Variance = string.Join("|", variance), FeatureSeq = v.FeatureSeq, FeaturedText = v.FeaturedText });
                }
            }
            InventoryItemModel.InventoryItemVM model = new InventoryItemModel.InventoryItemVM();
            model.item = newlistmer;
            return View(model);
        }
        public ActionResult FlashDeals(int id = 0, string c = "")
        {
            ViewBag.cityid = id;
            ViewBag.category = c;
            DateTime today = DateTime.UtcNow;
            List<com.kuazoo.InventoryItemShow> listmer = inventoryitemservice.GetInventoryItemListByCityFlashDeal(id, today, c).Result;
            List<InventoryItemModel.InventoryItemShow> newlistmer = new List<InventoryItemModel.InventoryItemShow>();
            foreach (var v in listmer)
            {
                if (v.Prize != null && v.FlashDeal != null)
                {
                    newlistmer.Add(new InventoryItemModel.InventoryItemShow() { InventoryItemId = v.InventoryItemId, Wishlist = v.Wishlist, Name = v.Name, ShortDesc = v.ShortDesc, Description = v.Description, Price = v.Price, TypeId = v.TypeId, TypeName = v.TypeName, Discount = v.Discount, MaximumSales = v.MaximumSales, Sales = v.Sales, SalesVisualMeter = v.SalesVisualMeter, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = v.SubImageId, SubImageName = v.SubImageName, SubImageUrl = v.SubImageUrl, Tag = v.Tag, Prize = new InventoryItemModel.PrizeShow() { PrizeId = v.Prize.PrizeId, GameType = v.Prize.GameType, Name = v.Prize.Name, Description = v.Prize.Description, ImageId = v.Prize.ImageId, ImageName = v.Prize.ImageName, ImageUrl = v.Prize.ImageUrl, SubImageId = v.Prize.SubImageId, SubImageName = v.Prize.SubImageName, SubImageUrl = v.Prize.SubImageUrl, Price = v.Prize.Price, SponsorName = v.Prize.SponsorName, Detail = v.Prize.Detail, Terms = v.Prize.Terms, ExpiryDate = v.Prize.ExpiryDate, ExpiryDateStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.Prize.ExpiryDate), GroupName = v.Prize.GroupName, FreeDeal = v.Prize.FreeDeal }, FlashDeal = new InventoryItemModel.FlashDeal() { FlashDealId = v.FlashDeal.FlashDealId, Discount = v.FlashDeal.Discount, MerchantId = v.FlashDeal.MerchantId, MerchantName = v.FlashDeal.MerchantName, InventoryItemId = v.FlashDeal.InventoryItemId, InventoryItemName = v.FlashDeal.InventoryItemName, StartTime = v.FlashDeal.StartTime, StartTimeStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.FlashDeal.StartTime), EndTime = v.FlashDeal.EndTime, EndTimeStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.FlashDeal.EndTime) } });
                }
                else if (v.Prize != null && v.FlashDeal == null)
                {
                    newlistmer.Add(new InventoryItemModel.InventoryItemShow() { InventoryItemId = v.InventoryItemId, Wishlist = v.Wishlist, Name = v.Name, ShortDesc = v.ShortDesc, Description = v.Description, Price = v.Price, TypeId = v.TypeId, TypeName = v.TypeName, Discount = v.Discount, MaximumSales = v.MaximumSales, Sales = v.Sales, SalesVisualMeter = v.SalesVisualMeter, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = v.SubImageId, SubImageName = v.SubImageName, SubImageUrl = v.SubImageUrl, Tag = v.Tag, Prize = new InventoryItemModel.PrizeShow() { PrizeId = v.Prize.PrizeId, GameType = v.Prize.GameType, Name = v.Prize.Name, Description = v.Prize.Description, ImageId = v.Prize.ImageId, ImageName = v.Prize.ImageName, ImageUrl = v.Prize.ImageUrl, SubImageId = v.Prize.SubImageId, SubImageName = v.Prize.SubImageName, SubImageUrl = v.Prize.SubImageUrl, Price = v.Prize.Price, SponsorName = v.Prize.SponsorName, Detail = v.Prize.Detail, Terms = v.Prize.Terms, ExpiryDate = v.Prize.ExpiryDate, ExpiryDateStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.Prize.ExpiryDate), GroupName = v.Prize.GroupName, FreeDeal = v.Prize.FreeDeal } });
                }
                else if (v.Prize == null && v.FlashDeal != null)
                {
                    newlistmer.Add(new InventoryItemModel.InventoryItemShow() { InventoryItemId = v.InventoryItemId, Wishlist = v.Wishlist, Name = v.Name, ShortDesc = v.ShortDesc, Description = v.Description, Price = v.Price, TypeId = v.TypeId, TypeName = v.TypeName, Discount = v.Discount, MaximumSales = v.MaximumSales, Sales = v.Sales, SalesVisualMeter = v.SalesVisualMeter, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = v.SubImageId, SubImageName = v.SubImageName, SubImageUrl = v.SubImageUrl, Tag = v.Tag, FlashDeal = new InventoryItemModel.FlashDeal() { FlashDealId = v.FlashDeal.FlashDealId, Discount = v.FlashDeal.Discount, MerchantId = v.FlashDeal.MerchantId, MerchantName = v.FlashDeal.MerchantName, InventoryItemId = v.FlashDeal.InventoryItemId, InventoryItemName = v.FlashDeal.InventoryItemName, StartTime = v.FlashDeal.StartTime, StartTimeStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.FlashDeal.StartTime), EndTime = v.FlashDeal.EndTime, EndTimeStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.FlashDeal.EndTime) } });
                }
                else
                {
                    newlistmer.Add(new InventoryItemModel.InventoryItemShow() { InventoryItemId = v.InventoryItemId, Wishlist = v.Wishlist, Name = v.Name, ShortDesc = v.ShortDesc, Description = v.Description, Price = v.Price, TypeId = v.TypeId, TypeName = v.TypeName, Discount = v.Discount, MaximumSales = v.MaximumSales, Sales = v.Sales, SalesVisualMeter = v.SalesVisualMeter, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = v.SubImageId, SubImageName = v.SubImageName, SubImageUrl = v.SubImageUrl, Tag = v.Tag });
                }
            }
            InventoryItemModel.InventoryItemVM model = new InventoryItemModel.InventoryItemVM();
            model.item = newlistmer;
            return View(model);
        }
        //public ActionResult Raffles(int id = 0, string c = "")
        //{
        //    ViewBag.cityid = id;
        //    ViewBag.category = c;
        //    DateTime today = DateTime.UtcNow;
        //    List<com.kuazoo.InventoryItemShow> listmer = inventoryitemservice.GetInventoryItemListByCityRaffle(id, today,c).Result;
        //    List<InventoryItemModel.InventoryItemShow> newlistmer = new List<InventoryItemModel.InventoryItemShow>();
        //    foreach (var v in listmer)
        //    {
        //        if (v.Prize != null && v.FlashDeal != null)
        //        {
        //            newlistmer.Add(new InventoryItemModel.InventoryItemShow() { InventoryItemId = v.InventoryItemId, Wishlist = v.Wishlist, Name = v.Name, ShortDesc = v.ShortDesc, Description = v.Description, Price = v.Price, TypeId = v.TypeId, TypeName = v.TypeName, Discount = v.Discount, MaximumSales = v.MaximumSales, Sales = v.Sales, SalesVisualMeter = v.SalesVisualMeter, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = v.SubImageId, SubImageName = v.SubImageName, SubImageUrl = v.SubImageUrl, Tag = v.Tag, Prize = new InventoryItemModel.PrizeShow() { PrizeId = v.Prize.PrizeId, GameType=v.Prize.GameType, Name = v.Prize.Name, Description = v.Prize.Description, ImageId = v.Prize.ImageId, ImageName = v.Prize.ImageName, ImageUrl = v.Prize.ImageUrl, SubImageId = v.Prize.SubImageId, SubImageName = v.Prize.SubImageName, SubImageUrl = v.Prize.SubImageUrl, Price = v.Prize.Price, SponsorName = v.Prize.SponsorName, Detail = v.Prize.Detail, Terms = v.Prize.Terms, ExpiryDate = v.Prize.ExpiryDate, ExpiryDateStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.Prize.ExpiryDate), GroupName = v.Prize.GroupName, FreeDeal = v.Prize.FreeDeal }, FlashDeal = new InventoryItemModel.FlashDeal() { FlashDealId = v.FlashDeal.FlashDealId, Discount = v.FlashDeal.Discount, MerchantId = v.FlashDeal.MerchantId, MerchantName = v.FlashDeal.MerchantName, InventoryItemId = v.FlashDeal.InventoryItemId, InventoryItemName = v.FlashDeal.InventoryItemName, StartTime = v.FlashDeal.StartTime, StartTimeStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.FlashDeal.StartTime), EndTime = v.FlashDeal.EndTime, EndTimeStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.FlashDeal.EndTime) } });
        //        }
        //        else if (v.Prize != null && v.FlashDeal == null)
        //        {
        //            newlistmer.Add(new InventoryItemModel.InventoryItemShow() { InventoryItemId = v.InventoryItemId, Wishlist = v.Wishlist, Name = v.Name, ShortDesc = v.ShortDesc, Description = v.Description, Price = v.Price, TypeId = v.TypeId, TypeName = v.TypeName, Discount = v.Discount, MaximumSales = v.MaximumSales, Sales = v.Sales, SalesVisualMeter = v.SalesVisualMeter, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = v.SubImageId, SubImageName = v.SubImageName, SubImageUrl = v.SubImageUrl, Tag = v.Tag, Prize = new InventoryItemModel.PrizeShow() { PrizeId = v.Prize.PrizeId, GameType=v.Prize.GameType, Name = v.Prize.Name, Description = v.Prize.Description, ImageId = v.Prize.ImageId, ImageName = v.Prize.ImageName, ImageUrl = v.Prize.ImageUrl, SubImageId = v.Prize.SubImageId, SubImageName = v.Prize.SubImageName, SubImageUrl = v.Prize.SubImageUrl, Price = v.Prize.Price, SponsorName = v.Prize.SponsorName, Detail = v.Prize.Detail, Terms = v.Prize.Terms, ExpiryDate = v.Prize.ExpiryDate, ExpiryDateStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.Prize.ExpiryDate), GroupName = v.Prize.GroupName, FreeDeal = v.Prize.FreeDeal } });
        //        }
        //        else if (v.Prize == null && v.FlashDeal != null)
        //        {
        //            newlistmer.Add(new InventoryItemModel.InventoryItemShow() { InventoryItemId = v.InventoryItemId, Wishlist = v.Wishlist, Name = v.Name, ShortDesc = v.ShortDesc, Description = v.Description, Price = v.Price, TypeId = v.TypeId, TypeName = v.TypeName, Discount = v.Discount, MaximumSales = v.MaximumSales, Sales = v.Sales, SalesVisualMeter = v.SalesVisualMeter, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = v.SubImageId, SubImageName = v.SubImageName, SubImageUrl = v.SubImageUrl, Tag = v.Tag, FlashDeal = new InventoryItemModel.FlashDeal() { FlashDealId = v.FlashDeal.FlashDealId, Discount = v.FlashDeal.Discount, MerchantId = v.FlashDeal.MerchantId, MerchantName = v.FlashDeal.MerchantName, InventoryItemId = v.FlashDeal.InventoryItemId, InventoryItemName = v.FlashDeal.InventoryItemName, StartTime = v.FlashDeal.StartTime, StartTimeStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.FlashDeal.StartTime), EndTime = v.FlashDeal.EndTime, EndTimeStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.FlashDeal.EndTime) } });
        //        }
        //        else
        //        {
        //            newlistmer.Add(new InventoryItemModel.InventoryItemShow() { InventoryItemId = v.InventoryItemId, Wishlist = v.Wishlist, Name = v.Name, ShortDesc = v.ShortDesc, Description = v.Description, Price = v.Price, TypeId = v.TypeId, TypeName = v.TypeName, Discount = v.Discount, MaximumSales = v.MaximumSales, Sales = v.Sales, SalesVisualMeter = v.SalesVisualMeter, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = v.SubImageId, SubImageName = v.SubImageName, SubImageUrl = v.SubImageUrl, Tag = v.Tag });
        //        }
        //    }
        //    InventoryItemModel.InventoryItemVM model = new InventoryItemModel.InventoryItemVM();
        //    model.item = newlistmer;
        //    return View(model);
        //}
        //public ActionResult DealDetail(int id = 0)
        //{
        //    DateTime today = DateTime.UtcNow;
        //    com.kuazoo.InventoryItemShow v = inventoryitemservice.GetInventoryItemDetail(id, today).Result;
        //    InventoryItemModel.InventoryItemShow model = new InventoryItemModel.InventoryItemShow();
        //    if (v != null && v.InventoryItemId != null && v.InventoryItemId != 0)
        //    {
        //        var reviewcount = reviewservice.GetReviewCount(v.InventoryItemId).Result;
        //        ReviewVMCount reviewvmcount = new ReviewVMCount();
        //        float rating = 0;
        //        if (reviewcount.Rating != 0 && reviewcount.ReviewCount != 0)
        //        {
        //            rating = (float)reviewcount.Rating / (float)reviewcount.ReviewCount;
        //            if (rating > 0)
        //            {
        //                rating = rating / 2;
        //            }

        //        }
        //        reviewvmcount.Rating = rating;
        //        reviewvmcount.ReviewCount = reviewcount.ReviewCount;

        //        if (v.Prize != null && v.FlashDeal != null)
        //        {
        //            model = new InventoryItemModel.InventoryItemShow() { InventoryItemId = v.InventoryItemId, Name = v.Name, ShortDesc=v.ShortDesc, Price = v.Price, TypeId = v.TypeId, TypeName = v.TypeName, Discount = v.Discount, MaximumSales = v.MaximumSales, Sales = v.Sales, SalesVisualMeter = v.SalesVisualMeter, ExpireDate = v.ExpireDate, ExpireDateStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.ExpireDate), ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = v.SubImageId, SubImageName = v.SubImageName, SubImageUrl = v.SubImageUrl, Prize = new InventoryItemModel.PrizeShow() { PrizeId = v.Prize.PrizeId, GameType=v.Prize.GameType, Name = v.Prize.Name, Description = v.Prize.Description, ImageId = v.Prize.ImageId, ImageName = v.Prize.ImageName, ImageUrl = v.Prize.ImageUrl, SubImageId = v.Prize.SubImageId, SubImageName = v.Prize.SubImageName, SubImageUrl = v.Prize.SubImageUrl, Price = v.Prize.Price, SponsorName = v.Prize.SponsorName, Detail = v.Prize.Detail, Terms = v.Prize.Terms, ExpiryDate = v.Prize.ExpiryDate, ExpiryDateStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.Prize.ExpiryDate), GroupName = v.Prize.GroupName, FreeDeal = v.Prize.FreeDeal }, FlashDeal = new InventoryItemModel.FlashDeal() { FlashDealId = v.FlashDeal.FlashDealId, Discount = v.FlashDeal.Discount, MerchantId = v.FlashDeal.MerchantId, MerchantName = v.FlashDeal.MerchantName, InventoryItemId = v.FlashDeal.InventoryItemId, InventoryItemName = v.FlashDeal.InventoryItemName, StartTime = v.FlashDeal.StartTime, StartTimeStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.FlashDeal.StartTime), EndTime = v.FlashDeal.EndTime, EndTimeStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.FlashDeal.EndTime) }, Description = v.Description, AddressLine1 = v.AddressLine1, AddressLine2 = v.AddressLine2, MerchantCityName = v.MerchantCityName, MerchantCountryName = v.MerchantCountryName, PostCode = v.PostCode, Latitude = v.Latitude, Longitude = v.Longitude, Tag = v.Tag, MerchantName = v.MerchantName, ReviewCount = reviewvmcount };
        //        }
        //        else if (v.Prize != null && v.FlashDeal == null)
        //        {
        //            model = new InventoryItemModel.InventoryItemShow() { InventoryItemId = v.InventoryItemId, Name = v.Name, ShortDesc=v.ShortDesc, Price = v.Price, TypeId = v.TypeId, TypeName = v.TypeName, Discount = v.Discount, MaximumSales = v.MaximumSales, Sales = v.Sales, SalesVisualMeter = v.SalesVisualMeter, ExpireDate = v.ExpireDate, ExpireDateStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.ExpireDate), ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = v.SubImageId, SubImageName = v.SubImageName, SubImageUrl = v.SubImageUrl, Prize = new InventoryItemModel.PrizeShow() { PrizeId = v.Prize.PrizeId, GameType=v.Prize.GameType, Name = v.Prize.Name, Description = v.Prize.Description, ImageId = v.Prize.ImageId, ImageName = v.Prize.ImageName, ImageUrl = v.Prize.ImageUrl, SubImageId = v.Prize.SubImageId, SubImageName = v.Prize.SubImageName, SubImageUrl = v.Prize.SubImageUrl, Price = v.Prize.Price, SponsorName = v.Prize.SponsorName, Detail = v.Prize.Detail, Terms = v.Prize.Terms, ExpiryDate = v.Prize.ExpiryDate, ExpiryDateStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.Prize.ExpiryDate), GroupName = v.Prize.GroupName, FreeDeal = v.Prize.FreeDeal }, Description = v.Description, AddressLine1 = v.AddressLine1, AddressLine2 = v.AddressLine2, MerchantCityName = v.MerchantCityName, MerchantCountryName = v.MerchantCountryName, PostCode = v.PostCode, Latitude = v.Latitude, Longitude = v.Longitude, Tag = v.Tag, MerchantName = v.MerchantName, ReviewCount = reviewvmcount };
        //        }
        //        else if (v.Prize == null && v.FlashDeal != null)
        //        {
        //            model = new InventoryItemModel.InventoryItemShow() { InventoryItemId = v.InventoryItemId, Name = v.Name, ShortDesc=v.ShortDesc, Price = v.Price, TypeId = v.TypeId, TypeName = v.TypeName, Discount = v.Discount, MaximumSales = v.MaximumSales, Sales = v.Sales, SalesVisualMeter = v.SalesVisualMeter, ExpireDate = v.ExpireDate, ExpireDateStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.ExpireDate), ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = v.SubImageId, SubImageName = v.SubImageName, SubImageUrl = v.SubImageUrl, FlashDeal = new InventoryItemModel.FlashDeal() { FlashDealId = v.FlashDeal.FlashDealId, Discount = v.FlashDeal.Discount, MerchantId = v.FlashDeal.MerchantId, MerchantName = v.FlashDeal.MerchantName, InventoryItemId = v.FlashDeal.InventoryItemId, InventoryItemName = v.FlashDeal.InventoryItemName, StartTime = v.FlashDeal.StartTime, StartTimeStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.FlashDeal.StartTime), EndTime = v.FlashDeal.EndTime, EndTimeStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.FlashDeal.EndTime) }, Description = v.Description, AddressLine1 = v.AddressLine1, AddressLine2 = v.AddressLine2, MerchantCityName = v.MerchantCityName, MerchantCountryName = v.MerchantCountryName, PostCode = v.PostCode, Latitude = v.Latitude, Longitude = v.Longitude, Tag = v.Tag, MerchantName = v.MerchantName, ReviewCount = reviewvmcount };
        //        }
        //        else
        //        {
        //            model = new InventoryItemModel.InventoryItemShow() { InventoryItemId = v.InventoryItemId, Name = v.Name, ShortDesc=v.ShortDesc, Price = v.Price, TypeId = v.TypeId, TypeName = v.TypeName, Discount = v.Discount, MaximumSales = v.MaximumSales, Sales = v.Sales, SalesVisualMeter = v.SalesVisualMeter, ExpireDate = v.ExpireDate, ExpireDateStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.ExpireDate), ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = v.SubImageId, SubImageName = v.SubImageName, SubImageUrl = v.SubImageUrl, Description = v.Description, AddressLine1 = v.AddressLine1, AddressLine2 = v.AddressLine2, MerchantCityName = v.MerchantCityName, MerchantCountryName = v.MerchantCountryName, PostCode = v.PostCode, Latitude = v.Latitude, Longitude = v.Longitude, Tag = v.Tag, MerchantName = v.MerchantName,ReviewCount = reviewvmcount};
        //        }

        //    }
        //    else
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }
        //    return View(model);
        //}
        public ActionResult Deals(String id)
        {
            DateTime today = DateTime.UtcNow;
            com.kuazoo.InventoryItemShow v = inventoryitemservice.GetInventoryItemDetailByName(id, today).Result;
            InventoryItemModel.InventoryItemShow model = new InventoryItemModel.InventoryItemShow();
            if (v != null && v.InventoryItemId != null && v.InventoryItemId != 0)
            {
                var reviewcount = reviewservice.GetReviewCount(v.InventoryItemId).Result;
                ReviewVMCount reviewvmcount = new ReviewVMCount();
                float rating = 0;
                if (reviewcount.Rating != 0 && reviewcount.ReviewCount != 0)
                {
                    rating = (float)reviewcount.Rating / (float)reviewcount.ReviewCount;
                    if (rating > 0)
                    {
                        rating = rating / 2;
                    }

                }
                reviewvmcount.Rating = rating;
                reviewvmcount.ReviewCount = reviewcount.ReviewCount;

                List<string> variance = new List<string>();
                //bool checkvar = true;
                //if (v.Variance.Count == 1)
                //{
                //    if (v.Variance[0].Price == v.Price && v.Variance[0].Discount == v.Discount)
                //    {
                //        checkvar = false;
                //    }
                //}
                //if (checkvar)
                //{
                if (v.FlashDeal == null)
                {
                    foreach (var vari in v.Variance)
                    {
                        variance.Add(vari.Variance + "`" + vari.Price + "`" + vari.Discount + "`" + vari.AvailabelLimit);
                    }
                }
                else
                {
                    foreach (var vari in v.Variance)
                    {
                        variance.Add(vari.Variance + "`" + vari.Price + "`" + (vari.Discount+v.FlashDeal.Discount) + "`" + vari.AvailabelLimit);
                    }
                }
                //}
                if (v.Prize != null && v.FlashDeal != null)
                {
                    model = new InventoryItemModel.InventoryItemShow() { InventoryItemId = v.InventoryItemId, Name = v.Name, ShortDesc = v.ShortDesc, GeneralDescription = v.GeneralDescription, Terms = v.Terms, Price = v.Price, TypeId = v.TypeId, TypeName = v.TypeName, Discount = v.Discount, MaximumSales = v.MaximumSales, Sales = v.Sales, SalesVisualMeter = v.SalesVisualMeter, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = v.SubImageId, SubImageName = v.SubImageName, SubImageUrl = v.SubImageUrl, Variance = string.Join("|", variance), MerchantContactNumber = v.MerchantContactNumber, MerchantEmail = v.MerchantEmail, MerchantFacebook = v.MerchantFacebook, MerchantWebsite = v.MerchantWebsite, MerchantDescription = v.MerchantDescription, MerchantSubImageId = v.MerchantSubImageId, MerchantSubImageName = v.MerchantSubImageName, MerchantSubImageUrl = v.MerchantSubImageUrl, MerchantSubImageUrlLink = v.MerchantSubImageUrlLink, Prize = new InventoryItemModel.PrizeShow() { PrizeId = v.Prize.PrizeId, GameType = v.Prize.GameType, Name = v.Prize.Name, Description = v.Prize.Description, ImageId = v.Prize.ImageId, ImageName = v.Prize.ImageName, ImageUrl = v.Prize.ImageUrl, SubImageId = v.Prize.SubImageId, SubImageName = v.Prize.SubImageName, SubImageUrl = v.Prize.SubImageUrl, Price = v.Prize.Price, SponsorName = v.Prize.SponsorName, Detail = v.Prize.Detail, Terms = v.Prize.Terms, ExpiryDate = v.Prize.ExpiryDate, ExpiryDateStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.Prize.ExpiryDate), GroupName = v.Prize.GroupName, FreeDeal = v.Prize.FreeDeal }, FlashDeal = new InventoryItemModel.FlashDeal() { FlashDealId = v.FlashDeal.FlashDealId, Discount = v.FlashDeal.Discount, MerchantId = v.FlashDeal.MerchantId, MerchantName = v.FlashDeal.MerchantName, InventoryItemId = v.FlashDeal.InventoryItemId, InventoryItemName = v.FlashDeal.InventoryItemName, StartTime = v.FlashDeal.StartTime, StartTimeStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.FlashDeal.StartTime), EndTime = v.FlashDeal.EndTime, EndTimeStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.FlashDeal.EndTime) }, Description = v.Description, AddressLine1 = v.AddressLine1, AddressLine2 = v.AddressLine2, MerchantCityName = v.MerchantCityName, MerchantCountryName = v.MerchantCountryName, PostCode = v.PostCode, Latitude = v.Latitude, Longitude = v.Longitude, Tag = v.Tag, MerchantName = v.MerchantName, ReviewCount = reviewvmcount };
                }
                else if (v.Prize != null && v.FlashDeal == null)
                {
                    model = new InventoryItemModel.InventoryItemShow() { InventoryItemId = v.InventoryItemId, Name = v.Name, ShortDesc = v.ShortDesc, GeneralDescription = v.GeneralDescription, Terms = v.Terms, Price = v.Price, TypeId = v.TypeId, TypeName = v.TypeName, Discount = v.Discount, MaximumSales = v.MaximumSales, Sales = v.Sales, SalesVisualMeter = v.SalesVisualMeter, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = v.SubImageId, SubImageName = v.SubImageName, SubImageUrl = v.SubImageUrl, Variance = string.Join("|", variance), MerchantContactNumber = v.MerchantContactNumber, MerchantEmail = v.MerchantEmail, MerchantFacebook = v.MerchantFacebook, MerchantWebsite = v.MerchantWebsite, MerchantDescription = v.MerchantDescription, MerchantSubImageId = v.MerchantSubImageId, MerchantSubImageName = v.MerchantSubImageName, MerchantSubImageUrl = v.MerchantSubImageUrl, MerchantSubImageUrlLink = v.MerchantSubImageUrlLink, Prize = new InventoryItemModel.PrizeShow() { PrizeId = v.Prize.PrizeId, GameType = v.Prize.GameType, Name = v.Prize.Name, Description = v.Prize.Description, ImageId = v.Prize.ImageId, ImageName = v.Prize.ImageName, ImageUrl = v.Prize.ImageUrl, SubImageId = v.Prize.SubImageId, SubImageName = v.Prize.SubImageName, SubImageUrl = v.Prize.SubImageUrl, Price = v.Prize.Price, SponsorName = v.Prize.SponsorName, Detail = v.Prize.Detail, Terms = v.Prize.Terms, ExpiryDate = v.Prize.ExpiryDate, ExpiryDateStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.Prize.ExpiryDate), GroupName = v.Prize.GroupName, FreeDeal = v.Prize.FreeDeal }, Description = v.Description, AddressLine1 = v.AddressLine1, AddressLine2 = v.AddressLine2, MerchantCityName = v.MerchantCityName, MerchantCountryName = v.MerchantCountryName, PostCode = v.PostCode, Latitude = v.Latitude, Longitude = v.Longitude, Tag = v.Tag, MerchantName = v.MerchantName, ReviewCount = reviewvmcount };
                }
                else if (v.Prize == null && v.FlashDeal != null)
                {
                    model = new InventoryItemModel.InventoryItemShow() { InventoryItemId = v.InventoryItemId, Name = v.Name, ShortDesc = v.ShortDesc, GeneralDescription = v.GeneralDescription, Terms = v.Terms, Price = v.Price, TypeId = v.TypeId, TypeName = v.TypeName, Discount = v.Discount, MaximumSales = v.MaximumSales, Sales = v.Sales, SalesVisualMeter = v.SalesVisualMeter, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = v.SubImageId, SubImageName = v.SubImageName, SubImageUrl = v.SubImageUrl, Variance = string.Join("|", variance), MerchantContactNumber = v.MerchantContactNumber, MerchantEmail = v.MerchantEmail, MerchantFacebook = v.MerchantFacebook, MerchantWebsite = v.MerchantWebsite, MerchantDescription = v.MerchantDescription, MerchantSubImageId = v.MerchantSubImageId, MerchantSubImageName = v.MerchantSubImageName, MerchantSubImageUrl = v.MerchantSubImageUrl, MerchantSubImageUrlLink = v.MerchantSubImageUrlLink, FlashDeal = new InventoryItemModel.FlashDeal() { FlashDealId = v.FlashDeal.FlashDealId, Discount = v.FlashDeal.Discount, MerchantId = v.FlashDeal.MerchantId, MerchantName = v.FlashDeal.MerchantName, InventoryItemId = v.FlashDeal.InventoryItemId, InventoryItemName = v.FlashDeal.InventoryItemName, StartTime = v.FlashDeal.StartTime, StartTimeStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.FlashDeal.StartTime), EndTime = v.FlashDeal.EndTime, EndTimeStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.FlashDeal.EndTime) }, Description = v.Description, AddressLine1 = v.AddressLine1, AddressLine2 = v.AddressLine2, MerchantCityName = v.MerchantCityName, MerchantCountryName = v.MerchantCountryName, PostCode = v.PostCode, Latitude = v.Latitude, Longitude = v.Longitude, Tag = v.Tag, MerchantName = v.MerchantName, ReviewCount = reviewvmcount };
                }
                else
                {
                    model = new InventoryItemModel.InventoryItemShow() { InventoryItemId = v.InventoryItemId, Name = v.Name, ShortDesc = v.ShortDesc, GeneralDescription = v.GeneralDescription, Terms = v.Terms, Price = v.Price, TypeId = v.TypeId, TypeName = v.TypeName, Discount = v.Discount, MaximumSales = v.MaximumSales, Sales = v.Sales, SalesVisualMeter = v.SalesVisualMeter, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = v.SubImageId, SubImageName = v.SubImageName, SubImageUrl = v.SubImageUrl, Variance = string.Join("|", variance), MerchantContactNumber = v.MerchantContactNumber, MerchantEmail = v.MerchantEmail, MerchantFacebook = v.MerchantFacebook, MerchantWebsite = v.MerchantWebsite, MerchantDescription = v.MerchantDescription, MerchantSubImageId = v.MerchantSubImageId, MerchantSubImageName = v.MerchantSubImageName, MerchantSubImageUrl = v.MerchantSubImageUrl, MerchantSubImageUrlLink = v.MerchantSubImageUrlLink, Description = v.Description, AddressLine1 = v.AddressLine1, AddressLine2 = v.AddressLine2, MerchantCityName = v.MerchantCityName, MerchantCountryName = v.MerchantCountryName, PostCode = v.PostCode, Latitude = v.Latitude, Longitude = v.Longitude, Tag = v.Tag, MerchantName = v.MerchantName, ReviewCount = reviewvmcount };
                }

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }
        public ActionResult Review(int id = 0)
        {
            ReviewModel model = new ReviewModel();

            //int gmt = int.Parse(ConfigurationManager.AppSettings["GMT"]);
            var inventory = inventoryitemservice.GetInventoryItemById(id).Result;
            try
            {
                var user = userservice.GetCurrentUser().Result;
                model.MemberFullName = user.FirstName + " " + user.LastName;
            }
            catch (Exception ex)
            {
                CustomException(ex);
                model.MemberFullName = "";
            }

            List<com.kuazoo.Review> reviewlist = reviewservice.GetReviewByInventoryItemId(id).Result;
            model.InventoryItemId = id;
            model.InventoryItemName = inventory.Name;
            List<ReviewVM> list = new List<ReviewVM>();
            foreach (var v in reviewlist)
            {
                //list.Add(new ReviewVM() { ReviewId = v.ReviewId, InventoryItemId = v.InventoryItemId, InventoryItemName = v.InventoryItemName, MemberEmail = v.MemberEmail, MemberId = v.MemberId, MemberFullName = v.MemberFullName, MemberImage = v.MemberImage, Message = v.Message, ReviewDate = v.ReviewDate.AddHours(gmt), ReviewDateStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.ReviewDate.AddHours(gmt)), Rating = v.Rating });
                list.Add(new ReviewVM() { ReviewId = v.ReviewId, InventoryItemId = v.InventoryItemId, InventoryItemName = v.InventoryItemName, MemberEmail = v.MemberEmail, MemberId = v.MemberId, MemberFullName = v.MemberFullName, MemberImage = v.MemberImage, Message = v.Message, ReviewDate = v.ReviewDate, ReviewDateStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.ReviewDate), Rating = v.Rating });
            }
            model.ReviewList = list;
            return View(model);
        }

        [HttpPost]
        public JsonResult ReviewAdd(int id, int rating, string message)
        {
            try
            {
                //int gmt = int.Parse(ConfigurationManager.AppSettings["GMT"]);
                int userid = userservice.GetCurrentUser().Result.UserId;
                com.kuazoo.Review rev = new com.kuazoo.Review();
                rev.InventoryItemId = id;
                rev.Rating = rating;
                rev.Message = message;
                rev.MemberId = userid;
                var result = reviewservice.CreateReview(rev).Result;
                if (result.ReviewId != null && result.ReviewId != 0)
                {
                    rev.ReviewId = result.ReviewId;
                    //rev.ReviewDate = result.ReviewDate.AddHours(gmt);
                    //rev.ReviewDateStr = string.Format("{0:dd MMM yyyy hh:mm tt}", result.ReviewDate.AddHours(gmt));
                    rev.ReviewDate = result.ReviewDate;
                    rev.ReviewDateStr = string.Format("{0:dd MMM yyyy hh:mm tt}", result.ReviewDate);
                    rev.MemberImage = result.MemberImage;
                    rev.MemberId = result.MemberId;
                    rev.MemberFullName = result.MemberFullName;
                    rev.MemberEmail = result.MemberEmail;
                    return Json(new { success = true, error = "", result = rev }, JsonRequestBehavior.AllowGet);
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
        [HttpGet]
        public JsonResult ReviewCount(int id)
        {
            try
            {
                var reviewcount = reviewservice.GetReviewCount(id).Result;
                ReviewVMCount reviewvmcount = new ReviewVMCount();
                float rating = 0;
                if (reviewcount.Rating != 0 && reviewcount.ReviewCount != 0)
                {
                    rating = (float)reviewcount.Rating / (float)reviewcount.ReviewCount;
                    if (rating > 0)
                    {
                        rating = rating / 2;
                    }

                }
                reviewvmcount.Rating = rating;
                reviewvmcount.ReviewCount = reviewcount.ReviewCount;
                return Json(new { success = true, error = "", result = reviewvmcount }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CustomException(ex);
                return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Wishlist()
        {
            return View();
        }
        [HttpGet]
        public ActionResult GetWishList(int id)
        {
            DateTime today = DateTime.UtcNow;
            com.kuazoo.InventoryItemShow v = inventoryitemservice.GetInventoryItemDetail(id, today).Result;
            InventoryItemModel.InventoryItemShow model = new InventoryItemModel.InventoryItemShow();
            if (v != null && v.InventoryItemId != null && v.InventoryItemId != 0)
            {
                List<string> variance = new List<string>();
                //bool checkvar = true;
                //if (v.Variance.Count == 1)
                //{
                //    if (v.Variance[0].Price == v.Price && v.Variance[0].Discount == v.Discount)
                //    {
                //        checkvar = false;
                //    }
                //}
                //if (checkvar)
                //{
                foreach (var vari in v.Variance)
                {
                    variance.Add(vari.Variance + "`" + vari.Price + "`" + vari.Discount + "`" + vari.AvailabelLimit);
                }
                //}
                if (v.Prize != null && v.FlashDeal != null)
                {
                    model = new InventoryItemModel.InventoryItemShow() { WishlistId = 0, InventoryItemId = v.InventoryItemId, Name = v.Name, ShortDesc = v.ShortDesc, Price = v.Price, TypeId = v.TypeId, TypeName = v.TypeName, Discount = v.Discount, MaximumSales = v.MaximumSales, Sales = v.Sales, SalesVisualMeter = v.SalesVisualMeter, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = v.SubImageId, SubImageName = v.SubImageName, SubImageUrl = v.SubImageUrl, Variance = string.Join("|", variance), Prize = new InventoryItemModel.PrizeShow() { PrizeId = v.Prize.PrizeId, GameType = v.Prize.GameType, Name = v.Prize.Name, Description = v.Prize.Description, ImageId = v.Prize.ImageId, ImageName = v.Prize.ImageName, ImageUrl = v.Prize.ImageUrl, SubImageId = v.Prize.SubImageId, SubImageName = v.Prize.SubImageName, SubImageUrl = v.Prize.SubImageUrl, Price = v.Prize.Price, SponsorName = v.Prize.SponsorName, Detail = v.Prize.Detail, Terms = v.Prize.Terms, ExpiryDate = v.Prize.ExpiryDate, ExpiryDateStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.Prize.ExpiryDate), GroupName = v.Prize.GroupName, FreeDeal = v.Prize.FreeDeal }, FlashDeal = new InventoryItemModel.FlashDeal() { FlashDealId = v.FlashDeal.FlashDealId, Discount = v.FlashDeal.Discount, MerchantId = v.FlashDeal.MerchantId, MerchantName = v.FlashDeal.MerchantName, InventoryItemId = v.FlashDeal.InventoryItemId, InventoryItemName = v.FlashDeal.InventoryItemName, StartTime = v.FlashDeal.StartTime, StartTimeStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.FlashDeal.StartTime), EndTime = v.FlashDeal.EndTime, EndTimeStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.FlashDeal.EndTime) } };
                }
                else if (v.Prize != null && v.FlashDeal == null)
                {
                    model = new InventoryItemModel.InventoryItemShow() { WishlistId = 0, InventoryItemId = v.InventoryItemId, Name = v.Name, ShortDesc = v.ShortDesc, Price = v.Price, TypeId = v.TypeId, TypeName = v.TypeName, Discount = v.Discount, MaximumSales = v.MaximumSales, Sales = v.Sales, SalesVisualMeter = v.SalesVisualMeter, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = v.SubImageId, SubImageName = v.SubImageName, SubImageUrl = v.SubImageUrl, Variance = string.Join("|", variance), Prize = new InventoryItemModel.PrizeShow() { PrizeId = v.Prize.PrizeId, GameType = v.Prize.GameType, Name = v.Prize.Name, Description = v.Prize.Description, ImageId = v.Prize.ImageId, ImageName = v.Prize.ImageName, ImageUrl = v.Prize.ImageUrl, SubImageId = v.Prize.SubImageId, SubImageName = v.Prize.SubImageName, SubImageUrl = v.Prize.SubImageUrl, Price = v.Prize.Price, SponsorName = v.Prize.SponsorName, Detail = v.Prize.Detail, Terms = v.Prize.Terms, ExpiryDate = v.Prize.ExpiryDate, ExpiryDateStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.Prize.ExpiryDate), GroupName = v.Prize.GroupName, FreeDeal = v.Prize.FreeDeal } };
                }
                else if (v.Prize == null && v.FlashDeal != null)
                {
                    model = new InventoryItemModel.InventoryItemShow() { WishlistId = 0, InventoryItemId = v.InventoryItemId, Name = v.Name, ShortDesc = v.ShortDesc, Price = v.Price, TypeId = v.TypeId, TypeName = v.TypeName, Discount = v.Discount, MaximumSales = v.MaximumSales, Sales = v.Sales, SalesVisualMeter = v.SalesVisualMeter, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = v.SubImageId, SubImageName = v.SubImageName, SubImageUrl = v.SubImageUrl, Variance = string.Join("|", variance), FlashDeal = new InventoryItemModel.FlashDeal() { FlashDealId = v.FlashDeal.FlashDealId, Discount = v.FlashDeal.Discount, MerchantId = v.FlashDeal.MerchantId, MerchantName = v.FlashDeal.MerchantName, InventoryItemId = v.FlashDeal.InventoryItemId, InventoryItemName = v.FlashDeal.InventoryItemName, StartTime = v.FlashDeal.StartTime, StartTimeStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.FlashDeal.StartTime), EndTime = v.FlashDeal.EndTime, EndTimeStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.FlashDeal.EndTime) } };
                }
                else
                {
                    model = new InventoryItemModel.InventoryItemShow() { WishlistId = 0, InventoryItemId = v.InventoryItemId, Name = v.Name, ShortDesc = v.ShortDesc, Price = v.Price, TypeId = v.TypeId, TypeName = v.TypeName, Discount = v.Discount, MaximumSales = v.MaximumSales, Sales = v.Sales, SalesVisualMeter = v.SalesVisualMeter, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = v.SubImageId, SubImageName = v.SubImageName, SubImageUrl = v.SubImageUrl, Variance = string.Join("|", variance) };
                }
            }
            return PartialView("Partial/_PartialWishList", model);
        }
        [HttpPost]
        public JsonResult WishlistAdd(int id)
        {
            try
            {
                var result = inventoryitemservice.CreateWishlist(id).Result;
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


        public ActionResult Search(string t)
        {
            DateTime today = DateTime.UtcNow;
            List<com.kuazoo.InventoryItemShow> listmer = inventoryitemservice.GetInventoryItemListBySearch(t, today).Result;
            List<InventoryItemModel.InventoryItemShow> newlistmer = new List<InventoryItemModel.InventoryItemShow>();
            foreach (var v in listmer)
            {
                if (v.Prize != null && v.FlashDeal != null)
                {
                    newlistmer.Add(new InventoryItemModel.InventoryItemShow() { InventoryItemId = v.InventoryItemId, Name = v.Name, ShortDesc = v.ShortDesc, Price = v.Price, TypeId = v.TypeId, TypeName = v.TypeName, Discount = v.Discount, MaximumSales = v.MaximumSales, Sales = v.Sales, SalesVisualMeter = v.SalesVisualMeter, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = v.SubImageId, SubImageName = v.SubImageName, SubImageUrl = v.SubImageUrl, Prize = new InventoryItemModel.PrizeShow() { PrizeId = v.Prize.PrizeId, GameType = v.Prize.GameType, Name = v.Prize.Name, Description = v.Prize.Description, ImageId = v.Prize.ImageId, ImageName = v.Prize.ImageName, ImageUrl = v.Prize.ImageUrl, SubImageId = v.Prize.SubImageId, SubImageName = v.Prize.SubImageName, SubImageUrl = v.Prize.SubImageUrl, Price = v.Prize.Price, SponsorName = v.Prize.SponsorName, Detail = v.Prize.Detail, Terms = v.Prize.Terms, ExpiryDate = v.Prize.ExpiryDate, ExpiryDateStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.Prize.ExpiryDate), GroupName = v.Prize.GroupName, FreeDeal = v.Prize.FreeDeal }, FlashDeal = new InventoryItemModel.FlashDeal() { FlashDealId = v.FlashDeal.FlashDealId, Discount = v.FlashDeal.Discount, MerchantId = v.FlashDeal.MerchantId, MerchantName = v.FlashDeal.MerchantName, InventoryItemId = v.FlashDeal.InventoryItemId, InventoryItemName = v.FlashDeal.InventoryItemName, StartTime = v.FlashDeal.StartTime, StartTimeStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.FlashDeal.StartTime), EndTime = v.FlashDeal.EndTime, EndTimeStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.FlashDeal.EndTime) } });
                }
                else if (v.Prize != null && v.FlashDeal == null)
                {
                    newlistmer.Add(new InventoryItemModel.InventoryItemShow() { InventoryItemId = v.InventoryItemId, Name = v.Name, ShortDesc = v.ShortDesc, Price = v.Price, TypeId = v.TypeId, TypeName = v.TypeName, Discount = v.Discount, MaximumSales = v.MaximumSales, Sales = v.Sales, SalesVisualMeter = v.SalesVisualMeter, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = v.SubImageId, SubImageName = v.SubImageName, SubImageUrl = v.SubImageUrl, Prize = new InventoryItemModel.PrizeShow() { PrizeId = v.Prize.PrizeId, GameType = v.Prize.GameType, Name = v.Prize.Name, Description = v.Prize.Description, ImageId = v.Prize.ImageId, ImageName = v.Prize.ImageName, ImageUrl = v.Prize.ImageUrl, SubImageId = v.Prize.SubImageId, SubImageName = v.Prize.SubImageName, SubImageUrl = v.Prize.SubImageUrl, Price = v.Prize.Price, SponsorName = v.Prize.SponsorName, Detail = v.Prize.Detail, Terms = v.Prize.Terms, ExpiryDate = v.Prize.ExpiryDate, ExpiryDateStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.Prize.ExpiryDate), GroupName = v.Prize.GroupName, FreeDeal = v.Prize.FreeDeal } });
                }
                else if (v.Prize == null && v.FlashDeal != null)
                {
                    newlistmer.Add(new InventoryItemModel.InventoryItemShow() { InventoryItemId = v.InventoryItemId, Name = v.Name, ShortDesc = v.ShortDesc, Price = v.Price, TypeId = v.TypeId, TypeName = v.TypeName, Discount = v.Discount, MaximumSales = v.MaximumSales, Sales = v.Sales, SalesVisualMeter = v.SalesVisualMeter, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = v.SubImageId, SubImageName = v.SubImageName, SubImageUrl = v.SubImageUrl, FlashDeal = new InventoryItemModel.FlashDeal() { FlashDealId = v.FlashDeal.FlashDealId, Discount = v.FlashDeal.Discount, MerchantId = v.FlashDeal.MerchantId, MerchantName = v.FlashDeal.MerchantName, InventoryItemId = v.FlashDeal.InventoryItemId, InventoryItemName = v.FlashDeal.InventoryItemName, StartTime = v.FlashDeal.StartTime, StartTimeStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.FlashDeal.StartTime), EndTime = v.FlashDeal.EndTime, EndTimeStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.FlashDeal.EndTime) } });
                }
                else
                {
                    newlistmer.Add(new InventoryItemModel.InventoryItemShow() { InventoryItemId = v.InventoryItemId, Name = v.Name, ShortDesc = v.ShortDesc, Price = v.Price, TypeId = v.TypeId, TypeName = v.TypeName, Discount = v.Discount, MaximumSales = v.MaximumSales, Sales = v.Sales, SalesVisualMeter = v.SalesVisualMeter, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = v.SubImageId, SubImageName = v.SubImageName, SubImageUrl = v.SubImageUrl });
                }
            }
            InventoryItemModel.InventoryItemVM model = new InventoryItemModel.InventoryItemVM();
            model.item = newlistmer;
            return View(model);
        }

        public ActionResult Faq()
        {
            return View();
        }
        public ActionResult ContactUs()
        {
            return View();
        }
        public ActionResult Careers()
        {
            return View();
        }

        public ActionResult MemberGoogle()
        {
            return View();
        }
        public ActionResult HowToGetTheSurPRIZE()
        {
            return View();
        }
        public ActionResult About()
        {
            return View();
        }
        public ActionResult PrivacyPolicy()
        {
            return View();
        }
        public ActionResult HowMOLPayCashWork()
        {
            return View();
        }


        public ActionResult CheckOut()
        {
            if (TempData["err"] != null)
            {
                ViewBag.err = TempData["err"];
            }
            return View();
        }
        public ActionResult CheckOutAut()
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("CheckOutShip");
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult CheckOutAut(FormCollection collection)
        {
            FormsAuthentication.SetAuthCookie("Guest", true);
            return RedirectToAction("CheckOutShip");
        }
        public ActionResult CheckOutShip()
        {
            if (Request.IsAuthenticated)
            {
                var ship = transactionservice.GetCurrentUserShipping();
                if (ship != null) ViewBag.ship = ship.Result;
                else
                {
                    var user = userservice.GetCurrentUser().Result;
                    if (user.FirstName != "Guest")
                    {
                        ViewBag.user = user;
                    }
                }
                var bill = transactionservice.GetCurrentUserBilling();
                if (bill != null) ViewBag.bill = bill.Result;
                return View();
            }
            else
            {
                return RedirectToAction("CheckOutAut");
            }
        }
        //public ActionResult CheckOutBill()
        //{
        //    if (Request.IsAuthenticated)
        //    {
        //        if (TempData["err"] != null)
        //        {
        //            ViewBag.err = TempData["err"];
        //        }
        //        var bill = transactionservice.GetCurrentUserBilling();
        //        if (bill != null) ViewBag.bill = bill.Result;
        //        return View();
        //    }
        //    else
        //    {
        //        return RedirectToAction("CheckOutAut");
        //    }
        //}
        [HttpPost]
        public JsonResult CheckOutBillAccount(string email)
        {
            try
            {

                bool result = userservice.CheckUserEmail(email).Result;
                if (result)
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                CustomException(ex);
                return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult CheckOutReview()
        {
            if (Request.IsAuthenticated)
            {
                if (TempData["err"] != null)
                {
                    ViewBag.err = TempData["err"];
                }
                //if (TempData["transactionid"] != null)
                //{
                //    BillModel.Bill model = new BillModel.Bill();
                //    model.TransactionId = Convert.ToInt32(TempData["transactionid"]);
                //    return View(model);
                //}
                //else
                //{
                //    return View();
                //}
                return View();
            }
            else
            {
                return RedirectToAction("CheckOutAut");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckOutReview(FormCollection collection, BillModel.Bill model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.TransactionId != 0)
                    {
                        int transid = model.TransactionId;
                        if (transactionservice.CheckTransactionPayment(transid).Result)
                        {
                            return RedirectToAction("CheckOutFinal", "Home");
                        }
                    }
                    if (model.Order == null || model.Order.Length < 1)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    if (model.Shipping == null || model.Shipping.Length < 1)
                    {
                        return RedirectToAction("CheckOutShip", "Home");
                    }
                    if (model.Billing == null || model.Shipping.Length < 1)
                    {
                        return RedirectToAction("CheckOutBill", "Home");
                    }

                    string order = model.Order;
                    if (order.Length > 1)
                    {
                        order = order.Substring(0, order.Length - 1);
                    }
                    order = "[" + order + "]";
                    List<OrderItem> listi = JsonHelper.JsonDeserialize<List<OrderItem>>(order);

                    string bill = model.Billing;
                    bill = "[" + bill + "]";
                    List<BillingC> billing = JsonHelper.JsonDeserialize<List<BillingC>>(bill);

                    string ship = model.Shipping;
                    ship = "[" + ship + "]";
                    List<ShippingC> shipping = JsonHelper.JsonDeserialize<List<ShippingC>>(ship);

                    if (shipping[0] == null)
                    {
                        return RedirectToAction("CheckOutShip", "Home");
                    }
                    else if (billing[0] == null)
                    {
                        return RedirectToAction("CheckOutBill", "Home");
                    }
                    else
                    {

                        bool result = transactionservice.CreateBillAccount(billing[0]).Result;
                        if (result)
                        {
                            FormsAuthentication.SignOut();
                            FormsAuthentication.SetAuthCookie(billing[0].email, true);
                            var user = userservice.GetCurrentUserByEmail(billing[0].email).Result;
                            if (user.KPoint < model.KPoint)
                            {
                                TempData["err"] = "33";
                                return RedirectToAction("CheckOut", "Home");
                            }
                            TransactionsHeader th = new TransactionsHeader();
                            th.MemberId = user.UserId;
                            DateTime transactiondate = DateTime.UtcNow;
                            th.TransactionDate = transactiondate;
                            th.TransactionStatus = (int)TransactionStatus.Pending;
                            th.KPoint = model.KPoint;
                            th.PromotionId = model.PromotionId;
                            th.PromotionType = model.PromotionType;
                            th.PromotionValue = model.PromotionValue;
                            List<TransactionsDetail> td = new List<TransactionsDetail>();
                            foreach (var v in listi)
                            {
                                td.Add(new TransactionsDetail() { InventoryItemId = v.id, FlashDealId = v.flashid, Variance = v.variance, Qty = v.qty, Price = v.price, Discount = v.discount });
                            }
                            th.Detail = td;
                            int transactionid = 0;
                            if (model.TransactionId == 0)
                            {
                                transactionid = transactionservice.CreateTransaction(th, Server.MapPath("/Rotativa")).Result;
                            }
                            else
                            {
                                transactionid = model.TransactionId;
                                transactionservice.CheckPoint(transactionid, th.KPoint);
                            }
                            bool res = transactionservice.CreateShipping(shipping[0], transactionid).Result;
                            bool res2 = transactionservice.CreateBilling(billing[0], transactionid).Result;
                            bool res3 = transactionservice.CreateShippingUser(shipping[0], user.UserId).Result;
                            bool res4 = transactionservice.CreateBillingUser(billing[0], user.UserId).Result;

                            decimal amount = listi.Sum(x => x.price);
                            decimal promotion = 0;
                            if (th.PromotionType == 1)
                            {
                                promotion = amount * th.PromotionValue / 100;
                            }
                            else if (th.PromotionType == 2)
                            {
                                promotion = th.PromotionValue;
                            }
                            if (th.KPoint > 0)
                            {
                                amount = amount - (th.KPoint / 100);
                            }
                            amount = amount - promotion;
                            //amount = Math.Round(amount, 2);

                            string orderid = GeneralService.TransactionCode(transactionid, transactiondate.AddHours(Kuazoo.Helper.defaultGMT));
                            string vcode = Security.Md5(amount.ToString("N2") + Kuazoo.Helper.merchantId + orderid + Kuazoo.Helper.merchantkey);
                            var molurl = "https://www.onlinepayment.com.my/MOLPay/pay/" + Kuazoo.Helper.merchantId + "/?";
                            molurl += "amount=" + amount.ToString("N2") ;
                            molurl += "&orderid=" + orderid;
                            molurl += "&bill_name=" + billing[0].fn + " " + billing[0].ln;
                            molurl += "&bill_email=" + billing[0].email;
                            molurl += "&bill_mobile=" + billing[0].ph;

                            var inventory = inventoryitemservice.GetInventoryItemById(td[0].InventoryItemId).Result;
                            string variance = td[0].Variance.Split('`')[0];
                            molurl += "&bill_desc=" + inventory.Name + ", " + variance;
                            molurl += "&country=MY";
                            molurl += "&currency=MYR";
                            molurl += "&vcode=" + vcode;
                            if (model.PaymentType == 3)
                            {
                                molurl += "&channel=credit";
                            }
                            else if (model.PaymentType == 2)
                            {
                                molurl += "&channel=maybank2u";
                            }
                            else
                            {
                                molurl += "&channel=cash";
                            }
                            model.TransactionId = transactionid;
                            model.MOLUrl = molurl;
                            //TempData["transactionid"] = transactionid;
                            return View(model);

                            //transactionservice.TransactionMemberNotif(transactionid, Helper.defaultGMT);

                            //return RedirectToAction("CheckOutFinal", "Home");
                        }
                        else
                        {
                            TempData["err"] = "3";
                            return RedirectToAction("CheckOutBill", "Home");
                        }
                    }
                }
                catch (Exception ex)
                {
                    CustomException(ex);
                    if (ex.Message == "User already assign.")
                    {
                        TempData["err"] = "3";
                        return RedirectToAction("CheckOutBill", "Home");
                    }
                    else
                    {
                        ViewBag.err = ex.Message;
                    }
                }
            }
            return View(model);
        }
        public ActionResult CheckOutFinal()
        {
            if (Request.IsAuthenticated)
            {
                DateTime today = DateTime.UtcNow;
                TransactionsHeader result = transactionservice.GetTransactionsListAnHour(today).Result;
                if (result == null && result.Detail == null)
                {
                    return RedirectToAction("Index");
                }
                TransactionModel.TransactionsHeaderVM model = new TransactionModel.TransactionsHeaderVM();
                model.header = result;
                List<com.kuazoo.InventoryItemShow> listmer = result.SimilarDetail;
                List<InventoryItemModel.InventoryItemShow> newlistmer = new List<InventoryItemModel.InventoryItemShow>();
                if (listmer != null)
                {
                    foreach (var v in listmer)
                    {
                        if (v.Prize != null && v.FlashDeal != null)
                        {
                            newlistmer.Add(new InventoryItemModel.InventoryItemShow() { InventoryItemId = v.InventoryItemId, Name = v.Name, ShortDesc = v.ShortDesc, Price = v.Price, TypeId = v.TypeId, TypeName = v.TypeName, Discount = v.Discount, MaximumSales = v.MaximumSales, Sales = v.Sales, SalesVisualMeter = v.SalesVisualMeter, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = v.SubImageId, SubImageName = v.SubImageName, SubImageUrl = v.SubImageUrl, Prize = new InventoryItemModel.PrizeShow() { PrizeId = v.Prize.PrizeId, GameType = v.Prize.GameType, Name = v.Prize.Name, Description = v.Prize.Description, ImageId = v.Prize.ImageId, ImageName = v.Prize.ImageName, ImageUrl = v.Prize.ImageUrl, SubImageId = v.Prize.SubImageId, SubImageName = v.Prize.SubImageName, SubImageUrl = v.Prize.SubImageUrl, Price = v.Prize.Price, SponsorName = v.Prize.SponsorName, Detail = v.Prize.Detail, Terms = v.Prize.Terms, ExpiryDate = v.Prize.ExpiryDate, ExpiryDateStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.Prize.ExpiryDate), GroupName = v.Prize.GroupName, FreeDeal = v.Prize.FreeDeal }, FlashDeal = new InventoryItemModel.FlashDeal() { FlashDealId = v.FlashDeal.FlashDealId, Discount = v.FlashDeal.Discount, MerchantId = v.FlashDeal.MerchantId, MerchantName = v.FlashDeal.MerchantName, InventoryItemId = v.FlashDeal.InventoryItemId, InventoryItemName = v.FlashDeal.InventoryItemName, StartTime = v.FlashDeal.StartTime, StartTimeStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.FlashDeal.StartTime), EndTime = v.FlashDeal.EndTime, EndTimeStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.FlashDeal.EndTime) } });
                        }
                        else if (v.Prize != null && v.FlashDeal == null)
                        {
                            newlistmer.Add(new InventoryItemModel.InventoryItemShow() { InventoryItemId = v.InventoryItemId, Name = v.Name, ShortDesc = v.ShortDesc, Price = v.Price, TypeId = v.TypeId, TypeName = v.TypeName, Discount = v.Discount, MaximumSales = v.MaximumSales, Sales = v.Sales, SalesVisualMeter = v.SalesVisualMeter, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = v.SubImageId, SubImageName = v.SubImageName, SubImageUrl = v.SubImageUrl, Prize = new InventoryItemModel.PrizeShow() { PrizeId = v.Prize.PrizeId, GameType = v.Prize.GameType, Name = v.Prize.Name, Description = v.Prize.Description, ImageId = v.Prize.ImageId, ImageName = v.Prize.ImageName, ImageUrl = v.Prize.ImageUrl, SubImageId = v.Prize.SubImageId, SubImageName = v.Prize.SubImageName, SubImageUrl = v.Prize.SubImageUrl, Price = v.Prize.Price, SponsorName = v.Prize.SponsorName, Detail = v.Prize.Detail, Terms = v.Prize.Terms, ExpiryDate = v.Prize.ExpiryDate, ExpiryDateStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.Prize.ExpiryDate), GroupName = v.Prize.GroupName, FreeDeal = v.Prize.FreeDeal } });
                        }
                        else if (v.Prize == null && v.FlashDeal != null)
                        {
                            newlistmer.Add(new InventoryItemModel.InventoryItemShow() { InventoryItemId = v.InventoryItemId, Name = v.Name, ShortDesc = v.ShortDesc, Price = v.Price, TypeId = v.TypeId, TypeName = v.TypeName, Discount = v.Discount, MaximumSales = v.MaximumSales, Sales = v.Sales, SalesVisualMeter = v.SalesVisualMeter, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = v.SubImageId, SubImageName = v.SubImageName, SubImageUrl = v.SubImageUrl, FlashDeal = new InventoryItemModel.FlashDeal() { FlashDealId = v.FlashDeal.FlashDealId, Discount = v.FlashDeal.Discount, MerchantId = v.FlashDeal.MerchantId, MerchantName = v.FlashDeal.MerchantName, InventoryItemId = v.FlashDeal.InventoryItemId, InventoryItemName = v.FlashDeal.InventoryItemName, StartTime = v.FlashDeal.StartTime, StartTimeStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.FlashDeal.StartTime), EndTime = v.FlashDeal.EndTime, EndTimeStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.FlashDeal.EndTime) } });
                        }
                        else
                        {
                            newlistmer.Add(new InventoryItemModel.InventoryItemShow() { InventoryItemId = v.InventoryItemId, Name = v.Name, ShortDesc = v.ShortDesc, Price = v.Price, TypeId = v.TypeId, TypeName = v.TypeName, Discount = v.Discount, MaximumSales = v.MaximumSales, Sales = v.Sales, SalesVisualMeter = v.SalesVisualMeter, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = v.SubImageId, SubImageName = v.SubImageName, SubImageUrl = v.SubImageUrl });
                        }
                    }
                }
                model.similar = newlistmer;

                var gameprize = gameservice.GetTransactionPrizeById(result.TransactionId).Result;
                int prizeid = gameprize.PrizeId;
                int qty = gameprize.Qty;
                com.kuazoo.Game game = gameservice.GetGameByPrizeId(prizeid, today).Result;
                model.Game = false;
                model.GameType = gameprize.GameType;
                if (model.GameType == 1)
                {
                    if (game != null && game.GameId != 0)
                    {
                        int gameplayed = gameservice.GetGameTransactionCount(result.TransactionId, game.GameId).Result;
                        if (gameplayed >= qty)
                        {
                            model.Game = false;
                        }
                        else
                        {
                            model.GameAttempt = qty - gameplayed;
                            model.Game = true;
                        }
                    }
                }
                return View(model);
            }
            else
            {
                return RedirectToAction("CheckOutAut");
            }
        }
        public ActionResult Participate(string id)
        {
            if (Request.IsAuthenticated)
            {
                try
                {
                    DateTime now = DateTime.UtcNow;
                    int transactionid = Convert.ToInt32(id.Substring(8));
                    transactionservice.UpdateTransactionPrizeParticipation(transactionid);
                    var prize = gameservice.GetPrizeByTransactionId(transactionid).Result;
                    GameModelFinish model = new GameModelFinish() { PrizeName = prize.PrizeName, PrizeImageUrl = prize.PrizeImageUrl, OrderNo = prize.OrderNo };
                    return View(model);
                }
                catch
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        public ActionResult Game(string id)
        {
            if (Request.IsAuthenticated)
            {
                try
                {
                    DateTime now = DateTime.UtcNow;
                    int transactionid = Convert.ToInt32(id.Substring(8));
                    var gameprize = gameservice.GetTransactionPrizeById(transactionid).Result;
                    int prizeid = gameprize.PrizeId;
                    int qty = gameprize.Qty;
                    if (qty == 0)
                    {
                        return RedirectToAction("Index");
                    }
                    com.kuazoo.Game v = gameservice.GetGameByPrizeId(prizeid, now).Result;
                    int gameplayed = gameservice.GetGameTransactionCount(transactionid, v.GameId).Result;
                    if (gameplayed >= qty)
                    {
                        return RedirectToAction("Index");
                    }
                    GameModelShow model = new GameModelShow() { GameId = v.GameId, GameName = v.Name, GameDescritpion = v.Description, GameInstruction = v.Instruction, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, GameAttempt = qty - gameplayed, OrderNo = id };
                    model.TransactionId = transactionid;
                    return View(model);
                }
                catch
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        //[HttpPost]
        //public ActionResult Game(GameModelShow Game)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            com.kuazoo.GameTransaction pro = new com.kuazoo.GameTransaction();
        //            pro.GameId = Game.GameId;
        //            pro.TransactionId = Game.TransactionId;
        //            pro.UserLatitude = Game.UserLatitude;
        //            pro.UserLongitude = Game.UserLongitude;
        //            bool result = gameservice.CreateGameTransaction(pro).Result;
        //            if (result == true)
        //            {
        //                TempData["msg"] = "1g";
        //            }
        //            else
        //            {
        //                TempData["msg"] = "3g";
        //            }
        //            return RedirectToAction("Index");
        //        }
        //        catch (Exception ex)
        //        {
        //            ViewBag.result = ex.Message;
        //        }
        //    }
        //    return View(Game);

        //}

        [HttpPost]
        public JsonResult GameCheck(int gameid, int id)
        {
            try
            {
                var gameprize = gameservice.GetTransactionPrizeById(id).Result;
                int qty = gameprize.Qty;
                if (qty == 0)
                {
                    return Json(new { success = false, error = "There is no attempt left." }, JsonRequestBehavior.AllowGet);
                }
                int gameplayed = gameservice.GetGameTransactionCount(id, gameid).Result;
                if (gameplayed >= qty)
                {
                    return Json(new { success = false, error = "There is no attempt left." }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = true, error = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CustomException(ex);
                return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult GameSpot(int gameid, int id, double x, double y, int time)
        {
            try
            {
                var gameprize = gameservice.GetTransactionPrizeById(id).Result;
                int qty = gameprize.Qty;
                if (qty == 0)
                {
                    return Json(new { success = false, error = "There is no attempt left." }, JsonRequestBehavior.AllowGet);
                }
                int gameplayed = gameservice.GetGameTransactionCount(id, gameid).Result;
                if (gameplayed >= qty)
                {
                    return Json(new { success = false, error = "There is no attempt left." }, JsonRequestBehavior.AllowGet);
                }
                com.kuazoo.GameTransaction pro = new com.kuazoo.GameTransaction();
                pro.GameId = gameid;
                pro.TransactionId = id;
                pro.UserLatitude = x;
                pro.UserLongitude = y;
                pro.TimeUsed = time;
                bool result = gameservice.CreateGameTransaction(pro).Result;
                return Json(new { success = true, error = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CustomException(ex);
                return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GameFinish(string id)
        {
            if (Request.IsAuthenticated)
            {
                try
                {
                    DateTime now = DateTime.UtcNow;
                    int transactionid = Convert.ToInt32(id.Substring(8));
                    var prize = gameservice.GetPrizeByTransactionId(transactionid).Result;

                    var gameprize = gameservice.GetTransactionPrizeById(transactionid).Result;
                    com.kuazoo.Game v = gameservice.GetGameByPrizeId(gameprize.PrizeId, now).Result;
                    int qty = gameprize.Qty;
                    int gameplayed = gameservice.GetGameTransactionCount(transactionid, v.GameId).Result;
                    int left = 0;
                    if (gameplayed < qty)
                    {
                        left = qty - gameplayed;
                    }
                    GameModelFinish model = new GameModelFinish() { InventoryItemId = prize.InventoryItemId, PrizeName = prize.PrizeName, PrizeImageUrl = prize.PrizeImageUrl, OrderNo = prize.OrderNo, AttemptLeft = left };
                    return View(model);
                }
                catch
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
        }


        [HttpPost]
        public JsonResult CheckOutPromotionCode(string id, int tId = 0)
        {
            try
            {
                var promo = transactionservice.CheckPromotionCode(id);
                if (promo != null)
                {
                    TransactionModel.PromotionCodeVM vm = new TransactionModel.PromotionCodeVM();
                    vm.Type = promo.Result.Type;
                    vm.TypeName = promo.Result.TypeName;
                    vm.Value = promo.Result.Value;
                    vm.Code = promo.Result.Code;
                    vm.PromotionId = promo.Result.PromotionId;
                    int userid = 0;
                    try
                    {
                        var user = userservice.GetCurrentUser().Result;
                        userid = user.UserId;
                        if (transactionservice.CheckTransactionPromotionCodeUsed(vm.PromotionId, userid, tId).Result)
                        {
                            return Json(new { success = false, error = "Promo code has been used." }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    catch
                    {
                        userid = 0;
                    }
                    if (userid != 0)
                    {

                    }
                    return Json(new { success = true, error = "", result = vm }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, error = "Invalid promo code, please try again." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                CustomException(ex);
                return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult StateList(string id)
        {
            List<com.kuazoo.State> listc = countryservice.GetStateListByName(id).Result;
            List<StateModel> newlistcat = new List<StateModel>();
            foreach (var v in listc)
            {
                newlistcat.Add(new StateModel() { StateId = v.StateId, StateName = v.Name });
            }
            IEnumerable<StateModel> ienuList = newlistcat;
            return Json(ienuList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult StateListInv(string id, int did = 0)
        {
            List<com.kuazoo.State> listc = countryservice.GetStateListByName(id).Result;
            bool restrict = false;
            if (did != 0)
            {
                try
                {
                    var inventory = inventoryitemservice.GetInventoryItemById(did).Result;
                    if (inventory.InventoryItemType.InventoryItemTypeId == 1)
                    {
                        restrict = true;
                    }
                }
                catch
                {

                }
            }
            List<StateModel> newlistcat = new List<StateModel>();
            foreach (var v in listc)
            {
                if (restrict == false)
                {
                    newlistcat.Add(new StateModel() { StateId = v.StateId, StateName = v.Name });
                }
                else
                {
                    if (v.Name.ToLower() == "perlis" || v.Name.ToLower() == "kedah" || v.Name.ToLower() == "penang" || v.Name.ToLower() == "perak" || v.Name.ToLower() == "kelantan" || v.Name.ToLower() == "terengganu" || v.Name.ToLower() == "pahang" || v.Name.ToLower() == "selangor" || v.Name.ToLower() == "kuala Lumpur" || v.Name.ToLower() == "putrajaya" || v.Name.ToLower() == "negeri sembilan" || v.Name.ToLower() == "melaka" || v.Name.ToLower() == "johor")
                    {
                        newlistcat.Add(new StateModel() { StateId = v.StateId, StateName = v.Name });
                    }
                }
            }
            IEnumerable<StateModel> ienuList = newlistcat;
            return Json(ienuList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult InventoryType(int id = 0)
        {
            bool restrict = false;
            if (id != 0)
            {
                try
                {
                    var inventory = inventoryitemservice.GetInventoryItemById(id).Result;
                    if (inventory.InventoryItemType.InventoryItemTypeId == 1)
                    {
                        restrict = true;
                    }
                }
                catch
                {

                }
            }
            return Json(restrict, JsonRequestBehavior.AllowGet);
        }


        public ActionResult WinnersCircle(string sort = "month", int page = 1)
        {
            InventoryItemModel.WinnerShowModel model = new InventoryItemModel.WinnerShowModel();
            com.kuazoo.WinnerModel winlist = prizeservice.GetWinnerList(sort, page - 1, 6, Helper.defaultGMT).Result;
            List<InventoryItemModel.WinnerShowVM> list = new List<InventoryItemModel.WinnerShowVM>();
            model.Total = 0;
            if (winlist != null)
            {
                model.Total = winlist.Total;
                foreach (var i in winlist.Item)
                {
                    List<InventoryItemModel.WinnerShow> listitem = new List<InventoryItemModel.WinnerShow>();
                    foreach (var v in i.WinnerList)
                    {
                        listitem.Add(new InventoryItemModel.WinnerShow() { WinnerId = v.WinnerId, WinnerDate = v.WinnerDate, PrizeImageUrl = v.PrizeImageUrl, PrizeName = v.PrizeName, FirstName = v.FirstName, LastName = v.LastName, MemberImageUrl = v.MemberImageUrl, Email = v.Email, City = v.City, State = v.State, Country = v.Country });
                    }
                    list.Add(new InventoryItemModel.WinnerShowVM() { Sort = i.Sort, WinnerList = listitem });
                }
            }
            ViewBag.currPage = page;
            ViewBag.currSort = sort;
            model.Item = list;
            return View(model);
        }

        [HttpPost]
        public JsonResult CheckLimitShare(int id, string type)
        {
            try
            {
                com.kuazoo.PointDetail detail = new PointDetail();
                var user = userservice.GetCurrentUser().Result;

                if (transactionservice.CheckDealPurchase(id, user.UserId).Result)
                {
                    detail.Type = KPointAction.PostAfterPurchase;
                }
                else
                {
                    detail.Type = KPointAction.PostBeforePurchase;
                }
                detail.InventoryItemId = id;
                detail.UserId = user.UserId;
                detail.Remarks = type;
                detail.Amount = PointService.GetPointAction(detail.Type).Result;

                bool check = pointservice.CheckLimit(detail, Helper.defaultGMT).Result;
                if (check)
                {
                    return Json(new { success = true, error = "", login = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, error = "Exceed the limit for today", login = true }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                CustomException(ex);
                return Json(new { success = false, error = ex.Message, login = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult CheckPostFB(int id, string type)
        {
            try
            {
                com.kuazoo.PointDetail detail = new PointDetail();
                var user = userservice.GetCurrentUser().Result;

                if (transactionservice.CheckDealPurchase(id, user.UserId).Result)
                {
                    detail.Type = KPointAction.PostAfterPurchase;
                }
                else
                {
                    detail.Type = KPointAction.PostBeforePurchase;
                }
                detail.InventoryItemId = id;
                detail.UserId = user.UserId;
                detail.Remarks = type;
                detail.Amount = PointService.GetPointAction(detail.Type).Result;

                bool check = pointservice.CheckLimit(detail, Helper.defaultGMT).Result;
                if (check)
                {

                    bool result = pointservice.CreatePoint(detail).Result;
                    if (result)
                    {
                        return Json(new { success = true, error = "" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, error = "Failed" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = false, error = "Exceed the limit for today" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                CustomException(ex);
                return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult ShareEmail(int id = 0)
        {
            ShareEmailModel model = new ShareEmailModel();
            model.InventoryItemId = id;
            model.EmailFromFlag = false;
            if (Request.IsAuthenticated && com.kuazoo.UserService.NotGuest().Result)
            {
                model.EmailFromFlag = true;
            }
            return View(model);
        }
        [HttpPost]
        public JsonResult ShareEmailSend(int id, string emailfrom, string emailto, string firstname, string lastname)
        {
            try
            {
                if (Request.IsAuthenticated && com.kuazoo.UserService.NotGuest().Result)
                {
                    var user = userservice.GetCurrentUser().Result;
                    emailfrom = user.Email;
                    firstname = user.FirstName;
                    lastname = user.LastName;
                }
                try
                {
                    var user = userservice.GetCurrentUser().Result;
                    string[] emailsplit = emailto.Split(',');
                    foreach (var v in emailsplit)
                    {
                        if (v != "" && v != " ")
                        {
                            com.kuazoo.PointDetail detail = new PointDetail();

                            if (transactionservice.CheckDealPurchase(id, user.UserId).Result)
                            {
                                detail.Type = KPointAction.PostAfterPurchase;
                            }
                            else
                            {
                                detail.Type = KPointAction.PostBeforePurchase;
                            }
                            detail.InventoryItemId = id;
                            detail.UserId = user.UserId;
                            detail.Remarks = "Email to " + v;
                            detail.Amount = PointService.GetPointAction(detail.Type).Result;

                            bool check = pointservice.CheckLimit(detail, Helper.defaultGMT).Result;
                            if (check)
                            {
                                bool result2 = pointservice.CreatePoint(detail).Result;
                            }
                        }
                    }
                }
                catch
                {

                }
                bool result = inventoryitemservice.ShareEmail(id, emailfrom, emailto, firstname, lastname);
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

        public void UpdateSalesVisualMeter(int id)
        {
            transactionservice.UpdateSalesVisualMeterNewInventory(id);
        }

        [HttpPost]
        public JsonResult SubscribeEmail(string id)
        {
            try
            {
                var result = userservice.CreateSubscribe(id).Result;
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
        public ActionResult VerifyPaymentAfter(string err = "")
        {
            if (err != "") ViewBag.err = err;
            return View();
        }

        [HttpPost]
        public ActionResult VerifyPayment(BillModel.BillCB model)
        {
            try
            {
                string key0 = Security.Md5(model.tranID + model.orderid + model.status + model.domain + model.amount + model.currency);
                string key1 = Security.Md5(model.paydate + model.domain + key0 + model.appcode + Kuazoo.Helper.merchantkey);
                int transactionid = GeneralService.ExtractTransactionId(model.orderid);
                if (model.skey == key1)
                {
                    TransactionsHeader th = new TransactionsHeader();
                    th.TransactionId = transactionid;
                    if (model.status == "00" || model.status == "22")
                    {
                        th.TransactionStatus = (int)TransactionStatus.Completed;
                    }
                    else
                    {
                        th.TransactionStatus = (int)TransactionStatus.Pending;
                    }
                    th.tranID = model.tranID;
                    th.orderid = model.orderid;
                    th.status = model.status;
                    th.domain = model.domain;
                    th.amount = decimal.Parse(model.amount);
                    th.currency = model.currency;
                    th.appcode = model.appcode;
                    th.paydate = DateTime.Parse(model.paydate);
                    th.skey = model.skey;
                    th.channel = model.channel;
                    th.error_code = model.error_code;
                    th.error_desc = model.error_desc;
                    int tid = transactionservice.CreateTransaction(th, Server.MapPath("/Rotativa"), Helper.defaultGMT).Result;
                    //if (th.status == "00")
                    //{
                    //transactionservice.TransactionMemberNotif(transactionid, Helper.defaultGMT);
                    //}
                    if (th.status == "00" || th.status == "22")
                    {
                        return RedirectToAction("VerifyPaymentAfter", "Home");
                    }
                    else
                    {
                        TempData["err"] = "5";
                        return RedirectToAction("VerifyPaymentAfter", "Home", new { err = 5 });
                    }
                }
                else
                {
                    TempData["err"] = "5";
                    return RedirectToAction("VerifyPaymentAfter", "Home", new { err = 5 });
                }
            }
            catch (Exception ex)
            {
                CustomException(ex);
                TempData["err"] = "5";
                return RedirectToAction("VerifyPaymentAfter", "Home", new { err = 5 });
            }
        }

        [HttpPost]
        public ActionResult CallbackPayment(BillModel.BillCB model)
        {
            try
            {
                CallbackPayment cal = new CallbackPayment();
                cal.nbcb = model.nbcb;
                cal.tranID = model.tranID;
                cal.orderid = model.orderid;
                cal.status = model.status;
                cal.domain = model.domain;
                cal.amount = decimal.Parse(model.amount);
                cal.currency = model.currency;
                cal.appcode = model.appcode;
                cal.paydate = DateTime.Parse(model.paydate);
                cal.skey = model.skey;
                cal.channel = model.channel;
                cal.error_code = model.error_code;
                cal.error_desc = model.error_desc;
                bool result = transactionservice.CreateCallbakLog(cal).Result;
                //string skeya = Security.Md5(model.tranID + model.domain + Kuazoo.Helper.merchantkey + model.amount);
                string key0 = Security.Md5(model.tranID + model.orderid + model.status + model.domain + model.amount + model.currency);
                string key1 = Security.Md5(model.paydate + model.domain + key0 + model.appcode + Kuazoo.Helper.merchantkey);
                int transactionid = GeneralService.ExtractTransactionId(model.orderid);
                if (model.skey == key1)
                {
                    TransactionsHeader th = new TransactionsHeader();
                    th.TransactionId = transactionid;
                    if (model.status == "00" || model.status == "22")
                    {
                        th.TransactionStatus = (int)TransactionStatus.Completed;
                    }
                    else
                    {
                        th.TransactionStatus = (int)TransactionStatus.Pending;
                    }
                    th.tranID = model.tranID;
                    th.orderid = model.orderid;
                    th.status = model.status;
                    th.domain = model.domain;
                    th.amount = decimal.Parse(model.amount);
                    th.currency = model.currency;
                    th.appcode = model.appcode;
                    th.paydate = DateTime.Parse(model.paydate);
                    th.skey = model.skey;
                    th.channel = model.channel;
                    th.error_code = model.error_code;
                    th.error_desc = model.error_desc;
                    int tid = transactionservice.CreateTransaction(th, Server.MapPath("/Rotativa"), Helper.defaultGMT).Result;
                    //if (th.status == "00")
                    //{
                    //transactionservice.TransactionMemberNotif(transactionid, Helper.defaultGMT);
                    //}
                    if (model.nbcb == "1")
                    {
                        ViewBag.msg = "CBTOKEN:MPSTATOK";
                    }
                }
            }
            catch
            {
            }
            return View();
        }

        public void RevertPayment(int transactionid)
        {
            TransactionsHeader th = new TransactionsHeader();
            th.TransactionId = transactionid;
            th.status = "11";
            int tid = transactionservice.CreateTransaction(th, Server.MapPath("/Rotativa"), Helper.defaultGMT).Result;
        }

        public void SendEmail(int transactionid)
        {
            transactionservice.TransactionMemberNotif(transactionid, Server.MapPath("/Rotativa"),Helper.defaultGMT);
        }
        public void SendEmailGift(int transactionid)
        {
            transactionservice.TransactionGiftNotice(transactionid, Server.MapPath("/Rotativa"), Helper.defaultGMT);
        }
        [HttpPost]
        public JsonResult CheckVarianceLimit(int id, string variance)
        {
            try
            {
                var result = transactionservice.CheckVarianceLimit(id, variance).Result;
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
        public JsonResult CheckTransactionPayment(int id)
        {
            try
            {
                var result = transactionservice.CheckTransactionPayment(id).Result;
                if (result)
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                CustomException(ex);
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult BuyFreeDeal(int id, int flashid, string variance, int qty, decimal disc, decimal price)
        {
            if (Request.IsAuthenticated)
            {
                var user  = userservice.GetCurrentUser().Result;
                FreeDealTransaction model = new FreeDealTransaction();
                model.InventoryItemId = id;
                model.MemberId = user.UserId;
                model.FlashDealId = flashid;
                model.Variance = variance;
                model.Qty = qty;
                model.Discount = disc;
                model.Price = price;
                var result = freedealservice.CreateFreeDeal(model, Server.MapPath("/Rotativa"), Helper.defaultGMT).Result;
                return Json(new { returnUrl = Url.Action("CheckOutFreeDeal","Home") }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { returnUrl = Url.Action("Index", "Home") }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult CheckOutFreeDeal()
        {
            if (Request.IsAuthenticated)
            {
                DateTime today = DateTime.UtcNow;
                TransactionsHeader result = freedealservice.GetTransactionFreeDealAnHour(today).Result;
                if (result == null && result.Detail == null)
                {
                    return RedirectToAction("Index");
                }
                TransactionModel.TransactionsHeaderVM model = new TransactionModel.TransactionsHeaderVM();
                model.header = result;
                List<com.kuazoo.InventoryItemShow> listmer = result.SimilarDetail;
                List<InventoryItemModel.InventoryItemShow> newlistmer = new List<InventoryItemModel.InventoryItemShow>();
                if (listmer != null)
                {
                    foreach (var v in listmer)
                    {
                        if (v.Prize != null && v.FlashDeal != null)
                        {
                            newlistmer.Add(new InventoryItemModel.InventoryItemShow() { InventoryItemId = v.InventoryItemId, Name = v.Name, ShortDesc = v.ShortDesc, Price = v.Price, TypeId = v.TypeId, TypeName = v.TypeName, Discount = v.Discount, MaximumSales = v.MaximumSales, Sales = v.Sales, SalesVisualMeter = v.SalesVisualMeter, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = v.SubImageId, SubImageName = v.SubImageName, SubImageUrl = v.SubImageUrl, Prize = new InventoryItemModel.PrizeShow() { PrizeId = v.Prize.PrizeId, GameType = v.Prize.GameType, Name = v.Prize.Name, Description = v.Prize.Description, ImageId = v.Prize.ImageId, ImageName = v.Prize.ImageName, ImageUrl = v.Prize.ImageUrl, SubImageId = v.Prize.SubImageId, SubImageName = v.Prize.SubImageName, SubImageUrl = v.Prize.SubImageUrl, Price = v.Prize.Price, SponsorName = v.Prize.SponsorName, Detail = v.Prize.Detail, Terms = v.Prize.Terms, ExpiryDate = v.Prize.ExpiryDate, ExpiryDateStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.Prize.ExpiryDate), GroupName = v.Prize.GroupName, FreeDeal = v.Prize.FreeDeal }, FlashDeal = new InventoryItemModel.FlashDeal() { FlashDealId = v.FlashDeal.FlashDealId, Discount = v.FlashDeal.Discount, MerchantId = v.FlashDeal.MerchantId, MerchantName = v.FlashDeal.MerchantName, InventoryItemId = v.FlashDeal.InventoryItemId, InventoryItemName = v.FlashDeal.InventoryItemName, StartTime = v.FlashDeal.StartTime, StartTimeStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.FlashDeal.StartTime), EndTime = v.FlashDeal.EndTime, EndTimeStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.FlashDeal.EndTime) } });
                        }
                        else if (v.Prize != null && v.FlashDeal == null)
                        {
                            newlistmer.Add(new InventoryItemModel.InventoryItemShow() { InventoryItemId = v.InventoryItemId, Name = v.Name, ShortDesc = v.ShortDesc, Price = v.Price, TypeId = v.TypeId, TypeName = v.TypeName, Discount = v.Discount, MaximumSales = v.MaximumSales, Sales = v.Sales, SalesVisualMeter = v.SalesVisualMeter, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = v.SubImageId, SubImageName = v.SubImageName, SubImageUrl = v.SubImageUrl, Prize = new InventoryItemModel.PrizeShow() { PrizeId = v.Prize.PrizeId, GameType = v.Prize.GameType, Name = v.Prize.Name, Description = v.Prize.Description, ImageId = v.Prize.ImageId, ImageName = v.Prize.ImageName, ImageUrl = v.Prize.ImageUrl, SubImageId = v.Prize.SubImageId, SubImageName = v.Prize.SubImageName, SubImageUrl = v.Prize.SubImageUrl, Price = v.Prize.Price, SponsorName = v.Prize.SponsorName, Detail = v.Prize.Detail, Terms = v.Prize.Terms, ExpiryDate = v.Prize.ExpiryDate, ExpiryDateStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.Prize.ExpiryDate), GroupName = v.Prize.GroupName, FreeDeal = v.Prize.FreeDeal } });
                        }
                        else if (v.Prize == null && v.FlashDeal != null)
                        {
                            newlistmer.Add(new InventoryItemModel.InventoryItemShow() { InventoryItemId = v.InventoryItemId, Name = v.Name, ShortDesc = v.ShortDesc, Price = v.Price, TypeId = v.TypeId, TypeName = v.TypeName, Discount = v.Discount, MaximumSales = v.MaximumSales, Sales = v.Sales, SalesVisualMeter = v.SalesVisualMeter, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = v.SubImageId, SubImageName = v.SubImageName, SubImageUrl = v.SubImageUrl, FlashDeal = new InventoryItemModel.FlashDeal() { FlashDealId = v.FlashDeal.FlashDealId, Discount = v.FlashDeal.Discount, MerchantId = v.FlashDeal.MerchantId, MerchantName = v.FlashDeal.MerchantName, InventoryItemId = v.FlashDeal.InventoryItemId, InventoryItemName = v.FlashDeal.InventoryItemName, StartTime = v.FlashDeal.StartTime, StartTimeStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.FlashDeal.StartTime), EndTime = v.FlashDeal.EndTime, EndTimeStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.FlashDeal.EndTime) } });
                        }
                        else
                        {
                            newlistmer.Add(new InventoryItemModel.InventoryItemShow() { InventoryItemId = v.InventoryItemId, Name = v.Name, ShortDesc = v.ShortDesc, Price = v.Price, TypeId = v.TypeId, TypeName = v.TypeName, Discount = v.Discount, MaximumSales = v.MaximumSales, Sales = v.Sales, SalesVisualMeter = v.SalesVisualMeter, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = v.SubImageId, SubImageName = v.SubImageName, SubImageUrl = v.SubImageUrl });
                        }
                    }
                }
                model.similar = newlistmer;
                return View(model);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
       
        [HttpPost]
        public JsonResult CheckFreeDealUsed(int id)
        {
            try
            {
                var result = inventoryitemservice.CheckFreeDeal(id,Request.IsAuthenticated).Result;
                return Json(new { success = result}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CustomException(ex);
                return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
