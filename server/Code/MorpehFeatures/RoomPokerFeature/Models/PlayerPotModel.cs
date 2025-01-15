namespace server.Code.MorpehFeatures.RoomPokerFeature.Models;

public class PlayerPotModel
{
    public readonly string Guid;
    public readonly string Nickname;
    public long PotCommitment;  
    public int HandStrength;
    public long ChipsRemaining;
    public bool IsFold;

    public PlayerPotModel(string guid, string nickname)
    {
        Guid = guid;
        Nickname = nickname;
    }

    public void SetBet(long chips)
    {
        PotCommitment += chips;
    }

    public void SetFold()
    {
        IsFold = true;
    }

    public void SetHandStrength(int handStrength)
    {
        HandStrength = handStrength;
    }
}