using Scellecs.Morpeh;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.NetworkModels;

namespace server.Code.MorpehFeatures.PlayersFeature.Components;

public struct PlayerRoomRemoteJoinSend : IComponent
{
    public int RoomId;
    public RoomPlayerNetworkModel RemotePlayer;
}