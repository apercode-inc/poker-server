using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.PlayersFeature.Dataframes;

public struct PlayerInitializeDataframe : INetworkDataframe
{
    public string Nickname;
    public string AvatarUrl;
    public int AvatarIndex;
    public int Level;
    public int Experience;
    public string PlayerGuid;
    
    public void Write(NetFrameWriter writer)
    {
        writer.WriteString(Nickname);
        writer.WriteString(AvatarUrl);
        writer.WriteInt(AvatarIndex);
        writer.WriteInt(Level);
        writer.WriteInt(Experience);
        writer.WriteString(PlayerGuid);
    }

    public void Read(NetFrameReader reader)
    {
        Nickname = reader.ReadString();
        AvatarUrl = reader.ReadString();
        AvatarIndex = reader.ReadInt();
        Level = reader.ReadInt();
        Experience = reader.ReadInt();
        PlayerGuid = reader.ReadString();
    }
}