using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using System.Net.Mail;

namespace com.kuazoo
{
    public class FreeDealService : IFreeDealService
    {

        public Response<bool> CreateFreeDeal(FreeDealTransaction model, string path, int gmt = 8)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                if (model.FreeDealId != 0)
                {
                    var entityFreeDeal = from d in context.kzTransactionFreeDeals
                                         where d.id == model.FreeDealId
                                     select d;
                    if (entityFreeDeal.Count() > 0)
                    {
                        entityFreeDeal.First().process_status = model.ProcessStatus;
                        entityFreeDeal.First().process_date = DateTime.UtcNow;
                        entityFreeDeal.First().last_action = "3";
                        context.SaveChanges();
                        response = Response<bool>.Create(true);
                    }
                    else
                    {
                        throw new CustomException(CustomErrorType.FreeDealNotFound);
                    }
                }
                else
                {
                    entity.kzTransactionFreeDeal mmentity = new entity.kzTransactionFreeDeal();
                    mmentity.inventoryitem_id = model.InventoryItemId;
                    if (model.FlashDealId != null && model.FlashDealId != 0)
                    {
                        mmentity.flashdeal_id = model.FlashDealId;
                    }
                    mmentity.user_id = model.MemberId;
                    mmentity.variance = model.Variance;
                    mmentity.qty = model.Qty;
                    mmentity.discount = model.Discount;
                    mmentity.price = model.Price;
                    mmentity.transaction_date = DateTime.UtcNow;
                    mmentity.process_status = false;
                    mmentity.last_action = "1";
                    context.AddTokzTransactionFreeDeals(mmentity);
                    context.SaveChanges();
                    response = Response<bool>.Create(true);
                    int transactionId = mmentity.id;
                    var entiyInventory = from d in context.kzInventoryItems
                                         where d.id == model.InventoryItemId
                                         select d.inventoryitem_type_id;
                    if (entiyInventory.First() == 2)
                    {
                        int qty = (int)model.Qty;
                        var entityCodeTra = from d in context.kzPreCodes
                                            where d.transaction_id == transactionId
                                            select d;
                        if (entityCodeTra.Count() > 0)
                        {
                            qty = qty - entityCodeTra.Count();
                        }
                        if (qty > 0)
                        {
                            var entityCode = (from d in context.kzPreCodes
                                              where d.transaction_id.HasValue == false
                                              orderby d.id ascending
                                              select d).Take(qty);
                            foreach (var c in entityCode)
                            {
                                c.transaction_id = transactionId;
                            }
                            context.SaveChanges();
                        }
                    }
                    TransactionFreeDealMemberNotif(mmentity.id,path, gmt);
                }
            }
            return response;
        }

        public Response<TransactionsHeader> GetTransactionFreeDealAnHour(DateTime today)
        {
            Response<TransactionsHeader> response = null;
            TransactionsHeader transaction = new TransactionsHeader();
            using (var context = new entity.KuazooEntities())
            {
                DateTime now = DateTime.UtcNow.AddHours(-1);
                var user = new UserService().GetCurrentUser().Result;
                if (user.UserId != 0)
                {
                    var entityTransaction = from d in context.kzTransactionFreeDeals
                                            where d.transaction_date >= now
                                            && d.last_action != "5"
                                            && d.user_id == user.UserId
                                            orderby d.transaction_date descending
                                            select d;
                    if (entityTransaction.Count() > 0)
                    {
                        var v = entityTransaction.First();
                        transaction.TransactionId = v.id;
                        transaction.TransactionDate = (DateTime)v.transaction_date;
                        transaction.MemberId = (int)v.user_id;
                        List<TransactionsDetail> detail = new List<TransactionsDetail>();
                        TransactionsDetail de = new TransactionsDetail();
                        de.InventoryItemId = (int)v.inventoryitem_id;
                        de.InventoryItemName = v.kzInventoryItem.name;

                        var entityImg = from d in context.kzInventoryItemImages
                                        from e in context.kzImages
                                        where d.image_id == e.id && d.inventoryitem_id == de.InventoryItemId
                                        && d.main == true
                                        select new { d.image_id, e.url };

                        if (entityImg.Count() > 0)
                        {
                            de.InventoryItemImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImg.First().url;
                        }
                        else
                        {
                            de.InventoryItemImageUrl = "";
                        }

                        if (v.flashdeal_id != null)
                        {
                            de.FlashDealId = v.flashdeal_id;
                        }
                        de.Variance = v.variance;
                        de.Qty = (int)v.qty;
                        de.Discount = (decimal)v.discount;
                        de.Price = (decimal)v.price;
                        if (v.kzInventoryItem.prize_id != null)
                        {
                            de.PrizeId = (int)v.kzInventoryItem.prize_id;
                            de.PrizeName = v.kzInventoryItem.kzPrize.name;
                            var entityImgPrize = from d in context.kzPrizeImages
                                                    from e in context.kzImages
                                                    where d.image_id == e.id && d.prize_id == de.PrizeId
                                                    && d.main == true
                                                    select new { d.image_id, e.url };

                            if (entityImgPrize.Count() > 0)
                            {
                                de.PrizeNameImageUrl = ConfigurationManager.AppSettings["uploadpath"] + entityImgPrize.First().url;
                            }
                            else
                            {
                                de.PrizeNameImageUrl = "";
                            }
                        }
                        detail.Add(de);
                        transaction.Detail = detail;

                        transaction.SimilarDetail = new InventoryItemService().GetInventoryItemListExpireSoon4(today, de.InventoryItemId).Result;

                    }
                }
                response = Response<TransactionsHeader>.Create(transaction);
            }

            return response;
        }

        public Response<List<FreeDealTransactionReport>> GetAllTransactionFreeDealList()
        {
            Response<List<FreeDealTransactionReport>> response = null;
            List<FreeDealTransactionReport> TransactionList = new List<FreeDealTransactionReport>();
            using (var context = new entity.KuazooEntities())
            {
                var entityTransaction = from d in context.kzTransactionFreeDeals.Include("kzInventoryItem")
                                        where d.last_action != "5"
                                        orderby d.transaction_date descending
                                        select d;
                foreach (var v in entityTransaction)
                {
                    FreeDealTransactionReport Transaction = new FreeDealTransactionReport();
                    Transaction.TransactionId = (int)v.id;
                    Transaction.TransactionDate = (DateTime)v.transaction_date;
                    Transaction.MerchantId = (int)v.kzInventoryItem.merchant_id;
                    Transaction.MerchantName = v.kzInventoryItem.kzMerchant.name;
                    Transaction.InventoryItemId = (int)v.inventoryitem_id;
                    Transaction.InventoryItemName = v.kzInventoryItem.name;
                    string variance = v.variance.Split('`')[0];
                    Transaction.Variance = variance;
                    var varianceentity = from d in context.kzVariances
                                         where d.inventoryitem_id == Transaction.InventoryItemId
                                         && d.name.ToLower() == variance.ToLower()
                                         select d;
                    Transaction.SKU = "";
                    if (varianceentity.Count() > 0)
                    {
                        Transaction.SKU = varianceentity.First().sku.ToString();
                    }
                    Transaction.Qty = (int)v.qty;
                    var voucherEntities = from d in context.kzPreCodes
                                          where d.transaction_id == Transaction.TransactionId
                                          select d;
                    List<string> _vo = new List<string>();
                    foreach (var vo in voucherEntities)
                    {
                        _vo.Add(vo.code);
                    }
                    Transaction.VoucherCode = string.Join(", ", _vo);

                    Transaction.MemberId = (int)v.user_id;
                    Transaction.MemberEmail = v.kzUser.email;
                    Transaction.MemberFirstName = v.kzUser.first_name;
                    Transaction.MemberLastName = v.kzUser.last_name;
                    Transaction.ProcessStatus = "No";
                    if (v.process_status != null && v.process_status == true)
                    {
                        Transaction.ProcessStatus = "Yes";
                    }
                    if (v.process_date != null)
                    {
                        Transaction.ProcessDate = (DateTime)v.process_date;
                    }

                    TransactionList.Add(Transaction);
                }
                response = Response<List<FreeDealTransactionReport>>.Create(TransactionList);
            }

            return response;
        }
        public Response<List<FreeDealTransactionReport>> GetSelectedTransactionFreeDealList(string transactionlist)
        {
            Response<List<FreeDealTransactionReport>> response = null;
            List<FreeDealTransactionReport> TransactionList = new List<FreeDealTransactionReport>();
            using (var context = new entity.KuazooEntities())
            {
                List<int> listtrans = new List<int>();
                string[] _tranlsit = transactionlist.Split(',');
                foreach (var v in _tranlsit)
                {
                    try
                    {
                        if (v != "")
                        {
                            listtrans.Add(int.Parse(v));
                        }
                    }
                    catch { }
                }
                var entityTransaction = from d in context.kzTransactionFreeDeals.Include("kzInventoryItem")
                                        where d.last_action != "5" && listtrans.Contains(d.id)
                                        orderby d.transaction_date descending
                                        select d;
                foreach (var v in entityTransaction)
                {
                    FreeDealTransactionReport Transaction = new FreeDealTransactionReport();
                    Transaction.TransactionId = (int)v.id;
                    Transaction.TransactionDate = (DateTime)v.transaction_date;
                    Transaction.MerchantId = (int)v.kzInventoryItem.merchant_id;
                    Transaction.MerchantName = v.kzInventoryItem.kzMerchant.name;
                    Transaction.InventoryItemId = (int)v.inventoryitem_id;
                    Transaction.InventoryItemName = v.kzInventoryItem.name;
                    string variance = v.variance.Split('`')[0];
                    Transaction.Variance = variance;
                    var varianceentity = from d in context.kzVariances
                                         where d.inventoryitem_id == Transaction.InventoryItemId
                                         && d.name.ToLower() == variance.ToLower()
                                         select d;
                    Transaction.SKU = "";
                    if (varianceentity.Count() > 0)
                    {
                        Transaction.SKU = varianceentity.First().sku.ToString();
                    }
                    Transaction.Qty = (int)v.qty;
                    var voucherEntities = from d in context.kzPreCodes
                                          where d.transaction_id == Transaction.TransactionId
                                          select d;
                    List<string> _vo = new List<string>();
                    foreach (var vo in voucherEntities)
                    {
                        _vo.Add(vo.code);
                    }
                    Transaction.VoucherCode = string.Join(", ", _vo);

                    Transaction.MemberId = (int)v.user_id;
                    Transaction.MemberEmail = v.kzUser.email;
                    Transaction.MemberFirstName = v.kzUser.first_name;
                    Transaction.MemberLastName = v.kzUser.last_name;
                    Transaction.ProcessStatus = "No";
                    if (v.process_status != null && v.process_status == true)
                    {
                        Transaction.ProcessStatus = "Yes";
                    }
                    if (v.process_date != null)
                    {
                        Transaction.ProcessDate = (DateTime)v.process_date;
                    }

                    TransactionList.Add(Transaction);
                }
                response = Response<List<FreeDealTransactionReport>>.Create(TransactionList);
            }

            return response;
        }

        public Response<Boolean> UpdateSelectedTransactionFreeDealList(string transactionlist)
        {
            Response<Boolean> response = null;
            using (var context = new entity.KuazooEntities())
            {
                List<int> listtrans = new List<int>();
                string[] _tranlsit = transactionlist.Split(',');
                foreach (var v in _tranlsit)
                {
                    try
                    {
                        if (v != "")
                        {
                            listtrans.Add(int.Parse(v));
                        }
                    }
                    catch { }
                }
                DateTime now = DateTime.UtcNow;
                var entityTransaction = from d in context.kzTransactionFreeDeals
                                        where d.last_action != "5" && listtrans.Contains(d.id)
                                        select d;
                foreach (var v in entityTransaction)
                {
                    if (v.process_status == null || v.process_status != true)
                    {
                        v.process_status = true;
                        v.process_date = now;
                    }
                }
                context.SaveChanges();
                response = Response<Boolean>.Create(true);
            }

            return response;
        }


        public void TransactionFreeDealMemberNotif(int Transactionid,string path, int gmt)
        {
            try
            {
                using (var context = new entity.KuazooEntities())
                {
                    var entityTransaction = from d in context.kzTransactionFreeDeals
                                            where d.id == Transactionid
                                            select d;
                    if (entityTransaction != null && entityTransaction.Count() > 0)
                    {
                        int userid = (int)entityTransaction.First().user_id;
                        var userEntity = from d in context.kzUsers
                                         where d.id == userid
                                         select d;
                        string to = userEntity.First().email;
                        string body = "";
                        string orderno = GeneralService.FreeTransactionCode(entityTransaction.First().id, (DateTime)entityTransaction.First().transaction_date);
                        string subject = "Thanks for Shopping at Kuazoo! Order No:" + orderno;

                        string customer = entityTransaction.First().kzUser.first_name + " " + entityTransaction.First().kzUser.last_name;
                        var merchanten = entityTransaction.First().kzInventoryItem.kzMerchant;
                        string merchant = merchanten.name;

                        DateTime transactiondate = (DateTime)entityTransaction.First().transaction_date;
                        transactiondate = transactiondate.AddHours(gmt);
                        string dealname = entityTransaction.First().kzInventoryItem.name;
                        int dealid = (int)entityTransaction.First().inventoryitem_id;
                        string url = WebSetting.Url + "/Home/Deals/" + kuazoo.helper.ReplaceSymbol.Replace(dealname) + "?d=1";
                        string fineprint = entityTransaction.First().kzInventoryItem.terms;

                        string imageurl = "";
                        var entityImg = from d in context.kzInventoryItemImages
                                        from e in context.kzImages
                                        where d.image_id == e.id && d.inventoryitem_id == dealid
                                        && d.main == true
                                        select new { d.image_id, e.url };

                        if (entityImg.Count() > 0)
                        {
                            imageurl = ConfigurationManager.AppSettings["uploadpath"] + entityImg.First().url;
                        }
                        string variance = entityTransaction.First().variance;
                        string[] _vari = variance.Split('`');
                        string variname = _vari[0];
                        var entityVariance = from d in context.kzVariances
                                             where d.name == variname && d.inventoryitem_id == dealid
                                             select d;
                        string sku = "";
                        if (entityVariance.Count() > 0)
                        {
                            sku = entityVariance.First().sku;
                        }
                        string interestdealimg = "<tr>";
                        string interestdeal = "<tr>";

                        var entitydeals = new InventoryItemService().GetInventoryItemListTakeV2(DateTime.UtcNow, 3).Result;
                        string _urldeal;
                        foreach (var v in entitydeals)
                        {
                            _urldeal = WebSetting.Url + "/Home/Deals/" + kuazoo.helper.ReplaceSymbol.Replace(v.Name) + "?d=1";
                            interestdealimg += @"<td style=""width:30%"">
												<a href=""" + _urldeal + @""">
													<img src=""" + v.ImageUrl + @""" alt=""" + v.Name + @""" style=""width:100%"">
												</a>
											</td>";
                            decimal pricedis = v.Price - (v.Price * v.Discount / 100);
                            interestdeal += @"<td style=""vertical-align: top;"">
												<table border=""0"" cellpadding=""0"" cellspacing=""0"" style=""width:100%"">
													<tr>
														<td style=""text-align:left;vertical-align:top"">
															" + v.Name + @"  
														</td>
														<td style=""text-align:right;color:#0084cf; width:100px;vertical-align:top"">
															RM " + pricedis.ToString("N0") + @"<br/>
															<span style=""font-size:13px;color:#666666"">" + v.Discount.ToString("N0") + @"% OFF</span>
														</td>
													</tr>	
												</table>
											</td>";
                        }
                        interestdealimg += @"</tr>";
                        interestdeal += @"</tr>";

                        if (entityTransaction.First().kzInventoryItem.inventoryitem_type_id == 1)
                        {
                            body = helper.Email.CreatePurchaseEmailFree(customer, merchant, orderno, transactiondate, url, dealname, "", sku, interestdealimg + interestdeal, to, imageurl, variname);

                            new GeneralService().SendEmail(subject, to, body);
                        }
                        else
                        {
                            List<string> voucherlist = new List<string>();
                            string vouchercode = "";
                            var entityCode = from d in context.kzPreCodes
                                             where d.transaction_id == Transactionid
                                             select d;
                            int i = 0;
                            foreach (var vo in entityCode)
                            {
                                i++;
                                if (i == 1)
                                {
                                    vouchercode += "<tr>";
                                }
                                voucherlist.Add(vo.code);
                                vouchercode += "<td><strong>Voucher Code: </strong> " + vo.code + "</td>";
                                if (i == 3)
                                {
                                    vouchercode += "</tr>";
                                    i = 0;
                                }
                            }
                            if (i != 0)
                            {
                                vouchercode += "</tr>";
                            }
                            //MemoryStream pdf = Getpdf(subject, voucherlist);
                            //Attachment att1 = new Attachment(pdf, "voucher.pdf");
                            body = helper.Email.CreatePurchaseServicesEmailFREE(customer, merchant, orderno, transactiondate, url, dealname, fineprint, sku, vouchercode, interestdealimg + interestdeal, to, imageurl, variname);
                            //MemoryStream pdf = GetpdfHtml2(body, path);
                            //Attachment att1 = new Attachment(pdf, "voucher.pdf");
                            //new GeneralService().SendEmail(subject, to, body, "", att1);
                            new GeneralService().SendEmail(subject, to, body);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        public MemoryStream Getpdf(string subject, List<String> voucher)
        {
            MemoryStream workStream = new MemoryStream();
            Document document = new Document(iTextSharp.text.PageSize.LETTER, 10, 10, 42, 35);
            PdfWriter.GetInstance(document, workStream).CloseStream = false;

            document.Open();
            document.Add(new Paragraph(subject));
            document.Add(new Paragraph("Your Voucher Code as below:"));
            foreach (var v in voucher)
            {
                document.Add(new Paragraph(v));
            }
            document.Close();
            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;

            return workStream;
        }

        public MemoryStream GetpdfHtml(string html)
        {
            MemoryStream workStream = new MemoryStream();
            Document document = new Document(iTextSharp.text.PageSize.LETTER, 10, 10, 42, 35);
            PdfWriter.GetInstance(document, workStream).CloseStream = false;

            HTMLWorker hw = new HTMLWorker(document);
            document.Open();
            hw.Parse(new StringReader(html.ToString()));
            document.Close();
            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;

            return workStream;
        }
        public MemoryStream GetpdfHtml2(string html, string path)
        {

            byte[] pdfByteArray = Rotativa.WkhtmltopdfDriver.ConvertHtml(path, "-q", html);
            var input = new MemoryStream(pdfByteArray);
            return input;
        }
    }
}
