using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Reflection;
using System.ComponentModel;
using System.Web.Security;

namespace com.kuazoo
{
    [DataContract]
    [KnownType(typeof(Error))]
    public class Response<T>
    {
        [DataMember(IsRequired = false)]
        public T Result { get; set; }
        [DataMember(IsRequired = false)]
        public Error Error { get; set; }
        [DataMember(IsRequired = false)]
        public string Id { get; set; }

        public Response() { }

        /// <summary>
        /// Creates a default success Response.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static Response<T> Create()
        {
            return new Response<T>(default(T), null);
        }
        /// <summary>
        /// Creates a success Response with type T.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static Response<T> Create(T result)
        {
            return new Response<T>(result, null);
        }
        /// <summary>
        /// Creates a error Response.
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public static Response<T> Create(Error error)
        {
            return new Response<T>(default(T), error);
        }

        protected Response(T result, Error error)
        {
            this.Result = result;
            this.Error = error;
        }
    }
    [DataContract]
    public class Error
    {
        [DataMember(IsRequired = true)]
        public int Code { get; set; }
        [DataMember(IsRequired = false)]
        public string Message { get; set; }
        [IgnoreDataMember]
        public object Exception { get; set; }

        public Error() { }

        public static Error Create()
        {
            return new Error(CustomErrorType.Unknown, null);
        }
        public static Error Create(CustomErrorType errorType)
        {
            return new Error(errorType, null);
        }
        public static Error Create(CustomErrorType errorType, Exception ex)
        {
            return new Error(errorType, ex);
        }
        public static Error Create(Exception ex)
        {
            return new Error(CustomErrorType.Unknown, ex);
        }
        public static Error Create(CustomException ex)
        {
            return new Error(ex.ErrorType, ex);
        }
        protected Error(CustomErrorType errorType, Exception ex)
        {
            this.Code = (int)errorType;
            this.Message = GetDescription(errorType);
            this.Exception = ex;
        }
        public static string GetDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
        }
    }
}
