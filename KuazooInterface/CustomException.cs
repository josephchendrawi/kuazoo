using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Web.Security;
using System.Data;
using System.Reflection;
using System.ComponentModel;


namespace com.kuazoo
{
    public class CustomException : Exception
    {
        private CustomErrorType errorType = CustomErrorType.Unknown;
        public CustomErrorType ErrorType
        {
            get
            {
                return errorType;
            }
        }
        /// <summary>
        /// Create exception with an explicit exception type.
        /// </summary>
        /// <param name="exceptionType"></param>
        public CustomException(CustomErrorType errorType)
            : base(GetDescription(errorType))
        {
            this.errorType = errorType;
        }
        public CustomException(CustomErrorType errorType, string message)
            : base(GetDescription(errorType) + " | " + message)
        {
            this.errorType = errorType;
        }

        public CustomException(CustomErrorType errorType, string message, Exception innerException)
            : base(GetDescription(errorType) + " | " + message, innerException)
        {
            this.errorType = errorType;
        }

        public static CustomException Create(CustomException ex)
        {
            // no wrapping for existing CustomException
            return ex;
        }
        public static CustomException Create(Exception ex)
        {
            if (ex is CustomException)
            {
                return Create(ex as CustomException);
            }
            else if (ex is MembershipCreateUserException)
            {
                return Create(ex as MembershipCreateUserException);
            }
            else if (ex is DataException)
            {
                return Create(ex as DataException);
            }
            return new CustomException(CustomErrorType.Unknown, ex.Message, ex);
        }
        public static CustomException Create(MembershipCreateUserException ex)
        {
            CustomErrorType errorType = ResolveCustomErrorType(ex.StatusCode);
            return new CustomException(errorType);
        }
        public static CustomException Create(DataException ex)
        {
            return new CustomException(CustomErrorType.Data, ex.Message);
        }
        public static CustomErrorType ResolveCustomErrorType(MembershipCreateStatus status)
        {
            switch (status)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return CustomErrorType.UserServiceDuplicateUserName;

                case MembershipCreateStatus.DuplicateEmail:
                    return CustomErrorType.UserServiceDuplicateEmail;

                case MembershipCreateStatus.InvalidPassword:
                    return CustomErrorType.UserServiceInvalidPassword;

                case MembershipCreateStatus.InvalidEmail:
                    return CustomErrorType.UserServiceInvalidEmail;

                case MembershipCreateStatus.InvalidAnswer:
                    return CustomErrorType.UserServiceInvalidAnswer;

                case MembershipCreateStatus.InvalidQuestion:
                    return CustomErrorType.UserServiceInvalidQuestion;

                case MembershipCreateStatus.InvalidUserName:
                    return CustomErrorType.UserServiceInvalidUserName;

                case MembershipCreateStatus.ProviderError:
                    return CustomErrorType.UserServiceProviderError;

                default:
                    return CustomErrorType.UserServiceUnknown;
            }
        }
        public static string GetDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
        }
    }
}
