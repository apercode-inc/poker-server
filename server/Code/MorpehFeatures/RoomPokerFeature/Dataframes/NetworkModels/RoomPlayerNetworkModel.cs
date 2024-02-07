using NetFrame;
using NetFrame.WriteAndRead;
using server.Code.MorpehFeatures.PokerFeature.Dataframes.NetworkModels;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.NetworkModels;

public struct RoomPlayerNetworkModel : IWriteable, IReadable
{
    public int Id;
    public string Nickname;
    public byte Seat;
    public bool IsDealer;
    public bool IsCards;
    public ulong Balance;
     
    public void Write(NetFrameWriter writer)
    {
        writer.WriteInt(Id);
        writer.WriteString(Nickname);
        writer.WriteByte(Seat);
        writer.WriteBool(IsDealer);
        writer.WriteBool(IsCards);
        writer.WriteULong(Balance);
    }

    public void Read(NetFrameReader reader)
    {
        Id = reader.ReadInt();
        Nickname = reader.ReadString();
        Seat = reader.ReadByte();
        IsDealer = reader.ReadBool();
        IsCards = reader.ReadBool();
        Balance = reader.ReadULong();
    }
}