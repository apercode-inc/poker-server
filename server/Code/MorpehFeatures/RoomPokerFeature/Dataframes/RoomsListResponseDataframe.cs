using NetFrame;
using NetFrame.WriteAndRead;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.NetworkModels;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;

public struct RoomsListResponseDataframe : INetworkDataframe
{
    public List<RoomNetworkModel> Rooms;

    public void Write(NetFrameWriter writer)
    {
        writer.WriteInt(Rooms?.Count ?? 0);

        if (Rooms != null)
        {
            foreach (var room in Rooms)
            {
                writer.Write(room);
            }
        }
    }

    public void Read(NetFrameReader reader)
    {
        var count = reader.ReadInt();

        if (count > 0)
        {
            Rooms = new List<RoomNetworkModel>();
            for (var i = 0; i < count; i++)
            {
                Rooms.Add(reader.Read<RoomNetworkModel>());
            }
        }
    }
}