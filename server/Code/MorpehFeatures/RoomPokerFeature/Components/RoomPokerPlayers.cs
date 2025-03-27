using Scellecs.Morpeh;
using server.Code.MorpehFeatures.RoomPokerFeature.Models;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Components;

public struct RoomPokerPlayers : IComponent
{
    public int TotalPlayersCount;
    public int DealerSeatPointer;
    public int MoverSeatPointer;
    public PlayerSeatModel[] PlayersBySeat;
    public List<PlayerPotModel> PlayerPotModels;
}

public class PlayerSeatModel
{
    public Entity Player;
    public bool IsOccupied;
}