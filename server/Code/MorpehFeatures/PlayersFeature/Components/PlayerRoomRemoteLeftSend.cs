using Scellecs.Morpeh;

namespace server.Code.MorpehFeatures.PlayersFeature.Components;

public struct PlayerRoomRemoteLeftSend : IComponent
{
    public int RoomId;
    public int PlayerId;
}