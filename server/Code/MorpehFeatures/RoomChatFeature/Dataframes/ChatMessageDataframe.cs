using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.RoomChatFeature.Dataframes;

public struct ChatMessageDataframe : INetworkDataframe
{
    public int Timestamp;
    public string Nickname;
    public string Text;
        
    public void Write(NetFrameWriter writer)
    {
        writer.WriteInt(Timestamp);
        writer.WriteString(Nickname);
        writer.WriteString(Text);
    }

    public void Read(NetFrameReader reader)
    {
        Timestamp = reader.ReadInt();
        Nickname = reader.ReadString();
        Text = reader.ReadString();
    }
}