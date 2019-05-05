using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Net;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using com.kuazoo;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
namespace Kuazoo
{
    public static class Helper
    {
        public static string SetFacebookId
        {
            get
            {
                string id = WebSetting.FaceBookId;
                return id;
            }
        }
        public static string SetFacebookSecret
        {
            get
            {
                string id = WebSetting.FaceBookSecret;
                return id;
            }
        }
        public static string SetFacebookAction
        {
            get
            {
                string id = WebSetting.FacebookAction;
                return id;
            }
        }
        public static string SetUrl
        {
            get
            {
                string id = WebSetting.Url;
                return id;
            }
        }

        public static bool FlashDealAlert()
        {
            bool result = false;
            int count = new com.kuazoo.InventoryItemService().GetInventoryItemFlashDealCount(DateTime.UtcNow);
            if (count > 0) result = true;
            return result;
        }
        public static int PendingUser()
        {
            int result = 0;
            //int user = new com.kuazoo.UserService().GetCurrentUserPending().Result;
            //if (user == 4)
            //{
            //    result = 1;
            //}
            return result;
        }
        public static string ReplaceSymbol(string name)
        {
            return name.Replace(" ", "-").Replace("!", "").Replace("+", "").Replace(":", "").Replace(" ", "").Replace("(", "").Replace(")", "").Replace("[", "").Replace("]", "").Replace("{", "").Replace("}", "").Replace("*", "").Replace("&", "").Replace("@", "").Replace("=", "").Replace("%", "").Replace("/","").Replace(".","");
            
        }
        public static string DecimalPlace(decimal price)
        {
            decimal temp = Math.Round(price, 2);
            if ((temp - (int)temp) != 0)
            {
                return price.ToString("N2");
            }
            else
            {
                return price.ToString("N0");
            }
        }
        public static Dictionary<string,int> GetListCity()
        {
            Dictionary<string, int> list = new Dictionary<string, int>();
            var result = new com.kuazoo.CountryService().GetCityListActive(1).Result;
            foreach (var v in result)
            {
                list.Add(v.Name, v.CityId);
            }
            return list;
        }
        public static Dictionary<string, int> GetListTag()
        {
            Dictionary<string, int> list = new Dictionary<string, int>();
            var result = new com.kuazoo.TagService().GetTagListActive().Result;
            foreach (var v in result)
            {
                list.Add(v.Name, v.TagId);
            }
            return list;
        }
        public static string StringBullet(string str)
        {
            if(str!=null && str.IndexOf("<b>")>-1){
                string first = str.Substring(0, str.IndexOf("<b>"));
                string second = str.Substring(str.IndexOf("<b>"),str.IndexOf("</b>")-str.IndexOf("<b>")+4);
                second = second.Replace("<b>-","<ul><li>").Replace("<b>", "<ul><li>").Replace("</b>", "</li></ul>").Replace("\n-","</li><li>").Replace("<li></li>","");
                                  
                string third = str.Substring(str.IndexOf("</b>")+4);
                return first+second+third;
            }
            else if (str != null)
            {
                return str;
            }
            else
            {
                return "";
            }
        }
        public static string currUserName
        {
            get
            {
                try
                {
                    var user = new UserService().GetCurrentUser().Result;

                    return user.Email;
                }
                catch
                {
                    return null;
                }
            }
        }
        public static string currAdminName
        {
            get
            {
                try
                {
                    var user = new AdminService().GetCurrentAdmin().Result;

                    return user.Email;
                }
                catch
                {
                    return null;
                }
            }
        }
        public static string currFirstLastName
        {
            get
            {
                try
                {
                    var user = new UserService().GetCurrentUser().Result;

                    return user.FirstName +" " +user.LastName;
                }
                catch
                {
                    return null;
                }
            }
        }
        public static string currFacebookId
        {
            get
            {
                try
                {
                    var user = new UserService().GetCurrentUser().Result;

                    return user.FacebookId;
                }
                catch
                {
                    return null;
                }
            }
        }
        public static string currImage
        {
            get
            {
                try
                {
                    var user = new UserService().GetCurrentUser().Result;

                    return user.ImageUrl;
                }
                catch
                {
                    return null;
                }
            }
        }
        public static decimal currKPoint
        {
            get
            {
                try
                {
                    var user = new UserService().GetCurrentUser().Result;

                    return user.KPoint;
                }
                catch
                {
                    return 0;
                }
            }
        }
        public static int defaultGMT
        {
            get
            {
                int gmt = int.Parse(ConfigurationManager.AppSettings["GMT"]);
                return gmt;
            }
        }
        public static string imagebaseURL
        {
            get
            {
                string url = ConfigurationManager.AppSettings["uploadpath"];
                return url;
            }
        }
        public static string merchantId
        {
            get
            {
                string url = ConfigurationManager.AppSettings["MOLmerchant"];
                return url;
            }
        }
        public static string merchantkey
        {
            get
            {
                string url = ConfigurationManager.AppSettings["MOLkey"];
                return url;
            }
        }
        public static int pageSize
        {
            get
            {
                int gmt = int.Parse(ConfigurationManager.AppSettings["PageSize"]);
                return gmt;
            }
        }
        public static string howkuazoowork
        {
            get
            {
                try
                {
                    var us = new GeneralService().GetStaticByName("how-kuazoo-work");
                    if (us != null)
                    {
                        return us.Result.Description;
                    }
                    else
                    {
                        return "";
                    }
                }
                catch
                {
                    return "";
                }
            }
        }
        //public static string uploadImage(HttpPostedFileBase FileModel)
        //{
        //    string uploadedPath = "";
        //    if (FileModel.ContentLength > 0)
        //    {
        //        var fileName = Path.GetFileName(FileModel.FileName);
        //        //var cloudinary = new Cloudinary(
        //        //    new Account(ConfigurationSettings.AppSettings["CloudName"].ToString(),
        //        //                ConfigurationSettings.AppSettings["CloudKey"].ToString(),
        //        //                ConfigurationSettings.AppSettings["CloudSecret"].ToString()));
        //        //var uploadParams = new ImageUploadParams()
        //        //{
        //        //    File = new FileDescription(fileName),
        //        //    PublicId = "sample_id",
        //        //    Transformation = new Transformation().Crop("limit").Width(40).Height(40),
        //        //    EagerTransforms = new List<Transformation>()
        //        //              {
        //        //                new Transformation().Width(200).Height(200).Crop("thumb").Gravity("face").
        //        //                  Radius(20).Effect("sepia"),
        //        //                new Transformation().Width(100).Height(150).Crop("fit").FetchFormat("png")
        //        //              },
        //        //    Tags = "special, for_homepage"
        //        //};

