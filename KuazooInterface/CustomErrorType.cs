using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;

namespace com.kuazoo
{
    public class LocalizedEnumAttribute : DescriptionAttribute
    {

        private PropertyInfo _nameProperty;
        private Type _resourceType;

        public LocalizedEnumAttribute(string displayNameKey)
            : base(displayNameKey)
        {

        }

        public Type NameResourceType
        {
            get
            {
                return _resourceType;
            }
            set
            {
                _resourceType = value;

                _nameProperty = _resourceType.GetProperty(this.Description, BindingFlags.Static | BindingFlags.Public);
            }
        }

        public override string Description
        {
            get
            {
                //check if nameProperty is null and return original display name value
                if (_nameProperty == null)
                {
                    return base.Description;
                }

                return (string)_nameProperty.GetValue(_nameProperty.DeclaringType, null);
            }
        }
    }

    public static class EnumExtender
    {
        public static string GetLocalizedDescription(this Enum @enum)
        {
            if (@enum == null)
                return null;

            string description = @enum.ToString();

            FieldInfo fieldInfo = @enum.GetType().GetField(description);
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Any())
                return attributes[0].Description;

            return description;
        }
    }
    public enum CustomErrorType
    {

        [Description("Unknown error.")]
        Unknown = 0,
        [Description("Database or entity error.")]
        Data = 1,
        [Description("Unauthorized access.")]
        Unauthorized = 2,
        /// <summary>
        /// User is not authenticated thus not allowed to perform operation.
        /// </summary>
        [Description("Unauthorized access.")]
        Unauthenticated = 3,
        /// <summary>
        /// Inconsistency in objects due to earlier operation errors. 
        /// Inconsistent error causes business logic error and thus will have limitation in future operations too.
        /// These errors should be logged as serious exception and should be reported to be fixed by administrators.
        /// </summary>
        [Description("Unauthorized access.")]
        ObjectInconsistency = 4,
        [Description("Invalid argument(s).")]
        InvalidArguments = 5,
        [Description("Missing required property(s).")]
        MissingRequiredProperty = 6,


        [Description("Admin service error.")]
        AdminUnknown = 100,
        [Description("Admin token invalid.")]
        AdminNotFound = 101,
        [Description("Admin already assign.")]
        AdminAlreadyAssign = 102,
        [Description("Admin failed.")]
        AdminFailed = 103,
        [Description("Admin failed delete.")]
        AdminFailedDelete = 104,
        [Description("Email or password wrong.")]
        AdminEmailPasswordWrong = 105,
        [Description("Password wrong.")]
        AdminPasswordWrong = 106,

        [Description("General user service error.")]
        UserServiceUnknown = 1000,
        [Description("Duplicate username.")]
        UserServiceDuplicateUserName = 1001,
        [Description("Invalid password.")]
        UserServiceInvalidPassword = 1002,
        [Description("Duplicate email.")]
        UserServiceDuplicateEmail = 1003,
        [Description("Invalid email.")]
        UserServiceInvalidEmail = 1004,
        [Description("Invalid answer.")]
        UserServiceInvalidAnswer = 1005,
        [Description("Invalid question.")]
        UserServiceInvalidQuestion = 1006,
        [Description("Invalid Username.")]
        UserServiceInvalidUserName = 1007,
        [Description("Provider error.")]
        UserServiceProviderError = 1008,
        [Description("Invalid username or password.")]
        UserServiceInvalidUsernamePassword = 1009,
        [Description("Email address is already taken. Please sign in.")]
        UserServiceAlreadyAssign = 1010,
        [Description("User failed.")]
        UserServiceFailed = 1011,
        [Description("User failed delete.")]
        UserServiceFailedDelete = 1012,
        [Description("User not found.")]
        UserServiceNotFound = 1013,
        [Description("Email or password wrong.")]
        UserServiceEmailPasswordWrong = 1014,
        [Description("Facebook ID exists. Try login instead.")]
        UserServiceFacebookIdExists = 1015,
        [Description("Facebook ID not found")]
        UserServiceFacebookIdNotFound = 1016,
        [Description("The Kuazoo account already have a linked Facebook account")]
        UserServiceLinkFacebookAccFailed = 1017,
        [Description("The facebook friend is already added into friends list")]
        UserServiceAddSameFbFriend = 1018,
        [Description("Account linking failed.")]
        UserServiceLinkError = 1019,
        [Description("Kuazoo can only be linked to Facebook account which uses the same email address.")]
        UserServiceFBLinkDifferentEmail = 1025,
        [Description("Password incorrect.")]
        UserServicePasswordWrong = 1026,


        [Description("Merchant service error.")]
        MerchantUnknown = 2000,
        [Description("Merchant token invalid.")]
        MerchantNotFound = 2001,
        [Description("Merchant already assign.")]
        MerchantAlreadyAssign = 2002,
        [Description("Merchant failed.")]
        MerchantFailed = 2003,
        [Description("Merchant failed delete.")]
        MerchantFailedDelete = 2004,


        [Description("InventoryItem service error.")]
        InventoryItemServiceUnknown = 3000,
        [Description("InventoryItem not found.")]
        InventoryItemNotFound = 3001,
        [Description("InventoryItem already assign.")]
        InventoryItemAlreadyAssign = 3002,
        [Description("InventoryItem failed.")]
        InventoryItemFailed = 3003,
        [Description("InventoryItem failed delete.")]
        InventoryItemFailedDelete = 3004,


        [Description("InventoryItemTag service error.")]
        InventoryItemTagServiceUnknown = 4000,
        [Description("InventoryItemTag not found.")]
        InventoryItemTagNotFound = 4001,
        [Description("InventoryItemTag already assign.")]
        InventoryItemTagAlreadyAssign = 4002,
        [Description("InventoryItemTag failed.")]
        InventoryItemTagFailed = 4003,
        [Description("InventoryItemTag failed delete.")]
        InventoryItemTagFailedDelete = 4004,

        [Description("Tag service error.")]
        TagServiceUnknown = 5000,
        [Description("Tag not found.")]
        TagNotFound = 5001,
        [Description("Tag already assign.")]
        TagAlreadyAssign = 5002,
        [Description("Tag failed.")]
        TagFailed = 5003,
        [Description("Tag failed delete.")]
        TagFailedDelete = 5004,

        [Description("FlashDeal service error.")]
        FlashDealServiceUnknown = 6000,
        [Description("FlashDeal not found.")]
        FlashDealNotFound = 6001,
        [Description("FlashDeal already assign.")]
        FlashDealAlreadyAssign = 6002,
        [Description("FlashDeal failed.")]
        FlashDealFailed = 6003,
        [Description("FlashDeal failed delete.")]
        FlashDealFailedDelete = 6004,

        [Description("Member service error.")]
        MemberServiceUnknown = 7000,
        [Description("Member not found.")]
        MemberNotFound = 7001,
        [Description("Member already assign.")]
        MemberAlreadyAssign = 7002,
        [Description("Member failed.")]
        MemberFailed = 7003,
        [Description("Member failed delete.")]
        MemberFailedDelete = 7004,

        [Description("Image service error.")]
        ImageServiceUnknown = 8000,
        [Description("Image not found.")]
        ImageNotFound = 8001,
        [Description("Image already assign.")]
        ImageAlreadyAssign = 8002,
        [Description("Image failed.")]
        ImageFailed = 8003,
        [Description("Image failed delete.")]
        ImageFailedDelete = 8004,

        [Description("Transaction service error.")]
        TransactionServiceUnknown = 9000,
        [Description("Transaction not found.")]
        TransactionNotFound = 9001,
        [Description("Transaction already assign.")]
        TransactionAlreadyAssign = 9002,
        [Description("Transaction failed.")]
        TransactionFailed = 9003,
        [Description("Transaction failed delete.")]
        TransactionFailedDelete = 9004,

        [Description("Review service error.")]
        ReviewServiceUnknown = 10000,
        [Description("Review not found.")]
        ReviewNotFound = 10001,
        [Description("Review already assign.")]
        ReviewAlreadyAssign = 10002,
        [Description("Review failed.")]
        ReviewFailed = 10003,
        [Description("Review failed delete.")]
        ReviewFailedDelete = 10004,

        [Description("Prize service error.")]
        PrizeServiceUnknown = 11000,
        [Description("Prize not found.")]
        PrizeNotFound = 11001,
        [Description("Prize already assign.")]
        PrizeAlreadyAssign = 11002,
        [Description("Prize failed.")]
        PrizeFailed = 11003,
        [Description("Prize failed delete.")]
        PrizeFailedDelete = 11004,
        [Description("Prize still in used.")]
        PrizeStillInUsed = 11005,
        [Description("Winner already assign.")]
        PrizeWinnerAlreadyAssign = 11006,

        [Description("Wishlist service error.")]
        WishlistServiceUnknown = 12000,
        [Description("Wishlist not found.")]
        WishlistNotFound = 12001,
        [Description("Item has been added to Wishlist.")]
        WishlistAlreadyAssign = 12002,
        [Description("Wishlist failed.")]
        WishlistFailed = 12003,
        [Description("Wishlist failed delete.")]
        WishlistFailedDelete = 12004,

        [Description("Promotion service error.")]
        PromotionServiceUnknown = 12000,
        [Description("Promotion not found.")]
        PromotionNotFound = 12001,
        [Description("Promotion already assign.")]
        PromotionAlreadyAssign = 12002,
        [Description("Promotion failed.")]
        PromotionFailed = 12003,
        [Description("Promotion failed delete.")]
        PromotionFailedDelete = 12004,

        [Description("Point service error.")]
        PointServiceUnknown = 13000,
        [Description("Point not found.")]
        PointNotFound = 13001,
        [Description("Point already assign.")]
        PointAlreadyAssign = 13002,
        [Description("Point failed.")]
        PointFailed = 13003,
        [Description("Point failed delete.")]
        PointFailedDelete = 13004,

        [Description("Static service error.")]
        StaticServiceUnknown = 14000,
        [Description("Static not found.")]
        StaticNotFound = 14001,
        [Description("Static already assign.")]
        StaticAlreadyAssign = 14002,
        [Description("Static failed.")]
        StaticFailed = 14003,
        [Description("Static failed delete.")]
        StaticFailedDelete = 14004,

        [Description("Country service error.")]
        CountryServiceUnknown = 15000,
        [Description("Country not found.")]
        CountryNotFound = 15001,
        [Description("Country already assign.")]
        CountryAlreadyAssign = 15002,
        [Description("Country failed.")]
        CountryFailed = 15003,
        [Description("Country failed delete.")]
        CountryFailedDelete = 15004,

        [Description("State service error.")]
        StateServiceUnknown = 16000,
        [Description("State not found.")]
        StateNotFound = 16001,
        [Description("State already assign.")]
        StateAlreadyAssign = 16002,
        [Description("State failed.")]
        StateFailed = 16003,
        [Description("State failed delete.")]
        StateFailedDelete = 16004,

        [Description("City service error.")]
        CityServiceUnknown = 16000,
        [Description("City not found.")]
        CityNotFound = 16001,
        [Description("City already assign.")]
        CityAlreadyAssign = 16002,
        [Description("City failed.")]
        CityFailed = 16003,
        [Description("City failed delete.")]
        CityFailedDelete = 16004,

        [Description("Game service error.")]
        GameServiceUnknown = 17000,
        [Description("Game not found.")]
        GameNotFound = 17001,
        [Description("Game already assign.")]
        GameAlreadyAssign = 17002,
        [Description("Game failed.")]
        GameFailed = 17003,
        [Description("Game failed delete.")]
        GameFailedDelete = 17004,



        [Description("Banner service error.")]
        BannerUnknown = 18000,
        [Description("Banner token invalid.")]
        BannerNotFound = 18001,
        [Description("Banner already assign.")]
        BannerAlreadyAssign = 18002,
        [Description("Banner failed.")]
        BannerFailed = 18003,
        [Description("Banner failed delete.")]
        BannerFailedDelete = 18004,

        [Description("Blog/Video/Deals service error.")]
        BVDUnknown = 19000,
        [Description("Blog/Video/Deals token invalid.")]
        BVDNotFound = 19001,
        [Description("Blog/Video/Deals already assign.")]
        BVDAlreadyAssign = 19002,
        [Description("Blog/Video/Deals failed.")]
        BVDFailed = 19003,
        [Description("Blog/Video/Deals failed delete.")]
        BVDFailedDelete = 19004,


        [Description("FreeDeal service error.")]
        FreeDealUnknown = 18000,
        [Description("FreeDeal token invalid.")]
        FreeDealNotFound = 18001,
        [Description("FreeDeal already assign.")]
        FreeDealAlreadyAssign = 18002,
        [Description("FreeDeal failed.")]
        FreeDealFailed = 18003,
        [Description("FreeDeal failed delete.")]
        FreeDealFailedDelete = 18004,
    }
}
