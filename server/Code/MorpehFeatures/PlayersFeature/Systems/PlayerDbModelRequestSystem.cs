using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Dataframes;

namespace server.Code.MorpehFeatures.PlayersFeature.Systems;

public class PlayerDbModelRequestSystem : ISystem
{
    [Injectable] private Stash<PlayerDbModelRequest> _playerDbModelRequest;
    [Injectable] private Stash<PlayerBalance> _playerBalanceChips;

    [Injectable] private NetFrameServer _server;
    
    private Filter _filter;

    //private ThreadSafeContainer<PlayerDbModelThreadSafe> _threadSafeContainer;

    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<PlayerDbModelRequest>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var entity in _filter)
        {
            //надо создать PlayerDbModelResponseSystem и вытащить запись из таблицы с игроками,
            //а пока что временно повесим баланс какой нибудь

            ulong chips = 10000;
            ulong gold = 50;
            _playerBalanceChips.Set(entity, new PlayerBalance
            {
                Chips = chips,
                Gold = gold,
            });

            var dataframe = new CurrencyInitDataframe
            {
                Chips = chips,
                Gold = gold,
            };
            _server.Send(ref dataframe, entity);

            _playerDbModelRequest.Remove(entity);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}