using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.AuthenticationFeature.Dataframes
{
    public struct AuthenticationDataframe : INetworkDataframe
    {
        public string UserId;
        
        public void Write(NetFrameWriter writer)
        {
            writer.WriteString(UserId);
        }

        public void Read(NetFrameReader reader)
        {
            UserId = reader.ReadString();
        }
    }
}