        //        //var uploadResult = cloudinary.Upload(uploadParams);

        //        ////upload azure
        //        ////access container
        //        //CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
        //        //CloudConfigurationManager.GetSetting("StorageConnectionString"));

        //        //// Create the blob client
        //        //CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

        //        //// Retrieve reference to a previously created container
        //        //CloudBlobContainer container = blobClient.GetContainerReference("upload");

        //        //// Create the container if it doesn't already exist
        //        //container.CreateIfNotExist();
        //        //container.SetPermissions(
        //        //new BlobContainerPermissions
        //        //{
        //        //    PublicAccess =
        //        //        BlobContainerPublicAccessType.Blob
        //        //});

        //        //// Retrieve reference to a blob named "myblob"
        //        //CloudBlob blob = container.GetBlobReference(fileName);
        //        //blob.Properties.ContentType = "image\\jpeg";    //make sure saved as jpeg

        //        //// Create or overwrite the "myblob" blob with contents from a local file


        //        //MemoryStream target = new MemoryStream();
        //        //FileModel.InputStream.CopyTo(target);
        //        ////StreamReader sourceStream = new StreamReader(FileModel.InputStream);
        //        ////byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
        //        //byte[] fileContents = target.ToArray();
        //        //target.Close();
        //        //blob.UploadByteArray(fileContents);
        //        //uploadedPath = fileName;

        //        //upload to local
        //        uploadedPath = fileName;
        //        var path = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/upload"), fileName);
        //        FileModel.SaveAs(path);


        //        ////try upload to ftp server first
        //        //string ftpBaseAddress = @"ftp://ftp.appstream.com.my";
        //        //string username = "appstrea";
        //        //string password = "hunk220use130";

        //        //FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(ftpBaseAddress + "/public_html/kuazoo/" + fileName);
        //        //request.Method = WebRequestMethods.Ftp.UploadFile;
        //        //request.Credentials = new NetworkCredential(username, password);
        //        //request.UsePassive = true;
        //        //request.UseBinary = true;
        //        //request.KeepAlive = false;
        //        //MemoryStream target = new MemoryStream();
        //        //FileModel.InputStream.CopyTo(target);
        //        ////StreamReader sourceStream = new StreamReader(FileModel.InputStream);
        //        ////byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
        //        //byte[] fileContents = target.ToArray();
        //        //target.Close();
        //        //request.ContentLength = fileContents.Length;

