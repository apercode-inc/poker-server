using NetFrame;
using NetFrame.WriteAndRead;
using server.Code.MorpehFeatures.NotificationFeature.Enums;

namespace server.Code.MorpehFeatures.NotificationFeature.Dataframes
{
    public struct NotificationDataframe : INetworkDataframe
    {
        public NotificationType Type;
        public string MessageText;

        public void Write(NetFrameWriter writer)
        {
            writer.WriteByte((byte) Type);
            writer.WriteString(MessageText);
        }

        public void Read(NetFrameReader reader)
        {
            Type = (NotificationType) reader.ReadByte();
            MessageText = reader.ReadString();
        }
    }
}