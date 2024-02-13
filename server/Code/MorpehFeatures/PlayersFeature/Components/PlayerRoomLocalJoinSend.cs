using Scellecs.Morpeh;
using server.Code.MorpehFeatures.CurrencyFeature.Enums;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.NetworkModels;

namespace server.Code.MorpehFeatures.PlayersFeature.Components;

public struct PlayerRoomLocalJoinSend : IComponent
{
    public int RoomId;
    public byte MaxPlayers;
    public byte Seat;
    public CurrencyType BalanceType;
    public ulong ContributionBalance;
    public ulong AllBalance;
    public List<RoomPlayerNetworkModel> RemotePlayers;
}