        //        //Stream requestStream = request.GetRequestStream();
        //        //requestStream.Write(fileContents, 0, fileContents.Length);
        //        //requestStream.Close();

        //        //FtpWebResponse response = (FtpWebResponse)request.GetResponse();
        //    }
        //    else
        //    {
        //        throw new Exception("invalid filemodel");
        //    }

        //    return uploadedPath;
        //}
        //public static string uploadImageWithName(HttpPostedFileBase FileModel, String name)
        //{
        //    string uploadedPath = "";
        //    if (FileModel.ContentLength > 0)
        //    {
        //        var fileName = Path.GetFileName(FileModel.FileName);
        //        //var cloudinary = new Cloudinary(
        //        //    new Account(ConfigurationSettings.AppSettings["CloudName"].ToString(),
        //        //                ConfigurationSettings.AppSettings["CloudKey"].ToString(),
        //        //                ConfigurationSettings.AppSettings["CloudSecret"].ToString()));
        //        //var uploadParams = new ImageUploadParams()
        //        //{
        //        //    File = new FileDescription(fileName),
        //        //    PublicId = "sample_id",
        //        //    Transformation = new Transformation().Crop("limit").Width(40).Height(40),
        //        //    EagerTransforms = new List<Transformation>()
        //        //              {
        //        //                new Transformation().Width(200).Height(200).Crop("thumb").Gravity("face").
        //        //                  Radius(20).Effect("sepia"),
        //        //                new Transformation().Width(100).Height(150).Crop("fit").FetchFormat("png")
        //        //              },
        //        //    Tags = "special, for_homepage"
        //        //};

        //        //var uploadResult = cloudinary.Upload(uploadParams);


        //        fileName = name + "-" + fileName;
        //        //upload to local
        //        uploadedPath = fileName;
        //        //var path = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/upload"), fileName);
        //        //FileModel.SaveAs(path);


        //        ////try upload to ftp server first
        //        string ftpBaseAddress = @"ftp://ftp.appstream.com.my";
        //        string username = "appstrea";
        //        string password = "hunk220use130";

        //        FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(ftpBaseAddress + "/public_html/kuazoo/" + fileName);
        //        request.Method = WebRequestMethods.Ftp.UploadFile;
        //        request.Credentials = new NetworkCredential(username, password);
        //        request.UsePassive = true;
        //        request.UseBinary = true;
        //        request.KeepAlive = false;
        //        MemoryStream target = new MemoryStream();
        //        FileModel.InputStream.CopyTo(target);
        //        //StreamReader sourceStream = new StreamReader(FileModel.InputStream);
        //        //byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
        //        byte[] fileContents = target.ToArray();
        //        target.Close();
        //        request.ContentLength = fileContents.Length;

        //        Stream requestStream = request.GetRequestStream();
        //        requestStream.Write(fileContents, 0, fileContents.Length);
        //        requestStream.Close();

        //        FtpWebResponse response = (FtpWebResponse)request.GetResponse();
        //    }
        //    else
        //    {
        //        throw new Exception("invalid filemodel");
        //    }

        //    return uploadedPath;
        //}
        #region local

