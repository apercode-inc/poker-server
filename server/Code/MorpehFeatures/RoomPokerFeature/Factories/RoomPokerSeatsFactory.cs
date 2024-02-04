using Scellecs.Morpeh;
using server.Code.GlobalUtils.CustomCollections;
using server.Code.MorpehFeatures.PokerFeature.Enums;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Factories;

public class RoomPokerSeatsFactory : IInitializer
{
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