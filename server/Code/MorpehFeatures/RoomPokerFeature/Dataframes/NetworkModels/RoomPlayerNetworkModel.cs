using NetFrame;
using NetFrame.WriteAndRead;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.NetworkModels;

public struct RoomPlayerNetworkModel : IWriteable, IReadable
{
    public int Id;
    public string Nickname;
    public byte Seat;
    public bool IsDealer;
    public ulong ContributionBalance;
    public ulong AllBalance;
    public CardsState CardsState;
    public List<RoomPokerCardNetworkModel> CardsModel;

    public void Write(NetFrameWriter writer)
    {
        writer.WriteInt(Id);
        writer.WriteString(Nickname);
        writer.WriteByte(Seat);
        writer.WriteBool(IsDealer);
        writer.WriteULong(ContributionBalance);
        writer.WriteULong(AllBalance);
        writer.WriteByte((byte) CardsState);
        
        writer.WriteInt(CardsModel?.Count ?? 0);

        if (CardsModel != null)
        {
            foreach (var user in CardsModel)
            {
                writer.Write(user);
            }
        }
    }

    public void Read(NetFrameReader reader)
    {
        Id = reader.ReadInt();
        Nickname = reader.ReadString();
        Seat = reader.ReadByte();
        IsDealer = reader.ReadBool();
        ContributionBalance = reader.ReadULong();
        AllBalance = reader.ReadULong();
        CardsState = (CardsState)reader.ReadByte();
        
        var count = reader.ReadInt();

        if (count > 0)
        {
            CardsModel = new List<RoomPokerCardNetworkModel>();
            for (var i = 0; i < count; i++)
            {
                CardsModel.Add(reader.Read<RoomPokerCardNetworkModel>());
            }
        }
    }
}