using NetFrame;
using NetFrame.WriteAndRead;
using server.Code.MorpehFeatures.NotificationFeature.Enums;

namespace server.Code.MorpehFeatures.NotificationFeature.Dataframes
{
    public struct NotificationDataframe : INetworkDataframe
    {
        public NotificationKind Kind;
        public string MessageText;

        public void Write(NetFrameWriter writer)
        {
            writer.WriteByte((byte)Kind);
            writer.WriteString(MessageText);
        }

        public void Read(NetFrameReader reader)
        {
            Kind = (NotificationKind)reader.ReadByte();
            MessageText = reader.ReadString();
        }
    }
}