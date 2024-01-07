using Scellecs.Morpeh;
using server.Code.MorpehFeatures.PlayersFeature.Components;

namespace NetFrame.Server;

public static class ServerExtension
{
    public static void Send<T>(this NetFrameServer server, ref T dataframe, Entity playerEntity)  where T : struct, INetworkDataframe
    {
        ref var playerId = ref playerEntity.GetComponent<PlayerId>();
        server.Send(ref dataframe, playerId.Id);
    }
}