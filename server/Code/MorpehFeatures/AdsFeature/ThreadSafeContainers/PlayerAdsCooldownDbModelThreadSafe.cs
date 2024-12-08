using Scellecs.Morpeh;
using server.Code.MorpehFeatures.AdsFeature.DbModels;

namespace server.Code.MorpehFeatures.AdsFeature.ThreadSafeContainers;

public class PlayerAdsCooldownDbModelThreadSafe
{
    public Entity Player;
    public List<DbPlayerAdsCooldownModel> AdsCooldownModels;
}