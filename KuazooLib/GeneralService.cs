using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.Globalization;
using System.IO;
using System.Threading;

namespace com.kuazoo
{
    public class GeneralService : IGeneralService
    {
        static string TransCode = "";
        public static void InitConfigSettings()
        {
            string configName = "local";
            //string configName = "office";
            //string configName = "prod";
            //string configName = "staging";
            switch (configName)
            {
                case "local":
                    WebSetting.FaceBookId = "634486043303789";
                    WebSetting.FaceBookSecret = "8f33325b010372f28466dbf9117a35e5";
                    WebSetting.FacebookAction = "kuazoodev";
                    WebSetting.Url = "http://localhost:2070";
                    WebSetting.TwitterKey = "PtorVGHb7oTjWf34y6M93ItRL";
                    WebSetting.TwitterSecret = "gLhxyQ6y8FUS4nCzmGvMQjOStKV2SX9ZhlQ6dOxySiX4EPZ2KD";
                    WebSetting.TwitterToken = "91772297-GbXTwBzck8fAFUq19xK9HCYJJNzUoO1NDiUvI3de0";
                    WebSetting.TwitterTokenSecret = "BrHKa0wpSODoSYLfn7q4RhBk7U00FYRVQafCi1PtvosxT";
                    TransCode = "KZ-";
                    break;
                //case "office":
                //    WebSetting.FaceBookId = "634484109970649";
                //    WebSetting.FaceBookSecret = "67be35e3b99ce8b5a3738b7ad0de82af";
                //    WebSetting.FacebookAction = "kuazoooffice";
                //    WebSetting.Url = "http://appstream.dlinkddns.com:8123/kuazoo";
                //    WebSetting.TwitterKey = "jalSHxqgiHE7Vxy4Ixhmoyr61";
                //    WebSetting.TwitterSecret = "38aRCsFHLDlA2fmz761JyRgsI7uZwpzLSEuuyuMePfoVoSFklH";
                //    WebSetting.TwitterToken = "91772297-GbXTwBzck8fAFUq19xK9HCYJJNzUoO1NDiUvI3de0";
                //    WebSetting.TwitterTokenSecret = "BrHKa0wpSODoSYLfn7q4RhBk7U00FYRVQafCi1PtvosxT";
                case "office":
                    WebSetting.FaceBookId = "634484109970649";
                    WebSetting.FaceBookSecret = "67be35e3b99ce8b5a3738b7ad0de82af";
                    WebSetting.FacebookAction = "kuazoooffice";
                    WebSetting.Url = "http://appstream1.cloudapp.net/kuazoo";
                    WebSetting.TwitterKey = "jalSHxqgiHE7Vxy4Ixhmoyr61";
                    WebSetting.TwitterSecret = "38aRCsFHLDlA2fmz761JyRgsI7uZwpzLSEuuyuMePfoVoSFklH";
                    WebSetting.TwitterToken = "91772297-GbXTwBzck8fAFUq19xK9HCYJJNzUoO1NDiUvI3de0";
                    WebSetting.TwitterTokenSecret = "BrHKa0wpSODoSYLfn7q4RhBk7U00FYRVQafCi1PtvosxT";
                    TransCode = "KZ-";
                    break;
                case "staging":
                    WebSetting.FaceBookId = "634484109970649";
                    WebSetting.FaceBookSecret = "67be35e3b99ce8b5a3738b7ad0de82af";
                    WebSetting.FacebookAction = "kuazoooffice";
                    WebSetting.Url = "http://www.kuazoo.com.my/kuazoostaging";
                    WebSetting.TwitterKey = "jalSHxqgiHE7Vxy4Ixhmoyr61";
                    WebSetting.TwitterSecret = "38aRCsFHLDlA2fmz761JyRgsI7uZwpzLSEuuyuMePfoVoSFklH";
                    WebSetting.TwitterToken = "91772297-GbXTwBzck8fAFUq19xK9HCYJJNzUoO1NDiUvI3de0";
                    WebSetting.TwitterTokenSecret = "BrHKa0wpSODoSYLfn7q4RhBk7U00FYRVQafCi1PtvosxT";
                    TransCode = "UAT-";
                    break;
                case "prod":
                    WebSetting.FaceBookId = "545383078927644";
                    WebSetting.FaceBookSecret = "e4eca52c7e6352a85127c8993f649e84";
                    WebSetting.FacebookAction = "kuazoo";
                    WebSetting.Url = "http://www.kuazoo.com.my";
                    WebSetting.TwitterKey = "jalSHxqgiHE7Vxy4Ixhmoyr61";
                    WebSetting.TwitterSecret = "38aRCsFHLDlA2fmz761JyRgsI7uZwpzLSEuuyuMePfoVoSFklH";
                    WebSetting.TwitterToken = "91772297-GbXTwBzck8fAFUq19xK9HCYJJNzUoO1NDiUvI3de0";
                    WebSetting.TwitterTokenSecret = "BrHKa0wpSODoSYLfn7q4RhBk7U00FYRVQafCi1PtvosxT";
                    TransCode = "KZ-";
                    break;
                default:
                    WebSetting.FaceBookId = "634486043303789";
                    WebSetting.FaceBookSecret = "8f33325b010372f28466dbf9117a35e5";
                    WebSetting.FacebookAction = "kuazoodev";
                    WebSetting.Url = "http://localhost:2070";
                    WebSetting.TwitterKey = "jalSHxqgiHE7Vxy4Ixhmoyr61";
                    WebSetting.TwitterSecret = "38aRCsFHLDlA2fmz761JyRgsI7uZwpzLSEuuyuMePfoVoSFklH";
                    WebSetting.TwitterToken = "91772297-GbXTwBzck8fAFUq19xK9HCYJJNzUoO1NDiUvI3de0";
                    WebSetting.TwitterTokenSecret = "BrHKa0wpSODoSYLfn7q4RhBk7U00FYRVQafCi1PtvosxT";
                    TransCode = "KZ-";
                    break;
            }
        }

