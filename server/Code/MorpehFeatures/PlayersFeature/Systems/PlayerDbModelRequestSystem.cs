using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.CurrencyFeature.Dataframe;
using server.Code.MorpehFeatures.CurrencyFeature.Enums;
using server.Code.MorpehFeatures.PlayersFeature.Components;
using server.Code.MorpehFeatures.PlayersFeature.Dataframes;

namespace server.Code.MorpehFeatures.PlayersFeature.Systems;

public class PlayerDbModelRequestSystem : ISystem
{
    [Injectable] private Stash<PlayerDbModelRequest> _playerDbModelRequest;
    [Injectable] private Stash<PlayerCurrency> _playerCurrency;

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

            long chips = 10000;
            long gold = 150;

            var currencyByType = new Dictionary<CurrencyType, long>
            {
                [CurrencyType.Chips] = chips,
                [CurrencyType.Gold] = gold,
            };
            
            _playerCurrency.Set(entity, new PlayerCurrency
            {
                CurrencyByType = currencyByType,
            });

            var dataframe = new CurrencyInitDataframe
            {
                CurrencyByType = currencyByType,
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