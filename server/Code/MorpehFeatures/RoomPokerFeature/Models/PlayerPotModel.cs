namespace server.Code.MorpehFeatures.RoomPokerFeature.Models;

public class PlayerPotModel
{
    public string Guid;
    public long PotCommitment;
    public int HandStrength;
    public long ChipsRemaining;
    public bool IsNonTurn;

    public PlayerPotModel(string guid)
    {
        Guid = guid;
    }

    public void SetBet(long chips)
    {
        PotCommitment += chips;
    }

    public void SetNonTurn()
    {
        IsNonTurn = true;
    }

    public void SetHandStrength(int handStrength)
    {
        HandStrength = handStrength;
    }
}