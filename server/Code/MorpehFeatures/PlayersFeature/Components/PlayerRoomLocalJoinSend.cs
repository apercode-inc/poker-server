using Scellecs.Morpeh;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.NetworkModels;

namespace server.Code.MorpehFeatures.PlayersFeature.Components;

public struct PlayerRoomLocalJoinSend : IComponent
{
    public int RoomId;
    public byte MaxPlayers;
    public byte Seat;
    public List<RoomPlayerNetworkModel> RemotePlayers;
    public int WaitTime;
}