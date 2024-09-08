namespace server.Code.MorpehFeatures.RoomPokerFeature.Models;

public class PlayerPotModel
{
    public string Guid;
    public long PotCommitment;
    public uint HandStrength;
    public long ChipsRemaining;
    public bool IsFolded;

    public PlayerPotModel(string guid)
    {
        Guid = guid;

        //ShowInfoPlayer();
    }

    public void SetBet(long chips)
    {
        PotCommitment += chips;
    }

    public void SetFold()
    {
        IsFolded = true;
    }

    public void SetHandStrength(uint handStrength)
    {
        HandStrength = handStrength;
    }
}