using Scellecs.Morpeh;

namespace server.Code.MorpehFeatures.PlayersFeature.Components;

public struct PlayerRoomCreateSend : IComponent
{
    public int RoomId;
    public byte MaxPlayers;
    public byte Seat;
}