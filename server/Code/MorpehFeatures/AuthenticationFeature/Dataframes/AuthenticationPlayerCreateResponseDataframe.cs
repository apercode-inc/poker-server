using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.AuthenticationFeature.Dataframes;

public struct AuthenticationPlayerCreateResponseDataframe : INetworkDataframe
{
    public string UserId;
    public string Nickname;
    public int AvatarIndex;
        
    public void Write(NetFrameWriter writer)
    {
        writer.WriteString(UserId);
        writer.WriteString(Nickname);
        writer.WriteInt(AvatarIndex);
    }

    public void Read(NetFrameReader reader)
    {
        UserId = reader.ReadString();
        Nickname = reader.ReadString();
        AvatarIndex = reader.ReadInt();
    }
}