        public void SendEmail(string subject, string to, string body, string cc = "", Attachment attach = null)
        {
            System.Net.Mail.MailMessage email = new System.Net.Mail.MailMessage();
            email.From = new MailAddress("noreply@kuazoo.com.my", "Kuazoo");
            email.To.Add(to);
            if (cc != "")
            {
                email.CC.Add(cc);
            }
            email.Subject = subject;
            email.IsBodyHtml = true;
            email.Body += body;
            if (attach != null)
            {
                email.Attachments.Add(attach);
            }
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Credentials = new NetworkCredential("noreply@kuazoo.com.my", "hfactory");
            smtpClient.Host = "smtp.gmail.com";
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;
            try
            {
                //try to send port 587  first 
                smtpClient.Send(email);
                try
                {
                    using (var context = new entity.KuazooEntities())
                    {
                        entity.kzEmailSendLog em = new entity.kzEmailSendLog();
                        em.email = to;
                        em.status = 1;
                        em.subject = subject;
                        em.creationdate = DateTime.UtcNow;
                        context.AddTokzEmailSendLogs(em);
                        context.SaveChanges();
                    }
                }
                catch
                {
                }
            }
            catch
            {
                try
                {
                    using (var context = new entity.KuazooEntities())
                    {
                        entity.kzEmailSendLog em = new entity.kzEmailSendLog();
                        em.email = to;
                        em.status = 0;
                        em.subject = subject;
                        em.creationdate = DateTime.UtcNow;
                        context.AddTokzEmailSendLogs(em);
                        context.SaveChanges();
                    }
                }
                catch
                {
                }
                //try to send port 26  then 
                smtpClient.Port = 26;
                smtpClient.Send(email);
                try
                {
                    using (var context = new entity.KuazooEntities())
                    {
                        entity.kzEmailSendLog em = new entity.kzEmailSendLog();
                        em.email = to;
                        em.status = 1;
                        em.subject = subject;
                        em.creationdate = DateTime.UtcNow;
                        context.AddTokzEmailSendLogs(em);
                        context.SaveChanges();
                    }
                }
                catch
                {
                }
            }
        }

        public static string TransactionCode(int id, DateTime date)
        {
            string result = TransCode + string.Format("{0:MMyy}", date) + "-";
            string code = "";
            if (id < 10000000)
            {
                code = "0000000" + id.ToString();
            }
            else if (id < 1000000)
            {
                code = "000000" + id.ToString();
            }
            else if (id < 100000)
            {
                code = "00000" + id.ToString();
            }
            else if (id < 10000)
            {
                code = "0000" + id.ToString();
            }
            else if (id < 1000)
            {
                code = "000" + id.ToString();
            }
            else if (id < 100)
            {
                code = "00" + id.ToString();
            }
            else if (id < 10)
            {
                code = "0" + id.ToString();
            }
            else
            {
                code = id.ToString();
            }
            result = result + code;
            return result;
        }
        public static string FreeTransactionCode(int id, DateTime date)
        {
            string result = TransCode+"FR-" + string.Format("{0:MMyy}", date) + "-";
            string code = "";
            if (id < 10000000)
            {
                code = "0000000" + id.ToString();
            }
            else if (id < 1000000)
            {
                code = "000000" + id.ToString();
            }
            else if (id < 100000)
            {
                code = "00000" + id.ToString();
            }
            else if (id < 10000)
            {
                code = "0000" + id.ToString();
            }
            else if (id < 1000)
            {
                code = "000" + id.ToString();
            }
            else if (id < 100)
            {
                code = "00" + id.ToString();
            }
            else if (id < 10)
            {
                code = "0" + id.ToString();
            }
            else
            {
                code = id.ToString();
            }
            result = result + code;
            return result;
        }
        public static Int32 ExtractTransactionId(string code)
        {
            string result = code.Substring(8);
            return Convert.ToInt32(result);
        }
        public static int WeeksInYear(DateTime date)
        {
            GregorianCalendar cal = new GregorianCalendar(GregorianCalendarTypes.Localized);
            return cal.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Monday);

        }



