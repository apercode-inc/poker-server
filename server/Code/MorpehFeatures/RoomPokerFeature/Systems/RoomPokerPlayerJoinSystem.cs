using Scellecs.Morpeh;
using server.Code.GlobalUtils;
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

    [Injectable] private Stash<PlayerNickname> _playerNickname;
    [Injectable] private Stash<PlayerId> _playerId;

    private Filter _filter;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<RoomPokerPlayerJoin>()
            .With<RoomPokerStats>()
            .With<RoomPokerPlayers>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var entity in _filter)
        {
            ref var roomPokerStats = ref _roomPokerStats.Get(entity);
            ref var roomPokerPlayers = ref _roomPokerPlayers.Get(entity);

            if (roomPokerStats.MaxPlayers == roomPokerPlayers.Players.Count)
            {
                Debug.LogError($"[RoomPokerPlayerJoinSystem.OnUpdate] trying to enter a crowded room");
                continue;
            }

            ref var roomPokerPlayerJoin = ref _roomPokerPlayerJoin.Get(entity);
            var joinPlayerEntity = roomPokerPlayerJoin.Player;

            if (roomPokerPlayers.Players.ContainsKey(joinPlayerEntity))
            {
                continue;
            }
            
            ref var roomPokerId = ref _roomPokerId.Get(entity);

            byte seat = 0;

            while (roomPokerPlayers.Players.ContainsValue(seat))
            {
                seat++;
            }
            
            roomPokerPlayers.Players.Add(joinPlayerEntity, seat);
            
            ref var playerRoomPoker = ref _playerRoomPoker.Get(joinPlayerEntity, out var exist);
            ref var playerId = ref _playerId.Get(joinPlayerEntity);
            ref var playerNickname = ref _playerNickname.Get(joinPlayerEntity);

            if (exist)
            {
                playerRoomPoker.RoomIds.Add(roomPokerId.Value);
            }
            else
            {
                _playerRoomPoker.Set(joinPlayerEntity, new PlayerRoomPoker
                {
                    RoomIds = new List<int> {roomPokerId.Value},
                });
            }

            var playersNetworkModels = new List<RoomPlayerNetworkModel>();

            foreach (var pair in roomPokerPlayers.Players)
            {
                var otherPlayer = pair.Key;
                var otherSeat = pair.Value;

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
                        Seat = seat,
                    }
                });

                playersNetworkModels.Add(new RoomPlayerNetworkModel
                {
                    Id = otherPlayerId.Id,
                    Nickname = otherPlayerNickname.Value,
                    Seat = otherSeat,
                });
            }
            
            _playerRoomLocalJoinSend.Set(joinPlayerEntity, new PlayerRoomLocalJoinSend
            {
                RoomId = roomPokerId.Value,
                MaxPlayers = roomPokerStats.MaxPlayers,
                Seat = seat,
                RemotePlayers = playersNetworkModels,
            });

            _roomPokerPlayerJoin.Remove(entity);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}