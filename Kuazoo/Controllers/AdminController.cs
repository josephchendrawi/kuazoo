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
using System.IO;
using System.Web.UI;
using System.Web.Security;
using DocumentFormat.OpenXml.Packaging;
using Kuazoo.Code;
using DocumentFormat.OpenXml.Spreadsheet;
using Newtonsoft.Json;

namespace Kuazoo.Controllers
{
    public class AdminController : BaseAdminController
    {
        private CountryService countryservice = new CountryService();
        private MerchantService merchantservice = new MerchantService();
        private InventoryItemService inventoryitemservice = new InventoryItemService();
        private FlashDealService flashdealservice = new FlashDealService();
        private TagService tagservice = new TagService();
        private MemberService memberservice = new MemberService();
        private TransactionService transactionservice = new TransactionService();
        private ImageService imageservice = new ImageService();
        private PrizeService prizeservice = new PrizeService();
        private PromoService promotionservice = new PromoService();
        private PointService pointservice = new PointService();
        private GeneralService generalservice = new GeneralService();
        private GameService gameservice = new GameService();
        private AdminService adminservice = new AdminService();
        private LandingService landingservice = new LandingService();
        private FreeDealService freedealservice = new FreeDealService();
        //
        // GET: /Admin/
        public ActionResult SignIn()
        {
            if (TempData["msg"] != null)
            {
                ViewBag.msg = TempData["msg"];
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn(LogOnModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    com.kuazoo.Admin pro = new com.kuazoo.Admin();
                    pro.Email = model.UserName;
                    pro.Password = model.Password;

                    bool result = adminservice.CheckLogin(pro).Result;
                    if (result)
                    {
                        FormsAuthentication.SetAuthCookie(pro.Email, model.RememberMe);
                        return RedirectToAction("Index", "Admin");
                    }
                    else
                    {
                        ViewBag.result = "3";
                        TempData["msg"] = "3";
                    }
                    return RedirectToAction("SignIn", "Admin");
                }
                catch (Exception ex)
                {
                    CustomException(ex);
                    ViewBag.result = ex.Message;
                }
            }
            return View(model);

        }
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("SignIn", "Admin");
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult IndexPrize_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<com.kuazoo.Prize> listmer = prizeservice.GetPrizeListExpireSoon().Result;
            List<InventoryItemModel.Prize> newlistmer = new List<InventoryItemModel.Prize>();
            foreach (var v in listmer)
            {
                newlistmer.Add(new InventoryItemModel.Prize() { PrizeId = v.PrizeId, Name = v.Name, ExpiryDate = v.ExpiryDate.AddHours(Helper.defaultGMT) });
            }
            IEnumerable<InventoryItemModel.Prize> ienuList = newlistmer;
            var result = ienuList.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult IndexDeal_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<com.kuazoo.InventoryItem> listmer = inventoryitemservice.GetInventoryItemListExpireSoon().Result;
            List<InventoryItemModel.InventoryItem> newlistmer = new List<InventoryItemModel.InventoryItem>();
            foreach (var v in listmer)
            {
                newlistmer.Add(new InventoryItemModel.InventoryItem() { InventoryItemId = v.InventoryItemId, Name = v.Name, ExpireDate = v.ExpireDate });
            }
            IEnumerable<InventoryItemModel.InventoryItem> ienuList = newlistmer;
            var result = ienuList.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult IndexHot_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<com.kuazoo.Transaction> listmer = transactionservice.GetTransactionListHot().Result;
            List<TransactionModel.Transaction> newlistmer = new List<TransactionModel.Transaction>();
            foreach (var v in listmer)
            {
                newlistmer.Add(new TransactionModel.Transaction() { InventoryItemId = v.InventoryItemId, InventoryItemName = v.InventoryItemName, Sales = v.Sales });
            }
            IEnumerable<TransactionModel.Transaction> ienuList = newlistmer;
            var result = ienuList.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #region merchant
        public ActionResult Merchant()
        {
            if (TempData["msg"] != null)
            {
                ViewBag.msg = TempData["msg"];
            }
            return View();
        }

