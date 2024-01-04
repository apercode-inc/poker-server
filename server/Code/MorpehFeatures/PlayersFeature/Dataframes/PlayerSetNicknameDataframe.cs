using NetFrame;
using NetFrame.WriteAndRead;

namespace chess_server.Code.MorpehFeature.PlayersFeature.Dataframes;

public struct PlayerSetNicknameDataframe : INetworkDataframe
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