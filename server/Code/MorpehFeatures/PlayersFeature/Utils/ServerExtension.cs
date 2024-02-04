using Scellecs.Morpeh;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;

namespace NetFrame.Server;

public static class ServerExtension
{
    public static void Send<T>(this NetFrameServer server, ref T dataframe, Entity playerEntity) where T : struct, INetworkDataframe
    {
        ref var playerId = ref playerEntity.GetComponent<PlayerId>();
        server.Send(ref dataframe, playerId.Id);
    }

    public static void SendInRoom<T>(this NetFrameServer server, ref T dataframe, Entity roomEntity) where T : struct, INetworkDataframe
    {
        ref var roomPokerPlayers = ref roomEntity.GetComponent<RoomPokerPlayers>();
        
        foreach (var playerBySeat in roomPokerPlayers.MarkedPlayersBySeat)
        {
            server.Send(ref dataframe, playerBySeat.Value);
        }
    }
}