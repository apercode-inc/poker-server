using Scellecs.Morpeh;

namespace server.Code.MorpehFeatures.AuthenticationFeature.Components;

public struct AuthenticationPlayerCreate : IComponent
{
    public string UserId;
    public string Nickname;
    public int AvatarIndex;
}