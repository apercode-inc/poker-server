using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.NetworkModels;

public struct RoomNetworkModel : IWriteable, IReadable
{
    public int Id;
    public byte CurrentPlayers;
    public byte MaxPlayers;
    public ulong SmallBet;
    public ulong BigBet;
    public List<RoomPlayerNetworkModel> Players;

    public void Write(NetFrameWriter writer)
    {
        writer.WriteInt(Id);
        writer.WriteByte(CurrentPlayers);
        writer.WriteByte(MaxPlayers);
        writer.WriteULong(SmallBet);
        writer.WriteULong(BigBet);
        
        writer.WriteInt(Players?.Count ?? 0);

        if (Players != null)
        {
            foreach (var player in Players)
            {
                writer.Write(player);
            }
        }
    }

    public void Read(NetFrameReader reader)
    {
        Id = reader.ReadInt();
        CurrentPlayers = reader.ReadByte();
        MaxPlayers = reader.ReadByte();
        SmallBet = reader.ReadULong();
        BigBet = reader.ReadULong();
        
        var count = reader.ReadInt();
        
        if (count > 0)
        {
            Players = new List<RoomPlayerNetworkModel>();
            for (var i = 0; i < count; i++)
            {
                Players.Add(reader.Read<RoomPlayerNetworkModel>());
            }
        }
    }
}