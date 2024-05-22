using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.RoomChatFeature.Dataframes;

public struct ChatMessageDataframe : INetworkDataframe
{
    public int SenderId;
    public int Timestamp;
    public string Text;
        
    public void Write(NetFrameWriter writer)
    {
        writer.WriteInt(SenderId);
        writer.WriteInt(Timestamp);
        writer.WriteString(Text);
    }

    public void Read(NetFrameReader reader)
    {
        SenderId = reader.ReadInt();
        Timestamp = reader.ReadInt();
        Text = reader.ReadString();
    }
}