        public ActionResult Merchant_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<com.kuazoo.Merchant> listmer = merchantservice.GetMerchantList().Result;
            List<MerchantModel> newlistmer = new List<MerchantModel>();
            foreach (var v in listmer)
            {
                newlistmer.Add(new MerchantModel() { Create = v.Create.AddHours(Helper.defaultGMT), MerchantId = v.MerchantId, Name = v.Name, CountryId = v.Country.CountryId, Country = new MasterModel.Country() { CountryId = v.Country.CountryId, CountryName = v.Country.Name }, AddressLine1 = v.AddressLine1, AddressLine2 = v.AddressLine2, CityId = v.City.CityId, City = new MasterModel.City() { CityId = v.City.CityId, CityName = v.City.Name }, PostCode = v.PostCode, ContactNumber = v.ContactNumber, Email = v.Email, Website = v.Website, Facebook = v.Facebook, Latitude = v.Latitude, Longitude = v.Longitude, StatusId = v.Status.StatusId, LastAction = v.LastAction = v.LastAction, Description = v.Description, SubImageId = String.Join(",", v.SubImageId), SubImageName = String.Join("|", v.SubImageName), SubImageUrl = String.Join("|", v.SubImageUrl) , SubImageUrlLink = String.Join("|",v.SubImageUrlLink) });
            }
            IEnumerable<MerchantModel> ienuList = newlistmer;
            var result = ienuList.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public String Merchant_Duplicate(int MerchantId)
        {
            string res = "";
            if (MerchantId != null && MerchantId > 0)
            {
                try
                {
                    Boolean result = merchantservice.DuplicateMerchant(MerchantId).Result;

                    if (result) res = "1";
                    else res = "2";
                }
                catch (Exception ex)
                {
                    CustomException(ex);
                    res = ex.Message;
                }

            }

            return res;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Merchant_Destroy([DataSourceRequest] DataSourceRequest request, MerchantModel Merchant)
        {

            try
            {
                if (Merchant != null)
                {
                    Boolean result = merchantservice.DeleteMerchant(Merchant.MerchantId).Result;

                    if (result) ViewBag.result = "1";
                    else ViewBag.result = "2";
                    ModelState.AddModelError("", ViewBag.result);

                }
            }
            catch (Exception ex)
            {
                CustomException(ex);
                ViewBag.result = ex.Message;
                ModelState.AddModelError("", ViewBag.result);
            }

            return Json(ModelState.ToDataSourceResult());
        }

        public ActionResult MerchantAdd()
        {
            return View();
        }

        [HttpPost]
        public ActionResult MerchantAdd(FormCollection collection, MerchantModel Merchant)
        {
            try
            {
                MerchantModel mv = new MerchantModel();

                com.kuazoo.Merchant mer = new com.kuazoo.Merchant();
                mer.MerchantId = 0;
                mer.Name = collection.Get("Name");
                mer.AddressLine1 = collection.Get("AddressLine1");
                mer.AddressLine2 = collection.Get("AddressLine2");
                com.kuazoo.City city = new com.kuazoo.City();
                city.CityId = Convert.ToInt16(collection.Get("CityDropDown").ToString());
                mer.City = city;
                mer.PostCode = collection.Get("PostCode");
                mer.ContactNumber = collection.Get("ContactNumber");
                mer.Email = collection.Get("Email");
                mer.Website = collection.Get("Website");
                mer.Facebook = collection.Get("Facebook");
                try
                {
                    string coord = collection.Get("TempLatLong");
                    char[] delimChars = { ',' };
                    string[] seperated = coord.Split(delimChars);
                    //check if
                    int length1 = seperated[0].Length;
                    int length2 = seperated[1].Length;
                    mer.Latitude = float.Parse(seperated[0].Substring(1, length1 - 1));
                    mer.Longitude = float.Parse(seperated[1].Substring(0, length2 - 2));
                }
                catch
                {
                    mer.Latitude = 0;
                    mer.Longitude = 0;
                }

                com.kuazoo.Status stat = new com.kuazoo.Status();
                stat.StatusId = (int)MerchantStatus.Premium;
                mer.Status = stat;

                if (Merchant.Description != null)
                {
                    string desc = Microsoft.JScript.GlobalObject.decodeURI(Merchant.Description);
                    mer.Description = desc;
                }
                else { mer.Description = ""; }
                if (Merchant.SubImageId != null && Merchant.SubImageId != "")
                {
                    List<int> subimg = new List<int>();
                    List<string> subimglink = new List<string>();
                    String[] subimagelist = Merchant.SubImageId.Split(',');
                    foreach (var v in subimagelist)
                    {
                        if (v != "")
                        {
                            subimg.Add(int.Parse(v));
                            subimglink.Add(collection.Get("txtSubImg" + v));
                        }
                    }
                    mer.SubImageId = subimg;
                    mer.SubImageUrlLink = subimglink;
                }
                bool result = merchantservice.CreateMerchant(mer).Result;
                ViewBag.result = "3";
                if (result == true)
                {
                    ViewBag.result = "1";
                }
                TempData["msg"] = ViewBag.result;
                return RedirectToAction("Merchant", "Admin");
            }
            catch (Exception ex)
            {
                CustomException(ex);
                ViewBag.result = ex.Message;
                return View();
            }
        }

        public ActionResult MerchantEdit(int MerchantId)
        {
            com.kuazoo.Merchant v = merchantservice.GetMerchantById(MerchantId).Result;
            MerchantModel merchant = new MerchantModel() { MerchantId = v.MerchantId, Name = v.Name, CountryId = v.Country.CountryId, Country = new MasterModel.Country() { CountryId = v.Country.CountryId, CountryName = v.Country.Name }, AddressLine1 = v.AddressLine1, AddressLine2 = v.AddressLine2, CityId = v.City.CityId, City = new MasterModel.City() { CityId = v.City.CityId, CityName = v.City.Name }, PostCode = v.PostCode, ContactNumber = v.ContactNumber, Email = v.Email, Website = v.Website, Facebook = v.Facebook, Latitude = v.Latitude, Longitude = v.Longitude, StatusId = v.Status.StatusId, LastAction = v.LastAction = v.LastAction, Description = v.Description, SubImageId = String.Join(",", v.SubImageId), SubImageName = String.Join("|", v.SubImageName), SubImageUrl = String.Join("|", v.SubImageUrl), SubImageUrlLink = String.Join("|", v.SubImageUrlLink) };
            return View(merchant);
        }
        [HttpPost]
        public ActionResult MerchantEdit(FormCollection collection,MerchantModel Merchant)
        {
            try
            {
                com.kuazoo.Merchant mer = new com.kuazoo.Merchant();
                mer.MerchantId = Merchant.MerchantId;
                mer.Name = Merchant.Name;
                mer.AddressLine1 = Merchant.AddressLine1;
                mer.AddressLine2 = Merchant.AddressLine2;
                com.kuazoo.City city = new com.kuazoo.City();
                city.CityId = Merchant.CityId;
                mer.City = city;
                mer.PostCode = Merchant.PostCode;
                mer.ContactNumber = Merchant.ContactNumber;
                mer.Email = Merchant.Email;
                mer.Website = Merchant.Website;
                mer.Facebook = Merchant.Facebook;
                try
                {
                    string coord = collection.Get("TempLatLong");
                    char[] delimChars = { ',' };
                    string[] seperated = coord.Split(delimChars);
                    //check if
                    int length1 = seperated[0].Length;
                    int length2 = seperated[1].Length;
                    mer.Latitude = float.Parse(seperated[0].Substring(1, length1 - 1));
                    mer.Longitude = float.Parse(seperated[1].Substring(0, length2 - 2));
                }
                catch
                {
                    mer.Latitude = 0;
                    mer.Longitude = 0;
                }
                com.kuazoo.Status stat = new com.kuazoo.Status();
                stat.StatusId = Merchant.StatusId;
                mer.Status = stat;
                if (Merchant.Description != null)
                {
                    string desc = Microsoft.JScript.GlobalObject.decodeURI(Merchant.Description);
                    mer.Description = desc;
                }
                else { mer.Description = ""; }
                if (Merchant.SubImageId != null && Merchant.SubImageId != "")
                {
                    List<int> subimg = new List<int>();
                    List<string> subimglink = new List<string>();
                    String[] subimagelist = Merchant.SubImageId.Split(',');
                    foreach (var v in subimagelist)
                    {
                        if (v != "")
                        {
                            subimg.Add(int.Parse(v));
                            subimglink.Add(collection.Get("txtSubImg" + v));
                        }
                    }
                    mer.SubImageId = subimg;
                    mer.SubImageUrlLink = subimglink;
                }

                bool result = merchantservice.CreateMerchant(mer).Result;

                if (result == true)
                {
                    ViewBag.result = "1";
                    TempData["msg"] = "11";
                }
                else
                {
                    ViewBag.result = "3";
                    TempData["msg"] = "33";
                }
                return RedirectToAction("Merchant", "Admin");
            }
            catch (Exception ex)
            {
                CustomException(ex);
                ViewBag.result = ex.Message;
            }
            return View(Merchant);
        }
        public ActionResult MerchantView(int MerchantId)
        {
            com.kuazoo.Merchant v = merchantservice.GetMerchantById(MerchantId).Result;
            MerchantModel merchant = new MerchantModel() { MerchantId = v.MerchantId, Name = v.Name, CountryId = v.Country.CountryId, Country = new MasterModel.Country() { CountryId = v.Country.CountryId, CountryName = v.Country.Name }, AddressLine1 = v.AddressLine1, AddressLine2 = v.AddressLine2, CityId = v.City.CityId, City = new MasterModel.City() { CityId = v.City.CityId, CityName = v.City.Name }, PostCode = v.PostCode, ContactNumber = v.ContactNumber, Email = v.Email, Website = v.Website, Facebook = v.Facebook, Latitude = v.Latitude, Longitude = v.Longitude, StatusId = v.Status.StatusId, LastAction = v.LastAction = v.LastAction, Description = v.Description, SubImageId = String.Join(",", v.SubImageId), SubImageName = String.Join("|", v.SubImageName), SubImageUrl = String.Join("|", v.SubImageUrl), SubImageUrlLink = String.Join("|", v.SubImageUrlLink) };
            return View(merchant);
        }
        #endregion

        #region InventoryItem
        public ActionResult InventoryItem()
        {
            //List<com.kuazoo.Tag> listtag = inventoryitemservice.GetTagList().Result;
            //List<InventoryItemModel.Tag> newlisttag = new List<InventoryItemModel.Tag>();
            //foreach (var v in listtag)
            //{
            //    newlisttag.Add(new InventoryItemModel.Tag() { TagId = v.TagId, TagName = v.Name });
            //}
            //ViewData["taglist"] = newlisttag;
            if (TempData["msg"] != null)
            {
                ViewBag.msg = TempData["msg"];
            }
            return View();
        }
        public ActionResult InventoryItem_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<com.kuazoo.InventoryItem> listmer = inventoryitemservice.GetInventoryItemList().Result;
            List<InventoryItemModel.InventoryItem> newlistmer = new List<InventoryItemModel.InventoryItem>();
            Boolean kuazooed = false;
            string name = "";
            foreach (var v in listmer)
            {
                kuazooed = false;
                if (v.SalesVisualMeter >= 100 || v.RemainSales < 1) kuazooed = true;
                name = v.Name;
                if (v.Draft)
                {
                    name = "Draft - " +name;
                }
                //newlistmer.Add(new InventoryItemModel.InventoryItem() { Create = v.Create.AddHours(Helper.defaultGMT), InventoryItemId = v.InventoryItemId, Name = v.Name, ShortDesc = v.ShortDesc, GeneralDescription = v.GeneralDescription, Price = v.Price, Margin = v.Margin, Description = v.Description, Terms = v.Terms, MerchantId = v.Merchant.MerchantId, MerchantName = v.Merchant.Name, MerchantCityName = v.Merchant.City.Name, MerchantCountryName = v.Merchant.Country.Name, CountryId = v.Country.CountryId, Country = new MasterModel.Country() { CountryId = v.Country.CountryId, CountryName = v.Country.Name }, CityId = v.City.CityId, City = new MasterModel.City() { CityId = v.City.CityId, CityName = v.City.Name }, Keyword = v.Keyword, InventoryItemTypeId = v.InventoryItemType.InventoryItemTypeId, InventoryItemTypeName = v.InventoryItemType.InventoryItemTypeName, Discount = v.Discount, ExpireDate = v.ExpireDate.AddHours(Helper.defaultGMT), MaximumSales = v.MaximumSales, RemainSales = v.RemainSales, PublishDate = v.PublishDate.AddHours(Helper.defaultGMT), MinimumTarget = v.MinimumTarget, SalesVisualMeter = v.SalesVisualMeter, LastAction = v.LastAction, Tag = v.Tag, TagName = v.TagName, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = String.Join("|", v.SubImageId), SubImageName = String.Join("|", v.SubImageName), SubImageUrl = String.Join("|", v.SubImageUrl), Flag = v.Flag, Featured = v.Featured, PrizeId = v.Prize.PrizeId, PrizeName = v.Prize.Name, Variance = String.Join("|", v.Variance) });
                newlistmer.Add(new InventoryItemModel.InventoryItem() { Create = v.Create.AddHours(Helper.defaultGMT), Priority = v.Priority, InventoryItemId = v.InventoryItemId, Name = name, MerchantId = v.Merchant.MerchantId, MerchantName = v.Merchant.Name, MerchantCityName = v.Merchant.City.Name, MerchantCountryName = v.Merchant.Country.Name, InventoryItemTypeId = v.InventoryItemType.InventoryItemTypeId, InventoryItemTypeName = v.InventoryItemType.InventoryItemTypeName, LastAction = v.LastAction, Flag = v.Flag, PrizeName = v.Prize!=null?v.Prize.Name:string.Empty,Kuazooed = kuazooed, Popular = v.Popular, Reviewed = v.Reviewed });
               
                //if ( v.Prize != null)
                //{
                //    newlistmer.Add(new InventoryItemModel.InventoryItem() { InventoryItemId = v.InventoryItemId, Name = v.Name, ShortDesc=v.ShortDesc, Price = v.Price, Description = v.Description, MerchantId = v.Merchant.MerchantId, MerchantName = v.Merchant.Name, MerchantCityName = v.Merchant.City.Name, MerchantCountryName = v.Merchant.Country.Name,  CountryId = v.Country.CountryId, Country = new MasterModel.Country() { CountryId = v.Country.CountryId, CountryName = v.Country.Name }, CityId = v.City.CityId, City = new MasterModel.City() { CityId = v.City.CityId, CityName = v.City.Name }, Keyword = v.Keyword, InventoryItemTypeId = v.InventoryItemType.InventoryItemTypeId, InventoryItemTypeName = v.InventoryItemType.InventoryItemTypeName, Discount = v.Discount, ExpireDate = v.ExpireDate, MaximumSales = v.MaximumSales, PublishDate = v.PublishDate, MinimumTarget = v.MinimumTarget, SalesVisualMeter=v.SalesVisualMeter, LastAction = v.LastAction, Tag = v.Tag, TagName = v.TagName, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = String.Join("|", v.SubImageId), SubImageName = String.Join("|", v.SubImageName), SubImageUrl = String.Join("|", v.SubImageUrl), Flag = v.Flag, Prize = new InventoryItemModel.Prize() { PrizeId = v.Prize.PrizeId, Name = v.Prize.Name, Description = v.Prize.Description, ImageId = v.Prize.ImageId, ImageName = v.Prize.ImageName, ImageUrl = v.Prize.ImageUrl, SubImageId = String.Join(",", v.Prize.SubImageId), SubImageName = String.Join("|", v.Prize.SubImageName), SubImageUrl = String.Join("|", v.Prize.SubImageUrl), Price = v.Prize.Price, SponsorName = v.Prize.SponsorName, Detail = v.Prize.Detail, Terms = v.Prize.Terms } });
                //}
                //else
                //{
                //    newlistmer.Add(new InventoryItemModel.InventoryItem() { InventoryItemId = v.InventoryItemId, Name = v.Name, ShortDesc=v.ShortDesc, Price = v.Price, Description = v.Description, MerchantId = v.Merchant.MerchantId, MerchantName = v.Merchant.Name, MerchantCityName = v.Merchant.City.Name, MerchantCountryName = v.Merchant.Country.Name,  CountryId = v.Country.CountryId, Country = new MasterModel.Country() { CountryId = v.Country.CountryId, CountryName = v.Country.Name }, CityId = v.City.CityId, City = new MasterModel.City() { CityId = v.City.CityId, CityName = v.City.Name }, Keyword = v.Keyword, InventoryItemTypeId = v.InventoryItemType.InventoryItemTypeId, InventoryItemTypeName = v.InventoryItemType.InventoryItemTypeName, Discount = v.Discount, ExpireDate = v.ExpireDate, MaximumSales = v.MaximumSales, PublishDate = v.PublishDate, MinimumTarget = v.MinimumTarget, SalesVisualMeter = v.SalesVisualMeter, LastAction = v.LastAction, Tag = v.Tag, TagName = v.TagName, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = String.Join("|", v.SubImageId), SubImageName = String.Join("|", v.SubImageName), SubImageUrl = String.Join("|", v.SubImageUrl), Flag = v.Flag });
                //}
            }
            IEnumerable<InventoryItemModel.InventoryItem> ienuList = newlistmer;
            var result = ienuList.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public String InventoryItem_Duplicate(int InventoryItemId)
        {
            string res = "";
            if (InventoryItemId != null && InventoryItemId>0)
            {
                try
                {
                    Boolean result = inventoryitemservice.DuplicateInventoryItem(InventoryItemId).Result;

                    if (result) res = "1";
                    else res = "2";
                }
                catch (Exception ex)
                {
                    CustomException(ex);
                    res = ex.Message;
                }

            }

            return res;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InventoryItem_Destroy([DataSourceRequest] DataSourceRequest request, InventoryItemModel.InventoryItem InventoryItem)
        {
            if (InventoryItem != null)
            {
                Boolean result = inventoryitemservice.DeleteInventoryItem(InventoryItem.InventoryItemId).Result;

                if (result) ViewBag.result = "1";
                else ViewBag.result = "2";
                ModelState.AddModelError("", ViewBag.result);

            }

            return Json(ModelState.ToDataSourceResult());
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InventoryItem_Update([DataSourceRequest] DataSourceRequest request, InventoryItemModel.InventoryItem InventoryItem)
        {
            if (InventoryItem != null)
            {
                int InventoryPriority = InventoryItem.Priority!=null ? InventoryItem.Priority<=0 ? 99999 : (int)InventoryItem.Priority : 99999;
                Boolean result = inventoryitemservice.UpdateInventoryItem(InventoryItem.InventoryItemId, InventoryPriority, InventoryItem.Popular, InventoryItem.Reviewed).Result;

                if (result) ViewBag.result = "3";
                else ViewBag.result = "4";
                ModelState.AddModelError("", ViewBag.result);
            }

            return Json(ModelState.ToDataSourceResult());
        }

        public ActionResult InventoryItemAdd(int PrizeId=0)
        {
            ViewBag.PrizeId = 0;
            if (PrizeId != 0)
            {
                ViewBag.PrizeId = PrizeId;
                var prize = prizeservice.GetPrizeById(PrizeId).Result;
                if (prize != null)
                {
                    ViewBag.ExpiryDate = prize.ExpiryDate.AddHours(Kuazoo.Helper.defaultGMT);
                    ViewBag.PublishDate = prize.PublishDate.AddHours(Kuazoo.Helper.defaultGMT);
                }
            } 
            return View();
        }
        public ActionResult InventoryItemSample()
        {
            return View();
        }

        public ActionResult InventoryItemHomeSample(int? page, int id = 0, string c = "")
        {
            TempData["msg"] = null;
            int pageSize = Helper.pageSize;
            int pageNumber = (page ?? 0);
            int totalCount = 0;
            ViewBag.cityid = id;
            ViewBag.category = c;
            DateTime today = DateTime.UtcNow;
            List<com.kuazoo.InventoryItemShow> listmer = inventoryitemservice.GetInventoryItemListByCity(id, today, ref totalCount, c, pageSize, pageNumber).Result;
            List<InventoryItemModel.InventoryItemShow> newlistmer = new List<InventoryItemModel.InventoryItemShow>();
            foreach (var v in listmer)
            {
                if (v.Prize != null && v.FlashDeal != null)
                {
                    newlistmer.Add(new InventoryItemModel.InventoryItemShow() { InventoryItemId = v.InventoryItemId, Wishlist = v.Wishlist, Name = v.Name, ShortDesc = v.ShortDesc, Description = v.Description, Price = v.Price, TypeId = v.TypeId, TypeName = v.TypeName, Discount = v.Discount, MaximumSales = v.MaximumSales, Sales = v.Sales, SalesVisualMeter = v.SalesVisualMeter, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = v.SubImageId, SubImageName = v.SubImageName, SubImageUrl = v.SubImageUrl, Tag = v.Tag, Prize = new InventoryItemModel.PrizeShow() { PrizeId = v.Prize.PrizeId, GameType = v.Prize.GameType, Name = v.Prize.Name, Description = v.Prize.Description, ImageId = v.Prize.ImageId, ImageName = v.Prize.ImageName, ImageUrl = v.Prize.ImageUrl, SubImageId = v.Prize.SubImageId, SubImageName = v.Prize.SubImageName, SubImageUrl = v.Prize.SubImageUrl, Price = v.Prize.Price, SponsorName = v.Prize.SponsorName, Detail = v.Prize.Detail, Terms = v.Prize.Terms, ExpiryDate = v.Prize.ExpiryDate, ExpiryDateStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.Prize.ExpiryDate), GroupName = v.Prize.GroupName }, FlashDeal = new InventoryItemModel.FlashDeal() { FlashDealId = v.FlashDeal.FlashDealId, Discount = v.FlashDeal.Discount, MerchantId = v.FlashDeal.MerchantId, MerchantName = v.FlashDeal.MerchantName, InventoryItemId = v.FlashDeal.InventoryItemId, InventoryItemName = v.FlashDeal.InventoryItemName, StartTime = v.FlashDeal.StartTime, StartTimeStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.FlashDeal.StartTime), EndTime = v.FlashDeal.EndTime, EndTimeStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.FlashDeal.EndTime) } });
                }
                else if (v.Prize != null && v.FlashDeal == null)
                {
                    newlistmer.Add(new InventoryItemModel.InventoryItemShow() { InventoryItemId = v.InventoryItemId, Wishlist = v.Wishlist, Name = v.Name, ShortDesc = v.ShortDesc, Description = v.Description, Price = v.Price, TypeId = v.TypeId, TypeName = v.TypeName, Discount = v.Discount, MaximumSales = v.MaximumSales, Sales = v.Sales, SalesVisualMeter = v.SalesVisualMeter, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = v.SubImageId, SubImageName = v.SubImageName, SubImageUrl = v.SubImageUrl, Tag = v.Tag, Prize = new InventoryItemModel.PrizeShow() { PrizeId = v.Prize.PrizeId, GameType = v.Prize.GameType, Name = v.Prize.Name, Description = v.Prize.Description, ImageId = v.Prize.ImageId, ImageName = v.Prize.ImageName, ImageUrl = v.Prize.ImageUrl, SubImageId = v.Prize.SubImageId, SubImageName = v.Prize.SubImageName, SubImageUrl = v.Prize.SubImageUrl, Price = v.Prize.Price, SponsorName = v.Prize.SponsorName, Detail = v.Prize.Detail, Terms = v.Prize.Terms, ExpiryDate = v.Prize.ExpiryDate, ExpiryDateStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.Prize.ExpiryDate), GroupName = v.Prize.GroupName } });
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

        [HttpPost]
        public ActionResult InventoryItemAdd(FormCollection collection, InventoryItemModel.InventoryItem InventoryItem, int PrizeId=0)
        {
            if (InventoryItem.Priority == null)
                InventoryItem.Priority = 99999;

            string submitbtn = collection.Get("_submit");
            bool check = true;
            if (submitbtn != "2")
            {
                if (ModelState.IsValid == false)
                {
                    var errors = ModelState
        .Where(x => x.Value.Errors.Count > 0)
        .Select(x => new { x.Key, x.Value.Errors })
        .ToArray();
                    ViewBag.errors = errors;
                    check = false;
                }
            }
            if (check)
            {
                try
                {
                    //string imageurlprize = "";
                    //if (imageUploadPrize != null)
                    //{
                    //    if (!Helper.IsValidImage(imageUploadPrize.FileName))
                    //    {
                    //        throw new Exception("File is not an image file");
                    //    }
                    //    string name = InventoryItem.Name;
                    //    if (name == null) name = "";
                    //    name = "Prize-" + name;
                    //    //imageurl = Helper.uploadImage(imageUpload);
                    //    imageurlprize = Helper.uploadImageWithName(imageUploadPrize, name);
                    //}
                    com.kuazoo.InventoryItem pro = new com.kuazoo.InventoryItem();
                    pro.InventoryItemId = 0;
                    pro.Name = InventoryItem.Name;
                    pro.ShortDesc = InventoryItem.ShortDesc;
                    com.kuazoo.Currency curr = new com.kuazoo.Currency();
                    pro.Price = InventoryItem.Price;
                    pro.Margin = InventoryItem.Margin;

                    if (InventoryItem.Description != null)
                    {
                        string desc = Microsoft.JScript.GlobalObject.decodeURI(InventoryItem.Description);
                        pro.Description = desc;
                    }
                    else { pro.Description = ""; }
                    if (InventoryItem.GeneralDescription != null)
                    {
                        string generaldesc = Microsoft.JScript.GlobalObject.decodeURI(InventoryItem.GeneralDescription);
                        pro.GeneralDescription = generaldesc;
                    }
                    else { pro.GeneralDescription = ""; }

                    if (InventoryItem.Terms != null)
                    {
                        string terms = Microsoft.JScript.GlobalObject.decodeURI(InventoryItem.Terms);
                        pro.Terms = terms;
                    }
                    else { pro.Terms = ""; }

                    com.kuazoo.Merchant merchant = new com.kuazoo.Merchant();
                    merchant.MerchantId = InventoryItem.MerchantId;
                    pro.Merchant = merchant;
                    com.kuazoo.City city = new com.kuazoo.City();
                    city.CityId = Convert.ToInt16(collection.Get("CityDropDown").ToString());
                    pro.City = city;
                    pro.Keyword = InventoryItem.Keyword;
                    com.kuazoo.InventoryItemType InventoryItemtype = new com.kuazoo.InventoryItemType();
                    InventoryItemtype.InventoryItemTypeId = InventoryItem.InventoryItemTypeId;
                    pro.InventoryItemType = InventoryItemtype;
                    pro.Discount = InventoryItem.Discount;
                    pro.ExpireDate = InventoryItem.ExpireDate.AddHours(-1 * Helper.defaultGMT);
                    pro.MaximumSales = InventoryItem.MaximumSales;
                    pro.RemainSales = InventoryItem.RemainSales;
                    pro.PublishDate = InventoryItem.PublishDate.AddHours(-1 * Helper.defaultGMT);
                    pro.MinimumTarget = InventoryItem.MinimumTarget;
                    pro.SalesVisualMeter = InventoryItem.SalesVisualMeter;
                    string tag = collection.Get("InventoryItemTagDropDown");
                    if (tag != null || tag != "")
                    {
                        pro.Tag = tag;
                    }
                    pro.ImageId = 0;
                    if (InventoryItem.ImageId != 0)
                    {
                        pro.ImageId = InventoryItem.ImageId;
                    }
                    if (InventoryItem.SubImageId != null && InventoryItem.SubImageId != "")
                    {
                        List<int> subimg = new List<int>();
                        String[] subimagelist = InventoryItem.SubImageId.Split(',');
                        foreach (var v in subimagelist)
                        {
                            if (v != "")
                            {
                                subimg.Add(int.Parse(v));
                            }
                        }
                        pro.SubImageId = subimg;
                    }
                    int checkvari = 0;
                    if (InventoryItem.Variance != null && InventoryItem.Variance != "")
                    {
                        List<VarianceDetail> listvariance = new List<VarianceDetail>();
                        String[] _listvar = InventoryItem.Variance.Split('|');
                        foreach (var v in _listvar)
                        {
                            if (v != "")
                            {
                                try
                                {
                                    checkvari = 1;
                                    string[] _var = v.Split(',');
                                    listvariance.Add(new VarianceDetail() { Variance = _var[0], Price = decimal.Parse(_var[1]), Discount = decimal.Parse(_var[2]), Margin = decimal.Parse(_var[3]), AvailabelLimit = int.Parse(_var[4]) });
                                }
                                catch
                                {
                                }
                            }
                        }
                        pro.Variance = listvariance;
                    }
                    if (checkvari == 0)
                    {
                        List<VarianceDetail> listvariance = new List<VarianceDetail>();
                        if (InventoryItem.VarianceName != null && InventoryItem.Variance != "")
                        {
                            listvariance.Add(new VarianceDetail() { Variance = Helper.ReplaceSymbol(InventoryItem.VarianceName), Price = InventoryItem.Price, Discount = InventoryItem.Discount, Margin = InventoryItem.Margin, AvailabelLimit = InventoryItem.Limit });
                            pro.Variance = listvariance;
                        }
                        else
                        {
                            listvariance.Add(new VarianceDetail() { Variance = Helper.ReplaceSymbol(InventoryItem.Name), Price = InventoryItem.Price, Discount = InventoryItem.Discount, Margin = InventoryItem.Margin, AvailabelLimit = InventoryItem.Limit });
                            pro.Variance = listvariance;
                        }
                    }
                    pro.Flag = true;//InventoryItem.Flag;
                    //pro.Featured =  InventoryItem.Featured;
                    pro.PrizeId = InventoryItem.PrizeId;

                    pro.Priority = (int)InventoryItem.Priority;

                    if (submitbtn == "1")
                    {
                        com.kuazoo.FlashDeal fd = new com.kuazoo.FlashDeal();
                        if (InventoryItem.FlashDeal != null && InventoryItem.FlashDeal.Discount != 0)
                        {
                            fd.FlashDealId = 0;
                            fd.Discount = InventoryItem.FlashDeal.Discount;
                            fd.StartTime = InventoryItem.FlashDeal.StartTime.AddHours(-1 * Helper.defaultGMT);
                            fd.EndTime = InventoryItem.FlashDeal.EndTime.AddHours(-1 * Helper.defaultGMT);
                            fd.Flag = true;
                            pro.FlashDeal = fd;
                        }
                    }
                    //List<com.kuazoo.FlashDeal> fdlist = new List<com.kuazoo.FlashDeal>();
                    //string[] fddis = InventoryItem.FlashDealDraft.Discount.Split('|');
                    //string[] fdstart = InventoryItem.FlashDealDraft.StartTime.Split('|');
                    //string[] fdend = InventoryItem.FlashDealDraft.EndTime.Split('|');
                    //for(int i=0;i<fddis.Length;i++)
                    //{
                    //    if (fddis[i] != "")
                    //    {
                    //        com.kuazoo.FlashDeal fd = new com.kuazoo.FlashDeal();
                    //        fd.Discount = decimal.Parse(fddis[i]);
                    //        fd.StartTime = DateTime.Parse(fdstart[i]);
                    //        fd.EndTime = DateTime.Parse(fdend[i]);
                    //        fd.Flag = true;
                    //        fdlist.Add(fd);
                    //    }
                    //}
                    //pro.FlashDeal = fdlist;
                    pro.Draft = false;
                    if (submitbtn == "2")
                    {
                        pro.Draft = true;
                    }
                    bool result = inventoryitemservice.CreateInventoryItem(pro).Result;
                    if (result == true)
                    {
                        ViewBag.result = "1";
                    }
                    else
                    {
                        ViewBag.result = "3";
                    }
                    TempData["msg"] = ViewBag.result;
                    if (PrizeId == 0)
                    {
                        return RedirectToAction("InventoryItem", "Admin");
                    }
                    else
                    {
                        return new RedirectResult(Url.Action("PrizeView", "Admin", new { PrizeId = PrizeId }) + "#inventory");
                    }
                }
                catch (Exception ex)
                {
                    CustomException(ex);
                    ViewBag.result = ex.Message;
                }
            }
            ViewBag.PrizeId = 0;
            if (PrizeId != 0)
            {
                ViewBag.PrizeId = PrizeId;
            }
            return View(InventoryItem);

        }

        public ActionResult InventoryItemEdit(int InventoryItemId, int PrizeId=0)
        {
            com.kuazoo.InventoryItem v = inventoryitemservice.GetInventoryItemById(InventoryItemId).Result;
            InventoryItemModel.InventoryItem InventoryItem;
            List<string> variance = new List<string>();
            List<string> tempvariance = new List<string>();
            foreach (var vari in v.Variance)
            {
                variance.Add(vari.Variance + "," + vari.Price + "," + vari.Discount+","+vari.Margin+","+vari.AvailabelLimit);
                tempvariance.Add(vari.Variance);
            }
            if (v.Prize != null)
            {
                InventoryItem = new InventoryItemModel.InventoryItem() { Priority = v.Priority, Draft=v.Draft, InventoryItemId = v.InventoryItemId, Name = v.Name, ShortDesc = v.ShortDesc, GeneralDescription = v.GeneralDescription, Price = v.Price, Margin = v.Margin, Description = v.Description, Terms = v.Terms, MerchantId = v.Merchant.MerchantId, MerchantName = v.Merchant.Name, MerchantCityName = v.Merchant.City.Name, MerchantCountryName = v.Merchant.Country.Name, CountryId = v.Country.CountryId, Country = new MasterModel.Country() { CountryId = v.Country.CountryId, CountryName = v.Country.Name }, CityId = v.City.CityId, City = new MasterModel.City() { CityId = v.City.CityId, CityName = v.City.Name }, Keyword = v.Keyword, InventoryItemTypeId = v.InventoryItemType.InventoryItemTypeId, InventoryItemTypeName = v.InventoryItemType.InventoryItemTypeName, Discount = v.Discount, ExpireDate = v.ExpireDate.AddHours(Helper.defaultGMT), MaximumSales = v.MaximumSales, RemainSales = v.RemainSales, PublishDate = v.PublishDate.AddHours(Helper.defaultGMT), MinimumTarget = v.MinimumTarget, SalesVisualMeter = v.SalesVisualMeter, Tag = v.Tag, TagName = v.TagName, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = String.Join(",", v.SubImageId), SubImageName = String.Join("|", v.SubImageName), SubImageUrl = String.Join("|", v.SubImageUrl), Flag = v.Flag, Featured = v.Featured, PrizeId = v.Prize.PrizeId, PrizeName = v.Prize.Name, Variance = String.Join("|", variance) + "|", TempVariance = string.Join("|", tempvariance) + "|" };
            }
            else
            {
                InventoryItem = new InventoryItemModel.InventoryItem() { Priority = v.Priority, Draft = v.Draft, InventoryItemId = v.InventoryItemId, Name = v.Name, ShortDesc = v.ShortDesc, GeneralDescription = v.GeneralDescription, Price = v.Price, Margin = v.Margin, Description = v.Description, Terms = v.Terms, MerchantId = v.Merchant.MerchantId, MerchantName = v.Merchant.Name, MerchantCityName = v.Merchant.City.Name, MerchantCountryName = v.Merchant.Country.Name, CountryId = v.Country.CountryId, Country = new MasterModel.Country() { CountryId = v.Country.CountryId, CountryName = v.Country.Name }, CityId = v.City.CityId, City = new MasterModel.City() { CityId = v.City.CityId, CityName = v.City.Name }, Keyword = v.Keyword, InventoryItemTypeId = v.InventoryItemType.InventoryItemTypeId, InventoryItemTypeName = v.InventoryItemType.InventoryItemTypeName, Discount = v.Discount, ExpireDate = v.ExpireDate.AddHours(Helper.defaultGMT), MaximumSales = v.MaximumSales, RemainSales = v.RemainSales, PublishDate = v.PublishDate.AddHours(Helper.defaultGMT), MinimumTarget = v.MinimumTarget, SalesVisualMeter = v.SalesVisualMeter, Tag = v.Tag, TagName = v.TagName, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = String.Join(",", v.SubImageId), SubImageName = String.Join("|", v.SubImageName), SubImageUrl = String.Join("|", v.SubImageUrl), Flag = v.Flag, Featured = v.Featured, Variance = String.Join("|", variance) + "|", TempVariance = string.Join("|", tempvariance) + "|" };
            }
            //if (v.Prize != null)
            //{
            //    InventoryItem = new InventoryItemModel.InventoryItem() { InventoryItemId = v.InventoryItemId, Name = v.Name, ShortDesc=v.ShortDesc, Price = v.Price, Description = v.Description, MerchantId = v.Merchant.MerchantId, MerchantName = v.Merchant.Name, MerchantCityName = v.Merchant.City.Name, MerchantCountryName = v.Merchant.Country.Name,  CountryId = v.Country.CountryId, Country = new MasterModel.Country() { CountryId = v.Country.CountryId, CountryName = v.Country.Name }, CityId = v.City.CityId, City = new MasterModel.City() { CityId = v.City.CityId, CityName = v.City.Name }, Keyword = v.Keyword, InventoryItemTypeId = v.InventoryItemType.InventoryItemTypeId, InventoryItemTypeName = v.InventoryItemType.InventoryItemTypeName, Discount = v.Discount, ExpireDate = v.ExpireDate, MaximumSales = v.MaximumSales, PublishDate = v.PublishDate, MinimumTarget = v.MinimumTarget, SalesVisualMeter = v.SalesVisualMeter, LastAction = v.LastAction, Tag = v.Tag, TagName = v.TagName, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = String.Join(",", v.SubImageId), SubImageName = String.Join("|", v.SubImageName), SubImageUrl = String.Join("|", v.SubImageUrl), Flag = v.Flag, Prize = new InventoryItemModel.Prize() { PrizeId = v.Prize.PrizeId, Name = v.Prize.Name, Description = v.Prize.Description, ImageId = v.Prize.ImageId, ImageName = v.Prize.ImageName, ImageUrl = v.Prize.ImageUrl, SubImageId = String.Join(",", v.Prize.SubImageId), SubImageName = String.Join("|", v.Prize.SubImageName), SubImageUrl = String.Join("|", v.Prize.SubImageUrl), Price = v.Prize.Price, SponsorName = v.Prize.SponsorName, Detail = v.Prize.Detail, Terms = v.Prize.Terms } };
            //}
            //else
            //{
            //    InventoryItem = new InventoryItemModel.InventoryItem() { InventoryItemId = v.InventoryItemId, Name = v.Name, ShortDesc=v.ShortDesc, Price = v.Price, Description = v.Description, MerchantId = v.Merchant.MerchantId, MerchantName = v.Merchant.Name, MerchantCityName = v.Merchant.City.Name, MerchantCountryName = v.Merchant.Country.Name,  CountryId = v.Country.CountryId, Country = new MasterModel.Country() { CountryId = v.Country.CountryId, CountryName = v.Country.Name }, CityId = v.City.CityId, City = new MasterModel.City() { CityId = v.City.CityId, CityName = v.City.Name }, Keyword = v.Keyword, InventoryItemTypeId = v.InventoryItemType.InventoryItemTypeId, InventoryItemTypeName = v.InventoryItemType.InventoryItemTypeName, Discount = v.Discount, ExpireDate = v.ExpireDate, MaximumSales = v.MaximumSales, PublishDate = v.PublishDate, MinimumTarget = v.MinimumTarget, SalesVisualMeter = v.SalesVisualMeter, Tag = v.Tag, TagName = v.TagName, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = String.Join(",", v.SubImageId), SubImageName = String.Join("|", v.SubImageName), SubImageUrl = String.Join("|", v.SubImageUrl), Flag = v.Flag };
            //}

            ViewBag.PrizeId = 0;
            if (PrizeId != 0)
            {
                ViewBag.PrizeId = PrizeId;
                var prize = prizeservice.GetPrizeById(PrizeId).Result;
                if (prize != null)
                {
                    ViewBag.ExpiryDate = prize.ExpiryDate.AddHours(Kuazoo.Helper.defaultGMT);
                    ViewBag.PublishDate = prize.PublishDate.AddHours(Kuazoo.Helper.defaultGMT);
                }
            } 
            return View(InventoryItem);
        }
        [HttpPost]
        public ActionResult InventoryItemEdit(HttpPostedFileBase imageUpload,HttpPostedFileBase imageUploadPrize, FormCollection collection, InventoryItemModel.InventoryItem InventoryItem, int PrizeId=0)
        {
            int InventoryPriority = InventoryItem.Priority != null ? InventoryItem.Priority <= 0 ? 99999 : (int)InventoryItem.Priority : 99999;
            InventoryItem.Priority = InventoryPriority;

            string submitbtn = collection.Get("_submit");
            bool check = true;
            if (submitbtn != "2")
            {
                if (ModelState.IsValid == false)
                {
                    var errors = ModelState
        .Where(x => x.Value.Errors.Count > 0)
        .Select(x => new { x.Key, x.Value.Errors })
        .ToArray();
                    ViewBag.errors = errors;
                    check = false;
                }
            }
            if (check)
            {
                try
                {


                    string imageurl = "";
                    string imageurlprize = "";
                    if (imageUpload != null)
                    {
                        if (!Helper.IsValidImage(imageUpload.FileName))
                        {
                            throw new Exception("File is not an image file");
                        }
                        string name = InventoryItem.Name;
                        if (name == null) name = "";
                        //imageurl = Helper.uploadImage(imageUpload);
                        imageurl = Helper.uploadImageWithName(imageUpload, name);
                    }
                    if (imageUploadPrize != null)
                    {
                        if (!Helper.IsValidImage(imageUploadPrize.FileName))
                        {
                            throw new Exception("File is not an image file");
                        }
                        string name = InventoryItem.Name;
                        if (name == null) name = "";
                        name = "Prize-" + name;
                        //imageurl = Helper.uploadImage(imageUpload);
                        imageurlprize = Helper.uploadImageWithName(imageUploadPrize, name);
                    }
                    com.kuazoo.InventoryItem pro = new com.kuazoo.InventoryItem();
                    pro.InventoryItemId = InventoryItem.InventoryItemId;
                    pro.Name = InventoryItem.Name;
                    pro.ShortDesc = InventoryItem.ShortDesc;
                    com.kuazoo.Currency curr = new com.kuazoo.Currency();
                    pro.Price = InventoryItem.Price;
                    pro.Margin = InventoryItem.Margin;

                    if (InventoryItem.Description != null)
                    {
                        string desc = Microsoft.JScript.GlobalObject.decodeURI(InventoryItem.Description);
                        pro.Description = desc;
                    }
                    else { pro.Description = ""; }
                    if (InventoryItem.GeneralDescription != null)
                    {
                        string generaldesc = Microsoft.JScript.GlobalObject.decodeURI(InventoryItem.GeneralDescription);
                        pro.GeneralDescription = generaldesc;
                    }
                    else { pro.GeneralDescription = ""; }

                    if (InventoryItem.Terms != null)
                    {
                        string terms = Microsoft.JScript.GlobalObject.decodeURI(InventoryItem.Terms);
                        pro.Terms = terms;
                    }
                    else { pro.Terms = ""; }

                    com.kuazoo.Merchant merchant = new com.kuazoo.Merchant();
                    merchant.MerchantId = InventoryItem.MerchantId;
                    pro.Merchant = merchant;
                    com.kuazoo.City city = new com.kuazoo.City();
                    city.CityId = Convert.ToInt16(collection.Get("CityDropDown").ToString());
                    pro.City = city;
                    pro.Keyword = InventoryItem.Keyword;
                    com.kuazoo.InventoryItemType InventoryItemtype = new com.kuazoo.InventoryItemType();
                    InventoryItemtype.InventoryItemTypeId = InventoryItem.InventoryItemTypeId;
                    pro.InventoryItemType = InventoryItemtype;
                    pro.Discount = InventoryItem.Discount;
                    pro.ExpireDate = InventoryItem.ExpireDate.AddHours(-1 * Helper.defaultGMT);
                    pro.MaximumSales = InventoryItem.MaximumSales;
                    pro.RemainSales = InventoryItem.RemainSales;
                    pro.PublishDate = InventoryItem.PublishDate.AddHours(-1 * Helper.defaultGMT);
                    pro.MinimumTarget = InventoryItem.MinimumTarget;
                    pro.SalesVisualMeter = InventoryItem.SalesVisualMeter;
                    pro.Tag = InventoryItem.Tag;
                    pro.Priority = (int)InventoryItem.Priority;
                    pro.ImageId = 0;
                    if (InventoryItem.ImageId != 0)
                    {
                        pro.ImageId = InventoryItem.ImageId;
                    }
                    if (InventoryItem.SubImageId != null && InventoryItem.SubImageId != "")
                    {
                        List<int> subimg = new List<int>();
                        String[] subimagelist = InventoryItem.SubImageId.Split(',');
                        foreach (var v in subimagelist)
                        {
                            if (v != "")
                            {
                                subimg.Add(int.Parse(v));
                            }
                        }
                        pro.SubImageId = subimg;
                    }


                    string tag = collection.Get("InventoryItemTagDropDown");
                    if (tag != null || tag != "")
                    {
                        pro.Tag = tag;
                    }
                    pro.Flag = InventoryItem.Flag;
                    //pro.Featured = InventoryItem.Featured;
                    pro.PrizeId = InventoryItem.PrizeId;

                    int checkvari = 0;
                    if (InventoryItem.Variance != null && InventoryItem.Variance != "")
                    {
                        List<VarianceDetail> listvariance = new List<VarianceDetail>();
                        String[] _listvar = InventoryItem.Variance.Split('|');
                        foreach (var v in _listvar)
                        {
                            if (v != "")
                            {
                                try
                                {
                                    checkvari = 1;
                                    string[] _var = v.Split(',');
                                    listvariance.Add(new VarianceDetail() { Variance = _var[0], Price = decimal.Parse(_var[1]), Discount = decimal.Parse(_var[2]), Margin = decimal.Parse(_var[3]), AvailabelLimit = int.Parse(_var[4]) });
                                }
                                catch
                                {
                                }
                            }
                        }
                        pro.Variance = listvariance;
                    }
                    if(checkvari==0)
                    {
                        List<VarianceDetail> listvariance = new List<VarianceDetail>();
                        listvariance.Add(new VarianceDetail() { Variance = InventoryItem.VarianceName.Replace("`", "").Replace(",", "").Replace("\"","").Replace("'", ""), Price = InventoryItem.Price, Discount = InventoryItem.Discount, Margin = InventoryItem.Margin, AvailabelLimit = InventoryItem.Limit });
                        pro.Variance = listvariance;
                    }
                    pro.Draft = false;
                    if (submitbtn == "2")
                    {
                        pro.Draft = true;
                    }
                    bool result = inventoryitemservice.CreateInventoryItem(pro).Result;

                    if (result == true)
                    {
                        ViewBag.result = "1";
                        TempData["msg"] = "11";
                    }
                    else
                    {
                        ViewBag.result = "3";
                        TempData["msg"] = "33";
                    }
                    if (PrizeId == 0)
                    {
                        return RedirectToAction("InventoryItem", "Admin");
                    }
                    else
                    {
                        return new RedirectResult(Url.Action("PrizeView", "Admin", new { PrizeId = PrizeId }) + "#inventory");
                    }
                }
                catch (Exception ex)
                {
                    CustomException(ex);
                    ViewBag.result = ex.Message;
                }
            }
            ViewBag.PrizeId = 0;
            if (PrizeId != 0)
            {
                ViewBag.PrizeId = PrizeId;
            }
            return View(InventoryItem);

        }

        public ActionResult InventoryItemView(int InventoryItemId, int PrizeId=0)
        {
            com.kuazoo.InventoryItem v = inventoryitemservice.GetInventoryItemById(InventoryItemId).Result;
            InventoryItemModel.InventoryItem InventoryItem;
            List<string> variance = new List<string>();
            foreach (var vari in v.Variance)
            {
                int used = (vari.AvailabelLimit-vari.Used);
                variance.Add(vari.Variance + "," + vari.Price + "," + vari.Discount + "," + vari.Margin + "," + vari.AvailabelLimit+","+used);
            }
            string name = v.Name;
            if (v.Draft)
            {
                name = "Draft - " +name;
            }
            if (v.Prize != null)
            {
                InventoryItem = new InventoryItemModel.InventoryItem() { Priority = v.Priority, Draft = v.Draft, InventoryItemId = v.InventoryItemId, Name = name, ShortDesc = v.ShortDesc, GeneralDescription = v.GeneralDescription, Price = v.Price, Margin = v.Margin, Description = v.Description, Terms = v.Terms, MerchantId = v.Merchant.MerchantId, MerchantName = v.Merchant.Name, MerchantCityName = v.Merchant.City.Name, MerchantCountryName = v.Merchant.Country.Name, CountryId = v.Country.CountryId, Country = new MasterModel.Country() { CountryId = v.Country.CountryId, CountryName = v.Country.Name }, CityId = v.City.CityId, City = new MasterModel.City() { CityId = v.City.CityId, CityName = v.City.Name }, Keyword = v.Keyword, InventoryItemTypeId = v.InventoryItemType.InventoryItemTypeId, InventoryItemTypeName = v.InventoryItemType.InventoryItemTypeName, Discount = v.Discount, ExpireDate = v.ExpireDate.AddHours(Helper.defaultGMT), MaximumSales = v.MaximumSales, RemainSales = v.RemainSales, PublishDate = v.PublishDate.AddHours(Helper.defaultGMT), MinimumTarget = v.MinimumTarget, SalesVisualMeter = v.SalesVisualMeter, Tag = v.Tag, TagName = v.TagName, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = String.Join(",", v.SubImageId), SubImageName = String.Join("|", v.SubImageName), SubImageUrl = String.Join("|", v.SubImageUrl), Flag = v.Flag, Featured = v.Featured, PrizeId = v.Prize.PrizeId, PrizeName = v.Prize.Name, Variance = String.Join("|", variance) + "|" };
            }
            else
            {
                InventoryItem = new InventoryItemModel.InventoryItem() { Priority = v.Priority, Draft = v.Draft, InventoryItemId = v.InventoryItemId, Name = name, ShortDesc = v.ShortDesc, GeneralDescription = v.GeneralDescription, Price = v.Price, Margin = v.Margin, Description = v.Description, Terms = v.Terms, MerchantId = v.Merchant.MerchantId, MerchantName = v.Merchant.Name, MerchantCityName = v.Merchant.City.Name, MerchantCountryName = v.Merchant.Country.Name, CountryId = v.Country.CountryId, Country = new MasterModel.Country() { CountryId = v.Country.CountryId, CountryName = v.Country.Name }, CityId = v.City.CityId, City = new MasterModel.City() { CityId = v.City.CityId, CityName = v.City.Name }, Keyword = v.Keyword, InventoryItemTypeId = v.InventoryItemType.InventoryItemTypeId, InventoryItemTypeName = v.InventoryItemType.InventoryItemTypeName, Discount = v.Discount, ExpireDate = v.ExpireDate.AddHours(Helper.defaultGMT), MaximumSales = v.MaximumSales, RemainSales = v.RemainSales, PublishDate = v.PublishDate.AddHours(Helper.defaultGMT), MinimumTarget = v.MinimumTarget, SalesVisualMeter = v.SalesVisualMeter, Tag = v.Tag, TagName = v.TagName, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = String.Join(",", v.SubImageId), SubImageName = String.Join("|", v.SubImageName), SubImageUrl = String.Join("|", v.SubImageUrl), Flag = v.Flag, Featured = v.Featured, Variance = String.Join("|", variance) + "|" };
            }
                //if(v.Prize != null)
            //{
            //    InventoryItem = new InventoryItemModel.InventoryItem() { InventoryItemId = v.InventoryItemId, Name = v.Name, ShortDesc=v.ShortDesc, Price = v.Price, Description = v.Description, MerchantId = v.Merchant.MerchantId, MerchantName = v.Merchant.Name, MerchantCityName = v.Merchant.City.Name, MerchantCountryName = v.Merchant.Country.Name,  CountryId = v.Country.CountryId, Country = new MasterModel.Country() { CountryId = v.Country.CountryId, CountryName = v.Country.Name }, CityId = v.City.CityId, City = new MasterModel.City() { CityId = v.City.CityId, CityName = v.City.Name }, Keyword = v.Keyword, InventoryItemTypeId = v.InventoryItemType.InventoryItemTypeId, InventoryItemTypeName = v.InventoryItemType.InventoryItemTypeName, Discount = v.Discount, ExpireDate = v.ExpireDate, MaximumSales = v.MaximumSales, PublishDate = v.PublishDate, MinimumTarget = v.MinimumTarget, SalesVisualMeter = v.SalesVisualMeter, LastAction = v.LastAction, Tag = v.Tag, TagName = v.TagName, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = String.Join(",", v.SubImageId), SubImageName = String.Join("|", v.SubImageName), SubImageUrl = String.Join("|", v.SubImageUrl), Flag = v.Flag, Prize = new InventoryItemModel.Prize() { PrizeId = v.Prize.PrizeId, Name = v.Prize.Name, Description = v.Prize.Description, ImageId = v.Prize.ImageId, ImageName = v.Prize.ImageName, ImageUrl = v.Prize.ImageUrl, SubImageId = String.Join(",", v.Prize.SubImageId), SubImageName = String.Join("|", v.Prize.SubImageName), SubImageUrl = String.Join("|", v.Prize.SubImageUrl), Price = v.Prize.Price, SponsorName = v.Prize.SponsorName, Detail = v.Prize.Detail, Terms = v.Prize.Terms } };
            //}
            //else
            //{
            //    InventoryItem = new InventoryItemModel.InventoryItem() { InventoryItemId = v.InventoryItemId, Name = v.Name, ShortDesc=v.ShortDesc, Price = v.Price, Description = v.Description, MerchantId = v.Merchant.MerchantId, MerchantName = v.Merchant.Name, MerchantCityName = v.Merchant.City.Name, MerchantCountryName = v.Merchant.Country.Name,  CountryId = v.Country.CountryId, Country = new MasterModel.Country() { CountryId = v.Country.CountryId, CountryName = v.Country.Name }, CityId = v.City.CityId, City = new MasterModel.City() { CityId = v.City.CityId, CityName = v.City.Name }, Keyword = v.Keyword, InventoryItemTypeId = v.InventoryItemType.InventoryItemTypeId, InventoryItemTypeName = v.InventoryItemType.InventoryItemTypeName, Discount = v.Discount, ExpireDate = v.ExpireDate, MaximumSales = v.MaximumSales, PublishDate = v.PublishDate, MinimumTarget = v.MinimumTarget, SalesVisualMeter = v.SalesVisualMeter, Tag = v.Tag, TagName = v.TagName, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = String.Join(",", v.SubImageId), SubImageName = String.Join("|", v.SubImageName), SubImageUrl = String.Join("|", v.SubImageUrl), Flag = v.Flag };
            //}
            ViewBag.PrizeId = 0;
            if (PrizeId != 0)
            {
                ViewBag.PrizeId = PrizeId;
            }
            return View(InventoryItem);
        }

        #region flashdealinventory

        public ActionResult FlashDealInventory_Read([DataSourceRequest] DataSourceRequest request, int InventoryItemId)
        {
            List<com.kuazoo.FlashDeal> listmer = flashdealservice.GetFlashDealListByInventoryItem(InventoryItemId).Result;
            List<InventoryItemModel.FlashDeal> newlistmer = new List<InventoryItemModel.FlashDeal>();
            foreach (var v in listmer)
            {
                newlistmer.Add(new InventoryItemModel.FlashDeal() { FlashDealId = v.FlashDealId, Discount = v.Discount, MerchantId = v.MerchantId, MerchantName = v.MerchantName, InventoryItemId = v.InventoryItemId, InventoryItemName = v.InventoryItemName, StartTime = v.StartTime, StartTimeStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.StartTime.AddHours(Helper.defaultGMT)), EndTime = v.EndTime, EndTimeStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.EndTime.AddHours(Helper.defaultGMT)), Flag = v.Flag, LastAction = v.LastAction });
            }
            IEnumerable<InventoryItemModel.FlashDeal> ienuList = newlistmer;
            var result = ienuList.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FlashDealInventory_Destroy([DataSourceRequest] DataSourceRequest request, InventoryItemModel.FlashDeal FlashDeal)
        {
            if (FlashDeal != null)
            {
                Boolean result = flashdealservice.DeleteFlashDeal(FlashDeal.FlashDealId).Result;

                if (result) ViewBag.result = "1";
                else ViewBag.result = "2";
                ModelState.AddModelError("", ViewBag.result);

            }

            return Json(ModelState.ToDataSourceResult());
        }
        public ActionResult FlashDealInventoryAdd(int InventoryItemId)
        {
            InventoryItemModel.FlashDeal model = new InventoryItemModel.FlashDeal();
            model.InventoryItemId = InventoryItemId;
            return View(model);
        }
        [HttpPost]
        public ActionResult FlashDealInventoryAdd(FormCollection collection, InventoryItemModel.FlashDeal FlashDeal)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    com.kuazoo.FlashDeal pro = new com.kuazoo.FlashDeal();
                    pro.FlashDealId = 0;
                    pro.InventoryItemId = FlashDeal.InventoryItemId;
                    pro.Discount = FlashDeal.Discount;
                    pro.StartTime = FlashDeal.StartTime.AddHours(-1*Helper.defaultGMT);
                    pro.EndTime = FlashDeal.EndTime.AddHours(-1*Helper.defaultGMT);
                    pro.Flag = true;//FlashDeal.Flag;

                    bool result = flashdealservice.CreateFlashDeal(pro).Result;
                    if (result == true)
                    {
                        ViewBag.result = "1";
                    }
                    else
                    {
                        ViewBag.result = "3";
                    }
                    TempData["msg"] = ViewBag.result;
                    //return RedirectToAction("FlashDeal", "Admin");
                }
                catch (Exception ex)
                {
                    CustomException(ex);
                    ViewBag.result = ex.Message;
                }
            }
            return View(FlashDeal);

        }

        public ActionResult FlashDealInventoryEdit(int FlashDealId)
        {
            com.kuazoo.FlashDeal v = flashdealservice.GetFlashDealById(FlashDealId).Result;
            InventoryItemModel.FlashDeal FlashDeal;
            FlashDeal = new InventoryItemModel.FlashDeal() { FlashDealId = v.FlashDealId, Discount = v.Discount, MerchantId = v.MerchantId, MerchantName = v.MerchantName, InventoryItemId = v.InventoryItemId, InventoryItemName = v.InventoryItemName, StartTime = v.StartTime.AddHours(Helper.defaultGMT), EndTime = v.EndTime.AddHours(Helper.defaultGMT), Flag = v.Flag, LastAction = v.LastAction };

            return View(FlashDeal);
        }
        [HttpPost]
        public ActionResult FlashDealInventoryEdit(FormCollection collection, InventoryItemModel.FlashDeal FlashDeal)
        {

            if (ModelState.IsValid)
            {
                try
                {

                    com.kuazoo.FlashDeal pro = new com.kuazoo.FlashDeal();
                    pro.FlashDealId = FlashDeal.FlashDealId;
                    pro.InventoryItemId = FlashDeal.InventoryItemId;
                    pro.Discount = FlashDeal.Discount;
                    pro.StartTime = FlashDeal.StartTime.AddHours(-1*Helper.defaultGMT);
                    pro.EndTime = FlashDeal.EndTime.AddHours(-1*Helper.defaultGMT);
                    pro.Flag = FlashDeal.Flag;


                    bool result = flashdealservice.CreateFlashDeal(pro).Result;

                    if (result == true)
                    {
                        ViewBag.result = "1";
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
            }
            return View(FlashDeal);

        }


        #endregion


        //[HttpPost]
        //public virtual ActionResult InventoryUploadImage(String Name, String strSubImage= "")//name and strsubimage come from post jquery
        //{
        //    HttpPostedFileBase imageUpload = Request.Files["imagefile"];
        //    bool isUpload = false;
        //    string message = "";
        //    string imageurl = "";
        //    string imagename = "";
        //    if (imageUpload != null)
        //    {
        //        try
        //        {
        //            if (!Helper.IsValidImage(imageUpload.FileName))
        //            {
        //                message = "File is not an image file";
        //            }
        //            if (Name == null) Name = "";
        //            string tempname = Name + "-" + imageUpload.FileName;
        //            string[] currentlist = strSubImage.Split('|');
        //            int flag=0;
        //            foreach (var v in currentlist)
        //            {
        //                if (v == tempname)
        //                {
        //                    flag = 1;
        //                    break;
        //                }
        //            }
        //            if (flag == 1)
        //            {
        //                message = "Image already upload!";
        //            }
        //            else
        //            {

        //                imagename = Helper.uploadImageWithName(imageUpload, Name);
        //                imageurl = ConfigurationManager.AppSettings["uploadpath"] + imagename;
        //                isUpload = true;
        //                message = "Image uploaded successfully!";
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            message = string.Format("File upload failed: {0}", ex.Message);
        //        }
        //    }
        //    return Json(new { isUpload = isUpload, message = message, imageurl = imageurl,imagename=imagename }, "text/html");
        //}
        #endregion

        #region featured

        public ActionResult FeaturedDeal()
        {
            if (TempData["msg"] != null)
            {
                ViewBag.msg = TempData["msg"];
            }
            return View();
        }
        public ActionResult FeaturedDeal_Read([DataSourceRequest] DataSourceRequest request)
        {
            DateTime today = DateTime.UtcNow;
            List<com.kuazoo.InventoryFeatured> listmer = inventoryitemservice.GetInventoryFeatured(today).Result;
            List<InventoryItemModel.InventoryFeatured> newlistmer = new List<InventoryItemModel.InventoryFeatured>();
            foreach (var v in listmer)
            {
                newlistmer.Add(new InventoryItemModel.InventoryFeatured() {  InventoryItemId = v.InventoryItemId, Name = v.Name, Featured=v.Featured , FeatureSeq = v.FeatureSeq, FeaturedText=v.FeaturedText });

            }
            IEnumerable<InventoryItemModel.InventoryFeatured> ienuList = newlistmer;
            var result = ienuList.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FeaturedDeal_Update([DataSourceRequest] DataSourceRequest request, InventoryItemModel.InventoryFeatured InventoryItem)
        {
            if (InventoryItem != null)
            {
                DateTime today = DateTime.UtcNow;
                com.kuazoo.InventoryFeatured feature = new com.kuazoo.InventoryFeatured();
                feature.InventoryItemId = InventoryItem.InventoryItemId;
                feature.Name = InventoryItem.Name;
                feature.Featured = InventoryItem.Featured;
                feature.FeatureSeq = InventoryItem.FeatureSeq;
                feature.FeaturedText = "";
                if (InventoryItem.FeaturedText != null && InventoryItem.FeaturedText!="")
                {
                    string desc = Microsoft.JScript.GlobalObject.decodeURI(InventoryItem.FeaturedText);
                    feature.FeaturedText = desc;
                }
                Boolean result = inventoryitemservice.CreateFeaturedDeal(today, feature).Result;

                if (result) ViewBag.result = "11";
                else ViewBag.result = "22";
                ModelState.AddModelError("", ViewBag.result);

            }

            return Json(ModelState.ToDataSourceResult());
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FeaturedDeal_Destroy([DataSourceRequest] DataSourceRequest request, InventoryItemModel.InventoryFeatured InventoryItem)
        {
            if (InventoryItem != null)
            {
                Boolean result = inventoryitemservice.DeleteInventoryItemFeature(InventoryItem.InventoryItemId).Result;

                if (result) ViewBag.result = "1";
                else ViewBag.result = "2";
                ModelState.AddModelError("", ViewBag.result);

            }

            return Json(ModelState.ToDataSourceResult());
        }

        public ActionResult FeaturedDealAdd()
        {
            return View();
        }

        [HttpPost]
        public virtual ActionResult FeaturedDealAdd_Add(int id, int seq)
        {
            string message = "";
            bool result = false;
            try
            {
                DateTime today = DateTime.UtcNow;
                com.kuazoo.InventoryFeatured feature = new com.kuazoo.InventoryFeatured();
                feature.InventoryItemId = id;
                feature.Featured = true;
                feature.FeatureSeq = seq;
                result = inventoryitemservice.CreateFeaturedDeal(today, feature).Result;
                if (result)
                {
                    message = "Inventory item successfully set as Featured Deal!";
                }
                else
                {
                    message = "Failed set as Featured Deal";
                }
            }
            catch (Exception ex)
            {
                CustomException(ex);
                result = false;
                message = string.Format("Error : }", ex.Message);
            }
            return Json(new {  message = message,result=result }, "text/html");
        }
        #endregion

        #region ArchivedInventory
        public ActionResult ArchivedInventory()
        {
            return View();
        }

        public ActionResult InventoryArchivedItem_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<com.kuazoo.InventoryItem> listmer = inventoryitemservice.GetInventoryItemArchivedList().Result;
            List<InventoryItemModel.InventoryItem> newlistmer = new List<InventoryItemModel.InventoryItem>();
            string name = "";
            foreach (var v in listmer)
            {
                name = v.Name;
                if (v.Draft)
                {
                    name = "Draft - " +name;
                }
                newlistmer.Add(new InventoryItemModel.InventoryItem() { Create = v.Create.AddHours(Helper.defaultGMT), InventoryItemId = v.InventoryItemId, Name = name, ShortDesc = v.ShortDesc, GeneralDescription = v.GeneralDescription, Price = v.Price, Margin = v.Margin, Description = v.Description, Terms = v.Terms, MerchantId = v.Merchant.MerchantId, MerchantName = v.Merchant.Name, MerchantCityName = v.Merchant.City.Name, MerchantCountryName = v.Merchant.Country.Name,  CountryId = v.Country.CountryId, Country = new MasterModel.Country() { CountryId = v.Country.CountryId, CountryName = v.Country.Name }, CityId = v.City.CityId, City = new MasterModel.City() { CityId = v.City.CityId, CityName = v.City.Name }, Keyword = v.Keyword, InventoryItemTypeId = v.InventoryItemType.InventoryItemTypeId, InventoryItemTypeName = v.InventoryItemType.InventoryItemTypeName, Discount = v.Discount, ExpireDate = v.ExpireDate.AddHours(Helper.defaultGMT), MaximumSales = v.MaximumSales, RemainSales = v.RemainSales, PublishDate = v.PublishDate.AddHours(Helper.defaultGMT), MinimumTarget = v.MinimumTarget, SalesVisualMeter = v.SalesVisualMeter, LastAction = v.LastAction, Tag = v.Tag, TagName = v.TagName, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = String.Join("|", v.SubImageId), SubImageName = String.Join("|", v.SubImageName), SubImageUrl = String.Join("|", v.SubImageUrl), Flag = v.Flag, PrizeId = v.Prize.PrizeId, PrizeName = v.Prize.Name, Variance = String.Join("|", v.Variance) });

            }
            IEnumerable<InventoryItemModel.InventoryItem> ienuList = newlistmer;
            var result = ienuList.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ArchivedInventoryView(int InventoryItemId, int PrizeId = 0)
        {
            com.kuazoo.InventoryItem v = inventoryitemservice.GetInventoryItemById(InventoryItemId).Result;
            InventoryItemModel.InventoryItem InventoryItem;
            string name = v.Name;
            if (v.Draft)
            {
                name = "Draft - " +name;
            }
            InventoryItem = new InventoryItemModel.InventoryItem() { InventoryItemId = v.InventoryItemId, Name = name, ShortDesc = v.ShortDesc, GeneralDescription = v.GeneralDescription, Price = v.Price, Margin = v.Margin, Description = v.Description, Terms = v.Terms, MerchantId = v.Merchant.MerchantId, MerchantName = v.Merchant.Name, MerchantCityName = v.Merchant.City.Name, MerchantCountryName = v.Merchant.Country.Name,  CountryId = v.Country.CountryId, Country = new MasterModel.Country() { CountryId = v.Country.CountryId, CountryName = v.Country.Name }, CityId = v.City.CityId, City = new MasterModel.City() { CityId = v.City.CityId, CityName = v.City.Name }, Keyword = v.Keyword, InventoryItemTypeId = v.InventoryItemType.InventoryItemTypeId, InventoryItemTypeName = v.InventoryItemType.InventoryItemTypeName, Discount = v.Discount, ExpireDate = v.ExpireDate.AddHours(Helper.defaultGMT), MaximumSales = v.MaximumSales, RemainSales = v.RemainSales, PublishDate = v.PublishDate.AddHours(Helper.defaultGMT), MinimumTarget = v.MinimumTarget, SalesVisualMeter = v.SalesVisualMeter, Tag = v.Tag, TagName = v.TagName, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = String.Join(",", v.SubImageId), SubImageName = String.Join("|", v.SubImageName), SubImageUrl = String.Join("|", v.SubImageUrl), Flag = v.Flag, PrizeId = v.Prize.PrizeId, PrizeName = v.Prize.Name, Variance = String.Join("|", v.Variance) };
            ViewBag.PrizeId = 0;
            if (PrizeId != 0)
            {
                ViewBag.PrizeId = PrizeId;
            }
            return View(InventoryItem);
        }
        #endregion

        #region flashdeal

        public ActionResult FlashDeal()
        {
            if (TempData["msg"] != null)
            {
                ViewBag.msg = TempData["msg"];
            }
            return View();
        }
        public ActionResult FlashDeal_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<com.kuazoo.FlashDeal> listmer = flashdealservice.GetFlashDealList().Result;
            List<InventoryItemModel.FlashDeal> newlistmer = new List<InventoryItemModel.FlashDeal>();
            foreach (var v in listmer)
            {
                newlistmer.Add(new InventoryItemModel.FlashDeal() { FlashDealId = v.FlashDealId, Discount = v.Discount, MerchantId = v.MerchantId, MerchantName = v.MerchantName, InventoryItemId = v.InventoryItemId, InventoryItemName = v.InventoryItemName, StartTime = v.StartTime.AddHours(Helper.defaultGMT), StartTimeStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.StartTime.AddHours(Helper.defaultGMT)), EndTime = v.EndTime.AddHours(Helper.defaultGMT), EndTimeStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.EndTime.AddHours(Helper.defaultGMT)), Flag = v.Flag, LastAction = v.LastAction });
            }
            IEnumerable<InventoryItemModel.FlashDeal> ienuList = newlistmer;
            var result = ienuList.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FlashDeal_Destroy([DataSourceRequest] DataSourceRequest request, InventoryItemModel.FlashDeal FlashDeal)
        {
            if (FlashDeal != null)
            {
                Boolean result = flashdealservice.DeleteFlashDeal(FlashDeal.FlashDealId).Result;

                if (result) ViewBag.result = "1";
                else ViewBag.result = "2";
                ModelState.AddModelError("", ViewBag.result);

            }

            return Json(ModelState.ToDataSourceResult());
        }
        public ActionResult FlashDealAdd()
        {
            return View();
        }
        [HttpPost]
        public ActionResult FlashDealAdd(FormCollection collection, InventoryItemModel.FlashDeal FlashDeal)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    com.kuazoo.FlashDeal pro = new com.kuazoo.FlashDeal();
                    pro.FlashDealId = 0;
                    pro.InventoryItemId = FlashDeal.InventoryItemId;
                    pro.Discount = FlashDeal.Discount;
                    pro.StartTime = FlashDeal.StartTime.AddHours(-1 * Helper.defaultGMT);
                    pro.EndTime = FlashDeal.EndTime.AddHours(-1 * Helper.defaultGMT);
                    pro.Flag = true;//FlashDeal.Flag;

                    bool result = flashdealservice.CreateFlashDeal(pro).Result;
                    if (result == true)
                    {
                        ViewBag.result = "1";
                    }
                    else
                    {
                        ViewBag.result = "3";
                    }
                    TempData["msg"] = ViewBag.result;
                    return RedirectToAction("FlashDeal", "Admin");
                }
                catch (Exception ex)
                {
                    CustomException(ex);
                    ViewBag.result = ex.Message;
                }
            }
            return View(FlashDeal);

        }

        public ActionResult FlashDealEdit(int FlashDealId)
        {
            com.kuazoo.FlashDeal v = flashdealservice.GetFlashDealById(FlashDealId).Result;
            InventoryItemModel.FlashDeal FlashDeal;
            FlashDeal = new InventoryItemModel.FlashDeal() { FlashDealId = v.FlashDealId, Discount = v.Discount, MerchantId = v.MerchantId, MerchantName = v.MerchantName, InventoryItemId = v.InventoryItemId, InventoryItemName = v.InventoryItemName, StartTime = v.StartTime.AddHours(Helper.defaultGMT), EndTime = v.EndTime.AddHours(Helper.defaultGMT), Flag = v.Flag, LastAction = v.LastAction };

            return View(FlashDeal);
        }
        [HttpPost]
        public ActionResult FlashDealEdit(FormCollection collection, InventoryItemModel.FlashDeal FlashDeal)
        {

            if (ModelState.IsValid)
            {
                try
                {

                    com.kuazoo.FlashDeal pro = new com.kuazoo.FlashDeal();
                    pro.FlashDealId = FlashDeal.FlashDealId;
                    pro.InventoryItemId = FlashDeal.InventoryItemId;
                    pro.Discount = FlashDeal.Discount;
                    pro.StartTime = FlashDeal.StartTime.AddHours(-1*Helper.defaultGMT);
                    pro.EndTime = FlashDeal.EndTime.AddHours(-1*Helper.defaultGMT);
                    pro.Flag = FlashDeal.Flag;


                    bool result = flashdealservice.CreateFlashDeal(pro).Result;

                    if (result == true)
                    {
                        ViewBag.result = "1";
                        TempData["msg"] = "11";
                    }
                    else
                    {
                        ViewBag.result = "3";
                        TempData["msg"] = "33";
                    }
                    return RedirectToAction("FlashDeal", "Admin");
                }
                catch (Exception ex)
                {
                    CustomException(ex);
                    ViewBag.result = ex.Message;
                }
            }
            return View(FlashDeal);

        }

        public ActionResult FlashDealView(int FlashDealId)
        {
            com.kuazoo.FlashDeal v = flashdealservice.GetFlashDealById(FlashDealId).Result;
            InventoryItemModel.FlashDeal FlashDeal;
            FlashDeal = new InventoryItemModel.FlashDeal() { FlashDealId = v.FlashDealId, Discount = v.Discount, MerchantId = v.MerchantId, MerchantName = v.MerchantName, InventoryItemId = v.InventoryItemId, InventoryItemName = v.InventoryItemName, StartTime = v.StartTime.AddHours(Helper.defaultGMT), EndTime = v.EndTime.AddHours(Helper.defaultGMT), Flag = v.Flag, LastAction = v.LastAction };

            return View(FlashDeal);
        }

        #endregion

        #region tag
        public ActionResult Tag()
        {
            List<com.kuazoo.Tag> listtag = tagservice.GetTagList().Result;
            List<InventoryItemModel.Tag> newlisttag = new List<InventoryItemModel.Tag>();
            foreach (var v in listtag)
            {
                newlisttag.Add(new InventoryItemModel.Tag() { TagId = v.TagId, TagName = v.Name });
            }
            if (TempData["msg"] != null)
            {
                ViewBag.msg = TempData["msg"];
            }
            ViewData["taglist"] = newlisttag;
            return View();
        }
        public ActionResult Tag_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<com.kuazoo.Tag> listmer = tagservice.GetTagList().Result;
            List<InventoryItemModel.Tag> newlistmer = new List<InventoryItemModel.Tag>();
            foreach (var v in listmer)
            {
                newlistmer.Add(new InventoryItemModel.Tag() { Create=v.Create.AddHours(Helper.defaultGMT), TagId = v.TagId, TagName = v.Name, ShowAsCategory = v.ShowAsCategory, ParentId = v.Parent != null ? v.Parent.TagId : 0, ParentName =  v.Parent != null ? v.Parent.Name : "", LastAction = v.LastAction });
            }
            IEnumerable<InventoryItemModel.Tag> ienuList = newlistmer;
            var result = ienuList.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Tag_Destroy([DataSourceRequest] DataSourceRequest request, InventoryItemModel.Tag Tag)
        {
            if (Tag != null)
            {
                Boolean result = tagservice.DeleteTag(Tag.TagId).Result;

                if (result) ViewBag.result = "1";
                else ViewBag.result = "2";
                ModelState.AddModelError("", ViewBag.result);

            }

            return Json(ModelState.ToDataSourceResult());
        }

        public ActionResult TagAdd()
        {
            return View();
        }
        [HttpPost]
        public ActionResult TagAdd(InventoryItemModel.Tag Tag)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string[] newtagname = Tag.TagName.Split(',');
                    for (int i = 0; i < newtagname.Length; i++)
                    {

                        com.kuazoo.Tag pro = new com.kuazoo.Tag();
                        pro.TagId = 0;
                        pro.Name = newtagname[i];
                        if (Tag.ParentId != null)
                        {
                            Tag parent = new Tag();
                            parent.TagId = (int)Tag.ParentId;
                            pro.Parent = parent;
                        }
                        else
                        {
                            pro.Parent = null;
                        }
                        pro.ShowAsCategory = Tag.ShowAsCategory;

                        bool result = tagservice.CreateTag(pro).Result;
                        if (result == true)
                        {
                            ViewBag.result = "1";
                        }
                        else
                        {
                            ViewBag.result = "3";
                        }
                    }
                    TempData["msg"] = ViewBag.result;
                    return RedirectToAction("Tag", "Admin");
                }
                catch (Exception ex)
                {
                    CustomException(ex);
                    ViewBag.result = ex.Message;
                }
            }
            return View(Tag);
        }
        public ActionResult TagEdit(int TagId)
        {
            com.kuazoo.Tag v = tagservice.GetTagById(TagId).Result;
            InventoryItemModel.Tag InventoryItem = new InventoryItemModel.Tag() { TagId = v.TagId, TagName = v.Name, ShowAsCategory = v.ShowAsCategory, ParentId = v.Parent != null ? v.Parent.TagId : 0, ParentName = v.Parent != null ? v.Parent.Name : "", LastAction = v.LastAction };
            return View(InventoryItem);
        }
        [HttpPost]
        public ActionResult TagEdit(InventoryItemModel.Tag Tag)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    com.kuazoo.Tag pro = new com.kuazoo.Tag();
                    pro.TagId = Tag.TagId;
                    pro.Name = Tag.TagName;
                    if (Tag.ParentId != null)
                    {
                        Tag parent = new Tag();
                        parent.TagId = (int)Tag.ParentId;
                        pro.Parent = parent;
                    }
                    else
                    {
                        pro.Parent = null;
                    }
                    pro.ShowAsCategory = Tag.ShowAsCategory;

                    bool result = tagservice.CreateTag(pro).Result;
                    if (result == true)
                    {
                        ViewBag.result = "1";
                        TempData["msg"] = "11";
                    }
                    else
                    {
                        ViewBag.result = "3";
                        TempData["msg"] = "33";
                    }
                    return RedirectToAction("Tag", "Admin");
                }
                catch (Exception ex)
                {
                    CustomException(ex);
                    ViewBag.result = ex.Message;
                }
            }
            return View(Tag);

        }
        public ActionResult TagView(int TagId)
        {
            com.kuazoo.Tag v = tagservice.GetTagById(TagId).Result;
            InventoryItemModel.Tag InventoryItem = new InventoryItemModel.Tag() { TagId = v.TagId, TagName = v.Name, ShowAsCategory = v.ShowAsCategory, ParentId = v.Parent != null ? v.Parent.TagId : 0, ParentName = v.Parent != null ? v.Parent.Name : "", LastAction = v.LastAction };
            return View(InventoryItem);
        }
        #endregion

        #region Promotion
        public ActionResult Promotion()
        {
            return View();
        }
        public ActionResult Promotion_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<com.kuazoo.Promotion> listmer = promotionservice.GetPromotionList().Result;
            List<InventoryItemModel.Promotion> newlistmer = new List<InventoryItemModel.Promotion>();
            foreach (var v in listmer)
            {
                string type = "Discount by %";
                if (v.PromoType == 2)
                {
                    type = "Discount by Fixed Value";
                }
                newlistmer.Add(new InventoryItemModel.Promotion() { Create=v.Create.AddHours(Helper.defaultGMT), PromotionId = v.PromotionId, PromoCode = v.PromoCode, PromoType = v.PromoType, PromoTypeStr = type, PromoValue = v.PromoValue, ValidDate = v.ValidDate.AddHours(Helper.defaultGMT), Flag = v.Flag, LastAction = v.LastAction });
            }
            IEnumerable<InventoryItemModel.Promotion> ienuList = newlistmer;
            var result = ienuList.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Promotion_Destroy([DataSourceRequest] DataSourceRequest request, InventoryItemModel.Promotion Promotion)
        {
            if (Promotion != null)
            {
                Boolean result = promotionservice.DeletePromotion(Promotion.PromotionId).Result;

                if (result) ViewBag.result = "1";
                else ViewBag.result = "2";
                ModelState.AddModelError("", ViewBag.result);

            }

            return Json(ModelState.ToDataSourceResult());
        }

        public ActionResult PromotionAdd()
        {
            return View();
        }
        [HttpPost]
        public ActionResult PromotionAdd(InventoryItemModel.Promotion Promotion)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    com.kuazoo.Promotion pro = new com.kuazoo.Promotion();
                    pro.PromotionId = 0;
                    pro.PromoCode = Promotion.PromoCode;
                    pro.PromoType = Promotion.PromoType;
                    pro.PromoValue = Promotion.PromoValue;
                    pro.ValidDate = Promotion.ValidDate.AddHours(-1*Helper.defaultGMT);
                    pro.Flag = Promotion.Flag;

                    bool result = promotionservice.CreatePromotion(pro).Result;
                    if (result == true)
                    {
                        ViewBag.result = "1";
                    }
                    else
                    {
                        ViewBag.result = "3";
                    }

                    TempData["msg"] = ViewBag.result;
                    return RedirectToAction("Promotion", "Admin");
                }
                catch (Exception ex)
                {
                    CustomException(ex);
                    ViewBag.result = ex.Message;
                }
            }
            return View(Promotion);

        }
        public ActionResult PromotionEdit(int PromotionId)
        {
            com.kuazoo.Promotion v = promotionservice.GetPromotionById(PromotionId).Result;
            string type = "Discount by %";
            if (v.PromoType == 2)
            {
                type = "Discount by Fixed Value";
            }
            InventoryItemModel.Promotion InventoryItem = new InventoryItemModel.Promotion() { PromotionId = v.PromotionId, PromoCode = v.PromoCode, PromoType = v.PromoType, PromoTypeStr = type, PromoValue = v.PromoValue, ValidDate = v.ValidDate.AddHours(Helper.defaultGMT), Flag = v.Flag, LastAction = v.LastAction };
            return View(InventoryItem);
        }
        [HttpPost]
        public ActionResult PromotionEdit(InventoryItemModel.Promotion Promotion)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    com.kuazoo.Promotion pro = new com.kuazoo.Promotion();
                    pro.PromotionId = Promotion.PromotionId;
                    pro.PromoCode = Promotion.PromoCode;
                    pro.PromoType = Promotion.PromoType;
                    pro.PromoValue = Promotion.PromoValue;
                    pro.ValidDate = Promotion.ValidDate.AddHours(-1*Helper.defaultGMT);
                    pro.Flag = Promotion.Flag;

                    bool result = promotionservice.CreatePromotion(pro).Result;
                    if (result == true)
                    {
                        ViewBag.result = "1";
                        TempData["msg"] = "11";
                    }
                    else
                    {
                        ViewBag.result = "3";
                        TempData["msg"] = "33";
                    }
                    return RedirectToAction("Promotion", "Admin");
                }
                catch (Exception ex)
                {
                    CustomException(ex);
                    ViewBag.result = ex.Message;
                }
            }
            return View(Promotion);

        }
        public ActionResult PromotionView(int PromotionId)
        {
            com.kuazoo.Promotion v = promotionservice.GetPromotionById(PromotionId).Result;
            string type = "Discount by %";
            if (v.PromoType == 2)
            {
                type = "Discount by Fixed Value";
            }
            InventoryItemModel.Promotion InventoryItem = new InventoryItemModel.Promotion() { PromotionId = v.PromotionId, PromoCode = v.PromoCode, PromoType = v.PromoType, PromoTypeStr = type, PromoValue = v.PromoValue, ValidDate = v.ValidDate.AddHours(Helper.defaultGMT), Flag = v.Flag, LastAction = v.LastAction };
            return View(InventoryItem);
        }
        #endregion

        #region member

        public ActionResult Member()
        {
            if (TempData["msg"] != null)
            {
                ViewBag.msg = TempData["msg"];
            }
            return View();
        }
        public ActionResult Member_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<com.kuazoo.Member> listmer = memberservice.GetMemberList().Result;
            List<MemberModel.Member> newlistmer = new List<MemberModel.Member>();
            string gender = "";
            foreach (var v in listmer)
            {
                if (v.Gender == 1) gender = "Male";
                else if (v.Gender == 2) gender = "Female";
                newlistmer.Add(new MemberModel.Member() { Create=v.Create.AddHours(Helper.defaultGMT), MemberId = v.MemberId, FirstName = v.FirstName, LastName = v.LastName, Email = v.Email, Gender = v.Gender, GenderStr = gender, DateOfBirth = v.DateOfBirth, MemberStatus = new MemberModel.MemberStatus() { MemberStatusId = v.MemberStatus.MemberStatusId, MemberStatusName = v.MemberStatus.MemberStatusName }, LastLockout = v.LastLockout, LastLockoutDate = v.LastLockoutDate, Facebook = new MemberModel.Facebook() { FacebookId = v.Facebook.FacebookId }});
            } 
            IEnumerable<MemberModel.Member> ienuList = newlistmer;
            var result = ienuList.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //public ActionResult MemberEdit(int MemberId)
        //{
        //    com.kuazoo.Member v = memberservice.GetMemberById(MemberId).Result;
        //    MemberModel.Member Member;
        //    Member = new MemberModel.Member() { MemberId = v.MemberId, FirstName = v.FirstName, LastName = v.LastName, Email = v.Email, Gender = v.Gender, DateOfBirth = v.DateOfBirth, MemberStatus = new MemberModel.MemberStatus() { MemberStatusId = v.MemberStatus.MemberStatusId, MemberStatusName = v.MemberStatus.MemberStatusName }, LastLockout = v.LastLockout, LastLockoutDate = v.LastLockoutDate };

        //    return View(Member);
        //}
        //[HttpPost]
        //public ActionResult MemberEdit(FormCollection collection, MemberModel.Member Member)
        //{

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {

        //            com.kuazoo.Member pro = new com.kuazoo.Member();
        //            pro.MemberId = Member.MemberId;
        //            pro.FirstName = Member.FirstName;
        //            pro.LastName = Member.LastName;
        //            pro.Email = Member.Email;
        //            pro.Gender = Member.Gender;
        //            pro.DateOfBirth = Member.DateOfBirth;
        //            MemberModel.MemberStatus ms = new MemberModel.MemberStatus();
        //            ms.MemberStatusId = Member.MemberStatus.MemberStatusId;
        //            pro.LastLockout = Member.LastLockout;
        //            pro.LastLockoutDate = Member.LastLockoutDate;

        //            bool result = memberservice.CreateMember(pro).Result;

        //            if (result == true)
        //            {
        //                ViewBag.result = "1";
        //                TempData["msg"] = "11";
        //            }
        //            else
        //            {
        //                ViewBag.result = "3";
        //                TempData["msg"] = "33";
        //            }
        //            return RedirectToAction("Member", "Admin");
        //        }
        //        catch (Exception ex)
        //        {
        //            ViewBag.result = ex.Message;
        //        }
        //    }
        //    return View(Member);

        //}
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Member_Destroy([DataSourceRequest] DataSourceRequest request, MemberModel.Member Member)
        {

            try
            {
                if (Member != null)
                {
                    Boolean result = memberservice.ChangeMemberStatus(Member.MemberId,3).Result;

                    if (result) ViewBag.result = "1";
                    else ViewBag.result = "2";
                    ModelState.AddModelError("", ViewBag.result);

                }
            }
            catch (Exception ex)
            {
                CustomException(ex);
                ViewBag.result = ex.Message;
                ModelState.AddModelError("", ViewBag.result);
            }

            return Json(ModelState.ToDataSourceResult());
        }

        public ActionResult MemberEdit(int MemberId)
        {
            com.kuazoo.Member v = memberservice.GetMemberById(MemberId).Result;
            MemberModel.Member Member;
            string gender = "";
            if (v.Gender == 1) gender = "Male";
            else if (v.Gender == 2) gender = "Female";
            Member = new MemberModel.Member() { MemberId = v.MemberId, FirstName = v.FirstName, LastName = v.LastName, Email = v.Email, Gender = v.Gender, GenderStr = gender, DateOfBirth = v.DateOfBirth, MemberStatus = new MemberModel.MemberStatus() { MemberStatusId = v.MemberStatus.MemberStatusId, MemberStatusName = v.MemberStatus.MemberStatusName }, LastLockout = v.LastLockout, LastLockoutDate = v.LastLockoutDate };
            return View(Member);
        }
        [HttpPost]
        public ActionResult MemberEdit(FormCollection collection, MemberModel.Member Member)
        {
            try
            {
                com.kuazoo.Member mer = new com.kuazoo.Member();
                mer.MemberId = Member.MemberId;
                mer.FirstName = Member.FirstName;
                mer.LastName = Member.LastName;
                mer.Email = Member.Email;
                mer.Gender = Member.Gender;
                mer.DateOfBirth = Member.DateOfBirth;
                com.kuazoo.MemberStatus status = new com.kuazoo.MemberStatus();
                status.MemberStatusId = Member.MemberStatus.MemberStatusId;
                mer.MemberStatus = status;
                bool result = memberservice.CreateMember(mer).Result;

                if (result == true)
                {
                    ViewBag.result = "1";
                    TempData["msg"] = "11";
                }
                else
                {
                    ViewBag.result = "3";
                    TempData["msg"] = "33";
                }
                return RedirectToAction("Member", "Admin");
            }
            catch (Exception ex)
            {
                CustomException(ex);
                ViewBag.result = ex.Message;
            }
            return View(Member);
        }
        public ActionResult MemberView(int MemberId)
        {
            com.kuazoo.Member v = memberservice.GetMemberById(MemberId).Result;
            MemberModel.Member Member;
            string gender = "";
            if (v.Gender == 1) gender = "Male";
            else if (v.Gender == 2) gender = "Female";
            Member = new MemberModel.Member() { MemberId = v.MemberId, FirstName = v.FirstName, LastName = v.LastName, Email = v.Email, Gender = v.Gender, GenderStr = gender, DateOfBirth = v.DateOfBirth, MemberStatus = new MemberModel.MemberStatus() { MemberStatusId = v.MemberStatus.MemberStatusId, MemberStatusName = v.MemberStatus.MemberStatusName }, LastLockout = v.LastLockout, LastLockoutDate = v.LastLockoutDate };

            return View(Member);
        }

        #endregion


        #region Transaction

        public ActionResult Transaction()
        {
            if (TempData["msg"] != null)
            {
                ViewBag.msg = TempData["msg"];
            }
            return View();
        }
        [HttpPost]
        public ActionResult Transaction(TransactionModel.TransactionDetail3 model)
        {
            try
            {
                if (model.DropdownType == 1)
                {
                    List<com.kuazoo.TransactionDetail3> listmer = transactionservice.GetAllTransactionList().Result;
                    List<TransactionModel.TransactionDetail3Extract> newlistmer = new List<TransactionModel.TransactionDetail3Extract>();
                    foreach (var v in listmer)
                    {
                        DateTime? processdate = null;
                        if (v.ProcessDate.HasValue)
                        {
                            processdate = v.ProcessDate.Value.AddHours(Helper.defaultGMT);
                        }
                        DateTime? paymentdate = null;
                        if (v.PaymentDate.HasValue)
                        {
                            paymentdate = v.PaymentDate.Value;//already local time
                        }
                        decimal? paymentamount = null;
                        if (v.PaymentAmount != 0)
                        {
                            paymentamount = v.PaymentAmount;
                        }
                        var GiftS = v.Gift == true ? "Yes" : "No";
                        newlistmer.Add(new TransactionModel.TransactionDetail3Extract() { TransactionId = v.TransactionId, OrderId = GeneralService.TransactionCode(v.TransactionId, v.TransactionDate), TransactionDate = v.TransactionDate.AddHours(Helper.defaultGMT), MerchantId = v.MerchantId, MerchantName = v.MerchantName, InventoryItemId = v.InventoryItemId, InventoryItemName = v.InventoryItemName, Variance = v.Variance, SKU = v.SKU, Qty = v.Qty, VoucherCode = v.VoucherCode, CustomerFirstName = v.MemberFirstName, CustomerLastName = v.MemberLastName, CustomerEmail = v.MemberEmail, CustomerPhone = v.MemberPhone, CustomerAddress1 = v.MemberAddress1, CustomerAddress2 = v.MemberAddress2, CustomerCity = v.MemberCity, CustomerCountry = v.MemberCountry, CustomerPostCode = v.MemberPostCode, CustomerState = v.MemberState, CustomerId = v.MemberId, ProcessDate = processdate, ProcessStatus = v.ProcessStatus, OrderCompletion = v.TransactionStatus, PaymentDate = paymentdate, PaymentStatus = v.PaymentStatus, PaymentStatusId = v.PaymentStatusId, PaymentAmount = paymentamount, PaymentMethod = v.PaymentMethod, Gift = GiftS, GiftNote = v.GiftNote });
                    }
                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment; filename=Transaction.xls");
                    Response.ContentType = "application/ms-excel";
                    Response.Charset = "";
                    StringWriter sw = new StringWriter();
                    HtmlTextWriter htw = new HtmlTextWriter(sw);
                    System.Web.UI.WebControls.GridView gv = new System.Web.UI.WebControls.GridView();
                    gv.DataSource = newlistmer.ToList();
                    gv.DataBind();
                    gv.RenderControl(htw);
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                    ViewBag.msg = "11";
                    TempData["msg"] = ViewBag.msg;
                    return RedirectToAction("Transaction");
                }
                else if (model.DropdownType == 2)
                {
                    if (model.TransactionSelected != null && model.TransactionSelected != "")
                    {
                        List<com.kuazoo.TransactionDetail3> listmer = transactionservice.GetSelectedTransactionList(model.TransactionSelected).Result;
                        List<TransactionModel.TransactionDetail3Extract> newlistmer = new List<TransactionModel.TransactionDetail3Extract>();
                        foreach (var v in listmer)
                        {
                            DateTime? processdate = null;
                            if (v.ProcessDate.HasValue)
                            {
                                processdate = v.ProcessDate.Value.AddHours(Helper.defaultGMT);
                            }
                            DateTime? paymentdate = null;
                            if (v.PaymentDate.HasValue)
                            {
                                paymentdate = v.PaymentDate.Value;//already local time
                            }
                            decimal? paymentamount = null;
                            if (v.PaymentAmount != 0)
                            {
                                paymentamount = v.PaymentAmount;
                            }
                            var GiftS = v.Gift == true ? "Yes" : "No";
                            newlistmer.Add(new TransactionModel.TransactionDetail3Extract() { TransactionId = v.TransactionId, OrderId = GeneralService.TransactionCode(v.TransactionId, v.TransactionDate), TransactionDate = v.TransactionDate.AddHours(Helper.defaultGMT), MerchantId = v.MerchantId, MerchantName = v.MerchantName, InventoryItemId = v.InventoryItemId, InventoryItemName = v.InventoryItemName, Variance = v.Variance, SKU = v.SKU, Qty = v.Qty, VoucherCode = v.VoucherCode, CustomerFirstName = v.MemberFirstName, CustomerLastName = v.MemberLastName, CustomerEmail = v.MemberEmail, CustomerPhone = v.MemberPhone, CustomerAddress1 = v.MemberAddress1, CustomerAddress2 = v.MemberAddress2, CustomerCity = v.MemberCity, CustomerCountry = v.MemberCountry, CustomerPostCode = v.MemberPostCode, CustomerState = v.MemberState, CustomerId = v.MemberId, ProcessDate = processdate, ProcessStatus = v.ProcessStatus, OrderCompletion = v.TransactionStatus, PaymentDate = paymentdate, PaymentStatus = v.PaymentStatus, PaymentStatusId = v.PaymentStatusId, PaymentAmount = paymentamount, PaymentMethod = v.PaymentMethod, Gift = GiftS, GiftNote = v.GiftNote });
                        }
                        Response.ClearContent();
                        Response.Buffer = true;
                        Response.AddHeader("content-disposition", "attachment; filename=Transaction.xls");
                        Response.ContentType = "application/ms-excel";
                        Response.Charset = "";
                        StringWriter sw = new StringWriter();
                        HtmlTextWriter htw = new HtmlTextWriter(sw);
                        System.Web.UI.WebControls.GridView gv = new System.Web.UI.WebControls.GridView();
                        gv.DataSource = newlistmer.ToList();
                        gv.DataBind();
                        gv.RenderControl(htw);
                        Response.Output.Write(sw.ToString());
                        Response.Flush();
                        Response.End();
                        ViewBag.msg = "21";
                        TempData["msg"] = ViewBag.msg;
                        return RedirectToAction("Transaction");
                    }
                    else
                    {
                        ViewBag.msg = "23";
                        return View(model);
                    }
                }
                else if (model.DropdownType == 3)
                {
                    if (model.TransactionSelected != null && model.TransactionSelected != "")
                    {
                        Boolean result = transactionservice.UpdateSelectedTransactionList(model.TransactionSelected).Result;

                        List<com.kuazoo.TransactionDetail3> listmer = transactionservice.GetSelectedTransactionList(model.TransactionSelected).Result;
                        List<TransactionModel.TransactionDetail3Extract> newlistmer = new List<TransactionModel.TransactionDetail3Extract>();
                        foreach (var v in listmer)
                        {
                            DateTime? processdate = null;
                            if (v.ProcessDate.HasValue)
                            {
                                processdate = v.ProcessDate.Value.AddHours(Helper.defaultGMT);
                            }
                            DateTime? paymentdate = null;
                            if (v.PaymentDate.HasValue)
                            {
                                paymentdate = v.PaymentDate.Value;//already local time
                            }
                            decimal? paymentamount = null;
                            if (v.PaymentAmount != 0)
                            {
                                paymentamount = v.PaymentAmount;
                            }
                            var GiftS = v.Gift == true ? "Yes" : "No";
                            newlistmer.Add(new TransactionModel.TransactionDetail3Extract() { TransactionId = v.TransactionId, OrderId = GeneralService.TransactionCode(v.TransactionId, v.TransactionDate), TransactionDate = v.TransactionDate.AddHours(Helper.defaultGMT), MerchantId = v.MerchantId, MerchantName = v.MerchantName, InventoryItemId = v.InventoryItemId, InventoryItemName = v.InventoryItemName, Variance = v.Variance, SKU = v.SKU, Qty = v.Qty, VoucherCode = v.VoucherCode, CustomerFirstName = v.MemberFirstName, CustomerLastName = v.MemberLastName, CustomerEmail = v.MemberEmail, CustomerPhone = v.MemberPhone, CustomerAddress1 = v.MemberAddress1, CustomerAddress2 = v.MemberAddress2, CustomerCity = v.MemberCity, CustomerCountry = v.MemberCountry, CustomerPostCode = v.MemberPostCode, CustomerState = v.MemberState, CustomerId = v.MemberId, ProcessDate = processdate, ProcessStatus = v.ProcessStatus, OrderCompletion = v.TransactionStatus, PaymentDate = paymentdate, PaymentStatus = v.PaymentStatus, PaymentStatusId = v.PaymentStatusId, PaymentAmount = paymentamount, PaymentMethod = v.PaymentMethod, Gift = GiftS, GiftNote = v.GiftNote });
                        }
                        Response.ClearContent();
                        Response.Buffer = true;
                        Response.AddHeader("content-disposition", "attachment; filename=Transaction.xls");
                        Response.ContentType = "application/ms-excel";
                        Response.Charset = "";
                        StringWriter sw = new StringWriter();
                        HtmlTextWriter htw = new HtmlTextWriter(sw);
                        System.Web.UI.WebControls.GridView gv = new System.Web.UI.WebControls.GridView();
                        gv.DataSource = newlistmer.ToList();
                        gv.DataBind();
                        gv.RenderControl(htw);
                        Response.Output.Write(sw.ToString());
                        Response.Flush();
                        Response.End();
                        ViewBag.msg = "31";
                        TempData["msg"] = ViewBag.msg;
                        return RedirectToAction("Transaction");
                    }
                    else
                    {
                        ViewBag.msg = "33";
                        return View(model);
                    }
                }
                else if (model.DropdownType == 4)
                {
                    if (model.TransactionSelected != null && model.TransactionSelected != "")
                    {
                        Boolean result = transactionservice.UpdateSelectedTransactionList(model.TransactionSelected).Result;
                        ViewBag.msg = "41";
                        TempData["msg"] = ViewBag.msg;
                        return RedirectToAction("Transaction");
                    }
                    else
                    {
                        ViewBag.msg = "43";
                        return View(model);
                    }
                }
            }
            catch{
                ViewBag.msg = "3";
                return View(model);
            }
            return View();
        }
        public ActionResult Transaction_StatusList()
        {
            List<string> statuslist = new List<string>();
            statuslist.Add("Completed");
            statuslist.Add("Pending");
            return Json(statuslist, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Process_StatusList()
        {
            List<string> statuslist = new List<string>();
            statuslist.Add("Yes");
            statuslist.Add("No");
            return Json(statuslist, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Payment_StatusList()
        {
            List<string> statuslist = new List<string>();
            statuslist.Add("Success");
            statuslist.Add("Pending");
            statuslist.Add("Pending Verification");
            statuslist.Add("Failed");
            return Json(statuslist, JsonRequestBehavior.AllowGet);
        }
        //public ActionResult Transaction_Read([DataSourceRequest] DataSourceRequest request)
        //{
        //    List<com.kuazoo.Transaction> listmer = transactionservice.GetTransactionList().Result;
        //    List<TransactionModel.Transaction> newlistmer = new List<TransactionModel.Transaction>();
        //    bool kuazooed = false;
        //    foreach (var v in listmer)
        //    {
        //        //if (v.Sales < v.MaximumSales) kuazooed = false;
        //        if (v.SalesVisualMeter >= 100) kuazooed = true;
        //        else kuazooed = false;
        //        newlistmer.Add(new TransactionModel.Transaction() { InventoryItemId = v.InventoryItemId, InventoryItemName = v.InventoryItemName, MerchantId = v.MerchantId, MerchantName = v.MerchantName, Sales = v.Sales, MaximumSales = v.MaximumSales, ExpireDate = v.ExpireDate.AddHours(Helper.defaultGMT), ExpireDateStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.ExpireDate.AddHours(Helper.defaultGMT)), Kuazooed = kuazooed,NewOrder=v.NewOrder });
        //    }
        //    IEnumerable<TransactionModel.Transaction> ienuList = newlistmer;
        //    var result = ienuList.ToDataSourceResult(request);
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
        public ActionResult TransactionDetail2_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<com.kuazoo.TransactionDetail3> listmer = transactionservice.GetAllTransactionList().Result;
            List<TransactionModel.TransactionDetail3> newlistmer = new List<TransactionModel.TransactionDetail3>();
            foreach (var v in listmer)
            {
                DateTime? processdate = null;
                if (v.ProcessDate.HasValue)
                {
                    processdate = v.ProcessDate.Value;//utc time//.AddHours(Helper.defaultGMT);
                }
                DateTime? paymentdate = null;
                if (v.PaymentDate.HasValue)
                {
                    paymentdate = v.PaymentDate.Value.AddHours(Helper.defaultGMT * -1);//convert to utc time
                }
                newlistmer.Add(new TransactionModel.TransactionDetail3() { TransactionId = v.TransactionId, OrderId = GeneralService.TransactionCode(v.TransactionId, v.TransactionDate), TransactionDate = v.TransactionDate, MerchantId = v.MerchantId, MerchantName = v.MerchantName, InventoryItemId = v.InventoryItemId, InventoryItemName = v.InventoryItemName, Variance = v.Variance, SKU = v.SKU, Qty = v.Qty, CustomerFirstName = v.MemberFirstName, CustomerLastName = v.MemberLastName, CustomerEmail = v.MemberEmail, CustomerPhone = v.MemberPhone, CustomerAddress1 = v.MemberAddress1, CustomerAddress2 = v.MemberAddress2, CustomerCity = v.MemberCity, CustomerCountry = v.MemberCountry, CustomerPostCode = v.MemberPostCode, CustomerState = v.MemberState, CustomerId = v.MemberId, ProcessDate = processdate, ProcessStatus = v.ProcessStatus, OrderCompletion = v.TransactionStatus, PaymentDate = paymentdate, PaymentStatus = v.PaymentStatus, PaymentStatusId = v.PaymentStatusId, Gift = v.Gift==true?"Yes":"No" });
            }
            IEnumerable<TransactionModel.TransactionDetail3> ienuList = newlistmer;
            var result = ienuList.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //public ActionResult TransactionDetail2_Update([DataSourceRequest] DataSourceRequest request, TransactionModel.TransactionDetail transaction)
        //{
        //    if (transaction != null)//&& ModelState.IsValid)
        //    {
        //        try
        //        {
        //            int TransactionId = transaction.TransactionId;
        //            int TransactionStatus = transaction.ProcessStatus.Id;
        //            transactionservice.UpdateTransactionStatus(TransactionId, TransactionStatus);
        //            //ModelState.AddModelError("", "Success Update");
        //            ModelState.AddModelError("", "1");

        //        }
        //        catch (Exception ex)
        //        {
        //            ModelState.AddModelError("", "2");
        //            //ModelState.AddModelError("", "Unable to update role. " + ex.Message);
        //        }
        //    }
        //    return Json(ModelState.ToDataSourceResult());
        //}
        //public ActionResult TransactionDetail(int InventoryItemId)
        //{
        //    if (TempData["msg"] != null)
        //    {
        //        ViewBag.msg = TempData["msg"];
        //    }
        //    ViewBag.InventoryItemId = InventoryItemId;

        //    List<TransactionModel.TransactionStatusDrop> statuslist = new List<TransactionModel.TransactionStatusDrop>();
        //    statuslist.Add(new TransactionModel.TransactionStatusDrop() { Id = 1, Name = "Done" });
        //    statuslist.Add(new TransactionModel.TransactionStatusDrop() { Id = 2, Name = "Ship" });
        //    statuslist.Add(new TransactionModel.TransactionStatusDrop() { Id = 3, Name = "Pending" });
        //    statuslist.Add(new TransactionModel.TransactionStatusDrop() { Id = 4, Name = "WaitingPayment" });

        //    ViewData["statuslist"] = statuslist;
        //    ViewData["defaultStatus"] = statuslist.First();   
        //    return View();
        //}
        //public ActionResult TransactionDetail_Read([DataSourceRequest] DataSourceRequest request, int InventoryItemId)
        //{
        //    List<com.kuazoo.TransactionDetail> listmer = transactionservice.GetTransactionDetailByInventoryItemId(InventoryItemId).Result;
        //    List<TransactionModel.TransactionDetail> newlistmer = new List<TransactionModel.TransactionDetail>();
        //    foreach (var v in listmer)
        //    {
        //        DateTime? processdate=null;
        //        if(v.ProcessDate.HasValue){
        //            processdate = v.ProcessDate.Value.AddHours(Helper.defaultGMT);
        //        }
        //        newlistmer.Add(new TransactionModel.TransactionDetail() { TransactionId = v.TransactionId, OrderId=GeneralService.TransactionCode(v.TransactionId,v.TransactionDate), InventoryItemId = v.InventoryItemId, InventoryItemName = v.InventoryItemName, MerchantId = v.MerchantId, MerchantName = v.MerchantName, TransactionDate = v.TransactionDate.AddHours(Helper.defaultGMT), TransactionDateStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.TransactionDate.AddHours(Helper.defaultGMT)), MemberId = v.MemberId, MemberEmail = v.MemberEmail, FlashDealId = v.FlashDealId, LastAction = v.LastAction, ProcessStatus = new TransactionModel.TransactionStatusDrop() { Id = v.TransactionTypeId, Name = v.TransactionType }, ProcessDate = processdate });
        //    }
        //    IEnumerable<TransactionModel.TransactionDetail> ienuList = newlistmer;
        //    var result = ienuList.ToDataSourceResult(request);
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
        //public ActionResult TransactionDetail_Update([DataSourceRequest] DataSourceRequest request, int InventoryItemId, TransactionModel.TransactionDetail transaction)
        //{
        //    if (transaction != null)//&& ModelState.IsValid)
        //    {
        //        try
        //        {
        //            int TransactionId = transaction.TransactionId;
        //            int TransactionStatus = transaction.ProcessStatus.Id;
        //            transactionservice.UpdateTransactionStatus(TransactionId, TransactionStatus);
        //            //ModelState.AddModelError("", "Success Update");
        //            ModelState.AddModelError("", "1");

        //        }
        //        catch (Exception ex)
        //        {
        //            ModelState.AddModelError("", "2");
        //            //ModelState.AddModelError("", "Unable to update role. " + ex.Message);
        //        }
        //    }
        //    return Json(ModelState.ToDataSourceResult());
        //}
        public ActionResult TransactionView(int TransactionId)
        {
            com.kuazoo.TransactionDetail v = transactionservice.GetTransactionDetailById(TransactionId).Result;
            ViewBag.InventoryItemId = v.InventoryItemId;

            string gender = "";
            if (v.Member.Gender == 0) gender = "Male";
            else if (v.Member.Gender == 1) gender = "Female";
            TransactionModel.TransactionDetail Transaction;
            Transaction = new TransactionModel.TransactionDetail()
            {
                TransactionId = v.TransactionId,
                InventoryItemId = v.InventoryItemId,
                InventoryItemName = v.InventoryItemName,
                MerchantId = v.MerchantId,
                MerchantName = v.MerchantName,
                TransactionDate = v.TransactionDate.AddHours(Helper.defaultGMT),
                TransactionDateStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.TransactionDate.AddHours(Helper.defaultGMT)),
                MemberId = v.MemberId,
                MemberEmail = v.MemberEmail,
                FlashDealId = v.FlashDealId,
                LastAction = v.LastAction,
                InventoryItem = new InventoryItemModel.InventoryItem() { InventoryItemId = v.InventoryItemId, Name = v.InventoryItem.Name, Price = v.InventoryItem.Price, Description = v.InventoryItem.Description, MerchantId = v.InventoryItem.Merchant.MerchantId, MerchantName = v.InventoryItem.Merchant.Name, MerchantCityName = v.InventoryItem.Merchant.City.Name, MerchantCountryName = v.InventoryItem.Merchant.Country.Name, CountryId = v.InventoryItem.Country.CountryId, Country = new MasterModel.Country() { CountryId = v.InventoryItem.Country.CountryId, CountryName = v.InventoryItem.Country.Name }, CityId = v.InventoryItem.City.CityId, City = new MasterModel.City() { CityId = v.InventoryItem.City.CityId, CityName = v.InventoryItem.City.Name }, Keyword = v.InventoryItem.Keyword, InventoryItemTypeId = v.InventoryItem.InventoryItemType.InventoryItemTypeId, InventoryItemTypeName = v.InventoryItem.InventoryItemType.InventoryItemTypeName, Discount = v.InventoryItem.Discount, ExpireDate = v.InventoryItem.ExpireDate.AddHours(Helper.defaultGMT), MaximumSales = v.InventoryItem.MaximumSales, PublishDate = v.InventoryItem.PublishDate.AddHours(Helper.defaultGMT), MinimumTarget = v.InventoryItem.MinimumTarget, SalesVisualMeter = v.InventoryItem.SalesVisualMeter, LastAction = v.InventoryItem.LastAction, Tag = v.InventoryItem.Tag, TagName = v.InventoryItem.TagName, ImageName = v.InventoryItem.ImageName, ImageUrl = v.InventoryItem.ImageUrl, Flag = v.InventoryItem.Flag },
                Member = new MemberModel.Member() { MemberId = v.MemberId, FirstName = v.Member.FirstName, LastName = v.Member.LastName, Email = v.Member.Email, Gender = v.Member.Gender, GenderStr = gender, DateOfBirth = v.Member.DateOfBirth, MemberStatus = new MemberModel.MemberStatus() { MemberStatusId = v.Member.MemberStatus.MemberStatusId, MemberStatusName = v.Member.MemberStatus.MemberStatusName }, LastLockout = v.Member.LastLockout, LastLockoutDate = v.Member.LastLockoutDate },
            };

            return View(Transaction);
        }

        public ActionResult FailedTransaction()
        {
            if (TempData["msg"] != null)
            {
                ViewBag.msg = TempData["msg"];
            }
            return View();
        }
        public ActionResult FailedTransaction_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<com.kuazoo.TransactionDetail3> listmer = transactionservice.GetAllTransactionFailedList().Result;
            List<TransactionModel.TransactionDetail3> newlistmer = new List<TransactionModel.TransactionDetail3>();
            foreach (var v in listmer)
            {
                DateTime? processdate = null;
                if (v.ProcessDate.HasValue)
                {
                    processdate = v.ProcessDate.Value;//utc time//.AddHours(Helper.defaultGMT);
                }
                DateTime? paymentdate = null;
                if (v.PaymentDate.HasValue)
                {
                    paymentdate = v.PaymentDate.Value.AddHours(Helper.defaultGMT * -1);//convert to utc time
                }
                newlistmer.Add(new TransactionModel.TransactionDetail3() { TransactionId = v.TransactionId, OrderId = GeneralService.TransactionCode(v.TransactionId, v.TransactionDate), TransactionDate = v.TransactionDate.AddHours(Helper.defaultGMT), MerchantId = v.MerchantId, MerchantName = v.MerchantName, InventoryItemId = v.InventoryItemId, InventoryItemName = v.InventoryItemName, Variance = v.Variance, SKU = v.SKU, Qty = v.Qty, CustomerFirstName = v.MemberFirstName, CustomerLastName = v.MemberLastName, CustomerEmail = v.MemberEmail, CustomerPhone = v.MemberPhone, CustomerAddress1 = v.MemberAddress1, CustomerAddress2 = v.MemberAddress2, CustomerCity = v.MemberCity, CustomerCountry = v.MemberCountry, CustomerPostCode = v.MemberPostCode, CustomerState = v.MemberState, CustomerId = v.MemberId, ProcessDate = processdate, ProcessStatus = v.ProcessStatus, OrderCompletion = v.TransactionStatus, PaymentDate = paymentdate, PaymentStatus = v.PaymentStatus, PaymentStatusId = v.PaymentStatusId });
            }
            IEnumerable<TransactionModel.TransactionDetail3> ienuList = newlistmer;
            var result = ienuList.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult InsertTransaction()
        //{
        //    if (TempData["msg"] != null)
        //    {
        //        ViewBag.msg = TempData["msg"];
        //    }
        //    return View();
        //}
        //[HttpPost]
        //public ActionResult InsertTransaction(TransactionModel.TempTransaction temp)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            com.kuazoo.TransactionDetail transaction = new com.kuazoo.TransactionDetail();
        //            transaction.InventoryItemId = temp.InventoryItemId;
        //            transaction.MemberId = temp.MemberId;
        //            transaction.TransactionDate = DateTime.Now;
        //            transaction.FlashDealId = null;
        //            bool result = transactionservice.CreateTransaction(transaction).Result;
        //            if (result == true)
        //            {
        //                ViewBag.result = "1";
        //            }
        //            else
        //            {
        //                ViewBag.result = "3";
        //            }
        //            return RedirectToAction("InsertTransaction", "Admin");
        //        }
        //        catch (Exception ex)
        //        {
        //            ViewBag.result = ex.Message;
        //        }
        //    }
        //    return View(temp);
        //}
        #endregion

        #region MinimumTarget

        public ActionResult MinimumTarget()
        {
            if (TempData["msg"] != null)
            {
                ViewBag.msg = TempData["msg"];
            }
            return View();
        }
        public ActionResult MinimumTarget_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<com.kuazoo.MinimumTarget> listmer = transactionservice.GetMinimumTargetList().Result;
            List<TransactionModel.MinimumTarget> newlistmer = new List<TransactionModel.MinimumTarget>();
            foreach (var v in listmer)
            {
                newlistmer.Add(new TransactionModel.MinimumTarget() { InventoryItemId = v.InventoryItemId, InventoryItemName = v.InventoryItemName, MerchantId = v.MerchantId, MerchantName = v.MerchantName, CurrentSales = v.CurrentSales, Minimum = v.Minimum });
            }
            IEnumerable<TransactionModel.MinimumTarget> ienuList = newlistmer;
            var result = ienuList.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult MinimumTargetDetail(int InventoryItemId)
        {
            if (TempData["msg"] != null)
            {
                ViewBag.msg = TempData["msg"];
            }
            ViewBag.InventoryItemId = InventoryItemId;
            return View();
        }
        public ActionResult MinimumTargetDetail_Read([DataSourceRequest] DataSourceRequest request, int InventoryItemId)
        {
            List<com.kuazoo.TransactionDetail> listmer = transactionservice.GetTransactionDetailByInventoryItemId(InventoryItemId).Result;
            List<TransactionModel.TransactionDetail> newlistmer = new List<TransactionModel.TransactionDetail>();
            foreach (var v in listmer)
            {
                newlistmer.Add(new TransactionModel.TransactionDetail() { TransactionId = v.TransactionId, InventoryItemId = v.InventoryItemId, InventoryItemName = v.InventoryItemName, MerchantId = v.MerchantId, MerchantName = v.MerchantName, TransactionDate = v.TransactionDate.AddHours(Helper.defaultGMT), TransactionDateStr = string.Format("{0:dd MMM yyyy hh:mm tt}", v.TransactionDate.AddHours(Helper.defaultGMT)), MemberId = v.MemberId, MemberEmail = v.MemberEmail, FlashDealId = v.FlashDealId, LastAction = v.LastAction });
            }
            IEnumerable<TransactionModel.TransactionDetail> ienuList = newlistmer;
            var result = ienuList.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region FreeDeal Transaction

        public ActionResult TransactionFreeDeal()
        {
            if (TempData["msg"] != null)
            {
                ViewBag.msg = TempData["msg"];
            }
            return View();
        }
        [HttpPost]
        public ActionResult TransactionFreeDeal(TransactionModel.TransactionDetail3 model)
        {
            try
            {
                if (model.DropdownType == 1)
                {
                    List<com.kuazoo.FreeDealTransactionReport> listmer = freedealservice.GetAllTransactionFreeDealList().Result;
                    List<TransactionModel.FreeDealTransactionReportExtract> newlistmer = new List<TransactionModel.FreeDealTransactionReportExtract>();
                    foreach (var v in listmer)
                    {
                        DateTime? processdate = null;
                        if (v.ProcessDate.HasValue)
                        {
                            processdate = v.ProcessDate.Value.AddHours(Helper.defaultGMT);
                        }
                        newlistmer.Add(new TransactionModel.FreeDealTransactionReportExtract() { TransactionId = v.TransactionId, OrderId = GeneralService.FreeTransactionCode(v.TransactionId, v.TransactionDate), TransactionDate = v.TransactionDate.AddHours(Helper.defaultGMT), MerchantId = v.MerchantId, MerchantName = v.MerchantName, InventoryItemId = v.InventoryItemId, InventoryItemName = v.InventoryItemName, Variance = v.Variance, SKU = v.SKU, Qty = v.Qty, VoucherCode = v.VoucherCode, CustomerFirstName = v.MemberFirstName, CustomerLastName = v.MemberLastName, CustomerEmail = v.MemberEmail,  CustomerId = v.MemberId, ProcessDate = processdate, ProcessStatus = v.ProcessStatus});
                    }
                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment; filename=Transaction.xls");
                    Response.ContentType = "application/ms-excel";
                    Response.Charset = "";
                    StringWriter sw = new StringWriter();
                    HtmlTextWriter htw = new HtmlTextWriter(sw);
                    System.Web.UI.WebControls.GridView gv = new System.Web.UI.WebControls.GridView();
                    gv.DataSource = newlistmer.ToList();
                    gv.DataBind();
                    gv.RenderControl(htw);
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                    ViewBag.msg = "11";
                    TempData["msg"] = ViewBag.msg;
                    return RedirectToAction("TransactionFreeDeal");
                }
                else if (model.DropdownType == 2)
                {
                    if (model.TransactionSelected != null && model.TransactionSelected != "")
                    {
                        List<com.kuazoo.FreeDealTransactionReport> listmer = freedealservice.GetSelectedTransactionFreeDealList(model.TransactionSelected).Result;
                        List<TransactionModel.FreeDealTransactionReportExtract> newlistmer = new List<TransactionModel.FreeDealTransactionReportExtract>();
                        foreach (var v in listmer)
                        {
                            DateTime? processdate = null;
                            if (v.ProcessDate.HasValue)
                            {
                                processdate = v.ProcessDate.Value.AddHours(Helper.defaultGMT);
                            }
                            newlistmer.Add(new TransactionModel.FreeDealTransactionReportExtract() { TransactionId = v.TransactionId, OrderId = GeneralService.FreeTransactionCode(v.TransactionId, v.TransactionDate), TransactionDate = v.TransactionDate.AddHours(Helper.defaultGMT), MerchantId = v.MerchantId, MerchantName = v.MerchantName, InventoryItemId = v.InventoryItemId, InventoryItemName = v.InventoryItemName, Variance = v.Variance, SKU = v.SKU, Qty = v.Qty, VoucherCode = v.VoucherCode, CustomerFirstName = v.MemberFirstName, CustomerLastName = v.MemberLastName, CustomerEmail = v.MemberEmail, CustomerId = v.MemberId, ProcessDate = processdate, ProcessStatus = v.ProcessStatus });
                        }
                        Response.ClearContent();
                        Response.Buffer = true;
                        Response.AddHeader("content-disposition", "attachment; filename=Transaction.xls");
                        Response.ContentType = "application/ms-excel";
                        Response.Charset = "";
                        StringWriter sw = new StringWriter();
                        HtmlTextWriter htw = new HtmlTextWriter(sw);
                        System.Web.UI.WebControls.GridView gv = new System.Web.UI.WebControls.GridView();
                        gv.DataSource = newlistmer.ToList();
                        gv.DataBind();
                        gv.RenderControl(htw);
                        Response.Output.Write(sw.ToString());
                        Response.Flush();
                        Response.End();
                        ViewBag.msg = "21";
                        TempData["msg"] = ViewBag.msg;
                        return RedirectToAction("Transaction");
                    }
                    else
                    {
                        ViewBag.msg = "23";
                        return View(model);
                    }
                }
                else if (model.DropdownType == 3)
                {
                    if (model.TransactionSelected != null && model.TransactionSelected != "")
                    {
                        Boolean result = freedealservice.UpdateSelectedTransactionFreeDealList(model.TransactionSelected).Result;

                        List<com.kuazoo.FreeDealTransactionReport> listmer = freedealservice.GetSelectedTransactionFreeDealList(model.TransactionSelected).Result;
                        List<TransactionModel.FreeDealTransactionReportExtract> newlistmer = new List<TransactionModel.FreeDealTransactionReportExtract>();
                        foreach (var v in listmer)
                        {
                            DateTime? processdate = null;
                            if (v.ProcessDate.HasValue)
                            {
                                processdate = v.ProcessDate.Value.AddHours(Helper.defaultGMT);
                            }
                            newlistmer.Add(new TransactionModel.FreeDealTransactionReportExtract() { TransactionId = v.TransactionId, OrderId = GeneralService.FreeTransactionCode(v.TransactionId, v.TransactionDate), TransactionDate = v.TransactionDate.AddHours(Helper.defaultGMT), MerchantId = v.MerchantId, MerchantName = v.MerchantName, InventoryItemId = v.InventoryItemId, InventoryItemName = v.InventoryItemName, Variance = v.Variance, SKU = v.SKU, Qty = v.Qty, VoucherCode = v.VoucherCode, CustomerFirstName = v.MemberFirstName, CustomerLastName = v.MemberLastName, CustomerEmail = v.MemberEmail, CustomerId = v.MemberId, ProcessDate = processdate, ProcessStatus = v.ProcessStatus });
                        }
                        Response.ClearContent();
                        Response.Buffer = true;
                        Response.AddHeader("content-disposition", "attachment; filename=Transaction.xls");
                        Response.ContentType = "application/ms-excel";
                        Response.Charset = "";
                        StringWriter sw = new StringWriter();
                        HtmlTextWriter htw = new HtmlTextWriter(sw);
                        System.Web.UI.WebControls.GridView gv = new System.Web.UI.WebControls.GridView();
                        gv.DataSource = newlistmer.ToList();
                        gv.DataBind();
                        gv.RenderControl(htw);
                        Response.Output.Write(sw.ToString());
                        Response.Flush();
                        Response.End();
                        ViewBag.msg = "31";
                        TempData["msg"] = ViewBag.msg;
                        return RedirectToAction("Transaction");
                    }
                    else
                    {
                        ViewBag.msg = "33";
                        return View(model);
                    }
                }
                else if (model.DropdownType == 4)
                {
                    if (model.TransactionSelected != null && model.TransactionSelected != "")
                    {
                        Boolean result = freedealservice.UpdateSelectedTransactionFreeDealList(model.TransactionSelected).Result;
                        ViewBag.msg = "41";
                        TempData["msg"] = ViewBag.msg;
                        return RedirectToAction("Transaction");
                    }
                    else
                    {
                        ViewBag.msg = "43";
                        return View(model);
                    }
                }
            }
            catch
            {
                ViewBag.msg = "3";
                return View(model);
            }
            return View();
        }
        public ActionResult TransactionFreeDeal_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<com.kuazoo.FreeDealTransactionReport> listmer = freedealservice.GetAllTransactionFreeDealList().Result;
            List<TransactionModel.FreeDealTransactionReport> newlistmer = new List<TransactionModel.FreeDealTransactionReport>();
            foreach (var v in listmer)
            {
                DateTime? processdate = null;
                if (v.ProcessDate.HasValue)
                {
                    processdate = v.ProcessDate.Value;//utc time//.AddHours(Helper.defaultGMT);
                }
                newlistmer.Add(new TransactionModel.FreeDealTransactionReport() { TransactionId = v.TransactionId, OrderId = GeneralService.FreeTransactionCode(v.TransactionId, v.TransactionDate), TransactionDate = v.TransactionDate, MerchantId = v.MerchantId, MerchantName = v.MerchantName, InventoryItemId = v.InventoryItemId, InventoryItemName = v.InventoryItemName, Variance = v.Variance, SKU = v.SKU, Qty = v.Qty, CustomerFirstName = v.MemberFirstName, CustomerLastName = v.MemberLastName, CustomerEmail = v.MemberEmail, CustomerId = v.MemberId, ProcessDate = processdate, ProcessStatus = v.ProcessStatus });
            }
            IEnumerable<TransactionModel.FreeDealTransactionReport> ienuList = newlistmer;
            var result = ienuList.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region Prize
        public ActionResult Prize()
        {
            if (TempData["msg"] != null)
            {
                ViewBag.msg = TempData["msg"];
            }
            return View();
        }
        public ActionResult Prize_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<com.kuazoo.Prize> listmer = prizeservice.GetPrizeList().Result;
            List<InventoryItemModel.Prize> newlistmer = new List<InventoryItemModel.Prize>();
            foreach (var v in listmer)
            {
                newlistmer.Add(new InventoryItemModel.Prize() { Create=v.Create.AddHours(Helper.defaultGMT), PrizeId = v.PrizeId, Name = v.Name, SponsorName = v.SponsorName, Price = v.Price, ExpiryDate = v.ExpiryDate.AddHours(Helper.defaultGMT),PublishDate = v.PublishDate.AddHours(Helper.defaultGMT),GroupName =v.GroupName, TotalRevenue= v.TotalRevenue, LastAction = v.LastAction,WinnerEmail = v.WinnerEmail });
            }
            IEnumerable<InventoryItemModel.Prize> ienuList = newlistmer;
            var result = ienuList.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public String Prize_Duplicate(int PrizeId)
        {
            string res = "";
            if (PrizeId != null && PrizeId > 0)
            {
                try
                {
                    Boolean result = prizeservice.DuplicatePrize(PrizeId).Result;

                    if (result) res = "1";
                    else res = "2";
                }
                catch (Exception ex)
                {
                    CustomException(ex);
                    res = ex.Message;
                }

            }

            return res;
        }
        //public ActionResult PrizeDetail(int PrizeId)
        //{
        //    if (TempData["msg"] != null)
        //    {
        //        ViewBag.msg = TempData["msg"];
        //    }
        //    ViewBag.PrizeId = PrizeId;
        //    return View();
        //}
        public ActionResult PrizeDetail_Read([DataSourceRequest] DataSourceRequest request, int PrizeId)
        {
            List<com.kuazoo.InventoryItem> listmer = inventoryitemservice.GetInventoryItemListByPrize(PrizeId).Result;
            List<InventoryItemModel.InventoryItem> newlistmer = new List<InventoryItemModel.InventoryItem>();
            bool kuazooed = false;
            string name = "";
            foreach (var v in listmer)
            {
                kuazooed = false;
                if (v.SalesVisualMeter >= 100 || v.RemainSales<1) kuazooed = true;
                name = v.Name;
                if (v.Draft)
                {
                    name = "Draft - " +name;
                }
                newlistmer.Add(new InventoryItemModel.InventoryItem() { InventoryItemId = v.InventoryItemId, Name = name, Kuazooed =kuazooed});

                //if (v.Prize != null)
                //{
                //    newlistmer.Add(new InventoryItemModel.InventoryItem() { InventoryItemId = v.InventoryItemId, Name = v.Name, ShortDesc = v.ShortDesc, Price = v.Price, Description = v.Description, MerchantId = v.Merchant.MerchantId, MerchantName = v.Merchant.Name, CityName = v.Merchant.City.Name, CountryName = v.Merchant.Country.Name, Keyword = v.Keyword, InventoryItemTypeId = v.InventoryItemType.InventoryItemTypeId, InventoryItemTypeName = v.InventoryItemType.InventoryItemTypeName, Discount = v.Discount, ExpireDate = v.ExpireDate, MaximumSales = v.MaximumSales, PublishDate = v.PublishDate, MinimumTarget = v.MinimumTarget, SalesVisualMeter = v.SalesVisualMeter, LastAction = v.LastAction, Tag = v.Tag, TagName = v.TagName, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = String.Join("|", v.SubImageId), SubImageName = String.Join("|", v.SubImageName), SubImageUrl = String.Join("|", v.SubImageUrl), Flag = v.Flag, Prize = new InventoryItemModel.Prize() { PrizeId = v.Prize.PrizeId, Name = v.Prize.Name, Description = v.Prize.Description, ImageId = v.Prize.ImageId, ImageName = v.Prize.ImageName, ImageUrl = v.Prize.ImageUrl, SubImageId = String.Join(",", v.Prize.SubImageId), SubImageName = String.Join("|", v.Prize.SubImageName), SubImageUrl = String.Join("|", v.Prize.SubImageUrl), Price = v.Prize.Price, SponsorName = v.Prize.SponsorName, Detail = v.Prize.Detail, Terms = v.Prize.Terms } });
                //}
                //else
                //{
                //    newlistmer.Add(new InventoryItemModel.InventoryItem() { InventoryItemId = v.InventoryItemId, Name = v.Name, ShortDesc = v.ShortDesc, Price = v.Price, Description = v.Description, MerchantId = v.Merchant.MerchantId, MerchantName = v.Merchant.Name, CityName = v.Merchant.City.Name, CountryName = v.Merchant.Country.Name, Keyword = v.Keyword, InventoryItemTypeId = v.InventoryItemType.InventoryItemTypeId, InventoryItemTypeName = v.InventoryItemType.InventoryItemTypeName, Discount = v.Discount, ExpireDate = v.ExpireDate, MaximumSales = v.MaximumSales, PublishDate = v.PublishDate, MinimumTarget = v.MinimumTarget, SalesVisualMeter = v.SalesVisualMeter, LastAction = v.LastAction, Tag = v.Tag, TagName = v.TagName, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = String.Join("|", v.SubImageId), SubImageName = String.Join("|", v.SubImageName), SubImageUrl = String.Join("|", v.SubImageUrl), Flag = v.Flag });
                //}
            }
            IEnumerable<InventoryItemModel.InventoryItem> ienuList = newlistmer;
            var result = ienuList.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PrizeWinner_Read([DataSourceRequest] DataSourceRequest request, int PrizeId)
        {
            List<com.kuazoo.TransactionDetail> listmer = transactionservice.GetTransactionDetailByPrize(PrizeId).Result;
            List<TransactionModel.TransactionUserPlay> newlistmer = new List<TransactionModel.TransactionUserPlay>();
            foreach (var v in listmer)
            {
                newlistmer.Add(new TransactionModel.TransactionUserPlay() { TransactionId = v.TransactionId, InventoryItemId = v.InventoryItemId, InventoryItemName = v.InventoryItemName,TransactionDate = v.TransactionDate.AddHours(Helper.defaultGMT), MemberId = v.MemberId, MemberEmail = v.MemberEmail, MemberPlay = v.MemberPlay, PaymentStatus= v.PaymentStatus });
            }
            IEnumerable<TransactionModel.TransactionUserPlay> ienuList = newlistmer;
            var result = ienuList.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Prize_Destroy([DataSourceRequest] DataSourceRequest request, InventoryItemModel.Prize Prize)
        {
            if (Prize != null)
            {
                Boolean result = prizeservice.DeletePrize(Prize.PrizeId).Result;

                if (result) ViewBag.result = "1";
                else ViewBag.result = "2";
                ModelState.AddModelError("", ViewBag.result);

            }

            return Json(ModelState.ToDataSourceResult());
        }
        public ActionResult PrizeGame51Winner_Read([DataSourceRequest] DataSourceRequest request, int PrizeId)
        {
            List<com.kuazoo.TransactionDetail> listmer = transactionservice.GetTransactionDetailByPrizeGame51Win(PrizeId).Result;
            List<TransactionModel.TransactionUserPlay> newlistmer = new List<TransactionModel.TransactionUserPlay>();
            foreach (var v in listmer)
            {
                newlistmer.Add(new TransactionModel.TransactionUserPlay() { No = v.No, TransactionId = v.TransactionId, InventoryItemId = v.InventoryItemId, InventoryItemName = v.InventoryItemName, TransactionDate = v.TransactionDate.AddHours(Helper.defaultGMT), MemberId = v.MemberId, MemberEmail = v.MemberEmail, MemberPlay=v.MemberPlay});
       
            }
            IEnumerable<TransactionModel.TransactionUserPlay> ienuList = newlistmer;
            var result = ienuList.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PrizeAdd()
        {
            return View();
        }
        [HttpPost]
        public ActionResult PrizeAdd(InventoryItemModel.Prize Prize)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    com.kuazoo.Prize pro = new com.kuazoo.Prize();
                    pro.PrizeId = 0;
                    pro.Name = Prize.Name;
                    pro.SponsorName = Prize.SponsorName;
                    pro.Price = Prize.Price;
                    pro.Description = "";
                    if (Prize.Description != null)
                    {
                        string desc = Microsoft.JScript.GlobalObject.decodeURI(Prize.Description);
                        pro.Description = desc;
                    }
                    pro.Terms = Prize.Terms;
                    pro.Detail = Prize.Detail;
                    pro.ExpiryDate = Prize.ExpiryDate.AddHours(-1*Helper.defaultGMT);
                    pro.PublishDate = Prize.PublishDate.AddHours(-1*Helper.defaultGMT);
                    pro.GroupName = Prize.GroupName;
                    pro.TotalRevenue = Prize.TotalRevenue;
                    pro.FakeVisualMeter = Prize.FakeVisualMeter;
                    pro.FreeDeal = Prize.FreeDeal;
                    pro.ImageId = 0;
                    if (Prize.ImageId != 0)
                    {
                        pro.ImageId = Prize.ImageId;
                    }
                    if (Prize.SubImageId != null && Prize.SubImageId != "")
                    {
                        List<int> subimg = new List<int>();
                        String[] subimagelist = Prize.SubImageId.Split(',');
                        foreach (var v in subimagelist)
                        {
                            if (v != "")
                            {
                                subimg.Add(int.Parse(v));
                            }
                        }
                        pro.SubImageId = subimg;
                    }

                    bool result = prizeservice.CreatePrize(pro).Result;
                    if (result == true)
                    {
                        ViewBag.result = "1";
                    }
                    else
                    {
                        ViewBag.result = "3";
                    }

                    TempData["msg"] = ViewBag.result;
                    return RedirectToAction("Prize", "Admin");
                }
                catch (Exception ex)
                {
                    CustomException(ex);
                    ViewBag.result = ex.Message;
                }
            }
            return View(Prize);

        }
        public ActionResult PrizeEdit(int PrizeId)
        {
            com.kuazoo.Prize v = prizeservice.GetPrizeById(PrizeId).Result;
            InventoryItemModel.Prize InventoryItem = new InventoryItemModel.Prize() { FreeDeal =v.FreeDeal, PrizeId = v.PrizeId, Name = v.Name, SponsorName = v.SponsorName, Price = v.Price, Description = v.Description, Terms = v.Terms, Detail = v.Detail, ExpiryDate = v.ExpiryDate.AddHours(Helper.defaultGMT), PublishDate = v.PublishDate.AddHours(Helper.defaultGMT), GroupName = v.GroupName, TotalRevenue = v.TotalRevenue, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = String.Join(",", v.SubImageId), SubImageName = String.Join("|", v.SubImageName), SubImageUrl = String.Join("|", v.SubImageUrl), LastAction = v.LastAction, FakeVisualMeter = v.FakeVisualMeter };
            return View(InventoryItem);
        }
        [HttpPost]
        public ActionResult PrizeEdit(InventoryItemModel.Prize Prize)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    com.kuazoo.Prize pro = new com.kuazoo.Prize();
                    pro.PrizeId = Prize.PrizeId;
                    pro.Name = Prize.Name;
                    pro.SponsorName = Prize.SponsorName;
                    pro.Price = Prize.Price;
                    pro.Description = "";
                    if (Prize.Description != null)
                    {
                        string desc = Microsoft.JScript.GlobalObject.decodeURI(Prize.Description);
                        pro.Description = desc;
                    }
                    pro.Terms = Prize.Terms;
                    pro.Detail = Prize.Detail;
                    pro.ExpiryDate = Prize.ExpiryDate.AddHours(-1*Helper.defaultGMT);
                    pro.PublishDate = Prize.PublishDate.AddHours(-1*Helper.defaultGMT);
                    pro.GroupName = Prize.GroupName;
                    pro.TotalRevenue = Prize.TotalRevenue;
                    pro.FakeVisualMeter = Prize.FakeVisualMeter;
                    pro.FreeDeal = Prize.FreeDeal;
                    pro.ImageId = 0;
                    if (Prize.ImageId != 0)
                    {
                        pro.ImageId = Prize.ImageId;
                    }
                    if (Prize.SubImageId != null && Prize.SubImageId != "")
                    {
                        List<int> subimg = new List<int>();
                        String[] subimagelist = Prize.SubImageId.Split(',');
                        foreach (var v in subimagelist)
                        {
                            if (v != "")
                            {
                                subimg.Add(int.Parse(v));
                            }
                        }
                        pro.SubImageId = subimg;
                    }
                    bool result = prizeservice.CreatePrize(pro).Result;
                    if (result == true)
                    {
                        ViewBag.result = "1";
                        TempData["msg"] = "11";
                    }
                    else
                    {
                        ViewBag.result = "3";
                        TempData["msg"] = "33";
                    }
                    return RedirectToAction("Prize", "Admin");
                }
                catch (Exception ex)
                {
                    CustomException(ex);
                    ViewBag.result = ex.Message;
                }
            }
            return View(Prize);

        }
        public ActionResult PrizeView(int PrizeId)
        {
            if (TempData["msg"] != null)
            {
                ViewBag.msg = TempData["msg"];
            }
            com.kuazoo.Prize v = prizeservice.GetPrizeById(PrizeId).Result;
            InventoryItemModel.Prize InventoryItem = new InventoryItemModel.Prize() { GameType = v.GameType, PrizeId = v.PrizeId, Name = v.Name, SponsorName = v.SponsorName, Price = v.Price, Description = v.Description, Terms = v.Terms, Detail = v.Detail, ExpiryDate = v.ExpiryDate.AddHours(Helper.defaultGMT), PublishDate = v.PublishDate.AddHours(Helper.defaultGMT), GroupName = v.GroupName, TotalRevenue = v.TotalRevenue, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = String.Join(",", v.SubImageId), SubImageName = String.Join("|", v.SubImageName), SubImageUrl = String.Join("|", v.SubImageUrl), LastAction = v.LastAction, WinnerEmail = v.WinnerEmail, ClosestWinnerEmail = v.ClosestWinnerEmail, FakeVisualMeter = v.FakeVisualMeter };
            return View(InventoryItem);
        }
        public ActionResult PrizeSetGame(int PrizeId, int Type)
        {
            com.kuazoo.Prize v = new com.kuazoo.Prize();
            v.PrizeId = PrizeId;
            v.GameType = Type;
            try
            {
                var result = prizeservice.UpdatePrizeGameType(v).Result;
                if (result)
                {
                    return new RedirectResult(Url.Action("PrizeView", "Admin", new { PrizeId = PrizeId }) + "#game");
                }
                else
                {
                    return RedirectToAction("Prize", "Admin");
                }
            }
            catch(Exception ex){
                TempData["msg"] = "Prize Not Found";
                return RedirectToAction("Prize", "Admin");
            }
        }
        [HttpPost]
        public JsonResult PrizeWinner(int prizeid, int transid, int invid)
        {
            try
            {
                var result = prizeservice.CreateWinner(prizeid, transid,invid).Result;
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

        #endregion



        #region ArchivedPrize
        public ActionResult ArchivedPrize()
        {
            if (TempData["msg"] != null)
            {
                ViewBag.msg = TempData["msg"];
            }
            return View();
        }
        public ActionResult ArchivedPrize_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<com.kuazoo.Prize> listmer = prizeservice.GetArchivedPrizeList().Result;
            List<InventoryItemModel.Prize> newlistmer = new List<InventoryItemModel.Prize>();
            foreach (var v in listmer)
            {
                newlistmer.Add(new InventoryItemModel.Prize() { Create = v.Create.AddHours(Helper.defaultGMT), PrizeId = v.PrizeId, Name = v.Name, SponsorName = v.SponsorName, Price = v.Price, ExpiryDate = v.ExpiryDate.AddHours(Helper.defaultGMT), PublishDate = v.PublishDate.AddHours(Helper.defaultGMT), GroupName = v.GroupName, TotalRevenue = v.TotalRevenue, LastAction = v.LastAction, WinnerEmail = v.WinnerEmail });
            }
            IEnumerable<InventoryItemModel.Prize> ienuList = newlistmer;
            var result = ienuList.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ArchivedPrizeView(int PrizeId)
        {
            com.kuazoo.Prize v = prizeservice.GetArchivedPrizeById(PrizeId).Result;
            InventoryItemModel.Prize InventoryItem = new InventoryItemModel.Prize() { GameType = v.GameType, PrizeId = v.PrizeId, Name = v.Name, SponsorName = v.SponsorName, Price = v.Price, Description = v.Description, Terms = v.Terms, Detail = v.Detail, ExpiryDate = v.ExpiryDate.AddHours(Helper.defaultGMT), PublishDate = v.PublishDate.AddHours(Helper.defaultGMT), GroupName = v.GroupName, TotalRevenue = v.TotalRevenue, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl, SubImageId = String.Join(",", v.SubImageId), SubImageName = String.Join("|", v.SubImageName), SubImageUrl = String.Join("|", v.SubImageUrl), LastAction = v.LastAction, WinnerEmail = v.WinnerEmail, ClosestWinnerEmail = v.ClosestWinnerEmail };
            return View(InventoryItem);
        }

        #endregion

        #region Game
        public ActionResult Game_Read([DataSourceRequest] DataSourceRequest request, int PrizeId=0)
        {
            List<com.kuazoo.Game> listmer = gameservice.GetGameList(PrizeId).Result;
            List<GameModel> newlistmer = new List<GameModel>();
            foreach (var v in listmer)
            {
                newlistmer.Add(new GameModel() { Create = v.Create.AddHours(Helper.defaultGMT), GameId = v.GameId, Name = v.Name, Description = v.Description,Instruction = v.Instruction , ExpiryDate = v.ExpiryDate.AddHours(Helper.defaultGMT), Latitude = v.Latitude,Longitude = v.Longitude, PrizeId = v.PrizeId,PrizeName = v.PrizeName, LastAction = v.LastAction, ImageId = v.ImageId, ImageName = v.ImageName,ImageUrl = v.ImageUrl });
            }
            IEnumerable<GameModel> ienuList = newlistmer;
            var result = ienuList.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Game_Destroy([DataSourceRequest] DataSourceRequest request, GameModel Game)
        {
            if (Game != null)
            {
                Boolean result = gameservice.DeleteGame(Game.GameId).Result;

                if (result) ViewBag.result = "1g";
                else ViewBag.result = "2g";
                ModelState.AddModelError("", ViewBag.result);

            }

            return Json(ModelState.ToDataSourceResult());
        }

        public ActionResult GameAdd(int PrizeId=0)
        {
            if (PrizeId == 0)
            {
                return RedirectToAction("Prize", "Admin");
            }
            ViewBag.PrizeId = PrizeId;
            var prize = prizeservice.GetPrizeById(PrizeId).Result;
            ViewBag.PrizeExpireDate = prize.ExpiryDate;
            return View();
        }
        [HttpPost]
        public ActionResult GameAdd(GameModel Game, int PrizeId = 0)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    com.kuazoo.Game pro = new com.kuazoo.Game();
                    pro.GameId = 0;
                    pro.Name = Game.Name;
                    pro.PrizeId = Game.PrizeId;

                    if (Game.Description != null)
                    {
                        string desc = Microsoft.JScript.GlobalObject.decodeURI(Game.Description);
                        pro.Description = desc;
                    }
                    if (Game.Instruction != null)
                    {
                        string desc = Microsoft.JScript.GlobalObject.decodeURI(Game.Instruction);
                        pro.Instruction = desc;
                    }
                    pro.ExpiryDate = Game.ExpiryDate.AddHours(-1 * Helper.defaultGMT);
                    pro.Latitude = Game.Latitude;
                    pro.Longitude = Game.Longitude;
                    pro.ImageId = 0;
                    if (Game.ImageId != 0)
                    {
                        pro.ImageId = Game.ImageId;
                    }

                    bool result = gameservice.CreateGame(pro).Result;
                    if (result == true)
                    {
                        ViewBag.result = "1g";
                    }
                    else
                    {
                        ViewBag.result = "3g";
                    }

                    TempData["msg"] = ViewBag.result;
                    return new RedirectResult(Url.Action("PrizeView", "Admin", new { PrizeId = PrizeId }) + "#game");
                }
                catch (Exception ex)
                {
                    CustomException(ex);
                    ViewBag.result = ex.Message;
                    ViewBag.PrizeId = PrizeId;
                    var prize = prizeservice.GetPrizeById(PrizeId).Result;
                    ViewBag.PrizeExpireDate = prize.ExpiryDate;
                }
            }
            else
            {
                if (PrizeId == 0)
                {
                    return RedirectToAction("Prize", "Admin");
                }
                ViewBag.PrizeId = PrizeId;
                var prize = prizeservice.GetPrizeById(PrizeId).Result;
                ViewBag.PrizeExpireDate = prize.ExpiryDate;
            }
            return View(Game);

        }
        public ActionResult GameEdit(int GameId, int PrizeId = 0)
        {
            if (PrizeId == 0)
            {
                return RedirectToAction("Prize", "Admin");
            }
            ViewBag.PrizeId = PrizeId;
            com.kuazoo.Game v = gameservice.GetGameById(GameId).Result;
            GameModel InventoryItem = new GameModel() { GameId = v.GameId, Name = v.Name, Description = v.Description, Instruction = v.Instruction, ExpiryDate = v.ExpiryDate.AddHours(Helper.defaultGMT), Latitude = v.Latitude, Longitude = v.Longitude, PrizeId = v.PrizeId, PrizeName = v.PrizeName, LastAction = v.LastAction, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl };
            return View(InventoryItem);
        }
        [HttpPost]
        public ActionResult GameEdit(GameModel Game, int PrizeId = 0)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    com.kuazoo.Game pro = new com.kuazoo.Game();
                    pro.GameId = Game.GameId;
                    pro.Name = Game.Name;
                    pro.PrizeId = Game.PrizeId;
                    if (Game.Description != null)
                    {
                        string desc = Microsoft.JScript.GlobalObject.decodeURI(Game.Description);
                        pro.Description = desc;
                    }
                    if (Game.Instruction != null)
                    {
                        string desc = Microsoft.JScript.GlobalObject.decodeURI(Game.Instruction);
                        pro.Instruction = desc;
                    }
                    pro.ExpiryDate = Game.ExpiryDate.AddHours(-1 * Helper.defaultGMT);
                    pro.Latitude = Game.Latitude;
                    pro.Longitude = Game.Longitude;
                    pro.ImageId = 0;
                    if (Game.ImageId != 0)
                    {
                        pro.ImageId = Game.ImageId;
                    }
                    bool result = gameservice.CreateGame(pro).Result;
                    if (result == true)
                    {
                        ViewBag.result = "1";
                        TempData["msg"] = "11g";
                    }
                    else
                    {
                        ViewBag.result = "3";
                        TempData["msg"] = "33g";
                    }
                    return new RedirectResult(Url.Action("PrizeView", "Admin", new { PrizeId = PrizeId }) + "#game");
                }
                catch (Exception ex)
                {
                    CustomException(ex);
                    ViewBag.result = ex.Message;
                }
            }
            return View(Game);

        }
        public ActionResult GameView(int GameId, int PrizeId=0)
        {
            if (PrizeId == 0)
            {
                return RedirectToAction("Prize", "Admin");
            }
            ViewBag.PrizeId = PrizeId;
            if (TempData["msg"] != null)
            {
                ViewBag.msg = TempData["msg"];
            }
            com.kuazoo.Game v = gameservice.GetGameById(GameId).Result;
            GameModel InventoryItem = new GameModel() { GameId = v.GameId, Name = v.Name, Description = v.Description, Instruction = v.Instruction, ExpiryDate = v.ExpiryDate.AddHours(Helper.defaultGMT), Latitude = v.Latitude, Longitude = v.Longitude, PrizeId = v.PrizeId, PrizeName = v.PrizeName, LastAction = v.LastAction, ImageId = v.ImageId, ImageName = v.ImageName, ImageUrl = v.ImageUrl };
            return View(InventoryItem);
        }

        #endregion


        #region image
        public ActionResult Image()
        {
            if (TempData["msg"] != null)
            {
                ViewBag.msg = TempData["msg"];
            }
            return View();
        }
        public ActionResult Image_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<com.kuazoo.Image> listmer = imageservice.GetImageList().Result;
            List<InventoryItemModel.Image> newlistmer = new List<InventoryItemModel.Image>();
            foreach (var v in listmer)
            {
                newlistmer.Add(new InventoryItemModel.Image() { ImageId = v.ImageId, Name = v.Name, Url = v.Url, LastAction = v.LastAction });
            }
            IEnumerable<InventoryItemModel.Image> ienuList = newlistmer;
            var result = ienuList.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Image_Destroy([DataSourceRequest] DataSourceRequest request, InventoryItemModel.Image Image)
        {
            if (Image != null)
            {
                Boolean result = imageservice.DeleteImage(Image.ImageId).Result;

                if (result) ViewBag.result = "1";
                else ViewBag.result = "2";
                ModelState.AddModelError("", ViewBag.result);

            }

            return Json(ModelState.ToDataSourceResult());
        }

        public ActionResult ImageAdd()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ImageAdd(HttpPostedFileBase imageUpload, InventoryItemModel.Image Image)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string imageurl = "";
                    if (imageUpload != null)
                    {
                        if (!Helper.IsValidImage(imageUpload.FileName))
                        {
                            throw new Exception("File is not an image file");
                        }
                        string name = DateTime.UtcNow.ToString("yyyyMMddhhmmss");
                        imageurl = Helper.uploadImageWithName(imageUpload, name);
                    }
                    com.kuazoo.Image pro = new com.kuazoo.Image();
                    pro.ImageId = 0;
                    pro.Name = imageurl;

                    bool result = imageservice.CreateImage(pro).Result;
                    if (result == true)
                    {
                        ViewBag.result = "1";
                    }
                    else
                    {
                        ViewBag.result = "3";
                    }

                    TempData["msg"] = ViewBag.result;
                    return RedirectToAction("Image", "Admin");
                }
                catch (Exception ex)
                {
                    CustomException(ex);
                    ViewBag.result = ex.Message;
                }
            }
            return View(Image);

        }
        public ActionResult ImageEdit(int ImageId)
        {
            com.kuazoo.Image v = imageservice.GetImageById(ImageId).Result;
            InventoryItemModel.Image InventoryItem = new InventoryItemModel.Image() { ImageId = v.ImageId, Name=v.Name, Url = v.Url, LastAction = v.LastAction };
            return View(InventoryItem);
        }
        [HttpPost]
        public ActionResult ImageEdit(HttpPostedFileBase imageUpload, InventoryItemModel.Image Image)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string imageurl = "";
                    if (imageUpload != null)
                    {
                        if (!Helper.IsValidImage(imageUpload.FileName))
                        {
                            throw new Exception("File is not an image file");
                        }
                        string name = DateTime.UtcNow.ToString("yyyyMMddhhmmss");
                        imageurl = Helper.uploadImageWithName(imageUpload, name);
                    }
                    
                    com.kuazoo.Image pro = new com.kuazoo.Image();
                    pro.ImageId = Image.ImageId;
                    if (imageurl == "") pro.Url = Image.Url;
                    else pro.Name = imageurl;

                    bool result = imageservice.CreateImage(pro).Result;
                    if (result == true)
                    {
                        ViewBag.result = "1";
                        TempData["msg"] = "11";
                    }
                    else
                    {
                        ViewBag.result = "3";
                        TempData["msg"] = "33";
                    }
                    return RedirectToAction("Image", "Admin");
                }
                catch (Exception ex)
                {
                    CustomException(ex);
                    ViewBag.result = ex.Message;
                }
            }
            return View(Image);

        }
        public ActionResult ImageView(int ImageId)
        {
            com.kuazoo.Image v = imageservice.GetImageById(ImageId).Result;
            InventoryItemModel.Image InventoryItem = new InventoryItemModel.Image() { ImageId = v.ImageId, Name = v.Name, Url = v.Url, LastAction = v.LastAction };
            return View(InventoryItem);
        }


        public ActionResult ImageList()
        {
            //List<com.kuazoo.Image> listmer = imageservice.GetImageListDesc().Result;
            //List<InventoryItemModel.Image> newlistmer = new List<InventoryItemModel.Image>();
            //foreach (var v in listmer)
            //{
            //    newlistmer.Add(new InventoryItemModel.Image() { ImageId = v.ImageId, Name = v.Name, Url = v.Url, LastAction = v.LastAction });
            //}
            //InventoryItemModel.ImageList inventorymodel = new InventoryItemModel.ImageList();
            //inventorymodel.List = newlistmer;
            //return View(inventorymodel);
            return View();
        }
        public ActionResult ImageListMulti()
        {
            //List<com.kuazoo.Image> listmer = imageservice.GetImageListDesc().Result;
            //List<InventoryItemModel.Image> newlistmer = new List<InventoryItemModel.Image>();
            //foreach (var v in listmer)
            //{
            //    newlistmer.Add(new InventoryItemModel.Image() { ImageId = v.ImageId, Name = v.Name, Url = v.Url, LastAction = v.LastAction });
            //}
            //InventoryItemModel.ImageList inventorymodel = new InventoryItemModel.ImageList();
            //inventorymodel.List = newlistmer;
            //return View(inventorymodel);
            return View();
        }
        public JsonResult ImageListData(int skip, int take)
        {
            List<com.kuazoo.Image> listmer = imageservice.GetImageListDescBySkipTake(skip,take).Result;
            List<InventoryItemModel.Image> newlistmer = new List<InventoryItemModel.Image>();
            foreach (var v in listmer)
            {
                newlistmer.Add(new InventoryItemModel.Image() { ImageId = v.ImageId, Name = v.Name, Url = v.Url, LastAction = v.LastAction });
            }
            IEnumerable<InventoryItemModel.Image> ienuList = newlistmer;
            return Json(ienuList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ImageListDataDraft(string imageid)
        {
            List<com.kuazoo.Image> listmer = imageservice.GetImageListDescByImageId(imageid).Result;
            List<InventoryItemModel.Image> newlistmer = new List<InventoryItemModel.Image>();
            foreach (var v in listmer)
            {
                newlistmer.Add(new InventoryItemModel.Image() { ImageId = v.ImageId, Name = v.Name, Url = v.Url, LastAction = v.LastAction });
            }
            IEnumerable<InventoryItemModel.Image> ienuList = newlistmer;
            return Json(ienuList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ImageListUploadImageMulti(HttpPostedFileBase imageUpload)
        {
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
                    imagename = Helper.uploadImageWithName(imageUpload, name);
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

        [HttpPost]
        public virtual ActionResult ImageListUploadImage()//name and strsubimage come from post jquery
        {
            HttpPostedFileBase imageUpload = Request.Files["imagefile"];
            bool isUpload = false;
            string message = "";
            string imageurl = "";
            string imagename = "";
            int result=0;
            if (imageUpload != null)
            {
                try
                {
                    if (!Helper.IsValidImage(imageUpload.FileName))
                    {
                        message = "File is not an image file";
                    }
                    string name = DateTime.UtcNow.ToString("yyyyMMddhhmmss");
                    imagename = Helper.uploadImageWithName(imageUpload, name);
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
        #endregion


        #region ExtractData

        public ActionResult ExtractData()
        {
            if (TempData["msg"] != null)
            {
                ViewBag.msg = TempData["msg"];
            }
            return View();
        }
        [HttpPost]
        public ActionResult ExtractData(TransactionModel.ExtractData ExtractData)
        {
            try
            {
                if (ExtractData.ReportType == 0)
                {
                    List<com.kuazoo.Member> listmer = memberservice.GetMemberListIdAge(ExtractData.Member.Status, ExtractData.Member.AgeStart, ExtractData.Member.AgeEnd).Result;
                    List<MemberModel.MemberExportData> newlistmer = new List<MemberModel.MemberExportData>();
                    string gender = "";
                    foreach (var v in listmer)
                    {
                        if (v.Gender == 0) gender = "Male";
                        else if (v.Gender == 1) gender = "Female";
                        newlistmer.Add(new MemberModel.MemberExportData() { MemberId = v.MemberId, FirstName = v.FirstName, LastName = v.LastName, Email = v.Email, Gender = gender, DateOfBirth = v.DateOfBirth, MemberStatus = v.MemberStatus.MemberStatusName });
                    }
                    if (newlistmer.Count() > 0)
                    {
                        Response.ClearContent();
                        Response.Buffer = true;
                        Response.AddHeader("content-disposition", "attachment; filename=MemberData.xls");
                        Response.ContentType = "application/ms-excel";
                        Response.Charset = "";
                        StringWriter sw = new StringWriter();
                        HtmlTextWriter htw = new HtmlTextWriter(sw);
                        System.Web.UI.WebControls.GridView gv = new System.Web.UI.WebControls.GridView();
                        gv.DataSource = newlistmer.ToList();
                        gv.DataBind();
                        gv.RenderControl(htw);
                        Response.Output.Write(sw.ToString());
                        Response.Flush();
                        Response.End();
                        ViewBag.msg = "1";
                        TempData["msg"] = ViewBag.msg;
                        return RedirectToAction("ExtractData");
                    }
                    else
                    {
                        ViewBag.msg = "2";
                    }
                }
                else if (ExtractData.ReportType == 1)
                {
                    List<com.kuazoo.InventoryItem> listmer = inventoryitemservice.GetInventoryItemListStatusTypeCityDate(ExtractData.InventoryItem.InventoryItemName, ExtractData.InventoryItem.PrizeName, ExtractData.InventoryItem.Status, ExtractData.InventoryItem.InventoryItemTypeId, ExtractData.InventoryItem.CityId, ExtractData.InventoryItem.PublishedStart.AddHours(-1 * Helper.defaultGMT), ExtractData.InventoryItem.PublishedEnd.AddHours(-1*Helper.defaultGMT)).Result;
                    List<InventoryItemModel.InventoryItemExportData> newlistmer = new List<InventoryItemModel.InventoryItemExportData>();
                    foreach (var v in listmer)
                    {
                        string status = "Active";
                        if (v.Flag == false) status = "Inactive";
                        if (v.Prize != null)
                        {
                            newlistmer.Add(new InventoryItemModel.InventoryItemExportData() { InventoryItemId = v.InventoryItemId, InventoryItemName = v.Name, ShortDesc = v.ShortDesc, Price = v.Price, GeneralDescription = v.GeneralDescription, Description = v.Description, MerchantId = v.Merchant.MerchantId, MerchantName = v.Merchant.Name, CityName = v.Merchant.City.Name, CountryName = v.Merchant.Country.Name, Keyword = v.Keyword, InventoryItemType = v.InventoryItemType.InventoryItemTypeName, Discount = v.Discount, ExpireDate = v.ExpireDate.AddHours(Helper.defaultGMT), MaximumSales = v.MaximumSales, PublishDate = v.PublishDate.AddHours(Helper.defaultGMT), MinimumTarget = v.MinimumTarget, Tag = v.TagName, Status = status, PrizeId = v.Prize.PrizeId, PrizeName = v.Prize.Name, PrizeDescription = v.Prize.Description, PrizePrice = v.Prize.Price, SponsorName = v.Prize.SponsorName, HowToRedeem = v.Prize.Detail, Terms = v.Prize.Terms });
                        }
                        else
                        {
                            newlistmer.Add(new InventoryItemModel.InventoryItemExportData() { InventoryItemId = v.InventoryItemId, InventoryItemName = v.Name, ShortDesc = v.ShortDesc, Price = v.Price, GeneralDescription = v.GeneralDescription, Description = v.Description, MerchantId = v.Merchant.MerchantId, MerchantName = v.Merchant.Name, CityName = v.Merchant.City.Name, CountryName = v.Merchant.Country.Name, Keyword = v.Keyword, InventoryItemType = v.InventoryItemType.InventoryItemTypeName, Discount = v.Discount, ExpireDate = v.ExpireDate.AddHours(Helper.defaultGMT), MaximumSales = v.MaximumSales, PublishDate = v.PublishDate.AddHours(Helper.defaultGMT), MinimumTarget = v.MinimumTarget, Tag = v.TagName, Status = status });
                        }
                    }
                    if (newlistmer.Count() > 0)
                    {
                        Response.ClearContent();
                        Response.Buffer = true;
                        Response.AddHeader("content-disposition", "attachment; filename=InventoryItemData.xls");
                        Response.ContentType = "application/ms-excel";
                        Response.Charset = "";
                        StringWriter sw = new StringWriter();
                        HtmlTextWriter htw = new HtmlTextWriter(sw);
                        System.Web.UI.WebControls.GridView gv = new System.Web.UI.WebControls.GridView();
                        gv.DataSource = newlistmer.ToList();
                        gv.DataBind();
                        gv.RenderControl(htw);
                        Response.Output.Write(sw.ToString());
                        Response.Flush();
                        Response.End();
                        ViewBag.msg = "1";
                        TempData["msg"] = ViewBag.msg;
                        return RedirectToAction("ExtractData");
                    }
                    else
                    {
                        ViewBag.msg = "2";
                    }
                }
                else if (ExtractData.ReportType == 2)
                {
                    List<com.kuazoo.TransactionDetail2> listmer = transactionservice.GetTransactionDetailIdDate2(ExtractData.Transaction.InventoryItem, ExtractData.Transaction.TransactionStart.AddHours(-1 * Helper.defaultGMT), ExtractData.Transaction.TransactionEnd.AddHours(-1 * Helper.defaultGMT)).Result;
                    List<TransactionModel.TransactionDetailExportData2> newlistmer = new List<TransactionModel.TransactionDetailExportData2>();
                    foreach (var v in listmer)
                    {
                        //if (v.FlashDeal != null)
                        //{
                        //    string status = "Active";
                        //    if (v.FlashDeal.Flag == false) status = "Inactive";
                        //    newlistmer.Add(new TransactionModel.TransactionDetailExportData2() { TransactionId = v.TransactionId, InventoryItemName = v.InventoryItemName, MerchantName = v.MerchantName, TransactionDate = v.TransactionDate.AddHours(Helper.defaultGMT), MemberEmail = v.MemberEmail, FlashDealId = v.FlashDealId, FlashDealDiscount = v.FlashDeal.Discount, FlashDealStatus = status, FlashDealStartTime = v.FlashDeal.StartTime.AddHours(Helper.defaultGMT), FlashDealEndTime = v.FlashDeal.EndTime.AddHours(Helper.defaultGMT) });
                        //}
                        //else
                        //{
                            newlistmer.Add(new TransactionModel.TransactionDetailExportData2() { TransactionId = v.TransactionId, OrderNo = v.OrderNo,TransactionDate=v.TransactionDate.AddHours(Helper.defaultGMT), MerchantName = v.MerchantName, InventoryItemName =v.InventoryItemName,KuazooSKU =v.SKU,Qty= v.Qty,MemberFirstName = v.MemberFirstName,MemberLastName =v.MemberLastName,MemberEmail = v.MemberEmail,Phone = v.Phone,AddressLine1 =v.AddressLine1,AddressLine2=v.AddressLine2,City =v.City,State=v.State,Country=v.Country,ZipCode=v.ZipCode  });
                        //}
                    }
                    if (newlistmer.Count() > 0)
                    {
                        Response.ClearContent();
                        Response.Buffer = true;
                        Response.AddHeader("content-disposition", "attachment; filename=TransactionData.xls");
                        Response.ContentType = "application/ms-excel";
                        Response.Charset = "";
                        StringWriter sw = new StringWriter();
                        HtmlTextWriter htw = new HtmlTextWriter(sw);
                        System.Web.UI.WebControls.GridView gv = new System.Web.UI.WebControls.GridView();
                        gv.DataSource = newlistmer.ToList();
                        gv.DataBind();
                        gv.RenderControl(htw);
                        Response.Output.Write(sw.ToString());
                        Response.Flush();
                        Response.End();
                        ViewBag.msg = "1";
                        TempData["msg"] = ViewBag.msg;
                        return RedirectToAction("ExtractData");
                    }
                    else
                    {
                        ViewBag.msg = "2";
                    }
                }
                return View(ExtractData);
            }
            catch (Exception ex)
            {
                CustomException(ex);
                ViewBag.msg = "3";
                return View(ExtractData);
            }
        }
        #endregion


        #region KPointSetting
        public ActionResult KPointSetting()
        {
            if (TempData["msg"] != null)
            {
                ViewBag.msg = TempData["msg"];
            }
            return View();
        }
        public ActionResult KPointSetting_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<com.kuazoo.PointAction> listmer = pointservice.GetListPointAction().Result;
            List<PointActionModel> newlistmer = new List<PointActionModel>();
            foreach (var v in listmer)
            {
                newlistmer.Add(new PointActionModel() { PointActionId = v.PointActionId, Code = v.Code, Amount = v.Amount, Description = v.Desc });
            }
            IEnumerable<PointActionModel> ienuList = newlistmer;
            var result = ienuList.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult KPointSettingEdit(int PointActionId)
        {
            com.kuazoo.PointAction v = pointservice.GetPointActionById(PointActionId).Result;
            PointActionModel point = new PointActionModel() { PointActionId = v.PointActionId, Code = v.Code, Amount = v.Amount, Description = v.Desc };
            return View(point);
        }
        [HttpPost]
        public ActionResult KPointSettingEdit(PointActionModel PointAction)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    com.kuazoo.PointAction pro = new com.kuazoo.PointAction();
                    pro.PointActionId = PointAction.PointActionId;
                    pro.Code = PointAction.Code;
                    pro.Desc = PointAction.Description;
                    pro.Amount = PointAction.Amount;

                    bool result = pointservice.UpdatePointAction(pro).Result;
                    if (result == true)
                    {
                        ViewBag.result = "1";
                        TempData["msg"] = "11";
                    }
                    else
                    {
                        ViewBag.result = "3";
                        TempData["msg"] = "33";
                    }
                    return RedirectToAction("KPointSetting", "Admin");
                }
                catch (Exception ex)
                {
                    CustomException(ex);
                    ViewBag.result = ex.Message;
                }
            }
            return View(PointAction);

        }
        public ActionResult KPointSettingView(int PointActionId)
        {
            com.kuazoo.PointAction v = pointservice.GetPointActionById(PointActionId).Result;
            PointActionModel point = new PointActionModel() { PointActionId = v.PointActionId, Code = v.Code, Amount = v.Amount, Description = v.Desc };
            return View(point);
        }
        #endregion

        #region Static
        public ActionResult Static()
        {
            if (TempData["msg"] != null)
            {
                ViewBag.msg = TempData["msg"];
            }
            return View();
        }
        public ActionResult Static_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<com.kuazoo.Static> listmer = generalservice.GetStaticList().Result;
            List<StaticModel> newlistmer = new List<StaticModel>();
            foreach (var v in listmer)
            {
                newlistmer.Add(new StaticModel() { StaticId = v.StaticId, Name = v.Name,  Description = v.Description });
            }
            IEnumerable<StaticModel> ienuList = newlistmer;
            var result = ienuList.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult StaticEdit(int StaticId)
        {
            com.kuazoo.Static v = generalservice.GetStaticById(StaticId).Result;
            StaticModel point = new StaticModel() { StaticId = v.StaticId, Name = v.Name, Description = v.Description };
            return View(point);
        }
        [HttpPost]
        public ActionResult StaticEdit(StaticModel Static)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    com.kuazoo.Static pro = new com.kuazoo.Static();
                    pro.StaticId = Static.StaticId;
                    pro.Name = Static.Name;
                    pro.Description = "";
                    if (Static.Description != null)
                    {
                        string desc = Microsoft.JScript.GlobalObject.decodeURI(Static.Description);
                        pro.Description = desc;
                    }

                    bool result = generalservice.UpdateStatic(pro).Result;
                    if (result == true)
                    {
                        ViewBag.result = "1";
                        TempData["msg"] = "11";
                    }
                    else
                    {
                        ViewBag.result = "3";
                        TempData["msg"] = "33";
                    }
                    return RedirectToAction("Static", "Admin");
                }
                catch (Exception ex)
                {
                    CustomException(ex);
                    ViewBag.result = ex.Message;
                }
            }
            return View(Static);

        }
        public ActionResult StaticView(int StaticId)
        {
            com.kuazoo.Static v = generalservice.GetStaticById(StaticId).Result;
            StaticModel point = new StaticModel() { StaticId = v.StaticId, Name = v.Name, Description = v.Description };
            return View(point);
        }
        #endregion

        public ActionResult Location()
        {
            if (TempData["msg"] != null)
            {
                ViewBag.msg = TempData["msg"];
            }
            return View();
        }
        #region Country
        public ActionResult Country()
        {
            return View();
        }
        public ActionResult Country_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<com.kuazoo.Country> listmer = countryservice.GetCountryList().Result;
            List<CountryModel> newlistmer = new List<CountryModel>();
            foreach (var v in listmer)
            {
                newlistmer.Add(new CountryModel() { CountryId = v.CountryId, CountryName = v.Name, LastAction = v.LastAction });
            }
            IEnumerable<CountryModel> ienuList = newlistmer;
            var result = ienuList.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Country_Destroy([DataSourceRequest] DataSourceRequest request, CountryModel Country)
        {
            if (Country != null)
            {
                Boolean result = countryservice.DeleteCountry(Country.CountryId).Result;

                if (result) ViewBag.result = "1";
                else ViewBag.result = "2";
                ModelState.AddModelError("", ViewBag.result);

            }

            return Json(ModelState.ToDataSourceResult());
        }

        public ActionResult CountryAdd()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CountryAdd(CountryModel Country)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    com.kuazoo.Country pro = new com.kuazoo.Country();
                    pro.CountryId = 0;
                    pro.Name = Country.CountryName;                   

                    bool result = countryservice.CreateCountry(pro).Result;
                    if (result == true)
                    {
                        ViewBag.result = "1";
                    }
                    else
                    {
                        ViewBag.result = "3";
                    }

                    TempData["msg"] = ViewBag.result;
                    return new RedirectResult(Url.Action("Location", "Admin") + "#country");
                }
                catch (Exception ex)
                {
                    CustomException(ex);
                    ViewBag.result = ex.Message;
                }
            }
            return View(Country);

        }
        public ActionResult CountryEdit(int CountryId)
        {
            com.kuazoo.Country v = countryservice.GetCountryById(CountryId).Result;
            CountryModel InventoryItem = new CountryModel() { CountryId = v.CountryId, CountryName = v.Name, LastAction = v.LastAction };
            return View(InventoryItem);
        }
        [HttpPost]
        public ActionResult CountryEdit(CountryModel Country)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    com.kuazoo.Country pro = new com.kuazoo.Country();
                    pro.CountryId = Country.CountryId;
                    pro.Name = Country.CountryName;
                    bool result = countryservice.CreateCountry(pro).Result;
                    if (result == true)
                    {
                        ViewBag.result = "1";
                        TempData["msg"] = "11";
                    }
                    else
                    {
                        ViewBag.result = "3";
                        TempData["msg"] = "33";
                    }
                    return new RedirectResult(Url.Action("Location", "Admin") + "#country");
                }
                catch (Exception ex)
                {
                    CustomException(ex);
                    ViewBag.result = ex.Message;
                }
            }
            return View(Country);

        }
        public ActionResult CountryView(int CountryId)
        {
            com.kuazoo.Country v = countryservice.GetCountryById(CountryId).Result;
            CountryModel InventoryItem = new CountryModel() { CountryId = v.CountryId, CountryName = v.Name,LastAction = v.LastAction };
            return View(InventoryItem);
        }
        #endregion
        #region City
        public ActionResult City()
        {
            return View();
        }
        public ActionResult City_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<com.kuazoo.City> listmer = countryservice.GetCityList().Result;
            List<CityModel> newlistmer = new List<CityModel>();
            foreach (var v in listmer)
            {
                newlistmer.Add(new CityModel() { CityId = v.CityId, CityName = v.Name, CountryId=v.CountryId,CountryName=v.CountryName, LastAction = v.LastAction });
            }
            IEnumerable<CityModel> ienuList = newlistmer;
            var result = ienuList.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult City_Destroy([DataSourceRequest] DataSourceRequest request, CityModel City)
        {
            if (City != null)
            {
                Boolean result = countryservice.DeleteCity(City.CityId).Result;

                if (result) ViewBag.result = "1";
                else ViewBag.result = "2";
                ModelState.AddModelError("", ViewBag.result);

            }

            return Json(ModelState.ToDataSourceResult());
        }

        public ActionResult CityAdd()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CityAdd(CityModel City)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    com.kuazoo.City pro = new com.kuazoo.City();
                    pro.CityId = 0;
                    pro.Name = City.CityName;
                    pro.CountryId = City.CountryId;
                    bool result = countryservice.CreateCity(pro).Result;
                    if (result == true)
                    {
                        ViewBag.result = "1";
                    }
                    else
                    {
                        ViewBag.result = "3";
                    }

                    TempData["msg"] = ViewBag.result;
                    return new RedirectResult(Url.Action("Location", "Admin") + "#city");
                }
                catch (Exception ex)
                {
                    CustomException(ex);
                    ViewBag.result = ex.Message;
                }
            }
            return View(City);

        }
        public ActionResult CityEdit(int CityId)
        {
            com.kuazoo.City v = countryservice.GetCityById(CityId).Result;
            CityModel InventoryItem = new CityModel() { CityId = v.CityId, CityName = v.Name, CountryId = v.CountryId, CountryName = v.CountryName, LastAction = v.LastAction };
            return View(InventoryItem);
        }
        [HttpPost]
        public ActionResult CityEdit(CityModel City)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    com.kuazoo.City pro = new com.kuazoo.City();
                    pro.CityId = City.CityId;
                    pro.Name = City.CityName;
                    pro.CountryId = City.CountryId;
                    bool result = countryservice.CreateCity(pro).Result;
                    if (result == true)
                    {
                        ViewBag.result = "1";
                        TempData["msg"] = "11";
                    }
                    else
                    {
                        ViewBag.result = "3";
                        TempData["msg"] = "33";
                    }
                    return new RedirectResult(Url.Action("Location", "Admin") + "#city");
                }
                catch (Exception ex)
                {
                    CustomException(ex);
                    ViewBag.result = ex.Message;
                }
            }
            return View(City);

        }
        public ActionResult CityView(int CityId)
        {
            com.kuazoo.City v = countryservice.GetCityById(CityId).Result;
            CityModel InventoryItem = new CityModel() { CityId = v.CityId, CityName = v.Name, CountryId = v.CountryId, CountryName = v.CountryName, LastAction = v.LastAction };
            return View(InventoryItem);
        }
        #endregion

        #region Admin
        public ActionResult Admin()
        {
            return View();
        }
        public ActionResult Admin_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<com.kuazoo.Admin> listmer = adminservice.GetAdminList().Result;
            List<AdminModel> newlistmer = new List<AdminModel>();
            foreach (var v in listmer)
            {
                newlistmer.Add(new AdminModel() { AdminId = v.AdminId, Email = v.Email,FirstName = v.FirstName,LastName =v.LastName, LastAction = v.LastAction });
            }
            IEnumerable<AdminModel> ienuList = newlistmer;
            var result = ienuList.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Admin_Destroy([DataSourceRequest] DataSourceRequest request, AdminModel Admin)
        {
            if (Admin != null)
            {
                Boolean result = adminservice.DeleteAdmin(Admin.AdminId).Result;

                if (result) ViewBag.result = "1";
                else ViewBag.result = "2";
                ModelState.AddModelError("", ViewBag.result);

            }

            return Json(ModelState.ToDataSourceResult());
        }

        public ActionResult AdminAdd()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AdminAdd(AdminModel Admin)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    com.kuazoo.Admin pro = new com.kuazoo.Admin();
                    pro.AdminId = 0;
                    pro.FirstName = Admin.FirstName;
                    pro.LastName = Admin.LastName;
                    pro.Email = Admin.Email;
                    pro.Password = Admin.Password;
                    bool result = adminservice.CreateAdmin(pro).Result;
                    if (result == true)
                    {
                        ViewBag.result = "1";
                    }
                    else
                    {
                        ViewBag.result = "3";
                    }

                    TempData["msg"] = ViewBag.result;
                    return RedirectToAction("Admin", "Admin");
                }
                catch (Exception ex)
                {
                    CustomException(ex);
                    ViewBag.result = ex.Message;
                }
            }
            return View(Admin);

        }
        public ActionResult AdminEdit(int AdminId)
        {
            com.kuazoo.Admin v = adminservice.GetAdminById(AdminId).Result;
            AdminModel2 InventoryItem = new AdminModel2() { AdminId = v.AdminId, Email = v.Email, FirstName = v.FirstName, LastName = v.LastName, LastAction = v.LastAction };
            return View(InventoryItem);
        }
        [HttpPost]
        public ActionResult AdminEdit(AdminModel2 Admin)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    com.kuazoo.Admin pro = new com.kuazoo.Admin();
                    pro.AdminId = Admin.AdminId;
                    pro.FirstName = Admin.FirstName;
                    pro.LastName = Admin.LastName;
                    pro.Email = Admin.Email;
                    bool result = adminservice.CreateAdmin(pro).Result;
                    if (result == true)
                    {
                        ViewBag.result = "1";
                        TempData["msg"] = "11";
                    }
                    else
                    {
                        ViewBag.result = "3";
                        TempData["msg"] = "33";
                    }
                    return RedirectToAction("Admin", "Admin");
                }
                catch (Exception ex)
                {
                    CustomException(ex);
                    ViewBag.result = ex.Message;
                }
            }
            return View(Admin);

        }
        public ActionResult AdminView(int AdminId)
        {
            com.kuazoo.Admin v = adminservice.GetAdminById(AdminId).Result;
            AdminModel InventoryItem = new AdminModel() { AdminId = v.AdminId, Email = v.Email, FirstName = v.FirstName, LastName = v.LastName, LastAction = v.LastAction };
            return View(InventoryItem);
        }
        #endregion

        #region subscribe

        public ActionResult Subscribe()
        {
            if (TempData["msg"] != null)
            {
                ViewBag.msg = TempData["msg"];
            }
            return View();
        }
        [HttpPost]
        public ActionResult Subscribe([DataSourceRequest] DataSourceRequest request)
        {

            List<com.kuazoo.Subscribe> listmer = memberservice.GetSubscribeList().Result;
            List<MemberModel.SubscribeList> newlistmer = new List<MemberModel.SubscribeList>();
            foreach (var v in listmer)
            {
                newlistmer.Add(new MemberModel.SubscribeList() { Email = v.Email, SubscribeDate = v.SubscribeDate.AddHours(Helper.defaultGMT) });
            }
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Transaction.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            System.Web.UI.WebControls.GridView gv = new System.Web.UI.WebControls.GridView();
            gv.DataSource = newlistmer.ToList();
            gv.DataBind();
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return RedirectToAction("Subscribe");
        }
        public ActionResult Subscribe_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<com.kuazoo.Subscribe> listmer = memberservice.GetSubscribeList().Result;
            List<MemberModel.SubscribeList> newlistmer = new List<MemberModel.SubscribeList>();
            foreach (var v in listmer)
            {
                newlistmer.Add(new MemberModel.SubscribeList() { Email = v.Email,SubscribeDate =v.SubscribeDate.AddHours(Helper.defaultGMT) });
            }
            IEnumerable<MemberModel.SubscribeList> ienuList = newlistmer;
            var result = ienuList.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        ///// <summary>
        ///// Create the Excel spreadsheet.
        ///// </summary>
        ///// <param name="model">Definition of the columns for the spreadsheet.</param>
        ///// <param name="data">Grid data.</param>
        ///// <param name="title">Title of the spreadsheet.</param>
        ///// <returns></returns>
        //public JsonResult ExportToExcel(string model, string data, string title)
        //{
        //    using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
        //    {
        //        try
        //        {
        //            /* Create the worksheet. */

        //            SpreadsheetDocument spreadsheet = Excel.CreateWorkbook(stream);
        //            Excel.AddBasicStyles(spreadsheet);
        //            Excel.AddAdditionalStyles(spreadsheet);
        //            Excel.AddWorksheet(spreadsheet, title);
        //            Worksheet worksheet = spreadsheet.WorkbookPart.WorksheetParts.First().Worksheet;


        //            /* Get the information needed for the worksheet */

        //            var modelObject = JsonConvert.DeserializeObject<dynamic>(model);
        //            var dataObject = JsonConvert.DeserializeObject<dynamic>(data);


        //            /* Add the column titles to the worksheet. */

        //            // For each column...
        //            for (int mdx = 0; mdx < modelObject.Count; mdx++)
        //            {
        //                // If the column has a title, use it.  Otherwise, use the field name.
        //                Excel.SetColumnHeadingValue(spreadsheet, worksheet, Convert.ToUInt32(mdx + 1),
        //                    modelObject[mdx].title == null ? modelObject[mdx].field.ToString() : modelObject[mdx].title.ToString(),
        //                    false, false);

        //                // Is there are column width defined?
        //                Excel.SetColumnWidth(worksheet, mdx + 1, modelObject[mdx].width != null
        //                    ? Convert.ToInt32(modelObject[mdx].width.ToString()) / 4
        //                    : 25);
        //            }


        //            /* Add the data to the worksheet. */

        //            // For each row of data...
        //            for (int idx = 0; idx < dataObject.Count; idx++)
        //            {
        //                // For each column...
        //                for (int mdx = 0; mdx < modelObject.Count; mdx++)
        //                {
        //                    try
        //                    {
        //                        // Set the field value in the spreadsheet for the current row and column.
        //                        Excel.SetCellValue(spreadsheet, worksheet, Convert.ToUInt32(mdx + 1), Convert.ToUInt32(idx + 2),
        //                            dataObject[idx][modelObject[mdx].field.ToString()].ToString(),
        //                            false, false);
        //                    }
        //                    catch
        //                    {
        //                    }
        //                }
        //            }


        //            /* Save the worksheet and store it in Session using the spreadsheet title. */

        //            worksheet.Save();
        //            spreadsheet.Close();
        //            byte[] file = stream.ToArray();
        //            Session[title] = file;
        //        }
        //        catch (Exception ex)
        //        {
        //            return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
        //        }
        //    }

        //    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        //}
        ///// <summary>
        ///// Download the spreadsheet.
        ///// </summary>
        ///// <param name="title">Title of the spreadsheet.</param>
        ///// <returns></returns>
        //public FileResult GetExcelFile(string title)
        //{
        //    // Is there a spreadsheet stored in session?
        //    if (Session[title] != null)
        //    {
        //        // Get the spreadsheet from seession.
        //        byte[] file = Session[title] as byte[];
        //        string filename = string.Format("{0}.xlsx", title);

        //        // Remove the spreadsheet from session.
        //        Session.Remove(title);

        //        // Return the spreadsheet.

        //        Response.Buffer = true;
        //        //Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}", filename));



        //        return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
        //    }
        //    else
        //    {
        //        throw new Exception(string.Format("{0} not found", title));
        //    }
        //}
        #endregion



        public void UpdateSalesVisualMeter(int id)
        {
            transactionservice.UpdateSalesVisualMeter(id, DateTime.UtcNow);
        }
        #region jsonlist
        public JsonResult CountryList()
        {
            List<com.kuazoo.Country> listc = countryservice.GetCountryListActive().Result;
            List<MasterModel.Country> newlistcat = new List<MasterModel.Country>();
            foreach (var v in listc)
            {
                newlistcat.Add(new MasterModel.Country() { CountryId = v.CountryId, CountryName = v.Name });
            }
            IEnumerable<MasterModel.Country> ienuList = newlistcat;
            return Json(ienuList, JsonRequestBehavior.AllowGet);

        }
        public JsonResult CityList(int countryid = 0, int stateid = 0)
        {
            List<com.kuazoo.City> listc = countryservice.GetCityListActive(countryid).Result;
            List<MasterModel.City> newlistcat = new List<MasterModel.City>();
            foreach (var v in listc)
            {
                newlistcat.Add(new MasterModel.City() { CityId = v.CityId, CityName = v.Name });
            }
            IEnumerable<MasterModel.City> ienuList = newlistcat;
            return Json(ienuList, JsonRequestBehavior.AllowGet);

        }
        public JsonResult MerchantList()
        {
            List<com.kuazoo.Merchant> listc = merchantservice.GetMerchantList().Result;
            List<MerchantModel> newlistcat = new List<MerchantModel>();
            listc = listc.Where(x => x.LastAction != "5").ToList();
            foreach (var v in listc)
            {
                newlistcat.Add(new MerchantModel() { MerchantId = v.MerchantId, Name = v.Name });
            }
            IEnumerable<MerchantModel> ienuList = newlistcat;
            return Json(ienuList, JsonRequestBehavior.AllowGet);

        }
        public JsonResult MerchantDetail(int id)
        {
            var result = merchantservice.GetMerchantById(id).Result;
            MerchantModel res = new MerchantModel();
            res.MerchantId = result.MerchantId;
            res.Name = result.Name;
            res.AddressLine1 = result.AddressLine1;
            res.City = new MasterModel.City() { CityName = result.City.Name };
            res.PostCode = result.PostCode;
            res.Latitude = result.Latitude;
            res.Longitude = result.Longitude;
            res.Country = new MasterModel.Country() { CountryName = result.Country.Name };
            res.Description = result.Description;
            res.SubImageUrl = string.Join("|", result.SubImageUrl);
            res.SubImageUrlLink = string.Join("|", result.SubImageUrlLink);
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public JsonResult TempSalesVisual(int id, decimal margin)
        {
            var result = transactionservice.GetTempSalesVisualMeterNewInventory(id, margin);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult InventoryItemList()
        {
            List<com.kuazoo.InventoryItem> listc = flashdealservice.GetInventoryItemList().Result;
            List<InventoryItemModel.InventoryItem> newlistcat = new List<InventoryItemModel.InventoryItem>();
            foreach (var v in listc)
            {
                newlistcat.Add(new InventoryItemModel.InventoryItem() { InventoryItemId = v.InventoryItemId, Name = v.Name });
            }
            IEnumerable<InventoryItemModel.InventoryItem> ienuList = newlistcat;
            return Json(ienuList, JsonRequestBehavior.AllowGet);

        }
        public JsonResult InventoryItemListNonFeatured()
        {
            DateTime today = DateTime.UtcNow;
            List<com.kuazoo.InventoryItem> listc = inventoryitemservice.GetInventoryNonFeatured(today).Result;
            List<InventoryItemModel.InventoryItem> newlistcat = new List<InventoryItemModel.InventoryItem>();
            foreach (var v in listc)
            {
                newlistcat.Add(new InventoryItemModel.InventoryItem() { InventoryItemId = v.InventoryItemId, Name = v.Name });
            }
            IEnumerable<InventoryItemModel.InventoryItem> ienuList = newlistcat;
            return Json(ienuList, JsonRequestBehavior.AllowGet);

        }
        public JsonResult InventoryItemListByMerchant(int MerchantId)
        {
            List<com.kuazoo.InventoryItem> listc = flashdealservice.GetInventoryItemListByMerchant(MerchantId).Result;
            List<InventoryItemModel.InventoryItem> newlistcat = new List<InventoryItemModel.InventoryItem>();
            foreach (var v in listc)
            {
                newlistcat.Add(new InventoryItemModel.InventoryItem() { InventoryItemId = v.InventoryItemId, Name = v.Name });
            }
            IEnumerable<InventoryItemModel.InventoryItem> ienuList = newlistcat;
            return Json(ienuList, JsonRequestBehavior.AllowGet);

        }
        public JsonResult InventoryItemTypeList()
        {
            List<com.kuazoo.InventoryItemType> listc = inventoryitemservice.GetInventoryItemTypeList().Result;
            List<InventoryItemModel.InventoryItemType> newlistcat = new List<InventoryItemModel.InventoryItemType>();
            foreach (var v in listc)
            {
                newlistcat.Add(new InventoryItemModel.InventoryItemType() { InventoryItemTypeId = v.InventoryItemTypeId, InventoryItemTypeName = v.InventoryItemTypeName });
            }
            IEnumerable<InventoryItemModel.InventoryItemType> ienuList = newlistcat;
            return Json(ienuList, JsonRequestBehavior.AllowGet);

        }
        public JsonResult PrizeList()
        {
            List<com.kuazoo.Prize> listc = inventoryitemservice.GetPrizeList().Result;
            List<InventoryItemModel.Prize> newlistcat = new List<InventoryItemModel.Prize>();
            foreach (var v in listc)
            {
                newlistcat.Add(new InventoryItemModel.Prize() { PrizeId = v.PrizeId, Name = v.Name});
            }
            IEnumerable<InventoryItemModel.Prize> ienuList = newlistcat;
            return Json(ienuList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult PrizeDetail(int id)
        {
            var result = prizeservice.GetPrizeById(id).Result;
            InventoryItemModel.Prize res = new InventoryItemModel.Prize();
            res.PrizeId = result.PrizeId;
            res.Name = result.Name;
            res.Description = result.Description != null?result.Description:"";
            res.Terms = result.Terms != null ? result.Terms : "";
            res.Detail = result.Detail != null ? result.Detail : "";
            res.ImageUrl = result.ImageUrl;
            res.SubImageUrl = string.Join("|", result.SubImageUrl);
            res.ExpiryDateStr = string.Format("{0:yyyy-MM-dd'T'HH:mm:ssZ}", DateTime.SpecifyKind(result.ExpiryDate, DateTimeKind.Utc));
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public JsonResult TagList()
        {
            List<com.kuazoo.Tag> listtag = inventoryitemservice.GetTagList().Result;
            List<InventoryItemModel.Tag> newlisttag = new List<InventoryItemModel.Tag>();
            foreach (var v in listtag)
            {
                newlisttag.Add(new InventoryItemModel.Tag() { TagId = v.TagId, TagName = v.Name });
            }
            IEnumerable<InventoryItemModel.Tag> ienuList = newlisttag;
            return Json(ienuList, JsonRequestBehavior.AllowGet);

        }
        public JsonResult MemberStatusList()
        {
            List<com.kuazoo.MemberStatus> listtag = memberservice.GetMemberStatusList().Result;
            List<MemberModel.MemberStatus> newlisttag = new List<MemberModel.MemberStatus>();
            foreach (var v in listtag)
            {
                newlisttag.Add(new MemberModel.MemberStatus() { MemberStatusId = v.MemberStatusId,MemberStatusName =v.MemberStatusName });
            }
            IEnumerable<MemberModel.MemberStatus> ienuList = newlisttag;
            return Json(ienuList, JsonRequestBehavior.AllowGet);

        }
        public JsonResult MemberList()
        {
            List<com.kuazoo.Member> listtag = memberservice.GetMemberList().Result;
            List<MemberModel.Member> newlisttag = new List<MemberModel.Member>();
            foreach (var v in listtag)
            {
                newlisttag.Add(new MemberModel.Member() { MemberId =v.MemberId, Email = v.Email });
            }
            IEnumerable<MemberModel.Member> ienuList = newlisttag;
            return Json(ienuList, JsonRequestBehavior.AllowGet);

        }
        #endregion

        public JsonResult GenerateCoupon(int id)
        {
            try
            {
                generalservice.GenerateCode(id);

                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("Failed", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult LandingPage()
        {
            if (TempData["msg"] != null)
            {
                ViewBag.msg = TempData["msg"];
            }
            return View();
        }


        public ActionResult Banner_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<com.kuazoo.Banner> listmer = landingservice.GetBannerList().Result;
            List<BannerModel> newlistmer = new List<BannerModel>();
            foreach (var v in listmer)
            {
                newlistmer.Add(new BannerModel() { BannerId = v.BannerId, Name = v.Name, Link = v.Link, SubImageId = v.SubImageId, SubImageName = v.SubImageName, SubImageUrl = v.SubImageUrl, SubImageUrlLink = v.SubImageUrlLink, LastAction = v.LastAction, Seq = v.Seq });
            }
            IEnumerable<BannerModel> ienuList = newlistmer;
            var result = ienuList.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public ActionResult BannerAdd()
        {
            return View();
        }

        [HttpPost]
        public ActionResult BannerAdd(FormCollection collection, BannerModel model)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    com.kuazoo.Banner bnr = new com.kuazoo.Banner();

                    bnr.BannerId = 0;
                    bnr.Name = model.Name;
                    bnr.Link = model.Link;
                    bnr.Seq = model.Seq != null ? model.Seq <= 0 ? 99999 : (int)model.Seq : 99999;

                    bnr.SubImageId = model.SubImageId;
                    bnr.SubImageName = model.SubImageName;
                    bnr.SubImageUrl = model.SubImageUrl;
                    bnr.SubImageUrlLink = model.SubImageUrlLink;

                    bool result = landingservice.CreateBanner(bnr).Result;
                    ViewBag.result = "3";
                    if (result == true)
                    {
                        ViewBag.result = "1";
                    }
                    TempData["msg"] = ViewBag.result;
                    return RedirectToAction("LandingPage", "Admin");
                }
                catch (Exception ex)
                {
                    CustomException(ex);
                    ViewBag.result = ex.Message;
                    return View();
                }

            }

            return View();
        }

        public ActionResult BannerEdit(int BannerId)
        {
            com.kuazoo.Banner v = landingservice.GetBannerById(BannerId).Result;
            BannerModel banner = new BannerModel() { BannerId = v.BannerId, Name = v.Name, Link = v.Link, SubImageId = v.SubImageId, SubImageName = v.SubImageName, SubImageUrl = v.SubImageUrl, SubImageUrlLink = v.SubImageUrlLink, LastAction = v.LastAction, Seq = v.Seq };
            return View(banner);
        }

        [HttpPost]
        public ActionResult BannerEdit(FormCollection collection, BannerModel model)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    com.kuazoo.Banner bnr = new com.kuazoo.Banner();

                    bnr.BannerId = model.BannerId;
                    bnr.Name = model.Name;
                    bnr.Link = model.Link;
                    bnr.Seq = model.Seq != null ? model.Seq <= 0 ? 99999 : (int)model.Seq : 99999;
                    
                    bnr.SubImageId = model.SubImageId;
                    bnr.SubImageName = model.SubImageName;
                    bnr.SubImageUrl = model.SubImageUrl;
                    bnr.SubImageUrlLink = model.SubImageUrlLink;

                    bool result = landingservice.CreateBanner(bnr).Result;
                    ViewBag.result = "33";
                    if (result == true)
                    {
                        ViewBag.result = "11";
                    }
                    TempData["msg"] = ViewBag.result;
                    return RedirectToAction("LandingPage", "Admin");
                }
                catch (Exception ex)
                {
                    CustomException(ex);
                    ViewBag.result = ex.Message;
                    return View();
                }

            }

            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Banner_Destroy([DataSourceRequest] DataSourceRequest request, BannerModel Banner)
        {

            try
            {
                if (Banner != null)
                {
                    Boolean result = landingservice.DeleteBanner(Banner.BannerId).Result;

                    if (result) ViewBag.result = "1";
                    else ViewBag.result = "2";
                    ModelState.AddModelError("", ViewBag.result);

                }
            }
            catch (Exception ex)
            {
                CustomException(ex);
                ViewBag.result = ex.Message;
                ModelState.AddModelError("", ViewBag.result);
            }

            return Json(ModelState.ToDataSourceResult());
        }


        public ActionResult BVD_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<com.kuazoo.BVD> listmer = landingservice.GetBVDList().Result;
            List<BVDModel> newlistmer = new List<BVDModel>();
            foreach (var v in listmer)
            {
                newlistmer.Add(new BVDModel() { BVDId = v.BVDId, Title = v.Title, Type = v.Type, Link = v.Link, SubImageId = v.SubImageId, SubImageName = v.SubImageName, SubImageUrl = v.SubImageUrl, SubImageUrlLink = v.SubImageUrlLink, LastAction = v.LastAction, Seq = v.Seq });
            }
            IEnumerable<BVDModel> ienuList = newlistmer;
            var result = ienuList.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BVD_Destroy([DataSourceRequest] DataSourceRequest request, BVDModel BVD)
        {

            try
            {
                if (BVD != null)
                {
                    Boolean result = landingservice.DeleteBVD(BVD.BVDId).Result;

                    if (result) ViewBag.result = "11";
                    else ViewBag.result = "22";
                    ModelState.AddModelError("", ViewBag.result);

                }
            }
            catch (Exception ex)
            {
                CustomException(ex);
                ViewBag.result = ex.Message;
                ModelState.AddModelError("", ViewBag.result);
            }

            return Json(ModelState.ToDataSourceResult());
        }


        public ActionResult BVDAdd()
        {
            return View();
        }

        [HttpPost]
        public ActionResult BVDAdd(FormCollection collection, BVDModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    com.kuazoo.BVD bvd = new com.kuazoo.BVD();

                    bvd.BVDId = 0;
                    bvd.Title = model.Title;
                    bvd.Type = model.Type;
                    bvd.Link = model.Link;
                    bvd.Seq = model.Seq != null ? model.Seq <= 0 ? 99999 : (int)model.Seq : 99999;
                    if (model.Type == 0)
                        bvd.UpdatedDate = model.UpdatedDate;

                    bvd.SubImageId = model.SubImageId;
                    bvd.SubImageName = model.SubImageName;
                    bvd.SubImageUrl = model.SubImageUrl;
                    bvd.SubImageUrlLink = model.SubImageUrlLink;

                    bool result = landingservice.CreateBVD(bvd).Result;
                    ViewBag.result = "4";
                    if (result == true)
                    {
                        ViewBag.result = "2";
                    }
                    TempData["msg"] = ViewBag.result;
                    return RedirectToAction("LandingPage", "Admin");
                }
                catch (Exception ex)
                {
                    CustomException(ex);
                    ViewBag.result = ex.Message;
                    return View();
                }

            }

            return View();
        }



        public ActionResult BVDEdit(int BVDId)
        {
            com.kuazoo.BVD v = landingservice.GetBVDById(BVDId).Result;
            BVDModel BVD = new BVDModel() { BVDId = v.BVDId, Title = v.Title, Type = v.Type, Link = v.Link, SubImageId = v.SubImageId, SubImageName = v.SubImageName, SubImageUrl = v.SubImageUrl, SubImageUrlLink = v.SubImageUrlLink, LastAction = v.LastAction, Seq = v.Seq, UpdatedDate = v.UpdatedDate };
            return View(BVD);
        }

        [HttpPost]
        public ActionResult BVDEdit(FormCollection collection, BVDModel model)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    com.kuazoo.BVD bvd = new com.kuazoo.BVD();

                    bvd.BVDId = model.BVDId;
                    bvd.Title = model.Title;
                    bvd.Type = model.Type;
                    bvd.Link = model.Link;
                    bvd.Seq = model.Seq != null ? model.Seq <= 0 ? 99999 : (int)model.Seq : 99999;
                    if(model.Type == 0)
                        bvd.UpdatedDate = model.UpdatedDate;

                    bvd.SubImageId = model.SubImageId;
                    bvd.SubImageName = model.SubImageName;
                    bvd.SubImageUrl = model.SubImageUrl;
                    bvd.SubImageUrlLink = model.SubImageUrlLink;

                    bool result = landingservice.CreateBVD(bvd).Result;
                    ViewBag.result = "44";
                    if (result == true)
                    {
                        ViewBag.result = "22";
                    }
                    TempData["msg"] = ViewBag.result;
                    return RedirectToAction("LandingPage", "Admin");
                }
                catch (Exception ex)
                {
                    CustomException(ex);
                    ViewBag.result = ex.Message;
                    return View();
                }

            }

            return View();
        }

        public ActionResult SliderPreview()
        {
            var link = Request.QueryString["link"];
            var imgurl = Request.QueryString["imgurl"];

            LandingModel.SliderPreview model = new LandingModel.SliderPreview();

            if (link.IndexOf("http") == -1)
                link = "http://" + link;

            model.Link = link;
            model.SubImageUrl = imgurl;

            return View(model);
        }

    }
}