        public static string uploadImage(HttpPostedFileBase FileModel)
        {
            string uploadedPath = "";
            if (FileModel.ContentLength > 0)
            {
                var fileName = Path.GetFileName(FileModel.FileName);

                //upload to local
                uploadedPath = fileName;
                var path = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/upload"), fileName);
                FileModel.SaveAs(path);
            }
            else
            {
                throw new Exception("invalid filemodel");
            }

            return uploadedPath;
        }
        public static string uploadImageWithName(HttpPostedFileBase FileModel, String name)
        {
            string uploadedPath = "";
            if (FileModel.ContentLength > 0)
            {
                var fileName = Path.GetFileName(FileModel.FileName);
                fileName = name + "-" + fileName;
                fileName = fileName.Replace(" ", "-");
                //upload to local
                uploadedPath = fileName;
                var path = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/upload"), fileName);
                FileModel.SaveAs(path);
                System.Drawing.Image resize = resizeImage(FileModel, 200, 200);
                resize.Save(Path.Combine(HttpContext.Current.Server.MapPath("~/Content/upload"), "thumb-" + fileName));
            }
            else
            {
                throw new Exception("invalid filemodel");
            }

            return uploadedPath;
        }
        public static string uploadProfileImage(HttpPostedFileBase FileModel, String name)
        {
            string uploadedPath = "";
            if (FileModel.ContentLength > 0)
            {
                var fileName = Path.GetFileName(FileModel.FileName);
                fileName = "profileimg" + name + fileName;
                fileName = fileName.Replace(" ", "-");
                //upload to local
                uploadedPath = fileName;
                var path = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/upload"), fileName);
                FileModel.SaveAs(path);
                System.Drawing.Image resize = resizeImage(FileModel, 200, 200);
                resize.Save(Path.Combine(HttpContext.Current.Server.MapPath("~/Content/upload"), "thumb-" + fileName));

            }
            else
            {
                throw new Exception("invalid filemodel");
            }

            return uploadedPath;
        }

        #endregion

        #region azure

        //public static string uploadImage(HttpPostedFileBase FileModel)
        //{
        //    string uploadedPath = "";
        //    if (FileModel.ContentLength > 0)
        //    {
        //        var fileName = Path.GetFileName(FileModel.FileName);

        //        //upload azure
        //        //access container
        //        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
        //        CloudConfigurationManager.GetSetting("StorageConnectionString"));

        //        // Create the blob client
        //        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

        //        // Retrieve reference to a previously created container
        //        CloudBlobContainer container = blobClient.GetContainerReference("upload");

        //        // Create the container if it doesn't already exist
        //        container.CreateIfNotExist();
        //        container.SetPermissions(
        //        new BlobContainerPermissions
        //        {
        //            PublicAccess =
        //                BlobContainerPublicAccessType.Blob
        //        });

        //        // Retrieve reference to a blob named "myblob"
        //        CloudBlob blob = container.GetBlobReference(fileName);
        //        blob.Properties.ContentType = "image\\jpeg";    //make sure saved as jpeg

        //        // Create or overwrite the "myblob" blob with contents from a local file


        //        MemoryStream target = new MemoryStream();
        //        FileModel.InputStream.CopyTo(target);
        //        //StreamReader sourceStream = new StreamReader(FileModel.InputStream);
        //        //byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
        //        byte[] fileContents = target.ToArray();
        //        target.Close();
        //        blob.UploadByteArray(fileContents);
        //        uploadedPath = fileName;
        //    }
        //    else
        //    {
        //        throw new Exception("invalid filemodel");
        //    }

        //    return uploadedPath;
        //}
        //public static string uploadImageWithName(HttpPostedFileBase FileModel, String name)
        //{
        //    string uploadedPath = "";
        //    if (FileModel.ContentLength > 0)
        //    {
        //        var fileName = Path.GetFileName(FileModel.FileName);
        //        fileName = name + "-" + fileName;
        //        fileName = fileName.Replace(" ", "-");
        //        //access container
        //        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
        //        CloudConfigurationManager.GetSetting("StorageConnectionString"));

        //        // Create the blob client
        //        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

        //        // Retrieve reference to a previously created container
        //        CloudBlobContainer container = blobClient.GetContainerReference("upload");

        //        // Create the container if it doesn't already exist
        //        container.CreateIfNotExist();
        //        container.SetPermissions(
        //        new BlobContainerPermissions
        //        {
        //            PublicAccess =
        //                BlobContainerPublicAccessType.Blob
        //        });

        //        // Retrieve reference to a blob named "myblob"
        //        CloudBlob blob = container.GetBlobReference(fileName);
        //        blob.Properties.ContentType = "image\\jpeg";    //make sure saved as jpeg

        //        // Create or overwrite the "myblob" blob with contents from a local file


        //        MemoryStream target = new MemoryStream();
        //        FileModel.InputStream.CopyTo(target);
        //        //StreamReader sourceStream = new StreamReader(FileModel.InputStream);
        //        //byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
        //        byte[] fileContents = target.ToArray();
        //        target.Close();
        //        blob.UploadByteArray(fileContents);
        //        uploadedPath = fileName;

