using Scellecs.Morpeh;
using server.Code.MorpehFeatures.RoomPokerFeature.Models;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Components;

public struct RoomPokerPlayers : IComponent
{
    public int TotalPlayersCount;
    public int DealerSeatPointer;
    public int MoverSeatPointer;
    public Entity[] PlayersBySeat;
    public List<PlayerPotModel> PlayerPotModels;
}