using Scellecs.Morpeh;
using Scellecs.Morpeh.Collections;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Components;
using server.Code.MorpehFeatures.RoomPokerFeature.Dataframes.NetworkModels;

namespace server.Code.MorpehFeatures.RoomPokerFeature.Systems;

public class RoomPokerPlayerJoinSystem : ISystem
{
    [Injectable] private Stash<RoomPokerId> _roomPokerId;
    [Injectable] private Stash<RoomPokerPlayerJoin> _roomPokerPlayerJoin;
    [Injectable] private Stash<RoomPokerStats> _roomPokerStats;
    [Injectable] private Stash<RoomPokerPlayers> _roomPokerPlayers;
    
    [Injectable] private Stash<PlayerRoomPoker> _playerRoomPoker;
    [Injectable] private Stash<PlayerRoomRemoteJoinSend> _playerRoomRemoteJoinSend;
    [Injectable] private Stash<PlayerRoomLocalJoinSend> _playerRoomLocalJoinSend;
    [Injectable] private Stash<PlayerDealer> _playerDealer;
    [Injectable] private Stash<PlayerCards> _playerCards;

    [Injectable] private Stash<PlayerNickname> _playerNickname;
    [Injectable] private Stash<PlayerId> _playerId;

    private Random _random;
    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _random = new Random();
        
        _filter = World.Filter
            .With<RoomPokerPlayerJoin>()
            .With<RoomPokerStats>()
            .With<RoomPokerPlayers>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var roomEntity in _filter)
        {
            ref var roomPokerStats = ref _roomPokerStats.Get(roomEntity);
            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(roomEntity);
            ref var roomPokerPlayerJoin = ref _roomPokerPlayerJoin.Get(roomEntity);
            
            var joinPlayerEntity = roomPokerPlayerJoin.Player;
            
            _roomPokerPlayerJoin.Remove(roomEntity);

            if (_playerRoomPoker.Has(joinPlayerEntity))
            {
                continue;
            }

            if (roomPokerStats.MaxPlayers == roomPokerPlayers.MarkedPlayersBySeat.Count)
            {
                continue;
            }

            if (roomPokerPlayers.MarkedPlayersBySeat.ContainsValue(joinPlayerEntity))
            {
                continue;
            }
            
            ref var roomPokerId = ref _roomPokerId.Get(roomEntity);

            var freeSeats = new FastList<byte>();

            for (byte index = 0; index < roomPokerStats.MaxPlayers; index++)
            {
                if (!roomPokerPlayers.MarkedPlayersBySeat.ContainsKey(index))
                {
                    freeSeats.Add(index);
                }
            }

            var randomIndex = _random.Next(0, freeSeats.length);
            var seatIndex = freeSeats.data[randomIndex];
            
            roomPokerPlayers.MarkedPlayersBySeat.Add(seatIndex, joinPlayerEntity);
            
            ref var playerId = ref _playerId.Get(joinPlayerEntity);
            ref var playerNickname = ref _playerNickname.Get(joinPlayerEntity);

            _playerRoomPoker.Set(joinPlayerEntity, new PlayerRoomPoker
            {
                RoomId = roomPokerId.Value,
            });

            var playersNetworkModels = new List<RoomPlayerNetworkModel>();

            foreach (var playerBySeat in roomPokerPlayers.MarkedPlayersBySeat)
            {
                var otherSeat = playerBySeat.Key;
                var otherPlayer = playerBySeat.Value;

                ref var otherPlayerNickname = ref _playerNickname.Get(otherPlayer);
                ref var otherPlayerId = ref _playerId.Get(otherPlayer);
                
                if (otherPlayer == joinPlayerEntity)
                {
                    continue;
                }
                
                _playerRoomRemoteJoinSend.Set(otherPlayer, new PlayerRoomRemoteJoinSend
                {
                    RoomId = roomPokerId.Value,
                    RemotePlayer = new RoomPlayerNetworkModel
                    {
                        Id = playerId.Id,
                        Nickname = playerNickname.Value,
                        Seat = seatIndex,
                        IsDealer = _playerDealer.Has(joinPlayerEntity),
                        IsCards = _playerCards.Has(joinPlayerEntity),
                    }
                });

                playersNetworkModels.Add(new RoomPlayerNetworkModel
                {
                    Id = otherPlayerId.Id,
                    Nickname = otherPlayerNickname.Value,
                    Seat = (byte) otherSeat,
                    IsDealer = _playerDealer.Has(otherPlayer),
                    IsCards = _playerCards.Has(otherPlayer),
                });
            }

            _playerRoomLocalJoinSend.Set(joinPlayerEntity, new PlayerRoomLocalJoinSend
            {
                RoomId = roomPokerId.Value,
                MaxPlayers = roomPokerStats.MaxPlayers,
                Seat = seatIndex,
                RemotePlayers = playersNetworkModels,
            });
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}