        //        System.Drawing.Image resize = resizeImage(FileModel, 200, 200);
        //        var filenamethumb = "thumb-" + fileName;
        //        // Retrieve reference to a blob named "myblob"
        //        CloudBlob blob2 = container.GetBlobReference(filenamethumb);
        //        blob2.Properties.ContentType = "image\\jpeg";    //make sure saved as jpeg

        //        MemoryStream target2 = new MemoryStream();
        //        resize.Save(target2, System.Drawing.Imaging.ImageFormat.Jpeg);
        //        byte[] fileContents2 = target2.ToArray();
        //        target2.Close();
        //        blob2.UploadByteArray(fileContents2);

        //    }
        //    else
        //    {
        //        throw new Exception("invalid filemodel");
        //    }

        //    return uploadedPath;
        //}

        //public static string uploadProfileImage(HttpPostedFileBase FileModel, String name)
        //{
        //    string uploadedPath = "";
        //    if (FileModel.ContentLength > 0)
        //    {
        //        var fileName = Path.GetFileName(FileModel.FileName);
        //        fileName = "profileimg" + name + fileName;
        //        fileName = fileName.Replace(" ", "-");

        //        //access container
        //        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
        //        CloudConfigurationManager.GetSetting("StorageConnectionString"));

        //        // Create the blob client
        //        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

        //        // Retrieve reference to a previously created container
        //        CloudBlobContainer container = blobClient.GetContainerReference("upload");

        //        // Create the container if it doesn't already exist
        //        container.CreateIfNotExist();
        //        container.SetPermissions(
        //        new BlobContainerPermissions
        //        {
        //            PublicAccess =
        //                BlobContainerPublicAccessType.Blob
        //        });

        //        // Retrieve reference to a blob named "myblob"
        //        CloudBlob blob = container.GetBlobReference(fileName);
        //        blob.Properties.ContentType = "image\\jpeg";    //make sure saved as jpeg

        //        // Create or overwrite the "myblob" blob with contents from a local file


        //        MemoryStream target = new MemoryStream();
        //        FileModel.InputStream.CopyTo(target);
        //        //StreamReader sourceStream = new StreamReader(FileModel.InputStream);
        //        //byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
        //        byte[] fileContents = target.ToArray();
        //        target.Close();
        //        blob.UploadByteArray(fileContents);
        //        uploadedPath = fileName;

        //        System.Drawing.Image resize = resizeImage(FileModel, 200, 200);
        //        var filenamethumb = "thumb-" + fileName;
        //        // Retrieve reference to a blob named "myblob"
        //        CloudBlob blob2 = container.GetBlobReference(filenamethumb);
        //        blob2.Properties.ContentType = "image\\jpeg";    //make sure saved as jpeg

        //        MemoryStream target2 = new MemoryStream();
        //        resize.Save(target2, System.Drawing.Imaging.ImageFormat.Jpeg);
        //        byte[] fileContents2 = target2.ToArray();
        //        target2.Close();
        //        blob2.UploadByteArray(fileContents2);
        //    }
        //    else
        //    {
        //        throw new Exception("invalid filemodel");
        //    }

        //    return uploadedPath;
        //}
        #endregion
        //#region azure

        //public static string uploadImage(HttpPostedFileBase FileModel)
        //{
        //    string uploadedPath = "";
        //    if (FileModel.ContentLength > 0)
        //    {
        //        var fileName = Path.GetFileName(FileModel.FileName);

        //        //upload azure
        //        //access container
        //        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
        //        CloudConfigurationManager.GetSetting("StorageConnectionString"));

        //        // Create the blob client
        //        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

        //        // Retrieve reference to a previously created container
        //        CloudBlobContainer container = blobClient.GetContainerReference("uploadstaging");

        //        // Create the container if it doesn't already exist
        //        container.CreateIfNotExist();
        //        container.SetPermissions(
        //        new BlobContainerPermissions
        //        {
        //            PublicAccess =
        //                BlobContainerPublicAccessType.Blob
        //        });

        //        // Retrieve reference to a blob named "myblob"
        //        CloudBlob blob = container.GetBlobReference(fileName);
        //        blob.Properties.ContentType = "image\\jpeg";    //make sure saved as jpeg

        //        // Create or overwrite the "myblob" blob with contents from a local file


        //        MemoryStream target = new MemoryStream();
        //        FileModel.InputStream.CopyTo(target);
        //        //StreamReader sourceStream = new StreamReader(FileModel.InputStream);
        //        //byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
        //        byte[] fileContents = target.ToArray();
        //        target.Close();
        //        blob.UploadByteArray(fileContents);
        //        uploadedPath = fileName;
        //    }
        //    else
        //    {
        //        throw new Exception("invalid filemodel");
        //    }

