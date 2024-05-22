using Scellecs.Morpeh;
using server.Code.MorpehFeatures.PlayersFeature.DbModels;

namespace server.Code.MorpehFeatures.PlayersFeature.Components;

public struct PlayerInitialize : IComponent
{
    public DbPlayerModel DbPlayerModel;
}