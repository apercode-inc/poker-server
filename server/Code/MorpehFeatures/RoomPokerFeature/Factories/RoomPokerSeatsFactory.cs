using Scellecs.Morpeh;
using server.Code.GlobalUtils.CustomCollections;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Factories;

public class RoomPokerSeatsFactory : IInitializer
{
    [Injectable] private Stash<PlayerSeat> _playerSeat;
    
    public World World { get; set; }

    public void OnAwake()
    {
    }

    public MovingMarkersDictionary<Entity, PokerPlayerMarkerType> Create(int size, int seat, Entity player)
    {
        var markedPlayersBySeat = new MovingMarkersDictionary<Entity, PokerPlayerMarkerType>(size)
        {
            {seat, player},
        };
        
        _playerSeat.Set(player, new PlayerSeat
        {
            SeatIndex = (byte) seat,
        });

        markedPlayersBySeat.SetSettingMarker(PokerPlayerMarkerType.DealerPlayer, MarkerSettingType.MoveWithRemoveForwardDirection, false);
        markedPlayersBySeat.SetSettingMarker(PokerPlayerMarkerType.ActivePlayer, MarkerSettingType.MoveWithRemoveForwardDirection, true);
        markedPlayersBySeat.SetSettingMarker(PokerPlayerMarkerType.DealerPlayer, MarkerSettingType.MoveForwardDirection, true);
        markedPlayersBySeat.SetSettingMarker(PokerPlayerMarkerType.ActivePlayer, MarkerSettingType.MoveForwardDirection, true);
        
        return markedPlayersBySeat;
    }

    public void Dispose()
    {
    }
}