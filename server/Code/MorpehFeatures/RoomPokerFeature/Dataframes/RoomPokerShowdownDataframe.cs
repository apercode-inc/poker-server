using NetFrame;
using NetFrame.WriteAndRead;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.NetworkModels;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;

public struct RoomPokerShowdownDataframe : INetworkDataframe
{
    public List<RoomPokerShowdownNetworkModel> ShowdownModels;

    public void Write(NetFrameWriter writer)
    {
        var hasModels = ShowdownModels != null;
        writer.WriteBool(hasModels);

        if (hasModels)
        {
            writer.WriteInt(ShowdownModels.Count);

            foreach (var user in ShowdownModels)
            {
                writer.Write(user);
            }
        }
    }

    public void Read(NetFrameReader reader)
    {
        if (reader.ReadBool())
        {
            var count = reader.ReadInt();
            ShowdownModels = new List<RoomPokerShowdownNetworkModel>();

            for (var i = 0; i < count; i++)
            {
                ShowdownModels.Add(reader.Read<RoomPokerShowdownNetworkModel>());
            }
        }
    }
}