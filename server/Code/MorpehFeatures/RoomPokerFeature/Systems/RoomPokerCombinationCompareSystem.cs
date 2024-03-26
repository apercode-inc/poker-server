using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerCombinationCompareSystem : ISystem
{
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<RoomPokerShowdown> _roomPokerShowdown;
    
    [Injectable] private Stash<PlayerPokerCombination> _playerPokerCombination;
    [Injectable] private Stash<PlayerNickname> _playerNickname; //todo test
    
    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<RoomPokerCardsToTable>()
            .With<RoomPokerPlayers>()
            .With<RoomPokerShowdown>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var roomEntity in _filter)
        {
            _roomPokerShowdown.Remove(roomEntity);
            
            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);
            
            foreach (var markedPlayer in roomPokerPlayers.MarkedPlayersBySeat)
            {
                var player = markedPlayer.Value;

                ref var playerPokerCombination = ref _playerPokerCombination.Get(player, out var combinationExist);
                
                if (!combinationExist)
                {
                    continue;
                }
                
                ref var playerNickname = ref _playerNickname.Get(player);
                
                Debug.LogColor($"playerNickname: {playerNickname.Value}", ConsoleColor.Yellow);
                Debug.LogColor($"{Debug.GetCardsLog(playerPokerCombination.CombinationCards, "^")}", ConsoleColor.DarkCyan);
                Debug.LogColor($"combination: {playerPokerCombination.CombinationType}", ConsoleColor.Blue);
                Console.WriteLine();
            }
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}