        public Response<bool> UpdateStatic(Static item)
        {
            Response<bool> response = null;
            using (var context = new entity.KuazooEntities())
            {
                if (item.StaticId == 0)
                {
                    throw new CustomException(CustomErrorType.StaticNotFound);
                }
                else
                {
                    var entityStatic = from d in context.kzStatics
                                       where d.id == item.StaticId
                                       select d;
                    if (entityStatic.Count() > 0)
                    {

                        entityStatic.First().description = item.Description;
                        context.SaveChanges();
                    }
                }
                response = Response<bool>.Create(true);
            }
            return response;
        }
        public Response<List<Static>> GetStaticList()
        {
            Response<List<Static>> response = null;
            List<Static> StaticList = new List<Static>();
            using (var context = new entity.KuazooEntities())
            {
                var entityStatic = from d in context.kzStatics
                                   select d;
                foreach (var v in entityStatic)
                {
                    Static Static = new Static();
                    Static.StaticId = v.id;
                    Static.Name = v.name;
                    Static.Description = v.description;
                    StaticList.Add(Static);
                }
                response = Response<List<Static>>.Create(StaticList);
            }

            return response;
        }
        public Response<Static> GetStaticByName(String name)
        {
            Response<Static> response = null;
            Static Static = new Static();
            using (var context = new entity.KuazooEntities())
            {
                var entityStatic = from d in context.kzStatics
                                   where d.name == name
                                   select d;
                if (entityStatic.Count() > 0)
                {
                    var v = entityStatic.First();
                    Static.StaticId = v.id;
                    Static.Name = v.name;
                    Static.Description = v.description;
                    response = Response<Static>.Create(Static);
                }
            }

            return response;
        }
        public Response<Static> GetStaticById(int id)
        {
            Response<Static> response = null;
            Static Static = new Static();
            using (var context = new entity.KuazooEntities())
            {
                var entityStatic = from d in context.kzStatics
                                   where d.id == id
                                   select d;
                if (entityStatic.Count() > 0)
                {
                    var v = entityStatic.First();
                    Static.StaticId = v.id;
                    Static.Name = v.name;
                    Static.Description = v.description;
                    response = Response<Static>.Create(Static);
                }
            }

            return response;
        }
        public Response<Boolean> GenerateCode(int count)
        {
            Response<Boolean> response = null;
            try
            {
                using (var context = new entity.KuazooEntities())
                {
                    List<string> couponcodeOld = new List<string>();
                    List<string> couponcodeNew = new List<string>();
                    DateTime now = DateTime.UtcNow.Date;
                    var entityCoupon = from d in context.kzPreCodes
                                       select d;
                    foreach (var v in entityCoupon)
                    {
                        couponcodeOld.Add(v.code);
                    }
                    char[] keys = "ABCDEFGHIJKLMNOPQRSTUVWXYZ01234567890".ToCharArray();
                    while (couponcodeNew.Count < count)
                    {
                        var voucher = GenerateCode(keys, 6);
                        if (!couponcodeOld.Contains(voucher))
                        {
                            couponcodeOld.Add(voucher);
                            couponcodeNew.Add(voucher);
                            entity.kzPreCode newEntity = new entity.kzPreCode();
                            newEntity.code = voucher;
                            context.AddTokzPreCodes(newEntity);
                        }
                    }
                    context.SaveChanges();


                    response = Response<Boolean>.Create(true);
                }
            }
            catch
            {
            }
            return response;
        }

        private static Random random = new Random();
        private static string GenerateCode(char[] keys, int lengthOfVoucher)
        {
            return Enumerable
                .Range(1, lengthOfVoucher) // for(i.. ) 
                .Select(k => keys[random.Next(0, keys.Length - 1)])  // generate a new random char 
                .Aggregate("", (e, c) => e + c); // join into a string
        }


        public static void LoggingException(string ip, string url, string exception, string message)
        {
            using (var context = new entity.KuazooEntities())
            {
                try
                {
                    entity.kzWebExceptionLogger log = new entity.kzWebExceptionLogger();
                    log.last_created = DateTime.UtcNow;
                    log.url = url;
                    log.ip_address = ip;
                    string user = Thread.CurrentPrincipal.Identity.Name;
                    log.logger = user;
                    log.err_exception = exception;
                    log.err_message = message;
                    context.AddTokzWebExceptionLoggers(log);
                    context.SaveChanges();
                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}
