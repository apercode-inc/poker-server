using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.CurrencyFeature.Enums;
using server.Code.MorpehFeatures.CurrencyFeature.Services;
using server.Code.MorpehFeatures.PlayersFeature.Components;

namespace server.Code.MorpehFeatures.CurrencyFeature.Systems;

public class CurrencyChangeTESTSystem : ISystem
{
    private const float COOLDOWN = 5.0f;
    
    [Injectable] private CurrencyPlayerService _currencyPlayerService;

    private Filter _filter;
    private float _timer;
    
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<PlayerId>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        _timer += deltaTime;

        if (_timer < COOLDOWN)
        {
            return;
        }

        _timer = 0;
        
        foreach (var player in _filter)
        {
            _currencyPlayerService.Give(player, CurrencyType.Chips, 100);
            _currencyPlayerService.Give(player, CurrencyType.Gold, 5);
        }
    }

    public void Dispose()
    {
        _filter = null;
    }
}