        //    return uploadedPath;
        //}
        //public static string uploadImageWithName(HttpPostedFileBase FileModel, String name)
        //{
        //    string uploadedPath = "";
        //    if (FileModel.ContentLength > 0)
        //    {
        //        var fileName = Path.GetFileName(FileModel.FileName);
        //        fileName = name + "-" + fileName;
        //        fileName = fileName.Replace(" ", "-");
        //        //access container
        //        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
        //        CloudConfigurationManager.GetSetting("StorageConnectionString"));

        //        // Create the blob client
        //        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

        //        // Retrieve reference to a previously created container
        //        CloudBlobContainer container = blobClient.GetContainerReference("uploadstaging");

        //        // Create the container if it doesn't already exist
        //        container.CreateIfNotExist();
        //        container.SetPermissions(
        //        new BlobContainerPermissions
        //        {
        //            PublicAccess =
        //                BlobContainerPublicAccessType.Blob
        //        });

        //        // Retrieve reference to a blob named "myblob"
        //        CloudBlob blob = container.GetBlobReference(fileName);
        //        blob.Properties.ContentType = "image\\jpeg";    //make sure saved as jpeg

        //        // Create or overwrite the "myblob" blob with contents from a local file


        //        MemoryStream target = new MemoryStream();
        //        FileModel.InputStream.CopyTo(target);
        //        //StreamReader sourceStream = new StreamReader(FileModel.InputStream);
        //        //byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
        //        byte[] fileContents = target.ToArray();
        //        target.Close();
        //        blob.UploadByteArray(fileContents);
        //        uploadedPath = fileName;

        //        System.Drawing.Image resize = resizeImage(FileModel, 200, 200);
        //        var filenamethumb = "thumb-" + fileName;
        //        // Retrieve reference to a blob named "myblob"
        //        CloudBlob blob2 = container.GetBlobReference(filenamethumb);
        //        blob2.Properties.ContentType = "image\\jpeg";    //make sure saved as jpeg

        //        MemoryStream target2 = new MemoryStream();
        //        resize.Save(target2, System.Drawing.Imaging.ImageFormat.Jpeg);
        //        byte[] fileContents2 = target2.ToArray();
        //        target2.Close();
        //        blob2.UploadByteArray(fileContents2);

        //    }
        //    else
        //    {
        //        throw new Exception("invalid filemodel");
        //    }

        //    return uploadedPath;
        //}

        //public static string uploadProfileImage(HttpPostedFileBase FileModel, String name)
        //{
        //    string uploadedPath = "";
        //    if (FileModel.ContentLength > 0)
        //    {
        //        var fileName = Path.GetFileName(FileModel.FileName);
        //        fileName = "profileimg" + name + fileName;
        //        fileName = fileName.Replace(" ", "-");

        //        //access container
        //        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
        //        CloudConfigurationManager.GetSetting("StorageConnectionString"));

        //        // Create the blob client
        //        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

        //        // Retrieve reference to a previously created container
        //        CloudBlobContainer container = blobClient.GetContainerReference("uploadstaging");

        //        // Create the container if it doesn't already exist
        //        container.CreateIfNotExist();
        //        container.SetPermissions(
        //        new BlobContainerPermissions
        //        {
        //            PublicAccess =
        //                BlobContainerPublicAccessType.Blob
        //        });

        //        // Retrieve reference to a blob named "myblob"
        //        CloudBlob blob = container.GetBlobReference(fileName);
        //        blob.Properties.ContentType = "image\\jpeg";    //make sure saved as jpeg

        //        // Create or overwrite the "myblob" blob with contents from a local file


        //        MemoryStream target = new MemoryStream();
        //        FileModel.InputStream.CopyTo(target);
        //        //StreamReader sourceStream = new StreamReader(FileModel.InputStream);
        //        //byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
        //        byte[] fileContents = target.ToArray();
        //        target.Close();
        //        blob.UploadByteArray(fileContents);
        //        uploadedPath = fileName;

        //        System.Drawing.Image resize = resizeImage(FileModel, 200, 200);
        //        var filenamethumb = "thumb-" + fileName;
        //        // Retrieve reference to a blob named "myblob"
        //        CloudBlob blob2 = container.GetBlobReference(filenamethumb);
        //        blob2.Properties.ContentType = "image\\jpeg";    //make sure saved as jpeg

