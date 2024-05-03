using Scellecs.Morpeh;
using server.Code.MorpehFeatures.PlayersFeature.DbModels;

namespace server.Code.MorpehFeatures.PlayersFeature.Components;

public struct PlayerDbEntry : IComponent
{
    public DbPlayerModel Model;
}