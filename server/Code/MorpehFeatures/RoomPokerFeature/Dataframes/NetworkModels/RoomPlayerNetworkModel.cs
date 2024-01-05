using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.NetworkModels;

public struct RoomPlayerNetworkModel : IWriteable, IReadable
{
    public string Nickname;
    
    public void Write(NetFrameWriter writer)
    {
        writer.WriteString(Nickname);
    }

    public void Read(NetFrameReader reader)
    {
        Nickname = reader.ReadString();
    }
}