        //        MemoryStream target2 = new MemoryStream();
        //        resize.Save(target2, System.Drawing.Imaging.ImageFormat.Jpeg);
        //        byte[] fileContents2 = target2.ToArray();
        //        target2.Close();
        //        blob2.UploadByteArray(fileContents2);
        //    }
        //    else
        //    {
        //        throw new Exception("invalid filemodel");
        //    }

        //    return uploadedPath;
        //}
        //#endregion
        public static bool IsValidImage(this string fileName)
        {
            Regex regex = new Regex(@"(.*?)\.(jpg|JPG|jpeg|JPEG|png|PNG|gif|GIF|bmp|BMP)$");
            return regex.IsMatch(fileName);
        }
        public static System.Drawing.Image resizeImage(HttpPostedFileBase FileModel, int heigth, int width, Boolean keepAspectRatio = true, Boolean getCenter = true)
        {

            int newheigth = heigth;
            System.Drawing.Image FullsizeImage = System.Drawing.Image.FromStream(FileModel.InputStream);
            //Fix CMYKs
            FullsizeImage = ConvertCMYK(FullsizeImage);
            // Prevent using images internal thumbnail
            FullsizeImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);
            FullsizeImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);

            if (keepAspectRatio || getCenter)
            {
                int bmpY = 0;
                double resize = (double)FullsizeImage.Width / (double)width;//get the resize vector
                if (getCenter)
                {
                    bmpY = (int)((FullsizeImage.Height - (heigth * resize)) / 2);// gives the Y value of the part that will be cut off, to show only the part in the center
                    Rectangle section = new Rectangle(new System.Drawing.Point(0, bmpY), new System.Drawing.Size(FullsizeImage.Width, (int)(heigth * resize)));// create the section to cut of the original image
                    

                    Bitmap orImg = new Bitmap((Bitmap)FullsizeImage);//for the correct effect convert image to bitmap.
                    FullsizeImage.Dispose();//clear the original image
                    using (Bitmap tempImg = new Bitmap(section.Width, section.Height))
                    {
                        Graphics cutImg = Graphics.FromImage(tempImg);//              set the file to save the new image to.
                        cutImg.DrawImage(orImg, 0, 0, section, GraphicsUnit.Pixel);// cut the image and save it to tempImg
                        FullsizeImage = tempImg;//save the tempImg as FullsizeImage for resizing later
                        orImg.Dispose();
                        cutImg.Dispose();
                        return FullsizeImage.GetThumbnailImage(width, heigth, null, IntPtr.Zero);
                    }
                }
                else newheigth = (int)(FullsizeImage.Height / resize);//  set the new heigth of the current image
            }//return the image resized to the given heigth and width
            return FullsizeImage.GetThumbnailImage(width, newheigth, null, IntPtr.Zero);
        }

        public static string GetImageFlags(System.Drawing.Image MyImage)
        {
            ImageFlags FlagVals = (ImageFlags)Enum.Parse(typeof(ImageFlags), MyImage.Flags.ToString());
            return FlagVals.ToString();
        }

        public static bool IsCMYK(System.Drawing.Image MyImage)
        {
            bool ReturnVal;

            if ((GetImageFlags(MyImage).IndexOf("Ycck") > -1) || (GetImageFlags(MyImage).IndexOf("Cmyk") > -1))
            { ReturnVal = true; }
            else
            { ReturnVal = false; }

            return ReturnVal;
        }

        public static Bitmap ConvertCMYK(System.Drawing.Image MyBitmap)
        {
            Bitmap NewBit = new Bitmap(MyBitmap.Width, MyBitmap.Height, PixelFormat.Format24bppRgb);

            Graphics MyGraph = Graphics.FromImage(NewBit);
            MyGraph.CompositingQuality = CompositingQuality.HighQuality;
            MyGraph.SmoothingMode = SmoothingMode.HighQuality;
            MyGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;

            Rectangle MyRect = new Rectangle(0, 0, MyBitmap.Width, MyBitmap.Height);
            MyGraph.DrawImage(MyBitmap, MyRect);

            Bitmap ReturnBitmap = new Bitmap(NewBit);

            MyGraph.Dispose();
            NewBit.Dispose();
            MyBitmap.Dispose();

            return ReturnBitmap;
        }
    }
}