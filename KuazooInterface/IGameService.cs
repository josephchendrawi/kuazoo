using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace com.kuazoo
{
    [ServiceContract]
    public interface IGameService
    {
        [OperationContract]
        Response<bool> CreateGame(Game Game);
        [OperationContract]
        Response<List<Game>> GetGameList(int PrizeId);
        [OperationContract]
        Response<bool> DeleteGame(int GameId);
    }

    [DataContract]
    public class Game
    {
        [DataMember]
        public int GameId { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string Instruction { get; set; }
        [DataMember]
        public double Latitude { get; set; }
        [DataMember]
        public double Longitude { get; set; }
        [DataMember]
        public DateTime ExpiryDate { get; set; }

        public int ImageId { get; set; }
        public string ImageName { get; set; }
        public string ImageUrl { get; set; }
        public string LastAction { get; set; }

        public int PrizeId { get; set; }
        public string PrizeName { get; set; }

        public DateTime Create { get; set; }
    }
    [DataContract]
    public class GamePrize
    {
        public int PrizeId { get; set; }
        public int Qty { get; set; }
        public int GameType { get; set; }
    }

    [DataContract]
    public class GameTransaction
    {
        [DataMember]
        public int GameId { get; set; }
        [DataMember]
        public int TransactionId { get; set; }
        [DataMember]
        public double UserLatitude { get; set; }
        [DataMember]
        public double UserLongitude { get; set; }
        [DataMember]
        public int TimeUsed { get; set; }
    }
    [DataContract]
    public class GameFinish
    {
        [DataMember]
        public int InventoryItemId { get; set; }
        [DataMember]
        public string PrizeName { get; set; }
        [DataMember]
        public string PrizeImageUrl { get; set; }
        public string OrderNo { get; set; }
    }
}
