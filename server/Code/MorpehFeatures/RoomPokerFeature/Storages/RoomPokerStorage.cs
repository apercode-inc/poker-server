using Scellecs.Morpeh;
using Scellecs.Morpeh.Collections;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.CleanupDestroyFeature.Components;
using server.Code.MorpehFeatures.ConfigsFeature.Constants;
using server.Code.MorpehFeatures.ConfigsFeature.Services;
using server.Code.MorpehFeatures.CurrencyFeature.Enums;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Systems;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Configs;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;
using server.Code.MorpehFeatures.RoomPokerFeature.Factories;
using server.Code.MorpehFeatures.RoomPokerFeature.Models;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Storages;

public class RoomPokerStorage : IInitializer
{
    [Injectable] private Stash<RoomPokerId> _roomPokerId;
    [Injectable] private Stash<RoomPokerStats> _roomPokerStats;
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    [Injectable] private Stash<RoomPokerMaxBet> _roomPokerMaxBet;
    [Injectable] private Stash<RoomPokerCardsToTable> _roomPokerCardsToTable;
    [Injectable] private Stash<RoomPokerReadyDestroy> _roomPokerReadyDestroy;

    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;

    [Injectable] private RoomPokerSeatsFactory _pokerSeatsFactory;
    [Injectable] private PlayerStorage _playerStorage;
    [Injectable] private ConfigsService _configsService;
    
    private Dictionary<int, Entity> _rooms;
    private int _idCounter;
    private Random _random;

    private Filter _filter;

    public World World { get; set; }

    public void OnAwake()
    {
        _rooms = new Dictionary<int, Entity>();
        _random = new Random();
        
        _filter = World.Filter
            .With<RoomPokerId>()
            .Build();
    }
    
    public void CreateRoom(Entity createdPlayer, byte maxPlayers, CurrencyType currencyType, long contribution, 
        long minContribution, long bigBet, bool isFastTurn)
    {
        if (_playerRoomPoker.Has(createdPlayer))
        {
            Logger.Error($"[RoomPokerStorageSystem.Add] the player is already in the room", true);
            return;
        }
        
        var roomEntity = World.CreateEntity();
        var seat = (byte) _random.Next(0, maxPlayers);
        
        var config = _configsService.GetConfig<RoomPokerSettingsConfig>(ConfigsPath.RoomPokerSettings);
        var turnTime = isFastTurn ? config.PlayerTurnTimeFast : config.PlayerTurnTime;
        var turnShowdownTime = isFastTurn ? config.PlayerTurnShowdownTimeFast : config.PlayerTurnShowdownTime;

        _roomPokerId.Set(roomEntity, new RoomPokerId
        {
            Value = _idCounter,
        });
        _roomPokerStats.Set(roomEntity, new RoomPokerStats
        {
            MaxPlayers = maxPlayers,
            CurrencyType = currencyType,
            Contribution = contribution,
            MinContribution = minContribution,
            BigBet = bigBet,
            TurnTime = turnTime,
            TurnShowdownTime = turnShowdownTime,
        });

        var playersBySeatModels = new PlayerSeatModel[maxPlayers];

        for (var i = 0; i < playersBySeatModels.Length; i++)
        {
            playersBySeatModels[i] = new PlayerSeatModel();
        }
        
        var playerSeatModel = playersBySeatModels[seat];
        playerSeatModel.Player = createdPlayer;

        _roomPokerPlayers.Set(roomEntity, new RoomPokerPlayers
        {
            TotalPlayersCount = 1,
            DealerSeatPointer = 0,
            PlayersBySeat = playersBySeatModels,
            PlayerPotModels = new List<PlayerPotModel>(),
        });
        _roomPokerMaxBet.Set(roomEntity, new RoomPokerMaxBet
        {
            Value = bigBet / 2,
        });
        _roomPokerCardsToTable.Set(roomEntity, new RoomPokerCardsToTable
        {
            State = CardToTableState.PreFlop,
            Cards = new Queue<CardModel>(),
        });

        _playerStorage.CreateForRoomAndSync(createdPlayer, currencyType, contribution, roomEntity, seat);

        _rooms.Add(_idCounter, roomEntity);

        _idCounter++;
    }

    public bool TryGetById(int id, out Entity roomEntity)
    {
        if (_rooms.TryGetValue(id, out var value))
        {
            roomEntity = value;
            return true;
        }

        roomEntity = null;
        return false;
    }

    public void Remove(int id)
    {
        _rooms.Remove(id);
        
        foreach (var entity in _filter)
        {
            ref var roomPokerId = ref _roomPokerId.Get(entity);

            if (roomPokerId.Value != id)
            {
                continue;
            }
            
            _roomPokerReadyDestroy.Set(entity);
            break;
        }
    }

    public void Dispose()
    {
        _rooms = null;
        _filter = null;
        _random = null;
    }
}