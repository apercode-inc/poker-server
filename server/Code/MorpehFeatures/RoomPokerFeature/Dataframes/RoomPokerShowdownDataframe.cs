using NetFrame;
using NetFrame.WriteAndRead;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.NetworkModels;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes;

public struct RoomPokerShowdownDataframe : INetworkDataframe
{
    public bool IsBankSync;
    public long Bank;
    public List<RoomPokerShowdownNetworkModel> ShowdownModels;

    public void Write(NetFrameWriter writer)
    {
        writer.WriteBool(IsBankSync);

        if (IsBankSync)
        {
            writer.WriteLong(Bank);
        }
        
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
        IsBankSync = reader.ReadBool();

        if (IsBankSync)
        {
            Bank = reader.ReadLong();
        }
        
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