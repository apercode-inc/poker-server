using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.GlobalUtils;
using server.Code.Injection;
using server.Code.MorpehFeatures.CleanupDestroyFeature.Components;
using server.Code.MorpehFeatures.CurrencyFeature.Enums;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Enums;
using server.Code.MorpehFeatures.RoomPokerFeature.Models;

namespace server.Code.MorpehFeatures.PlayersFeature.Systems;

public class PlayerStorage : IInitializer
{
    [Injectable] private Stash<PlayerId> _playerId;
    [Injectable] private Stash<Destroy> _destroy;
    [Injectable] private Stash<PlayerDbModelRequest> _playerDbModelRequest;
    [Injectable] private Stash<PlayerAuthData> _playerAuthData;

    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;
    [Injectable] private Stash<PlayerRoomCreateSend> _playerRoomCreateSend;
    [Injectable] private Stash<PlayerPokerContribution> _playerPokerContribution;
    [Injectable] private Stash<PlayerPokerCurrentBet> _playerPokerCurrentBet;
    [Injectable] private Stash<PlayerCards> _playerCards;
    [Injectable] private Stash<PlayerSeat> _playerSeat;
    
    [Injectable] private NetFrameServer _server;

    private Filter _filter;

    private Dictionary<int, Entity> _playersByIds;
    private Dictionary<string, Entity> _playerByGuids;

    public World World { get; set; }

    public void OnAwake()
    {
        _playersByIds = new Dictionary<int, Entity>();
        _playerByGuids = new Dictionary<string, Entity>();
        
        _filter = World.Filter
            .With<PlayerId>()
            .Build();
    }

    public void Add(int id)
    {
        var newEntity = World.CreateEntity();
        _playerId.Set(newEntity, new PlayerId
        {
            Id = id,
        });
        
        _playersByIds.Add(id, newEntity);
    }

    public void AddAuth(Entity player, string guid, int playerId)
    {
        //
        Logger.Debug($"guid = {guid} playerId = {playerId}", ConsoleColor.Red);
        
        if (_playerByGuids.TryGetValue(guid, out var existingPlayer))
        {
            Logger.Debug("Предыдущего игрока отключаем", ConsoleColor.Red);
            ref var existingPlayerId = ref _playerId.Get(existingPlayer);
            _server.Disconnect(existingPlayerId.Id);
            //Remove(existingPlayerId.Id);
        }
        
        Logger.Debug("Новый Игрок записан", ConsoleColor.Red);
        _playerAuthData.Set(player, new PlayerAuthData
        {
            Guid = guid,
        });
        _playerByGuids.Add(guid, player);
    }
    
    public void CreateForRoomAndSync(Entity createdPlayer, CurrencyType currencyType, long contribution,
        Entity roomEntity, byte seat)
    {
        _playerRoomPoker.Set(createdPlayer, new PlayerRoomPoker
        {
            RoomEntity = roomEntity,
        });
        _playerSeat.Set(createdPlayer, new PlayerSeat
        {
            SeatIndex = seat,
        });
        _playerPokerContribution.Set(createdPlayer, new PlayerPokerContribution
        {
            CurrencyType = currencyType,
            Value = contribution,
        });
        _playerPokerCurrentBet.Set(createdPlayer);
        _playerCards.Set(createdPlayer, new PlayerCards
        {
            Cards = new Queue<CardModel>(),
            CardsState = CardsState.Empty,
        });
        _playerRoomCreateSend.Set(createdPlayer);
    }

    public void Remove(int id)
    {
        _playersByIds.Remove(id);
        
        foreach (var entity in _filter)
        {
            ref var playerId = ref _playerId.Get(entity);
            ref var playerAuthData = ref _playerAuthData.Get(entity, out var exist);

            if (exist)
            {
                _playerByGuids.Remove(playerAuthData.Guid);
            }

            if (playerId.Id == id)
            {
                _destroy.Set(entity);
                break;
            }
        }
    }

    public bool TryGetPlayerById(int id, out Entity player)
    {
        if (_playersByIds.TryGetValue(id, out var value))
        {
            player = value;
            return true;
        }

        player = null;
        return false;
    }

    public void Dispose()
    {
        _filter = null;
    }
}